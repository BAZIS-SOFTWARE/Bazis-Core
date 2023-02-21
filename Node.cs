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
    public class Node : ModelObject
    {
        public Point3D Position { get; }
        public Point3D Normal { get; set; } = new Point3D();
        public Node(int number, Point3D position) : base(number)
        {
            ObjKind = ObjKind.Node;
            Position = position;
            ObjType = "Узлы";
            NumberOfPoints = 1;

            InitialColor = Color.FromArgb(0, 0, 255);
            MasterColor = Color.FromArgb(0, 0, 255);
            SlaveColor = Color.FromArgb(0, 0, 255);
        }

        public override ObjKind ObjKind { get;}

        public override string ObjType { get; }

        public override int NumberOfPoints { get; }

        public override IEnumerable<Point3D> GetPoints()
        {
            yield return Position;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} : {2}",Number,ObjType,Position);
        }

        public override Point3D CalcCentralPoint()
        {
            return Position;
        }
    }
}
