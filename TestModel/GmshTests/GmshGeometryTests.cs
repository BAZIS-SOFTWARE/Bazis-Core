using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GmshWrapper;
using Model.GmshObject;

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
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements1DTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            { 
                var lines = geometry.GetElements1D(); 
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetControlPointsTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var points = geometry.GetControlPoints();
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
