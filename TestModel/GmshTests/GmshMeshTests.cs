using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GmshObject;
using Model.GmshWrapper;

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
    }
}
