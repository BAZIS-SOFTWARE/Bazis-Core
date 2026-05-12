using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmshApi;
using OperationalController.GmshController;
using MathNet.Spatial;
using MathNet.Spatial.Euclidean;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using System.Diagnostics.Metrics;

namespace OperationalController.MeshObjsCreator
{
    public class MiddleSurfaceBuilder
    {
        private IGmshController controller;
        private int pointsPerCurve;

        public enum Orientation
        {
            CW = -1,
            CCW = 1
        }

        /// <summary>
        /// Конструктор построения серединной поверхности
        /// </summary>
        /// <param name="controller">Контроллер IGmshController</param>
        /// <param name="pointsPerCurve">Число точек на кривой</param>
        /// <exception cref="ArgumentException">Исключение возникает в случае когда число переданных точек разбиения кривой меньше 2</exception>
        public MiddleSurfaceBuilder(IGmshController controller, int pointsPerCurve = 16)
        {
            this.controller = controller;
            if (pointsPerCurve < 2)
                throw new ArgumentException("Число точек разбиения кривой не может быть меньше 2");
            this.pointsPerCurve = pointsPerCurve;
        }

        /// <summary>
        /// Строит серединные поверхности по наборам исходных поверхностей surfTags
        /// </summary>
        /// <param name="surfTags">Идентификаторы поверхностей в формате (главная_поверхность_0, второстепенная_поверхность_0,...
        ///                                                               главная_поверхность_n, второстепенная_поверхность_n)</param>
        public void Build(int[] surfTags)
        {
            CheckArguments(surfTags);
            var simData = MakeSimilar(surfTags);
            AverageLoops(simData);


        }

        private void AverageLoops(List<((int, int[][]), (int, int[][]))> simData)
        {
            var loopsData = new List<double[][]>();
            foreach(var data in simData)
            {
                var master = data.Item1;
                var slave = data.Item2;
                var loopsLength = master.Item2.Length;

                var loopData = new double[loopsLength][];
                for(var i = 0; i < loopsLength; ++i)
                {
                    double[] average = default;
                    for (var j = 0; j < master.Item2[i].Length; ++j)
                    {
                        var mCurve = Math.Abs(master.Item2[i][j]);
                        var sCurve = Math.Abs(slave.Item2[i][j]);

                        var mSlaveSurf = controller.Gmsh.Model.GetAdjacencies(1, mCurve).Item1.Where(v => v != master.Item1).ToList();
                        var sSlaveSurf = controller.Gmsh.Model.GetAdjacencies(1, sCurve).Item1.Where(v => v != slave.Item1).ToList();

                        var is3dCase = mSlaveSurf.Count == 0 || sSlaveSurf.Count == 0 || mSlaveSurf[0] != sSlaveSurf[0];

                        var mBounds = GetCurveParametrization(master.Item2[i][j]);
                        var sBounds = GetCurveParametrization(slave.Item2[i][j]);

                        average = is3dCase ? GetAverageCurveCoordinates(mCurve, mBounds, sCurve, sBounds)
                                           : GetAverageCurveCoordinates(mCurve, mBounds, sCurve, sBounds, mSlaveSurf[0]);
                    }
                    loopData[i] = average;
                }
                loopsData.Add(loopData);
            }
        }

        private double[] GetAverageCurveCoordinates(int mCurve, double[] mBounds, int sCurve, double[] sBounds, int surface)
        {
            var mReparams = controller.Gmsh.Model.ReparametrizeOnSurface(1, mCurve, mBounds, surface);
            var sReparams = controller.Gmsh.Model.ReparametrizeOnSurface(1, sCurve, sBounds, surface);

            if (mReparams.Length != sReparams.Length)
                throw new Exception($"Массивы репараметризированных кривых на поверхность {surface} имеют разную длинну");

            var averageUv = new double[mReparams.Length];
            for (var i = 0; i < mReparams.Length; i += 2)
            {
                averageUv[i] = (mReparams[i] + sReparams[i]) * 0.5;
                averageUv[i + 1] = (mReparams[i + 1] + sReparams[i + 1]) * 0.5;
            }

            var average = controller.Gmsh.Model.GetValue(2, surface, averageUv);

            return average;
        }

        private double[] GetAverageCurveCoordinates(int mCurve, double[] mBounds, int sCurve, double[] sBounds)
        {
            var mCoords = controller.Gmsh.Model.GetValue(1, mCurve, mBounds);
            var sCoords = controller.Gmsh.Model.GetValue(1, sCurve, sBounds);

            if (mCoords.Length != sCoords.Length)
                throw new Exception($"Массивы координат кривых {mCurve} и {sCurve} имеют разную длинну");

            var average = new double[mCoords.Length];
            for (var i = 0; i < mCoords.Length; ++i)
                average[i] = (mCoords[i] + sCoords[i]) * 0.5;

            return average;
        }

        private double[] GetCurveParametrization(int curve)
        {
            var mCurve = Math.Abs(curve);
            var bounds = controller.Gmsh.Model.GetParametrizationBounds(1, mCurve);
            if (curve < 0)
                Swap(ref bounds.Item1[0], ref bounds.Item2[0]);

            var result = new double[pointsPerCurve];
            var step = (bounds.Item2[0] - bounds.Item1[0]) * (pointsPerCurve - 1);

            for (var i = 0; i < result.Length; ++i)
                result[i] = bounds.Item1[0] + step * i;

            return result;
        }

        private void CheckArguments(int[] surfTags)
        {
            for(var i = 0; i < surfTags.Length; i += 2)
            {
                var mBounds = controller.Gmsh.Model.GetBoundary([2, surfTags[i]]);
                var sBounds = controller.Gmsh.Model.GetBoundary([2, surfTags[i + 1]]);

                if (mBounds.Length != sBounds.Length)
                    throw new Exception($"У поверхности {surfTags[i]} количество точек не совпадает с количеством точек у поверхности {surfTags[i + 1]}");
            }
        }

        private List<((int, int[][]), (int, int[][]))> MakeSimilar(int[] surfTags)
        {
            var loopsData = new List<((int, int[][]), (int, int[][]))>();

            for (var i = 0; i < surfTags.Length; i +=2)
            {
                var mLoop = GetOrientedCurveLoop(surfTags[i]);
                var sLoop = GetOrientedCurveLoop(surfTags[i + 1]);

                var mOrients = GetSurfaceOrientation(mLoop, surfTags[i]);
                var sOrients = GetSurfaceOrientation(sLoop, surfTags[i + 1]);

                mLoop = SortCurveLoop(mLoop, surfTags[i], mOrients);
                sLoop = SortCurveLoop(sLoop, surfTags[i + 1], sOrients);   

                var simData = MakeSimilar(surfTags[i], mLoop, surfTags[i + 1], sLoop);
                mLoop = simData.Item1;
                sLoop = simData.Item2;

                loopsData.Add(((surfTags[i], mLoop), (surfTags[i + 1], sLoop)));
            }

            return loopsData;
        }

        private (int[][], int[][]) MakeSimilar(int masterSurface, int[][] mLoops, int slaveSurface, int[][] sLoops)
        {
            var masterDict = MakeHoleKeys(masterSurface, mLoops);
            var slaveDict = MakeHoleKeys(slaveSurface, sLoops);

            var masterList = new List<int[]>() { mLoops[0] };
            var slaveList = new List<int[]>() { sLoops[0] };

            foreach (var kvp in masterDict)
            {
                if(slaveDict.ContainsKey(kvp.Key))
                {
                    var slaveIndex = slaveDict[kvp.Key];
                    masterList.Add(mLoops[kvp.Value]);
                    slaveList.Add(sLoops[slaveIndex]);
                }
                
            }
            return (masterList.ToArray(), slaveList.ToArray());
        }

        private Dictionary<string, int> MakeHoleKeys(int surface, int[][] loops)
        {
            var dict = new Dictionary<string, int>();

            for (var i = 1; i < loops.Length; ++i)
            {
                var surfList = new List<int>();
                for (var j = 0; j < loops[i].Length; ++j)
                {
                    var surf = controller.Gmsh.Model.GetAdjacencies(1, loops[i][j]).Item1
                                                                                    .Where(v => v != surface)
                                                                                    .First();
                    surfList.Add(surf);
                }
                var surfKeys = surfList.OrderBy(v => v).Select(v => ToString());
                var key = string.Join(" ", surfKeys);
                dict.Add(key, i);
            }

            return dict;
        }

        private Orientation[] GetSurfaceOrientation(int[][] orCurveLoop, int surface)
        {
            var orientations = new List<Orientation>();
            for(var i = 0; i < orCurveLoop.Length; ++i)
            {
                if (orCurveLoop[i].Length == 1)
                    orientations.Add(DetectCircleOrientation(orCurveLoop[i][0]));
                if (orCurveLoop[i].Length == 2)
                    throw new NotImplementedException("Поверхности построенные на двух кривых пока не рассматриваются (hard case)");
                else
                    orientations.Add(DetectThreePointsCase(orCurveLoop[i], surface));
            }

            return orientations.ToArray();
        }

        private Orientation DetectThreePointsCase(int[] loop, int surface)
        {
            var vecList = new List<Vector3D>();

            for (var i = 0; i < 3; ++i)
            {
                var bounds = controller.Gmsh.Model.GetBoundary([1, loop[i]]);
                var coords = controller.Gmsh.Model.GetValue(bounds[0], bounds[1], []);

                var vec = new Vector3D(coords[0], coords[1], coords[2]);
                vecList.Add(vec);
            }

            var first = (vecList[1] - vecList[0]).Normalize();
            var second = (vecList[2] - vecList[1]).Normalize();
            var cross = first.CrossProduct(second);

            var dp = cross.DotProduct(UnitVector3D.ZAxis);


            return dp < 0 ? Orientation.CW : Orientation.CCW;
        }

        private Orientation DetectCircleOrientation(int curveTag) => curveTag > 0 ? Orientation.CCW : Orientation.CW;


        private int[][] SortCurveLoop(int[][] loops, int surface, Orientation[] orientations)
        {
            var sortedLoops = new List<List<int>>();

            for (var i = 0; i < loops.Length; ++i)
            {
                var refVec = Vector<double>.Build.Dense([1, 0]);
                var currentLoop = new List<(int, double)>();
                for (var j = 0; j < loops[i].Length; ++j)
                {
                    var bounds = controller.Gmsh.Model.GetBoundary([1, loops[i][j]], true, false);
                    var idPoint = j == loops[i].Length - 1 ? bounds[3] : bounds[1];
                    var coords = controller.Gmsh.Model.ReparametrizeOnSurface(bounds[0], idPoint, [], surface);

                    var vec = Vector<double>.Build.Dense(coords);
                    vec = vec.Normalize(2);
                    vec = Vector<double>.Build.Dense([vec[0] * (int)orientations[i], vec[1]]);

                    var m = Matrix<double>.Build.Dense(2, 2);
                    m.SetRow(0, refVec);
                    m.SetRow(1, vec);

                    var det = m.Determinant();
                    var dot = refVec.DotProduct(vec);

                    var angle = Math.Acos(dot);
                    angle = det < 0  ? 2 * Math.PI - angle : angle;

                    currentLoop.Add((loops[i][j] * (int)orientations[i], angle));
                }
                var sortedLoop = currentLoop.OrderBy(v => v.Item2).Select(v => v.Item1).ToList();
                sortedLoops.Add(sortedLoop);
            }
            return sortedLoops.Select(v => v.ToArray()).ToArray();
        }

        private int[][] GetOrientedCurveLoop(int surface)
        {
            var loops = controller.Gmsh.Model.Occ.GetCurveLoops(surface).Item2;
            var bounds = controller.Gmsh.Model.GetBoundary([2, surface]).Where((v,i) => (i & 1) == 1)
                                                                        .ToDictionary(k => Math.Abs(k), v => v);
            var oriented = new int[loops.Length][];

            for (var i = 0; i < loops.Length; ++i)
            {
                oriented[i] = new int[loops[i].Length];
                for (var j = 0; j < loops[i].Length; ++j)
                    oriented[i][j] = bounds[loops[i][j]];
            }

            return oriented;
        }

        private void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        private MiddleSurfaceBuilder()
        {
        }
    }
}
