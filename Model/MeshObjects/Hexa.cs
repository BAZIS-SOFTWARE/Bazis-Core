using MathNet.Numerics.LinearAlgebra;
using Model.Utilities;
using ModelInterfaces.IntegralObject;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.MeshObjects
{
    public class Hexa : Element3D
    {
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Гекса;

        public Hexa(int number, INode[] nodes) : base(number, nodes)
        {
            var masterSurface = GetMasterSurface(4, surfaces);
            this.nodes = SortNodes(masterSurface.Number, surfaces).ToList();
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("ksi", 0.0f);
            intPoint.AddCoordinate("eta", 0.0f);
            intPoint.AddCoordinate("psi", 0.0f);
            intPoint.Weigt = CalcVolume();
            intPoints.Add(intPoint);
            jacobians.Add(DetJ(intPoint));
        }



        public Hexa(int number, INode[] nodes, int intpNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intpNumber)
            {
                case 1:

                    intPoint = new IntegralPoint();
                    intPoint.AddCoordinate("ksi", 0.0f);
                    intPoint.AddCoordinate("eta", 0.0f);
                    intPoint.AddCoordinate("psi", 0.0f);
                    intPoint.Weigt = CalcVolume();
                    intPoints.Add(intPoint);

                    jacobians.Add(DetJ(intPoint));
                    break;
                case 8:
                    var coordinate = (float)Math.Sqrt(3) / 3;
                    for (var i = -1; i <= 1; i += 2)
                        for (var j = -1; j <= 1; j += 2)
                            for (var k = -1; k <= 1; k += 2)
                            {
                                intPoint = new IntegralPoint();
                                intPoint.AddCoordinate("ksi", i * coordinate);
                                intPoint.AddCoordinate("eta", j * coordinate);
                                intPoint.AddCoordinate("psi", k * coordinate);
                                intPoint.Weigt = 1;
                                intPoints.Add(intPoint);
                                
                                jacobians.Add(DetJ(intPoint));
                            }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        /// <inheritdoc/>

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
        /// <inheritdoc/>

        public override double DetJ(IIntegralPoint intPoint)
        {
            var matrix = countDerivativeFormFunction(intPoint);
            var matrixPoints = Matrix<double>.Build.Dense(8, 3);
            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
                matrixPoints[i, 2] = nodes[i].Position._z;
            }

            var result = matrix.Multiply(matrixPoints);
            return result.Determinant();
        }
        /// <inheritdoc/>

        public override double[] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return countDerivativeFormFunction(intPoint).ToColumnMajorArray();
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var ksi = intPoint["ksi"];
            var eta = intPoint["eta"];
            var psi = intPoint["psi"];

            var answer = new List<double>();
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                    for (var k = -1; k <= 1; k += 2)
                        answer.Add((1 + i * ksi) * (1 + j * eta) * (1 + k * psi) / 8);
            return answer.ToArray();
        }


        private Matrix<double> countDerivativeFormFunction(IIntegralPoint intPoint)
        {
            var ksi = intPoint["ksi"];
            var eta = intPoint["eta"];
            var psi = intPoint["psi"];

            var answer = Matrix<double>.Build.Dense(3, 8);
            var num = 0;
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                    for (var k = -1; k <= 1; k += 2)
                    {
                        var a = answer[0, num];
                        answer[0, num] = i * (1 + j * eta) * (1 + k * psi) / 8;
                        answer[1, num] = j * (1 + i * ksi) * (1 + k * psi) / 8;
                        answer[2, num] = k * (1 + j * eta) * (1 + i * ksi) / 8;
                        num++;
                    }
            return answer;
        }
    }
}

