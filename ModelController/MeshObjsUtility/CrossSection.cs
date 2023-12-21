using Geometry;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace ModelController.MeshObjsUtility
{
    public class CrossSection
    {
        /// <returns>
        /// Сечение элемента плоскостью
        /// Возвращает словарь (номер элемента, плоскость его сечения)
        /// </returns>
        public Dictionary<int, ElementSurface> GetSectionSurfaces(IEnumerable<Element3D> elements, Plane plane)
        {
            var result = new Dictionary<int, ElementSurface>();
            var index = 0;
            foreach (var element in elements)
            {
                var points = new List<Point3D>();
                foreach (var surface in element.GetSurfaces())
                {
                    foreach (var line in surface.GetLinesFromSurface(surface))
                    {
                        var point1 = line.P0;
                        var point2 = line.P1;
                        if (!IsIntersect(point1, point2, plane))
                            continue;
                        var sectionPoint = GetSectionPoint(point1, point2, plane);
                        points.Add(sectionPoint);
                    }
                }

                var nodes = points
                    .Distinct()
                    .Select((p, i) => new Node(i, p))
                    .ToArray();
                if (nodes.Length >= 3 && nodes.Length <= 6)
                {
                    if (index == 363)
                    {
                        var elementPoints = element.GetVertexes().Select(n => n.Position);
                        var sectionPoints = nodes.Select(n => n.Position);
                        Debug(sectionPoints.Concat(elementPoints));
                    }
                    var surface = new ElementSurface(index++, nodes);
                    result.Add(element.Number, surface);
                }
                else if (nodes.Length > 6)
                {
                    throw new Exception("Ошибка при построении сечения. Узлов плоскости более 6");
                }
            }

            return result;
        }

        /// <summary>
        /// Проверка на пересечение прямой ab с плоскостью plane. 
        /// Возвращает true, если есть точка пересечения
        /// </summary>
        private bool IsIntersect(Point3D a, Point3D b, Plane plane)
        {
            var d1 = Math.Round(plane.PointPosition(a), 2);
            var d2 = Math.Round(plane.PointPosition(b), 2);
            if (double.IsNaN(d1) || double.IsNaN(d2))
                throw new ArgumentException("Degenerate plane");
            if (Math.Abs(d1) < 10e-2 && Math.Abs(d2) < 10e-2) // лежат в одной плоскости
                return false;
            return Math.Sign(d1) * Math.Sign(d2) <= 0;
        }

        /// <summary>
        /// Вычисляет точку пересечения прямой и плоскости
        /// </summary>
        private Point3D GetSectionPoint(Point3D P0, Point3D P1, Plane plane)
        {
            if (Math.Abs(plane.PointPosition(P1)) < 1e-2)
                return P1;
            if (Math.Abs(plane.PointPosition(P0)) < 1e-2)
                return P0;
            var A = plane.Normal._x;
            var B = plane.Normal._y;
            var C = plane.Normal._z;
            var D = plane.Shifting;
            var dx = P1._x - P0._x;
            var dy = P1._y - P0._y;
            var dz = P1._z - P0._z;
            var t = -(A * P0._x + B * P0._y + C * P0._z + D)
                    / (A * dx + B * dy + C * dz);
            var x = (float)Math.Round(dx * t + P0._x, 2);
            var y = (float)Math.Round(dy * t + P0._y, 2);
            var z = (float)Math.Round(dz * t + P0._z, 2);
            return new Point3D(x, y, z);
        }

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