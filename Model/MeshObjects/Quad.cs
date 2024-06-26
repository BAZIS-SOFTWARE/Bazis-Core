using MathNet.Numerics.Distributions;
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
            surface = new ElementSurface(number,0, new INode[] { nodes[0], nodes[1], nodes[2], nodes[3] });

            var intPoint = CreateIntegrationPoint(2, 0, 0, 0.0);
            intPoints.Add(intPoint);
            jacobians.Add(DetJ(intPoint));
        }


        public Quad(int number, INode[] nodes, int intpNumber) : base(number, nodes)
        {
            surface = new ElementSurface(number, 0, new INode[] { nodes[0], nodes[1], nodes[2], nodes[3] });
            this.nodes = surface.GetVertexes().ToList();

            IntegralPoint intPoint;
            switch (intpNumber)
            {
                case 1:
                    intPoint = CreateIntegrationPoint(2, 0, 0, 0.0);
                    intPoints.Add(intPoint);
                    jacobians.Add(DetJ(intPoint));
                    break;
                case 4:
                    var coordinate = Math.Sqrt(3)/3;
                    var counter = 0;
                    foreach (var item in surface.GetVertexes())
                    {
                        if(counter == 0)
                            intPoint = CreateIntegrationPoint(1, -1,-1,coordinate);
                        else if(counter == 1)
                            intPoint = CreateIntegrationPoint(1, -1, 1, coordinate);
                        else if (counter == 2)
                            intPoint = CreateIntegrationPoint(1, 1, 1, coordinate);
                        else 
                            intPoint = CreateIntegrationPoint(1, 1, -1, coordinate);

                        intPoints.Add(intPoint);
                        jacobians.Add(DetJ(intPoint));
                        counter++;
                    }
                    break;
                default:
                    throw new ArgumentException();
            }

        }

        private IntegralPoint CreateIntegrationPoint(int w, int ksi, int eta, double coordinate)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("ksi", ksi * coordinate);
            intPoint.AddCoordinate("eta", eta * coordinate);
            intPoint.Weigt = w;

            return intPoint;
        }

        /// <inheritdoc/>

        public override double CalcSquare()
        {
            var sum = 0.0;

            foreach (var item in jacobians)
                sum += item;

            return Math.Abs(sum);
        }


        /// <inheritdoc/>

        public override double DetJ(IIntegralPoint intPoint)
        {
            var derFormFunctions = FormFunctionDerrivatives(intPoint);
            var matrixPoints = Matrix<double>.Build.Dense(4, 2);

            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
            }

            var result = derFormFunctions.Multiply(matrixPoints);
            return result.Determinant();
        }

        private Matrix<double> FormFunctionDerrivatives(IIntegralPoint intPoint)
        {
            var eta = intPoint["eta"];
            var ksi = intPoint["ksi"];
            var formFunctDerivatives = new double[,]
            {
                { -(1 - eta), (1 - eta), 1 + eta, -(1+eta) },
                { -(1 - ksi), -(1 + ksi), 1 + ksi , 1 - ksi }
};
            return Matrix<double>.Build.DenseOfArray(formFunctDerivatives).Divide(4);
        }

        /// <inheritdoc/>

        public override double[] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return FormFunctionDerrivatives(intPoint).ToColumnMajorArray();
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {

            var ksi = intPoint["ksi"];
            var eta = intPoint["eta"];

            var answer = new List<double>();
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                    answer.Add((1 + i * ksi) * (1 + j * eta) / 4);
            return answer.ToArray();
        }
    }
}
