using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using System;

namespace Model.MeshObjects
{
    public class Tetra : Element3D
    {
        public Tetra(int number, INode[] nodes) : base(number, nodes)
        {
            if (surfaces.Count != 4)
                throw new Exception($"Коллличество поверхностей в тетраэлементе {number} должно быть 4");
            CreateIntegrationPoint(1, 1.0 / 4, 1.0 / 4, 1.0 / 4, 1.0 / 4);
        }



        public Tetra(int number, INode[] nodes, int intpNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intpNumber)

            {
                case 1:
                    CreateIntegrationPoint(1, 1.0 / 4, 1.0 / 4, 1.0 / 4, 1.0 / 4);
                    break;
                case 4:
                    var a = 0.58541020;
                    var b = 0.13819660;
                    CreateIntegrationPoint(1.0 / 4, a, b, b, b);
                    CreateIntegrationPoint(1.0 / 4, b, a, b, b);
                    CreateIntegrationPoint(1.0 / 4, b, b, a, b);
                    CreateIntegrationPoint(1.0 / 4, b, b, b, a);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Тетра;

        /// <inheritdoc/>

        public override double CalcVolume()
        {
            var mCoord = new float[4, 4];
            var p = new Point3D[] { nodes[0].Position, nodes[1].Position, nodes[2].Position, nodes[3].Position };
            for (int j = 0; j < p.Length; j++)
            {
                mCoord[j, 0] = 1;
                mCoord[j, 1] = p[j]._x;
                mCoord[j, 2] = p[j]._y;
                mCoord[j, 3] = p[j]._z;
            }

            var cMatr = Math.Abs(Matrix<float>.Build.DenseOfArray(mCoord).Determinant() / 6);
            //var str = cMatr.ToString(4, 4);
            return cMatr;
        }
/// <inheritdoc/>

        public void CreateIntegrationPoint(double w, double l1, double l2, double l3,double l4)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("l1", l1);
            intPoint.AddCoordinate("l2", l2);
            intPoint.AddCoordinate("l3", l3);
            intPoint.AddCoordinate("l4", l4);
            intPoint.Weigt = w;
            intPoints.Add(intPoint);
            jacobiMatrixes.Add(CalcJacobi(intPoint));
        }

        /// <inheritdoc/>

        public override Matrix<double> CalcJacobi(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(4, 3);

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

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return new double[,] { { 1, 0,0,-1 }, { 0, 1,0,-1 }, { 0, 0, 1,-1 } };
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            return new double[] { intPoint["l1"], intPoint["l2"], intPoint["l3"], intPoint["l4"] };
        }
    }
}
