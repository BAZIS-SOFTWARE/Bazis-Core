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
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var line = geometry.GetElements1D().First();
                geometry.DeleteEntities(new int[] { line.Number });
                var lines = geometry.GetElements1D();
            }
        }
    }
}
