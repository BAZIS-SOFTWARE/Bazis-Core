using MathNet.Numerics.Providers.SparseSolver;
using MathNet.Spatial.Euclidean;
using Model.GeometryObjects;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using static Community.CsharpSqlite.Sqlite3;

namespace OperationalController.MeshObjsCreator
{
    public static class MiddleSurfaceBuilder
    {
        public static string MessageLog { get; private set; }
        /// <summary>
        /// Строит серединную поверхность между masterSurf и slaveSurf поверхностями, если это возможно
        /// </summary>
        /// <param name="controller">Контроллер gmsh</param>
        /// <param name="masterSurf">Основная поверхность</param>
        /// <param name="slaveSurf">Вспомогательная поверхность</param>
        public static bool Build(IGmshController controller, int masterSurf, int slaveSurf)
        {
            var surfaceMap = new Dictionary<int, List<int>>();

            FillAdjacentSurfaceMap(controller, masterSurf, surfaceMap);
            FillAdjacentSurfaceMap(controller, slaveSurf, surfaceMap);
            //Везде где есть пары усредняем UV координаты, переводим в локальные.
            //У кого нет пары мы должны опросить все кривые без пар, все точки кривых опросить на принадлежность поверхности которая есть в SurfaceMap
            foreach (var kvp in surfaceMap)
            {
                //Если есть пары строим только их, мы не строим кривые у которых нет пары, значит они будут при рассмотрении другой поверхности,
                //но если нам нужно замкнуть 
                /*if (kvp.Value.Count == 1)
                {
                    var adj = controller.Gmsh.Model.GetAdjacencies(1, kvp.Value[0]);
                     
                }*/

            }
            //Карта смежных ребер ключ общее ребро - основная кривая


            return true;
        }



        private static void FillAdjacentSurfaceMap(IGmshController controller, int surface, Dictionary<int, List<int>> map)
        {
            foreach (var loop in GetOrientedCurveLoops(controller, surface))
            {
                foreach (var curve in loop)
                {
                    var absCurve = Math.Abs(curve);
                    var adjSurf = controller.Gmsh.Model.GetAdjacencies(1, absCurve).Item1
                                                       .Where(v => v != surface)
                                                       .First();
                    var adjCurve = GetOrientedCurveLoops(controller, adjSurf)[0].Where(v => Math.Abs(v) == absCurve)
                                                                                .First();
                    if (!map.ContainsKey(adjSurf))
                        map.Add(adjSurf, new List<int>());
                    map[adjSurf].Add(adjCurve);
                }
            }
        }
        /// <summary>
        /// Строит ориентированный цикл кривых (внешний и внутренние(отверстия))
        /// </summary>
        /// <param name="surface">Поверхность на которую опирается цикл кривых</param>
        /// <returns>Массив массивов ориентированных кривых</returns>
        private static int[][] GetOrientedCurveLoops(IGmshController controller, int surface)
        {
            var curveCounts = controller.Gmsh.Model.Occ.GetCurveLoops(surface).Item2.Select(v => v.Length).ToList();
            var curves = controller.Gmsh.Model.GetBoundary([2, surface], false);

            var start = 0;
            var loops = new int[curveCounts.Count][];

            for (var i = 0; i < curveCounts.Count; ++i)
            {
                var elements = curveCounts[i];
                loops[i] = new int[elements];
                var end = start + (elements << 1);
                var inCounter = 0;

                for (var j = start; j < end; j += 2)
                    loops[i][inCounter++] = curves[j + 1];
                start += (elements << 1);
            }

            return loops;
        }
    }
}
