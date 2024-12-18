using System;
using System.Collections.Generic;
using Geometry;
using Model.Interfaces;
using Model.Interfaces.GeometryObjects;

namespace Model.GeometryObjects
{
    public class Line : ModelObject, ILine
    {
        private GeometryPoint[] points;

        public Line(int number, GeometryPoint[] points) : base(number)
        {
            NumberOfPoints = points.Length;
            this.points = points;
        }

        /// <inheritdoc/>
        public override ObjType ObjType { get; } = ObjType.Линия;
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

        public IEnumerable<IGeometryPoint> GetVertexes()
        {
            foreach (var point in points)
            {
                yield return point;
            }
        }
    }
}