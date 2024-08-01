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

            //CreateIntegrationPoint(2, 0, 0, 0);
        }



        public Hexa(int number, INode[] nodes, int intpNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intpNumber)
            {
                case 1:

                    CreateIntegrationPoint(2, 0, 0, 0);
                    break;
                case 8:
                    throw new NotImplementedException("Не реализовано создание точек интегрирования");
                default:
                    throw new ArgumentException();
            }
        }

        /// <inheritdoc/>

        public override float CalcVolume()
        {
            throw new NotImplementedException("Не реализовано вычисление объема");
        }
        /// <inheritdoc/>

        public override double DetJ(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(8, 3);

            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
                matrixPoints[i, 2] = nodes[i].Position._z;
            }

            var result = matrixDFF.Multiply(matrixPoints);
            return result.Determinant();
        }

/// <inheritdoc/>

        public override void CreateIntegrationPoint(float w, double ksi, double eta, double dze)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("ksi", ksi);
            intPoint.AddCoordinate("eta", eta);
            intPoint.AddCoordinate("dze", dze);
            intPoint.Weigt = w;

            intPoints.Add(intPoint);
            jacobians.Add(DetJ(intPoint));
        }
/// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }
    }
}

