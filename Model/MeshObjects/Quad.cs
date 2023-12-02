using Geometry;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.MeshObjects
{
    public class Quad : Element2D
    {
        public override ElementKind ElementKind { get; } = ElementKind.Квадрат;

        public Quad(int number, Node[] nodes) : base(number, nodes)
        {
            surface = new ElementSurface(0, new Node[] { nodes[0], nodes[1], nodes[2], nodes[3] });
        }
        //public override float[] GetFormFunctions(Point3D point3D)
        //{
        //    return FormFunctionSquare(point3D);
        //}

        //public override float[] GetDerivativesFormFunctions(Point3D point3D)
        //{
        //    return DerivativeFormFunctionSquare(point3D);
        //}


        //public float[][] CoefficientKQuad(float k, float t, float h, int firstPoint, int secondPoint)
        //{
        //    return IntegralInVolumeBdbQuad(k, t).Add(IntegralSquareNnQuad(firstPoint, secondPoint, h)).ToRowArrays();
        //}

        //public float[][] CoefficientCQuad(float t)
        //{
        //    var supPoint = new[] { -(float)Math.Sqrt(0.6), 0, (float)Math.Sqrt(0.6) };
        //    var kef = new[] { 5 / 9f, 8 / 9f, 5 / 9f };
        //    var sum = new DenseMatrix(4, 4).Multiply(0);
        //    for (int i = 0; i < 3; i++)
        //    {
        //        for (int j = 0; j < 3; j++)
        //        {
        //            var n = FormFunctionSquareInLocal(supPoint[i], supPoint[j]);
        //            var m = new DenseMatrix(1, 4);
        //            for (var k = 0; k < 4; k++)
        //                m[0, k] = n[k];
        //            var J = DetJQuad(supPoint[i], supPoint[j]);
        //            sum = sum.Add(m.Transpose().Multiply(m).Multiply(J * kef[i] * kef[j]));
        //        }
        //    }

        //    return sum.Multiply(t).ToRowArrays();
        //}

        //public float[] CoefficientFQuad(float h, float tInf, int firstPoint, int secondPoint)
        //{
        //    return IntegralSquareNQuad(firstPoint, secondPoint, h, tInf);
        //}

        public override float CalcSquare()
        {
            var sum = 0f;

            for (int i = 0; i < nodes.Count - 3; i++)
            {
                for (int j = i + 1; j < nodes.Count - 2; j++)
                {
                    for (int k = j + 1; k < nodes.Count - 1; k++)
                    {

                        var x = new Node[] { (Node)nodes[i], (Node)nodes[j], (Node)nodes[k] };
                        var tetra = new Triangle(0, x);
                        sum += tetra.CalcSquare();

                    }
                }
            }
            return sum / 2;
        }

        private float[] FormFunctionSquare(Point3D point3D)
        {
            var center = CalcCentr();
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
            var center = CalcCentr();
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

        public override float DetJ(Point3D point3D)
        {
            throw new NotImplementedException();
        }

        //private Matrix<float> IntegralSquareNnQuad(int numberFirstPoint, int numberSecondPoint, float h)
        //{
        //    var ans = new DenseMatrix(4, 4)
        //    {
        //        [numberFirstPoint, numberFirstPoint] = 2,
        //        [numberSecondPoint, numberSecondPoint] = 2,
        //        [numberFirstPoint, numberSecondPoint] = 1,
        //        [numberSecondPoint, numberFirstPoint] = 1
        //    };
        //    var z = (float)Math.Sqrt(
        //        (nodes[numberFirstPoint].Position._x - nodes[numberSecondPoint].Position._x).Square() +
        //        (nodes[numberFirstPoint].Position._y - nodes[numberSecondPoint].Position._y).Square());
        //    return ans.Multiply(h * z / 6f);
        //}

        //private float[] IntegralSquareNQuad(int numberFirstPoint, int numberSecondPoint, float h, float tInf)
        //{
        //    var ans = new DenseVector(4)
        //    {
        //        [numberFirstPoint] = 1,
        //        [numberSecondPoint] = 1
        //    };
        //    var z = (float)Math.Sqrt(
        //        (nodes[numberFirstPoint].Position._x - nodes[numberSecondPoint].Position._x).Square() +
        //        (nodes[numberFirstPoint].Position._y - nodes[numberSecondPoint].Position._y).Square());
        //    return ans.Multiply(h * tInf * z / 2f).ToArray();
        //}

        //private Matrix<float> IntegralInVolumeBdbQuad(float k, float t)
        //{
        //    var supPoint = new[] { -(float)Math.Sqrt(0.6), 0, (float)Math.Sqrt(0.6) };
        //    var kef = new[] { 5 / 9f, 8 / 9f, 5 / 9f };
        //    var d = DenseMatrix.CreateDiagonal(2, 2, k);
        //    var sum = new DenseMatrix(4, 4).Multiply(0);
        //    for (var i = 0; i < 3; i++)
        //    {
        //        for (var j = 0; j < 3; j++)
        //        {
        //            var bt = CountBQuad(supPoint[i], supPoint[j]).Transpose();
        //            var b = bt.Transpose();
        //            var J = DetJQuad(supPoint[i], supPoint[j]);
        //            sum = sum.Add(bt.Multiply(d).Multiply(b).Multiply(J * kef[i] * kef[j]));
        //        }
        //    }

        //    return sum.Multiply(t);
        //}

        //private float DetJQuad(float ksi, float eta)
        //{
        //    var matrix = CountBQuad(ksi, eta);
        //    var matrixPoints = new DenseMatrix(4, 2);
        //    for (var i = 0; i < nodes.Count; i++)
        //    {
        //        matrixPoints[i, 0] = nodes[i].Position._x;
        //        matrixPoints[i, 1] = nodes[i].Position._y;
        //    }

        //    var result = matrix.Multiply(matrixPoints);
        //    return result.Determinant();
        //}

        //private DenseMatrix CountBQuad(float ksi, float eta)
        //{
        //    var matrix = new DenseMatrix(2, 4);
        //    var k = 0;
        //    for (var i = -1; i <= 1; i += 2)
        //        for (var j = -1; j <= 1; j += 2)
        //        {
        //            matrix[0, k] = i * (1 + j * eta) / 4;
        //            matrix[1, k] = j * (1 + i * ksi) / 4;
        //            k++;
        //        }
        //    return matrix;
        //}
    }
}
