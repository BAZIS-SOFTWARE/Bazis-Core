using Model.Interfaces.ObjectsFinders;
using Model.MeshObjects;
using Model.ObjectsCollections;
using Octree;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace OperationalController.MeshObjsCreator
{
    public class MultiPassSection2
    {
        private class GmshPoint
        {
            internal Vector2 Coords { get; private set; }
            internal int Id { get; private set; }
            internal GmshPoint(Vector2 coords, double meshSize = 0)
            {
                Coords = coords;
                Id = controller.Gmsh.Model.Occ.AddPoint(coords.X, coords.Y, 0, meshSize);
                if (Id == 0)
                    throw new Exception(controller.Gmsh.Logger.GetLastError());
            }

            public override bool Equals(object obj) => obj is GmshPoint other && Id == other.Id;

            public override int GetHashCode() => Id.GetHashCode();

            //public override string ToString() => GetKey(Coords);

            //internal static string GetKey(Vector2 coords) => $"{coords.X.ToString("F4", CultureInfo.InvariantCulture)} {coords.Y.ToString("F4", CultureInfo.InvariantCulture)}";
        }

        private class GmshEdge
        {
            public int TransfinitePoints { get; set; }
            public int Id { get; set; }
            public GmshPoint Begin { get; private set; }
            public GmshPoint End { get; set; }
            public GmshEdge(GmshPoint begin, GmshPoint end)
            {
                Begin = begin;
                End = end;

                Id = controller.Gmsh.Model.Occ.AddLine(Begin.Id, End.Id);
                if (Id == 0)
                    throw new Exception(controller.Gmsh.Logger.GetLastError());
            }
        }

        private static IGmshController controller;
        private Dictionary<string, GmshEdge> edges;
        private List<List<List<(GmshEdge, bool)>>> cellSegments;
        private PointOctree<GmshPoint> points;

        public Vector3 Origin { get; set; }
        public Vector3 AxisX { get; set; }
        public Vector3 AxisY { get; set; }
        public double MeshSize { get; set; }
        public bool Recombine { get; set; } = true;
        public List<Node> Nodes { get; private set; }

        public int TransfinitePoints { get; set; } = 10;


        public MultiPassSection2(IGmshController controller)
        {
            MultiPassSection2.controller = controller;
            edges = new Dictionary<string, GmshEdge>();
            cellSegments = new List<List<List<(GmshEdge, bool)>>>();
        }


        public IEnumerable<ObjectsSet<Element2D>> Create(List<List<Vector2>> cells)
        {
            controller.Gmsh.Model.Add("MultiPassSection");

            edges.Clear();
            cellSegments.Clear();

            cells = cells.Select(v => v.Distinct().ToList()).ToList();
            var cellPoints = BuildOctree(cells);

            foreach (var points in cellPoints)
            {
                var lastPoint = points[0];
                var first = lastPoint;
                var segments = new List<List<(GmshEdge, bool)>>();

                for (var i = 1; i < points.Count; ++i)
                {
                    CreateEdges(lastPoint, points[i], segments);
                    lastPoint = points[i];
                }
                CreateEdges(lastPoint, first, segments);
                cellSegments.Add(segments);
            }

            var surfList = CreateSurfaces();

            controller.Gmsh.Model.Occ.Synchronize();

            controller.Gmsh.Model.Mesh.Generate(2);
            controller.Gmsh.Model.Mesh.Recombine();

            var error = controller.Gmsh.Logger.GetLastError();

            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            var sets = GetElementSets();
            controller.Gmsh.Model.Remove();

            return sets;
        }

        public IEnumerable<ObjectsSet<Element2D>> CreateTransfinite(List<List<Vector2>> cells)
        {
            controller.Gmsh.Model.Add("MultiPassSection");

            edges.Clear();
            cellSegments.Clear();

            cells = cells.Select(v => v.Distinct().ToList()).ToList();
            var cellPoints = BuildOctree(cells);

            foreach (var points in cellPoints)
            {
                var lastPoint = points[0];
                var first = lastPoint;
                var segments = new List<List<(GmshEdge, bool)>>();

                for (var i = 1; i < points.Count; ++i)
                {
                    CreateTransfiniteEdges(lastPoint, points[i], segments);
                    lastPoint = points[i];
                }
                CreateTransfiniteEdges(lastPoint, first, segments);
                AdjustSegments(segments);
                cellSegments.Add(segments);
            }

            var surfList = CreateSurfaces();
            controller.Gmsh.Model.Occ.Synchronize();
            TransfiniteSurfaces(surfList, cellPoints);

            controller.Gmsh.Model.Mesh.Generate(2);

            var error = controller.Gmsh.Logger.GetLastError();

            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            var sets = GetElementSets();
            controller.Gmsh.Model.Remove();

            return sets;
        }

        private List<int> CreateSurfaces()
        {
            var surfaceList = new List<int>();
            foreach (var cell in cellSegments)
            {
                var wireList = new List<int>();
                foreach (var segment in cell)
                {
                    foreach (var edge in segment)
                    {
                        wireList.Add(edge.Item2 ? -edge.Item1.Id : edge.Item1.Id);
                    }
                }
                var wire = controller.Gmsh.Model.Occ.AddWire(wireList.ToArray());
                if (wire == 0)
                    throw new Exception(controller.Gmsh.Logger.GetLastError());
                var surface = controller.Gmsh.Model.Occ.AddPlaneSurface([wire]);
                if (surface == 0)
                    throw new Exception(controller.Gmsh.Logger.GetLastError());
                surfaceList.Add(surface);
            }

            return surfaceList;
        }

        private void TransfiniteSurfaces(List<int> surfList, List<List<GmshPoint>> cellPoints)
        {
            foreach (var cell in cellSegments)
                foreach (var segment in cell)
                    foreach (var edge in segment)
                        controller.Gmsh.Model.Mesh.SetTransfiniteCurve(edge.Item1.Id, edge.Item1.TransfinitePoints);

            for (var i = 0; i < cellPoints.Count; ++i)
            {
                if (cellPoints[i].Count == 3)
                {
                    var nAxisY = Vector3.Normalize(AxisY);
                    cellPoints[i] = cellPoints[i].OrderByDescending(v => Vector3.Dot(new Vector3(v.Coords, 0) - Origin, nAxisY)).ToList();
                    //cellPoints[i] = cellPoints[i].OrderByDescending(v => v.Coords.Y).ToList();
                }
                var corners = cellPoints[i].Select(v => v.Id).ToArray();
                controller.Gmsh.Model.Mesh.SetTransfiniteSurface(surfList[i], GmshApi.Arrangement.Left, corners);
                if(Recombine)
                    controller.Gmsh.Model.Mesh.SetRecombine(2, surfList[i]);
            }
        }

        private void CreateEdges(GmshPoint lastPoint, GmshPoint currentPoint, List<List<(GmshEdge, bool)>> segments)
        {
            var lastPos = new Vector3(lastPoint.Coords, 0);
            var curPos = new Vector3(currentPoint.Coords, 0);

            var items = GetNearbySegment(lastPos, curPos);
            if (items.Length < 2)
                throw new Exception($"Не найден сегмент между точками {lastPoint.Id} и {currentPoint.Id}");
            items = items.OrderBy(v => (v.Coords - lastPoint.Coords).LengthSquared()).ToArray();

            var last = items[0];
            var curSegments = new List<(GmshEdge, bool)>();

            for (var i = 1; i < items.Length; ++i)
            {
                var current = items[i];
                var key = $"{last.Id} {current.Id}";
                var inverseKey = $"{current.Id} {last.Id}";

                if (edges.ContainsKey(key))
                {
                    var edge = edges[key];
                    curSegments.Add((edge, false));
                }
                else if (edges.ContainsKey(inverseKey))
                {
                    var edge = edges[inverseKey];
                    curSegments.Add((edge, true));
                }
                else
                {
                    var edge = new GmshEdge(last, current);
                    curSegments.Add((edge, false));
                    edges.Add(key, edge);
                }
                last = current;
            }
            segments.Add(curSegments);
        }

        private void CreateTransfiniteEdges(GmshPoint lastPoint, GmshPoint currentPoint, List<List<(GmshEdge, bool)>> segments)
        {
            var lastPos = new Vector3(lastPoint.Coords, 0);
            var curPos = new Vector3(currentPoint.Coords, 0);

            var items = GetNearbySegment(lastPos, curPos);
            if (items.Length < 2)
                throw new Exception($"Не найден сегмент между точками {lastPoint.Id} и {currentPoint.Id}");
            items = items.OrderBy(v => (v.Coords - lastPoint.Coords).LengthSquared()).ToArray();

            var segCount = items.Length - 1;
            var actualPoints = TransfinitePoints + segCount - 1;
            var trPointsPerEdge = actualPoints / segCount;
            var trAdPoints = actualPoints - trPointsPerEdge * segCount;
            var last = items[0];
            var newEdges = 0;

            var curSegments = new List<(GmshEdge, bool)>();

            for (var i = 1; i < items.Length; ++i)
            {
                var current = items[i];
                var key = $"{last.Id} {current.Id}";
                var inverseKey = $"{current.Id} {last.Id}";

                if (edges.ContainsKey(key))
                {
                    var edge = edges[key];
                    curSegments.Add((edge, false));
                }
                else if (edges.ContainsKey(inverseKey))
                {
                    var edge = edges[inverseKey];
                    curSegments.Add((edge, true));
                }
                else
                {
                    var edge = new GmshEdge(last, current);
                    curSegments.Add((edge, false));
                    edge.TransfinitePoints = trPointsPerEdge;
                    edges.Add(key, edge);
                    ++newEdges;
                }
                last = current;
            }
            if (newEdges > 0)
                for (var i = 0; i < trAdPoints; ++i)
                    ++curSegments[i].Item1.TransfinitePoints;
            segments.Add(curSegments);
        }

        private void AdjustSegments(List<List<(GmshEdge, bool)>> segments)
        {
            if (segments.Count == 4)
            {
                for (var i = 0; i < 2; ++i)
                {
                    var opIndex = i ^ 2;
                    var sum = segments[i].Sum(v => v.Item1.TransfinitePoints) - segments[i].Count + 1;
                    var opSum = segments[opIndex].Sum(v => v.Item1.TransfinitePoints) - segments[opIndex].Count + 1;
                    if (sum > opSum)
                        DistributeTransfinition(segments[opIndex], sum);
                    else if (opSum > sum)
                        DistributeTransfinition(segments[i], opSum);
                }
            }
            else
            {
                var index = 0;
                var maxSum = int.MinValue;
                for (var i = 0; i < segments.Count; ++i)
                {
                    var sum = segments[i].Sum(v => v.Item1.TransfinitePoints) - segments[i].Count + 1;
                    if (sum > maxSum)
                    {
                        index = i;
                        maxSum = sum;
                    }
                }

                for (var i = 0; i < segments.Count; ++i)
                {
                    if (i != index)
                        DistributeTransfinition(segments[i], maxSum);
                }
            }
        }

        private void DistributeTransfinition(List<(GmshEdge, bool)> segments, int sum)
        {
            var segCount = segments.Count;
            var actualPoints = sum + segCount - 1;
            var trPointsPerEdge = actualPoints / segCount;
            var trAdPoints = actualPoints - trPointsPerEdge * segCount;

            foreach (var edge in segments)
                edge.Item1.TransfinitePoints = trPointsPerEdge;

            for (var i = 0; i < trAdPoints; ++i)
                ++segments[i].Item1.TransfinitePoints;
        }

        private List<List<GmshPoint>> BuildOctree(List<List<Vector2>> cells, float initialNode = 0.1f)
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;

            var maxX = float.MinValue;
            var maxY = float.MinValue;

            foreach (var cell in cells.SelectMany(v => v))
            {
                minX = MathF.Min(minX, cell.X);
                minY = MathF.Min(minY, cell.Y);

                maxX = MathF.Max(maxX, cell.X);
                maxY = MathF.Max(maxY, cell.Y);
            }

            var centerX = (maxX + minX) * 0.5f;
            var centerY = (maxY + minY) * 0.5f;

            var sizeX = maxX - minX;
            var sizeY = maxY - minY;

            var size = MathF.Max(sizeX, sizeY);

            points = new PointOctree<GmshPoint>(size, new Vector3(centerX, centerY, 0), initialNode);
            var result = new List<List<GmshPoint>>();

            foreach (var cell in cells)
            {
                var inPoints = new List<GmshPoint>();
                foreach (var vec in cell)
                {
                    var pos = new Vector3(vec, 0);
                    var items = points.GetNearby(pos, 1e-4f);
                    if (items.Length == 0)
                    {
                        items = [new GmshPoint(vec, MeshSize)];
                        points.Add(items[0], pos);
                    }
                    inPoints.Add(items[0]);
                }
                result.Add(inPoints);
            }

            return result;
        }

        private GmshPoint[] GetNearbySegment(Vector3 segmentStart, Vector3 segmentEnd, float radius = 0.1f)
        {
            var direction = segmentEnd - segmentStart;
            var length = direction.Length();

            var normalizedDirection = Vector3.Normalize(direction);
            var ray = new Ray(segmentStart, normalizedDirection);

            var candidates = points.GetNearby(ray, radius);
            var result = new List<GmshPoint>();

            foreach (var item in candidates)
            {
                var pointPosition = new Vector3(item.Coords, 0);

                var toPoint = pointPosition - segmentStart;
                var projection = Vector3.Dot(toPoint, normalizedDirection);

                if (projection > -1e-4 && length - projection > -1e-4)
                    result.Add(item);
            }

            return result.ToArray();
        }


        private List<ObjectsSet<Element2D>> GetElementSets()
        {
            var dimTags = controller.Gmsh.Model.GetEntities(2);
            var rnd = new Random();
            var result = new List<ObjectsSet<Element2D>>();

            Nodes = controller.GetNodes();

            foreach (var node in Nodes)
                To3D(node);


            for (var i = 0; i < dimTags.Length; i += 2)
            {
                var tag = dimTags[i + 1];
                var set = new ObjectsSet<Element2D>($"MultiPassSection_{tag}");

                var objects = GetMeshEntities(2, tag, Nodes);

                foreach (var item in objects)
                    set.Add(item.Number, item);

                var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                set.SetColor(color);

                result.Add(set);
            }

            return result;
        }


        private List<Element2D> GetMeshEntities(int dim, int tag, List<Node> nodes)
        {
            var list = new List<Element2D>();
            if (nodes.Count > 0)
            {
                var data = controller.Gmsh.Model.Mesh.GetElements(dim, tag);
                for (var i = 0; i < data.Item1.Length; ++i)
                {
                    var properties = controller.Gmsh.Model.Mesh.GetElementProperties(data.Item1[i]);
                    var nodesCount = properties.numNodes;
                    var elements = data.Item2[i];

                    for (var j = 0; j < elements.Length; ++j)
                    {
                        var nodesPerElems = new Node[nodesCount];
                        for (int k = 0; k < nodesCount; ++k)
                        {
                            var tagStride = j * nodesCount + k;
                            nodesPerElems[k] = ByNumberFinder.Find(nodes, (int)data.Item3[i][tagStride]);
                            if (nodesPerElems[k] == null)
                                throw new Exception($"Элемент с номером {data.Item3[i][tagStride]} не найден");
                        }
                        if (nodesPerElems.Length == 3)
                            list.Add(new Triangle_c((int)elements[j], nodesPerElems));
                        else if (nodesPerElems.Length == 4)
                            list.Add(new Quad((int)elements[j], nodesPerElems));
                        else
                            throw new Exception($"Элемент с номером {(int)elements[j]} не поддерживается, так как содержит {nodesPerElems.Length} вершин");
                    }
                }
            }
            return list;
        }

        private void To3D(Node vec)
        {
            var transformPoint = Origin + AxisX * vec.Position._x + AxisY * vec.Position._y;

            vec.Position._x = transformPoint.X;
            vec.Position._y = transformPoint.Y;
            vec.Position._z = transformPoint.Z;
        }

        private MultiPassSection2()
        {

        }
    }
}
