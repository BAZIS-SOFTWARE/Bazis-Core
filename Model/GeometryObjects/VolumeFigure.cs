using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model.GeometryObjects
{
    /// <summary>
    /// VolumeFigure
    /// </summary>
    public class VolumeFigure : ModelObject, ISurfaceObject<GeometrySurface, GeometryPoint>, IVolume
    {
        SurfaceFigure[] surfaceFigure;

        /// <summary>
        /// Figure3D
        /// </summary>
        /// <param name="number"></param>
        /// <param name="surfaceFigures"></param>
        public VolumeFigure(int number, SurfaceFigure[] surfaceFigures) : base(number)
        {
            Color = Color.FromArgb(255, 125, 0);
            surfaceFigure = surfaceFigures;

        }
/// <inheritdoc/>

        public int NumberOfSides => surfaceFigure.Length;
/// <inheritdoc/>

        public override int NumberOfPoints => GetVertexes().Count();
/// <inheritdoc/>

        public override ObjType ObjType => ObjType.Объем;
/// <inheritdoc/>

        public override IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var point in GetVertexes())
            {
                yield return point.Position;
            }
        }
/// <inheritdoc/>
        public IEnumerable<SurfaceFigure> GetSurfaceFigures()
        {
            foreach (var item in surfaceFigure)
                yield return item;
        }

        public IEnumerable<GeometrySurface> GetSurfaces()
        {
            foreach (var item in surfaceFigure)
                foreach (var surface in item.GetSurfaces())
                    yield return surface;
        }
/// <inheritdoc/>

        public double CalcVolume()
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public IEnumerable<GeometryPoint> GetVertexes()
        {
            foreach (var surface in surfaceFigure)
            {
                foreach (var point in surface.GetVertexes())
                {
                    yield return point;
                }
            }
        }
    }
}
