using System;

namespace SurfaceMesher
{
    [Obsolete]
    public static class Vector
    {
        /// <summary>
        /// Вычислить векторное произведение векторов
        /// </summary>
        public static double CrossProduct(Point2D a, Point2D b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        /// <summary>
        /// Вычислить скалярное произведение векторов
        /// </summary>
        public static double DotProduct(Point2D a, Point2D b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// Вычислить длину радиус-вектора
        /// </summary>
        /// <returns></returns>
        public static double GetVectorLength(Point2D vec)
        {
            return Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        }

        /// <summary>
        /// Вычислить квадрат длины радиус-вектора
        /// </summary>
        /// <returns></returns>
        public static double GetSquaredVectorLength(Point2D vec)
        {
            return vec.X * vec.X + vec.Y * vec.Y;
        }
    }
}
