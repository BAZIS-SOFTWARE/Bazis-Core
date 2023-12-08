using System;
using System.Collections.Generic;
using ModelInterfaces;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra;
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
        }

        public Triangle(int number, Node[] nodes, int points) : base(number, nodes, points)
        {
        }

        /// <inheritdoc/>

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
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public override float[] GetFormFunctions(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }
    }
}
