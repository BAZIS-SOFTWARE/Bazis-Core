using GmshApi;
using GmshApi.GmshController;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace TestModel.GmshTests
{
    [TestClass]
    public class MeshQualityTests
    {
        private GmshController controller = new GmshController("C:\\Windows\\System32\\gmsh.dll");
        private static string modelFolder = @"..\..\..\Examples\GMSHConnector\";

        [TestInitialize]
        public void Init()
        {
            var argv = string.Empty;
            var ierr = 0;
            controller.gmshInitialize(0, ref argv, 0, 0, ref ierr);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var ierr = 0;
            controller.gmshFinalize(ref ierr);
        }

        [TestMethod]
        public void GetInvertedGradientError_ShouldReturnCorrectAverageValue()
        {
            /*using (var geometry = new GeometryObject(path + "Cilindr.step"))
            {
                ControllerIO.SetCurrentModelFileName("Cilindr.step");
                var mesh = geometry.Generate(3);
                var values = mesh.QualityAnalyse().GetInvertedGradientError();
                Assert.AreEqual(0.83, values.Average(), 0.1);
            }*/
        }

        [TestMethod]
        public void GetInvertedConditionNumber_ShouldReturnCorrectAverageValue()
        {
            /*using (var geometry = new GeometryObject(path + "Cilindr.step"))
            {
                ControllerIO.SetCurrentModelFileName("Cilindr.step");
                var mesh = geometry.Generate(3);
                var values = mesh.QualityAnalyse().GetInvertedConditionNumber();
                Assert.AreEqual(0.803, values.Average(), 0.1);
            }*/
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