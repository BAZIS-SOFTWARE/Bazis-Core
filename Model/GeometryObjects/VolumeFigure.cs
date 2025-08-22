using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace Model.GeometryObjects
{
    /// <summary>
    /// VolumeFigure
    /// </summary>
    public class VolumeFigure : IVolume
    {
        /// <summary>
        /// SurfaceMeshSize
        /// </summary>
        public double SurfaceMeshSize { get; set; }
        /// <summary>
        /// GradientMeshPower
        /// </summary>
        public double GradientMeshPower { get; set; }
        /// <summary>
        /// LayerThickness
        /// </summary>
        public double LayerThickness { get; set; }
        /// <summary>
        /// CoreMeshSize
        /// </summary>
        public double CoreMeshSize { get; set; }

        SurfaceFigure[] surfaceFigure;
        /// <summary>
        /// Number
        /// </summary>
        public int Number { get; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Figure3D
        /// </summary>
        /// <param name="number"></param>
        /// <param name="surfaceFigures"></param>
        public VolumeFigure(int number, SurfaceFigure[] surfaceFigures)
        {
            Number = number;
            surfaceFigure = surfaceFigures;

        }
        /// <inheritdoc/>
        public override string ToString()
        {
            
            var strArr = surfaceFigure.Select(x => x.Number.ToString()).ToArray();
            var strSurfsNumbers = string.Join(" ", strArr);
            return $"{Number} {Name}: {strSurfsNumbers}";
        }
        /// <inheritdoc/>

        public int NumberOfSides => surfaceFigure.Length;
/// <inheritdoc/>

        public int NumberOfPoints => GetVertexes().Count();

/// <inheritdoc/>

        public IEnumerable<Point3D> GetCoordinates()
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GeometrySurface> GetSurfaces()
        {
            foreach (var item in surfaceFigure)
                foreach (var surface in item.GetSurfaces())
                    yield return surface;
        }
/// <inheritdoc/>

        public double CalcVolume()
        {
            // TO DO
            // Реализовать метод дифференциала
            // строим набор сечений фигуры
            // вычисляем площадь каждой
            // складываем все, умножая на шаг между ними
            return 0;
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
