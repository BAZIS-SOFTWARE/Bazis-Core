using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
                if (value == 1)
                {
                    var intPoint = CreateIntegrationPoint(4, 0, 0);

                    intPoints.Add(intPoint);
                }
                else
                {
                    var coordinate = Math.Sqrt(3) / 3;
                    var int0 = CreateIntegrationPoint(1, -coordinate, -coordinate);
                    intPoints.Add(int0);

                    var int1 = CreateIntegrationPoint(1, coordinate, -coordinate);
                    intPoints.Add(int1);

                    var int2 = CreateIntegrationPoint(1, coordinate, coordinate);
                    intPoints.Add(int2);

                    var int3 = CreateIntegrationPoint(1, -coordinate, coordinate);
                    intPoints.Add(int3);
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

            IncidentMatrix = new int[4][];

            IncidentMatrix[0] = [1, 3];
            IncidentMatrix[1] = [0, 2];
            IncidentMatrix[2] = [1, 3];
            IncidentMatrix[3] = [0, 2];
        }


        public Quad(int number, Node[] nodes, int level) : this(number, nodes)
        {
            Level = level;

        }

        /// <summary>
        /// CreateIntegrationPoint
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public override IEnumerable<IntegralPoint> CreateIntegrationPoints(int level)
        {

            if (level == 1)
            {
                yield return CreateIntegrationPoint(4, 0, 0);
            }
            else
            {
                var coordinate = Math.Sqrt(3) / 3;
                yield return CreateIntegrationPoint(1, -coordinate, -coordinate);
                yield return CreateIntegrationPoint(1, coordinate, -coordinate);
                yield return CreateIntegrationPoint(1, coordinate, coordinate);
                yield return CreateIntegrationPoint(1, -coordinate, coordinate);
            }
        }

        /// <inheritdoc/>

        public override double CalcSquare()
        {
            var sqr = 0.0;
            var counter = 0;

            foreach (var item in GetIntegralPoints())
            {
                sqr += Math.Abs(item.Jacobi.Determinant()) * intPoints[counter].Weigt;
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
