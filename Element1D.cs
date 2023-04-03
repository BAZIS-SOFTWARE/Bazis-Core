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
    public class Element1D : Element, IBeamElement
    {
        Node[] nodes;
        public Element1D(int number, Node[] nodes, ObjKind objKind) : base(number, nodes, objKind)
        {
            InitialColor = Color.FromArgb(0, 125, 255);
            MasterColor = Color.FromArgb(0, 125, 255);
            SlaveColor = Color.FromArgb(0, 125, 255);
            this.nodes = nodes;
            ObjType = "Элементы1D";
            ObjKind = objKind;
        }
        public override ObjKind ObjKind { get; }

        public override string ObjType { get; }

        public override Point3D CalcCentralPoint()
        {
            var center = new Point3D();

            foreach (var node in nodes)
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }

        public float CalcLength()
        {
            return Vector.GetVectorLenght(nodes[1].Position.Sub(nodes[0].Position));        
        }
    }
}
