using Geometry;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces;
using ModelInterfaces.IntegralObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.MeshObjects
{
    public class Tetra : Element3D
    {
        public Tetra(int number, Node[] nodes) : base(number, nodes)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddDirection("l1", 1 / 4);
            intPoint.AddDirection("l2", 1 / 4);
            intPoint.AddDirection("l3", 1 / 4);
            intPoint.AddDirection("l4", 1 / 4);
            intPoint.Weigt = CalcVolume();
            intPoints.Add(intPoint);
        }

        public Tetra(int number, Node[] nodes, int point) : base(number, nodes, point)
        {
        }

        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Тетра;

        /// <inheritdoc/>

        public override float CalcVolume()
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

        public override float DetJ(IIntegralPoint point3D)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public override float[] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public override float[] GetFormFunctions(IIntegralPoint intPoint)
        {
            return new float[] { intPoint["l1"], intPoint["l2"], intPoint["l3"], intPoint["l4"] };
        }
    }
}
