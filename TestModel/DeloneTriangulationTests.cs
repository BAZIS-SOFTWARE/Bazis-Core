using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Model.MeshObjects;
using ModelController.MeshObjsCreator.Delone2D;
using SurfaceMesher;

namespace TestModel
{
    [TestClass]
    public class DeloneTriangulationTests
    {
        [TestMethod]
        public void GetElements_ReturnsCorrectTriangles_OnArbitraryPoints()
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
            var instance = new SurfaceMeshCreator(new DeloneTriangulation());
            
            var triangulation = instance.DeloneTriangulation(nodes).ToList();

            Assert.AreEqual(11, triangulation.Count);
        }

        [TestMethod]
        public void GetElements_twiceCall()
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
            var instance = new SurfaceMeshCreator(new DeloneTriangulation());
            var triangulation = instance.DeloneTriangulation(nodes).ToList();

            Assert.AreEqual(11, triangulation.Count);

            triangulation = instance.DeloneTriangulation(nodes).ToList();

            Assert.AreEqual(11, triangulation.Count);
        }

        [TestMethod]
        public void GetElements_ReturnCorrectTriangles_WhenThreePointsOnLine()
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
            var instance = new SurfaceMeshCreator(new DeloneTriangulation());
            var triangulation = instance.DeloneTriangulation(nodes).ToList();

            Assert.AreEqual(7, triangulation.Count);
        }

        [TestMethod]
        public void GetElements_ReturnsCorrectTriangleNumber_OnAmbiguousCase()
        {
            var nodes = new List<Node>
            {
                new Node(0, new Point3D(1, 1, 0)),
                new Node(1, new Point3D(-1, 1, 0)),
                new Node(2, new Point3D(1, -1, 0)),
                new Node(3, new Point3D(-1, -1, 0))
            };
            var instance = new SurfaceMeshCreator(new DeloneTriangulation());
            var triangulation = instance.DeloneTriangulation(nodes).ToList();
            Assert.AreEqual(2, triangulation.Count);
        }

        [TestMethod]
        [Timeout(10000)]
        public void GetTriangles_ShouldBeFast_WhenManyNodesGiven()
        {
            using (var reader = new StreamReader("../../../Examples/2D_axi/10e5_nodes.txt"))
            {
                var nodes = new List<Node>();
                var index = 0;
                while (!reader.EndOfStream)
                {
                    var nodePosition = reader.ReadLine().Split().Select(int.Parse).ToArray();
                    var node = new Node(index++, new Point3D(nodePosition[0], nodePosition[1], 0));
                    nodes.Add(node);
                }

                var instance = new SurfaceMeshCreator(new DeloneTriangulation());
                var triangulation = instance.DeloneTriangulation(nodes).ToList();
            }
        }
    }
}
