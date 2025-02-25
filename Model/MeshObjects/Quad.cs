using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.MeshObjects
{
    public class Quad : Element2D
    {
        /// <inheritdoc/>
        public override int Level
        {
            get
            {
                if (intPoints.Count == 1)
                    return 1;
                else return 2;
            }
            set
            {
                intPoints.Clear();
                jacobiMatrixes.Clear();
                if (value == 1)
                {
                    CreateIntegrationPoint(2, 0, 0);
                }
                else
                {
                    var coordinate = Math.Sqrt(3) / 3;
                    CreateIntegrationPoint(1, -coordinate, -coordinate);
                    CreateIntegrationPoint(1, coordinate, -coordinate);
                    CreateIntegrationPoint(1, coordinate, coordinate);
                    CreateIntegrationPoint(1, -coordinate, coordinate);
                }
            }
        }
        /// <inheritdoc/>
        public override ElementKind ElementKind { get; } = ElementKind.Квадрат;

        public Quad(int number, Node[] nodes) : base(number, nodes)
        {
            if (nodes.Length < 4)
                throw new Exception($"Коллличество узлов в элементе {ElementKind} {number} должно быть не меньше 4");

            Level = 1;
        }


        public Quad(int number, Node[] nodes, int level) : this(number, nodes)
        {
            Level = level;

        }
        /// <inheritdoc/>

        public void CreateIntegrationPoint(double w, double ksi, double eta)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("ksi", ksi);
            intPoint.AddCoordinate("eta", eta);
            intPoint.Weigt = w;

            intPoints.Add(intPoint);
            jacobiMatrixes.Add(CalcJacobi(intPoint));
        }

        /// <inheritdoc/>

        public override double CalcSquare()
        {
            var sqr = 0.0;
            var counter = 0;

            foreach (var item in GetJacobiMatrixes())
            {
                sqr += Math.Abs(item.Determinant()) * intPoints[counter].Weigt;
                counter++;
            }
            return sqr;
        }


        /// <inheritdoc/>

        public override Matrix<double> CalcJacobi(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(4, 2);

            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
            }

            var result = matrixDFF.Multiply(matrixPoints);
            return result;
        }

        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var ksi = intPoint["ksi"];
            var eta = intPoint["eta"];

            var formFunctions = new double[]
            {
                (1-ksi)*(1-eta)/4,
                (1+ksi)*(1-eta)/4,
                (1+ksi)*(1+eta)/4,
                (1-ksi)*(1+eta)/4,
            };

            return formFunctions;
        }
        /// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            var eta = intPoint["eta"];
            var ksi = intPoint["ksi"];
            return new double[,]
            {
                // по ksi
                { -(1 - eta) / 4, (1 - eta) / 4, (1 + eta) / 4, -(1 + eta) / 4 },

                // по eta
                { -(1 - ksi) / 4, -(1 + ksi) / 4, (1 + ksi) / 4, (1 - ksi) / 4 }
            };

        }
    }
}
