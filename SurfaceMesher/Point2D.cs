using System;

namespace SurfaceMesher
{
    /// <summary>
    /// Класс, описывающий точку на плоскости
    /// </summary>
    [Obsolete]
    public class Point2D
    {
        public double X { get; private set; }

        public double Y { get; private set; }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point2D Sub(Point2D b)
        {
            return new Point2D(X - b.X, Y - b.Y);
        }

        public Point2D Add(Point2D b)
        {
            return new Point2D(X + b.X, Y + b.Y);
        }

        public Point2D Mult(double factor)
        {
            return new Point2D(factor * X, factor * Y);
        }

        public double GetLength()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}