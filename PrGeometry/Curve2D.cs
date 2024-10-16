using Geometry.Exteisions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geometry
{
    public class Curve2D
    {
        List<Point2D> points;

        public Curve2D(List<Point2D>  points)
        {
            this.points = points;
        }

        public bool IsPointBelongCurve(Point2D point, double delta = 1e-2)
        {
            var segments = points.Bigrams(false)
                .Select(s => new Segment2D(s.Item1, s.Item2));
            foreach (var segment in segments)
            {
                if (segment.P1._x <= point._x
                    && point._x <= segment.P2._x)
                {
                    var x1 = segment.P1._x;
                    var x2 = segment.P2._x;
                    var y1 = segment.P1._y;
                    var y2 = segment.P2._y;
                    var value = y1 * (point._x - x2) / (x1 - x2) + y2 * (point._x - x1) / (x2 - x1); // linear interpolation
                    if (Math.Abs(value - point._y) <= delta)
                        return true;
                }
            }
            return false;
        }
    }
}
