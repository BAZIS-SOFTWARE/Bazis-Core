using Geometry;
using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using System.Collections.Generic;
using System.Drawing;

namespace Model.GeometryObjects
{
    /// <summary>
    /// Figure2D
    /// </summary>
    public class Figure2D : ModelObject, IFigure2D
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
        public Figure2D(int number, IGeometrySurface[] surfaces) : base(number)
        {
            var points = new HashSet<GeometryPoint>();

            foreach (var surface in surfaces)
            {
                foreach (var point in surface.GetVertexes())
                {

                }
            }

            this.points = points;

            this.surfaces = surfaces;

            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);
        }
/// <inheritdoc/>

        public int NumberOfSides { get; } = 1;
/// <inheritdoc/>

        public override int NumberOfPoints { get { return points.Length; } }
/// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Фигура2D;


        /// <summary>
        /// Points
        /// </summary>
        GeometryPoint[] points { get; }
/// <inheritdoc/>

        public override IEnumerable<Point3D> GetCoordinates()
        {
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
    }
}
