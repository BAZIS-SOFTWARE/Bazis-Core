using Geometry;
using Model.GeometryObjects;
using Model.Interfaces.GeometryObjects;
using ModelControllerInterfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace ModelController.MeshObjsUtility
{
    /// <summary>
    /// Построение плоскости сечения сеточных объектов
    /// </summary>
    public class CrossSection : ICrossSectionMaker
    {
        /// <inheritdoc/>
        public ISurfaceFigure GetSectionSurfaces(IEnumerable<IElement3D> elements, Plane plane)
        {
            var result = new List<GeometrySurface>();
            foreach (var element in elements)
            {
                var points = new List<Point3D>();
                foreach (var surface in element.GetSurfaces())
                {
                    foreach (var line in surface.GetLinesFromSurface())
                    {
                        if (!IsIntersect(line, plane))
                            continue;
                        var sectionPoint = GetSectionPoint(line, plane);
                        if (!IsPointNearOfAnyPoint(sectionPoint, points))
                            points.Add(sectionPoint);
                    }
                }
                var sectionPoints = points
                    .Select((p, index) => new GeometryPoint(index, p))
                    .ToArray();
                if (sectionPoints.Length >= 3 && sectionPoints.Length <= 6)
                {
                    var sectionSurface = new GeometrySurface(element.Number, sectionPoints);
                    result.Add(sectionSurface);
                }
                else if (sectionPoints.Length > 6)
                    throw new Exception("Ошибка при построении сечения. Узлов плоскости более 6");
            }
            return new SurfaceFigure(0, result.ToArray());
        }

        /// <summary>
        /// Check if a segment intersects a plane 
        /// </summary>
        private bool IsIntersect(Segment3D segment, Plane plane)
        {
            var d1 = plane.PointPosition(segment.P0);
            var d2 = plane.PointPosition(segment.P1);
            if (double.IsNaN(d1) || double.IsNaN(d2))
                throw new ArgumentException("Вырожденная плоскость");
            if (Math.Abs(d1) < 10e-4 && Math.Abs(d2) < 10e-4) // segment belongs to the plane
                return false;
            return Math.Sign(d1) * Math.Sign(d2) <= 0;
        }

        /// <summary>
        /// Calculates the intersection point of a segment and a plane
        /// </summary>
        private Point3D GetSectionPoint(Segment3D segment, Plane plane)
        {
            var A = plane.Normal._x;
            var B = plane.Normal._y;
            var C = plane.Normal._z;
            var D = plane.Shifting;
            var a = segment.P0;
            var b = segment.P1;
            var dx = b._x - a._x;
            var dy = b._y - a._y;
            var dz = b._z - a._z;
            var t = -(A * a._x + B * a._y + C * a._z + D) / (A * dx + B * dy + C * dz);
            var x = dx * t + a._x;
            var y = dy * t + a._y;
            var z = dz * t + a._z;
            return new Point3D(x, y, z);
        }

        /// <summary>
        /// Check if the given list already contains the given point with some accuracy
        /// </summary>
        private bool IsPointNearOfAnyPoint(Point3D p, List<Point3D> points, float epsilon = 5*1e-3f)
        {
            foreach (var point in points)
            {
                if (Math.Abs(point._x - p._x) < epsilon
                    && Math.Abs(point._y - p._y) < epsilon
                    && Math.Abs(point._z - p._z) < epsilon)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Output the list of points content in the log.txt file
        /// </summary>
        private void Debug(IEnumerable<Point3D> points)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            using (var writer = new StreamWriter("./log.txt"))
            {
                foreach (var point in points)
                {
                    writer.WriteLine($"({point._x}, {point._y}, {point._z})");
                }
            }
        }
    }
}