using System;
using System.Globalization;
using Geometry.Attribute;
using Geometry.Interfaces;

namespace Geometry
{
    /// <summary>
    /// Point2D
    /// </summary>
    public class Point2D : IPoint2D, IPointFunctions<Point2D>
    {

        // Для расчета GetHashCode(), в 10 раз отличается от 1/INV_FACTOR для того,
        // чтобы у близких объектов, которые могут оказаться равными был одинаковый хеш
        private const int FACTOR = 1000;
        private const float INV_FACTOR = 1e-4f;

        public double _x { get; set; }

        public double _y { get; set; }

        /// <summary>
        /// Point3D
        /// </summary>
        public Point2D()
        { _x = 0; _y = 0; }

        public Point2D(double x, double y)
        {           
            _x = x; 
            _y = y;
        }
/// <inheritdoc/>

        public bool Equals(Point2D other)
        {
            if (this == other)
                return true;
            return false;
        }
/// <inheritdoc/>

        public override int GetHashCode()
        {
            var x = (int)(_x * FACTOR);
            var y = (int)(_y * FACTOR);
            return ((x << 5) ^ y) ^ 31;
        }
        /// <inheritdoc/>

        public override string ToString()
{
            return string.Format(CultureInfo.InvariantCulture,"{0}|{1}", _x, _y);
        }
/// <inheritdoc/>


        public Point2D Sum(Point2D AddedValue)
        {
            return new Point2D(_x + AddedValue._x, _y + AddedValue._y);
        }
/// <inheritdoc/>

        public Point2D Sub(Point2D SubstractValue)
        {
            return new Point2D(_x - SubstractValue._x, _y - SubstractValue._y);
        }
/// <inheritdoc/>

        public Point2D Div(float divide)
        {
            return new Point2D(_x / divide, _y / divide);
        }
/// <inheritdoc/>

        public Point2D Mult(float multiplier)
        {
            return new Point2D(_x * multiplier, _y * multiplier);
        }
/// <inheritdoc/>

        public double GetSquareSumCoords()
        {
            return _x * _x + _y * _y;
        }

        /// <summary>
        /// Проверка на равенство
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        [Warning("Не использовать оператор при работе с словарями!")]
        public static bool operator ==(Point2D c1, Point2D c2)
        {
            return Math.Abs(c1._x - c2._x) <= INV_FACTOR &&
                   Math.Abs(c1._y - c2._y) <= INV_FACTOR;
        }
        /// <summary>
        /// Проверка на неравенство
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return !(p1 == p2);
        }
    }
}
