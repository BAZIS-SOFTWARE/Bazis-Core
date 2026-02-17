using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Model.MeshObjects;

namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Класс для выдавливания плоских поверхностей и формирования тетра сетки
    /// </summary>
    public class TetraExtruder
    {
        private class Element2DSorter : IComparer<Element2D>
        {
            public static Point3D Center { get; set; }
            public int Compare(Element2D x, Element2D y)
            {
                var xL = x.CalcCentr().Sub(Center);
                var yL = y.CalcCentr().Sub(Center);

                var xSquared = xL._x * xL._x + xL._y * xL._y + xL._z * xL._z;
                var ySquared = yL._x * yL._x + yL._y * yL._y + yL._z * yL._z;

                if (Math.Abs(xSquared - ySquared) < 1e-4)
                    return 0;
                else if (xSquared > ySquared)
                    return 1;
                return -1;
            }
        }
        private int nodeStart;
        private int tetraStart;

        /// <summary>
        /// Количество тетраэдров которые были пропущены алгоритмом (например если была получена неизвестная конфигурация ребер)
        /// </summary>
        public uint MissedElements { get; private set; }
        public Dictionary<int, Node> Nodes { get; private set; } = new Dictionary<int, Node>();
        public TetraExtruder(int nodeStart = 0, int tetraStart = 0)
        {
            this.nodeStart = nodeStart;
            this.tetraStart = tetraStart;
        }

        public List<List<Tetra>> ExtrudeAlongVector(IEnumerable<Element2D> curLayer, Point3D dir, int layers = 1, float layerSize = 1)
        {
            CheckFirstLayer(curLayer);
            dir = Vector.GetVectorNorm(dir);
            var layerDir = dir.Mult(layerSize);

            var tetLayers = new List<List<Tetra>>();

            var sortedLayer = SortFirstLayer(curLayer);
            curLayer = sortedLayer;

            for (var i = 0; i < layers; ++i)
            {
                var nextLayer = CreateNextTrianglesLayer(curLayer, layerDir);
                var layer = CreateLayerTetrahedrons(curLayer, nextLayer);
                if (layer.Count != 0)
                    tetLayers.Add(layer);
                curLayer = nextLayer;
            }
            return tetLayers;
        }


        private List<Element2D> CreateNextTrianglesLayer(IEnumerable<Element2D> curLayer, Point3D layerDir)
        {
            var nextLayer = new List<Element2D>();
            var visited = new Dictionary<int, Node>();

            foreach (var triangle in curLayer)
            {
                var points = new List<Node>();
                foreach (var node in triangle.GetVertexes())
                {
                    if (!visited.ContainsKey(node.Number))
                    {
                        var coords = node.Position.Sum(layerDir);
                        var point = new Node(nodeStart, coords);
                        Nodes.Add(nodeStart++, point);
                        points.Add(point);
                        visited.Add(node.Number, point);
                    }
                    else
                        points.Add(visited[node.Number]);
                }
                var newTris = new Triangle_c(0, points.ToArray());//Рассмотреть возможность создание Quad
                nextLayer.Add(newTris);
            }
            return nextLayer;
        }

        private List<Tetra> CreateLayerTetrahedrons(IEnumerable<Element2D> curLayer, List<Element2D> nextLayer)
        {
            var index = 0;
            var lateralFaces = new Dictionary<string, string>();
            var layer = new List<Tetra>();

            foreach (var triangle in curLayer)
            {
                var downNodes = triangle.GetVertexes().ToArray();
                var upNodes = nextLayer[index++].GetVertexes().ToArray();

                var curLatFaces = GetLateralFaces(downNodes, upNodes);
                var edges = GetEdgesConfiguration(lateralFaces, curLatFaces);
                var indices = ExtractIndicesFromEdges(edges, downNodes, upNodes);

                var tetras = CreateTetrahedrons(downNodes, upNodes, indices, lateralFaces);
                if (tetras != null)
                    layer.AddRange(tetras);
            }
            return layer;
        }

        private Tetra[] CreateTetrahedrons(Node[] down, Node[] up, List<int[]> indices, Dictionary<string, string> lateralFaces)
        {
            if (indices.Count < 2)
                return SplitExistingSingleSurface(down, up, indices, lateralFaces);
            else if (indices.Count == 2)
                return SplitExistingTwoSurfaces(down, up, indices, lateralFaces);
            return SplitExistingThreeSurfaces(down, up, indices);
        }

        private Tetra[] SplitExistingSingleSurface(Node[] down, Node[] up, List<int[]> indices, Dictionary<string, string> lateralFaces)
        {
            var tetras = new Tetra[3];
            var downSet = new HashSet<int>() { 0, 1, 2 };
            var edge = indices.Count == 0 ? [0, 4] : indices[0];
            var endIndex = edge[1] - 3;

            var topIndex = downSet.Except([edge[0], endIndex]).First();

            tetras[0] = new Tetra(tetraStart, [down[0], down[1], down[2], up[topIndex]]);
            tetras[1] = new Tetra(tetraStart + 1, [down[edge[0]], up[edge[0]], up[endIndex], up[topIndex]]);
            tetras[2] = new Tetra(tetraStart + 2, [down[endIndex], down[edge[0]], up[endIndex], up[topIndex]]);
            tetraStart += 3;

            if (indices.Count == 0)
            {
                var key = GetLateralFace(down[edge[0]], down[endIndex], up[edge[0]], up[endIndex]);
                WriteLateralFace(lateralFaces, key, down[edge[0]], up[endIndex]);
            }

            var key0 = GetLateralFace(down[endIndex], down[topIndex], up[endIndex], up[topIndex]);
            WriteLateralFace(lateralFaces, key0, down[endIndex], up[topIndex]);

            var key1 = GetLateralFace(down[edge[0]], down[topIndex], up[edge[0]], up[topIndex]);
            WriteLateralFace(lateralFaces, key1, down[edge[0]], up[topIndex]);

            return tetras;
        }

        private Tetra[] SplitExistingTwoSurfaces(Node[] down, Node[] up, List<int[]> indices, Dictionary<string, string> lateralFaces)
        {
            var tetras = new Tetra[3];
            var firstUp = indices[0][1] - 3;
            var secondUp = indices[1][1] - 3;

            if (firstUp == secondUp)
            {
                var start = Math.Min(indices[0][0], indices[1][0]);
                var end = Math.Max(indices[0][0], indices[1][0]);

                tetras[0] = new Tetra(tetraStart, [down[0], down[1], down[2], up[firstUp]]);
                tetras[1] = new Tetra(tetraStart + 1, [down[start], down[end], up[end], up[firstUp]]);
                tetras[2] = new Tetra(tetraStart + 2, [down[start], up[end], up[start], up[firstUp]]);

                var key0 = GetLateralFace(down[start], down[end], up[start], up[end]);
                WriteLateralFace(lateralFaces, key0, down[start], up[end]);
                tetraStart += 3;

                return tetras;
            }
            else if (indices[0][0] == indices[1][0])
            {
                tetras[0] = new Tetra(tetraStart, [down[0], down[1], down[2], up[secondUp]]);
                tetras[1] = new Tetra(tetraStart + 1, [down[firstUp], down[indices[0][0]], up[firstUp], up[secondUp]]);
                tetras[2] = new Tetra(tetraStart + 2, [down[indices[0][0]], up[indices[0][0]], up[firstUp], up[secondUp]]);

                var key0 = GetLateralFace(down[firstUp], down[secondUp], up[firstUp], up[secondUp]);
                WriteLateralFace(lateralFaces, key0, down[firstUp], up[secondUp]);
                tetraStart += 3;

                return tetras;
            }
            else if (indices[0][0] == secondUp)
            {
                tetras[0] = new Tetra(tetraStart, [down[0], down[1], down[2], up[firstUp]]);
                tetras[1] = new Tetra(tetraStart + 1, [down[indices[1][0]], up[indices[1][0]], up[secondUp], up[firstUp]]);
                tetras[2] = new Tetra(tetraStart + 2, [down[indices[0][0]], down[indices[1][0]], up[secondUp], up[firstUp]]);

                var key0 = GetLateralFace(down[firstUp], down[indices[1][0]], up[firstUp], up[indices[1][0]]);
                WriteLateralFace(lateralFaces, key0, down[indices[1][0]], up[firstUp]);
                tetraStart += 3;

                return tetras;
            }
            tetras[0] = new Tetra(tetraStart, [down[0], down[1], down[2], up[secondUp]]);
            tetras[1] = new Tetra(tetraStart + 1, [down[indices[0][0]], up[indices[0][0]], up[firstUp], up[secondUp]]);
            tetras[2] = new Tetra(tetraStart + 2, [down[indices[1][0]], down[indices[0][0]], up[firstUp], up[secondUp]]);
            tetraStart += 3;

            var key = GetLateralFace(down[secondUp], down[indices[0][0]], up[secondUp], up[indices[0][0]]);
            WriteLateralFace(lateralFaces, key, down[indices[0][0]], up[secondUp]);

            return tetras;
        }

        private Tetra[] SplitExistingThreeSurfaces(Node[] down, Node[] up, List<int[]> indices)
        {
            var config = GetEdgeConfig(down, up, indices);
            if (config == null)
                return null;

            var tetras = new Tetra[3];

            var topIndex = indices[config.Item1][1] - 3;
            var downSlaveIndex = indices[config.Item3][0];
            var upSlaveIndex = indices[config.Item3][1] - 3;

            tetras[0] = new Tetra(tetraStart, [down[0], down[1], down[2], up[topIndex]]);
            tetras[1] = new Tetra(tetraStart + 1, [down[downSlaveIndex], up[downSlaveIndex], up[upSlaveIndex], up[topIndex]]);
            tetras[2] = new Tetra(tetraStart + 2, [down[upSlaveIndex], down[downSlaveIndex], up[upSlaveIndex], up[topIndex]]);
            tetraStart += 3;

            return tetras;
        }

        private Tuple<int, int, int> GetEdgeConfig(Node[] down, Node[] up, List<int[]> indices)
        {
            var upA = indices[0][1] - 3;
            var upB = indices[1][1] - 3;
            var upC = indices[2][1] - 3;
            if (upA == upB)
                return Tuple.Create(0, 1, 2);
            else if (upA == upC)
                return Tuple.Create(0, 2, 1);
            else if (upB == upC)
                return Tuple.Create(1, 2, 0);
            MissedElements += 3;
            return null;
        }

        private List<string> GetLateralFaces(Node[] down, Node[] up)
        {
            return new List<string>() { GetLateralFace(down[0], down[1], up[0], up[1]),
                                        GetLateralFace(down[1], down[2], up[1], up[2]),
                                        GetLateralFace(down[2], down[0], up[2], up[0])};
        }

        private string GetLateralFace(Node downA, Node downB, Node upA, Node upB)
        {
            var downEdge = $"{Math.Min(downA.Number, downB.Number)} {Math.Max(downA.Number, downB.Number)} ";
            var upEdge = $"{Math.Min(upA.Number, upB.Number)} {Math.Max(upA.Number, upB.Number)}";
            return downEdge + upEdge;
        }

        private void WriteLateralFace(Dictionary<string, string> lateralFaces, string key, Node down, Node up)
        {
            lateralFaces.Add(key, $"{down.Number} {up.Number}");
        }

        private List<string> GetEdgesConfiguration(Dictionary<string, string> lateralFaces, List<string> curLatFaces)
        {
            var edges = new List<string>();
            foreach (var face in curLatFaces)
                if (lateralFaces.ContainsKey(face))
                    edges.Add(lateralFaces[face]);
            return edges;
        }

        private IEnumerable<Element2D> SortFirstLayer(IEnumerable<Element2D> layer)
        {
            var xSum = 0.0;
            var ySum = 0.0;
            var zSum = 0.0;

            var index = 0;
            foreach (var elem in layer)
            {
                foreach (var point in elem.GetVertexes())
                {
                    xSum += point.Position._x;
                    ySum += point.Position._y;
                    zSum += point.Position._z;
                    ++index;
                }
            }
            Element2DSorter.Center = new Point3D((float)xSum / index, (float)ySum / index, (float)zSum / index);
            var comparer = new Element2DSorter();
            return layer.OrderBy(v => v, comparer);
        }

        private void CheckFirstLayer(IEnumerable<Element2D> layer)
        {
            foreach (var element in layer)
                if (element.NumberOfPoints == 4)
                    throw new Exception("Элементы типа Quad пока не поддерживаются");
        }

        private List<int[]> ExtractIndicesFromEdges(List<string> edges, Node[] down, Node[] up)
        {
            var nodes = down.Concat(up).Select((v, i) => new { K = v.Number, I = i }).ToDictionary(v => v.K, i => i.I);
            var indices = new List<int[]>();
            foreach (var edgeStr in edges)
            {
                var tokens = edgeStr.Split(' ');
                var keyA = int.Parse(tokens[0]);
                var keyB = int.Parse(tokens[1]);

                var edge = new int[2] { nodes[keyA], nodes[keyB] };
                indices.Add(edge);
            }
            return indices;
        }
    }
}
