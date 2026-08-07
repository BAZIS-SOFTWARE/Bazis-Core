using System;
using System.Collections.Generic;
using System.Linq;

namespace SurfaceMesher 
{
    /// <summary>
    /// Построение (условной) триангуляции Делоне на плоскости
    /// </summary>
    public class DeloneTriangulation_m2
    {
        private double xMin;
        private double xMax;
        private double yMin;
        private double yMax;
        private double largestDimension;
        private Node2D[] nodes;
        private int lastRecentylyUsedTriangle;
        private Contour contour;
        private HashSet<Edge> fixedEdges;
        private int[,] vertices;
        private int[,] adjacency;
        private int trianglesNumber;

        private const double epsilon = 1e-4;
        private const double defaultQuadQualityThreshold = 0.5;

        /// <summary>
        /// Выполнить операцию улучшения сетки с заданным числом итераций.
        /// Если не задан контур, произойдет исключение
        /// </summary>
        public IEnumerable<Node2D> RefineMesh(int iterations = 1)
        {
            if (contour == null)
                throw new Exception("Ошибка триангуляции. Контур не задан");
            var output = nodes.ToList();
            for(var iteration = 0; iteration < iterations; iteration++)
            {
                for (var k = 0; k < output.Count - 3; k++)
                {
                    if (contour.GetContourId(output[k]) != -1
                        || fixedEdges.Any(e => e.A.Index == output[k].Index || e.B.Index == output[k].Index))
                    {
                        continue;
                    }
                    double x = 0, y = 0;
                    var n = 0;
                    for (var i = 0; i < trianglesNumber; i++)
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            if (vertices[i, j] == k)
                            {
                                x += output[vertices[i, (j - 1 + 3) % 3]].X + output[vertices[i, (j + 1) % 3]].X;
                                y += output[vertices[i, (j - 1 + 3) % 3]].Y + output[vertices[i, (j + 1) % 3]].Y;
                                n += 2;
                                break;
                            }
                        }
                    }
                    output[k] = new Node2D(output[k].Index, x / n, y / n);
                }
            }
            return output.Take(output.Count - 3);
        }

        /// <summary>
        /// Построить триангуляцию с ограничениями
        /// </summary>
        /// <param name="sourceNodes">Все узлы, включая граничные, на которых будет построена триангуляция</param>
        /// <param name="constraints">Ребра-ограничения, которые должны быть в полученной триангуляции</param>
        /// <param name="boundaries">Список контуров, описывающих границу триангуляции</param>
        public void BuildTriangulation(List<Node2D> sourceNodes, IEnumerable<Edge> constraints, List<List<Node2D>> boundaries)
        {
            BuildTriangulation(sourceNodes);
            contour = new Contour(boundaries);
            var boundaryEdges = GetBoundaryEdges(boundaries);
            fixedEdges = constraints.Union(boundaryEdges).ToHashSet();
            foreach (var constrainedEdge in fixedEdges)
            {
                var initialTriangle = GetInitialTriangle(constrainedEdge);
                GetSwappedEdges(initialTriangle, constrainedEdge);
            }
        }

        /// <summary>
        /// Получить множество треугольников из триангуляции
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Triangle2D> GetTriangles()
        {
            for (var i = 0; i < trianglesNumber; i++)
            {
                if (vertices[i, 0] >= nodes.Length - 3 || vertices[i, 1] >= nodes.Length - 3 || vertices[i, 2] >= nodes.Length - 3)
                    continue;
                var triangle = new Triangle2D(i, nodes[vertices[i, 0]], nodes[vertices[i, 1]], nodes[vertices[i, 2]]);
                if (contour == null || IsTriangleInsideContour(i))
                {
                    yield return triangle;
                }
            }
        }

        /// <summary>
        /// Получить смешанную квадратно-треугольную сетку. Для каждой пары соседних треугольников
        /// проверяется, можно ли объединить их в выпуклый четырёхугольник приемлемого качества
        /// (углы близки к 90 градусам), и жадно, начиная с наиболее качественных пар, выполняется
        /// объединение. Треугольники, для которых не нашлось подходящей пары, возвращаются как есть.
        /// Ограниченные рёбра (<see cref="fixedEdges"/>) никогда не удаляются, поэтому такие пары
        /// не объединяются. Возвращает элементы типа <see cref="Quad2D"/> и <see cref="Triangle2D"/>
        /// </summary>
        /// <param name="qualityThreshold">
        /// Минимально допустимое качество четырёхугольника от 0 (вырожденный) до 1 (все углы 90°)
        /// </param>
        public IEnumerable<Element> GetQuadsTriangles(double qualityThreshold = defaultQuadQualityThreshold)
        {
            var merged = new bool[trianglesNumber];
            var candidates = new List<QuadCandidate>();
            for (var i = 0; i < trianglesNumber; i++)
            {
                if (!IsValidTriangle(i))
                    continue;
                for (var e = 0; e < 3; e++)
                {
                    var j = adjacency[i, e];
                    if (j <= i || !IsValidTriangle(j))
                        continue;
                    var edgeA = vertices[i, e];
                    var edgeB = vertices[i, (e + 1) % 3];
                    if (IsEdgeConstrained((edgeA, edgeB)))
                        continue;
                    var a = GetOppositeVertexInTriangle(i, (edgeA, edgeB));
                    var c = GetOppositeVertexInTriangle(j, (edgeA, edgeB));
                    if (!IsQuadConvex(a, edgeA, c, edgeB))
                        continue;
                    var quality = GetQuadQuality(a, edgeA, c, edgeB);
                    if (quality < qualityThreshold)
                        continue;
                    candidates.Add(new QuadCandidate(quality, i, j, a, edgeA, c, edgeB));
                }
            }
            foreach (var candidate in candidates.OrderByDescending(c => c.Quality))
            {
                if (merged[candidate.TriangleOne] || merged[candidate.TriangleTwo])
                    continue;
                merged[candidate.TriangleOne] = true;
                merged[candidate.TriangleTwo] = true;
                yield return new Quad2D(candidate.TriangleOne, nodes[candidate.A], nodes[candidate.B], nodes[candidate.C], nodes[candidate.D]);
            }
            for (var i = 0; i < trianglesNumber; i++)
            {
                if (merged[i] || !IsValidTriangle(i))
                    continue;
                yield return new Triangle2D(i, nodes[vertices[i, 0]], nodes[vertices[i, 1]], nodes[vertices[i, 2]]);
            }
        }

        private bool IsValidTriangle(int triangleIndex)
        {
            if (triangleIndex == -1)
                return false;
            if (vertices[triangleIndex, 0] >= nodes.Length - 3
                || vertices[triangleIndex, 1] >= nodes.Length - 3
                || vertices[triangleIndex, 2] >= nodes.Length - 3)
            {
                return false;
            }
            return contour == null || IsTriangleInsideContour(triangleIndex);
        }

        // a, b, c, d - вершины четырёхугольника в порядке обхода (как в IsQuadConvex)
        private double GetQuadQuality(int a, int b, int c, int d)
        {
            var angleA = GetAngleDeg(nodes[a], nodes[d], nodes[b]);
            var angleB = GetAngleDeg(nodes[b], nodes[a], nodes[c]);
            var angleC = GetAngleDeg(nodes[c], nodes[b], nodes[d]);
            var angleD = GetAngleDeg(nodes[d], nodes[c], nodes[a]);
            var maxDeviation = new[] { angleA, angleB, angleC, angleD }.Max(angle => Math.Abs(angle - 90));
            return Math.Max(0, 1 - maxDeviation / 90);
        }

        // Угол в вершине vertex между лучами к neighborOne и neighborTwo, в градусах
        private double GetAngleDeg(Node2D vertex, Node2D neighborOne, Node2D neighborTwo)
        {
            var v1 = neighborOne.Position.Sub(vertex.Position);
            var v2 = neighborTwo.Position.Sub(vertex.Position);
            var dot = v1.X * v2.X + v1.Y * v2.Y;
            var cross = Vector.CrossProduct(v1, v2);
            return Math.Abs(Math.Atan2(cross, dot)) * 180 / Math.PI;
        }

        /// <summary>
        /// Проверяет принадлежность узла триангуляции.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>true если внутри, иначе false</returns>
        public bool IsNodeInsideContour(Node2D node)
        {
            var currentTriangle = trianglesNumber - 1;
            var (nextTriangle, _) = MoveTowardPoint(node, currentTriangle);
            while (nextTriangle != currentTriangle)
            {
                currentTriangle = nextTriangle;
                if (nextTriangle == -1)
                    return false;
                (nextTriangle, _) = MoveTowardPoint(node, currentTriangle);
            }

            for (var i = 0; i < 3; i++)
            {
                if (IsNodeOnEdge((nodes[vertices[nextTriangle, i]], nodes[vertices[nextTriangle, (i + 1) % 3]]), node))
                {
                    var adjacentTriangle = adjacency[nextTriangle, i];
                    if (IsTriangleInsideContour(adjacentTriangle))
                        return true;
                }
            }
            return IsTriangleInsideContour(nextTriangle);
        }

        private void BuildTriangulation(List<Node2D> sourceNodes)
        {
            lastRecentylyUsedTriangle = 0;
            xMax = sourceNodes.Select(p => p.Position.X).Max();
            yMax = sourceNodes.Select(p => p.Position.Y).Max();
            xMin = sourceNodes.Select(p => p.Position.X).Min();
            yMin = sourceNodes.Select(p => p.Position.Y).Min();
            largestDimension = Math.Max(Math.Abs(xMax - xMin), Math.Abs(yMax - yMin));
            var maxNodeIndex = sourceNodes.Select(p => p.Index).Max();
            var n = sourceNodes.Count;
            nodes = new Node2D[n + 3];
            vertices = new int[2 * n + 1, 3];
            adjacency = new int[2 * n + 1, 3];
            AddSuperTriangle(maxNodeIndex);
            trianglesNumber = 1;
            var i = 0;
            foreach (var index in GetSortedNodesIndeces(sourceNodes))
            {
                nodes[i] = sourceNodes[index];
                InsertPointIntoTriangulation(i);
                i++;
            }
        }

        private bool IsNodeOnEdge((Node2D, Node2D) edge, Node2D node)
        {
            var vectorOne = edge.Item2.Position.Sub(edge.Item1.Position);
            var vectorTwo = node.Position.Sub(edge.Item1.Position);
            var crossProduct = Vector.CrossProduct(vectorOne, vectorTwo); //edit
            if (Math.Abs(crossProduct) < epsilon)
                return true;
            return false;
        }

        private IEnumerable<Edge> GetBoundaryEdges(List<List<Node2D>> boundaries)
        {
            return boundaries.Select(boundary => boundary.Bigrams(true).Select(pair => new Edge(pair.Item1, pair.Item2)))
                             .SelectMany(x => x);
        }

        private bool IsTriangleInsideContour(int triangleIndex)
        {
            if(contour == null)
                throw new Exception("Ошибка триангуляции. Контур не задан");
            if (triangleIndex == -1)
                return false;
            if (vertices[triangleIndex, 0] >= nodes.Length - 3
                || vertices[triangleIndex, 1] >= nodes.Length - 3
                || vertices[triangleIndex, 2] >= nodes.Length - 3)
            {
                return false;
            }
                if (GetContourId(vertices[triangleIndex, 0]) == GetContourId(vertices[triangleIndex, 1])
                && GetContourId(vertices[triangleIndex, 1]) == GetContourId(vertices[triangleIndex, 2])
                && GetContourId(vertices[triangleIndex, 2]) == GetContourId(vertices[triangleIndex, 0])
                && GetContourId(vertices[triangleIndex, 0]) != -1)
            {
                var startNode = nodes[vertices[triangleIndex, 0]];
                var currentNode = contour.GetElementAfter(startNode);
                while(currentNode.Index != startNode.Index)
                {
                    if (currentNode.Index == nodes[vertices[triangleIndex, 1]].Index)
                        return true;
                    else if (currentNode.Index == nodes[vertices[triangleIndex, 2]].Index)
                        return false;
                    currentNode = contour.GetElementAfter(currentNode);
                }
            }
            return true;
        }

        private int GetContourId(int nodeIndex)
        {
            return contour.GetContourId(nodes[nodeIndex]);
        }

        //Returns adjacent triangles and new (swapped) edge
        private List<((int, int), (int, int))> GetSwappedEdges(int initialTriangle, Edge constrainedEdge)
        {
            var newEdges = new List<((int, int), (int, int))>();
            var queue = new Queue<((int, int), (int, int), int)>();
            GetIntersectedEdges(initialTriangle, constrainedEdge).ToList().ForEach(x => queue.Enqueue(x));
            while (queue.Count > 0)
            {
                var (adjacentTriangles, edge, invariantTriangle) = queue.Dequeue();
                var oppositeVertexOne = GetOppositeVertexInTriangle(adjacentTriangles.Item1, edge);
                var oppositeVertexTwo = GetOppositeVertexInTriangle(adjacentTriangles.Item2, edge);
                if (IsQuadConvex(oppositeVertexOne, edge.Item1, oppositeVertexTwo, edge.Item2))
                {
                    SwapEdge(adjacentTriangles.Item1, adjacentTriangles.Item2, edge, invariantTriangle, false);
                    var newEdge = new Edge(nodes[oppositeVertexOne], nodes[oppositeVertexTwo]);
                    if (newEdge.IsIntersectWith(constrainedEdge))
                        queue.Enqueue((adjacentTriangles, (oppositeVertexOne, oppositeVertexTwo), invariantTriangle));
                    else
                        newEdges.Add((adjacentTriangles, (oppositeVertexOne, oppositeVertexTwo)));
                }
                else
                    queue.Enqueue((adjacentTriangles, edge, invariantTriangle));
            }
            return newEdges;
        }

        // (triangleOne, triangleTwo); adjacentEdge, nextTriangleInLocalizationPath
        private IEnumerable<((int, int), (int, int), int)> GetIntersectedEdges(int initialTriangle, Edge constrainedEdge)
        {
            var localizationPath = GetLocalizationPath(constrainedEdge.B, initialTriangle);
            for (var i = 0; i < localizationPath.Count - 1; i++)
            {
                var triangleOne = localizationPath[i].Item1;
                var triangleTwo = localizationPath[i + 1].Item1;
                var edgeBegin = localizationPath[i].Item2.Item1;
                var edgeEnd = localizationPath[i].Item2.Item2;
                var edge = new Edge(nodes[edgeBegin], nodes[edgeEnd]);
                var invariantTriangle = -1;
                if (localizationPath.Count > 2)
                {
                    invariantTriangle = (i == localizationPath.Count - 2)
                        ? localizationPath[i - 1].Item1
                        : localizationPath[i + 2].Item1;
                }
                if (edge.IsIntersectWith(constrainedEdge))
                    yield return ((triangleOne, triangleTwo), (edgeBegin, edgeEnd), invariantTriangle);
            }
        }

        private int GetOppositeVertexInTriangle(int triangleIndex, (int, int) edge)
        {
            for (var i = 0; i < 3; i++)
            {
                if (vertices[triangleIndex, i] != edge.Item1 && vertices[triangleIndex, i] != edge.Item2)
                {
                    return vertices[triangleIndex, i];
                }
            }
            throw new Exception($"Ошибка триангуляции: невозможо определить противоположную вершину по ребру ({edge.Item1}, {edge.Item2}) в треугольнике {triangleIndex}");
        }

        private bool IsQuadConvex(int a, int b, int c, int d)
        {
            var crossProdOne = Vector.CrossProduct(nodes[b].Position.Sub(nodes[a].Position), nodes[c].Position.Sub(nodes[b].Position)); //edit
            var crossProdTwo = Vector.CrossProduct(nodes[c].Position.Sub(nodes[b].Position), nodes[d].Position.Sub(nodes[c].Position));
            var crossProdThree = Vector.CrossProduct(nodes[d].Position.Sub(nodes[c].Position), nodes[a].Position.Sub(nodes[d].Position));
            var crossProdFour = Vector.CrossProduct(nodes[a].Position.Sub(nodes[d].Position), nodes[b].Position.Sub(nodes[a].Position));
            if ((crossProdOne > 0 && crossProdTwo > 0 && crossProdThree > 0 && crossProdFour > 0)
                || (crossProdOne < 0 && crossProdTwo < 0 && crossProdThree < 0 && crossProdFour < 0))
                return true;
            return false;
        }

        private void InsertPointIntoTriangulation(int nodeIndex)
        {
            var j = GetLocalizationPath(nodes[nodeIndex], trianglesNumber - 1).Last().Item1;
            trianglesNumber += 2;
            AddPointIntoTriangle(nodeIndex, j);
            if (adjacency[j, 1] != -1 && !IsSatisfyDeloneCondition(adjacency[j, 1], j, (vertices[j, 1], vertices[j, 2])))
                SwapEdge(adjacency[j, 1], j, (vertices[j, 1], vertices[j, 2]));
            if (adjacency[trianglesNumber - 2, 1] != -1 && !IsSatisfyDeloneCondition(adjacency[trianglesNumber - 2, 1], trianglesNumber - 2, (vertices[trianglesNumber - 2, 1], vertices[trianglesNumber - 2, 2])))
                SwapEdge(adjacency[trianglesNumber - 2, 1], trianglesNumber - 2, (vertices[trianglesNumber - 2, 1], vertices[trianglesNumber - 2, 2]));
            if (adjacency[trianglesNumber - 1, 1] != -1 && !IsSatisfyDeloneCondition(adjacency[trianglesNumber - 1, 1], trianglesNumber - 1, (vertices[trianglesNumber - 1, 1], vertices[trianglesNumber - 1, 2])))
                SwapEdge(adjacency[trianglesNumber - 1, 1], trianglesNumber - 1, (vertices[trianglesNumber - 1, 1], vertices[trianglesNumber - 1, 2]));
        }

        private int GetInitialTriangle(Edge edge)
        {
            var startNode = edge.A;
            var localizationPath = GetLocalizationPath(startNode, lastRecentylyUsedTriangle);
            var currentTriangle = localizationPath[localizationPath.Count - 1].Item1;
            lastRecentylyUsedTriangle = currentTriangle;
            var iteration = 0;
            while (iteration < 1000)
            {
                var AB = new Edge(nodes[vertices[currentTriangle, 0]], nodes[vertices[currentTriangle, 1]]);
                var BC = new Edge(nodes[vertices[currentTriangle, 1]], nodes[vertices[currentTriangle, 2]]);
                var CA = new Edge(nodes[vertices[currentTriangle, 2]], nodes[vertices[currentTriangle, 0]]);
                if (AB.Equals(edge) || BC.Equals(edge) || CA.Equals(edge)) // edge already exists
                {
                    return currentTriangle;
                }
                if (AB.IsIntersectWith(edge) || BC.IsIntersectWith(edge) || CA.IsIntersectWith(edge))
                {
                    return currentTriangle;
                }
                else
                {
                    MakePointFirstInTriangle(currentTriangle, startNode);
                    if (adjacency[currentTriangle, 0] != -1)
                    {
                        if (currentTriangle == adjacency[currentTriangle, 0])
                            throw new Exception("Ошибка построения триангуляции. Неверно построена матрица смежности треугольников");
                        currentTriangle = adjacency[currentTriangle, 0];
                    }
                    else
                    {
                        if (currentTriangle == adjacency[currentTriangle, 0])
                            throw new Exception("Ошибка построения триангуляции. Неверно построена матрица смежности треугольников");
                        currentTriangle = adjacency[currentTriangle, 2];
                    }
                }
                iteration++;
            }
            throw new Exception("Алгоритм поиска начального треугольника при вставке рёбер зациклился. Убедитесь, что в коллекции нет дублирующихся узлов");
        }

        private bool IsEdgeConstrained((int, int) edge)
        {
            if (fixedEdges == null)
                return false;
            var e = new Edge(nodes[edge.Item1], nodes[edge.Item2]);
            return fixedEdges.Contains(e);
        }

        private void SwapEdge(int L, int R, (int, int) edge, int invariantTriangle = -1, bool checkDeloneConditionAfterSwapping = true)
        {
            if (IsEdgeConstrained(edge))
                return;
            var l = GetOppositeVertexInTriangle(L, edge);
            var k = GetOppositeVertexInTriangle(R, edge);
            MakePointFirstInTriangle(L, nodes[l]);
            MakePointFirstInTriangle(R, nodes[k]);
            var A = adjacency[R, 2];
            var B = adjacency[R, 0];
            var C = adjacency[L, 2];
            var D = adjacency[L, 0];
            if (invariantTriangle == D || invariantTriangle == B || invariantTriangle == -1)
            {
                adjacency[L, 1] = A;
                adjacency[L, 2] = R;
                adjacency[R, 1] = C;
                adjacency[R, 2] = L;
                if (A != -1)
                {
                    for(var i = 0; i < 3; i++)
                    {
                        if (adjacency[A, i] == R)
                        {
                            adjacency[A, i] = L;
                            break;
                        }
                    }
                }
                if (C != -1)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        if (adjacency[C, i] == L)
                        {
                            adjacency[C, i] = R;
                            break;
                        }
                    }
                }
                vertices[L, 2] = k;
                vertices[R, 2] = l;
            }
            else if (invariantTriangle == A || invariantTriangle == C)
            {
                adjacency[L, 0] = R;
                adjacency[L, 1] = B;
                adjacency[R, 0] = L;
                adjacency[R, 1] = D;
                if (D != -1)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        if (adjacency[D, i] == L)
                        {
                            adjacency[D, i] = R;
                            break;
                        }
                    }
                }
                if (B != -1)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        if (adjacency[B, i] == R)
                        {
                            adjacency[B, i] = L;
                            break;
                        }
                    }
                }
                vertices[L, 1] = k;
                vertices[R, 1] = l;
            }
            else
                throw new Exception($"Ошибка триангуляции. Невозможно задать инвариантный треугольник с индексом {invariantTriangle}");

            if(checkDeloneConditionAfterSwapping)
            {
                if (adjacency[L, 1] != -1 && !IsSatisfyDeloneCondition(adjacency[L, 1], L, (vertices[L, 1], vertices[L, 2])))
                    SwapEdge(L, adjacency[L, 1], (vertices[L, 1], vertices[L, 2]));
                if (adjacency[L, 0] != -1 && !IsSatisfyDeloneCondition(adjacency[L, 0], L, (vertices[L, 0], vertices[L, 1])))
                    SwapEdge(L, adjacency[L, 0], (vertices[L, 0], vertices[L, 1]));
                if (adjacency[R, 1] != -1 && !IsSatisfyDeloneCondition(adjacency[R, 1], R, (vertices[R, 1], vertices[R, 2])))
                    SwapEdge(R, adjacency[R, 1], (vertices[R, 1], vertices[R, 2]));
                if (adjacency[R, 0] != -1 && !IsSatisfyDeloneCondition(adjacency[R, 0], R, (vertices[R, 0], vertices[R, 1])))
                    SwapEdge(R, adjacency[R, 0], (vertices[R, 0], vertices[R, 1]));
            }
        }

        private void MakePointFirstInTriangle(int triangleIndex, Node2D point)
        {
            if (nodes[vertices[triangleIndex, 0]].Index != point.Index 
                && nodes[vertices[triangleIndex, 1]].Index != point.Index
                && nodes[vertices[triangleIndex, 2]].Index != point.Index)
            {
                throw new ArgumentException($"Ошибка триангуляции: точка {point.Index} не найдена в треугольнике с индексом {triangleIndex}");
            }
            while (nodes[vertices[triangleIndex, 0]].Index != point.Index)
            {
                var temp1 = vertices[triangleIndex, 0];
                var temp2 = adjacency[triangleIndex, 0];
                vertices[triangleIndex, 0] = vertices[triangleIndex, 1];
                vertices[triangleIndex, 1] = vertices[triangleIndex, 2];
                vertices[triangleIndex, 2] = temp1;
                adjacency[triangleIndex, 0] = adjacency[triangleIndex, 1];
                adjacency[triangleIndex, 1] = adjacency[triangleIndex, 2];
                adjacency[triangleIndex, 2] = temp2;
            }
        }

        //Returns a traverse path consisting of triangleIndex and edge 
        private List<(int, (int, int))> GetLocalizationPath(Node2D node, int currentTriangle)
        {
            var visited = new List<(int, (int, int))>();
            var (nextTriangle, edge) = MoveTowardPoint(node, currentTriangle);
            while (nextTriangle != currentTriangle)
            {
                visited.Add((currentTriangle, edge));
                currentTriangle = nextTriangle;
                (nextTriangle, edge) = MoveTowardPoint(node, currentTriangle);
                if (visited.Count > 1000)
                    throw new Exception("Ошибка триангуляции. Алгоритм локализации точки зациклился");
            }
            visited.Add((currentTriangle, (-1, -1)));
            return visited;
        }

        //Returns next triangle and edge which adjacent with currentTriangle
        private (int, (int, int)) MoveTowardPoint(Node2D point, int currentTriangle)
        {
            if (currentTriangle == -1)
                throw new ArgumentException("Ошибка триангуляции. Невозможно определить треугольник с индексом -1");
            var p = point.Position;
            var A = nodes[vertices[currentTriangle, 0]].Position;
            var B = nodes[vertices[currentTriangle, 1]].Position;
            var C = nodes[vertices[currentTriangle, 2]].Position;
            var Sap = (p.X - A.X) * (B.Y - A.Y) - (p.Y - A.Y) * (B.X - A.X);
            var Sbp = (p.X - B.X) * (C.Y - B.Y) - (p.Y - B.Y) * (C.X - B.X);
            var Scp = (p.X - C.X) * (A.Y - C.Y) - (p.Y - C.Y) * (A.X - C.X);
            if ((Sap <= 0 && Sbp <= 0 && Scp <= 0)
                || (Sap < epsilon && Sbp <= 0 && Scp <= 0)
                || (Sbp < epsilon && Sap <= 0 && Scp <= 0)
                || (Scp < epsilon && Sap <= 0 && Sbp <= 0))
            {
                return (currentTriangle, (-1, -1));
            }
            if (Sap > 0 && Sap >= Sbp && Sap >= Scp)
            { 
                return (adjacency[currentTriangle, 0], (vertices[currentTriangle, 0], vertices[currentTriangle, 1]));
            }
            if (Sbp > 0 && Sbp >= Sap && Sbp >= Scp)
            {
                return (adjacency[currentTriangle, 1], (vertices[currentTriangle, 1], vertices[currentTriangle, 2]));
            }
            if (Scp > 0 && Scp >= Sap && Scp >= Sbp)
            {
                return (adjacency[currentTriangle, 2], (vertices[currentTriangle, 2], vertices[currentTriangle, 0]));
            }
            throw new Exception("Ошибка при построении триангуляции. Не удалось локализовать текущую точку");
        }

        private bool IsSatisfyDeloneCondition(int triangleAdjacent, int triangleCircled, (int, int) edgeOrderedByCircledTriangle)
        {
            var p = nodes[GetOppositeVertexInTriangle(triangleAdjacent, edgeOrderedByCircledTriangle)].Position;
            var v3 = nodes[GetOppositeVertexInTriangle(triangleCircled, edgeOrderedByCircledTriangle)].Position;
            var v1 = nodes[edgeOrderedByCircledTriangle.Item1].Position;
            var v2 = nodes[edgeOrderedByCircledTriangle.Item2].Position;
            var cosA = (v1.X - v3.X) * (v2.X - v3.X) + (v1.Y - v3.Y) * (v2.Y - v3.Y); // dot product
            var cosB = (v2.X - p.X) * (v1.X - p.X) + (v2.Y - p.Y) * (v1.Y - p.Y); // -same-
            var sinA = Math.Abs((v1.X - v3.X) * (v2.Y - v3.Y) - (v2.X - v3.X) * (v1.Y - v3.Y)); // cross product
            var sinB = Math.Abs((v2.X - p.X) * (v1.Y - p.Y) - (v1.X - p.X) * (v2.Y - p.Y)); // -same-
            var sinAB = sinA * cosB + cosA * sinB;
            if ((cosA < 0 && cosB < 0) || sinAB < 0)
                return false;
            return true;
        }

        private void AddPointIntoTriangle(int point, int triangle)
        {
            vertices[trianglesNumber - 2, 0] = point;
            vertices[trianglesNumber - 2, 1] = vertices[triangle, 1];
            vertices[trianglesNumber - 2, 2] = vertices[triangle, 2];
            vertices[trianglesNumber - 1, 0] = point;
            vertices[trianglesNumber - 1, 1] = vertices[triangle, 2];
            vertices[trianglesNumber - 1, 2] = vertices[triangle, 0];
            var adj2 = adjacency[triangle, 1];
            var adj3 = adjacency[triangle, 2];
            if (adj2 >= 0)
            {
                for (var m = 0; m < 3; m++)
                {
                    if (adjacency[adj2, m] == triangle)
                    {
                        adjacency[adj2, m] = trianglesNumber - 2;
                        break;
                    }
                }
            }
            if (adj3 >= 0)
            {
                for (var m = 0; m < 3; m++)
                {
                    if (adjacency[adj3, m] == triangle)
                    {
                        adjacency[adj3, m] = trianglesNumber - 1;
                        break;
                    }
                }
            }
            adjacency[trianglesNumber - 2, 0] = triangle;
            adjacency[trianglesNumber - 2, 1] = adj2;
            adjacency[trianglesNumber - 2, 2] = trianglesNumber - 1;
            adjacency[trianglesNumber - 1, 0] = trianglesNumber - 2;
            adjacency[trianglesNumber - 1, 1] = adj3;
            adjacency[trianglesNumber - 1, 2] = triangle;
            vertices[triangle, 2] = vertices[triangle, 1];
            vertices[triangle, 1] = vertices[triangle, 0];
            vertices[triangle, 0] = point;
            adjacency[triangle, 2] = trianglesNumber - 2;
            adjacency[triangle, 1] = adjacency[triangle, 0];
            adjacency[triangle, 0] = trianglesNumber - 1;
        }

        private void AddSuperTriangle(int maxNodeIndex)
        {
            nodes[nodes.Length - 3] = new Node2D(maxNodeIndex + 1, xMin - 3 * largestDimension, yMin - largestDimension);
            nodes[nodes.Length - 2] = new Node2D(maxNodeIndex + 2, xMin + 3 * largestDimension, yMin - largestDimension);
            nodes[nodes.Length - 1] = new Node2D(maxNodeIndex + 3, xMin, yMin +  3 * largestDimension);
            vertices[0, 0] = nodes.Length - 3;
            vertices[0, 1] = nodes.Length - 2;
            vertices[0, 2] = nodes.Length - 1;
            adjacency[0, 0] = -1;
            adjacency[0, 1] = -1;
            adjacency[0, 2] = -1;
        }

        private IEnumerable<int> GetSortedNodesIndeces(List<Node2D> points)
        {
            var mappedNodes = points.Select(n => ((n.Position.X - xMin) / largestDimension, (n.Position.Y - yMin) / largestDimension)).ToList();
            var xMaxScaled = mappedNodes.Select(e => e.Item1).Max();
            var yMaxScaled = mappedNodes.Select(e => e.Item2).Max();

            var rows = (int)Math.Round(Math.Pow(points.Count, 0.25));
            var bins = new Dictionary<int, int>();
            for(var c = 0; c < points.Count; c++)
            {
                var i = (int)(0.999 * mappedNodes[c].Item2 / yMaxScaled * rows); // row
                var j = (int)(0.999 * mappedNodes[c].Item1 / xMaxScaled * rows); // column
                var binIndex = i % 2 == 1
                    ? (i + 1) * rows - j
                    : i * rows + j + 1;
                bins.Add(c, binIndex);
            }
            return bins.OrderBy(p => p.Value).Select(p => p.Key);
        }
    }

    // Кандидат на объединение пары соседних треугольников в четырёхугольник:
    // Quality - качество получившегося квада, TriangleOne/TriangleTwo - исходные треугольники,
    // A, B, C, D - индексы узлов четырёхугольника в порядке обхода
    internal readonly struct QuadCandidate
    {
        public double Quality { get; }
        public int TriangleOne { get; }
        public int TriangleTwo { get; }
        public int A { get; }
        public int B { get; }
        public int C { get; }
        public int D { get; }

        public QuadCandidate(double quality, int triangleOne, int triangleTwo, int a, int b, int c, int d)
        {
            Quality = quality;
            TriangleOne = triangleOne;
            TriangleTwo = triangleTwo;
            A = a;
            B = b;
            C = c;
            D = d;
        }
    }
}