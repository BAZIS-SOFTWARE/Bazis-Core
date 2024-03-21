using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;

namespace ModelController.MeshObjsCreator.Delone2D
{
    // S. W. Sloan. A fast algorithm for constructing Delaunay triangulations in the plane
    public class DeloneTriangulation2D
    {
        private List<DeloneNode2D> points;
        private List<DeloneNode2D> sortedPoints;
        private int[,] vertices;
        private int[,] adjacency;
        private int triangleNumber;
        private float largestDimension;

        public IEnumerable<DeloneNode2D[]> GetTriangles(List<DeloneNode2D> source)
        {
            vertices = new int[2 * source.Count + 6, 3];
            adjacency = new int[2 * source.Count + 6, 3];
            sortedPoints = SortPoints(source);
            points = MapPointsToUnitSquare(sortedPoints).ToList();
            triangleNumber = 1;
            BuildTriangulation();
            for (var i = 0; i < triangleNumber; i++)
            {
                var isRealTriangle = true;
                for (var j = 0; j < 3; j++)
                {
                    if (vertices[i, j] >= points.Count - 3)
                    {
                        isRealTriangle = false;
                        break;
                    }
                }
                if (isRealTriangle)
                    yield return new[] { sortedPoints[vertices[i, 0]], sortedPoints[vertices[i, 1]], sortedPoints[vertices[i, 2]] };
            }
        }

        private void BuildTriangulation()
        {
            AddSuperTriangle();
            for (var i = 0; i < points.Count - 3; i++)
            {
                var j = triangleNumber - 1;
                j = LocalizePoint(i, j);
                //Debug($"Point {i} localized in triangle {vertices[j, 0]}-{vertices[j, 1]}-{vertices[j, 2]}");
                triangleNumber += 2;
                AddPointIntoTriangle(i, j);
                var stack = new Stack<int>();
                if (adjacency[j, 1] >= 0)
                    stack.Push(j);
                if (adjacency[triangleNumber - 2, 1] >= 0)
                    stack.Push(triangleNumber - 2);
                if (adjacency[triangleNumber - 1, 1] >= 0)
                    stack.Push(triangleNumber - 1);
                while (stack.Count > 0)
                {
                    var L = stack.Pop();
                    var R = adjacency[L, 1];
                    //Debug($"Triangle {vertices[R, 0]}-{vertices[R, 1]}-{vertices[R, 2]} opposite {vertices[L, 0]}-{vertices[L, 1]}-{vertices[L, 2]}");
                    var v1 = points[vertices[L, 2]].Position;
                    var v2 = points[vertices[L, 1]].Position;
                    var oppositeVertex = -1;
                    var oppositeVertexIndex = -1;
                    for (var k = 0; k < 3; k++)
                    {
                        if (vertices[R, k] != vertices[L, 1] && vertices[R, k] != vertices[L, 2])
                        {
                            oppositeVertex = vertices[R, k];
                            oppositeVertexIndex = k;
                            break;
                        }
                    }
                    var v3 = points[oppositeVertex].Position;
                    var p = points[i].Position;
                    if (!IsSatisfyDeloneCondition(p, v1, v2, v3))
                    {
                        SwapEdge(L, R, i, oppositeVertex, oppositeVertexIndex);
                        if (adjacency[L, 1] >= 0)
                            stack.Push(L);
                        if (adjacency[R, 1] >= 0)
                            stack.Push(R);
                    }
                }
            }
        }

        private void SwapEdge(int L, int R, int i, int oppositeVertex, int oppositeVetexIndex)
        {
            //Debug($"Swap edge {i}-{oppositeVertex}");
            var C = adjacency[L, 2];
            var A = adjacency[R, (oppositeVetexIndex + 2) % 3];
            if (A >= 0) // A meets L
            {
                for (var m = 0; m < 3; m++)
                {
                    if (adjacency[A, m] == R)
                    {
                        adjacency[A, m] = L;
                        break;
                    }
                }
            }
            if (C >= 0) // C meets R
            {
                for (var m = 0; m < 3; m++)
                {
                    if (adjacency[C, m] == L)
                    {
                        adjacency[C, m] = R;
                        break;
                    }
                }
            }
            for (var m = 0; m < 3; m++) // make P first point of R
            {
                if (vertices[R, m] == oppositeVertex)
                {
                    vertices[R, (m + 2) % 3] = i;
                    break;
                }
            }
            for (var m = 0; m < 3; m++) // R meets C
            {
                if (adjacency[R, m] == L)
                {
                    adjacency[R, m] = C;
                    break;
                }
            }
            for (var m = 0; m < 3; m++) // R meets L
            {
                if (adjacency[R, m] == A)
                {
                    adjacency[R, m] = L;
                    break;
                }
            }
            for (var m = 0; m < 3; m++) // make i point first in R
            {
                if (vertices[R, 0] != i)
                {
                    var temp1 = vertices[R, 0];
                    var temp2 = adjacency[R, 0];
                    vertices[R, 0] = vertices[R, 1];
                    vertices[R, 1] = vertices[R, 2];
                    vertices[R, 2] = temp1;
                    adjacency[R, 0] = adjacency[R, 1];
                    adjacency[R, 1] = adjacency[R, 2];
                    adjacency[R, 2] = temp2;
                }
            }
            vertices[L, 2] = oppositeVertex;
            for (var m = 0; m < 3; m++) // L meets R
            {
                if (adjacency[L, m] == C)
                {
                    adjacency[L, m] = R;
                    break;
                }
            }
            for (var m = 0; m < 3; m++) // L meets A
            {
                if (adjacency[L, m] == R)
                {
                    adjacency[L, m] = A;
                    break;
                }
            }
            //Debug($"New triangle: {vertices[L, 0]}-{vertices[L, 1]}-{vertices[L, 2]}");
            //Debug($"New triangle: {vertices[R, 0]}-{vertices[R, 1]}-{vertices[R, 2]}");
        }

        private int LocalizePoint(int point, int currentTriangle)
        {
            var used = new HashSet<int>();
            var nextTriangle = MoveTowardPoint(point, currentTriangle);
            while (nextTriangle != currentTriangle)
            {
                currentTriangle = nextTriangle;
                nextTriangle = MoveTowardPoint(point, currentTriangle);
                if (used.Contains(nextTriangle))
                    throw new Exception("Ошибка при построении триангуляции. Алгоритм локализации точки зациклился.");
                used.Add(currentTriangle);
            }
            return currentTriangle;
        }

        private int MoveTowardPoint(int point, int currentTriangle)
        {
            var p = points[point].Position;
            var A = points[vertices[currentTriangle, 0]].Position;
            var B = points[vertices[currentTriangle, 1]].Position;
            var C = points[vertices[currentTriangle, 2]].Position;
            var Sap = (p._x - A._x) * (B._y - A._y) - (p._y - A._y) * (B._x - A._x);
            var Sbp = (p._x - B._x) * (C._y - B._y) - (p._y - B._y) * (C._x - B._x);
            var Scp = (p._x - C._x) * (A._y - C._y) - (p._y - C._y) * (A._x - C._x);
            const float epsilon = 1e-4f;
            if ((Sap < 0 && Sbp < 0 && Scp < 0)
                || (Sap < epsilon && Sbp < 0 && Scp < 0)
                || (Sbp < epsilon && Sap < 0 && Scp < 0)
                || (Scp < epsilon && Sap < 0 && Sbp < 0))
            {
                return currentTriangle;
            }
            if (Sap > 0f && Sap >= Sbp && Sap >= Scp)
            {
                return adjacency[currentTriangle, 0];
            }
            if (Sbp > 0f && Sbp >= Sap && Sbp >= Scp)
            {
                return adjacency[currentTriangle, 1];
            }
            if (Scp > 0f && Scp >= Sap && Scp >= Sbp)
            {
                return adjacency[currentTriangle, 2];
            }
            throw new Exception("Ошибка при построении триангуляции. Не удалось локализовать текущую точку");
        }

        private bool IsSatisfyDeloneCondition(Point2D p, Point2D v1, Point2D v2, Point2D v3)
        {
            var cosA = (v1._x - v3._x) * (v2._x - v3._x) + (v1._y - v3._y) * (v2._y - v3._y);
            var cosB = (v2._x - p._x) * (v1._x - p._x) + (v2._y - p._y) * (v1._y - p._y);
            var sinA = (v1._x - v3._x) * (v2._y - v3._y) - (v2._x - v3._x) * (v1._y - v3._y);
            var sinB = (v2._x - p._x) * (v1._y - p._y) - (v1._x - p._x) * (v2._y - p._y);
            var sinAB = sinA * cosB + cosA * sinB;
            if ((cosA < 0 && cosB < 0) || sinAB < 0)
                return false;
            return true;
        }

        private void AddPointIntoTriangle(int point, int triangle)
        {
            vertices[triangleNumber - 2, 0] = point;
            vertices[triangleNumber - 2, 1] = vertices[triangle, 1];
            vertices[triangleNumber - 2, 2] = vertices[triangle, 2];
            vertices[triangleNumber - 1, 0] = point;
            vertices[triangleNumber - 1, 1] = vertices[triangle, 2];
            vertices[triangleNumber - 1, 2] = vertices[triangle, 0];
            var adj2 = adjacency[triangle, 1];
            var adj3 = adjacency[triangle, 2];
            if (adj2 >= 0)
            {
                for (var m = 0; m < 3; m++)
                {
                    if (adjacency[adj2, m] == triangle)
                    {
                        adjacency[adj2, m] = triangleNumber - 2;
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
                        adjacency[adj3, m] = triangleNumber - 1;
                        break;
                    }
                }
            }
            adjacency[triangleNumber - 2, 0] = triangle;
            adjacency[triangleNumber - 2, 1] = adj2;
            adjacency[triangleNumber - 2, 2] = triangleNumber - 1;
            adjacency[triangleNumber - 1, 0] = triangleNumber - 2;
            adjacency[triangleNumber - 1, 1] = adj3;
            adjacency[triangleNumber - 1, 2] = triangle;
            vertices[triangle, 2] = vertices[triangle, 1];
            vertices[triangle, 1] = vertices[triangle, 0];
            vertices[triangle, 0] = point;
            adjacency[triangle, 2] = triangleNumber - 2;
            adjacency[triangle, 1] = adjacency[triangle, 0];
            adjacency[triangle, 0] = triangleNumber - 1;
            //Debug($"New triangle: {vertices[nT - 2, 0]}-{vertices[nT - 2, 1]}-{vertices[nT - 2, 2]}");
            //Debug($"New triangle: {vertices[nT - 1, 0]}-{vertices[nT - 1, 1]}-{vertices[nT - 1, 2]}");
            //Debug($"New triangle: {vertices[triangle, 0]}-{vertices[triangle, 1]}-{vertices[triangle, 2]}");
        }

        private void AddSuperTriangle()
        {
            var A = new DeloneNode2D(points.Count - 3, new Point2D(-10 * largestDimension, -10 * largestDimension));
            var B = new DeloneNode2D(points.Count - 2, new Point2D(10 * largestDimension, -10 * largestDimension));
            var C = new DeloneNode2D(points.Count - 1, new Point2D(0, 10 * largestDimension));
            points.Add(A);
            points.Add(B);
            points.Add(C);
            vertices[0, 0] = points.Count - 3;
            vertices[0, 1] = points.Count - 2;
            vertices[0, 2] = points.Count - 1;
            adjacency[0, 0] = -1;
            adjacency[0, 1] = -1;
            adjacency[0, 2] = -1;
        }

        private IEnumerable<DeloneNode2D> MapPointsToUnitSquare(List<DeloneNode2D> nodes)
        {
            var xMax = nodes.Select(p => p.Position._x).Max();
            var yMax = nodes.Select(p => p.Position._y).Max();
            var xMin = nodes.Select(p => p.Position._x).Min();
            var yMin = nodes.Select(p => p.Position._y).Min();
            largestDimension = Math.Max(Math.Abs(xMax - xMin), Math.Abs(yMax - yMin));
            foreach (var node in nodes)
            {
                var x = (node.Position._x - xMin);
                var y = (node.Position._y - yMin);
                yield return new DeloneNode2D(node.Number, new Point2D(x, y));
            }
        }

        private List<DeloneNode2D> SortPoints(List<DeloneNode2D> points)
        {
            var rows = (int)Math.Ceiling(Math.Pow(points.Count, 0.25));
            var bins = new Dictionary<DeloneNode2D, int>();
            foreach (var point in points)
            {
                var p = (int)(point.Position._y * rows * 0.999); // row
                var q = (int)(point.Position._x * rows * 0.999); // column
                var binIndex = p % 2 == 1
                    ? (p + 1) * rows - q
                    : p * rows + q + 1;
                bins.Add(point, binIndex);
            }
            return bins.OrderBy(p => p.Value).Select(p => p.Key).ToList();
        }

        private void Debug(string message)
        {
            Console.WriteLine(message);
        }
    }
}