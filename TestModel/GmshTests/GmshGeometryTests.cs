using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;
using System.Linq;
using GmshApi.GmshController;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GmshGeometryTests
    {
        private static string modelFolder = @"..\..\Examples\GMSHConnector\";
        [TestInitialize]
        public void Init()
        {
            ControllerConnection.Initialize();
        }
        [TestCleanup]
        public void Cleanup()
        {
            ControllerConnection.Finalize();
        }
        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetCurvesTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                ControllerModel.ModelSetCurrent(geometry.Id.ToString());
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
                ControllerModel.ModelSetCurrent(geometry.Id.ToString());
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
                ControllerModel.ModelSetCurrent(geometry.Id.ToString());
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
                ControllerModel.ModelSetCurrent(geometry.Id.ToString());
                var curveUntil = geometry.GetСurves().First();
                geometry.CurveSteps = 15; // but default is 20
                var curveAfter = geometry.GetСurves().First();
                Assert.IsTrue(curveAfter.NumberOfPoints < curveUntil.NumberOfPoints);
            }
        }

        [DataTestMethod]
        [Timeout(60000)]
        [DataRow(10000)]
        public void ModelTest(int count)
        {
            var geObjects = new GeometryObject[count];
            for(var i = 0; i < count; ++i)
            {
                geObjects[i] = new GeometryObject();
                Assert.AreEqual(true, ControllerModel.ModelFind(geObjects[i].Id.ToString()));
            }
            Assert.AreEqual(geObjects.Length + 1, ControllerModel.ModelList().Length);


            for (var i = 0; i < count; ++i)
            {
                ControllerModel.ModelSetCurrent(geObjects[i].Id.ToString());
                geObjects[i].Dispose();
            }

            var list = ControllerModel.ModelList();
            Assert.AreEqual(true, list.Length == 1 && list[0] == string.Empty);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 1 })]
        public void RemoveEntityTest(string fileName, int[] dimArray)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                for (var i = 0; i < dimArray.Length; ++i)
                {
                    var entities = ControllerGeometry.GetEntitiesTags(-1);
                    var lengthBefore = entities.Length;
                    ControllerModel.ModelSetCurrent(geometry.Id.ToString());
                    geometry.DeleteEntities(new int[] { dimArray[i], entities[1] });//Delete first id
                    var newEntities = ControllerGeometry.GetEntitiesTags(-1);
                    var lengthAfter = newEntities.Length;
                    Assert.IsTrue(lengthBefore > lengthAfter);
                }
            }
        }
    }
}
