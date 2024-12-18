using Geometry;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using System;
using System.Drawing;
using System.Net;

namespace Model.MeshObjects
{
    public class Beam : Element, ILineObject<Node>
    {
        public Beam(int number, Node[] nodes) : base(number, nodes)
        {
            InitialColor = Color.FromArgb(255, 192, 203);
            MasterColor = Color.FromArgb(255, 192, 203);
            SlaveColor = Color.FromArgb(255, 192, 203);

            //var intPoint = new IntegralPoint();
            //intPoint.AddCoordinate("ksi", 0.0f);
            //intPoint.Weigt = CalcLength();
            //intPoints.Add(intPoint);
            //jacobians.Add(DetJ(intPoint));
        }



        public Beam(int number, Node[] nodes, int intpNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intpNumber)
            {
                case 1:
                    intPoint = new IntegralPoint();
                    intPoint.AddCoordinate("ksi", 0.0f);
                    intPoint.Weigt = 2;
                    intPoints.Add(intPoint);
                    break;
                case 2:
                    intPoint = new IntegralPoint();
                    intPoint.AddCoordinate("ksi", -(float)Math.Sqrt(3) / 3);
                    intPoint.Weigt = 1;
                    intPoints.Add(intPoint);
                    jacobiMatrixes.Add(CalcJacobi(intPoint));

                    intPoint = new IntegralPoint();
                    intPoint.AddCoordinate("ksi", (float)Math.Sqrt(3) / 3);
                    intPoint.Weigt = 1;
                    intPoints.Add(intPoint);
                    jacobiMatrixes.Add(CalcJacobi(intPoint));
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Стержень;

        /// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Элемент1D;
        /// <inheritdoc/>

        public override Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var node in nodes)
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }
        /// <inheritdoc/>

        public float CalcLength()
        {
            return Vector.GetVectorLenght(nodes[1].Position.Sub(nodes[0].Position));
        }
        /// <inheritdoc/>

        public override Matrix<double> CalcJacobi(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(2, 1);

            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
            }

            var result = matrixDFF.Multiply(matrixPoints);
            return result;
        }
        /// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return new double[,]
            {
                {-0.5,0.5}
            };
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var answer = new double[2];
            answer[0] = (1 - intPoint["ksi"]) / 2;
            answer[1] = (1 + intPoint["ksi"]) / 2;
            return answer;
        }
    }
}
