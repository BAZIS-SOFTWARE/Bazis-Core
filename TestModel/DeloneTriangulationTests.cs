using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Elements;
using ModelController.MeshObjsCreator;

namespace TestModel
{
    [TestClass]
    public class DeloneTriangulationTests
    {
        [TestMethod]
        public void GetTriangles_ReturnsCorrectTriangles_OnArbitraryPoints_1()
        {
            var nodes = new List<Node>
            {
                new Node(0, new Point3D(0, 7, 0)),
                new Node(1, new Point3D(-5, 5, 0)),
                new Node(2, new Point3D(5, 5, 0)),
                new Node(3, new Point3D(-2, 3, 0)),
                new Node(4, new Point3D(3, 1, 0)),
                new Node(5, new Point3D(-4, -1, 0)),
                new Node(6, new Point3D(1, -2, 0)),
                new Node(7, new Point3D(-6, -4, 0)),
                new Node(8, new Point3D(5, -4, 0))
            };
            var instance = new DeloneTriangulation(nodes);
            var triangulation = instance.GetTriangles().ToList();

            Assert.AreEqual(11, triangulation.Count);
        }

        [TestMethod]
        public void GetTriangles_ReturnsCorrectTriangles_OnArbitraryPoints_2()
        {
            var nodes = new List<Node>
            {
                new Node(0, new Point3D(2, 2, 0)),
                new Node(1, new Point3D(1, -1, 0)),
                new Node(2, new Point3D(3, 2, 0)),
                new Node(3, new Point3D(-4, 2, 0)),
                new Node(4, new Point3D(0, 5, 0)),
                new Node(5, new Point3D(-3, 4, 0)),
                new Node(6, new Point3D(-5, -3, 0)),
                new Node(7, new Point3D(3, -3, 0)),
                new Node(8, new Point3D(2, -4, 0)),
                new Node(9, new Point3D(-1, -2, 0))
            };
            var instance = new DeloneTriangulation(nodes);
            var triangulation = instance.GetTriangles().ToList();

            Assert.AreEqual(11, triangulation.Count);
        }

        [TestMethod]
        public void GetTriangles_ReturnCorrectTriangles_WhenThreePointsOnLine()
        {
            var nodes = new List<Node>
            {
                new Node(0, new Point3D(1, 1, 0)),
                new Node(1, new Point3D(3, 4, 0)),
                new Node(2, new Point3D(-2, 3, 0)),
                new Node(3, new Point3D(-2, 2, 0)),
                new Node(4, new Point3D(-1, -1, 0)),
                new Node(5, new Point3D(-2, -3, 0)),
                new Node(6, new Point3D(4, -2, 0))
            };
            var instance = new DeloneTriangulation(nodes);
            var triangulation = instance.GetTriangles().ToList();
            Assert.AreEqual(7, triangulation.Count);
        }

        [TestMethod]
        public void GetTriangles_ReturnsCorrectTriangleNumber_OnAmbiguousCase()
        {
            var nodes = new List<Node>
            {
                new Node(0, new Point3D(1, 1, 0)),
                new Node(1, new Point3D(-1, 1, 0)),
                new Node(2, new Point3D(1, -1, 0)),
                new Node(3, new Point3D(-1, -1, 0))
            };
            var instance = new DeloneTriangulation(nodes);
            var triangulation = instance.GetTriangles();
            Assert.AreEqual(2, triangulation.Count());
        }

        [TestMethod]
        public void SampleTest()
        {
            var nodes = new List<Node>
            {
                new Node(0, new Point3D(-2, 2, 0)),
                new Node(1, new Point3D(5, 4, 0)),
                new Node(2, new Point3D(5, 1, 0)),
                new Node(3, new Point3D(2, 3, 0)),
                new Node(4, new Point3D(-2, 5, 0)),
                new Node(5, new Point3D(-5, 1, 0)),
                new Node(6, new Point3D(-2, -2, 0)),
                new Node(7, new Point3D(6, -2, 0)),
                new Node(8, new Point3D(4, -4, 0)),
                new Node(9, new Point3D(-4, -4, 0)),
                new Node(10, new Point3D(-7, -2, 0)),
                new Node(11, new Point3D(3, 2, 0)),
                new Node(12, new Point3D(3, 5, 0)),
                new Node(13, new Point3D(9, 4, 0)),
                new Node(14, new Point3D(8, 1, 0)),
                new Node(15, new Point3D(2, -3, 0)),
                new Node(16, new Point3D(-4, -1, 0)),
                new Node(17, new Point3D(-7, 2, 0)),
                new Node(18, new Point3D(0, 0, 0)),
                new Node(19, new Point3D(3, 0, 0)),
                new Node(20, new Point3D(0, 5, 0)),
            };
            var instance = new DeloneTriangulation(nodes);
            var triangulation = instance.GetTriangles().ToList();
            Assert.AreEqual(30, triangulation.Count);
        }
    }
}
