using Geometry;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model.MeshObjects
{
    /// <summary>
    /// Surface class
    /// </summary>
    public class ElementSurface : ModelObject, ISurface<Node>
    {
        Node[] nodes;
        /// <summary>
        /// Surface constructor
        /// </summary>
        /// <param name="elementNumber"></param>
        /// <param name="surfaceNumber"></param>
        /// <param name="nodes"></param>
        public ElementSurface(int elementNumber, int surfaceNumber, Node[] nodes) : base(surfaceNumber)
        {
            if (nodes.Length < 3)
                throw new Exception("Для создания поверхности требуется не меньше 3-х узлов!");

            nodes.RoundSort(); //Anticlockwise(nodes);
            this.nodes = nodes;

            ElementNumber = elementNumber;

            InitialColor = Color.FromArgb(224, 0, 177);
            MasterColor = Color.FromArgb(224, 0, 177);
            SlaveColor = Color.FromArgb(224, 0, 177);
        }
        
        /// <summary>
        /// this
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public Node this[int ind]
        {
            get { return nodes[ind]; }
        }
        /// <inheritdoc/>

        public override string ToString()
        {
            var strArr = nodes.Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", strArr);

            return $"{base.ToString()} : {str}";
        }

        /// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Поверхность;
        /// <inheritdoc/>

        public override int NumberOfPoints { get { return nodes.Length; } }
/// <inheritdoc/>

        public int ElementNumber { get; }

        /// <inheritdoc/>

        public Point3D CalcNormal()
        {
            var fvectors = nodes[1].Position.Sub(nodes[0].Position);
            var svectors = nodes[2].Position.Sub(nodes[0].Position);
            return Vector.CrossProd(fvectors, svectors);
        }
/// <inheritdoc/>

        public double CalcSquare()
        {
            var square = 0.0f;
            for (int i = 0; i < NumberOfPoints - 2; i++)
            {
                var v1 = this[i + 1].Position.Sub(this[0].Position);
                var v2 = this[i + 2].Position.Sub(this[0].Position);

                var vProd = Vector.CrossProd(v1, v2);
                square += Math.Abs(Vector.GetVectorLenght(vProd) / 2);
            }


            return square;
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

        public bool Equals(Node other)
        {
            var points = other.GetCoordinates().ToArray();
            if (points.Contains(nodes[0].Position, new Point3DEqualityComparer()) &&
points.Contains(nodes[1].Position, new Point3DEqualityComparer()) &&
points.Contains(nodes[2].Position, new Point3DEqualityComparer()))
                return true;
            else
                return false;
        }
        /// <summary>
        /// GetDistance
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float GetDistance(Point3D point)
        {
            var normal = CalcNormal();
            var nNormal = Vector.GetVectorNorm(normal);
            var temp = point.Sub(nodes[0].Position);

            return nNormal._x * temp._x + nNormal._y * temp._y + nNormal._z * temp._z;
        }
        /// <inheritdoc/>

        public override Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var point in GetCoordinates())
            {
                center = center.Sum(point);
            }
            return center.Div(NumberOfPoints);
        }
/// <inheritdoc/>

        public Segment3D[] GetLinesFromSurface()
        {
            var lines = new Segment3D[nodes.Length];
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                lines[i] = new Segment3D(nodes[i].Position, nodes[i + 1].Position);
            }
            lines[nodes.Length - 1] = new Segment3D(nodes[nodes.Length - 1].Position, nodes[0].Position);
            return lines;
        }

        /// <inheritdoc/>

        public IEnumerable<Node> GetVertexes()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }
/// <inheritdoc/>

        public void ReverseVertexes()
        {
            nodes = nodes.Reverse().ToArray();
        }
    }
}