using System;

namespace Geometry
{
    /// <summary>
    /// Frame
    /// </summary>
    public class Frame
    {
        public Point3D Dir_X { get; private set; }
        public Point3D Dir_Y { get; private set; }
        public Point3D Dir_Z { get; private set; }
        public Point3D Centre { get; private set; }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{Centre};{Dir_X};{Dir_Y};{Dir_Z}";
        }

        //public Frame(Point3D centre, Point3D x, Point3D y, Point3D z)
        //{
        //    var dirX = x.Sub(centre);
        //    var dirY = y.Sub(centre);
        //    var dirZ = z.Sub(centre);

        //    var centerXSub = Math.Abs(Vector.GetVectorLenght(dirX)) <= 1e-5;
        //    var centerYSub = Math.Abs(Vector.GetVectorLenght(dirY)) <= 1e-5;
        //    var centerZSub = Math.Abs(Vector.GetVectorLenght(dirZ)) <= 1e-5;
        //    if (centerXSub || centerYSub || centerZSub)
        //        throw new Exception("Расстояние между концами осей близко или равно 0.0");

        //    var XYCos = Math.Abs(Vector.GetCosAngleVectors(dirX, dirY)) >= 1e-2;
        //    var XZCos = Math.Abs(Vector.GetCosAngleVectors(dirX, dirZ)) >= 1e-2;
        //    var YZCos = Math.Abs(Vector.GetCosAngleVectors(dirZ, dirY)) >= 1e-2;
        //    if (XYCos || XZCos || YZCos)
        //        throw new Exception("Используемые вектора не являются ортогональными по отношению друг к другу");

        //    X = x;
        //    Y = y;
        //    Z = z;
        //    Centre = centre;
        //}

        public Frame()
        {
            Dir_Z = new Point3D(0,0,1);
            Dir_Y = new Point3D(0,1,0);
            Dir_X = new Point3D(1,0,0);
            Centre = new Point3D();
        }

        public Frame(Point3D p0, Point3D p1, Point3D p2)
        {
            if (p0 == p1 || p0 == p2)
                throw new Exception("Попытка построения поверхности с помощью одной точки");

            var plane = new Plane(p0, p2, p1);
            Dir_Z = plane.Normal;
            Dir_Z = Vector.GetVectorNorm(Dir_Z);

            Dir_X = p2.Sub(p0);
            Dir_X = Vector.GetVectorNorm(Dir_X);

            Dir_Y = Vector.CrossProd(Dir_Z, Dir_X);

            //Dir_Z = new Point3D(p0._x + dirZ._x, p0._y + dirZ._y, p0._z + dirZ._z);
            //Dir_Y = new Point3D(p0._x + dirY._x, p0._y + dirY._y, p0._z + dirY._z);
            //Dir_X = new Point3D(p0._x + dirX._x, p0._y + dirX._y, p0._z + dirX._z);
            Centre = p0;
        }

        public Frame Shift(Point3D shifting)
        {
            //var dirX = Dir_X.Sub(Centre);
            var lenX = Vector.GetVectorLenght(Dir_X);
            var sclX = shifting._x / lenX;
            var hsCentrX = new Point3D(Centre._x + sclX * Dir_X._x, Centre._y + sclX * Dir_X._y, Centre._z + sclX * Dir_X._z);
            var deltaX = hsCentrX.Sub(Centre);

            //var dirY = Dir_Y.Sub(Centre);
            var lenY = Vector.GetVectorLenght(Dir_Y);
            var sclY = shifting._y / lenY;
            var hsCentrY = new Point3D(Centre._x + sclY * Dir_Y._x, Centre._y + sclY * Dir_Y._y, Centre._z + sclY * Dir_Y._z);
            var deltaY = hsCentrY.Sub(Centre);

            //var dirZ = Dir_Z.Sub(Centre);
            var lenZ = Vector.GetVectorLenght(Dir_Z);
            var sclZ = shifting._z / lenZ;
            var hsCentrZ = new Point3D(Centre._x + sclZ * Dir_Z._x, Centre._y + sclZ * Dir_Z._y, Centre._z + sclZ * Dir_Z._z);
            var deltaZ = hsCentrZ.Sub(Centre);


            var delta = deltaX.Sum(deltaY).Sum(deltaZ);
            var sCentr = Centre.Sum(delta);
            //var sX = Dir_X.Sum(delta);
            //var sY = Dir_Y.Sum(delta);
            //var sZ = Dir_Z.Sum(delta);

            return new Frame(sCentr, Dir_Y, Dir_X);
        }

        public Frame Rotate(Point3D axis, float angle)
        {
            //var dirX = Dir_X.Sub(Centre);
            var dir_X = Vector.GetVectorRotation(Dir_X, axis, angle);
            //var x = Centre.Sum(rotX); //new Point3D(Centre._x + rotX._x, Centre._y + rotX._y, Centre._z + rotX._z);

            //var dirY = Dir_Y.Sub(Centre);
            var dir_Y = Vector.GetVectorRotation(Dir_Y, axis, angle);
            //var y = Centre.Sum(rotY);

            //var dirZ = Dir_Z.Sub(Centre);
            //var rotZ = Vector.GetVectorRotation(dirZ, axis, angle);
            //var z = Centre.Sum(rotZ);

            return new Frame(Centre, dir_Y, dir_X);
        }
    }
}
