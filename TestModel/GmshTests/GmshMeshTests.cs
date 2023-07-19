using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GmshObject;
using Model.GmshWrapper;
using System.Linq;
using Model.GmshController;
using System;
using System.Collections;

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
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(1);
                var e1d = mesh.GetNodes();
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements1DTest(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(1);
                var e1d = mesh.GetElement1D();
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements2DTest(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(2);
                var e2d = mesh.GetElement2D();
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements3DTest(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(3);
                var e3d = mesh.GetElement3D();
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetDensityForNodesTest(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(1);
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
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(2);
                var length1 = mesh.GetElement2D().ToArray().Length;
                mesh.Density = 0.5;
                mesh.Generate(2);
                var length2 = mesh.GetElement2D().ToArray().Length;
                Assert.AreEqual(true, length2 > length1);
                mesh.Density = 1;
                mesh.Generate(2);
                var length3 = mesh.GetElement2D().ToArray().Length;
                Assert.AreEqual(true, length3 < length2);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetDensityFor3DElementsTest(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(3);
                var length1 = mesh.GetElement3D().ToArray().Length;
                mesh.Density = 0.5;
                mesh.Generate(3);
                var length2 = mesh.GetElement3D().ToArray().Length;
                Assert.AreEqual(true, length2 > length1);
                mesh.Density = 1;
                mesh.Generate(3);
                var length3 = mesh.GetElement3D().ToArray().Length;
                Assert.AreEqual(true, length3 < length2);
            }
        }
    }
}
