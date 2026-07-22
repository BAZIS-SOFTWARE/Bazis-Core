using QuickGraph;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using OperationalController.GmshController;
using Model.Interfaces.MeshObjects;
using Model.ObjectsCollections;
using Model.MeshObjects;

namespace OperationalController.MeshObjsCreator
{
    public class MultiPassSection
    {
        private static IGmshController controller;
        private UndirectedGraph<GmshPoint, GmshEdge> graph;
        private Dictionary<string, GmshPoint> points;
        private List<int> curveLoop;
        private List<List<GmshEdge>> segmentsLoop;
        private List<(int, float)> corners;

        public Vector3 Origin { get; set; }
        public Vector3 AxisX { get; set; }
        public Vector3 AxisY { get; set; }

        public int TransfinitePoints { get; set; } = 10;
        private class GmshPoint
        {
            internal Vector2 Coords { get; private set; }
            internal int Id { get; private set; }
            internal GmshPoint(Vector2 coords)
            {
                Coords = coords;
                Id = controller.Gmsh.Model.Occ.AddPoint(coords.X, coords.Y, 0);
                if (Id == 0)
                    throw new Exception(controller.Gmsh.Logger.GetLastError());
            }

            public override bool Equals(object obj) => obj is GmshPoint other && Id == other.Id;

            public override int GetHashCode() => Id.GetHashCode();

            public override string ToString() => GetKey(Coords);

            internal static string GetKey(Vector2 coords) => $"{coords.X.ToString("F4", CultureInfo.InvariantCulture)} {coords.Y.ToString("F4", CultureInfo.InvariantCulture)}";
        }

        private class GmshEdge : Edge<GmshPoint>
        {
            public int TransfinitePoints { get; set; }
            public int Id { get; set; }
            public GmshEdge(GmshPoint source, GmshPoint target) : base(source, target)
            {
                Id = controller.Gmsh.Model.Occ.AddLine(source.Id, target.Id);
                if (Id == 0)
                    throw new Exception(controller.Gmsh.Logger.GetLastError());
            }
            internal Vector2 ToVector(bool isInverted = false) => isInverted ? Source.Coords - Target.Coords
                                                                             : Target.Coords - Source.Coords;
        }

        internal MultiPassSection(IGmshController controller)
        {
            MultiPassSection.controller = controller;
            graph = new UndirectedGraph<GmshPoint, GmshEdge>();
            points = new Dictionary<string, GmshPoint>();
            curveLoop = new List<int>();
            segmentsLoop = new List<List<GmshEdge>>();
            corners = new List<(int, float)>();
        }

        private static List<List<Vector2>> SortCells(List<List<Vector2>> cells)
        {
            var cellItems = new List<(List<Vector2>, Vector2)>();
            foreach (var cell in cells)
            {
                var cx = 0f;
                var cy = 0f;

                foreach (var vec in cell)
                {
                    cx += vec.X;
                    cy += vec.Y;
                }

                var center = new Vector2(cx / cell.Count, cy / cell.Count);
                cellItems.Add((cell, center));
            }
            return cellItems.OrderBy(v => v.Item2.Y)
                            .ThenBy(v => v.Item2.X)
                            .Select(v => v.Item1)
                            .ToList();
        }

        private static List<Vector2> SortByAngle(List<Vector2> points)
        {
            if (points == null || points.Count == 0)
                throw new ArgumentException("Облако точек не может быть пустым");

            var cx = 0f;
            var cy = 0f;
            foreach (var p in points)
            {
                cx += p.X;
                cy += p.Y;
            }

            var center = new Vector2(cx / points.Count, cy / points.Count);
            var sorted = new List<Vector2>(points);

            sorted.Sort((a, b) =>
            {
                var dx1 = a.X - center.X;
                var dy1 = a.Y - center.Y;
                var dx2 = b.X - center.X;
                var dy2 = b.Y - center.Y;

                var ang1 = Math.Atan2(dy1, dx1);
                var ang2 = Math.Atan2(dy2, dx2);

                if (ang1 < 0) ang1 += 2 * Math.PI;
                if (ang2 < 0) ang2 += 2 * Math.PI;

                var result = ang1.CompareTo(ang2);

                if (result == 0)
                {
                    var dist1 = dx1 * dx1 + dy1 * dy1;
                    var dist2 = dx2 * dx2 + dy2 * dy2;
                    return dist1.CompareTo(dist2);
                }

                return result;
            });

            return sorted;
        }

        public IEnumerable<ObjectsSet<Element2D>> Create(List<List<Vector2>> cells)
        {
            if (AxisX == Vector3.Zero)
                throw new Exception("Задайте ось X для свойства AxisX");
            if (AxisY == Vector3.Zero)
                throw new Exception("Задайте ось Y для свойства AxisY");

            controller.Gmsh.Model.Add("MultiPassSection");
            cells = SortCells(cells);

            for (var i = 0; i < cells.Count; ++i)
            {
                cells[i] = SortByAngle(cells[i]);
                var last = cells[i][0];
                segmentsLoop.Add(new List<GmshEdge>());
                var sLoopIndex = 0;

                GmshEdge prevEdge = null;

                for (var j = 1; j < cells[i].Count; ++j)
                {
                    var current = cells[i][j];
                    var isExist = false;

                    var source = AddVertex(last);
                    var target = AddVertex(current);
                    var curEdge = AddEdge(source, target, ref isExist);

                    curveLoop.Add(isExist ? -curEdge.Id : curEdge.Id);

                    if (prevEdge != null)
                    {
                        if (segmentsLoop[sLoopIndex].Count == 0)
                        {
                            segmentsLoop[sLoopIndex].Add(prevEdge);
                            corners.Add(curveLoop[curveLoop.Count - 2] < 0 ? (prevEdge.Target.Id, prevEdge.Target.Coords.Y)
                                                                           : (prevEdge.Source.Id, prevEdge.Source.Coords.Y));
                        }
                        if (IsMultiEdge(prevEdge, curEdge))
                            segmentsLoop[sLoopIndex].Add(curEdge);
                        else
                        {
                            segmentsLoop.Add(new List<GmshEdge>() { curEdge });
                            corners.Add(curveLoop[curveLoop.Count - 1] < 0 ? (curEdge.Target.Id, curEdge.Target.Coords.Y)
                                                                           : (curEdge.Source.Id, curEdge.Source.Coords.Y));
                            sLoopIndex++;
                        }
                    }

                    prevEdge = curEdge;
                    last = current;
                }

                var cur = cells[i][0];
                var exist = false;

                var src = AddVertex(last);
                var trgt = AddVertex(cur);
                var crEdge = AddEdge(src, trgt, ref exist);

                curveLoop.Add(exist ? -crEdge.Id : crEdge.Id);

                if (IsMultiEdge(prevEdge, crEdge))
                    segmentsLoop[sLoopIndex].Add(crEdge);
                else
                {
                    segmentsLoop.Add(new List<GmshEdge>() { crEdge });
                    corners.Add(curveLoop[curveLoop.Count - 1] < 0 ? (crEdge.Target.Id, crEdge.Target.Coords.Y)
                                                               : (crEdge.Source.Id, crEdge.Source.Coords.Y));
                }
                var surface = AddSurface();

                corners.Clear();
                segmentsLoop.Clear();
                curveLoop.Clear();
            }

            controller.Gmsh.Model.Mesh.Generate(2);
            var error = controller.Gmsh.Logger.GetLastError();

            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            var nodes = controller.GetNodes();
            foreach (var node in nodes)
                To3D(node);

            var elements = controller.GetMeshSets<Element2D>(nodes);

            controller.Gmsh.Model.Remove();
            return elements;
        }

        private void To3D(Node vec)
        {
            var transformPoint = Origin + AxisX * vec.Position._x + AxisY * vec.Position._y;

            vec.Position._x = transformPoint.X;
            vec.Position._y = transformPoint.Y;
            vec.Position._z = transformPoint.Z;
        }

        private int AddSurface()
        {
            var newCorners = SortCorners();

            var wire = controller.Gmsh.Model.Occ.AddWire(curveLoop.ToArray());
            if (wire == 0)
                throw new Exception(controller.Gmsh.Logger.GetLastError());

            var surface = controller.Gmsh.Model.Occ.AddPlaneSurface([wire]);
            if (surface == 0)
                throw new Exception(controller.Gmsh.Logger.GetLastError());

            controller.Gmsh.Model.Occ.Synchronize();

            TransfiniteCurves();

            controller.Gmsh.Model.Mesh.SetTransfiniteSurface(surface, GmshApi.Arrangement.Left, newCorners.ToArray());
            controller.Gmsh.Model.Mesh.SetRecombine(2, surface);

            return surface;
        }

        private void TransfiniteCurves()
        {
            for (var i = 0; i < segmentsLoop.Count; ++i)
            {
                var empty = new List<GmshEdge>();
                var filled = new List<GmshEdge>();
                foreach (var item in segmentsLoop[i])
                {
                    if (item.TransfinitePoints == 0)
                        empty.Add(item);
                    else
                        filled.Add(item);
                }
                if (empty.Count > 0)
                {
                    var pointsOnEdge = segmentsLoop[i].Count + 1;

                    var filledPoints = 0;
                    foreach (var data in filled)
                        filledPoints += data.TransfinitePoints - 2;

                    var transfinited = GetOppositeTransfinition(i);

                    var dividend = transfinited - pointsOnEdge - filledPoints;
                    if (dividend < 0)
                        throw new Exception("The number of transfinition points is small, set a higher threshold");

                    if (dividend > 0)
                    {
                        var divisor = empty.Count;

                        int baseValue = dividend / divisor;
                        int remainder = dividend % divisor;

                        for (int j = 0; j < divisor; ++j)
                        {
                            empty[j].TransfinitePoints = baseValue + 2;
                            controller.Gmsh.Model.Mesh.SetTransfiniteCurve(empty[j].Id, empty[j].TransfinitePoints);
                        }

                        for (int j = 0; j < remainder; ++j)
                        {
                            empty[j].TransfinitePoints++;
                            controller.Gmsh.Model.Mesh.SetTransfiniteCurve(empty[j].Id, empty[j].TransfinitePoints);
                        }
                    }
                }
            }
        }

        private int GetOppositeTransfinition(int index)
        {
            if (segmentsLoop.Count == 3)
                return TransfinitePoints;

            if (index < 0 || index > 3)
                throw new ArgumentException("Диапазон ввода index должен быть от 0 до 3");
            var opp = index ^ 2;

            var filled = 0;
            var total = 0;
            foreach (var segment in segmentsLoop[opp])
            {
                if (segment.TransfinitePoints != 0)
                    ++filled;
                total += segment.TransfinitePoints;
            }
            if (segmentsLoop[opp].Count != filled)
                return TransfinitePoints;
            return total - (filled - 1);
        }

        private List<int> SortCorners()
        {
            if (corners.Count < 3)
                throw new Exception("Количество угловых точек должно быть > 3");

            var max = float.MinValue;
            var maxIndex = 0;
            for (var i = 0; i < corners.Count; ++i)
            {
                if (corners[i].Item2 > max)
                {
                    max = corners[i].Item2;
                    maxIndex = i;
                }
            }

            var newCorners = new List<int>();
            for (int i = maxIndex; i < corners.Count; ++i)
                newCorners.Add(corners[i].Item1);
            for (int i = 0; i < maxIndex; ++i)
                newCorners.Add(corners[i].Item1);

            return newCorners;
        }

        private GmshPoint AddVertex(Vector2 coord)
        {
            var key = GmshPoint.GetKey(coord);
            if (!points.ContainsKey(key))
            {
                var point = new GmshPoint(coord);
                points[key] = point;
                graph.AddVertex(point);
            }
            return points[key];
        }

        private GmshEdge AddEdge(GmshPoint source, GmshPoint target, ref bool isExists)
        {
            GmshEdge edge;
            isExists = graph.TryGetEdge(source, target, out edge);
            if (isExists)
                return edge;
            edge = new GmshEdge(source, target);
            graph.AddEdge(edge);
            return edge;
        }

        private bool IsMultiEdge(GmshEdge last, GmshEdge current, float epsilon = 1e-4f)
        {
            var lastVec = last.ToVector();
            var curVec = current.ToVector();

            lastVec = Vector2.Normalize(lastVec);
            curVec = Vector2.Normalize(curVec);

            var dp = Math.Abs(Vector2.Dot(lastVec, curVec));

            return Math.Abs(dp - 1) < epsilon;
        }

        private MultiPassSection()
        {
        }
    }
}
