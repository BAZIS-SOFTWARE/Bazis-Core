using Functions.Sort;
using Geometry;
using ModelInterfaces.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelInterfaces.ObjectsSorters
{
    /// <summary>
    /// DistanceSorter
    /// </summary>
    [Obsolete("Не используйте. Вместо этого применяйте SortByDistance<T> из библиотеки Geometry")]
    public static class DistanceSorter
    {
        /// <summary>
        /// SortByDistance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        public static void SortByDistance <T>(this List<T> objs) where T : IModelObject
        {
            var fistNode = 0;

            while (fistNode < objs.Count - 1)
            {
                var dists = new List<float>();
                for (int j = fistNode + 1; j < objs.Count; j++) // простейшая пузырьковая сортировка  
                {
                    var dist = Vector.GetVectorLenght(objs[fistNode].CalcCentralPoint().Sub(objs[j].CalcCentralPoint()));
                    dists.Add(dist);
                }

                var minInd = dists.IndexOf(dists.Min()) + 1;


                objs.SwapList(fistNode + 1, minInd + fistNode);

                fistNode++;
            }
        }       
    }
}
