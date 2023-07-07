using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.GmshUser;
using System.IO;
using System.Linq;

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
            GmshUserConnection.Free();
        }

        [TestMethod]
        public void TestCreateGmshController()
        {
            using (var connector = new GmshMeshObject(srcDir + "Cilindr1D.stp"))
            {
            }
        }

        [DataTestMethod]
        //.geo test
        [DataRow("sphere", ".geo", "Элементы1D", "expected1D")]
        [DataRow("sphere", ".geo", "Элементы2D", "expected2D")]
        [DataRow("sphere", ".geo", "Элементы3D", "expected3D")]
        //.igs test
        [DataRow("model", ".igs", "Элементы1D", "expectedmodel1D")]
        [DataRow("model", ".igs", "Элементы2D", "expectedmodel2D")]
        [DataRow("model", ".igs", "Элементы3D", "expectedmodel3D")]
        //.stp test
        [DataRow("Cilindr", ".stp", "Элементы1D", "expectedCilindr1D")]
        [DataRow("Cilindr", ".stp", "Элементы2D", "expectedCilindr2D")]
        [DataRow("Cilindr", ".stp", "Элементы3D", "expectedCilindr3D")]//Не проходит, но парсер строит ModelData
        public void TestScriptGmshController(ref string scriptName, ref string scriptExt,
                                            ref string objType, ref string expected)
        {
            var actual = false;
            using (var connector = new GmshMeshObject(srcDir + scriptName + scriptExt))
            {
                connector.Generate(objType);
                var data = connector.GetModelDataFromParcer();
                var id = connector.Id;
                actual = CompareContentOfFiles(dstDir + scriptName + id.ToString() + ".inp",
                                     srcDir + expected + ".inp");
                connector.DeleteDstFile = true;
            }
            Assert.AreEqual(true, actual);
        }

        [DataTestMethod]
        [DataRow("sphere1D", ".geo", "Элементы1D", "0.2 0.5")]
        [DataRow("sphere2D", ".geo", "Элементы2D", "0.02 0.025")]
        [DataRow("sphere3D", ".geo", "Элементы3D", "1 5")]
        public void TestDencityGmshController(ref string scriptName, ref string scriptExt,
                                                 ref string objType, ref string dencity)
        {
            string actual = string.Empty;
            using (var connector = new GmshMeshObject(srcDir + scriptName + scriptExt))
            {
                connector.Generate(objType);
                connector.Dencity = dencity;
                actual = connector.Dencity;
                connector.DeleteDstFile = true;
            }
            Assert.AreEqual(dencity, actual);
        }


        [DataTestMethod]
        [DataRow("sphere2D", ".geo", "Элементы2D")]
        [DataRow("sphere3D", ".geo", "Элементы3D")]
        public void TestQuadrangulateGmshController(ref string scriptName, ref string scriptExt, ref string objType)
        {
            using (var connector = new GmshMeshObject(srcDir + scriptName + scriptExt))
            {
                connector.Generate(objType);
                connector.QuadrangulateMesh();
                connector.DeleteDstFile = true;
            }
        }

        [DataTestMethod]
        [DataRow("sphere2D", ".geo", "Элементы2D")]
        [DataRow("sphere3D", ".geo", "Элементы3D")]
        public void TestRefineGmshController(ref string scriptName, ref string scriptExt, ref string objType)
        {
            using (var connector = new GmshMeshObject(srcDir + scriptName + scriptExt))
            {
                connector.Generate(objType);
                connector.RefineMesh();
                connector.DeleteDstFile = true;
            }
        }

        [DataTestMethod]
        [DataRow("sphere", ".geo", "Элементы3D")]
        public void TestMeshGetElementsGmshController(ref string scriptName, ref string scriptExt, ref string objType)
        {
            using (var connector = new GmshMeshObject(srcDir + scriptName + scriptExt))
            {
                connector.Generate("Элементы2D");
                var nodes = connector.GetNodes();
                var elements1D = connector.GetElement1D();
                var elements2D = connector.GetElement2D();
                var elements3D = connector.GetElement3D();
                connector.DeleteDstFile = true;
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr", ".step")]
        public void TestGeometryGetElementsGmshController(ref string scriptName, ref string scriptExt)
        {
            using (var connector = new GmshGeometryObject(srcDir + scriptName + scriptExt))
            {
                connector.Generate("Элементы2D");
                var nodes = connector.GetNodes();
                var curves = connector.GetCurves();
                var elements1D = connector.GetElement1D();
                var surfaces = connector.GetSurfaces();
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr", ".step", "model", ".igs")]
        public void TestGmshListModel(ref string firstName, ref string firstExt,
                                      ref string secondName, ref string secondExt)
        {
            var firstModel = new GmshGeometryObject(srcDir + firstName + firstExt);//Name is "1"
            var actual = GmshModelObjectData.ModelList();
            CollectionAssert.AreEqual(new string[] { "", "1" }, actual);
            var secondModel = new GmshMeshObject(srcDir + secondName + secondExt);//Name is "2"
            var actual2 = GmshModelObjectData.ModelList();
            CollectionAssert.AreEqual(new string[] { "", "1", "2" }, actual2);
            firstModel.Dispose();
            secondModel.Dispose();
            var actual3 = GmshModelObjectData.ModelList();
            CollectionAssert.AreEqual(new string[] {""}, actual3);
        }

        [DataTestMethod]
        [DataRow("test", ".geo")]
        public void TestSpline(ref string scriptName, ref string scriptExt)
        {
            using (var connector = new GmshGeometryObject(srcDir + scriptName + scriptExt))
            {
                var curves = connector.GetCurves();
                var curve = curves.First();
                var nodes = curve.GetPoints()
                    .Select((point, index) => new Node(index++, point))
                    .ToArray();
                var x = new Curve(1, nodes);
            }
        }

        private string[] CuttedString(string path) => File.ReadAllLines(path).Skip(2).ToArray();

        private bool CompareContentOfFiles(string actualPath, string expectedPath)
        {
            var expected = CuttedString(expectedPath);
            var actual = CuttedString(actualPath);
            return Enumerable.SequenceEqual(expected, actual);
        }
    }
}
