using Geometry;
using Model;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelController.MeshObjsUtility
{
    public class CrossSection
    {
        /// <returns>
        /// Сечение элемента плоскостью
        /// Возвращает словарь (номер элемента, плоскость его сечения)
        /// </returns>
        public Dictionary<int, Surface> GetSectionSurfaces(List<Element3D> elements, Plane plane)
        {
            var result = new Dictionary<int, Surface>();
            int index = 0;
            foreach (var element in elements)
            {
                var points = new HashSet<Point3D>();
                foreach (var surface in element.GetSurface())
                {
                    foreach (var line in surface.GetLinesFromSurface(surface))
                    {
                        var point1 = line.P0;
                        var point2 = line.P1;
                        if (IsIntersect(point1, point2, plane))
                        {
                            var sectionPoint = GetSectionPoint(new Line(point1, point2), plane);
                            if (!points.Contains(sectionPoint))
                                points.Add(sectionPoint);
                        }
                    }
                }
                if (points.Count >= 3)
                {
                    var nodes = points.Select((p, i) => new Node(i, p)).ToArray();
                    var surface = new Surface(index++, nodes, ObjKind.ElementSurface);
                    result.Add(element.Number, surface);
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
            var d1 = plane.PointPosition(a);
            var d2 = plane.PointPosition(b);
            if (Math.Abs(d1) < 1e-6 && Math.Abs(d2) < 1e-6)
                return false;
            return Math.Sign(d1) != Math.Sign(d2);
        }

        /// <summary>
        /// Вычисляет точку пересечения прямой и плоскости
        /// </summary>
        private Point3D GetSectionPoint(Line line, Plane plane)
        {
            var A = plane.Normal._x;
            var B = plane.Normal._y;
            var C = plane.Normal._z;
            var D = plane.Shifting;
            var t = (-A * line.P1._x - B * line.P1._y - C * line.P1._z - D) /
                (A * (line.P0._x - line.P1._x) + B * (line.P0._y - line.P1._y) + C * (line.P0._z - line.P1._z));
            var x = (float)Math.Round(line.P1._x * (1 - t) + line.P0._x * t, 6);
            var y = (float)Math.Round(line.P1._y * (1 - t) + line.P0._y * t, 6);
            var z = (float)Math.Round(line.P1._z * (1 - t) + line.P0._z * t, 6);
            return new Point3D(x, y, z);
        }
    }
}