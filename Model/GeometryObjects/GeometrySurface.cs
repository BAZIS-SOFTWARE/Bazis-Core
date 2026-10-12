using Geometry;
using Model.MeshObjects;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.GeometryObjects
{
    public class GeometrySurface : ISurface<GeometryPoint>
    {
        GeometryPoint[] points;
        /// <summary>
        /// Surface constructor
        /// </summary>
        /// <param name="number"></param>
        /// <param name="points"></param>
        public GeometrySurface(int number, GeometryPoint[] points)
        {
            if (points.Length < 3)
                throw new Exception("Для создания поверхности требуется не меньше 3-х узлов!");

            points.RoundSort(); //Anticlockwise(nodes);
            this.points = points;

            Number = number;

        }
        /// <inheritdoc/>

        public bool IsInner => false;

        /// <inheritdoc/>

        public int GetVertexIndex(GeometryPoint vertex)
        {
            return Array.IndexOf(points, vertex);
        }
        /// <summary>
        /// this
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public GeometryPoint this[int ind]
        {
            get { return points[ind]; }
            set { points[ind] = value; }
        }
        /// <inheritdoc/>

        public override string ToString()
        {
            var strArr = points.Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", strArr);

            return $"{base.ToString()} : {str}";
        }

        /// <inheritdoc/>

        //public override ObjType ObjType { get; } = ObjType.Поверхность;
        /// <inheritdoc/>

        public int NumberOfPoints { get { return points.Length; } }
/// <inheritdoc/>

        public int Number { get; }

        /// <inheritdoc/>

        public Point3D CalcNormal()
        {
            var fvectors = points[1].Position.Sub(points[0].Position);
            var svectors = points[2].Position.Sub(points[0].Position);
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
                square += Math.Abs(Vector.GetVectorLength(vProd) / 2);
            }


            return square;
        }
        /// <inheritdoc/>

        public IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var node in points)
            {
                yield return node.Position;
            }
        }
        /// <inheritdoc/>

        public bool Equals(Node other)
        {
            var points = other.GetCoordinates().ToArray();
            if (points.Contains(this.points[0].Position, new Point3DEqualityComparer()) &&
points.Contains(this.points[1].Position, new Point3DEqualityComparer()) &&
points.Contains(this.points[2].Position, new Point3DEqualityComparer()))
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
            var temp = point.Sub(points[0].Position);

            return nNormal._x * temp._x + nNormal._y * temp._y + nNormal._z * temp._z;
        }
        /// <inheritdoc/>

        public Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var point in GetCoordinates())
            {
                center = center.Sum(point);
            }
            return center.Div(points.Length);
        }
/// <inheritdoc/>

        public Segment3D[] GetLinesFromSurface()
        {
            var lines = new Segment3D[points.Length];
            for (int i = 0; i < points.Length - 1; i++)
            {
                lines[i] = new Segment3D(points[i].Position, points[i + 1].Position);
            }
            lines[points.Length - 1] = new Segment3D(points[points.Length - 1].Position, points[0].Position);
            return lines;
        }
        /// <inheritdoc/>

        public IEnumerable<GeometryPoint> GetVertexes()
        {
            foreach (var node in points)
            {
                yield return node;
            }
        }
/// <inheritdoc/>

        public void ReverseVertexes()
        {
            points = points.Reverse().ToArray();
        }
    }
}
