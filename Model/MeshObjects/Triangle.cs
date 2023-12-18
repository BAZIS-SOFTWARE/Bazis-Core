using System;
using ModelInterfaces;
using System.Linq;
using Geometry;
using MathNet.Numerics.LinearAlgebra.Single;
using ModelInterfaces.IntegralObject;

namespace Model.MeshObjects
{
    public class Triangle : Element2D
    {

        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Треугольник;

        public Triangle(int number, Node[] nodes) : base(number, nodes)
        {
            surface = new ElementSurface(0, new Node[] { nodes[0], nodes[1], nodes[2] });

            var intPoint = new IntegralPoint();
            intPoint.AddDirection("l1", 1 / 3);
            intPoint.AddDirection("l2", 1 / 3);
            intPoint.AddDirection("l3", 1 / 3);
            intPoint.Weigt = CalcSquare();
            intPoints.Add(intPoint);
            jacobians.Add(DetJ(intPoint));
        }

        public Triangle(int number, Node[] nodes, int intpNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intpNumber)

            {
                case 1:
                    intPoint = new IntegralPoint();
                    intPoint.AddDirection("l1", 1 / 3);
                    intPoint.AddDirection("l2", 1 / 3);
                    intPoint.AddDirection("l3", 1 / 3);
                    intPoint.Weigt = CalcSquare();
                    intPoints.Add(intPoint);

                    jacobians.Add(DetJ(intPoint));
                    break;
                case 3:
                    for (int i = 0; i < 3; i++)
                    {
                        intPoint = new IntegralPoint();
                        intPoint.AddDirection("l1", i == 2 ? 0 : 1 / 2);
                        intPoint.AddDirection("l2", i == 0 ? 0 : 1 / 2);
                        intPoint.AddDirection("l3", i == 1 ? 0 : 1 / 2);
                        intPoint.Weigt = 1/6f;
                        intPoints.Add(intPoint);

                        jacobians.Add(DetJ(intPoint));
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public override float CalcSquare()
        {
            var pAr = GetCoordinates().ToArray();

            var v1 = new Point3D(pAr[1]._x - pAr[0]._x, pAr[1]._y - pAr[0]._y, pAr[1]._z - pAr[0]._z);
            var v2 = new Point3D(pAr[2]._x - pAr[0]._x, pAr[2]._y - pAr[0]._y, pAr[2]._z - pAr[0]._z);

            var vProd = Geometry.Vector.CrossProd(v1, v2);
            var square = Math.Abs(Geometry.Vector.GetVectorLenght(vProd) / 2);
            return square;
        }

        /// <inheritdoc/>

        public override float DetJ(IIntegralPoint point)
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

        public override float[] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            // Расчитано для линейного элемента с одной т.интегрирования в центре
            // по формулам 14.11а и б
            return new float[] { 1, 0, 0, 1, -1, -1 };
        }
        /// <inheritdoc/>

        public override float[] GetFormFunctions(IIntegralPoint intPoint)
        {
            return new float[] { intPoint["l1"], intPoint["l2"], intPoint["l3"] };
        }
    }
}
