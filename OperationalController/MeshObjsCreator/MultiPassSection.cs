using GmshApi;
using Microsoft.Scripting.Utils;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsFinders;
using Model.MeshObjects;
using Model.ObjectsCollections;
using OperationalController.GmshController;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using static IronPython.Modules._ast;

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
        private List<SurfaceTransfiniteData> transfiniteSurfaces;

        public Vector3 Origin { get; set; }
        public Vector3 AxisX { get; set; }
        public Vector3 AxisY { get; set; }
        public List<Node> Nodes { get; private set; }

        // Базовое желаемое число точек на логическую сторону прохода. Это не жесткий
        // максимум: общие стороны могут получить больше точек, если соседняя ячейка
        // уже разбила эту границу на несколько участков.
        public int TransfinitePoints { get; set; } = 10;

        // Мягкое ограничение размера внутренних элементов для свободных/recombine
        // поверхностей. Значение считается от среднего шага transfinite-кривых и
        // временно применяется только на время генерации этой секции.
        public double InteriorMeshSizeFactor { get; set; } = 1.5;

        // Дополнительное равномерное дробление после генерации. По умолчанию
        // отключено, чтобы не делать проходы плотнее заданных TransfinitePoints.
        public int InteriorRefineSteps { get; set; } = 0;
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

        private class SurfaceTransfiniteData
        {
            internal int Surface { get; }
            internal List<int> Corners { get; }
            internal List<List<GmshEdge>> SegmentsLoop { get; }

            internal SurfaceTransfiniteData(int surface, List<int> corners, List<List<GmshEdge>> segmentsLoop)
            {
                Surface = surface;
                Corners = corners;
                SegmentsLoop = segmentsLoop;
            }
        }

        public MultiPassSection(IGmshController controller)
        {
            MultiPassSection.controller = controller;
            graph = new UndirectedGraph<GmshPoint, GmshEdge>();
            points = new Dictionary<string, GmshPoint>();
            curveLoop = new List<int>();
            segmentsLoop = new List<List<GmshEdge>>();
            corners = new List<(int, float)>();
            transfiniteSurfaces = new List<SurfaceTransfiniteData>();
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

        /// <summary>
        /// Дополняет вершины каждой ячейки точками других ячеек, которые лежат на её рёбрах,
        /// но отсутствуют в исходном наборе вершин. Форма ячеек не изменяется, только уточняется
        /// контур: между соседними исходными вершинами ребра вставляются промежуточные точки.
        /// Порядок обхода вершин (по часовой стрелке) сохраняется, исходная коллекция не модифицируется.
        /// </summary>
        /// <param name="cells">Список ячеек, каждая — список вершин (3 или 4), отсортированных по часовой стрелке.</param>
        /// <param name="tolerance">Допуск для сравнения координат с плавающей точкой.</param>
        /// <returns>Новый список ячеек с добавленными промежуточными вершинами на рёбрах.</returns>
        /// <example>
        /// <code>
        /// var cells = new List&lt;List&lt;Vector2&gt;&gt;
        /// {
        ///     new List&lt;Vector2&gt; { new Vector2(-1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0.6f), new Vector2(-0.4f, 0.6f) },
        ///     new List&lt;Vector2&gt; { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.4f, 0.6f), new Vector2(0f, 0.6f) },
        ///     new List&lt;Vector2&gt; { new Vector2(-0.4f, 0.6f), new Vector2(0.4f, 0.6f), new Vector2(0f, 1f) }
        /// };
        ///
        /// var refinedCells = MultiPassSection.InsertSharedEdgePoints(cells);
        ///
        /// // refinedCells[2] == [(-0.4, 0.6), (0, 0.6), (0.4, 0.6), (0, 1)]
        /// // точка (0, 0.6) добавлена между (-0.4, 0.6) и (0.4, 0.6)
        /// </code>
        /// </example>
        public static List<List<Vector2>> InsertSharedEdgePoints(List<List<Vector2>> cells, float tolerance = 1e-4f)
        {
            if (cells == null)
                throw new ArgumentNullException(nameof(cells));

            // Собираем все уникальные граничные точки: исходные вершины и
            // endpoints частично наложенных коллинеарных ребер. Произвольные
            // пересечения ребер не вставляем, чтобы не менять форму ячеек.
            var allBoundaryPoints = new List<Vector2>();
            var allEdges = new List<(Vector2 Start, Vector2 End)>();
            foreach (var cell in cells)
            {
                foreach (var vertex in cell)
                {
                    AddUniquePoint(allBoundaryPoints, vertex, tolerance);
                }

                for (var i = 0; i < cell.Count; ++i)
                    allEdges.Add((cell[i], cell[(i + 1) % cell.Count]));
            }

            for (var i = 0; i < allEdges.Count; ++i)
            {
                for (var j = i + 1; j < allEdges.Count; ++j)
                {
                    foreach (var point in GetSharedSegmentPoints(
                        allEdges[i].Start,
                        allEdges[i].End,
                        allEdges[j].Start,
                        allEdges[j].End,
                        tolerance))
                    {
                        AddUniquePoint(allBoundaryPoints, point, tolerance);
                    }
                }
            }

            var result = new List<List<Vector2>>(cells.Count);

            foreach (var cell in cells)
            {
                var updatedCell = new List<Vector2>();
                var vertexCount = cell.Count;

                for (var i = 0; i < vertexCount; ++i)
                {
                    var edgeStart = cell[i];
                    var edgeEnd = cell[(i + 1) % vertexCount]; // по модулю замыкает последнее ребро на первую вершину

                    updatedCell.Add(edgeStart);

                    // Точки других ячеек, которые лежат строго на этом ребре и ещё не
                    // присутствуют среди исходных вершин текущей ячейки.
                    var extraPoints = allBoundaryPoints
                        .Where(v => !cell.Any(cv => AreVectorsEqual(cv, v, tolerance)))
                        .Where(v => IsPointOnSegment(v, edgeStart, edgeEnd, tolerance))
                        .OrderBy(v => Vector2.DistanceSquared(edgeStart, v)) // упорядочиваем вдоль ребра от edgeStart к edgeEnd
                        .ToList();

                    updatedCell.AddRange(extraPoints);
                }

                result.Add(updatedCell);
            }

            return result;
        }

        private static void AddUniquePoint(List<Vector2> points, Vector2 point, float tolerance)
        {
            if (!points.Any(v => AreVectorsEqual(v, point, tolerance)))
                points.Add(point);
        }

        private static List<Vector2> GetSharedSegmentPoints(
            Vector2 firstStart,
            Vector2 firstEnd,
            Vector2 secondStart,
            Vector2 secondEnd,
            float tolerance)
        {
            var result = new List<Vector2>();
            var firstVector = firstEnd - firstStart;
            var secondVector = secondEnd - secondStart;
            var startDelta = secondStart - firstStart;

            if (Math.Abs(Cross(firstVector, secondVector)) > tolerance ||
                Math.Abs(Cross(firstVector, startDelta)) > tolerance)
            {
                return result;
            }

            AddEndpointIfShared(result, firstStart, secondStart, secondEnd, tolerance);
            AddEndpointIfShared(result, firstEnd, secondStart, secondEnd, tolerance);
            AddEndpointIfShared(result, secondStart, firstStart, firstEnd, tolerance);
            AddEndpointIfShared(result, secondEnd, firstStart, firstEnd, tolerance);
            return result;
        }

        private static void AddEndpointIfShared(
            List<Vector2> points,
            Vector2 point,
            Vector2 segmentStart,
            Vector2 segmentEnd,
            float tolerance)
        {
            if (IsPointOnSegment(point, segmentStart, segmentEnd, tolerance))
                AddUniquePoint(points, point, tolerance);
        }

        private static float Cross(Vector2 first, Vector2 second) =>
            first.X * second.Y - first.Y * second.X;

        /// <summary>
        /// Проверяет, лежит ли точка на отрезке [segmentStart, segmentEnd] с учётом погрешности вычислений.
        /// </summary>
        private static bool IsPointOnSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, float tolerance)
        {
            var edgeVector = segmentEnd - segmentStart;
            var pointVector = point - segmentStart;

            var edgeLength = edgeVector.Length();
            if (edgeLength < tolerance)
                return AreVectorsEqual(point, segmentStart, tolerance);

            // Расстояние от точки до прямой, на которой лежит отрезок (через векторное произведение).
            var cross = edgeVector.X * pointVector.Y - edgeVector.Y * pointVector.X;
            var distanceToLine = Math.Abs(cross) / edgeLength;
            if (distanceToLine > tolerance)
                return false;

            // Проверяем, что проекция точки попадает внутрь отрезка, а не на его продолжение.
            var projection = Vector2.Dot(pointVector, edgeVector) / edgeLength;
            return projection >= -tolerance && projection <= edgeLength + tolerance;
        }

        /// <summary>
        /// Сравнивает две точки с учётом допустимой погрешности вычислений с плавающей точкой.
        /// </summary>
        private static bool AreVectorsEqual(Vector2 a, Vector2 b, float tolerance) =>
            Vector2.DistanceSquared(a, b) <= tolerance * tolerance;

        public IEnumerable<ObjectsSet<Element2D>> Create(List<List<Vector2>> cells)
        {
            if (AxisX == Vector3.Zero)
                throw new Exception("Задайте ось X для свойства AxisX");
            if (AxisY == Vector3.Zero)
                throw new Exception("Задайте ось Y для свойства AxisY");

            controller.Gmsh.Model.Add("MultiPassSection");
            transfiniteSurfaces.Clear();
            cells = cells.Select(v => SortByAngle(v.Distinct().ToList())).ToList();
            cells = InsertSharedEdgePoints(cells);
            cells = SortCells(cells);

            for (var i = 0; i < cells.Count; ++i)
            {
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

            // Поверхности помечаем transfinite только после создания всех ячеек:
            // поздние соседние ячейки могут добавить точки на общие ребра и тем
            // самым изменить совместимость противоположных сторон.
            SetTransfiniteSurfaces();

            var previousMeshSizeMax = ApplyInteriorMeshSizeLimit();
            try
            {
                controller.Gmsh.Model.Mesh.Generate(2);
                RefineInteriorMesh();
            }
            finally
            {
                RestoreMeshSizeMax(previousMeshSizeMax);
            }

            var error = controller.Gmsh.Logger.GetLastError();

            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            var sets = GetElementSets();
            controller.Gmsh.Model.Remove();
            transfiniteSurfaces.Clear();

            return sets;
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

            controller.Gmsh.Model.Mesh.SetRecombine(2, surface);
            // Данные поверхности сохраняем для отложенного SetTransfiniteSurface:
            // применять его сразу опасно, пока не обработаны все соседние проходы.
            transfiniteSurfaces.Add(new SurfaceTransfiniteData(
                surface,
                newCorners,
                segmentsLoop.Select(v => v.ToList()).ToList()));

            return surface;
        }

        private void RefineInteriorMesh()
        {
            for (var i = 0; i < InteriorRefineSteps; ++i)
                controller.Gmsh.Model.Mesh.Refine();
        }

        private double? ApplyInteriorMeshSizeLimit()
        {
            if (InteriorMeshSizeFactor <= 0)
                return null;

            var targetSize = GetInteriorMeshSizeLimit();
            if (targetSize <= 0)
                return null;

            var previousMeshSizeMax = controller.Gmsh.Option.GetNumber("Mesh.MeshSizeMax");
            var meshSizeMax = previousMeshSizeMax > 0
                ? Math.Min(previousMeshSizeMax, targetSize)
                : targetSize;

            // Gmsh options глобальны для текущей модели, поэтому старое значение
            // обязательно возвращается в finally после генерации.
            controller.Gmsh.Option.SetNumber("Mesh.MeshSizeMax", meshSizeMax);
            return previousMeshSizeMax;
        }

        private static void RestoreMeshSizeMax(double? previousMeshSizeMax)
        {
            if (previousMeshSizeMax.HasValue)
                controller.Gmsh.Option.SetNumber("Mesh.MeshSizeMax", previousMeshSizeMax.Value);
        }

        private double GetInteriorMeshSizeLimit()
        {
            var totalLength = 0.0;
            var totalIntervals = 0;

            foreach (var edge in graph.Edges)
            {
                if (edge.TransfinitePoints <= 1)
                    continue;

                totalLength += edge.ToVector().Length();
                totalIntervals += edge.TransfinitePoints - 1;
            }

            if (totalIntervals == 0)
                return 0;

            return totalLength / totalIntervals * InteriorMeshSizeFactor;
        }

        private void SetTransfiniteSurfaces()
        {
            // Не смешиваем transfinite и free/recombine surfaces внутри одной
            // секции: на общих границах это давало несогласованную сетку. Если
            // хоть одна поверхность не подходит, вся секция остается free/recombine.
            if (transfiniteSurfaces.Any(v => !CanSetTransfiniteSurface(v)))
                return;

            foreach (var surfaceData in transfiniteSurfaces)
            {
                controller.Gmsh.Model.Mesh.SetTransfiniteSurface(
                    surfaceData.Surface,
                    GmshApi.Arrangement.Left,
                    surfaceData.Corners.ToArray());
            }
        }

        private static bool CanSetTransfiniteSurface(SurfaceTransfiniteData surfaceData)
        {
            if (surfaceData.Corners.Count > 4)
                return false;

            if (surfaceData.Corners.Count != surfaceData.SegmentsLoop.Count)
                return false;

            if (surfaceData.SegmentsLoop.Count != 4)
                return true;

            var firstPairIsCompatible =
                GetLogicalTransfinitePoints(surfaceData.SegmentsLoop[0]) ==
                GetLogicalTransfinitePoints(surfaceData.SegmentsLoop[2]);
            var secondPairIsCompatible =
                GetLogicalTransfinitePoints(surfaceData.SegmentsLoop[1]) ==
                GetLogicalTransfinitePoints(surfaceData.SegmentsLoop[3]);

            return firstPairIsCompatible && secondPairIsCompatible;
        }

        private void TransfiniteCurves()
        {
            if (segmentsLoop.Count != 4)
            {
                // Нечетырехугольные контуры не могут быть обычной transfinite
                // surface, но их граничные кривые все равно нужно дискретизировать
                // согласованно с уже созданными соседями.
                for (var i = 0; i < segmentsLoop.Count; ++i)
                {
                    var target = Math.Max(TransfinitePoints, GetRequiredTransfinitePoints(segmentsLoop[i]));
                    DistributeTransfinitePoints(segmentsLoop[i], target, i, -1);
                }
                return;
            }

            for (var i = 0; i < segmentsLoop.Count; ++i)
            {
                var opposite = GetOppositeSideIndex(i);
                if (i > opposite)
                    continue;

                var target = GetTargetTransfinitePoints(segmentsLoop[i], segmentsLoop[opposite]);

                DistributeTransfinitePoints(segmentsLoop[i], target, i, opposite);
                DistributeTransfinitePoints(segmentsLoop[opposite], target, opposite, i);

                ValidateOppositeTransfinitePoints(i, opposite, target);
            }
        }

        private int GetTargetTransfinitePoints(IReadOnlyList<GmshEdge> firstSide, IReadOnlyList<GmshEdge> oppositeSide)
        {
            return Math.Max(
                TransfinitePoints,
                Math.Max(
                    GetRequiredTransfinitePoints(firstSide),
                    GetRequiredTransfinitePoints(oppositeSide)));
        }

        private static int GetRequiredTransfinitePoints(IReadOnlyList<GmshEdge> segments)
        {
            var filledPoints = 0;
            foreach (var segment in segments)
            {
                if (segment.TransfinitePoints > 0)
                    filledPoints += segment.TransfinitePoints - 2;
            }

            return segments.Count + 1 + filledPoints;
        }

        private void DistributeTransfinitePoints(IReadOnlyList<GmshEdge> segments, int target, int sideIndex, int oppositeSideIndex)
        {
            var empty = new List<GmshEdge>();
            var filledPoints = 0;

            foreach (var segment in segments)
            {
                if (segment.TransfinitePoints == 0)
                    empty.Add(segment);
                else
                {
                    if (segment.TransfinitePoints < 2)
                        throw CreateTransfiniteException(
                            "Transfinite curve segment has fewer than two points.",
                            sideIndex,
                            oppositeSideIndex,
                            segments,
                            target);

                    filledPoints += segment.TransfinitePoints - 2;
                }
            }

            var required = segments.Count + 1 + filledPoints;
            var dividend = target - required;

            if (dividend < 0)
                throw CreateTransfiniteException(
                    "Calculated target is smaller than the points already required by the side.",
                    sideIndex,
                    oppositeSideIndex,
                    segments,
                    target);

            if (empty.Count == 0)
            {
                var current = GetLogicalTransfinitePoints(segments);
                if (current > target)
                    throw CreateTransfiniteException(
                        "Side has more already assigned transfinite points than the calculated target.",
                        sideIndex,
                        oppositeSideIndex,
                        segments,
                        target);

                DistributeAdditionalPoints(segments, target - current);
                return;
            }

            var baseValue = dividend / empty.Count;
            var remainder = dividend % empty.Count;

            for (var i = 0; i < empty.Count; ++i)
            {
                empty[i].TransfinitePoints = baseValue + 2;
                if (i < remainder)
                    empty[i].TransfinitePoints++;

                if (empty[i].TransfinitePoints < 2)
                    throw CreateTransfiniteException(
                        "Transfinite distribution assigned fewer than two points to a segment.",
                        sideIndex,
                        oppositeSideIndex,
                        segments,
                        target);

                controller.Gmsh.Model.Mesh.SetTransfiniteCurve(empty[i].Id, empty[i].TransfinitePoints);
            }

            var distributed = GetLogicalTransfinitePoints(segments);
            if (distributed != target)
                throw CreateTransfiniteException(
                    "Distributed transfinite point count does not match target.",
                    sideIndex,
                    oppositeSideIndex,
                    segments,
                    target);
        }

        private void DistributeAdditionalPoints(IReadOnlyList<GmshEdge> segments, int pointsToAdd)
        {
            if (pointsToAdd == 0)
                return;

            var baseValue = pointsToAdd / segments.Count;
            var remainder = pointsToAdd % segments.Count;

            for (var i = 0; i < segments.Count; ++i)
            {
                segments[i].TransfinitePoints += baseValue;
                if (i < remainder)
                    segments[i].TransfinitePoints++;

                controller.Gmsh.Model.Mesh.SetTransfiniteCurve(segments[i].Id, segments[i].TransfinitePoints);
            }
        }

        private void ValidateOppositeTransfinitePoints(int sideIndex, int oppositeSideIndex, int target)
        {
            var sidePoints = GetLogicalTransfinitePoints(segmentsLoop[sideIndex]);
            var oppositePoints = GetLogicalTransfinitePoints(segmentsLoop[oppositeSideIndex]);

            if (sidePoints != target || oppositePoints != target)
                throw new Exception(
                    "Opposite transfinite sides have incompatible point counts. " +
                    $"Side index: {sideIndex}. Opposite side index: {oppositeSideIndex}. " +
                    $"Configured TransfinitePoints: {TransfinitePoints}. Target points: {target}. " +
                    $"Side points: {sidePoints}. Opposite side points: {oppositePoints}.");
        }

        private static int GetLogicalTransfinitePoints(IReadOnlyList<GmshEdge> segments)
        {
            var total = 0;
            foreach (var segment in segments)
                total += segment.TransfinitePoints;

            return total - (segments.Count - 1);
        }

        private int GetOppositeSideIndex(int index)
        {
            if (index < 0 || index > 3)
                throw new ArgumentException("Диапазон ввода index должен быть от 0 до 3");

            return index ^ 2;
        }

        private int GetOppositeTransfinition(int index)
        {
            var opp = GetOppositeSideIndex(index);

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

        private Exception CreateTransfiniteException(
            string message,
            int sideIndex,
            int oppositeSideIndex,
            IReadOnlyList<GmshEdge> segments,
            int target)
        {
            var filledPoints = 0;
            var emptyCount = 0;
            foreach (var segment in segments)
            {
                if (segment.TransfinitePoints == 0)
                    emptyCount++;
                else
                    filledPoints += segment.TransfinitePoints - 2;
            }

            var required = segments.Count + 1 + filledPoints;

            return new Exception(
                $"{message} " +
                $"Side index: {sideIndex}. Opposite side index: {oppositeSideIndex}. " +
                $"Configured TransfinitePoints: {TransfinitePoints}. Required points: {required}. " +
                $"Target points: {target}. Segment count: {segments.Count}. " +
                $"Filled points: {filledPoints}. Empty segment count: {emptyCount}.");
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
