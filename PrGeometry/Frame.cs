using System;

namespace Geometry
{
    /// <summary>
    /// Frame
    /// </summary>
    public class Frame
    {
        public Point3D X { get; }
        public Point3D Y { get; }
        public Point3D Z { get; }
        public Point3D Centre { get; }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{Centre};{X};{Y};{Z}";
        }

        public Frame(Point3D centre,Point3D x, Point3D y, Point3D z)
        {
            var dirX = x.Sub(centre);
            var dirY = y.Sub(centre);
            var dirZ = z.Sub(centre);

            var centerXSub = Math.Abs(Vector.GetVectorLenght(dirX)) <= 1e-5;
            var centerYSub = Math.Abs(Vector.GetVectorLenght(dirY)) <= 1e-5;
            var centerZSub = Math.Abs(Vector.GetVectorLenght(dirZ)) <= 1e-5;
            if (centerXSub || centerYSub || centerZSub)
                throw new Exception("Расстояние между концами осей близко или равно 0.0");

            var XYCos = Math.Abs(Vector.GetCosAngleVectors(dirX, dirY)) >= 1e-2;
            var XZCos = Math.Abs(Vector.GetCosAngleVectors(dirX, dirZ)) >= 1e-2;
            var YZCos = Math.Abs(Vector.GetCosAngleVectors(dirZ, dirY)) >= 1e-2;
            if (XYCos || XZCos || YZCos)
                throw new Exception("Используемые вектора не являются ортогональными по отношению друг к другу");

            X = x;
            Y = y;
            Z = z;
            Centre = centre;
        }

        public Frame()
        {
            Z = new Point3D(0,0,1);
            Y = new Point3D(0,1,0);
            X = new Point3D(1,0,0);
            Centre = new Point3D();
        }

        public Frame(Point3D p0, Point3D p1, Point3D p2)
        {
            if (p0 == p1 || p0 == p2)
                throw new Exception("Попытка построения поверхности с помощью одной точки");

            var plane = new Plane(p0, p2, p1);
            var dirZ = plane.Normal;
            var dirX = p2.Sub(p0);

            var dirY = Vector.CrossProd(dirZ, dirX);

            Z = new Point3D(p0._x + dirZ._x, p0._y + dirZ._y, p0._z + dirZ._z);
            Y = new Point3D(p0._x + dirY._x, p0._y + dirY._y, p0._z + dirY._z);
            X = new Point3D(p0._x + dirX._x, p0._y + dirX._y, p0._z + dirX._z);
            Centre = p0;
        }

        public Frame Shift(Point3D shifting)
        {
            var dirX = X.Sub(Centre);
            var lenX = Vector.GetVectorLenght(dirX);
            var sclX = shifting._x / lenX;
            var hsCentrX = new Point3D(Centre._x + sclX * dirX._x, Centre._y + sclX * dirX._y, Centre._z + sclX * dirX._z);
            var deltaX = hsCentrX.Sub(Centre);

            var dirY = Y.Sub(Centre);
            var lenY = Vector.GetVectorLenght(dirY);
            var sclY = shifting._y / lenY;
            var hsCentrY = new Point3D(Centre._x + sclY * dirY._x, Centre._y + sclY * dirY._y, Centre._z + sclY * dirY._z);
            var deltaY = hsCentrY.Sub(Centre);

            var dirZ = Z.Sub(Centre);
            var lenZ = Vector.GetVectorLenght(dirZ);
            var sclZ = shifting._z / lenZ;
            var hsCentrZ = new Point3D(Centre._x + sclZ * dirZ._x, Centre._y + sclZ * dirZ._y, Centre._z + sclZ * dirZ._z);
            var deltaZ = hsCentrZ.Sub(Centre);


            var delta = deltaX.Sum(deltaY).Sum(deltaZ);
            var sCentr = Centre.Sum(delta);
            var sX = X.Sum(delta);
            var sY = Y.Sum(delta);
            var sZ = Z.Sum(delta);

            return new Frame(sCentr,sX, sY, sZ);
        }

        public Frame Rotate(Point3D axis, float angle)
        {
            var dirX = X.Sub(Centre);
            var rotX = Vector.GetVectorRotation(dirX, axis, angle);
            var x = Centre.Sum(rotX); //new Point3D(Centre._x + rotX._x, Centre._y + rotX._y, Centre._z + rotX._z);

            var dirY = Y.Sub(Centre);
            var rotY = Vector.GetVectorRotation(dirY, axis, angle);
            var y = Centre.Sum(rotY);

            var dirZ = Z.Sub(Centre);
            var rotZ = Vector.GetVectorRotation(dirZ, axis, angle);
            var z = Centre.Sum(rotZ);

            return new Frame(Centre,x, y, z);
        }
    }
}
