using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GmshObject;
using Model.GmshWrapper;
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
        public void GetElements1D(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(1);
                var elements = mesh.GetElement1D();
                Assert.AreEqual(46, elements.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements2D(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(2);
                var elements = mesh.GetElement2D();
                Assert.AreEqual(370, elements.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements3D(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(3);
                var elements = mesh.GetElement3D();
                Assert.AreEqual(707, elements.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetNodes(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(2);
                var nodes = mesh.GetNodes();
                Assert.AreEqual(187, nodes.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestRefineMesh(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(2);
                var nodesUntil = mesh.GetNodes();
                mesh.RefineMesh();
                var nodesAfter = mesh.GetNodes();
                Assert.IsTrue(nodesAfter.Count() > nodesUntil.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestDensity(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(2);
                var untilElements = mesh.GetElement2D();
                mesh.Density = 0.5;
                mesh.Generate(2);
                var afterElements = mesh.GetElement2D();
                Assert.IsTrue(untilElements.Count() < afterElements.Count());
            }
        }
    }
}
