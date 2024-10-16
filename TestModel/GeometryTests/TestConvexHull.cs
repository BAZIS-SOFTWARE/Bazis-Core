using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PrTests
{
    [TestClass]
    public class TestConvexHull
    {
        /// <summary>
        /// Метод по созданию списка точек для тестовых методов
        /// </summary>
        /// <param name="pointValues">Двумерный массив, подмассивами которого являются массивы для создания точек</param>
        /// <returns>Список точек, созданный по многомерному массиву данных для точек</returns>
        public List<Point2D> CreateListOfPoints(params int[][] pointValues) =>
            pointValues.Select(pv => new Point2D(pv[0], pv[1])).ToList();

        [TestMethod]
        public void ConvexHullGraham_ShouldReturnCorrectPointSequence()
        {
            var points = new List<Point2D>
            {
                new Point2D(2, 0), new Point2D(2, 1),
                new Point2D(4, 0), new Point2D(3, 3),
                new Point2D(6, 1), new Point2D(6, 3),
                new Point2D(5, 2), new Point2D(4, 5),
                new Point2D(2, 5), new Point2D(0, 3),
                new Point2D(0, 1)
            };

            var hullCreator = new Hull2DCreator(points);

            var convexHull = hullCreator.GetHullGraham();

            var expected = new List<Point2D>
            {
                new Point2D(2, 0), new Point2D(4, 0),
                new Point2D(6, 1), new Point2D(6, 3),
                new Point2D(4, 5), new Point2D(2, 5),
                new Point2D(0, 3), new Point2D(0, 1)
            };
            Assert.IsTrue(expected.SequenceEqual(convexHull));
        }

        [TestMethod]
        public void ConvexHull_ShouldFail_WhenLessThenThreePointsInSequence()
        {
            var points = new List<Point2D>()
            {
                new Point2D(1, 2), new Point2D(2, 3),
            };
            Assert.ThrowsException<ArgumentException>(() => new Hull2DCreator(points).GetHullGraham());
        }

        [TestMethod]
        [DataRow(new[] {1, 1}, new[] {1, 1}, new[] {1, 1}, new[] {2, 2})]
        [DataRow(new[] { 1, 1 }, new[] { 1, 1 }, new[] { 2, 2 }, new[] { 2, 2 })]
        [DataRow(new[] { 2, 2 }, new[] { 2, 2 }, new[] { 2, 2 }, new[] { 2, 2 })]
        public void ConvexHull_ShouldFail_WhenLessThenThreeRealPointsInSequenceAndSomePointsInOnePlace(params int[][] pointsValues)
        {
            Assert.ThrowsException<ArgumentException>(() => new Hull2DCreator(CreateListOfPoints(pointsValues)).GetHullGraham());
        }

        [TestMethod]
        [DataRow(new[] { 1, 1 }, new[] { 1, 1 }, new[] {1, 1})]
        [DataRow(new[] { 10, 10 }, new[] { 9, 9 }, new[] { 8, 8 }, new[] {7, 7}, new[] {6, 6}, new[] {5, 5})]
        [DataRow(new[] { -10, -10 }, new[] { -9, -9 }, new[] { -8, -8 },new[] {-7, -7}, new[] {-6, -6}, new[] {-5, -5})]
        public void ConvexHull_ShouldFail_WhenAllPointsInSequenceBelongOneLine(params int[][] pointsValues)
        {
            Assert.ThrowsException<ArgumentException>(() => new Hull2DCreator(CreateListOfPoints(pointsValues)).GetHullGraham());
        }

        [TestMethod]
        public void ConvexHullGraham_ShouldReturnCorrectPointSequenceInRhombus()
        {
            var checkPoints = new List<Point2D>()
            {
                new Point2D(0, -2), new Point2D(1, -1), new Point2D(2, 0), new Point2D(1, 1), 
                new Point2D(0, 2), new Point2D(-1, 1), new Point2D(-2, 0), new Point2D(-1, -1),
                // inner points
                new Point2D(0, 0), new Point2D(0, 1), new Point2D(1, 0), new Point2D(-1, 0), new Point2D(0, -1)
            };

            var expectedPoints = new List<Point2D>()
            {
                new Point2D(0, -2), new Point2D(2, 0),
                new Point2D(0, 2),  new Point2D(-2, 0)
            };

            var hullCreator = new Hull2DCreator(checkPoints);
            var convexHull = hullCreator.GetHullGraham();

            Assert.IsTrue(expectedPoints.SequenceEqual(convexHull));
        }

        [TestMethod]
        public void ConvexHullGraham_ShouldReturnCorrectPointSequenceInHexagon()
        {
            var checkPoints = new List<Point2D>()
            {
                new Point2D(-1, -3), new Point2D(0, -3), new Point2D(1, -3), new Point2D(3, 0),
                new Point2D(1, 3), new Point2D(0, 3), new Point2D(-1, 3), new Point2D(-3, 0),
                // inner points
                new Point2D(0, 0), new Point2D(0, 1), new Point2D(1, 0), new Point2D(-1, 0), new Point2D(0, -1)
            };

            var expectedPoints = new List<Point2D>()
            {
                new Point2D(-1, -3), new Point2D(1, -3), new Point2D(3, 0),
                new Point2D(1, 3), new Point2D(-1, 3), new Point2D(-3, 0)
            };

            var hullCreator = new Hull2DCreator(checkPoints);
            var convexHull = hullCreator.GetHullGraham();

            Assert.IsTrue(expectedPoints.SequenceEqual(convexHull));
        }

        [TestMethod]
        public void ConvexHullGraham_ShouldReturnCorrectPointSequenceInConvexPolygon()
        {
            var checkPoints = new List<Point2D>()
            {
                new Point2D(1, 0), new Point2D(2, 0), new Point2D(3, 0), new Point2D(4, 0),
                new Point2D(6, 1), new Point2D(6, 2), new Point2D(6, 3), new Point2D(5, 4),
                new Point2D(4, 5), new Point2D(3, 5), new Point2D(2, 5), new Point2D(1, 4),
                new Point2D(0, 3), new Point2D(0, 2), new Point2D(0, 1),
                
                // inner points
                new Point2D(3, 1), new Point2D(5, 1), new Point2D(5, 3), new Point2D(4, 4),
                new Point2D(3, 4), new Point2D(2, 4), new Point2D(1, 3), new Point2D(1, 2),
            };

            var expectedPoints = new List<Point2D>()
            {
                new Point2D(1, 0), new Point2D(4, 0),
                new Point2D(6, 1), new Point2D(6, 3),
                new Point2D(4, 5), new Point2D(2, 5),
                new Point2D(0, 3), new Point2D(0, 1)
            };

            var hullCreator = new Hull2DCreator(checkPoints);
            var convexHull = hullCreator.GetHullGraham();

            Assert.IsTrue(expectedPoints.SequenceEqual(convexHull));
        }
    }
}
