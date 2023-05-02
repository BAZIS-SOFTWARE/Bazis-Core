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

        public override string ObjType { get; }
        /// <inheritdoc/>
        public override ObjKind ObjKind { get;}
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
            var pAr = GetPoints().ToArray();

            var mCoord = new float[4, 4];
            var p = new Point3D[] { pAr[0], pAr[1], pAr[2], pAr[3] };
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

        public override IEnumerable<Surface> GetSurface()
        {
            foreach (var surface in surfaces)
            {
                yield return surface;
            }
        }
    }
}
