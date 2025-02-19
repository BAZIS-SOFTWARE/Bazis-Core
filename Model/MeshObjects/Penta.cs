using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;

namespace Model.MeshObjects
{
    public class Penta : Element3D
    {
        /// <inheritdoc/>
        public override ElementKind ElementKind { get; } = ElementKind.Пента;

        public Penta(int number, Node[] nodes) : base(number, nodes)
        {
            if (nodes.Length < 6)
                throw new Exception($"Коллличество узлов в пентаэлементе {number} должно быть не меньше 5");
            //var masterSurface = GetMasterSurface(3, surfaces);
            //this.nodes = SortNodes(masterSurface.Number, surfaces);

            surfaces.Add(new ElementSurface(number, 0, new Node[] { nodes[0], nodes[1], nodes[2] }));
            surfaces.Add(new ElementSurface(number, 1, new Node[] { nodes[3], nodes[5], nodes[4] }));
            surfaces.Add(new ElementSurface(number, 2, new Node[] { nodes[0], nodes[2], nodes[5], nodes[3] }));
            surfaces.Add(new ElementSurface(number, 3, new Node[] { nodes[0], nodes[3], nodes[4], nodes[1] }));
            surfaces.Add(new ElementSurface(number, 4, new Node[] { nodes[1], nodes[4], nodes[5], nodes[2] }));

            CreateIntegrationPoint(1, 0.3333, 0.3333, 0);
        }

        public Penta(int number, Node[] nodes, int intpNumber) : base(number, nodes, intpNumber)
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
            jacobiMatrixes.Add(CalcJacobi(intPoint));
        }

        /// <inheritdoc/>

        public override Matrix<double> CalcJacobi(IIntegralPoint intPoint)
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
            return result;
        }
/// <inheritdoc/>

        public override double CalcVolume()
        {
            var vol = 0.0;
            var counter = 0;

            foreach (var item in GetJacobiMatrixes())
            {
                vol += item.Determinant() * intPoints[counter].Weigt;
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
