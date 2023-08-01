using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;
using System.Linq;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GmshMeshTests
    {
        private static string modelFolder = @"..\..\Examples\GMSHConnector\";

        [TestInitialize]
        public void Init()
        {
            GmshWrapperConnection.Initialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            GmshWrapperConnection.Finalize();
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetNodes(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(2);
                var nodes = mesh.GetNodes();
                Assert.AreEqual(187, nodes.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestRefineMesh(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(2);
                var nodesUntil = mesh.GetNodes();
                mesh.RefineMesh();
                var nodesAfter = mesh.GetNodes();
                Assert.IsTrue(nodesAfter.Count() > nodesUntil.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements1DTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(1);
                var elements = mesh.GetElement1D();
                Assert.AreEqual(46, elements.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements2D(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(2);
                var elements = mesh.GetElement2D();
                Assert.AreEqual(370, elements.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements3D(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(3);
                var elements = mesh.GetElement3D();
                Assert.AreEqual(707, elements.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestDensity(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(2);
                var untilElements = mesh.GetElement2D();
                mesh.Density = 0.5;
                geometry.Remesh(2);
                var afterElements = mesh.GetElement2D();
                Assert.IsTrue(untilElements.Count() < afterElements.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetDensityForNodesTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(1);
                var length1 = mesh.GetNodes().ToArray().Length;
                mesh.Density = 0.5;
                mesh.Generate(1);
                var length2 = mesh.GetNodes().ToArray().Length;
                Assert.AreEqual(true, length2 > length1);
                mesh.Density = 1;
                mesh.Generate(1);
                var length3 = mesh.GetNodes().ToArray().Length;
                Assert.AreEqual(true, length3 < length2);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetDensityFor2DElementsTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(2);
                var length1 = mesh.GetElement2D().ToArray().Length;
                mesh.Density = 0.5;
                mesh.Remesh(2);
                var length2 = mesh.GetElement2D().ToArray().Length;
                Assert.AreEqual(true, length2 > length1);
                mesh.Density = 1;
                mesh.Remesh(2);
                var length3 = mesh.GetElement2D().ToArray().Length;
                Assert.AreEqual(true, length3 < length2);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetDensityFor3DElementsTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(3);
                var length1 = mesh.GetElement3D().ToArray().Length;
                mesh.Density = 0.5;
                mesh.Remesh(3);
                var length2 = mesh.GetElement3D().ToArray().Length;
                Assert.AreEqual(true, length2 > length1);
                mesh.Density = 1;
                mesh.Remesh(3);
                var length3 = mesh.GetElement3D().ToArray().Length;
                Assert.AreEqual(true, length3 < length2);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 1, 2, 3 }, new int[] { 112, 112, 74 })]
        public void GetSurfaceByIdTest(string fileName, int[] tags, int[] expLength )
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(2);
                for (var i = 0; i < tags.Length; ++i)
                {
                    var actual = mesh.GetSurfaceById(tags[i]).ToArray();
                    Assert.AreEqual(expLength[i], actual.Length);
                }
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestChangeTriangulationAlgorithm2D(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(2);
                var nodesCountUntil = mesh.GetNodes().Count();
                mesh.ChangeTriangulationAlgorithm2D(Triangulation2DAlgorithm.Automatic);
                mesh.Generate(2);
                var nodesCountAfter = mesh.GetNodes().Count();
                Assert.IsTrue(nodesCountAfter != nodesCountUntil);
            }
        }
        /*
        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestChangeTriangulationAlgorithm3D(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(3);
                var nodesCountUntil = mesh.GetNodes().Count();
                mesh.ChangeTriangulationAlgorithm3D(Triangulation3DAlgorithm.MMG3D);
                mesh.Generate(3);
                var nodesCountAfter = mesh.GetNodes().Count();
                Assert.IsTrue(nodesCountAfter != nodesCountUntil);
            }
        }*/

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestSetPointsOnCurve(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(2);
                var nodesCountUntil = mesh.GetNodes().Count();
                mesh.SetMeshPointsOnCurve(10, TransfiniteAlgorithm.Progressive, 1.5, 4);
                mesh.Remesh(2);
                var nodesCountAfter = mesh.GetNodes().Count();
                Assert.IsTrue(nodesCountAfter != nodesCountUntil);
                var lengthBefore = mesh.GetElement2D().ToArray().Length;    
                mesh.RemoveSurfaceById(2);
                var lengthAfter = mesh.GetElement2D().ToArray().Length;
                Assert.AreEqual(true,lengthBefore > lengthAfter);
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", 2)]
        public void DeleteSurfaceTest(string fileName, int surfaceId)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(2);
                var lengthBefore = mesh.GetElement2D().ToArray().Length;
                mesh.RemoveSurfaceById(2);
                var lengthAfter = mesh.GetElement2D().ToArray().Length;
                Assert.AreEqual(true, lengthBefore > lengthAfter);
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 206, 261, 251, 265, 210, 213 })]
        public void DeleteMeshElementsTest(string fileName, int[] elTags)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var mesh = geometry.Generate(2);
                var lengthBefore = mesh.GetElement2D().ToArray().Length;
                mesh.RemoveElements2D(elTags);
                var lengthAfter = mesh.GetElement2D().ToArray().Length;
                Assert.IsTrue(lengthBefore > lengthAfter);
            }
        }
    }
}