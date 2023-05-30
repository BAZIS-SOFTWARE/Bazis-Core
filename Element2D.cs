using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Element2D : SurfaceElement, IElement2D
    {
        Surface surface;

        public Element2D(int number, Node[] nodes, ObjKind objKind) : base(number, nodes, objKind)
        {
            InitialColor = Color.FromArgb(0, 0, 255);
            MasterColor = Color.FromArgb(0, 0, 255);
            SlaveColor = Color.FromArgb(0, 0, 255);
            ObjType = "Элементы2D";
            ObjKind = objKind;
            if (objKind == ObjKind.Triangle)
            {              
                surface = new Surface(0, new Node[] { nodes[0], nodes[1], nodes[2] },ObjKind.ElementSurface);
            }

            else if (objKind == ObjKind.Quad)
            {
                surface = new Surface(0, new Node[] { nodes[0], nodes[1], nodes[2], nodes[3] }, ObjKind.ElementSurface);
            }
        }

        public override ObjKind ObjKind { get; }

        public override string ObjType { get; }

        public override int NumberOfSides { get { return 1; } }

        public override Point3D CalcCentralPoint()
        {
            var center = new Point3D();

            foreach (var node in GetNodes())
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }

        public Point3D CalcNormal()
        {
            return surface.CalcNormal();
        }

        public float CalcSquare()
        {


            var pAr = GetPoints().ToArray();

            var v1 = new Point3D(pAr[1]._x - pAr[0]._x, pAr[1]._y - pAr[0]._y, pAr[1]._z - pAr[0]._z);
            var v2 = new Point3D(pAr[2]._x - pAr[0]._x, pAr[2]._y - pAr[0]._y, pAr[2]._z - pAr[0]._z);

            var vProd = Vector.CrossProd(v1, v2);
            var square = Math.Abs(Vector.GetVectorLenght(vProd) / 2);
            return square;
        }

        public override IEnumerable<Surface> GetSurface()
        {
            yield return surface;
        }
    }
}
