using System;
using System.Collections.Generic;
using ModelInterfaces;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra;

namespace Model.MeshObjects
{
    public class Triangle : Element2D
    {
        private float[][] trianglePoints;
/// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Треугольник;

        public Triangle(int number, Node[] nodes) : base(number, nodes)
        {
            surface = new Surface(0, new Node[] { nodes[0], nodes[1], nodes[2] });
            trianglePoints = new[]
            {
                new[]{1/2f,0f,1/2f},
                new[]{1/2f,1/2f,0f},
                new[]{0,1/2f,1/2f}
            };
        }
//        /// <inheritdoc/>

//        public override float[] GetDerivativesFormFunctions(Point3D point3D)
//        {
//            return DerivativeFormFunctionTriangle().ToColumnMajorArray();
//        }
///// <inheritdoc/>

//        public override float[] GetFormFunctions(Point3D point3D)
//        {
//            return FormFunctionTriangle(point3D);
//        }
/// <inheritdoc/>

        //public float[][] CoefficientKTriangle(float t, float k, float h, int firstPoint, int secondPoint) =>
        //    VolumeIntegralBdbTriangle(t, k).Add(SquareIntegralNnTriangle(h, firstPoint, secondPoint)).ToRowArrays();

        //public float[][] CoefficientCTriangle(float t)
        //{
        //    return VolumeIntegralNnTriangle(t).ToRowArrays();
        //}

        //public float[] CoefficientFTriangle(float tInf, float h, int firstPoint, int secondPoint)
        //{
        //    return SquareIntegralN(firstPoint, secondPoint).Multiply(tInf * h).ToArray();
        //}

        public override float CalcSquare()
        {
            var pAr = GetPoints().ToArray();

            var v1 = new Point3D(pAr[1]._x - pAr[0]._x, pAr[1]._y - pAr[0]._y, pAr[1]._z - pAr[0]._z);
            var v2 = new Point3D(pAr[2]._x - pAr[0]._x, pAr[2]._y - pAr[0]._y, pAr[2]._z - pAr[0]._z);

            var vProd = Geometry.Vector.CrossProd(v1, v2);
            var square = Math.Abs(Geometry.Vector.GetVectorLenght(vProd) / 2);
            return square;
        }

        private float[] FormFunctionTriangle(Point3D point3D)
        {
            var answer = new float[3];
            var s = Math.Abs(Pseudoscalar(nodes[0].Position, nodes[1].Position, nodes[2].Position));
            for (var i = 0; i < 3; i++)
            {
                var s1 = Math.Abs(Pseudoscalar(point3D, nodes[i].Position, nodes[(i + 1) % 3].Position));
                answer[(i + 2) % 3] = s1 / s;
            }

            return answer;
        }

        private Matrix<float> DerivativeFormFunctionTriangle()
        {
            var matrix = new DenseMatrix(2, 3);
            for (var i = 0; i < 3; i++)
            {
                matrix[0, i] = nodes[(i + 1) % 3].Position._y - nodes[(i + 2) % 3].Position._y;
                matrix[1, i] = nodes[(i + 1) % 3].Position._x - nodes[(i + 2) % 3].Position._x;
            }

            return matrix.Multiply(1f / Pseudoscalar(nodes[0].Position, nodes[1].Position, nodes[2].Position));
        }
        //private Matrix<float> CountBTriangle()
        //{
        //    var matrix = new DenseMatrix(2, 3);
        //    for (var i = 0; i < 3; i++)
        //    {
        //        matrix[0, i] = nodes[(i + 1) % 3].Position._y - nodes[(i + 2) % 3].Position._y;
        //        matrix[1, i] = nodes[(i + 1) % 3].Position._x - nodes[(i + 2) % 3].Position._x;
        //    }

        //    return matrix.Multiply(1f / Pseudoscalar(nodes[0].Position, nodes[1].Position, nodes[2].Position));
        //}

        public override float DetJ(Point3D point)
        {
            var m = new DenseMatrix(2, 2)
            {
                [0, 0] = nodes[0].Position._x - nodes[2].Position._x,
                [0, 1] = nodes[0].Position._y - nodes[2].Position._y,
                [1, 0] = nodes[1].Position._x - nodes[2].Position._x,
                [1, 1] = nodes[1].Position._y - nodes[2].Position._y
            };
            return m.Determinant();
        }

        //private Matrix<float> VolumeIntegralBdbTriangle(float t, float k)
        //{
        //    var d = new DiagonalMatrix(2, 2, k);
        //    var b = CountBTriangle();
        //    var s = Pseudoscalar(nodes[0].Position, nodes[1].Position, nodes[2].Position) / 2f;
        //    var j = DetJTriangle();
        //    return b.Transpose().Multiply(d).Multiply(b).Multiply(t * j * s);
        //}

        //private Matrix<float> SquareIntegralNnTriangle(float h, int firstPoint, int secondPoint)
        //{
        //    var constPoint = 3 - firstPoint - secondPoint;
        //    var n = new DenseMatrix(1, 3)
        //    {
        //        [0, firstPoint] = 1 / 2f,
        //        [0, secondPoint] = 1 / 2f,
        //        [0, constPoint] = 0
        //    };
        //    var first = nodes[firstPoint].Position;
        //    var second = nodes[secondPoint].Position;
        //    var sideLength = (first._x - second._x).Square() + (first._y - second._y).Square();

        //    return n.Transpose().Multiply(n).Multiply(h * sideLength);
        //}

        //private Matrix<float> VolumeIntegralNnTriangle(float t)
        //{
        //    var sum = new DenseMatrix(3, 3).Multiply(0);

        //    foreach (var n in trianglePoints)
        //    {
        //        var matrix = new DenseMatrix(1, 3, n);
        //        var transposed = matrix.Transpose();
        //        sum = sum.Add(transposed.Multiply(matrix));
        //    }
        //    return sum.Multiply(DetJTriangle() * t);
        //}

        //private Vector<float> SquareIntegralN(int firstPoint, int secondPoint)
        //{
        //    var constPoint = 3 - firstPoint - secondPoint;
        //    var first = nodes[firstPoint].Position;
        //    var second = nodes[secondPoint].Position;
        //    var sideLength = (first._x - second._x).Square() + (first._y - second._y).Square();
        //    var n = new DenseVector(3)
        //    {
        //        [firstPoint] = 1 / 2f,
        //        [secondPoint] = 1 / 2f,
        //        [constPoint] = 0
        //    };
        //    return n.Multiply(sideLength);
        //}


        private float Pseudoscalar(Point3D i, Point3D j, Point3D k) =>
            (j._x - i._x) * (k._y - j._y) - (k._x - j._x) * (j._y - i._y);
    }
}
