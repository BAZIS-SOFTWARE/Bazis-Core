using Geometry;
using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Elements
{
    public class Tetra : Element3D
    {
        public Tetra(int number, Node[] nodes) : base(number, nodes, ObjKind.Tetra)
        {
        }

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

        public override float DetJ(Point3D point3D)
        {
            throw new NotImplementedException();
        }

        public override float[] GetDerivativesFormFunctions(Point3D point3D)
        {
            throw new NotImplementedException();
        }

        public override float[] GetFormFunctions(Point3D point3D)
        {
            return FormFunctionTetra(point3D);
        }

        private float[] FormFunctionTetra(Point3D point3D)
        {
            var mCoord = Matrix<float>.Build.DenseOfArray(new float[,]
{
                    { 1, nodes[0].Position._x, nodes[0].Position._y,nodes[0].Position._z },
                    { 1, nodes[1].Position._x, nodes[1].Position._y,nodes[1].Position._z },
                    { 1, nodes[2].Position._x, nodes[2].Position._y,nodes[2].Position._z },
                    { 1, nodes[3].Position._x, nodes[3].Position._y,nodes[3].Position._z }
});
            var dN = mCoord.Inverse(); // differential matrix {dN/dn}

            var vCoord = Vector<float>.Build.Dense(new float[] { 1, point3D._x, point3D._y, point3D._z });
            var result = dN.Multiply(vCoord);
            return result.ToArray();
        }
    }
}
