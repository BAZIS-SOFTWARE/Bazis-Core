using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using ModelInterfaces;

namespace Model.MeshObjects
{
    public abstract class Element : ModelObject, IElement
    {
        protected List<INode> nodes = new List<INode>();
        public Element(int number, Node[] nodes) : base(number)
        {
            if (nodes.Length == 0)
                throw new Exception("Массив узлов не может быть пустой!");

            foreach (var node in nodes)
            {
                node.AddElement(this);
                this.nodes.Add(node);
            }
        }

        /// <inheritdoc/>

        public override int NumberOfPoints { get { return nodes.Count(); } }
/// <inheritdoc/>

        public abstract ElementKind ElementKind { get; }
/// <inheritdoc/>

        public abstract float DetJ(Point3D point3D);
/// <inheritdoc/>

        public IEnumerable<INode> GetNodes()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }
/// <inheritdoc/>

        public override IEnumerable<Point3D> GetPoints()
        {
            foreach (var node in nodes)
            {
                yield return node.Position;
            }
        }
/// <inheritdoc/>

        public override string ToString()
        {
            var strArr = GetNodes().Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", strArr);

            return string.Format("{0} {1} : {2}", Number, ObjType, str);
        }
    }
}
