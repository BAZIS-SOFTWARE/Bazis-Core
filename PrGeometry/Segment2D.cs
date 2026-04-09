using MathNet.Numerics.LinearAlgebra;
using System;
using System.Drawing;

namespace Geometry
{
    /// <summary>
    /// Segment2D
    /// </summary>
    public class Segment2D
    {
        public Point2D P1 { get; }
        public Point2D P2 { get; }

        double[] coefficients;

        private const double epsilon = 1e-4;
        /// <summary>
        /// Segment2D
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public Segment2D(Point2D p1, Point2D p2)
        {
            if (p1 == p2)
                throw new ArgumentException("Degenerate segment. Points are same");
            P1 = p1;
            P2 = p2;

            coefficients = GetLineEquationCoefficients();
        }
        /// <summary>
        /// GetIntersection
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Point2D GetIntersection(Segment2D other)
        {
            double x1 = P1._x, y1 = P1._y;
            double x2 = P2._x, y2 = P2._y;
            double x3 = other.P1._x, y3 = other.P1._y;
            double x4 = other.P2._x, y4 = other.P2._y;

            var denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (denom == 0) return null; // Параллельны

            var ua = Math.Round(((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denom,3);
            var ub = Math.Round(((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denom,3);

            // Если ua и ub между 0 и 1, пересечение на отрезках
            if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
            {
                return new Point2D(x1 + ua * (x2 - x1), y1 + ua * (y2 - y1));
            }
            return null; // Нет пересечения
        }

        /// <summary>
        /// IsIntersectWith
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool HasIntersectedPoint(Segment2D other)
        {
            var t_o_p1 = GetOrientationSign(other.P1);
            var t_o_p2 = GetOrientationSign(other.P2);

            var o_t_p1 = other.GetOrientationSign(P1);
            var o_t_p2 = other.GetOrientationSign(P2);

            if (t_o_p1 * t_o_p2 > 0)
                return false;

            if (o_t_p1 * o_t_p2 > 0)
                return false;

            //один поглащает другой или полностью совпадают

            if(o_t_p1 == 0 & o_t_p2 == 0 && t_o_p1 == 0 & t_o_p2 == 0)
                return false;

            //var x_other_min = Math.Min(other.P1._x, other.P2._x);
            //var x_other_max = Math.Max(other.P1._x, other.P2._x);
            //var x_min = Math.Min(P1._x, P2._x);
            //var x_max = Math.Max(P1._x, P2._x);

            //if(x_other_min < x_max && x_other_min > x_min)



            return true;


            //var firstEquationCoefficients = GetLineEquationCoefficients();
            //var secondEquationCoefficients = other.GetLineEquationCoefficients();
            //var matrix = Matrix<float>.Build.DenseOfArray(new[,]
            //{
            //    { firstEquationCoefficients[0], firstEquationCoefficients[1] },
            //    { secondEquationCoefficients[0], secondEquationCoefficients[1] }
            //});
            //var vector = Vector<float>.Build.Dense(new[]
            //{
            //    -firstEquationCoefficients[2],
            //    -secondEquationCoefficients[2]
            //});
            //var solution = matrix.Solve(vector);
            //var intersectionPoint = new Point2D(solution[0], solution[1]);
            //return IsPointBelongSegment(intersectionPoint) && other.IsPointBelongSegment(intersectionPoint);
        }
        /// <summary>
        /// IsPointBelongSegment
        /// </summary>
        /// <param name="point"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public bool IsPointBelongSegment(Point2D point, double delta = 0.0001)
        {
            // Проверяем расстояние до сегмента
            // изменено 12.10.2025
            // var value = coefficients[0] * point._x + coefficients[1] * point._y + coefficients[2];
            var a = (P2._x - P1._x) * (P1._y - point._y);
            var b = (P1._x - point._x) * (P2._y - P1._y);
            var length = Math.Sqrt(Math.Pow(P2._x - P1._x, 2) +
                Math.Pow(P2._y - P1._y, 2));

            var value = Math.Abs(a - b) / length;

            if (Math.Abs(value) > delta)
                return false;

            var temp = Math.Min(P1._x, P2._x);

            if (point._x < Math.Min(P1._x, P2._x)
                   || point._x > Math.Max(P1._x, P2._x))
                return false;

            if (point._y < Math.Min(P1._y, P2._y)
       || point._y > Math.Max(P1._y, P2._y))
                return false;

            return true;
        }

        public bool IsPointBelongLine(Point2D point)
        {
            //if (Math.Abs(P1._x - P2._x) < epsilon)
            //{
            //    var segment = new Segment2D(new Point2D(P1._y, P1._x), new Point2D(P2._y, P2._x));
            //    return segment.IsPointBelongLine(new Point2D(point._y, point._x));
            //}
            //var coefficients = GetLineEquationCoefficients();
            var value = coefficients[0] * point._x + coefficients[1] * point._y + coefficients[2];
            return Math.Abs(value) < epsilon;
        }

        public int GetOrientationSign(Point2D p0)
        {
            var a = P2.Sub(P1);
            var b = p0.Sub(P1);
            var crossProduct = Math.Round(a._x * b._y - a._y * b._x, 3);
            return Math.Sign(crossProduct);
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"({P1._x};{P1._y}),({P2._x};{P2._y})";
        }

        //in terms of line equation: Ax + By + C = 0 => (A, B, C)
        private double[] GetLineEquationCoefficients()
        {
            return new[] { (P1._y - P2._y), (P2._x - P1._x), (P1._x * P2._y - P1._y * P2._x) };
        }
        /// <summary>
        /// GetPointOnLine
        /// </summary>
        /// <param name="subLineLength"></param>
        /// <returns></returns>
        public Point2D GetPointOnLine(float subLineLength)
        {
            var point = new Point2D();


            if (GetLength() == 0) return new Point2D();

            point._x = P1._x + subLineLength * ((P2._x - P1._x) / GetLength());
            point._y = P1._y + subLineLength * ((P2._y - P1._y) / GetLength());



            return point;
        }

        /// <summary>
        /// GetLength
        /// </summary>
        /// <returns></returns>
        public float GetLength()
        {
            return (float)Math.Sqrt(Math.Pow((P2._x - P1._x), 2) + Math.Pow((P2._y - P1._y), 2));
        }
    }
}
