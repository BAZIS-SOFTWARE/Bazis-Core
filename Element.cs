using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Model.Interfaces;

namespace Model
{
    public abstract class Element : ModelObject, IElement
    {
        Node[] nodes;
        public Element(int number, Node[] nodes, ObjKind objKind) : base(number)
        {
            if (nodes.Length == 0)
                throw new Exception("Массив узлов не может быть пустой!");
            this.nodes = nodes;
        }

        public override ObjKind ObjKind { get; }
        
        public override int NumberOfPoints { get { return nodes.Length; } }

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

        public override string ToString()
        {
            var strArr = GetNodes().Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", strArr);

            return string.Format("{0} {1} : {2}", Number, ObjType, str);
        }
    }
}
