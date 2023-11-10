using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelController.MeshObjsCreator;

namespace TestModel
{
    [TestClass]
    public class DeloneTriangulationTests
    {
        [TestMethod]
        public void GetTriangles_ReturnsCorrectTriangles_OnArbitraryPoints()
        {
            var points = new List<Point2D>
            {
                new Point2D(0, 7),
                new Point2D(-5, 5),
                new Point2D(5, 5),
                new Point2D(-2, 3),
                new Point2D(3, 1),
                new Point2D(-4, -1),
                new Point2D(1, -2),
                new Point2D(-6, -4),
                new Point2D(5, -4)
            };
            var instance = new DeloneTriangulation(points);
            var triangulation = instance.GetTriangles();

            var expected = new HashSet<DeloneTriangle>
            {
                new DeloneTriangle(1, 0, 3),
                new DeloneTriangle(1, 3, 4),
                new DeloneTriangle(3, 4, 2),
                new DeloneTriangle(2, 4, 6),
                new DeloneTriangle(2, 6, 5),
                new DeloneTriangle(7, 8, 5),
                new DeloneTriangle(5, 8, 2),
                new DeloneTriangle(2, 3, 8),
                new DeloneTriangle(0, 3, 8),
                new DeloneTriangle(0, 7, 1),
                new DeloneTriangle(0, 7, 8)
            };

            Assert.IsTrue(expected.SetEquals(triangulation));
        }

        [TestMethod]
        public void GetTriangles_ReturnCorrectTriangles_WhenThreePointsOnLine()
        {
            var points = new List<Point2D>
            {
                new Point2D(1, 1),
                new Point2D(3, 4),
                new Point2D(-2, 3),
                new Point2D(-2, 2),
                new Point2D(-1, -1),
                new Point2D(-2, -3),
                new Point2D(4, -2)
            };
            var instance = new DeloneTriangulation(points);
            var triangulation = instance.GetTriangles();

            var expected = new HashSet<DeloneTriangle>
            {
                new DeloneTriangle(1, 0, 2),
                new DeloneTriangle(0, 4, 2),
                new DeloneTriangle(3, 4, 2),
                new DeloneTriangle(5, 4, 3),
                new DeloneTriangle(5, 6, 4),
                new DeloneTriangle(6, 4, 0),
                new DeloneTriangle(6, 0, 1)
            };

            Assert.IsTrue(expected.SetEquals(triangulation));
        }

        [TestMethod]
        public void GetTriangles_ReturnsSingleTriangle_OnThreePoints()
        {
            var points = new List<Point2D>
            {
                new Point2D(0, 0),
                new Point2D(1, 0),
                new Point2D(0, 1),
            };
            var instance = new DeloneTriangulation(points);
            var triangulation = instance.GetTriangles();

            var expected = new HashSet<DeloneTriangle>
            {
                new DeloneTriangle(1, 0, 2),
            };

            Assert.IsTrue(expected.SetEquals(triangulation));
        }

        [TestMethod]
        public void GetTriangles_ReturnsCorrectTriangleNumber_OnAmbiguousCase()
        {
            var points = new List<Point2D>
            {
                new Point2D(1, 1),
                new Point2D(-1, 1),
                new Point2D(1, -1),
                new Point2D(-1, -1)
            };
            var instance = new DeloneTriangulation(points);
            var triangulation = instance.GetTriangles();
            Assert.AreEqual(2, triangulation.Count());
        }
    }
}
