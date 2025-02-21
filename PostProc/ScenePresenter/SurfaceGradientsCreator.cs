using Geometry;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System;
using Project.Interfaces.Results;

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
        public IEnumerable<ResultFigure> CreateGradientObjects(ItemRange[] itemsRanges, List<Point3D> nodesCoords, List<float> nodesResults)
        {
            //int nodesOnSide = nodesCoords.Count;
            var subSurfs = new List<ResultFigure>();

            if (itemsRanges.Length == 1)
                subSurfs.Add(new ResultFigure(nodesCoords, itemsRanges[0].Color));

            else
            {
                var check = CheckOutOfRanges(itemsRanges, nodesResults);
                if (check > 0)
                    subSurfs.Add(new ResultFigure(nodesCoords, itemsRanges[itemsRanges.Length - 1].Color));
                else if (check < 0)
                    subSurfs.Add(new ResultFigure(nodesCoords, itemsRanges[0].Color));
                else
                {
                    for (int i = 0; i < itemsRanges.Length; i++)
                    {
                        var itemRange = itemsRanges[i];

                        if (CheckAllResultsInsideRange(itemRange, nodesResults))
                        {
                            subSurfs.Add(new ResultFigure(nodesCoords, itemsRanges[i].Color));
                        }
                        else
                        {
                            var surfPoints = new List<Point3D>();
                            for (int j = 0; j < nodesCoords.Count; j++)
                            {

                                for (int k = j + 1; k < nodesCoords.Count; k++)
                                {
                                    float[] ndRes;
                                    Point3D[] ndCoords;
                                    SortValues.Sort(nodesCoords[j], nodesCoords[k], nodesResults[j], nodesResults[k], out ndRes, out ndCoords);

                                    //проверить результаты в диапазоне или нет
                                    List<Point3D> edCoords;

                                    var resu = TryIntervalsIntersec(itemRange, ndRes, ndCoords,out edCoords);

                                    if (resu)
                                    {
                                        if (i == 0) edCoords[0] = ndCoords[0];
                                        else if (i == itemsRanges.Length - 1) edCoords[1] = ndCoords[1];

                                        foreach (var r in edCoords)
                                            if (!surfPoints.Contains(r))
                                                surfPoints.Add(r);
                                    }
                                }
                            }

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
        public bool TryIntervalsIntersec(ItemRange itemRange, float[] ndRes, Point3D[] ndCoords, out List<Point3D> edCoords)
        {
            edCoords = new List<Point3D>();

            if (Math.Abs(ndRes[0] - ndRes[1]) < difference)
            {
                if (ndRes[0] >= itemRange.Min && ndRes[0] <= itemRange.Max)
                {
                    edCoords.AddRange(ndCoords);
                    return true;
                }

                return false;
            }
            else
            {
                //Сводим задачу к построению пересечения двух одномерных отрезков.
                var ab = ndCoords[1].Sub(ndCoords[0]);
                var firstCoefficient = (itemRange.Min - ndRes[0]) / (ndRes[1] - ndRes[0]);
                var secondCoefficient = (itemRange.Max - ndRes[0]) / (ndRes[1] - ndRes[0]);
                var minCoefficient = Math.Min(firstCoefficient, secondCoefficient);
                var maxCoefficient = Math.Max(firstCoefficient, secondCoefficient);

                if (minCoefficient > 1 || maxCoefficient < 0)
                    return false;
                var p1 = ndCoords[0].Sum(ab.Mult(Math.Max(0, minCoefficient)));
                var p2 = ndCoords[0].Sum(ab.Mult(Math.Min(1, maxCoefficient)));
                edCoords = new List<Point3D> { p1,p2};

                return true;
            }

        }

        private bool CheckAllResultsInsideRange(ItemRange itemRange, List<float> values)
        {
            var counter = 0;
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] >= itemRange.Min && values[i] <= itemRange.Max)
                    counter++;
            }

            if (counter == values.Count)
            {
                return true;
            }
            else return false;
        }

        private int CheckOutOfRanges(ItemRange[] itemsRanges, List<float> nodesResults)
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
