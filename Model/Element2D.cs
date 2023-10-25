using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Model
{
    public class Element2D : SurfaceElement, IElement2D
    {
        Surface surface;
        private float[][] trianglePoints;
        public Element2D(int number, Node[] nodes, ObjKind objKind) : base(number, nodes, objKind)
        {
            InitialColor = Color.FromArgb(0, 0, 255);
            MasterColor = Color.FromArgb(0, 0, 255);
            SlaveColor = Color.FromArgb(0, 0, 255);
            ObjType = "Элементы2D";
            ObjKind = objKind;
            if (objKind == ObjKind.Triangle)
            {              
                surface = new Surface(0, new Node[] { nodes[0], nodes[1], nodes[2] },ObjKind.ElementSurface);
            }

            else if (objKind == ObjKind.Quad)
            {
                surface = new Surface(0, new Node[] { nodes[0], nodes[1], nodes[2], nodes[3] }, ObjKind.ElementSurface);
            }
        }

        public override ObjKind ObjKind { get; }

        public override string ObjType { get; }

        public override int NumberOfSides { get { return 1; } }

        public override Point3D CalcCentralPoint()
        {
            var center = new Point3D();

            foreach (var node in GetNodes())
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }

        public Point3D CalcNormal()
        {
            return surface.CalcNormal();
        }

        public float CalcSquare()
        {
            var pAr = GetPoints().ToArray();

            var v1 = new Point3D(pAr[1]._x - pAr[0]._x, pAr[1]._y - pAr[0]._y, pAr[1]._z - pAr[0]._z);
            var v2 = new Point3D(pAr[2]._x - pAr[0]._x, pAr[2]._y - pAr[0]._y, pAr[2]._z - pAr[0]._z);

            var vProd = Geometry.Vector.CrossProd(v1, v2);
            var square = Math.Abs(Geometry.Vector.GetVectorLenght(vProd) / 2);
            return square;
        }

        public override float[] GetFormFunctions(Point3D point3D)
        {
            switch (ObjKind)
            {
                case ObjKind.Triangle:
                    return FormFunctionTriangle(point3D);
                case ObjKind.Quad:
                    return FormFunctionSquare(point3D);
            }

            throw new NotImplementedException("Не найден вид объектов!");
        }

        public override float[] GetDerivativesFormFunctions(Point3D point3D)
        {
            switch (ObjKind)
            {
                case ObjKind.Triangle:
                    throw new NotImplementedException();
                case ObjKind.Quad:
                    return DerivativeFormFunctionSquare(point3D);
            }

            throw new NotImplementedException("Не найден вид объектов!");
        }
        public float[][] CoefficientKQuad(float k, float t, float h, int firstPoint, int secondPoint)
        {
            return IntegralInVolumeBdbQuad(k, t).Add(IntegralSquareNnQuad(firstPoint, secondPoint, h)).ToRowArrays();
        }

        public float[][] CoefficientCQuad(float t)
        {
            var supPoint = new[] { -(float)Math.Sqrt(0.6), 0, (float)Math.Sqrt(0.6) };
            var kef = new[] { 5 / 9f, 8 / 9f, 5 / 9f };
            var sum = new DenseMatrix(4, 4).Multiply(0);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var n = FormFunctionSquareInLocal(supPoint[i], supPoint[j]);
                    var m = new DenseMatrix(1, 4);
                    for (var k = 0; k < 4; k++)
                        m[0, k] = n[k];
                    var J = DetJQuad(supPoint[i], supPoint[j]);
                    sum = sum.Add(m.Transpose().Multiply(m).Multiply(J * kef[i] * kef[j]));
                }
            }

            return sum.Multiply(t).ToRowArrays();
        }

        public float[] CoefficientFQuad(float h, float tInf, int firstPoint, int secondPoint)
        {
            return IntegralSquareNQuad(firstPoint, secondPoint, h, tInf);
        }

        public float[][] CoefficientKTriangle(float t, float k, float h, int firstPoint, int secondPoint) =>
            VolumeIntegralBdbTriangle(t, k).Add(SquareIntegralNnTriangle(h, firstPoint, secondPoint)).ToRowArrays();

        public float[][] CoefficientCTriangle(float t)
        {
            return VolumeIntegralNnTriangle(t).ToRowArrays();
        }

        public float[] CoefficientFTriangle(float tInf, float h, int firstPoint, int secondPoint)
        {
            return SquareIntegralN(firstPoint, secondPoint).Multiply(tInf * h).ToArray();
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

        private float[] FormFunctionSquare(Point3D point3D)
        {
            var center = CalcCentralPoint();
            var supportVector = center.Sub(nodes[0].Position);
            var a = Math.Abs(supportVector._x);
            var b = Math.Abs(supportVector._y);
            var ksi = (point3D._x - center._x) / a;
            var eta = (point3D._y - center._y) / b;
            return FormFunctionSquareInLocal(ksi, eta);
        }

        private static float[] FormFunctionSquareInLocal(float ksi, float eta)
        {
            var answer = new List<float>();
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                    answer.Add((1 + i * ksi) * (1 + j * eta) / 4);
            return answer.ToArray();
        }

        private float[] DerivativeFormFunctionSquare(Point3D point3D)
        {
            var center = CalcCentralPoint();
            var supportVector = center.Sub(nodes[0].Position);
            var a = Math.Abs(supportVector._x);
            var b = Math.Abs(supportVector._y);
            var pointKsi = (point3D._x - center._x) / a;
            var pointEta = (point3D._y - center._y) / b;
            var answer = new List<float>();
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                {
                    answer.Add(i * (1 + j * pointEta) / 4);
                    answer.Add(j * (1 + i * pointKsi) / 4);
                }

            return answer.ToArray();
        }


        private Matrix<float> IntegralSquareNnQuad(int numberFirstPoint, int numberSecondPoint, float h)
        {
            var ans = new DenseMatrix(4, 4)
            {
                [numberFirstPoint, numberFirstPoint] = 2,
                [numberSecondPoint, numberSecondPoint] = 2,
                [numberFirstPoint, numberSecondPoint] = 1,
                [numberSecondPoint, numberFirstPoint] = 1
            };
            var z = (float)Math.Sqrt(
                (nodes[numberFirstPoint].Position._x - nodes[numberSecondPoint].Position._x).Square() +
                (nodes[numberFirstPoint].Position._y - nodes[numberSecondPoint].Position._y).Square());
            return ans.Multiply(h * z / 6f);
        }

        private float[] IntegralSquareNQuad(int numberFirstPoint, int numberSecondPoint, float h, float tInf)
        {
            var ans = new DenseVector(4)
            {
                [numberFirstPoint] = 1,
                [numberSecondPoint] = 1
            };
            var z = (float)Math.Sqrt(
                (nodes[numberFirstPoint].Position._x - nodes[numberSecondPoint].Position._x).Square() +
                (nodes[numberFirstPoint].Position._y - nodes[numberSecondPoint].Position._y).Square());
            return ans.Multiply(h * tInf * z / 2f).ToArray();
        }

        private Matrix<float> IntegralInVolumeBdbQuad(float k, float t)
        {
            var supPoint = new[] { -(float)Math.Sqrt(0.6), 0, (float)Math.Sqrt(0.6) };
            var kef = new[] { 5 / 9f, 8 / 9f, 5 / 9f };
            var d = DenseMatrix.CreateDiagonal(2, 2, k);
            var sum = new DenseMatrix(4, 4).Multiply(0);
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var bt = CountBQuad(supPoint[i], supPoint[j]).Transpose();
                    var b = bt.Transpose();
                    var J = DetJQuad(supPoint[i], supPoint[j]);
                    sum = sum.Add(bt.Multiply(d).Multiply(b).Multiply(J * kef[i] * kef[j]));
                }
            }

            return sum.Multiply(t);
        }

        private float DetJQuad(float ksi, float eta)
        {
            var matrix = CountBQuad(ksi, eta);
            var matrixPoints = new DenseMatrix(4, 2);

            for (var i = 0; i < nodes.Length; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
            }

            return matrix.Multiply(matrixPoints).Determinant();
        }

        private DenseMatrix CountBQuad(float ksi, float eta)
        {
            var matrix = new DenseMatrix(2, 4);
            var k = 0;
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                {
                    matrix[0, k] = (i * (1 + j * eta) / 4);
                    matrix[1, k] = (j * (1 + i * ksi) / 4);
                    k++;
                }

            return matrix;
        }

        private float Pseudoscalar(Point3D i, Point3D j, Point3D k) =>
            (j._x - i._x) * (k._y - j._y) - (k._x - j._x) * (j._y - i._y);

        private Matrix<float> CountBTriangle()
        {
            var matrix = new DenseMatrix(2, 3);
            for (var i = 0; i < 3; i++)
            {
                matrix[0, i] = nodes[(i + 1) % 3].Position._y - nodes[(i + 2) % 3].Position._y;
                matrix[1, i] = nodes[(i + 1) % 3].Position._x - nodes[(i + 2) % 3].Position._x;
            }

            return matrix.Multiply(1f / Pseudoscalar(nodes[0].Position, nodes[1].Position, nodes[2].Position));
        }

        private float DetJTriangle()
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

        private Matrix<float> VolumeIntegralBdbTriangle(float t, float k)
        {
            var d = new DiagonalMatrix(2, 2, k);
            var b = CountBTriangle();
            var s = Pseudoscalar(nodes[0].Position, nodes[1].Position, nodes[2].Position) / 2f;
            var j = DetJTriangle();
            return b.Transpose().Multiply(d).Multiply(b).Multiply(t * j * s);
        }

        private Matrix<float> SquareIntegralNnTriangle(float h, int firstPoint, int secondPoint)
        {
            var constPoint = 3 - firstPoint - secondPoint;
            var n = new DenseMatrix(1, 3)
            {
                [0, firstPoint] = 1 / 2f,
                [0, secondPoint] = 1 / 2f,
                [0, constPoint] = 0
            };
            var first = nodes[firstPoint].Position;
            var second = nodes[secondPoint].Position;
            var sideLength = (first._x - second._x).Square() + (first._y - second._y).Square();

            return n.Transpose().Multiply(n).Multiply(h * sideLength);
        }

        private Matrix<float> VolumeIntegralNnTriangle(float t)
        {
            var sum = new DenseMatrix(3, 3).Multiply(0);

            foreach (var n in trianglePoints)
            {
                var matrix = new DenseMatrix(1, 3, n);
                var transposed = matrix.Transpose();
                sum = sum.Add(transposed.Multiply(matrix));
            }
            return sum.Multiply(DetJTriangle() * t);
        }

        private Vector<float> SquareIntegralN(int firstPoint, int secondPoint)
        {
            var constPoint = 3 - firstPoint - secondPoint;
            var first = nodes[firstPoint].Position;
            var second = nodes[secondPoint].Position;
            var sideLength = (first._x - second._x).Square() + (first._y - second._y).Square();
            var n = new DenseVector(3)
            {
                [firstPoint] = 1 / 2f,
                [secondPoint] = 1 / 2f,
                [constPoint] = 0
            };
            return n.Multiply(sideLength);
        }

        public override IEnumerable<Surface> GetSurface()
        {
            yield return surface;
        }
    }
    public static class FloatOverride
    {
        public static float Square(this float value)
        {
            return value * value;
        }
    }
}
