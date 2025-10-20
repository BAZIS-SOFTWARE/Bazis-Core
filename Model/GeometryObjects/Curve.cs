using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Model.Interfaces;

namespace Model.GeometryObjects
{
    public class Curve : ModelObject, ILineObject<GeometryPoint>
    {
        private GeometryPoint[] points;

        public Curve(int number, GeometryPoint[] points) : base(number)
        {
            NumberOfPoints = points.Length;
            this.points = points;
        }

        /// <inheritdoc/>
        public override ObjType ObjType { get; } = ObjType.Кривая;
        /// <inheritdoc/>

        public override int NumberOfPoints { get; }
        /// <summary>
        /// NodeNumbers
        /// </summary>
        public int[] PointsNumbers { get; set; }

        /// <inheritdoc/>

        public override Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var point in points)
            {
                center = center.Sum(point.Position);
            }
            return center.Div(NumberOfPoints);
        }
        /// <inheritdoc/>

        public float CalcLength()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>

        public override IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var point in points)
            {
                yield return point.Position;
            }
        }
/// <inheritdoc/>

        public IEnumerable<GeometryPoint> GetVertexes()
        {
            foreach (var point in points)
            {
                yield return point;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            //var strArr = points.Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", PointsNumbers);

            return $"{base.ToString()} : {str}";
        }
    }
}