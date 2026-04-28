using System;

namespace Geometry
{
    public class Frame2D
    {
        private readonly Point3D origin;
        private readonly Point3D xAxis;
        private readonly Point3D yAxis;

        public Frame2D(Point3D p0, Point3D p1, Point3D p2)
        {
            origin = p0;
            xAxis = Vector.GetVectorNorm(p1.Sub(p0));
            var normal = Vector.CrossProd(xAxis, p2.Sub(p0));
            yAxis = Vector.GetVectorNorm(Vector.CrossProd(normal, xAxis));
        }

        public Point2D MapToLocalPlaneCoordinates(Point3D point)
        {
            var x = (float)Math.Round(Vector.DotProd(point.Sub(origin), xAxis), 4);
            var y = (float)Math.Round(Vector.DotProd(point.Sub(origin), yAxis), 4);
            return new Point2D(x, y);
        }

        public Point3D UnmapToGlobalCoordinates(Point2D point)
        {
            var x = (float)Math.Round(origin._x + point._x * xAxis._x + point._y * yAxis._x, 4);
            var y = (float)Math.Round(origin._y + point._x * xAxis._y + point._y * yAxis._y, 4);
            var z = (float)Math.Round(origin._z + point._x * xAxis._z + point._y * yAxis._z, 4);
            return new Point3D(x, y, z);
        }
    }
}
