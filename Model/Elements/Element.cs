using System.Collections.Generic;
using System.Linq;
using Geometry;
using ModelInterfaces;

namespace Model.Elements
{
    public abstract class Element : ModelObject, IElement
    {
        protected List<INode> nodes;
        public Element(int number) : base(number)
        {
            nodes = new List<INode>();
        }
        //{
        //    if (nodes.Length == 0)
        //        throw new Exception("Массив узлов не может быть пустой!");
        //    this.nodes = nodes;

        //    foreach (var node in nodes)
        //        node.AddElement(this);
        //}

        public override ObjKind ObjKind { get; }

        public override int NumberOfPoints { get { return nodes.Count(); } }

        public abstract float DetJ(Point3D point3D);

        public abstract float[] GetDerivativesFormFunctions(Point3D point3D);
        public abstract float[] GetFormFunctions(Point3D point3D);

        public IEnumerable<INode> GetNodes()
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
