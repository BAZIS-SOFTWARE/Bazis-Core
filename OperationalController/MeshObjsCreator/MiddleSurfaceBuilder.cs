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

namespace OperationalController.MeshObjsCreator
{
    public class MiddleSurfaceBuilder
    {
        private IGmshController controller;

        public MiddleSurfaceBuilder(IGmshController controller)
        {
            this.controller = controller;
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
            //TODO: проходимся по основному циклу и ищем есть ли у обоих кривых одинаковая поверхность, если есть то строим поверхность по средней uv параметризации
            //Иначе по средней 3D параметризации, для дырок строим только по средней UV параметризации так как общие поверхности мы уже проверили
            //и они гарантированно есть
            foreach(var data in simData)
            {
                var master = data.Item1;
                var slave = data.Item2;
                for(var i = 0; i < master.Item2[0].Length; ++i)
                {
                    var mCurve = Math.Abs(master.Item2[0][i]);
                    var sCurve = Math.Abs(slave.Item2[0][i]);

                    var mSlaveSurf = controller.Gmsh.Model.GetAdjacencies(1, mCurve).Item1.Where(v => v != master.Item1).First();
                    var sSlaveSurf = controller.Gmsh.Model.GetAdjacencies(1, sCurve).Item1.Where(v => v != slave.Item1).First();

                    
                    

                }
            }
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

                //TODO: сначала нужно сделать чтобы совпало направление!
                mLoop = SortCurveLoop(mLoop, surfTags[i]);
                sLoop = SortCurveLoop(sLoop, surfTags[i + 1]);

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

        private int[][] SortCurveLoop(int[][] loops, int surface)
        {
            var refVec = Vector<double>.Build.Dense([0, 1]);
            var sortedLoops = new List<List<int>>();

            for (var i = 0; i < loops.Length; ++i)
            {
                var currentLoop = new List<(int, double)>();
                for (var j = 0; j < loops[i].Length; ++j)
                {
                    var bounds = controller.Gmsh.Model.GetBoundary([1, loops[i][j]], true, false);
                    var idPoint = j == loops[i].Length - 1 ? bounds[3] : bounds[1];
                    var coords = controller.Gmsh.Model.ReparametrizeOnSurface(bounds[0], idPoint, [], surface);

                    var vec = Vector<double>.Build.Dense(coords);
                    vec = vec.Normalize(2);

                    var m = Matrix<double>.Build.Dense(2, 2);
                    m.SetRow(0, refVec);
                    m.SetRow(1, vec);

                    var det = m.Determinant();
                    var dot = refVec.DotProduct(vec);

                    var angle = Math.Acos(dot);
                    angle = det < 0 ? 2 * Math.PI - angle : angle;

                    currentLoop.Add((loops[i][j], angle));
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

        private MiddleSurfaceBuilder()
        {
        }
    }
}
