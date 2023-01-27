using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Model.Interfaces;

namespace Model
{
    public class Curve : ModelObject, ILineObject
    {
        Node[] nodes;
        public Curve(int number, Node [] nodes) : base(number)
        {
            ObjType = "Линия";
            NumberOfPoints = nodes.Length;
            this.nodes = nodes;
        }

        public override ObjKind ObjKind { get;  }
        public override string ObjType { get; }

        public override int NumberOfPoints { get; }

        public float CalcLength()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Node> GetNodes()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }

        public override IEnumerable<Point3D> GetPoints()
        {
            foreach (var node in nodes)
            {
                yield return node.Position;
            }
        }
    }
}
