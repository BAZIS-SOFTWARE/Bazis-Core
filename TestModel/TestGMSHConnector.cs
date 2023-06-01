using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelParcer;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Interfaces;
using SceneInterface;

namespace TestModel
{
    [TestClass]
    public class TestGMSHConnector
    {
        private string srcDir;
        private string dstDir;
        [TestInitialize]
        public void Initialize()
        {
            srcDir = @"..\..\Examples\GMSHConnector\";
            dstDir = srcDir;
        }

        [TestCleanup]
        public void Cleanup()
        {
            GMSHServer.Finalize();
        }

        [TestMethod]
        public void TestCreateGMSHConnector()
        {
            using(var connector = new GMSHConnector(srcDir + "Cilindr1D.stp"))
            {
            }
        }

        [DataTestMethod]
        //.geo test
        [DataRow("sphere1D", ".geo", "Элементы1D", "expected1D")]
        [DataRow("sphere2D", ".geo", "Элементы2D", "expected2D")]
        [DataRow("sphere3D", ".geo", "Элементы3D", "expected3D")]
        //.igs test
        [DataRow("model1D", ".igs", "Элементы1D", "expectedmodel1D")]
        [DataRow("model2D", ".igs", "Элементы2D", "expectedmodel2D")]
        [DataRow("model3D", ".igs", "Элементы3D", "expectedmodel3D")]
        //.stp test
        [DataRow("Cilindr1D", ".stp", "Элементы1D", "expectedCilindr1D")]
        [DataRow("Cilindr2D", ".stp", "Элементы2D", "expectedCilindr2D")]
        [DataRow("Cilindr3D", ".stp", "Элементы3D", "expectedCilindr3D")]//Не проходит, но парсер строит ModelData
        public void TestScriptGMSHConnector(ref string scriptName, ref string scriptExt,
                                            ref string objType, ref string expected)
        {
            using (var connector = new GMSHConnector(srcDir + scriptName + scriptExt))
            {
                connector.Generate(objType);
                var data = connector.GetModelData();
                CompareContentOfFiles(dstDir + scriptName + ".inp",
                                     srcDir + expected + ".inp");
                connector.DeleteDstFile = true;
            }
        }

        [DataTestMethod]
        [DataRow("sphere1D", ".geo", "Элементы1D", "0.2 0.5")]
        [DataRow("sphere2D", ".geo", "Элементы2D", "0.02 0.025")]
        [DataRow("sphere3D", ".geo", "Элементы3D", "1 5")]
        public void TestDensityGMSHConnector(ref string scriptName, ref string scriptExt, 
                                                 ref string objType, ref string density)
        {
            using (var connector = new GMSHConnector(srcDir + scriptName + scriptExt))
            {
                connector.Generate(objType);
                connector.MeshDensity = density;
                var actual = connector.MeshDensity;
                Assert.AreEqual(density, actual);
                connector.DeleteDstFile = true;
            }
        }

        [DataTestMethod]
        [DataRow("sphere2D", ".geo", "Элементы2D")]
        [DataRow("sphere3D", ".geo", "Элементы3D")]
        public void TestQuadrangulateGMSHConnector(ref string scriptName, ref string scriptExt, ref string objType)
        {
            using (var connector = new GMSHConnector(srcDir + scriptName + scriptExt))
            {
                connector.Generate(objType);
                connector.Quadrangulate();
                connector.DeleteDstFile = true;
            }
        }

        [DataTestMethod]
        [DataRow("sphere2D", ".geo", "Элементы2D")]
        [DataRow("sphere3D", ".geo", "Элементы3D")]
        public void TestTesselateGMSHConnector(ref string scriptName, ref string scriptExt, ref string objType)
        {
            using (var connector = new GMSHConnector(srcDir + scriptName + scriptExt))
            {
                connector.Generate(objType);
                connector.Tesselate();
                connector.DeleteDstFile = true;
            }
        }

        private string[] CuttedString(string path) => File.ReadAllLines(path).Skip(2).ToArray();

        private void CompareContentOfFiles(string actualPath, string expectedPath)
        {
            var expected = CuttedString(expectedPath);
            var actual = CuttedString(actualPath);
            CollectionAssert.AreEqual(expected,actual);
        }

    }
}
