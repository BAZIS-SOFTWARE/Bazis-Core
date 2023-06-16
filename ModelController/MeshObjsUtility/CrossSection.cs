using Geometry;
using Model;
using Model.Interfaces;
using System;
using System.Collections.Generic;

namespace ModelController.MeshObjsUtility
{
    public static class IEnumerableExtentions
    {
        //public static IEnumerable<Tuple<T, T>> Bigrams<T>(this IEnumerable<T> items)
        //{
        //    var enumerator = items.GetEnumerator();
        //    enumerator.MoveNext();
        //    var first = enumerator.Current;
        //    var head = items.First();
        //    while (enumerator.MoveNext())
        //    {
        //        var second = enumerator.Current;
        //        yield return Tuple.Create(first, second);
        //        first = second;
        //    }
        //    yield return Tuple.Create(head, first);
        //}
    }
    /// <summary>
    /// CutSection
    /// </summary>
    public class CrossSection
    {
        /// <summary>
        /// GetCrossNodes
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public Dictionary<int, Surface> GetSectionSurfaces(List<Element3D> elements, Plane plane)
        {
            Dictionary<int, Surface> dictionary = new Dictionary<int, Surface>();
            int index = 0;
            foreach (Element3D element in elements)
            {
                List<Node> list = new List<Node>();
                HashSet<Point3D> hashSet = new HashSet<Point3D>();
                foreach (Surface item4 in element.GetSurface()) // get lines
                {
                    foreach (Line item5 in item4.GetLinesFromSurface(item4))
                    {
                        Point3D item = item5.P0;
                        Point3D item2 = item5.P1;
                        if (Math.Sign(plane.PointPosition(item)) != Math.Sign(plane.PointPosition(item2)))
                        {
                            Point3D sectionPoint = GetCrossSectionPoint(new Line(item, item2), plane);
                            if (!hashSet.Contains(sectionPoint))
                            {
                                Node item3 = new Node(index++, sectionPoint);
                                hashSet.Add(sectionPoint);
                                list.Add(item3);
                            }
                        }
                    }
                }
                if (list.Count >= 3)
                {
                    Surface value = new Surface(index++, list.ToArray(), ObjKind.ElementSurface);
                    dictionary.Add(element.Number, value);
                }
            }
            return dictionary;
        }

        private Point3D GetCrossSectionPoint(Line line, Plane plane)
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