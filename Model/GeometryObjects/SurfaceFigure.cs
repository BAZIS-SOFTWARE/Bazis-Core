using Geometry;
using Geometry.Attribute;
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
        bool transfiniteFlag;
        bool recombineFlag;
        /// <summary>
        /// IsRecombined
        /// </summary>
        public bool IsRecombined 
        {
            get { return recombineFlag; }
        }

        public bool IsTransfinited
        {
            get { return transfiniteFlag; }
        }
        /// <summary>
        /// MakeRecombine
        /// </summary>
        public void MakeRecombine()
        {
            recombineFlag = true;
        }

        /// <summary>
        /// MakeTransfinite
        /// </summary>
        public void MakeTransfinite()
        {
            transfiniteFlag = true;
        }
        /// <summary>
        /// Surface
        /// </summary>
        protected GeometrySurface[] surfaces;


        /// <summary>
        /// Номера образующих кривых. Временное решение.
        /// Сделано для быстрого поиска связанных с поверхностью кривых 
        /// </summary>
        public int[] CurveNumbers { get; set; }
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
            //var strArr = surfaces.Select(x => x.Number.ToString()).ToArray();
            
            // Выводим номера кривых по которым построена поверхность
            var str = string.Join(" ", CurveNumbers);

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
