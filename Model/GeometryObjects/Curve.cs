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
            Dim = 1;
            NumberOfPoints = points.Length;
            this.points = points;
        }

        /// <inheritdoc/>
        public override ObjType ObjType { get; } = ObjType.Кривая;
        /// <inheritdoc/>

        public override int NumberOfPoints { get; }

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

        public double CalcLength()
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
            return base.ToString();
        }
    }
}
