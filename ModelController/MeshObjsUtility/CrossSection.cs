using Geometry;
using Model;
using Model.Elements;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelController.MeshObjsUtility
{
    //public class Point3DComparer : IEqualityComparer<Point3D>
    //{
    //    private const double epsilon = 1e-3;

    //    public bool Equals(Point3D a, Point3D b)
    //    {
    //        return Math.Abs(a._x - b._x) < epsilon
    //               && Math.Abs(a._y - b._y) < epsilon
    //               && Math.Abs(a._z - b._z) < epsilon;
    //    }

    //    public int GetHashCode(Point3D obj)
    //    {
    //        unchecked
    //        {
    //            var hashCode = obj._x.GetHashCode();
    //            hashCode = (hashCode * 397) ^ obj._y.GetHashCode();
    //            hashCode = (hashCode * 397) ^ obj._z.GetHashCode();
    //            return hashCode;
    //        }
    //    }
    //}

    public class CrossSection
    {

        /// <returns>
        /// Сечение элемента плоскостью
        /// Возвращает словарь (номер элемента, плоскость его сечения)
        /// </returns>
        public Dictionary<int, Surface> GetSectionSurfaces(List<Element3D> elements, Plane plane)
        {
            var comparer = new Point3DEqualityComparer();
            var result = new Dictionary<int, Surface>();
            var index = 0;
            foreach (var element in elements)
            {
                var points = new List<Point3D>();
                foreach (var surface in element.GetSurface())
                {
                    foreach (var line in surface.GetLinesFromSurface(surface))
                    {
                        var point1 = line.P0;
                        var point2 = line.P1;
                        if (IsIntersect(point1, point2, plane))
                        {
                            var sectionPoint = GetSectionPoint(new Segment3D(point1, point2), plane);
                            points.Add(sectionPoint);
                        }
                    }
                }
                var nodes = points
                    .Distinct(comparer)
                    .Select((p, i) => new Node(i, p))
                    .ToArray();
                if (nodes.Length >= 3
                    && !IsAllPointsOnLine(nodes.Select(x => x.Position).ToList()))
                {
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
            if (float.IsNaN(d1) || float.IsNaN(d2))
                throw new Exception();
            if (Math.Abs(d1) < 10e-6 && Math.Abs(d2) < 10e-6)
                return false;
            return Math.Sign(d1) * Math.Sign(d2) <= 0;
        }

        /// <summary>
        /// Вычисляет точку пересечения прямой и плоскости
        /// </summary>
        private Point3D GetSectionPoint(Segment3D line, Plane plane)
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

        private bool IsAllPointsOnLine(List<Point3D> points)
        {
            var initialVector = new Point3D(points.Last()._x - points[0]._x,
                points.Last()._y - points[0]._y, points.Last()._z - points[0]._z);
            for (var i = 0; i < points.Count - 1; i++)
            {
                var currentVector = new Point3D(points[i + 1]._x - points[i]._x,
                    points[i + 1]._y - points[i]._y, points[i + 1]._z - points[i]._z);
                if (Math.Abs(currentVector._x / initialVector._x - currentVector._y / initialVector._y) >= 1e-4)
                    return false;
            }
            return true;
        }
    }
}