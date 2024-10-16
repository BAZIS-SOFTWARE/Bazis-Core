using Geometry.Exteisions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geometry
{
    public class Polygon
    {
        private readonly List<Point2D> points;
        private const float epsilon = 1e-4f;

        public Polygon(List<Point2D> points)
        {
            var hullCreator = new Hull2DCreator(points);
            this.points = hullCreator.GetHullGraham().ToList();
        }

        //Gauss's area formula
        //Seems legit because the hull creator traverse on points in a defined direction
        public float GetPolygonArea()
        {
            var sum = 0f;
            var n = points.Count;
            for (var i = 0; i < n - 1; i++)
            {
                sum += (points[i]._x * points[i + 1]._y - points[i + 1]._x * points[i]._y);
            }
            sum += (points[n - 1]._x * points[0]._y - points[0]._x * points[n - 1]._y);
            return 0.5f * Math.Abs(sum);
        }

        public bool IsPointInsidePolygon(Point2D point)
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

        public bool IsSelectedByRectangle(RectangleBox rectangle)
        {
            var maxX = (int)points.Select(p => p._x).Max();
            var maxY = (int)points.Select(p => p._y).Max();
            var minX = (int)points.Select(p => p._x).Min();
            var minY = (int)points.Select(p => p._y).Min();
            var figureBox = new RectangleBox(minX, maxX, minY, maxY);
            if (!figureBox.IsIntersectWithOther(rectangle))
                return false;
            if (figureBox.IsInnerOther(rectangle))
                return true;
            var rectanglePoints = rectangle.GetPoints();
            if (rectanglePoints.Any(p => IsPointInsidePolygon(p)))
                return true;
            foreach (var rectangleSegment in rectanglePoints.Bigrams(true).Select(p => new Segment2D(p.Item1, p.Item2)))
            {
                foreach (var contourSegment in points.Bigrams(true).Select(p => new Segment2D(p.Item1, p.Item2)))
                {
                    if (contourSegment.IsIntersectWith(rectangleSegment))
                        return true;
                }
            }
            return false;
        }
    }
}
