using Geometry.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Geometry
{
    /// <summary>
    /// Библиотека "Вектор"
    /// </summary>
    public static class Vector
    {
        /// <summary>
        /// Проверить вектора на сонаправленность
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static bool IsVectorsCoDirectional(List<Point3D> temp)
        {
            var markList_a = new int[temp.Count];
            var markList_b = new int[temp.Count];
            var markList_c = new int[temp.Count];

            for (int i = 0; i < temp.Count; i++)
            {
                markList_a[i] = Math.Sign(temp[i]._x);
                markList_b[i] = Math.Sign(temp[i]._y);
                markList_c[i] = Math.Sign(temp[i]._z);
            }

            if (markList_a.Contains(-1) && markList_a.Contains(1))
                return false;
            if (markList_b.Contains(-1) && markList_b.Contains(1))
                return false;
            if (markList_c.Contains(-1) && markList_c.Contains(1))
                return false;

            return true;
        }
        /// <summary>
        /// GetVectorLength
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double GetVectorLength(Point2D p)
        {
            return Math.Sqrt(Math.Pow(p._x, 2) + Math.Pow(p._y, 2));
        }


        /// <summary>
        /// Найти длину вектора
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double GetVectorLength(Point3D p)
        {
            return Math.Sqrt(Math.Pow(p._x, 2) + Math.Pow(p._y, 2) + Math.Pow(p._z, 2));
        }
        /// <summary>
        /// Найти норму вектора
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Point3D GetVectorNorm(Point3D p)
        {
            var length = GetVectorLength(p);
            var invLength = 1 / length;

            return p.Mult(invLength);
        }
        /// <summary>
        /// Вращение вектора вокруг оси на угол
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Point3D GetVectorRotation(Point3D vec, Point3D axis, float angle)
        {

            var cos = Math.Cos(angle * Math.PI / 180);
            var sin = Math.Sin(angle * Math.PI / 180);

            var m00 = cos + (1 - cos) * axis._x * axis._x;
            var m01 = (1 - cos) * axis._x * axis._y - sin * axis._z;
            var m02 = (1 - cos) * axis._x * axis._z + sin * axis._y;

            var m10 = (1 - cos) * axis._y * axis._x + sin * axis._z;
            var m11 = cos + (1 - cos) * axis._y * axis._y;
            var m12 = (1 - cos) * axis._y * axis._z - sin * axis._x;

            var m20 = (1 - cos) * axis._z * axis._x - sin * axis._y;
            var m21 = (1 - cos) * axis._z * axis._y + sin * axis._x;
            var m22 = cos + (1 - cos) * axis._z * axis._z;

            var array = new[,]
            {
                { m00, m01,m02 },
                { m10, m11,m12 },
                { m20, m21,m22 }
            };


            var vecIni = Vector<double>.Build.DenseOfArray(new double[] { vec._x, vec._y, vec._z });
            var matrix = Matrix<double>.Build.DenseOfArray(array);
            var vecRot = matrix.Multiply(vecIni);
            return new Point3D(vecRot[0], vecRot[1], vecRot[2]);
        }
        /// <summary>
        /// Векторное произведение векторов
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Point3D CrossProd(Point3D v1, Point3D v2)
        {
            var result = new Point3D();
            var a = new double[] { v1._x, v1._y, v1._z };
            var b = new double[] { v2._x, v2._y, v2._z };

            var x = a[1] * b[2] - a[2] * b[1];
            var y = a[2] * b[0] - a[0] * b[2];
            var z = a[0] * b[1] - a[1] * b[0];

            result._x = x;
            result._y = y;
            result._z = z;

            return result;
        }

        /// <summary>
        /// Скалярное произведение векторов
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double DotProd(Point3D v1, Point3D v2)
        {
            var a = v1._x * v2._x;
            var b = v1._y * v2._y;
            var c = v1._z * v2._z;

            return a + b + c;
        }
        /// <summary>
        /// Сложение векторов
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Point3D GetVectorsSumm(Point3D v1, Point3D v2)
        {
            Point3D result = new Point3D();
            var a = new double[] { v1._x, v1._y, v1._z };
            var b = new double[] { v2._x, v2._y, v2._z };

            result._x = a[0] + b[0];
            result._y = a[1] + b[1];
            result._z = a[2] + b[2];

            return result;
        }
        /// <summary>
        /// Cos угла между векторами
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <returns></returns>
        public static double GetCosAngleVectors(Point3D n1, Point3D n2)
        {
            var prod1 = DotProd(n1,n2);

            var n1_lenght = GetVectorLength(n1);
            var n2_lenght = GetVectorLength(n2);
            return prod1 / (n1_lenght * n2_lenght);
        }

        /// <summary>
        /// Check collinearity vectors (vector on one straight line)
        /// </summary>
        /// <returns></returns>
        public static bool CheckCollinearityVectors(Point3D vector1, Point3D vector2)
        {
            if (vector1 == new Point3D(0, 0, 0) || vector2 == new Point3D(0, 0, 0))
                return true;

            return 1 - Math.Abs(GetCosAngleVectors(vector1, vector2)) <= 1e-5;       
        }

    }
}
