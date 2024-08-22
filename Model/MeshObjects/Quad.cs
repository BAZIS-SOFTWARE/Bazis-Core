using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces.IntegralObject;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.MeshObjects
{
    public class Quad : Element2D
    {
        /// <inheritdoc/>
        public override ElementKind ElementKind { get; } = ElementKind.Квадрат;

        public Quad(int number, INode[] nodes) : base(number, nodes)
        {
            CreateIntegrationPoint(2, 0, 0);
        }


        public Quad(int number, INode[] nodes, int intpNumber) : base(number, nodes)
        {
            this.nodes = surface.GetVertexes().ToList();

            switch (intpNumber)
            {
                case 1:
                    CreateIntegrationPoint(2, 0, 0);
                    break;
                case 4:
                    var coordinate = Math.Sqrt(3)/3;
                    CreateIntegrationPoint(1, -coordinate, -coordinate);
                    CreateIntegrationPoint(1, -coordinate, coordinate);
                    CreateIntegrationPoint(1, coordinate, coordinate);
                    CreateIntegrationPoint(1, coordinate, -coordinate);
                    break;
                default:
                    throw new ArgumentException();
            }

        }
/// <inheritdoc/>

        public void CreateIntegrationPoint(double w, double ksi, double eta)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("ksi", ksi);
            intPoint.AddCoordinate("eta", eta);
            intPoint.Weigt = w;

            intPoints.Add(intPoint);
            jacobians.Add(DetJ(intPoint));
        }

        /// <inheritdoc/>

        public override double CalcSquare()
        {
            return surface.CalcSquare();
        }


        /// <inheritdoc/>

        public override double DetJ(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions).Divide(4);
            var matrixPoints = Matrix<double>.Build.Dense(4, 2);

            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
            }

            var result = matrixDFF.Multiply(matrixPoints);
            return result.Determinant();
        }

        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var ksi = intPoint["ksi"];
            var eta = intPoint["eta"];

            var answer = new List<double>();

            // TO DO проверить правильность счета. Проверить формулу и коэффициенты
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                    answer.Add((1 + i * ksi) * (1 + j * eta) / 4);
            return answer.ToArray();
        }
/// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            var eta = intPoint["eta"];
            var ksi = intPoint["ksi"];
            return new double[,]
            {
                { -(1 - eta), (1 - eta), 1 + eta, -(1+eta) },
                { -(1 - ksi), -(1 + ksi), 1 + ksi , 1 - ksi }
};
        }
    }
}
