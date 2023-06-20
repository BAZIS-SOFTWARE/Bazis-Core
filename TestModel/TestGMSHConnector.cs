using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.ModelParcer;
using System.IO;
using System.Linq;

namespace TestModel
{
    [TestClass]
    public class TestGMSHConnector
    {
        /*private string srcDir;
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
            GMSHConnector.Finalize();
        }

        [TestMethod]
        public void TestCreateGMSHConnector()
        {
            using(var connector = new GMSHConnector("C:\\Users\\kns\\Downloads\\gmsh-stable-Windows64\\gmsh-4.11.1-Windows64\\examples\\Cilindr.stp", "Элементы2D"))
            {
                connector.CreateModelData();
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
            
            GMSHConnector.GMSHCreateFromScript(srcDir + scriptName + scriptExt, objType, false);
            CompareContentOfFiles(dstDir + scriptName + ".inp",
                                  srcDir + expected + ".inp");
        }

        private string[] CuttedString(string path) => File.ReadAllLines(path).Skip(2).ToArray();

        private void CompareContentOfFiles(string actualPath, string expectedPath)
        {
            var expected = CuttedString(expectedPath);
            var actual = CuttedString(actualPath);
            CollectionAssert.AreEqual(expected,actual);
        }*/

    }
}
