using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using ModelInterfaces.IntegralObject;
using ModelInterfaces.MeshObjects;

namespace Model.MeshObjects
{
    public class Penta : Element3D
    {
        /// <inheritdoc/>
        public override ElementKind ElementKind { get; } = ElementKind.Пента;

        public Penta(int number, INode[] nodes) : base(number, nodes)
        {
            var masterSurface = GetMasterSurface(3, surfaces);
            this.nodes = SortNodes(masterSurface.Number, surfaces).ToList();

            CreateIntegrationPoint(1, 0.3333, 0.3333, 0);
        }

        public Penta(int number, INode[] nodes, int intpNumber) : base(number, nodes, intpNumber)
        {
            var masterSurface = GetMasterSurface(3, surfaces);
            this.nodes = SortNodes(masterSurface.Number, surfaces).ToList();

            switch (intpNumber)
            {
                case 1:
                    CreateIntegrationPoint(1, 0.3333, 0.3333, 0);
                    break;
                case 2:
                    CreateIntegrationPoint(0.5f, 0.3333, 0.3333, -0.5774);
                    CreateIntegrationPoint(0.5f, 0.3333, 0.3333, 0.5774);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
/// <inheritdoc/>

        public void CreateIntegrationPoint(double w, double ksi, double eta, double dze)
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

        public override double DetJ(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(6, 3);

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

        public override float CalcVolume()
        {
            var vol = 0.0f;
            var counter = 0;

            foreach (var item in GetDetsJ())
            {
                vol += (float)(item * intPoints[counter].Weigt);
                counter++;
            }
            return Math.Abs(vol);
        }
        /// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            var eta = intPoint["eta"];
            var ksi = intPoint["ksi"];
            var dze = intPoint["dze"];

            return new double[,]
            {
                // по ksi
                { -(1 - dze) / 2, (1 - dze) / 2,0, -(1 + dze) / 2,(1 + dze) / 2, 0},
                // по eta
                { -(1 - dze) / 2, 0,(1 - dze) / 2, -(1 + dze) / 2,0,(1 + dze) / 2},
                // по dze
                { -(1 - ksi - eta) / 2, -(ksi / 2), -(eta / 2),(1 - ksi - eta) / 2, ksi / 2, eta / 2}
};

        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var eta = intPoint["eta"];
            var ksi = intPoint["ksi"];
            var dze = intPoint["dze"];

            var formFunctions = new double[]
            {
                (1 - ksi - eta) * (1 - dze) / 2,
                ksi * (1 - dze) / 2,
                eta * (1 - dze) / 2,
                (1 - ksi - eta) * (1 + dze) / 2,
                ksi * (1 + dze) / 2,
                eta * (1 + dze) / 2
            };
            return formFunctions;
        }
    }
}
