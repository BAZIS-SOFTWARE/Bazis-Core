using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            this.surfaces = surfaces;

            Color = Color.FromArgb(255, 125, 0);
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
            var strArr = surfaces.Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", strArr);

            return $"{base.ToString()} : {str}";
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

        public override IEnumerable<Point3D> GetCoordinates()
        {
            var points = GetVertexes();
            foreach (var point in points)
            {
                yield return point.Position;
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
            var points = new List<GeometryPoint>();

            foreach (var surface in surfaces)
            {
                foreach (var point in surface.GetVertexes())
                {
                    if (!points.Contains(point))
                        points.Add(point);
                }
            }
            return points;
        }
    }
}
