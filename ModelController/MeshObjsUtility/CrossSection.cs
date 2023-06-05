using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model;
using Model.Interfaces;
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
        /// <returns>
        /// a pair of adjacent elements in a items list and a pair (last, first).
        /// </returns>
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

    public class CrossSection
    {
        /// <returns>
        /// a dictionary where the key is the element number and the value is a list of points of section by the given plane.
        /// </returns>
      
        public Dictionary<int, Surface> GetSectionSurfaces(List<Element3D> elements, Plane plane)
        {
            var result = new Dictionary<int, Surface>();
            var objectNumber = 0;
            foreach (var element in elements)
            {
                var nodes = new List<Node>();
                var usedPoints = new HashSet<Point3D>();
                foreach (var elementSurface in element.GetSurface())
                {
                    foreach (var pair in elementSurface.GetPoints().Bigrams())
                    {
                        var point1 = pair.Item1;
                        var point2 = pair.Item2;
                        if (Math.Sign(plane.PointPosition(point1)) == Math.Sign(plane.PointPosition(point2)))
                            continue;
                        var crossSectionPoint = GetSectionPoint(new Line(point1, point2), plane);
                        if (!usedPoints.Contains(crossSectionPoint))
                        {
                            var node = new Node(objectNumber++, crossSectionPoint);
                            usedPoints.Add(crossSectionPoint);
                            nodes.Add(node);
                        }
                    }
                }
                if (nodes.Count >= 3)
                {
                    var surface = new Surface(objectNumber++, nodes.ToArray(), ObjKind.ElementSurface);
                    result.Add(element.Number, surface);
                }
            }
            return result;
        }

        /// <returns>
        /// the coefficients of the cannonical plane equation (Ax + By + Cz + D = 0) by given three points.
        /// </returns>

        private float[] GetPlaneEquationCoefficients(Point3D a, Point3D b, Point3D c)
        {
            var A = Matrix<float>.Build.DenseOfArray(new float[,] {
                { b._y - a._y, b._z - a._z },
                { c._y - a._y, c._z - a._z } })
                .Determinant();
            var B = Matrix<float>.Build.DenseOfArray(new float[,] {
                { b._x - a._x, b._z - a._z },
                { c._x - a._x, c._z - a._z } })
                .Determinant();
            var C = Matrix<float>.Build.DenseOfArray(new float[,] {
                { b._x - a._x, b._y - a._y },
                { c._x - a._x, c._y - a._y } })
                .Determinant();
            var D = -a._x * A + a._y * B - a._z * C;
            return new float[4] { A, -B, C, D };
        }

        /// <returns>
        /// the point of intersection of the given plane and the given line
        /// </returns>

        private Point3D GetSectionPoint(Line line, Plane plane)
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

