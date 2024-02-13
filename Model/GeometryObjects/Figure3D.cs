using Geometry;
using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GeometryObjects
{
    public class Figure3D : ModelObject, IFigure3D
    {
        IGeometrySurface[] surfaces;

        /// <summary>
        /// Figure3D
        /// </summary>
        /// <param name="number"></param>
        /// <param name="surfaces"></param>
        public Figure3D(int number, IGeometrySurface[] surfaces) : base(number)
        {
            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);

            this.surfaces = surfaces;
        }
/// <inheritdoc/>

        public int NumberOfSides => surfaces.Length;
/// <inheritdoc/>

        public override int NumberOfPoints => throw new NotImplementedException();

        public override ObjType ObjType => ObjType.Фигура3D;
/// <inheritdoc/>

        public override IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var item in surfaces)
            {
                foreach (var point in item.GetCoordinates())
                {
                    yield return point;
                }

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
    }
}
