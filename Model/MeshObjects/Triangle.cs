using System;
using System.Linq;
using Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;

namespace Model.MeshObjects
{
    public class Triangle : Element2D
    {

        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Треугольник;

        public Triangle(int number,Node[] nodes) : base(number, nodes)
        {
            CreateIntegrationPoint(1.0 / 2, 1.0 / 3, 1.0 / 3, 1.0 / 3);
        }

        public Triangle(int number, Node[] nodes, int intpNumber) : base(number, nodes)
        {
            switch (intpNumber)

            {
                case 1:
                    CreateIntegrationPoint(1.0 / 2, 1.0 / 3, 1.0 / 3, 1.0 / 3);
                    break;
                case 3:
                    CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 0, 1.0 / 2);
                    CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 1.0 / 2, 0);
                    CreateIntegrationPoint(1.0 / 6, 0, 1.0 / 2, 1.0 / 2);
                    break;
                default:
                    throw new ArgumentException();
            }
        }        
/// <inheritdoc/>

        public override double CalcSquare()
        {
            var pAr = GetCoordinates().ToArray();

            var v1 = new Point3D(pAr[1]._x - pAr[0]._x, pAr[1]._y - pAr[0]._y, pAr[1]._z - pAr[0]._z);
            var v2 = new Point3D(pAr[2]._x - pAr[0]._x, pAr[2]._y - pAr[0]._y, pAr[2]._z - pAr[0]._z);

            var vProd = Geometry.Vector.CrossProd(v1, v2);
            var square = Math.Abs(Geometry.Vector.GetVectorLenght(vProd) / 2);
            return square;
        }

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

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return new double[,] { { 1, 0,-1 }, { 0, 1,-1 } };
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            return new double[] { intPoint["l1"], intPoint["l2"], intPoint["l3"] };
        }
/// <inheritdoc/>

        public void CreateIntegrationPoint(double w, double l1, double l2, double l3)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("l1", l1);
            intPoint.AddCoordinate("l2", l2);
            intPoint.AddCoordinate("l3", l2);
            intPoint.Weigt = w;

            intPoints.Add(intPoint);
            jacobiMatrixes.Add(CalcJacobi(intPoint));
        }
    }
}
