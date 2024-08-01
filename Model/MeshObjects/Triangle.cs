using System;
using System.Linq;
using Geometry;
using MathNet.Numerics.LinearAlgebra.Single;
using ModelInterfaces.IntegralObject;
using ModelInterfaces.MeshObjects;

namespace Model.MeshObjects
{
    public class Triangle : Element2D
    {

        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Треугольник;

        public Triangle(int number, INode[] nodes) : base(number, nodes)
        {
            surface = new ElementSurface(number,0, new INode[] { nodes[0], nodes[1], nodes[2] });

            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("l1", 1 / 3);
            intPoint.AddCoordinate("l2", 1 / 3);
            intPoint.AddCoordinate("l3", 1 / 3);
            intPoint.Weigt = CalcSquare();
            intPoints.Add(intPoint);
            jacobians.Add(DetJ(intPoint));
        }

        public Triangle(int number, INode[] nodes, int intpNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intpNumber)

            {
                case 1:
                    intPoint = new IntegralPoint();
                    intPoint.AddCoordinate("l1", 1 / 3);
                    intPoint.AddCoordinate("l2", 1 / 3);
                    intPoint.AddCoordinate("l3", 1 / 3);
                    intPoint.Weigt = CalcSquare();
                    intPoints.Add(intPoint);

                    jacobians.Add(DetJ(intPoint));
                    break;
                case 3:
                    for (int i = 0; i < 3; i++)
                    {
                        intPoint = new IntegralPoint();
                        intPoint.AddCoordinate("l1", i == 2 ? 0 : 1 / 2);
                        intPoint.AddCoordinate("l2", i == 0 ? 0 : 1 / 2);
                        intPoint.AddCoordinate("l3", i == 1 ? 0 : 1 / 2);
                        intPoint.Weigt = 1/6f;
                        intPoints.Add(intPoint);

                        jacobians.Add(DetJ(intPoint));
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }        

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

        public override double DetJ(IIntegralPoint point)
        {
            var m = new DenseMatrix(2, 2)
            {
                [0, 0] = nodes[0].Position._x - nodes[2].Position._x,
                [0, 1] = nodes[0].Position._y - nodes[2].Position._y,
                [1, 0] = nodes[1].Position._x - nodes[2].Position._x,
                [1, 1] = nodes[1].Position._y - nodes[2].Position._y
            };
            return m.Determinant();
        }
        /// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            // Расчитано для линейного элемента с одной т.интегрирования в центре
            // по формулам 14.11а и б
            return new double[,] { { 1, 0, 0 }, { 1, -1, -1 } };
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            return new double[] { intPoint["l1"], intPoint["l2"], intPoint["l3"] };
        }
/// <inheritdoc/>

        public override void CreateIntegrationPoint(int w, double ksi, double eta)
        {
            throw new NotImplementedException();
        }
    }
}
