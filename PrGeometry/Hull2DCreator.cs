using System;
using System.Collections.Generic;
using System.Linq;

namespace Geometry
{
    public class Hull2DCreator
    {
        private readonly IEnumerable<Point2D> points;

        public Hull2DCreator(IEnumerable<Point2D> points)
        {
            if (points.Count() < 3)
                throw new ArgumentException("Unable to build a convex hull. Number of points must be 3 or more.");
            var segment = new Segment2D(points.Last(), points.First());
            if (points.All(x => segment.IsPointBelongSegment(x)))
                throw new ArgumentException("Unable to build a convex hull. All points belongs one line.");
            this.points = points;
        }

        /// <summary>
        /// Создание выпуклой оболочки по алгоритму Грэхэма.
        /// Из набора точек создается замыкающаяся выпуклая оболочка
        /// https://neerc.ifmo.ru/wiki/index.php?title=%D0%A1%D1%82%D0%B0%D1%82%D0%B8%D1%87%D0%B5%D1%81%D0%BA%D0%B8%D0%B5_%D0%B2%D1%8B%D0%BF%D1%83%D0%BA%D0%BB%D1%8B%D0%B5_%D0%BE%D0%B1%D0%BE%D0%BB%D0%BE%D1%87%D0%BA%D0%B8:_%D0%94%D0%B6%D0%B0%D1%80%D0%B2%D0%B8%D1%81,_%D0%93%D1%80%D1%8D%D1%85%D0%B5%D0%BC,_%D0%AD%D0%BD%D0%B4%D1%80%D1%8E,_%D0%A7%D0%B5%D0%BD,_QuickHull#.D0.90.D0.BB.D0.B3.D0.BE.D1.80.D0.B8.D1.82.D0.BC_.D0.93.D1.80.D1.8D.D1.85.D0.B5.D0.BC.D0.B0
        /// </summary>
        /// <returns>Коллекцию точек оболочки по порядку против часовой стрелки от самой нижней левой точки</returns>
        public IEnumerable<Point2D> GetHullGraham()
        {
            var startPoint = points
                .OrderBy(p => p._y)
                .ThenBy(p => p._x)
                .First();
            var sortedPoints = points
                .Distinct()
                .Where(point => point != startPoint)
                .GroupBy(point => GetPolarAngle(startPoint, point))
                .Select(group => 
                    group.OrderByDescending(point => Vector.GetVectorLength(point.Sub(startPoint)))
                         .First())
                .OrderBy(point => GetPolarAngle(startPoint, point))
                .ToArray();
            var stack = new Stack<Point2D>();
            stack.Push(startPoint);
            stack.Push(sortedPoints[0]);
            stack.Push(sortedPoints[1]);
            for(var i = 2; i < sortedPoints.Length; i++)
            {
                while (new Segment2D(NextToTop(stack), stack.Peek()).GetOrientationSign(sortedPoints[i]) <= 0)
                {
                    stack.Pop();
                }
                stack.Push(sortedPoints[i]);
            }
            return stack.Reverse();
        }

        /// <summary>
        /// Создание выпуклой оболочки по алгоритму Эдрю.
        /// Из набора точек создается верхняя и нижняя полуоболочки, объединение которых дает целую замыкающуюся выпуклую оболочку.
        /// https://neerc.ifmo.ru/wiki/index.php?title=%D0%A1%D1%82%D0%B0%D1%82%D0%B8%D1%87%D0%B5%D1%81%D0%BA%D0%B8%D0%B5_%D0%B2%D1%8B%D0%BF%D1%83%D0%BA%D0%BB%D1%8B%D0%B5_%D0%BE%D0%B1%D0%BE%D0%BB%D0%BE%D1%87%D0%BA%D0%B8:_%D0%94%D0%B6%D0%B0%D1%80%D0%B2%D0%B8%D1%81,_%D0%93%D1%80%D1%8D%D1%85%D0%B5%D0%BC,_%D0%AD%D0%BD%D0%B4%D1%80%D1%8E,_%D0%A7%D0%B5%D0%BD,_QuickHull#.D0.90.D0.BB.D0.B3.D0.BE.D1.80.D0.B8.D1.82.D0.BC_.D0.AD.D0.BD.D0.B4.D1.80.D1.8E
        /// </summary>
        /// <returns>Коллекцию точек оболочки по порядку против часовой стрелки от самой нижней левой точки</returns>
        public IEnumerable<Point2D> GetHullAndrew()
        {
            bool IsNonClockWiseRotation(Point2D o, Point2D a, Point2D b)
            {
                return ((a._x - o._x) * (b._y - o._y) - (a._y - o._y) * (b._x - o._x)) <= 0;
            }

            var sortedPoints = points.OrderBy(p => p._x).ThenBy(p => p._y).ToList();
            var byPassAr = new Point2D[sortedPoints.Count() * 2];
            var lastIndex = 0;

            //lower hull
            for(var i = 0; i < sortedPoints.Count(); ++i)
            {
                while (lastIndex >= 2 && IsNonClockWiseRotation(byPassAr[lastIndex - 2], byPassAr[lastIndex - 1], sortedPoints[i]))
                    lastIndex--;
                byPassAr[lastIndex++] = sortedPoints[i];
            }

            //upper hull
            var lowerBound = lastIndex + 1;
            for (var i = sortedPoints.Count() - 2; i >= 0; i--)
            {
                while (lastIndex >= lowerBound && IsNonClockWiseRotation(byPassAr[lastIndex - 2], byPassAr[lastIndex - 1], sortedPoints[i]))
                    lastIndex--;
                byPassAr[lastIndex++] = sortedPoints[i];
            }

            var result = new Point2D[lastIndex - 1];
            Array.Copy(byPassAr, 1, result, 0, lastIndex - 1);

            return result;
        }

        private Point2D NextToTop(Stack<Point2D> stack)
        {
            var top = stack.Pop();
            var nextToTop = stack.Peek();
            stack.Push(top);
            return nextToTop;
        }

        //Angle between vector i = (1, 0) and vector (localCenter - p)
        private float GetPolarAngle(Point2D localCenter, Point2D p)
        {
            var dotProduct = p._x - localCenter._x;
            var length = Vector.GetVectorLength(p.Sub(localCenter));
            return (float)Math.Acos(dotProduct / length);
        }
    }
}