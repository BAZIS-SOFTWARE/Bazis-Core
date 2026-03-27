using MathNet.Numerics.RootFinding;
using MathNet.Spatial.Euclidean;
using Microsoft.Scripting.Utils;
using Model.GeometryObjects;
using Mono.Unix.Native;
using OperationalController.GmshController;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;

namespace OperationalController.MeshObjsCreator
{
    public class MidSurfaceBuilder
    {
        private int points;
        private IGmshController controller;
        private Dictionary<int, int> nodeSet;
        private Dictionary<int, int> curveSet;

        private struct SurfaceData
        {
            public int SurfaceTag { get; private set; }
            public Dictionary<int, double[]> CurvePointMap {  get; private set; }
            public double[][] HolePoints { get; private set; }
            public int[] CurveTags { get; private set; }
            public SurfaceData(IGmshController controller, int surfaceTag, int points)
            {
                SurfaceTag = surfaceTag;

                var curveBounds = DetectCurveBounds(controller);
                CurveTags = CreateReorderedCurveBounds(curveBounds);
                CreateCurvePointMap(controller, points);
            }

            private void CreateCurvePointMap(IGmshController controller, int points)
            {
                CurvePointMap = new Dictionary<int, double[]>();

                var mul = new int[] { 1, points, -1, -points };
                var add = new int[] { 0, points - 1, points * points - 1, points * (points - 1) };

                for (var i = 0; i < CurveTags.Length; ++i)
                {
                    var index = Math.Abs(CurveTags[i]);

                    var curveP = controller.Gmsh.Model.GetParametrizationBounds(1, index);
                    if (CurveTags[i] < 0)
                        Swap(curveP.Item1, curveP.Item2);
                    var offset = (curveP.Item2[0] - curveP.Item1[0]) / (points - 1);

                    var tArray = new double[points];
                    for (var j = 1; j < points - 1; ++j)
                        tArray[j] = curveP.Item1[0] + j * offset;
                    var surfParams = controller.Gmsh.Model.ReparametrizeOnSurface(1, index, tArray, SurfaceTag);

                    FillCurveParametrization(points, surfParams, mul[i], add[i]);
                }
            }

            private void FillCurveParametrization(int points, double[] surfParams, int mul, int add)
            {
                for (var j = 1; j < points - 1; ++j)
                {
                    var arrStride = j * 2;
                    var key = j * mul + add;
                    CurvePointMap.Add(key, [surfParams[arrStride], surfParams[arrStride + 1]]);
                }
            }

            private Dictionary<CurveBound, int> DetectCurveBounds(IGmshController controller)
            {
                var curves = controller.Gmsh.Model.GetBoundary([2, SurfaceTag]).Where((v, i) => (i & 1) == 1).ToArray();
                if (curves.Length != 4)
                    throw new ArgumentException("В настоящее время поддерживается только поверхности, построенные на 4 кривых");

                var bounds = curves.ToDictionary(v => Math.Abs(v), k => k);
                var contours = controller.Gmsh.Model.Occ.GetCurveLoops(SurfaceTag).Item2;
                var external = contours[0].Select(v => bounds[v]).ToArray();

                var boundData = ProcessExternalContour(controller, external);
                ProcessHolesContour(controller, contours.Skip(1).ToArray());
                
                return boundData;
            }

            private Dictionary<CurveBound, int> ProcessExternalContour(IGmshController controller, int[] loop)
            {
                var result = new Dictionary<CurveBound, int>();

                for (var i = 0; i < loop.Length; ++i)
                {
                    var curveTag = Math.Abs(loop[i]);
                    var curveP = controller.Gmsh.Model.GetParametrizationBounds(1, curveTag);
                    var surfParams = controller.Gmsh.Model.ReparametrizeOnSurface(1, curveTag, [curveP.Item1[0], curveP.Item2[0]], SurfaceTag);

                    var uMinSign = Math.Sign(surfParams[0]);
                    var uMaxSign = Math.Sign(surfParams[2]);
                    var vMinSign = Math.Sign(surfParams[1]);
                    var vMaxSign = Math.Sign(surfParams[3]);

                    var bound = CurveBound.Unknown;
                    if (uMinSign == uMaxSign)
                        bound = uMinSign > 0 ? CurveBound.Up : CurveBound.Down;
                    else if (vMinSign == vMaxSign)
                        bound = vMinSign > 0 ? CurveBound.Left : CurveBound.Right;
                    if (bound == CurveBound.Unknown)
                        throw new Exception($"Неизвестный тип границы кривой {curveTag}");
                    result.Add(bound, loop[i]);
                }
                return result;
            }

            private void ProcessHolesContour(IGmshController controller, int[][] holes)
            {
                
            }

            private int[] CreateReorderedCurveBounds(Dictionary<CurveBound, int> map)
            {
                var result = new int[] { map[CurveBound.Down], map[CurveBound.Right], map[CurveBound.Up], map[CurveBound.Left] };
                return result;
            }

            private void Swap(double[] a, double[] b)
            {
                var temp = a[0];
                a[0] = b[0];
                b[0] = temp;
            }
        }

        public enum CurveBound
        {
            Unknown,
            Left,
            Right,
            Up,
            Down
        }

        public MidSurfaceBuilder(IGmshController controller, int points = 15)
        {
            if (points < 3)
                throw new ArgumentException("Число точек не должно быть меньше 3");

            this.points = points;
            this.controller = controller;

            nodeSet = new Dictionary<int, int>();
            curveSet = new Dictionary<int, int>();
        }

        public void Build(IEnumerable<int> surfaces)
        {
            //TODO: обработка отверстий
            //1: Узнать есть ли они, если есть то (получить параметризацию отверстия (кривых относительно поверхности))
            //2: Есть ли отверстия на другой поверхности (получить параметризацию отверстия (кривых относительно поверхности), если нет ничего не делаем
            //3: Если есть, проверим, что отверстие сквозное (кривая опирается на поверхность кроме той в которой сейчас находится, а эта
            // поверхность принадлежит другой кривой с противоположного контура), если нет ничего не делаем
            controller.Gmsh.Model.Add("MiddleSurface");

            var surfs = surfaces.ToList();
            if (surfs.Count < 2)
                throw new ArgumentException("Число поверхностей меньше 2");
            if ((surfs.Count & 1) != 0)
                throw new ArgumentException("Рассматриваются только парные поверхности");
            for (var i = 0; i < surfs.Count; i += 2)
            {
                controller.Gmsh.Model.SetCurrent(controller.Gmsh.Model.List()[0]);
                var mData = new SurfaceData(controller, surfs[i], points);
                var sData = new SurfaceData(controller, surfs[i + 1], points);
                CreateMidSurface(ref mData, ref sData);
            }
            controller.Gmsh.Model.SetCurrent(controller.Gmsh.Model.List()[1]);
            var nodes = controller.Gmsh.Model.Occ.GetEntities(0);
            controller.Gmsh.Model.Occ.Remove(nodes);
            controller.Gmsh.Model.Occ.Synchronize();
        }

        private void CreateMidSurface(ref SurfaceData master, ref SurfaceData slave)
        {
            var mul = new int[] { 1, points, -1, -points };
            var add = new int[] { -1, -1, points * points, points * points };

            var curveList = new List<int>();
            var curves = master.CurveTags;
            for (var i = 0; i < curves.Length; ++i)
            {
                var mCurve = Math.Abs(curves[i]);
                var curve = 0;
                if (!curveSet.ContainsKey(mCurve))
                {
                    var pointList = CreateMiddlePoints(ref master, ref slave, i, mul[i], add[i]);

                    controller.Gmsh.Model.SetCurrent(controller.Gmsh.Model.List()[1]);

                    curve = controller.Gmsh.Model.Occ.AddSpline(pointList.ToArray());
                    curveSet.Add(mCurve, curve);

                    controller.Gmsh.Model.SetCurrent(controller.Gmsh.Model.List()[0]);
                }
                else
                    curve = -curveSet[mCurve];

                curveList.Add(curve);
            }
            controller.Gmsh.Model.SetCurrent(controller.Gmsh.Model.List()[1]);
            var loop = controller.Gmsh.Model.Occ.AddCurveLoop(curveList.ToArray());
            if (loop == 0)
                throw new Exception("Не могу создать петлю кривых");
            controller.Gmsh.Model.Occ.AddSurfaceFilling(loop);
        }

        private List<int> CreateMiddlePoints(ref SurfaceData master, ref SurfaceData slave, int curveIndex, int mul, int add)
        {
            var pointList = new List<int>();
            for (var j = 1; j <= points; ++j)
            {
                double[] mPoint, sPoint;
                int mInit = 0, sInit = 0;

                if (j == 1 || j == points)
                {
                    mPoint = GetCurveBoundaryPoint(master.CurveTags[curveIndex], j, ref mInit);
                    sPoint = GetCurveBoundaryPoint(slave.CurveTags[curveIndex], j, ref sInit);

                    if (mInit == sInit)
                    {
                        pointList.Add(mInit);
                        continue;
                    }
                }
                else
                {
                    var key = j * mul + add;
                    mPoint = controller.Gmsh.Model.GetValue(2, master.SurfaceTag, master.CurvePointMap[key]);
                    sPoint = controller.Gmsh.Model.GetValue(2, slave.SurfaceTag, slave.CurvePointMap[key]);
                }

                var mVec = new Vector3D(mPoint[0], mPoint[1], mPoint[2]);
                var sVec = new Vector3D(sPoint[0], sPoint[1], sPoint[2]);
                var coords = (mVec + sVec) / 2;

                controller.Gmsh.Model.SetCurrent(controller.Gmsh.Model.List()[1]);

                var tag = controller.Gmsh.Model.Occ.AddPoint(coords.X, coords.Y, coords.Z);

                controller.Gmsh.Model.SetCurrent(controller.Gmsh.Model.List()[0]);
                if (j == 1 || j == points)
                {
                    nodeSet.Add(mInit, tag);
                    nodeSet.Add(sInit, tag);
                }
                pointList.Add(tag);
            }
            return pointList;
        }

        private double[] GetCurveBoundaryPoint(int curve, int pos, ref int pointTag)
        {
            var points = controller.Gmsh.Model.GetBoundary([1, curve], false);

            pointTag = pos == 1 ? points[1] : points[3];
            if (nodeSet.ContainsKey(pointTag))
            {
                pointTag = nodeSet[pointTag];
                return [];
            }

            return controller.Gmsh.Model.GetValue(0, pointTag, []);
        }

        private MidSurfaceBuilder()
        {
        }
    }
}
