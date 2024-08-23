using Geometry;
using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Model.GeometryObjects
{
    /// <summary>
    /// VolumeFigure
    /// </summary>
    public class VolumeFigure : ModelObject, IVolumeFigure
    {
        IGeometrySurface[] surfaces;

        IGeometryPoint[] points
        {
            get
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
                return points.ToArray();
            }

        }

        /// <summary>
        /// Figure3D
        /// </summary>
        /// <param name="number"></param>
        /// <param name="surfaces"></param>
        public VolumeFigure(int number, IGeometrySurface[] surfaces) : base(number)
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

        public IEnumerable<IGeometrySurface> GetSurfaces()
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
