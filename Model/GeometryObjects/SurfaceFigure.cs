using Geometry;
using Geometry.Attribute;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Model.GeometryObjects
{
    /// <summary>
    /// SurfaceFigure
    /// </summary>
    public class SurfaceFigure : ModelObject, ISurfaceObject<GeometrySurface, GeometryPoint>, ISquare
    {
        /// <summary>
        /// Surface
        /// </summary>
        protected GeometrySurface[] surfaces;
        /// <summary>
        /// Figure2D
        /// </summary>
        /// <param name="number"></param>
        /// <param name="surfaces"></param>
        public SurfaceFigure(int number, GeometrySurface[] surfaces) : base(number)
        {
            Dim = 2;
            this.surfaces = surfaces;

        }
/// <inheritdoc/>

        public int NumberOfSides { get; } = 1;
/// <inheritdoc/>

        public override int NumberOfPoints { get { return GetVertexes().Count(); } }
/// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Поверхность;
        /// <summary>
        /// Volume
        /// </summary>
        public VolumeFigure Volume { get; internal set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <inheritdoc/>

        public double CalcSquare()
        {
            var square = 0.0;
            foreach (var surface in surfaces)
            {
                square += surface.CalcSquare();
            }
            return square;
        }
/// <inheritdoc/>

        public override Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var vert in GetVertexes())
            {
                center = center.Sum(vert.Position);
            }
            return center.Div(NumberOfPoints);
        }

        /// <inheritdoc/>

        public override IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var surface in surfaces)
            {
                foreach (var point in surface.GetCoordinates())
                {
                    //if (!points.Contains(point))
                    yield return point;
                }
            }
        }

        /// <inheritdoc/>

        public IEnumerable<GeometrySurface> GetSurfaces()
        {
            foreach (var surface in surfaces)
            {
                yield return surface;
            }

        }

        /// <inheritdoc/>

        public IEnumerable<GeometryPoint> GetVertexes()
        {
            var points = new HashSet<GeometryPoint>();

            foreach (var surface in surfaces)
            {
                foreach (var point in surface.GetVertexes())
                {
                    //if (!points.Contains(point))
                        points.Add(point);
                }
            }
            return points;
        }
    }
}
