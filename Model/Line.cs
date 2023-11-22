using System;
using System.Collections.Generic;
using Geometry;
using ModelInterfaces;

namespace Model
{
    public class Line : ModelObject, ILineObject
    {
        private Node[] nodes;

        public Line(int number, Node[] nodes) : base(number)
        {
            ObjType = "Линия";
            NumberOfPoints = nodes.Length;
            ObjKind = ObjKind.Curve;
            this.nodes = nodes;
        }
/// <inheritdoc/>

        public override ObjKind ObjKind { get; }
        /// <inheritdoc/>
        public override string ObjType { get; }
/// <inheritdoc/>

        public override int NumberOfPoints { get; }      
/// <inheritdoc/>

        public override Point3D CalcCentralPoint()
        {
            var center = new Point3D();

            foreach (var node in nodes)
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }
/// <inheritdoc/>

        public float CalcLength()
        {
            throw new NotImplementedException();
        }
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
    }
}