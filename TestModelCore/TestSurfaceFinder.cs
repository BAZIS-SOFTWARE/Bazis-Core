using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.MeshObjects;
using Model.Utilities;

namespace ModelTests
{
    [TestClass]
    public class TestSurfaceFinder
    {
        [TestMethod]
        public void FindSurfaceNodes_ReturnsCorrectSurfaces_OnHexa()
        {
            var nodes = new List<Node>
            {
                new Node(1, new Point3D(0, 0, 0)),
                new Node(2, new Point3D(1, 0, 0)),
                new Node(3, new Point3D(0, 1, 0)),
                new Node(4, new Point3D(0, 0, 1)),
                new Node(5, new Point3D(1, 1, 0)),
                new Node(6, new Point3D(1, 0, 1)),
                new Node(7, new Point3D(0, 1, 1)),
                new Node(8, new Point3D(1, 1, 1))
            };
            var surfaces = SurfacesFinder.FindSurfaceNodes(nodes.ToArray())
                .Select(array => array.Select(node => node.Number).ToHashSet());

            var expected = new HashSet<HashSet<int>>
            {
                new HashSet<int> { 1, 2, 3, 5 },
                new HashSet<int> { 2, 5, 6, 8 },
                new HashSet<int> { 3, 5, 7, 8 },
                new HashSet<int> { 1, 2, 4, 6 },
                new HashSet<int> { 1, 3, 4, 7 },
                new HashSet<int> { 4, 6, 7, 8 },
            };

            Assert.IsTrue(expected.All(expectedSet => 
                surfaces.Any(actualSet => actualSet.SetEquals(expectedSet))));
        }

        [TestMethod]
        public void FindSurfaceNodes_ReturnsCorrectSurfaceNumber_OnPenta()
        {
            var nodes = new List<Node>
            {
                new Node(1, new Point3D(0, 0, 0)),
                new Node(2, new Point3D(0, 1, 0)),
                new Node(4, new Point3D(1, 0, 0)),
                new Node(5, new Point3D(0, 0, 1)),
                new Node(6, new Point3D(0, 1, 1)),
                new Node(8, new Point3D(1, 0, 1))
            };

            var surfaces = SurfacesFinder.FindSurfaceNodes(nodes.ToArray()).ToList();

            Assert.AreEqual(5, surfaces.Count);
        }

        [TestMethod]
        public void FindSurfaceNodes_ReturnsCorrectSurfaceNumber_OnPyramid()
        {
            var nodes = new List<Node>
            {
                new Node(1, new Point3D(0, 0, 0)),
                new Node(2, new Point3D(0, 1, 0)),
                new Node(3, new Point3D(1, 1, 0)),
                new Node(4, new Point3D(1, 0, 0)),
                new Node(5, new Point3D(0.5f, 0.5f, 1)),
            };

            var surfaces = SurfacesFinder.FindSurfaceNodes(nodes.ToArray()).ToList();

            Assert.AreEqual(5, surfaces.Count);
        }

        [TestMethod]
        public void Sample()
        {
            var nodes = new List<Node>
            {
                new Node(1, new Point3D(0, 0, 0)),
                new Node(2, new Point3D(0, 1, 0)),
                new Node(3, new Point3D(1, 1, 0)),
                new Node(4, new Point3D(1, 0, 0)),
                new Node(5, new Point3D(0, 0, 1)),
                new Node(6, new Point3D(0, 1, 1)),
                new Node(7, new Point3D(1, 1, 1)),
                new Node(8, new Point3D(1, 0, 1)),
                new Node(9, new Point3D(1, 1.5f, 0.5f)),
                new Node(10, new Point3D(0, 1.5f, 0.5f))
            };

            var surfaces = SurfacesFinder.FindSurfaceNodes(nodes.ToArray()).ToList();

            Assert.AreEqual(7, surfaces.Count);
        }
    }
}