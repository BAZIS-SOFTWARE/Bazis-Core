using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Model.GeometryObjects
{
    /// <summary>
    /// VolumeFigure
    /// </summary>
    public class VolumeFigure : ModelObject, ISurfaceObject<GeometrySurface, GeometryPoint>, IVolume
    {
        protected GeometrySurface[] surfaces;
        SurfaceFigure[] surfaceFigure;

        GeometryPoint[] points
        {
            get
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
                return points.ToArray();
            }

        }

        /// <summary>
        /// Figure3D
        /// </summary>
        /// <param name="number"></param>
        /// <param name="surfaces"></param>
        public VolumeFigure(int number, GeometrySurface[] surfaces) : base(number)
        {
            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);

            this.surfaces = surfaces;
        }
/// <inheritdoc/>

        public int NumberOfSides => surfaces.Length;
/// <inheritdoc/>

        public override int NumberOfPoints => points.Length;
/// <inheritdoc/>

        public override ObjType ObjType => ObjType.Фигура3D;
/// <inheritdoc/>

        public override IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var point in points)
            {
                yield return point.Position;
            }
        }
/// <inheritdoc/>

        public IEnumerable<GeometrySurface> GetSurfaces()
        {
            foreach (var item in surfaces)
            {
                yield return item;
            }
        }
/// <inheritdoc/>

        public double CalcVolume()
        {
            throw new NotImplementedException();
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
