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
    public class Element1D : Element, ILineObject
    {
        public Element1D(int number, Node[] nodes, ObjKind objKind) : base(number, nodes, objKind)
        {
            InitialColor = Color.FromArgb(255, 192, 203);
            MasterColor = Color.FromArgb(255, 192, 203);
            SlaveColor = Color.FromArgb(255, 192, 203);

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

        public override float[] GetFormFunctions(Point3D point3D)
        {
            var answer = new float[2];
            answer[0] = (nodes[1].Position._x - point3D._x) / CalcLength();
            answer[1] = (point3D._x - nodes[0].Position._x) / CalcLength();
            return answer;
        }
    }
}
