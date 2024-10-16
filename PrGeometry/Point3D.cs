using System;
using System.Collections.Generic;
using System.Globalization;
using Geometry.Attribute;
using Geometry.Interfaces;

namespace Geometry
{   
    /// <summary>
    /// Point3D
    /// </summary>
    public class Point3D : IPoint3D, IPointFunctions<Point3D>
    {
        /// <inheritdoc/>
        // Для расчета GetHashCode(), в 10 раз отличается от 1/INV_FACTOR для того,
        // чтобы у близких объектов, которые могут оказаться равными был одинаковый хеш
        private const int FACTOR = 1000;
        private const float INV_FACTOR = 1e-4f;

        public float _x { get; set; }
/// <inheritdoc/>

        public float _y { get; set; }
        /// <inheritdoc/>

        public float _z { get; set; }

        /// <summary>
        /// Point3D
        /// </summary>
        public Point3D()
        { _x = 0; _y = 0; _z = 0; }
        /// <summary>
        /// Point3D
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Point3D(float x, float y, float z)
        { _x = x; _y = y; _z = z; }
/// <inheritdoc/>

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}", _x, _y, _z);
        }
/// <inheritdoc/>

        public Point3D Sum(Point3D AddedValue)
        {
            return new Point3D(_x + AddedValue._x, _y + AddedValue._y, _z + AddedValue._z);
        }
/// <inheritdoc/>

        public Point3D Sub(Point3D SubstractValue)
        {
            return new Point3D(_x - SubstractValue._x, _y - SubstractValue._y, _z - SubstractValue._z);
        }
/// <inheritdoc/>

        public Point3D Div(float divide)
        {
            return new Point3D(_x / divide, _y / divide, _z / divide);
        }
/// <inheritdoc/>

        public Point3D Mult(float multiplier)
        {
            return new Point3D(_x * multiplier, _y * multiplier, _z * multiplier);
        }
/// <inheritdoc/>

        public float GetSquareSumCoords()
        {
            return _x * _x + _y * _y + _z * _z;
        }
/// <inheritdoc/>

        public Point3D GetPointProectionOnPlane(Plane plane)
        {
            var projectedPoint = new Point3D(0,0,0);

            var nx = plane.Normal._x;
            var ny = plane.Normal._y;
            var nz = plane.Normal._z;

            var nx2 = nx * nx;
            var ny2 = ny * ny;
            var nz2 = nz * nz;

            var squaresumm = (nx2 + ny2 + nz2);

            var d = plane.Shifting;

            var t = -(nx * _x + ny * _y + nz * _z + d) / squaresumm;

            projectedPoint._x = _x + nx * t; // surface normal coord x
            projectedPoint._y = _y + ny * t; // surface normal coord y
            projectedPoint._z = _z + nz * t; // surface normal coord z

            return projectedPoint;
        }
        /// <summary>
        /// Рассчитать растояние до линии
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public float GetDistanceToLine(Segment3D line)
        {
            var directVec = line.GetDirectVector();

            var tempVec = Sub(line.P0);

            var vecProd = Vector.CrossProd(tempVec, directVec);

            var distance = Vector.GetVectorLenght(vecProd) / Vector.GetVectorLenght(directVec);

            return distance;
        }
/// <inheritdoc/>

        public bool IsBelongTo(Plane plane)
        {
            var normal = plane.Normal;

            if (
                Math.Abs(normal._x * _x +
                normal._y * _y +
                normal._z * _z + plane.Shifting) < 1e-2
                )
                return true;
            else return false;
        }
        /// <summary>
        /// Equals to other point
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Point3D other)
        {
            if (this == other)
                return true;
            else return false;
        }
/// <inheritdoc/>

        public override int GetHashCode()
        {
            var x = (int)(_x * FACTOR);
            var y = (int)(_y * FACTOR);
            var z = (int)(_z * FACTOR);
            return ((((x << 5) ^ y) << 5) ^ z) ^ 31;
        }
        /// <summary>
        /// equal operator
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        [Warning("Не использовать оператор при работе с словарями!")]
        public static bool operator ==(Point3D c1, Point3D c2)
        {
            return Math.Abs(c1._x - c2._x) <= INV_FACTOR &&
                   Math.Abs(c1._y - c2._y) <= INV_FACTOR &&
                   Math.Abs(c1._z - c2._z) <= INV_FACTOR;
        }
        /// <summary>
        /// not equal operator
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator !=(Point3D p1, Point3D p2)
        {
            return !(p1 == p2);
        }
/// <inheritdoc/>

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is Point3D point3D)
                return Equals(point3D);
            else return false;
        }
    }
}
