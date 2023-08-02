using GmshApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModel.GmshTests
{
    [TestClass]
    public class MeshQualityTests
    {
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
        [DataRow(0, 20.9)]
        [DataRow(1, 54.2)]
        [DataRow(2, 150)]
        public void GetMinJacobianValues_ShouldReturnCorrectValues(int position, double expectedValue)
        {
            var path = "../../Examples/GmshConnector/Cilindr.step";
            if (!File.Exists(path))
                throw new FileNotFoundException();
            using (var geometry = new GeometryObject(path))
            {
                var mesh = geometry.Generate(3);
                var minJacobianValues = mesh.QualityAnalyse().GetMinJacobianValues();
                Assert.AreEqual(expectedValue, minJacobianValues[position]);
            }
        }
    }
}
