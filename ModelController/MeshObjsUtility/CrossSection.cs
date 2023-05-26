using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsUtility
{
    public static class IEnumerableExtentions
    {
        public static IEnumerable<Tuple<T, T>> Bigrams<T>(this IEnumerable<T> items)
        {
            var enumerator = items.GetEnumerator();
            enumerator.MoveNext();
            var first = enumerator.Current;
            var head = items.First();
            while (enumerator.MoveNext())
            {
                var second = enumerator.Current;
                yield return Tuple.Create(first, second);
                first = second;
            }
            yield return Tuple.Create(head, first);
        }
    }
    /// <summary>
    /// CutSection
    /// </summary>
    public class CrossSection
    {
        /// <summary>
        /// GetCrossNodes
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public Dictionary<int, List<Point3D>> GetCrossNodes(List<Element3D> elements, Plane plane)
        {
            int numProcs = Environment.ProcessorCount;
            var result = new ConcurrentDictionary<int, List<Point3D>>(numProcs * 2, elements.Count);
            Parallel.ForEach(elements, element =>
            {
                var points = new HashSet<Point3D>();
                foreach (var surface in element.GetSurface())
                {
                    foreach (var pair in surface.GetPoints().Bigrams())
                    {
                        var point1 = pair.Item1;
                        var point2 = pair.Item2;
                        if (Math.Sign(plane.PointPosition(point1)) == Math.Sign(plane.PointPosition(point2)))
                            continue;
                        var crossSectionPoint = GetCrossSectionPoint(new Line(point1, point2), plane);
                        points.Add(crossSectionPoint);
                    }
                }
                result[element.Number] = points.ToList();
            });
            return result.ToDictionary(x => x.Key, x => x.Value);
        }

        private float GetDet(float[,] matrix)
        {
            var A = Matrix<float>.Build.DenseOfArray(matrix);
            return A.Determinant();
        }

        private float[] GetPlaneEquationCoefficients(Point3D a, Point3D b, Point3D c)
        {
            var A = GetDet(new float[,] { { b._y - a._y, b._z - a._z },
                                          { c._y - a._y, c._z - a._z } });
            var B = GetDet(new float[,] { { b._x - a._x, b._z - a._z },
                                          { c._x - a._x, c._z - a._z } });
            var C = GetDet(new float[,] { { b._x - a._x, b._y - a._y },
                                            { c._x - a._x, c._y - a._y } });
            var D = -a._x * A + a._y * B - a._z * C;
            return new float[4] { A, -B, C, D };
        }

        private Point3D GetCrossSectionPoint(Line line, Plane plane)
        {
            var coefficients = GetPlaneEquationCoefficients(plane.P0, plane.P1, plane.P2);
            var A = coefficients[0];
            var B = coefficients[1];
            var C = coefficients[2];
            var D = coefficients[3];
            var t = (-A * line.P1._x - B * line.P1._y - C * line.P1._z - D) /
                (A * (line.P0._x - line.P1._x) + B * (line.P0._y - line.P1._y) + C * (line.P0._z - line.P1._z));

            var x = line.P1._x * (1 - t) + line.P0._x * t;
            var y = line.P1._y * (1 - t) + line.P0._y * t;
            var z = line.P1._z * (1 - t) + line.P0._z * t;
            return new Point3D(x, y, z);
        }
    }
}

