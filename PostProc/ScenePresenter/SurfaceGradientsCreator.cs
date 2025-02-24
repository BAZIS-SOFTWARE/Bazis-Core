using Geometry;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System;
using Project.Interfaces.Results;
using Geometry.Exteisions;
using Model.Extensions;

namespace PostProc.ScenePresenter
{
    /// <summary>
    /// SurfaceGradientsCreator
    /// </summary>
    public class SurfaceGradientsCreator
    {
        //Point3DEqualityComparer comparer = new Point3DEqualityComparer();
        /// <summary>
        /// CreateGradientObjects
        /// </summary>
        float difference = 1e-6f;
        public IEnumerable<ResultFigure> CreateGradientObjects(ItemRange[] itemsRanges, List<ResultItem> resItems)
        {
            //int nodesOnSide = nodesCoords.Count;
            var subSurfs = new List<ResultFigure>();

            if (itemsRanges.Length == 1)
                subSurfs.Add(new ResultFigure(resItems.Select(x => x.Coord), itemsRanges[0].Color));

            else
            {
                var check = CheckOutOfRanges(itemsRanges, resItems.Select(x => x.Value));
                if (check > 0)
                    subSurfs.Add(new ResultFigure(resItems.Select(x => x.Coord), itemsRanges[itemsRanges.Length - 1].Color));
                else if (check < 0)
                    subSurfs.Add(new ResultFigure(resItems.Select(x => x.Coord), itemsRanges[0].Color));
                else
                {
                    for (int i = 0; i < itemsRanges.Length; i++)
                    {
                        var itemRange = itemsRanges[i];

                        if (CheckAllResultsInsideRange(itemRange, resItems))
                        {
                            subSurfs.Add(new ResultFigure(resItems.Select(x => x.Coord), itemsRanges[i].Color));
                        }
                        else
                        {
                            var surfPoints = new List<Point3D>();

                            foreach (var item in resItems.Bigrams(true))
                            {
                                //проверить результаты в диапазоне или нет
                                List<Point3D> edCoords;
                                Tuple<ResultItem, ResultItem> temp = item;
                                var inv = false;
                                if (item.Item1.Value.CompareTo(item.Item2.Value) > 0)
                                {
                                    temp = new Tuple<ResultItem, ResultItem>(item.Item2, item.Item1);
                                    inv = true;
                                }
                                    
                                var resu = TryIntervalsIntersec(itemRange, temp.Item1, temp.Item2, out edCoords);

                                if (resu)
                                {
                                    if (i == 0) edCoords[0] = temp.Item1.Coord;
                                    else if (i == itemsRanges.Length - 1) edCoords[1] = temp.Item2.Coord;

                                    if (inv)
                                        edCoords.SwapList(1, 0);

                                    foreach (var r in edCoords)
                                        if (!surfPoints.Contains(r))
                                            surfPoints.Add(r);
                                }
                            }


                            //for (int j = 0; j < resItems.Count; j++)
                            //{

                            //    for (int k = j + 1; k < resItems.Count; k++)
                            //    {
                            //        float[] ndRes;
                            //        Point3D[] ndCoords;
                            //        SortValues.Sort(resItems[j], resItems[k], nodesResults[j], nodesResults[k], out ndRes, out ndCoords);

                            //        //проверить результаты в диапазоне или нет
                            //        List<Point3D> edCoords;

                            //        var resu = TryIntervalsIntersec(itemRange, ndRes, ndCoords,out edCoords);

                            //        if (resu)
                            //        {
                            //            if (i == 0) edCoords[0] = ndCoords[0];
                            //            else if (i == itemsRanges.Length - 1) edCoords[1] = ndCoords[1];

                            //            foreach (var r in edCoords)
                            //                if (!surfPoints.Contains(r))
                            //                    surfPoints.Add(r);
                            //        }
                            //    }
                            //}

                            if (surfPoints.Count() > 2)
                                subSurfs.Add(new ResultFigure(surfPoints, itemsRanges[i].Color));
                        }
                    }
                }
            }

            return subSurfs;
        }


        /// <summary>
        /// FindIntervalsIntersec
        /// </summary>
        /// <param name="itemRange"></param>
        /// <param name="ndRes"></param>
        /// <param name="edCoords"></param>
        /// <returns></returns>
        public bool TryIntervalsIntersec(ItemRange itemRange, ResultItem res0, ResultItem res1, out List<Point3D> edCoords)
        {
            edCoords = new List<Point3D>();

            if (Math.Abs(res1.Value - res0.Value) < difference)
            {
                if (res0.Value >= itemRange.Min && res0.Value <= itemRange.Max)
                {
                    edCoords.Add(res0.Coord);
                    edCoords.Add(res1.Coord);
                    return true;
                }

                return false;
            }
            else
            {
                //Сводим задачу к построению пересечения двух одномерных отрезков.
                var ab = res1.Coord.Sub(res0.Coord);
                var firstCoefficient = (itemRange.Min - res0.Value) / (res1.Value - res0.Value);
                var secondCoefficient = (itemRange.Max - res0.Value) / (res1.Value - res0.Value);
                var minCoefficient = Math.Min(firstCoefficient, secondCoefficient);
                var maxCoefficient = Math.Max(firstCoefficient, secondCoefficient);

                if (minCoefficient > 1 || maxCoefficient < 0)
                    return false;
                var p1 = res0.Coord.Sum(ab.Mult(Math.Max(0, minCoefficient)));
                var p2 = res0.Coord.Sum(ab.Mult(Math.Min(1, maxCoefficient)));
                edCoords = new List<Point3D> { p1,p2};

                return true;
            }

        }

        private bool CheckAllResultsInsideRange(ItemRange itemRange, List<ResultItem> resultItems)
        {
            var counter = 0;

            foreach (var item in resultItems)
            {
                if (item.Value >= itemRange.Min && item.Value <= itemRange.Max)
                    counter++;
            }

            if (counter == resultItems.Count)
            {
                return true;
            }
            else return false;
        }

        private int CheckOutOfRanges(ItemRange[] itemsRanges, IEnumerable<float> nodesResults)
        {
            var ranValMax = itemsRanges[itemsRanges.Length - 1].Max;
            var ranValMin = itemsRanges[0].Min;

            var ndValMax = nodesResults.Max();
            var ndValMin = nodesResults.Min();

            if (ranValMax - ndValMin < difference)
                return 1;
            else if (ranValMin - ndValMax > difference)
                return -1;
            else
            {
                return 0;
            }
        }
    }
}
