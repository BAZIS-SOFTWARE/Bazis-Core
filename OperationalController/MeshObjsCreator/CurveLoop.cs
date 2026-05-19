using MathNet.Spatial.Euclidean;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OperationalController.MeshObjsCreator
{
    public class CurveLoop
    {
        private int pointsPerCurve;
        private IGmshController controller;
        public class CurveLoopNode
        {
            public int Begin { get; private set; }
            public int End { get; private set; }

            public double[] AverageCoords { get; private set; }
            /// <summary>
            /// Идентификатор кривой со знаком, знак показывает что кривая должна рассматриваться инвертированно ее созданию (параматризация с конца)
            /// </summary>
            public int Tag { get; private set; }

            public CurveLoopNode(int begin, int end, int tag)
            {
                Begin = begin;
                End = end;
                Tag = tag;
            }

            public void SetAverageCoords(double[] coords) => AverageCoords = coords;

            public double[] GetParametrization(IGmshController controller, int pointsPerCurve = 16)
            {
                var absTag = Math.Abs(Tag);
                var bounds = controller.Gmsh.Model.GetParametrizationBounds(1, absTag);
                if (Tag < 0)
                    Swap(ref bounds.Item1[0], ref bounds.Item2[0]);

                var result = new double[pointsPerCurve];
                var step = (bounds.Item2[0] - bounds.Item1[0]) / (pointsPerCurve - 1);

                for (var i = 0; i < result.Length; ++i)
                    result[i] = bounds.Item1[0] + step * i;

                return result;
            }

            public void Reverse()
            {
                Tag = -Tag;
                Swap();
            }

            private void Swap<T>(ref T a, ref T b)
            {
                var temp = a;
                a = b;
                b = temp;
            }

            private void Swap()
            {
                var temp = Begin;
                Begin = End;
                End = temp;
            }
        }

        /// <summary>
        /// Ориентация (по часовой\ против часовой стрелки)
        /// </summary>
        public enum Orient
        {
            CW = -1,
            CCW = 1
        }
        //public double[] AverageCoords { get; private set; }
        public LinkedList<CurveLoopNode> Loop { get; private set; }
        public Orient Orientation { get; private set; }
        public int Surface { get; private set; }

        /// <summary>
        /// Конструктор для построения внутреннего замкнутого контура - отверстие
        /// </summary>
        /// <param name="controller">Контроллер Gmsh</param>
        /// <param name="surface">Поверхность которой принадлежит петля</param>
        /// <param name="index">Индекс петли</param>
        public CurveLoop(IGmshController controller, int surface, int index, int pointsPerCurve = 16)
        {
            this.pointsPerCurve = pointsPerCurve;
            this.controller = controller;
            Loop = new LinkedList<CurveLoopNode>();
            Surface = surface;
            BuildLoop(index);
            CheckLoop();
            Orientation = GetOrientation();
        }

        /// <summary>
        /// Проверка построения петли
        /// </summary>
        /// <exception cref="Exception">Возникает в случае если две кривые не соединены между собой</exception>
        public void CheckLoop()
        {
            if (Loop.First.Value.Begin != Loop.Last.Value.End)
                throw new Exception($"Кривая {Math.Abs(Loop.First.Value.Tag)} и кривая {Math.Abs(Loop.Last.Value.Tag)} не соединены общей точкой");
            var current = Loop.First;
            while (current != null)
            {
                var prev = current.Previous;
                if (prev != null)
                    if (prev.Value.End != current.Value.Begin)
                        throw new Exception($"Кривая {Math.Abs(prev.Value.Tag)} и кривая {Math.Abs(current.Value.Tag)} не соединены общей точкой");
                current = current.Next;
            }
        }

        /// <summary>
        /// Сделать подобной src текущую петлю 
        /// </summary>
        /// <param name="src">Петля источник</param>
        /// <exception cref="Exception">Возникает в случае разного числа кривых у двух поверхностей</exception>
        public void MakeSimilar(CurveLoop src)
        {
            if (src.Loop.Count != Loop.Count)
                throw new Exception($"Геометрическая поверхность {Surface} и {src.Surface} имеет разное число кривых");
            if (src.Orientation != Orientation)
            {
                var begin = src.Loop.First.Value.Begin;
                var coords = controller.Gmsh.Model.GetValue(0, begin, []);
                Reverse(coords);
            }
            else
            {
                var begin = src.Loop.First.Value.Begin;
                var coords = controller.Gmsh.Model.GetValue(0, begin, []);
                Rebuild(coords);
            }
        }

        /*
        /// <summary>
        /// Усредняем поврехности двух контуров, работает только на 4 кривых
        /// </summary>
        /// <param name="similarSlave">Починенный контур, должен быть приведен к подобному вызовом MakeSimilar</param>
        public void AverageSurfaces(CurveLoop similarSlave)
        {
            if (Loop.Count != 4 || similarSlave.Loop.Count != 4)
                return;

            var master = Loop.First;
            var slave = similarSlave.Loop.First;

            var mCurves = new List<double[]>();
            var sCurves = new List<double[]>();

            while (master != null)
            {
                var mParams = master.Value.GetParametrization(controller, pointsPerCurve);
                var sParams = slave.Value.GetParametrization(controller, pointsPerCurve);

                mParams = controller.Gmsh.Model.ReparametrizeOnSurface(1, Math.Abs(master.Value.Tag), mParams, Surface);
                sParams = controller.Gmsh.Model.ReparametrizeOnSurface(1, Math.Abs(slave.Value.Tag), mParams, similarSlave.Surface);

                mCurves.Add(mParams);
                sCurves.Add(sParams);

                master = master.Next;
                slave = slave.Next;
            }

            var mCoords = GetGridCoords(mCurves, Surface);
            var sCoords = GetGridCoords(sCurves, similarSlave.Surface);

            if (mCoords.Length != sCoords.Length)
                throw new Exception($"У поверхности {Surface} и поверхности {similarSlave.Surface} получено разное число координат сетки");
            
            for(var i = 0; i < mCoords.Length; ++i)
                mCoords[i] = (mCoords[i] + sCoords[i]) * 0.5;

            AverageCoords = mCoords;
        }*/

        /// <summary>
        /// Перестроение петли начиная с ближайшего к coords узла, сохраняя ориентацию петли
        /// </summary>
        /// <param name="coords">Ближайшая координата относительно которой предпочтительно перестроить петлю</param>
        public void Rebuild(double[] coords)
        {
            var start = GetClosestNode(coords, false);
            var newLoop = new LinkedList<CurveLoopNode>();

            var current = start;
            LinkedListNode<CurveLoopNode> end = null;

            for (var i = 0; i < 2; ++i)
            {
                while (current != end)
                {
                    var node = current.Value;
                    newLoop.AddLast(node);
                    current = current.Next;
                }
                end = start;
                current = Loop.First;
            }
            Loop = newLoop;
        }

        /// <summary>
        /// Инвертирует петлю
        /// </summary>
        /// <param name="coords">Ближайшая координата относительно которой предпочтительно перестроить петлю</param>
        public void Reverse(double[] coords = null)
        {
            Orientation = Orientation == Orient.CW ? Orient.CCW : Orient.CW;

            var start = coords != null ? GetClosestNode(coords) : Loop.Last;
            var newLoop = new LinkedList<CurveLoopNode>();

            var current = start;
            LinkedListNode<CurveLoopNode> end = null;

            for (var i = 0; i < 2; ++i)
            {
                while (current != end)
                {
                    var node = current.Value;
                    node.Reverse();
                    newLoop.AddLast(node);
                    current = current.Previous;
                }
                end = start;
                current = Loop.Last;
            }
            Loop = newLoop;
        }

        private double[] GetGridCoords(List<double[]> mCurves, int surface)
        {
            var last = pointsPerCurve - 1;

            var averageUv = new List<double>();
            for (var i = 1; i < last; ++i)
            {
                var opIndex0 = last - i;
                var mStride0 = i * 2;
                var sStride0 = opIndex0 * 2;

                var bounds0 = new double[] { mCurves[0][mStride0], mCurves[0][mStride0 + 1], mCurves[2][sStride0], mCurves[2][sStride0 + 1] };
                var u0Step = (bounds0[2] - bounds0[0]) / (pointsPerCurve - 1);
                var v0Step = (bounds0[3] - bounds0[1]) / (pointsPerCurve - 1);

                for (var j = 1; j < last; ++j)
                {
                    var opIndex1 = last - j;
                    var mStride1 = j * 2;
                    var sStride1 = opIndex1 * 2;

                    var bounds1 = new double[] { mCurves[1][mStride1], mCurves[1][mStride1 + 1], mCurves[3][sStride1], mCurves[3][sStride1 + 1] };
                    var u1Step = (bounds1[2] - bounds1[0]) / (pointsPerCurve - 1);
                    var v1Step = (bounds1[3] - bounds1[1]) / (pointsPerCurve - 1);

                    var u0 = bounds0[0] + u0Step * j;
                    var v0 = bounds0[1] + v0Step * j;
                    var u1 = bounds1[0] + u1Step * j;
                    var v1 = bounds1[1] + v1Step * j;

                    var avU = (u0 + u1) * 0.5;
                    var avV = (v0 + v1) * 0.5;

                    averageUv.Add(avU);
                    averageUv.Add(avV);
                }
            }

            var coords = controller.Gmsh.Model.GetValue(2, surface, averageUv.ToArray());
            return coords;
        }

        private LinkedListNode<CurveLoopNode> GetClosestNode(double[] coords, bool bindEnd = true)
        {
            var current = Loop.First;
            var src = new Vector3D(coords[0], coords[1], coords[2]);

            var min = double.MaxValue;
            var result = current;

            while (current != null)
            {
                var point = controller.Gmsh.Model.GetValue(0, bindEnd ? current.Value.End : current.Value.Begin, []);
                var pVec = new Vector3D(point[0], point[1], point[2]);
                var vec = pVec - src;
                var dp = vec.DotProduct(vec);
                if (dp < min)
                {
                    result = current;
                    min = dp;
                }
                current = current.Next;
            }
            return result;
        }

        private Orient DetectCircleOrientation(int curveTag) => curveTag > 0 ? Orient.CCW : Orient.CW;

        private Orient GetOrientation()
        {
            if (Loop.Count == 1)
                return DetectCircleOrientation(Loop.First.Value.Tag);
            else if (Loop.Count > 2)
                return DetectOrientationByThreeCurves();
            throw new NotImplementedException("Петля построенная на двух кривых пока не поддерживается");
        }

        private Orient DetectOrientationByThreeCurves()
        {
            var curves = Loop.Take(2).ToList();

            var begin = controller.Gmsh.Model.GetValue(0, curves[0].Begin, []);
            var middle = controller.Gmsh.Model.GetValue(0, curves[0].End, []);
            var end = controller.Gmsh.Model.GetValue(0, curves[1].End, []);

            var bVec = new Vector3D(begin[0], begin[1], begin[2]);
            var mVec = new Vector3D(middle[0], middle[1], middle[2]);
            var eVec = new Vector3D(end[0], end[1], end[2]);

            var first = (mVec - bVec).Normalize();
            var second = (eVec - mVec).Normalize();
            var cross = first.CrossProduct(second);

            var dp = cross.DotProduct(UnitVector3D.ZAxis);
            if (Math.Abs(dp) < 1e-4)
            {
                dp = cross.DotProduct(UnitVector3D.XAxis);
                if(Math.Abs(dp) < 1e-4)
                    dp = cross.DotProduct(UnitVector3D.YAxis);
            }

            return dp < 0 ? Orient.CW : Orient.CCW;
        }

        private void BuildLoop(int index)
        {
            var loops = controller.Gmsh.Model.Occ.GetCurveLoops(Surface).Item2;
            if (index > loops.Length - 1)
                throw new ArgumentException("Неправильный индекс отверстия");

            var set = loops[index].ToHashSet();
            var allCurves = controller.Gmsh.Model.GetBoundary([2, Surface], false, true).Where((v, i) => (i & 1) == 1)
                                                                                        .ToArray();
            var curvesList = new List<int>();
            foreach (var curve in allCurves)
                if (set.Contains(Math.Abs(curve)))
                    curvesList.Add(curve);

            var curves = curvesList.ToArray();

            for (var i = 0; i < curves.Length; ++i)
            {
                var points = controller.Gmsh.Model.GetBoundary([1, curves[i]], false, false).Where((v, i) => (i & 1) == 1)
                                                                                            .ToArray();
                var node = new CurveLoopNode(points[0], points[1], curves[i]);
                Loop.AddLast(node);
            }
        }

        private CurveLoop()
        {
        }
    }
}
