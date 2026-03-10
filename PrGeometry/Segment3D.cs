using System;
using System.Collections.Generic;

namespace Geometry
{
    /// <summary>
    /// Segment3D
    /// </summary>
    public class Segment3D
    {
        /// <summary>
        /// P0
        /// </summary>
        public Point3D P0 { get; }
        /// <summary>
        /// P1
        /// </summary>
        public Point3D P1 { get; }
        /// <summary>
        /// Segment3D
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public Segment3D(Point3D p0, Point3D p1)
        {
            P0 = p0;
            P1 = p1;
        }
        /// <summary>
        /// GetDirectVector
        /// </summary>
        /// <returns></returns>
        public Point3D GetDirectVector()
        {
            return P1.Sub(P0);
        }
        /// <summary>
        /// GetLength
        /// </summary>
        /// <returns></returns>
        public float GetLength()
        {
            return (float)Math.Sqrt(Math.Pow((P1._x - P0._x), 2) + Math.Pow((P1._y - P0._y), 2) + Math.Pow((P1._z - P0._z), 2));
        }

        public Point3D GetPointOnLine(float subLineLength)
        {
            var point3D = new Point3D();


            if (GetLength() == 0) return new Point3D();

            point3D._x = subLineLength * ((P1._x - P0._x) / GetLength());
            point3D._y = subLineLength * ((P1._y - P0._y) / GetLength());
            point3D._z = subLineLength * ((P1._z - P0._z) / GetLength());

            return point3D;
        }

        public Point3D GetPlaneIntersection(ref bool state, Plane plane)
        {
            var vectorDirection = new Point3D(P1._x - P0._x, P1._y - P0._y, P1._z - P0._z);            
            if (Vector.DotProd(vectorDirection, plane.Normal) == 0)
                throw new Exception("Прямая параллельна плоскости");

            if (PointOnPlane(plane, P0) == 0) return P0;
            else if(PointOnPlane(plane, P1) == 0) return P1;

            var p0Pr = P0.GetPointProectionOnPlane(plane);
            var p1Pr = P1.GetPointProectionOnPlane(plane);

            var v0 = P0.Sub(p0Pr);
            var v1 = P1.Sub(p1Pr);

            state = Vector.IsVectorsCoDirectional(new List<Point3D>() { v0, v1 });
            if (state) return new Point3D();
            else
            {
                var l0 = Vector.GetVectorLength(v0);
                var l1 = Vector.GetVectorLength(v1);

                var lenSub0 = (1 - (l0 / (l0 + l1)));
                var lenSub1 = (l0 / (l0 + l1));
                p0Pr = p0Pr.Mult(lenSub0);
                p1Pr = p1Pr.Mult(lenSub1);
                p0Pr = p0Pr.Sum(p1Pr);

                return p0Pr;
            }

        }

        float PointOnPlane (Plane plane, Point3D point)
        {
            return plane.Normal._x * point._x + plane.Normal._y * point._y + plane.Normal._z * point._z + plane.Shifting;
        }

    }
}
