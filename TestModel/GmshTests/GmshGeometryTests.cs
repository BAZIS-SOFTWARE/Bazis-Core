using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GmshWrapper;
using Model.GmshObject;
using System.Linq;
using Model;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GmshGeometryTests
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
        public void GetCurvesTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var curves = geometry.GetСurves();
                Assert.AreEqual(6, curves.Count());
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements1DTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            { 
                var lines = geometry.GetElements1D();
                Assert.AreEqual(6, lines.Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetControlPointsTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var points = geometry.GetControlPoints();
                Assert.AreEqual(4, points.Count());
            }
        }

        [DataTestMethod]
        [DataRow("test.geo")]
        public void TestCurveSteps(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var curveUntil = geometry.GetСurves().First();
                geometry.CurveSteps = 15; // but default is 20
                var curveAfter = geometry.GetСurves().First();
                Assert.IsTrue(curveAfter.GetNodes().Count() < curveUntil.GetNodes().Count());
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestDeleteEntity(string fileName)
        {
            using (var mesh = new MeshObject(modelFolder + fileName))
            {
                mesh.Generate(1);
                var nodes = mesh.GetElement1D();
                mesh.DeleteEntities(new int[] { 11 }, true);
                mesh.Synchronize();
                mesh.Generate(1);
                var nodesAfter = mesh.GetElement1D();
            }
        }

        [DataTestMethod]
        [Timeout(60000)]
        [DataRow(10000)]
        public void ModelTest(int count)
        {
            var models = new GeometryObject[count];
            for(var i = 0; i < count; ++i)
            {
                models[i] = new GeometryObject();
                Assert.AreEqual(true, GmshWrapperModel.ModelFind(models[i].Id.ToString()));
            }
            Assert.AreEqual(models.Length + 1, GmshWrapperModel.ModelList().Length);
            for (var i = 0; i < count; ++i)
                models[i].Dispose();
            var list = GmshWrapperModel.ModelList();
            Assert.AreEqual(true, list.Length == 1 && list[0] == string.Empty);
        }
    }
}
