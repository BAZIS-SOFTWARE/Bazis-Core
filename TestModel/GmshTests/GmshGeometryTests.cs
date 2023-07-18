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
    }
}
