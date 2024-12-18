using Geometry;
using Model.Interfaces;
using Model.Interfaces.GeometryObjects;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model.GeometryObjects
{
    /// <summary>
    /// SurfaceFigure
    /// </summary>
    public class SurfaceFigure : ModelObject, ISurfaceFigure
    {
        /// <summary>
        /// Surface
        /// </summary>
        protected IGeometrySurface[] surfaces;
        /// <summary>
        /// Figure2D
        /// </summary>
        /// <param name="number"></param>
        /// <param name="surfaces"></param>
        public SurfaceFigure(int number, IGeometrySurface[] surfaces) : base(number)
        {
            this.surfaces = surfaces;

            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);
        }
/// <inheritdoc/>

        public int NumberOfSides { get; } = 1;
/// <inheritdoc/>

        public override int NumberOfPoints { get { return GetVertexes().Count(); } }
/// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Фигура2D;

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

        public IEnumerable<IGeometrySurface> GetSurfaces()
        {
            foreach (var surface in surfaces)
            {
                yield return surface;
            }

        }
/// <inheritdoc/>

        public IEnumerable<IGeometryPoint> GetVertexes()
        {
            var points = new List<IGeometryPoint>();

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
