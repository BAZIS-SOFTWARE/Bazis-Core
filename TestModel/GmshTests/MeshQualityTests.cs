using GmshApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace TestModel.GmshTests
{
    [TestClass]
    public class MeshQualityTests
    {
        private string path = @"..\..\..\Examples\GmshConnector\";

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

        [TestMethod]
        public void GetInvertedGradientError_ShouldReturnCorrectAverageValue()
        {
            using (var geometry = new GeometryObject(path + "Cilindr.step"))
            {
                GmshWrapperIO.SetCurrentModelFileName("Cilindr.step");
                var mesh = geometry.Generate(3);
                var values = mesh.QualityAnalyse().GetInvertedGradientError();
                Assert.AreEqual(0.83, values.Average(), 0.1);
            }
        }

        [TestMethod]
        public void GetInvertedConditionNumber_ShouldReturnCorrectAverageValue()
        {
            using (var geometry = new GeometryObject(path + "Cilindr.step"))
            {
                GmshWrapperIO.SetCurrentModelFileName("Cilindr.step");
                var mesh = geometry.Generate(3);
                var values = mesh.QualityAnalyse().GetInvertedConditionNumber();
                Assert.AreEqual(0.803, values.Average(), 0.1);
            }
        }

        //[TestMethod]
        //public void GetMinJacobianValues_ShouldReturnCorrectAverageValue()
        //{
        //    using (var geometry = new GeometryObject(path))
        //    {
        //        var mesh = geometry.Generate(3);
        //        var values = mesh.QualityAnalyse().GetMinJacobianValues();
        //        Assert.AreEqual(54.2, values.Average(), 1e-2);
        //    }
        //}
    }
}