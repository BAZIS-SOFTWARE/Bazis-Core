using MathNet.Numerics.LinearAlgebra.Double;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Spatial.Euclidean;
using static OperationalController.MeshObjsCreator.CurveLoop.Curve;
using Geometry;
using MathNet.Numerics.Financial;

namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Замкнутый цикл кривых рассматриваемый относительно какой либо поверхности Gmsh
    /// </summary>
    public class CurveLoop
    {
        /// <summary>
        /// Кривая на поверхности
        /// </summary>
        public class Curve
        {
            private const int refPoints = 15;
            /// <summary>
            /// Квадрант местоположения точки
            /// </summary>
            [Flags]
            public enum Quadrant
            {
                Unknown = 0,
                Quadrant_0 = 1,
                Quadrant_3 = 2,
                Quadrant_1 = 4,
                Quadrant_2 = 8,
                Right = Quadrant_0 | Quadrant_3,
                Left = Quadrant_1 | Quadrant_2,
                Up = Quadrant_0 | Quadrant_1,
                Down = Quadrant_2 | Quadrant_3,
                All = Right | Left | Up | Down
            }
            /// <summary>
            /// Знаковая кривая, знак минус означает, что кривая должна рассматриваться инвертированым способом относительно ее создания
            /// </summary>
            public int Tag { get; private set; }
            /// <summary>
            /// Поверхность относительно которой рассматривается кривая
            /// </summary>
            public int SurfaceTag { get; private set; }
            /// <summary>
            /// Границы кривой (идентификаторы точек)
            /// </summary>
            public int[] Bounds { get; private set; }
            /// <summary>
            /// Квадранты точек и их координаты в трехмерном пространстве
            /// </summary>
            public (Quadrant[], double[]) Coords { get; private set; }
            /// <summary>
            /// Параметрические (2D) координаты
            /// </summary>
            public double[] ParametricCoords { get; private set; }
            /// <summary>
            /// Конструктор кривой относительно плоскости
            /// </summary>
            /// <param name="tag">Знаковый идентификатор кривой</param>
            /// <param name="surfaceTag">Идентификатор поверхности</param>
            public Curve(int tag, int surfaceTag)
            {
                Tag = tag;
                SurfaceTag = surfaceTag;
                Bounds = GetBounds();

                CreateCoordinates();
            }

            public Curve(int tag, int surfaceTag, (Quadrant[], double[]) coords, double[] uv)
            {
                Tag = tag; 
                SurfaceTag = surfaceTag;
                Bounds = GetBounds();

                Coords = coords;
                ParametricCoords = uv;
            }
            /// <summary>
            /// Возвращает смежную поверхность в которой находится кривая
            /// </summary>
            /// <returns></returns>
            public int GetAdjacentSurface()
            {
                var absTag = Math.Abs(Tag);
                var surface = controller.Gmsh.Model.GetAdjacencies(1, absTag).Item1
                                                   .Where(v => v != SurfaceTag)
                                                   .FirstOrDefault();
                return surface;
            }

            /// <summary>
            /// Возвращает смежную кривую, которая лежит в смежной поверхности
            /// </summary>
            /// <param name="adjacentSurface">Смежная поверхность</param>
            /// <returns>Смежная кривая</returns>
            public Curve GetAdjacent(int adjacentSurface)
            {

                var absTag = Math.Abs(Tag);
                var curveTag = controller.Gmsh.Model.GetBoundary([2, adjacentSurface], false).Where((v,i) => (i & 1) == 1)
                                                                                             .Where(v => Math.Abs(v) == absTag)
                                                                                             .First();
                return new Curve(curveTag, adjacentSurface);
            }
            /// <summary>
            /// Возвращает границы (идентификаторы точек) для ориентированной кривой
            /// </summary>
            /// <returns>Идентификаторы точек</returns>
            private int[] GetBounds()
            {
                return controller.Gmsh.Model.GetBoundary([1, Tag], false)
                                            .Where((v, i) => (i & 1) == 1)
                                            .ToArray();
            }

            /// <summary>
            /// Инвертирует кривую меняя знаковый идентификатор, меняет параметрические и 3d координаты а также квадранты точек
            /// </summary>
            public void Reverse()
            {
                var quads = Coords.Item1.Reverse().ToArray();
                ParametricCoords = GetReversedCoords(ParametricCoords, 2);
                Coords = (quads, GetReversedCoords(Coords.Item2));
                Swap(ref Bounds[0], ref Bounds[1]);

                Tag = -Tag;
            }
            /// <summary>
            /// Определяет невозможность совмещения двух кривых (вычисление усредненной кривой)
            /// </summary>
            /// <param name="a">Кривая a</param>
            /// <param name="b">Кривая b</param>
            /// <returns>Статус невозможности совмещения</returns>
            public static bool operator !=(Curve a, Curve b)
            {
                //Подходит только для кривых из CurveLoop

                if(a.SurfaceTag != b.SurfaceTag)
                    return true;

                var aQuads = a.Coords.Item1;
                var bQuads = b.Coords.Item1;

                return !IsOppositePoint(aQuads[0], bQuads[bQuads.Length - 1]);
            }
            /// <summary>
            /// Определяет возможность совмещения двух кривых (вычисление усредненной кривой)
            /// </summary>
            /// <param name="a">Кривая a</param>
            /// <param name="b">Кривая b</param>
            /// <returns>Статус возможности совмещения</returns>
            public static bool operator ==(Curve a, Curve b) => !(a != b);
            /// <summary>
            /// Совмещает две кривые (возвращает усредненную кривую, выбирает направление кривой a)
            /// </summary>
            /// <param name="a">Кривая a</param>
            /// <param name="b">Кривая b</param>
            /// <exception cref="Exception">Количество локальных, параметрических координат и квадрантов должно совпадать</exception>
            /// <returns>Усредненная кривая</returns>
            public static Curve operator +(Curve a, Curve b)
            {
                /*if (a != b)
                    return null;*/
                if (a.ParametricCoords.Length != b.ParametricCoords.Length)
                    throw new Exception("Количество параметрических координат должно совпадать");
                if (a.Coords.Item1.Length != b.Coords.Item1.Length)
                    throw new Exception("Количество квадрантов должно совпадать");
                if (a.Coords.Item2.Length != b.Coords.Item2.Length)
                    throw new Exception("Количество локальных координат должно совпадать");

                var length = a.Coords.Item1.Length;
                var quads = new Quadrant[length];
                Array.Copy(a.Coords.Item1, quads, length);

                var coords = AverageCoords(a.Coords.Item2, b.Coords.Item2, 3);
                var uv = AverageCoords(a.ParametricCoords, b.ParametricCoords, 2);
                //TODO: Через параметрические получить локальные!

                var newCurve = new Curve(a.Tag, a.SurfaceTag, (quads, coords), uv);

                return newCurve;
            }

            private static double[] AverageCoords(double[] src0, double[] src1, int attribs)
            {
                var length = src0.Length / attribs;
                var dst = new double[src0.Length];

                for (var i = 0; i < length; ++i)
                {
                    var srcStride = src0.Length - (i + 1) * attribs;
                    var dstStride = i * attribs;

                    for (var j = 0; j < attribs; ++j)
                        dst[dstStride + j] = (src0[srcStride + j] + src1[srcStride + j]) * 0.5;
                }

                return dst;
            }

            private static bool IsOppositePoint(Quadrant a, Quadrant b)
            {
                var min = Math.Min((int)a, (int)b);
                var max = Math.Max((int)a, (int)b);

                return min << 1 == max;
            }

            private double[] GetReversedCoords(double[] src, int attribs = 3)
            {
                var dst = new double[src.Length];
                var count = src.Length / attribs;

                for (var i = 0; i < count; ++i)
                {
                    var dstStride = i * attribs;
                    var srcStride = src.Length - (i + 1) * attribs;

                    for(var j = 0; j < attribs; ++j)
                        dst[dstStride + j] = src[srcStride + j];
                }

                return dst;
            }

            private void CreateCoordinates()
            {
                var absTag = Math.Abs(Tag);
                var t = new double[refPoints];
                var tMinMax = controller.Gmsh.Model.GetParametrizationBounds(1, absTag);

                if (Tag < 0)
                    Swap(ref tMinMax.Item1[0], ref tMinMax.Item2[0]);
                var offset = (tMinMax.Item2[0] - tMinMax.Item1[0]) / (refPoints - 1);

                for (var j = 0; j < refPoints; ++j)
                    t[j] = tMinMax.Item1[0] + j * offset;

                ParametricCoords = controller.Gmsh.Model.ReparametrizeOnSurface(1, absTag, t, SurfaceTag);
                //Coords = GetCoordinates();
            }

            private Quadrant GetQuadrant(double[] center, bool isBeginPoint = false)
            {
                var p = controller.Gmsh.Model.GetParametrizationBounds(2, SurfaceTag);
                var n = controller.Gmsh.Model.GetNormal(SurfaceTag, [(p.Item1[0] + p.Item2[0]) * 0.5, (p.Item1[1] + p.Item2[1]) * 0.5]);

                var coords = Coords.Item2;
                var last = coords.Length - 3;
                var begin = 0;
                if (Tag < 0)
                    Swap(ref begin, ref last);
                var point = isBeginPoint ? new double[] { coords[begin], coords[begin + 1], coords[begin + 2] }
                                         : [coords[last], coords[last + 1], coords[last + 2]];

                var vec = new Vector3D(point[0] - center[0], point[1] - center[1], point[2] - center[2]);
                var vecLen = vec.Length;
                var cos = UnitVector3D.XAxis.DotProduct(vec) / vecLen;

                var normal = new Vector3D(n[0], n[1], n[2]);
                var mix = normal.DotProduct(UnitVector3D.XAxis.CrossProduct(vec));
                var sin = mix / vecLen;

                if (cos >= 0)
                    return sin >= 0 ? Quadrant.Quadrant_0 : Quadrant.Quadrant_3;
                else
                    return sin >= 0 ? Quadrant.Quadrant_1 : Quadrant.Quadrant_2;
            }

            private (Quadrant[], double[]) GetCoordinates()
            {
                var coords = controller.Gmsh.Model.GetValue(2, SurfaceTag, ParametricCoords);
                var quads = new Quadrant[coords.Length  / 3];
                var pc = ParametricCoords;

                for (var i = 0; i < quads.Length; ++i)
                {
                    var stride = i << 1;
                    if (pc[stride] >= 0 && pc[stride + 1] < 0)
                        quads[i] = Quadrant.Quadrant_0;
                    else if (pc[stride] >= 0 && pc[stride + 1] >= 0)
                        quads[i] = Quadrant.Quadrant_1;
                    else if (pc[stride] < 0 && pc[stride + 1] >= 0)
                        quads[i] = Quadrant.Quadrant_2;
                    else if (pc[stride] < 0 && pc[stride + 1] < 0)
                        quads[i] = Quadrant.Quadrant_3;
                }
                return (quads, coords);
            }

            public static void Swap<T>(ref T a, ref T b)
            {
                T tmp = a; 
                a = b;
                b = tmp;
            }
        }

        private static IGmshController controller;
        /// <summary>
        /// Ориентация кривой (против часовой\ по часовой)
        /// </summary>
        public enum Orientation
        {
            /// <summary>
            /// Смешанное (кривые построены на разных гранях)
            /// </summary>
            Unorder,
            /// <summary>
            /// Против часовой
            /// </summary>
            Ccw,
            /// <summary>
            /// По часовой
            /// </summary>
            Cw
        }
        /// <summary>
        /// Поверхность, которой принадлежит цикл кривых
        /// </summary>
        public int Surface {  get; private set; }
        /// <summary>
        /// Порядок обхода петли кривых
        /// </summary>
        public Orientation Order { get; private set; }
        /// <summary>
        /// Список циклов, первый внешний, остальные отверстия
        /// </summary>
        public LinkedList<Curve> Loop { get; private set; }
        /// <summary>
        /// Отверстия
        /// </summary>
        public Dictionary<string, CurveLoop> Holes { get; private set; }
        /// <summary>
        /// Установить флаг проверки граничных точек кривых при вызове метода IsOk
        /// </summary>
        public bool CheckCurveBounds { get; set; } = true;
        /// <summary>
        /// Цикл кривых относительно поверхности
        /// </summary>
        /// <param name="controller">Контроллер gmsh</param>
        /// <param name="surface">Поверхность</param>
        public CurveLoop(IGmshController controller, int surface)
        {
            CurveLoop.controller = controller;
            Surface = surface;
            Loop = new LinkedList<Curve>();
            Holes = new Dictionary<string, CurveLoop>();

            var loops = GetOrientedCurveLoops(surface);
            foreach (var curveId in loops[0])
            {
                var curve = new Curve(curveId, Surface);
                Loop.AddLast(curve);
            }
            Order = GetOrientation(Loop);

            for (var i = 1; i < loops.Length; ++i)
            {
                var hole = new CurveLoop(controller, Surface, loops[i]);
                var key = hole.GetAdjacentSurfaceLoopKey(hole.Loop);
                Holes.Add(key, hole);
            }
        }
        /// <summary>
        /// Возвращает усредненный CurveLoop
        /// </summary>
        /// <param name="master"></param>
        /// <param name="slave"></param>
        /// <returns></returns>
        public static CurveLoop operator +(CurveLoop master, CurveLoop slave)
        {
            master.MakeSimilar(slave);
            master.MakeSimilarHoles(slave);

            var mLoop = master.Loop.First;
            var sLoop = slave.Loop.First;

            var loop = new List<Curve>();
            AverageCurves(mLoop, sLoop, loop);

            var dict = new Dictionary<string, List<Curve>>();
            foreach (var kvp in master.Holes)
            {
                if(slave.Holes.ContainsKey(kvp.Key))
                {
                    var current = new List<Curve>();

                    mLoop = kvp.Value.Loop.First;
                    sLoop = slave.Holes[kvp.Key].Loop.First;

                    AverageCurves(mLoop, sLoop, current);
                    dict.Add(kvp.Key, current);
                }
            }

            return new CurveLoop(controller, master.Surface, loop, dict);
        }

        /// <summary>
        /// Сделать подобным цикл slave 
        /// </summary>
        /// <param name="slave">Цикл</param>
        /// <exception cref="Exception">Число кривых цикла должно быть одинаково, ориентация только одновременно неупордоченных или ориентированных</exception>
        public void MakeSimilar(CurveLoop slave)
        {
            var sLoop = slave.Loop;

            if (Order == Orientation.Unorder && slave.Order > Orientation.Unorder ||
                Order > Orientation.Unorder && slave.Order == Orientation.Unorder)
                throw new Exception("Один из циклов является неупорядоченным, два цикла могут быть только одновременно быть неупордоченными");

            if (Loop.Count != sLoop.Count)
                throw new Exception("Один из циклов является неупорядоченным, два цикла могут быть только одновременно быть неупордоченными");

            //var firstQuad = Loop.First.Value.Coords.Item1[0];
            var c = GetCenterOfMass(Loop);
            var firstQuad = GetPointOrientation(Loop.First.Value, c, true);
            if (firstQuad != sLoop.First.Value.Coords.Item1[0])
            {
                var actual = slave.SearchQuadrant(firstQuad);
                if (actual == null)
                    throw new Exception($"Не могу найти противоположную точку в квадранте {firstQuad.ToString()}");
                if (Order != slave.Order)
                    slave.Reverse(actual);
                else
                    slave.Reorder(actual);
            }
            else if (Order != slave.Order)
                slave.Reverse();

            MakeSimilarHoles(slave);
        }

        /// <summary>
        /// Возвращает смежный цикл - кривые исходного цикла будут перепараметризированы на смежные поверхности
        /// </summary>
        /// <returns>Смежный цикл</returns>
        public CurveLoop GetAdjacentCurveLoop()
        {
            var loop = new List<Curve>();

            foreach (var curve in Loop)
            {
                //КРИВАЯ МОЖЕТ БЫТЬ В ОДНОЙ ЕДИНСТВЕННОЙ ПОВЕРХНОСТИ
                var adjS = curve.GetAdjacentSurface();//Брать смежную поверхность
                var adj = adjS == 0 ? curve : curve.GetAdjacent(adjS);//Узнать знак внутри 
                loop.Add(adj);
            }

            var holes = new Dictionary<string, List<Curve>>();
            foreach (var kvp in Holes)
            {
                var current = new List<Curve>();
                foreach (var hole in kvp.Value.Loop)
                {
                    var adjS = hole.GetAdjacentSurface();
                    var adj = hole.GetAdjacent(adjS);
                    current.Add(adj);
                }
                holes.Add(kvp.Key, current);
            }

            return new CurveLoop(controller, 0, loop, holes);
        }
        /// <summary>
        /// Построить gmsh CurveLoop
        /// </summary>
        /// <exception cref="Exception">OCC Exceptions</exception>
        public void BuildCurveLoop()
        {
            var pointSet = new Dictionary<int, int>();
            var curveSet = new Dictionary<int, int>();

            var CreatePoint = (double x, double y, double z) =>
            {
                var tag = controller.Gmsh.Model.Occ.AddPoint(x, y, z);
                if (tag == 0)
                    throw new Exception(controller.Gmsh.Logger.GetLastError());
                return tag;
            };

            var CreateBoundPoint = (Curve curve, bool isBegin = true) =>
            {
                var bounds = curve.Bounds;
                var bound = isBegin ? bounds[0] : bounds[1];

                if (!pointSet.ContainsKey(bound))
                {
                    var coords = curve.Coords.Item2;
                    var stride = isBegin ? 0 : coords.Length - 3;

                    pointSet.Add(bound, CreatePoint(coords[stride], coords[stride + 1], coords[stride + 2]));
                }
                return pointSet[bound];
            };

            var BuildLoop = (LinkedList<Curve> loop) =>
            {
                var curveList = new List<int>();
                foreach (var curve in loop)
                {
                    var absCurve = Math.Abs(curve.Tag); ;
                    if (!curveSet.ContainsKey(absCurve))
                    {
                        var tempPoints = new List<int>() { CreateBoundPoint(curve) };
                        var coords = curve.Coords.Item2;

                        for (var i = 3; i < coords.Length - 3; i += 3)
                            tempPoints.Add(CreatePoint(coords[i], coords[i + 1], coords[i + 2]));

                        tempPoints.Add(CreateBoundPoint(curve, false));
                        var curveTag = controller.Gmsh.Model.Occ.AddSpline(tempPoints.ToArray());
                        controller.Gmsh.Model.Occ.Remove(controller.Gmsh.Model.Occ.GetEntities(0));
                        curveSet.Add(absCurve, curveTag);
                    }
                    curveList.Add(curveSet[absCurve]);
                }
            };

            BuildLoop(Loop);
            foreach(var loop in Holes.Values)
                BuildLoop(loop.Loop);

        }
        /// <summary>
        /// Для каждой кривой цикла находит квадрант, выполняет поиск по локальным координатам
        /// </summary>
        public static List<List<Quadrant>> GetPointQuadrants()
        {
            //1.Расчет центра масс (берутся все точки кривой а не только граничные)
            //2.А вот для выявлении ориентации точки берутся только граничные точки кривой, причем только последняя
            var quads = new List<List<Quadrant>>();
            var external = new List<Quadrant>();

            

            return null;
        }

        public static Quadrant GetPointOrientation(Curve curve, double[] center, bool isBeginPoint = false)
        {
            var p = controller.Gmsh.Model.GetParametrizationBounds(2, curve.SurfaceTag);
            var n = controller.Gmsh.Model.GetNormal(curve.SurfaceTag, [(p.Item1[0] + p.Item2[0]) * 0.5, (p.Item1[1] + p.Item2[1]) * 0.5]);

            var coords = curve.Coords.Item2;
            var last = coords.Length - 3;
            var begin = 0;
            if (curve.Tag < 0)
                Swap(ref begin, ref last);
            var point = isBeginPoint ? new double[] { coords[begin], coords[begin + 1], coords[begin +2] }
                                     : [coords[last], coords[last + 1], coords[last + 2]];

            var vec = new Vector3D(point[0] - center[0], point[1] - center[1], point[2] - center[2]);
            var vecLen = vec.Length;
            var cos = UnitVector3D.XAxis.DotProduct(vec) / vecLen;

            var normal = new Vector3D(n[0], n[1], n[2]);
            var mix = normal.DotProduct(UnitVector3D.XAxis.CrossProduct(vec));
            var sin = mix / vecLen;

            if (cos >= 0)
                return sin >= 0 ? Quadrant.Quadrant_0 : Quadrant.Quadrant_3;
            else
                return sin >= 0 ? Quadrant.Quadrant_1 : Quadrant.Quadrant_2;
        }

        public static double[] GetCenterOfMass(LinkedList<Curve> loop)
        {
            var center = new double[3];
            var points = 0;
            foreach(var curve in loop)
            {
                var coords = curve.Coords.Item2;
                points += coords.Length / 3;
                for (var i = 0; i < coords.Length; i += 3)
                    for (var j = 0; j < 3; ++j)
                        center[j] += coords[i + j];
            }
            for (var i = 0; i < 3; ++i)
                center[i] /= points;

            return center;
        }

        /// <summary>
        /// Специальный конструктор для построения ориентированных отверстий
        /// </summary>
        /// <param name="controller">Контроллер gmsh</param>
        /// <param name="surface">Поверхность</param>
        /// <param name="loop">Цикл кривых</param>
        private CurveLoop(IGmshController controller, int surface, int[] loop)
        {
            CurveLoop.controller = controller;
            Surface = surface;
            Loop = new LinkedList<Curve>();
            Holes = new Dictionary<string, CurveLoop>();

            foreach (var curveId in loop)
            {
                var curve = new Curve(curveId, Surface);
                Loop.AddLast(curve);
            }

            Order = GetOrientation(Loop);
        }
        /// <summary>
        /// Специальный конструктор для создания неориентированных циклов кривых
        /// </summary>
        /// <param name="controller">Контроллер gmsh</param>
        /// <param name="surface">Поверхность</param>
        /// <param name="loop">Внешний контур</param>
        /// <param name="holes">Контуры отверстий</param>
        private CurveLoop(IGmshController controller, int surface, List<Curve> loop, Dictionary<string, List<Curve>> holes)
        {
            CurveLoop.controller = controller;
            Surface = surface;
            Order = Orientation.Unorder;
            Holes = new Dictionary<string, CurveLoop>();
            Loop = Sort(loop);

            foreach (var kvp in holes)
            {
                var hole = new CurveLoop(controller, Surface, kvp.Value);
                Holes.Add(kvp.Key, hole);
            }
            CheckCurveBounds = false;
        }
        /// <summary>
        /// Специальный конструктор для построения неориентированных отверстий
        /// </summary>
        /// <param name="controller">Контроллер gmsh</param>
        /// <param name="surface">Поверхность</param>
        /// <param name="holes">Отверстия</param>
        private CurveLoop(IGmshController controller, int surface, List<Curve> holes)
        {
            CurveLoop.controller = controller;
            Surface = surface;
            Order = Orientation.Unorder;
            Loop = Sort(holes);
            Holes = new Dictionary<string, CurveLoop>();
            CheckCurveBounds = false;
        }

        private static void AverageCurves(LinkedListNode<Curve> mLoop, LinkedListNode<Curve> sLoop, List<Curve> current)
        {
            while (mLoop != null)
            {
                var newCurve = mLoop.Value + sLoop.Value;
                current.Add(newCurve);

                mLoop = mLoop.Next;
                sLoop = sLoop.Next;
            }
        }


        private void MakeSimilarHoles(CurveLoop slave)
        {
            foreach(var kvp in Holes)
            {
                var mCl = kvp.Value;

                if(slave.Holes.ContainsKey(kvp.Key))
                {
                    var sCl = slave.Holes[kvp.Key];
                    if (mCl.Order == Orientation.Unorder && sCl.Order > Orientation.Unorder ||
                        mCl.Order > Orientation.Unorder && sCl.Order == Orientation.Unorder)
                        throw new Exception("Один из циклов является неупорядоченным, два цикла могут быть только одновременно быть неупордоченными");

                    var mLoop = mCl.Loop;
                    var sLoop = sCl.Loop;

                    if (mLoop.Count != sLoop.Count)
                        throw new Exception($"У поверхности {Surface} {mLoop.Count} кривых, " +
                                            $"у поверхности {slave.Surface} {sLoop.Count} кривыx. Количество кривых должно совпадать.");

                    var firstQuad = mLoop.First.Value.Coords.Item1[0];
                    if (firstQuad != sLoop.First.Value.Coords.Item1[0])
                    {
                        var actual = SearchQuadrant(firstQuad);
                        if (actual == null)
                            throw new Exception($"Не могу найти противоположную точку в квадранте {firstQuad.ToString()}");
                        if (mCl.Order != sCl.Order)
                            sCl.Reverse(actual);
                        else
                            sCl.Reorder(actual);
                    }
                    else if (mCl.Order != sCl.Order)
                        sCl.Reverse();
                }
            }
        }

        private string GetAdjacentSurfaceLoopKey(LinkedList<Curve> curves)
        {
            var sb = new StringBuilder();
            foreach (var curve in curves)
            {
                var adj = curve.GetAdjacentSurface();

                sb.Append(adj);
                sb.Append(' ');
            }
            return sb.ToString(0, sb.Length - 1);
        }

        private LinkedListNode<Curve> SearchQuadrant(Quadrant reference)
        {
            var current = Loop.First;
            while (current != null)
            {
                var curve = current.Value;
                var quad = curve.Coords.Item1[0];

                if (reference == quad)
                    return current;

                current = current.Next;
            }
            return null;
        }

        private void Reverse(LinkedListNode<Curve> node = null)
        {
            node = node ?? Loop.First;
            var loop = new LinkedList<Curve>();

            var current = node;
            while (current != null)
            {
                var curve = current.Value;
                curve.Reverse();

                var newNode = loop.AddLast(curve);
                current = current.Previous;
            }

            current = Loop.Last;
            while (current != node)
            {
                var curve = current.Value;
                curve.Reverse();

                var newNode = loop.AddLast(curve);
                current = current.Previous;
            }

            Loop = loop;

            Order = Order == Orientation.Cw ? Orientation.Ccw : Orientation.Cw;
        }

        private void Reorder(LinkedListNode<Curve> node = null)
        {
            node = node ?? Loop.First;
            var loop = new LinkedList<Curve>();

            var current = node;
            while (current != null)
            {
                var curve = current.Value;
                var newNode = loop.AddLast(curve);

                current = current.Next;
            }

            current = Loop.First;
            while (current != node)
            {
                var curve = current.Value;
                var newNode = loop.AddLast(curve);

                current = current.Next;
            }

            Loop = loop;
        }
        //Сортировка опасная - может быть вечный цикл
        private LinkedList<Curve> Sort(List<Curve> curves)
        {
            var end = new Dictionary<string, (int, Curve)>();
            var loop = new LinkedList<Curve>();

            for (var i = 0; i < curves.Count; ++i)
            {
                var coords = curves[i].Coords.Item2;
                var last = coords.Length / 3 - 1;
                var endKey = CoordsToString(coords, last, 3);
                var beginKey = CoordsToString(coords, 0, 3);

                if (!end.ContainsKey(endKey))
                    end.Add(endKey, (Convert.ToInt32(beginKey == endKey), curves[i]));
                else if(endKey != beginKey)
                {
                    if (!end.ContainsKey(beginKey))
                    {
                        curves[i].Reverse();
                        end.Add(beginKey, (0, curves[i]));
                    }
                    else if (end[endKey].Item1 != 1)
                        DirectionSort(end, endKey);
                }
                loop.AddFirst(curves[i]);
            }
    
            /*
            var loop = new LinkedList<Curve>();

            var begin = curves.ToDictionary(k => CoordsToString(k.Coords.Item2, 0, 3), v => v);
            var crds = curves[0].Coords.Item2;
            var key = CoordsToString(crds, crds.Length / 3 - 1, 3); 

            for (var i = 0; i < curves.Count; ++i)
            {
                var current = begin[key];
                crds = current.Coords.Item2;
                loop.AddLast(current);
                key = CoordsToString(crds, crds.Length / 3 - 1, 3);
            }*/

            return loop;
        }

        private void DirectionSort(Dictionary<string, (int, Curve)> end, string endKey)
        {
            while (endKey.Contains(endKey))
            {
                var curve = end[endKey];
                curve.Item2.Reverse();
                var coords = curve.Item2.Coords.Item2;
                var last = coords.Length / 3 - 1;
                endKey = CoordsToString(coords, last, 3);
            }
        }

        private string CoordsToString(double[] coords, int index, int attribs)
        {
            var sb = new StringBuilder();
            var stride = index * attribs;
            for (var i = 0; i < 3; ++i)
            {
                sb.Append((int)Math.Round(coords[stride + i] * 1000));
                sb.Append(' ');
            }
            return sb.ToString(0, sb.Length - 1);
        }

        /// <summary>
        /// Проверяет все петли кривых на корректность построения
        /// </summary>
        /// <param name="message">Сообщение ошибки</param>
        /// <returns>true если петля кривых валидна</returns>
        public bool IsOk(ref string message)
        {
            if (!IsOk(Loop, Order, ref message))
                return false;

            foreach(var kvp in Holes)
                if (!IsOk(kvp.Value.Loop, kvp.Value.Order, ref message))
                    return false;

            return true;
        }

        private bool IsOk(LinkedList<Curve> loop, Orientation orientation, ref string message)
        {
            var current = loop.First;
            while (current != null)
            {
                if (current.Previous != null)
                {
                    var actual = current.Value;
                    var last = current.Previous.Value;

                    if (!CheckCurves(actual, last, ref message, orientation != Orientation.Unorder))
                        return false;
                }
                if (current.Next == null)
                {
                    if (!CheckCurves(loop.First.Value, current.Value, ref message, orientation != Orientation.Unorder))
                        return false;
                }

                current = current.Next;
            }
            return true;
        }

        /// <summary>
        /// Строит ориентированный цикл кривых (внешний и внутренние(отверстия))
        /// </summary>
        /// <param name="surface">Поверхность на которую опирается цикл кривых</param>
        /// <returns>Массив массивов ориентированных кривых</returns>
        public static int[][] GetOrientedCurveLoops(int surface)
        {
            var curveCounts = controller.Gmsh.Model.Occ.GetCurveLoops(surface).Item2.Select(v => v.Length).ToList();
            var curves = controller.Gmsh.Model.GetBoundary([2, surface], false);

            var start = 0;
            var loops = new int[curveCounts.Count][];

            for(var i = 0; i < curveCounts.Count; ++i)
            {
                var elements = curveCounts[i];
                loops[i] = new int[elements];
                var end = start + (elements << 1);
                var inCounter = 0;

                for(var j = start; j < end ; j += 2)
                    loops[i][inCounter++] = curves[j + 1];
                start += (elements << 1);
            }
            
            return loops;

            //Исправлять! Может быть одна единственная кривая!
            /*var curves = controller.Gmsh.Model.GetBoundary([2, surface], false);
            var points = controller.Gmsh.Model.GetBoundary(curves, false);

            var loops = new List<List<int>>();
            var current = new List<int>();
            for (var i = 0; i < points.Length; i +=4)
            {
                var curveStride = (i >> 1) + 1;
                if(i - 1 > 0 && points[i - 1] != points[i + 1])
                {
                    loops.Add(current);
                    current = new List<int>();
                }
                current.Add(curves[curveStride]);
            }
            loops.Add(current);

            var result = loops.Select(v => v.ToArray()).ToArray();
            return result;*/

            /*var b = controller.Gmsh.Model.GetBoundary([2, surface], false);
            var s = controller.Gmsh.Model.Occ.GetCurveLoops(surface).Item2;


            var bounds = controller.Gmsh.Model.GetBoundary([2, surface], false)
                                              .Where((v,i) => (i & 1) == 1)
                                              .ToDictionary(k => Math.Abs(k), v => v);

            return controller.Gmsh.Model.Occ.GetCurveLoops(surface).Item2.Select(v => v.Select(k => bounds[k]).ToArray())
                                                                         .ToArray();*/
        }

        private bool CheckCurves(Curve actual, Curve last, ref string message, bool checkAllParams = true)
        {
            var cBound = actual.Bounds;
            var pBound = last.Bounds;

            if (CheckCurveBounds && pBound[1] != cBound[0])
            {
                message = $"Идентификатор предыдущей кривой {last.Tag} = {pBound[1]}, идентификатор текущей  кривой {actual.Tag} = {cBound[0]}";
                return false;
            }

            var cCoords = actual.Coords;
            var pCoords = last.Coords;

            if (!CheckCoords(cCoords.Item2, pCoords.Item2, 3))
            {
                message = $"Координаты предыдущей кривой {last.Tag} в точке {pBound[1]}" +
                          $" не совпадает с координатами текущей  кривой {actual.Tag} = {cBound[0]}";
                return false;
            }
            if (checkAllParams)
            {
                if (!CheckCoords(actual.ParametricCoords, last.ParametricCoords, 2))
                {
                    message = $"Параметрические координаты предыдущей кривой {last.Tag} в точке {pBound[1]}" +
                              $" не совпадает с параметрическими координатами текущей  кривой {actual.Tag} в точке {cBound[0]}";
                    return false;
                }
                if (actual.Coords.Item1[0] != pCoords.Item1[pCoords.Item1.Length - 1])
                {
                    message = $"Квадрант предыдущей кривой {last.Tag} в точке {pBound[1]} = {pCoords.Item1[pCoords.Item1.Length - 1].ToString()}" +
                              $" не совпадает с квадрантом текущей  кривой {actual.Tag} в точке {cBound[0]} = {actual.Coords.Item1[0].ToString()}";
                    return false;
                }
            }
            return true;
        }

        private bool CheckCoords(double[] current, double[] prev, int attribs)
        {
            var pLast = prev.Length - attribs;

            for (var i = 0; i < attribs; ++i)
                if (Math.Abs(current[i] - prev[pLast + i]) > 1e-2)
                    return false;

            return true;
        }

        private Orientation GetOrientation(LinkedList<Curve> loop)
        {
            if (loop.Count > 1)
            {
                var first = loop.First;
                var second = first.Next;

                var fUv = first.Value.ParametricCoords;
                var sUv = second.Value.ParametricCoords;

                var fLast = fUv.Length - 2;
                var sLast = sUv.Length - 2;

                var matrix = new DenseMatrix(2);

                matrix.SetColumn(0, [fUv[fLast] - fUv[0], fUv[fLast + 1] - fUv[1]]);
                matrix.SetColumn(1, [sUv[sLast] - sUv[0], sUv[sLast + 1] - sUv[1]]);

                var det = matrix.Determinant();

                return det > 0 ? Orientation.Cw : Orientation.Ccw;
            }
            else
                return loop.First.Value.Tag > 0 ? Orientation.Cw : Orientation.Ccw;
        }
    }
}
