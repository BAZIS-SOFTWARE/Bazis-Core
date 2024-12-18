using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.MeshObjects
{
    public class Triangle_c : Element2D
    {
        /// <summary>
        /// Triangle_c
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        public Triangle_c(int number, Node[] nodes) : base(number, nodes)
        {
            CreateIntegrationPoint(1.0 / 2, 1.0 / 3, 1.0 / 3);
        }
        /// <summary>
        /// Triangle_c
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        /// <param name="intpNumber"></param>
        /// <exception cref="ArgumentException"></exception>
        public Triangle_c(int number, Node[] nodes, int intpNumber) : base(number, nodes)
        {
            switch (intpNumber)

            {
                case 1:
                    CreateIntegrationPoint(1.0 / 2, 1.0 / 3, 1.0 / 3);
                    break;
                case 3:
                    CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 0);
                    CreateIntegrationPoint(1.0 / 6, 0, 1.0 / 2);
                    CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 1.0 / 2);
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// CreateIntegrationPoint
        /// </summary>
        /// <param name="w"></param>
        /// <param name="ksi"></param>
        /// <param name="eta"></param>
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

        public override ElementKind ElementKind => ElementKind.Треугольник;
/// <inheritdoc/>

        public override Matrix<double> CalcJacobi(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(3, 2);

            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
            }

            var result = matrixDFF.Multiply(matrixPoints);
            return result;
        }
/// <inheritdoc/>

        public override double CalcSquare()
        {
            var vol = 0.0;
            var counter = 0;

            foreach (var item in GetJacobiMatrixes())
            {
                vol += Math.Abs(item.Determinant()) * intPoints[counter].Weigt;
                counter++;
            }
            return Math.Abs(vol);
        }
/// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return new double[,]
{
                { -1, 1, 0},// i,j,k to ksi axis
                { -1, 0, 1} // i,j,k to eta axis
};
        }
/// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var ksi = intPoint["ksi"];
            var eta = intPoint["eta"];

            var formFunctions = new double[]
            {
                1 - ksi - eta,
                ksi,
                eta
            };
            return formFunctions;
        }
    }
}
