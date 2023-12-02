using Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Model.MeshObjects
{
    public class Hexa : Element3D
    {
        float[] supPoint = { -(float)Math.Sqrt(0.6), 0, (float)Math.Sqrt(0.6) };
        float[] kef = { 5 / 9f, 8 / 9f, 5 / 9f };

        public override ElementKind ElementKind { get; } = ElementKind.Гекса;

        public Hexa(int number, Node[] nodes) : base(number, nodes)
        {
        }

        public override float CalcVolume()
        {
            var sum = 0f;

            for (int i = 0; i < nodes.Count - 3; i++)
            {
                for (int j = i + 1; j < nodes.Count - 2; j++)
                {
                    for (int k = j + 1; k < nodes.Count - 1; k++)
                    {
                        for (int l = k + 1; l < nodes.Count; l++)
                        {
                            var tetra = new Tetra(0, new Node[] { (Node)nodes[i], (Node)nodes[j], (Node)nodes[k], (Node)nodes[l] });
                            sum += tetra.CalcVolume();
                        }
                    }
                }
            }
            return sum / 10;
        }

        public override float DetJ(Point3D point3D)
        {
            return DetJHex(point3D._x, point3D._y, point3D._z);
        }

        //public override float[] GetDerivativesFormFunctions(Point3D point3D)
        //{
        //    return DerivativeFormFunctionHex(point3D).ToColumnMajorArray();
        //}

        //public override float[] GetFormFunctions(Point3D point3D)
        //{
        //    return FormFunctionHex(point3D);
        //}

        public float[][] CoefficientKHex(float k, float h, ConstantDirection direction, float constValue)
        {
            return VolumeIntegralBdbHex(k).Add(SquareIntegralNnHex(direction, constValue, h)).ToRowArrays();
        }

        public float[] CoefficientFHex(ConstantDirection direction, float constValue, float h, float tInf)
        {
            return SquareIntegralNHex(direction, constValue).Multiply(h * tInf).ToArray();
        }

        public float[][] CoefficientCHex()
        {
            return VolumeIntegralNnHex().ToRowArrays();
        }

        private float[] FormFunctionHex(Point3D point3D)
        {
            var center = CalcCentr();
            var supportVector = center.Sub(nodes[0].Position);
            var a = Math.Abs(supportVector._x);
            var b = Math.Abs(supportVector._y);
            var c = Math.Abs(supportVector._z);
            var ksi = (point3D._x - center._x) / a;
            var eta = (point3D._y - center._y) / b;
            var psi = (point3D._z - center._z) / c;
            return FormFunctionInLocalHex(ksi, eta, psi);
        }

        private float[] FormFunctionInLocalHex(float ksi, float eta, float psi)
        {
            var answer = new List<float>();
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                    for (var k = -1; k <= 1; k += 2)
                        answer.Add((1 + i * ksi) * (1 + j * eta) * (1 + k * psi) / 8);
            return answer.ToArray();
        }


        private Matrix<float> DerivativeFormFunctionHex(Point3D point3D)
        {
            var pointKsi = point3D._x;
            var pointEta = point3D._y;
            var pointPsi = point3D._z;
            var answer = new DenseMatrix(3, 8);
            var num = 0;
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                    for (var k = -1; k <= 1; k += 2)
                    {
                        answer[0, num] = i * (1 + j * pointEta) * (1 + k * pointPsi) / 8;
                        answer[1, num] = j * (1 + i * pointKsi) * (1 + k * pointPsi) / 8;
                        answer[2, num] = k * (1 + j * pointEta) * (1 + i * pointKsi) / 8;
                        num++;
                    }
            return answer;
        }

        private float DetJHex(float ksi, float eta, float psi)
        {
            var matrix = CountBHex(ksi, eta, psi);
            var matrixPoints = new DenseMatrix(8, 3);
            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
                matrixPoints[i, 2] = nodes[i].Position._z;
            }

            var result = matrix.Multiply(matrixPoints);
            return result.Determinant();
        }

        private DenseMatrix CountBHex(float ksi, float eta, float psi)
        {
            var matrix = new DenseMatrix(3, 8);
            var n = 0;
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                    for (var k = -1; k <= 1; k += 2)
                    {
                        matrix[0, n] = i * (1 + j * eta) * (1 + k * psi) / 8;
                        matrix[1, n] = j * (1 + i * ksi) * (1 + k * psi) / 8;
                        matrix[2, n] = k * (1 + j * eta) * (1 + i * ksi) / 8;
                        n++;
                    }

            return matrix;
        }

        private Matrix<float> SquareIntegralNnHex(ConstantDirection direction, float constValue, float h)
        {
            var sum = new DenseMatrix(8, 8).Multiply(0);
            switch (direction)
            {
                case ConstantDirection.Ksi:
                    for (var i = 0; i < 3; i++)
                        for (var j = 0; j < 3; j++)
                        {
                            var form = new DenseMatrix(1, 8);
                            var x = FormFunctionInLocalHex(constValue, supPoint[i], supPoint[j]);
                            for (var k = 0; k < x.Length; k++) form[0, k] = x[k];
                            var transposedForm = form.Transpose();
                            var jacobian = DetJHex(constValue, supPoint[i], supPoint[j]);
                            sum = sum.Add(transposedForm.Multiply(form).Multiply(kef[i] * kef[j] * jacobian));
                        }

                    return sum.Multiply(h);
                case ConstantDirection.Eta:
                    for (var i = 0; i < 3; i++)
                        for (var j = 0; j < 3; j++)
                        {
                            var form = new DenseMatrix(1, 8,
                                FormFunctionInLocalHex(supPoint[i], constValue, supPoint[j]));
                            var transposedForm = form.Transpose();
                            var jacobian = DetJHex(supPoint[i], constValue, supPoint[j]);
                            sum = sum.Add(transposedForm.Multiply(form).Multiply(kef[i] * kef[j] * jacobian));
                        }

                    return sum.Multiply(h);
                case ConstantDirection.Psi:
                    for (var i = 0; i < 3; i++)
                        for (var j = 0; j < 3; j++)
                        {
                            var form = new DenseMatrix(1, 8,
                                FormFunctionInLocalHex(supPoint[i], supPoint[j], constValue));
                            var transposedForm = form.Transpose();
                            var jacobian = DetJHex(supPoint[i], supPoint[j], constValue);
                            sum = sum.Add(transposedForm.Multiply(form).Multiply(kef[i] * kef[j] * jacobian));
                        }

                    return sum.Multiply(h);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private Vector<float> SquareIntegralNHex(ConstantDirection direction, float constValue)
        {
            var sum = new DenseVector(8).Multiply(0);
            switch (direction)
            {
                case ConstantDirection.Ksi:
                    for (var i = 0; i < 3; i++)
                        for (var j = 0; j < 3; j++)
                        {
                            var form = new DenseVector(FormFunctionInLocalHex(constValue, supPoint[i], supPoint[j]));
                            var jacobian = DetJHex(constValue, supPoint[i], supPoint[j]);
                            sum = sum.Add(form.Multiply(jacobian * kef[i] * kef[j]));
                        }

                    return sum;
                case ConstantDirection.Eta:
                    for (var i = 0; i < 3; i++)
                        for (var j = 0; j < 3; j++)
                        {
                            var form = new DenseVector(FormFunctionInLocalHex(supPoint[i], constValue, supPoint[j]));
                            var jacobian = DetJHex(supPoint[i], constValue, supPoint[j]);
                            sum = sum.Add(form.Multiply(kef[i] * kef[j] * jacobian));
                        }

                    return sum;
                case ConstantDirection.Psi:
                    for (var i = 0; i < 3; i++)
                        for (var j = 0; j < 3; j++)
                        {
                            var form = new DenseVector(FormFunctionInLocalHex(supPoint[i], supPoint[j], constValue));
                            var jacobian = DetJHex(supPoint[i], supPoint[j], constValue);
                            sum = sum.Add(form.Multiply(kef[i] * kef[j] * jacobian));
                        }

                    return sum;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private Matrix<float> VolumeIntegralNnHex()
        {
            var sum = new DenseMatrix(8, 8).Multiply(0);
            for (var i = 0; i < 3; i++)
                for (var j = 0; j < 3; j++)
                    for (var k = 0; k < 3; k++)
                    {
                        var n = FormFunctionInLocalHex(supPoint[i], supPoint[j], supPoint[k]);
                        var m = new DenseMatrix(1, 8, n);
                        var transposed = m.Transpose();
                        var jacobian = DetJHex(supPoint[i], supPoint[j], supPoint[k]);
                        sum = sum.Add(transposed.Multiply(m).Multiply(kef[i] * kef[j] * kef[k] * jacobian));
                    }

            return sum;
        }

        private Matrix<float> VolumeIntegralBdbHex(float k)
        {
            var d = DenseMatrix.CreateDiagonal(3, 3, k);
            var sum = new DenseMatrix(8, 8).Multiply(0);
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    for (var l = 0; l < 3; l++)
                    {
                        var b = CountBHex(supPoint[i], supPoint[j], supPoint[l]);
                        var bt = b.Transpose();
                        var jacobian = DetJHex(supPoint[i], supPoint[j], supPoint[l]);
                        sum = sum.Add(bt.Multiply(d).Multiply(b).Multiply(jacobian * kef[i] * kef[j] * kef[l]));
                    }
                }
            }

            return sum;
        }

        private float detJPenta()
        {
            return CountHPenta();
        }
        private float CountHPenta()
        {
            var topHeight = nodes.Max(x => x.Position._z);
            var botHeight = nodes.Min(x => x.Position._z);
            var bottom = nodes.Where(x => Math.Abs(x.Position._z - botHeight) < 1e-3);
            var top = nodes.Where(z => Math.Abs(z.Position._z - topHeight) < 1e-3);
            var crawlOrder = bottom.Concat(top).ToArray();
            return (crawlOrder[3].Position._z - crawlOrder[0].Position._z) / 2;
        }
    }

    public enum ConstantDirection
    {
        Ksi,
        Eta,
        Psi
    }
}

