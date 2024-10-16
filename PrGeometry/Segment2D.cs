using MathNet.Numerics.LinearAlgebra;
using System;

namespace Geometry
{
    /// <summary>
    /// Segment2D
    /// </summary>
    public class Segment2D
    {
        public Point2D P1 { get; }
        public Point2D P2 { get; }

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
        }
        /// <summary>
        /// IsIntersectWith
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsIntersectWith(Segment2D other)
        {
            var firstEquationCoefficients = GetLineEquationCoefficients();
            var secondEquationCoefficients = other.GetLineEquationCoefficients();
            var matrix = Matrix<float>.Build.DenseOfArray(new[,] 
            {
                { firstEquationCoefficients[0], firstEquationCoefficients[1] },
                { secondEquationCoefficients[0], secondEquationCoefficients[1] }
            });
            var vector = Vector<float>.Build.Dense(new[]
            { 
                -firstEquationCoefficients[2], 
                -secondEquationCoefficients[2]
            });
            var solution = matrix.Solve(vector);
            var intersectionPoint = new Point2D(solution[0], solution[1]);
            return IsPointBelongSegment(intersectionPoint) && other.IsPointBelongSegment(intersectionPoint);
        }
        /// <summary>
        /// IsPointBelongSegment
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointBelongSegment(Point2D point)
        {
            if (Math.Abs(P1._x - P2._x) < epsilon)
            {
                var segment = new Segment2D(new Point2D(P1._y, P1._x), new Point2D(P2._y, P2._x));
                return segment.IsPointBelongSegment(new Point2D(point._y, point._x));
            }
            var coefficients = GetLineEquationCoefficients();
            var value = coefficients[0] * point._x + coefficients[1] * point._y + coefficients[2];
            return Math.Abs(value) < epsilon
                   && Math.Min(P1._x, P2._x) <= point._x
                   && point._x <= Math.Max(P1._x, P2._x);
        }

        public bool IsPointBelongLine(Point2D point)
        {
            if (Math.Abs(P1._x - P2._x) < epsilon)
            {
                var segment = new Segment2D(new Point2D(P1._y, P1._x), new Point2D(P2._y, P2._x));
                return segment.IsPointBelongLine(new Point2D(point._y, point._x));
            }
            var coefficients = GetLineEquationCoefficients();
            var value = coefficients[0] * point._x + coefficients[1] * point._y + coefficients[2];
            return Math.Abs(value) < epsilon;
        }

        public int GetOrientationSign(Point2D p0)
        {
            var a = P2.Sub(P1);
            var b = p0.Sub(P2);
            var crossProduct = Math.Round(a._x * b._y - a._y * b._x, 3);
            return Math.Sign(crossProduct);
        }

        public override string ToString()
        {
            return $"({P1._x};{P1._y}),({P2._x};{P2._y})";
        }

        //in terms of line equation: Ax + By + C = 0 => (A, B, C)
        private float[] GetLineEquationCoefficients()
        {
            return new[] { (P1._y - P2._y), (P2._x - P1._x), (P1._x * P2._y - P1._y * P2._x) };
        }
    }
}
