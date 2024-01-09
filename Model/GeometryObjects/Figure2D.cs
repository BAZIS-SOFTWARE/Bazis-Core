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
    public class Figure2D : ModelObject, ISurfaceObject<IGeometrySurface, IPoint>
    {
        /// <summary>
        /// Surface
        /// </summary>
        protected GeometrySurface surface;
        /// <summary>
        /// Figure2D
        /// </summary>
        /// <param name="number"></param>
        /// <param name="points"></param>
        public Figure2D(int number, GeometryPoint[] points) : base(number)
        {
            this.points = points;

            surface = new GeometrySurface(number, points);

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
            yield return surface;
        }
    }
}
