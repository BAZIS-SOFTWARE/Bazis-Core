using Geometry;
using ModelInterfaces;
using ModelInterfaces.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model.MeshObjects
{
    /// <summary>
    /// Surface class
    /// </summary>
    public class ElementSurface : ModelObject, IElementSurface
    {
        Node[] nodes;
        /// <summary>
        /// Surface constructor
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        public ElementSurface(int number, Node[] nodes) : base(number)
        {
            if (nodes.Length < 3)
                throw new Exception("Для создания поверхности требуется не меньше 3-х узлов!");

            nodes.RoundSort(); //Anticlockwise(nodes);
            this.nodes = nodes;

            InitialColor = Color.FromArgb(224, 0, 177);
            MasterColor = Color.FromArgb(224, 0, 177);
            SlaveColor = Color.FromArgb(224, 0, 177);
        }
        
        /// <summary>
        /// this
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public INode this[int ind]
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

        public Point3D CalcNormal()
        {
            var fvectors = nodes[1].Position.Sub(nodes[0].Position);
            var svectors = nodes[2].Position.Sub(nodes[0].Position);
            return Vector.CrossProd(fvectors, svectors);
        }

        public float CalcSquare()
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

        public bool Equals(INode other)
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

        public Segment3D[] GetLinesFromSurface(ISurface<INode> surface)
        {
            var n = surface.GetCoordinates().Count();
            var points = surface.GetCoordinates().ToArray();
            var lines = new Segment3D[n];
            for (int i = 0; i < n - 1; i++)
            {
                lines[i] = new Segment3D(points[i], points[i + 1]);
            }
            lines[n - 1] = new Segment3D(points[n - 1], points[0]);
            return lines;
        }
/// <inheritdoc/>

        public IEnumerable<INode> GetVertexes()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }
    }
}