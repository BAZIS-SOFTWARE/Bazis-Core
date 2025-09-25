using Geometry.Exteisions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Geometry
{
    public class Polygon
    {
        private readonly List<Point2D> points;
        /// <summary>
        /// Poligon
        /// </summary>
        /// <param name="points">
        /// Отсортированные по или против часовой стрелки точки</param>
        /// <exception cref="ArgumentException"></exception>
        internal Polygon(List<Point2D> points)
        {
            this.points = points;
        }

        public IEnumerable<Point2D> GetPoints()
        {
            foreach (var item in points)
            {
                yield return item;
            }

        }

        //Gauss's area formula (фор-ла шнурования или землемера)
        //Ф-ла подходит как для выпуклово так и для вогнутого многоугольника
        public double GetPolygonArea()
        {
            var sum = 0.0;
            var n = points.Count;
            for (var i = 0; i < n - 1; i++)
            {
                sum += (points[i]._x * points[i + 1]._y - points[i + 1]._x * points[i]._y);
            }
            sum += (points[n - 1]._x * points[0]._y - points[0]._x * points[n - 1]._y);
            return 0.5 * Math.Abs(sum);
        }

        public bool IsPointInsidePolygon(Point2D point, float epsilon = 0.01f)
        {
            var sourceArea = GetPolygonArea();
            var integratedArea = 0d;
            foreach (var segment in points.Bigrams(true))
            {
                var a = segment.Item2.Sub(segment.Item1);
                var b = point.Sub(segment.Item1);
                var triangleArea = 0.5 * Math.Abs(a._x * b._y - a._y * b._x);
                integratedArea += triangleArea;
            }
            return Math.Abs(integratedArea - sourceArea) < epsilon;
        }
    }
}
