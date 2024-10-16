using Geometry.Interfaces;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    /// <summary>
    /// Plane class
    /// </summary>
    public class Plane : IPlane
    {
        /// <inheritdoc/>
        public Point3D Normal { get; }
/// <inheritdoc/>

        public float Shifting { get; set; }
        /// <summary>
        /// Plane
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public Plane(Point3D p0, Point3D p1, Point3D p2)
        {
            //var x1 = p0._x; var x2 = p1._x; var x3 = p2._x;
            //var y1 = p0._y; var y2 = p1._y; var y3 = p2._y;
            //var z1 = p0._z; var z2 = p1._z; var z3 = p2._z;

            var fvectors = p1.Sub(p0);  //new Point3D(p1._x - p0._x, p1._y - p0._y, p1._z - p0._z);
            var svectors = p2.Sub(p0); //new Point3D(p2._x - p0._x, p2._y - p0._y, p2._z - p0._z);

            Normal = Vector.CrossProd(fvectors, svectors);
            if (Vector.GetVectorLenght(Normal) < 1e-6)
                throw new Exception("Вектор нормали плоскости равен нулю");
            Shifting =  -(Normal._x * p0._x + Normal._y * p0._y + Normal._z * p0._z);
        }
        /// <summary>
        /// Plane
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="normal"></param>
        public Plane(Point3D p0, Point3D normal)
        {
            if (Vector.GetVectorLenght(normal) < 1e-6)
                throw new Exception("Вектор нормали плоскости равен нулю");
            Normal = normal;
            Shifting = -(Normal._x * p0._x + Normal._y * p0._y + Normal._z * p0._z);
        }
        /// <summary>
        /// Plane
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="shifting"></param>
        public Plane(Point3D normal, float shifting)
        {
            if (Vector.GetVectorLenght(normal) < 1e-6)
                throw new Exception("Вектор нормали плоскости равен нулю");
            Normal = normal;
            Shifting = shifting;
        }
/// <inheritdoc/>


        public float PointPosition(Point3D point)
        {
            var lenght = Vector.GetVectorLenght(Normal);

            return (Normal._x * point._x + Normal._y * point._y + Normal._z * point._z + Shifting) / lenght;
        }
/// <inheritdoc/>

        public float PlanePosition(Plane plane)
        {      
            return (plane.Shifting - Shifting) / Vector.GetVectorLenght(Normal);
        }
    }
}
