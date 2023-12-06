using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces;

namespace Model.MeshObjects
{
    public abstract class Element : ModelObject, IElement
    {
        protected List<Node> nodes = new List<Node>();

        protected List<IntegralPoint> points = new List<IntegralPoint>();
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
        /// <summary>
        /// this
        /// </summary>
        /// <param name="nodeInd"></param>
        /// <returns></returns>
        public INode this[int nodeInd] => nodes[nodeInd];

        /// <inheritdoc/>

        public override int NumberOfPoints { get { return nodes.Count(); } }
/// <inheritdoc/>

        public abstract ElementKind ElementKind { get; }


        /// <inheritdoc/>
        public IEnumerator<INode> GetEnumerator()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }

        /// <inheritdoc/>

        public override IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var node in nodes)
            {
                yield return node.Position;
            }
        }
/// <inheritdoc/>

        public override string ToString()
        {
            var strArr = nodes.Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", strArr);

            return $"{Number} {(int)ObjType} {(int)ElementKind} {points.Count} : {str}";
        }
/// <inheritdoc/>

        public IEnumerable<INode> GetVertexes()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }
        /// <inheritdoc/>

        public abstract float DetJ(Point3D integralPoint);
/// <inheritdoc/>

        public IEnumerable<IIntegralPoint> GetIntegralPoints()
        {
            foreach (var point in points)
            {
                yield return point;
            }
        }
    }
}
