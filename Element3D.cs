using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model
{
    public class Element3D : SurfaceElement, IElement3D
    {
        private List<Surface> surfaces;
        public Element3D(int number, Node[] nodes, ObjKind objKind) : base(number, nodes,objKind)
        {

            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);
            ObjKind = objKind;
            ObjType = "Элементы3D";
            surfaces = NodeSorter.GetSurfaces(nodes).ToList();
        }
/// <inheritdoc/>


        public override bool ViewState 
        { 
            get => base.ViewState;
            set
            {
                base.ViewState = value;
                foreach (var surface in surfaces)
                {
                    surface.ViewState = value;
                }
            }
        }



        /// <inheritdoc/>

        public override string ObjType { get; }
        /// <inheritdoc/>
        public override ObjKind ObjKind { get;}
/// <inheritdoc/>

        public override int NumberOfSides { get { return surfaces.Count(); } }       

        /// <inheritdoc/>

        public override Point3D CalcCentralPoint()
        {
            var center = new Point3D();

            foreach (var node in GetNodes())
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }

        /// <summary>
        /// Calc volume
        /// </summary>
        /// <returns></returns>
        public float CalcVolume()
        {
            //nodes[]
            //var pAr = GetPoints().ToArray();

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

        public override float[] GetFormFunctions(Point3D point3D)
        {
            if (ObjKind == ObjKind.Tetra)
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
            else return new float[0];
        }

        /// <inheritdoc/>

        public override IEnumerable<Surface> GetSurface()
        {
            foreach (var surface in surfaces)
            {
                yield return surface;
            }
        }
    }
}
