using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using GmshApi;
using OperationalController.GmshController;

namespace TestModeCore_test.GmshApi
{
    [TestClass]
    public class ModelTests
    {
        private IGmshController controller = new GmshController();
        private static string modelFolder = @"..\..\..\Examples\GMSHAPI\";

        [TestInitialize]
        public void Init()
        {
            controller.Load(@"..\..\..\..\GmshApi\gmsh.dll");
        }
        [TestCleanup]
        public void Cleanup()
        {
            controller.Gmsh.Clear();
        }
        [DataTestMethod]
        [DataRow(new string[] { "" })]
        public void TestEmptyModelList(string[] expected)
        {
            var list = controller.Gmsh.Model.List();
            CollectionAssert.AreEqual(expected, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "", "1" }, "1")]
        public void AddSingleModelInListAndCheck(string[] expected, string added)
        {
            controller.Gmsh.Model.Add(added);
            var list = controller.Gmsh.Model.List();
            CollectionAssert.AreEqual(expected, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "", "1", "2", "1" }, "1", "2", "1")]//Возможны не уникальные значения
        [DataRow(new string[] { "", "ABCD", "cde", "1" }, "ABCD", "cde", "1")]
        [DataRow(new string[] { "", "", "", "" }, "", "", "")]
        public void AddSeveralModelsInListAndCheck(string[] expected, string add1, string add2, string add3)
        {
            controller.Gmsh.Model.Add(add1);
            controller.Gmsh.Model.Add(add2);
            controller.Gmsh.Model.Add(add3);
            var list = controller.Gmsh.Model.List();
            CollectionAssert.AreEqual(expected, list);
        }
        [TestMethod]
        public void RemoveInEmptyList()
        {
            controller.Gmsh.Model.Remove();
            var list = controller.Gmsh.Model.List();
            CollectionAssert.AreEqual(new[] { "" }, list);
        }
        [DataTestMethod]
        [DataRow("1")]
        [DataRow("2")]
        [DataRow("3")]
        public void AddRemoveTest(string add)
        {
            controller.Gmsh.Model.Add(add);
            controller.Gmsh.Model.Remove();
            var list = controller.Gmsh.Model.List();
            CollectionAssert.AreEqual(new[] { "" }, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "", "2", "8" }, "2", "8", "15", "16")]
        public void AddRemoveTwoTest(string[] expected, string add1, string add2, string add3, string add4)
        {
            controller.Gmsh.Model.Add(add1);
            controller.Gmsh.Model.Add(add2);
            controller.Gmsh.Model.Add(add3);
            controller.Gmsh.Model.Add(add4);
            controller.Gmsh.Model.Remove();
            controller.Gmsh.Model.Remove();
            var list = controller.Gmsh.Model.List();
            CollectionAssert.AreEqual(expected, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "1", "7", "15" }, "0", "15")]
        [DataRow(new string[] { "", "", "6" }, "100", "6")]
        [DataRow(new string[] { "1", "7", "15" }, "9", "")]
        [DataRow(new string[] { "2", "", "" }, "1", "2")]
        public void ModelSetCurrentFalseTrueTest(string[] add, string unsuccess, string success)
        {
            for (var i = 0; i < add.Length; ++i)
                controller.Gmsh.Model.Add(add[i]);
            controller.Gmsh.Model.SetCurrent(unsuccess);
            var error = controller.Gmsh.Logger.GetLastError();
            Assert.IsFalse(string.IsNullOrEmpty(error));
            controller.Gmsh.Logger.Write("", LoggerLevel.error);
            controller.Gmsh.Model.SetCurrent(success);
            error = controller.Gmsh.Logger.GetLastError();
            Assert.IsTrue(string.IsNullOrEmpty(error));
        }
        [DataTestMethod]
        [DataRow(new string[] { "1", "7", "15" }, "0", "15")]
        [DataRow(new string[] { "", "", "6" }, "", "")]
        [DataRow(new string[] { "1", "7", "15" }, "7", "7")]
        [DataRow(new string[] { "2", "", "" }, "10", "")]
        [DataRow(new string[] { "10", "20", "30" }, "21", "30")]
        public void ModelSetCurrentGetCurrentTest(string[] add, string set, string get)
        {
            for (var i = 0; i < add.Length; ++i)
                controller.Gmsh.Model.Add(add[i]);
            controller.Gmsh.Model.SetCurrent(set);
            var current = controller.Gmsh.Model.GetCurrent();
            Assert.AreEqual(get, current);
        }
        [DataTestMethod]
        [DataRow(new string[] { "", "", "" }, "", true)]
        [DataRow(new string[] { "0", "1", "2", "3" }, "f", false)]
        [DataRow(new string[] { "2", "6", "8", "15" }, "3", false)]
        [DataRow(new string[] { "0", "1", "2", "3" }, "3", true)]
        [DataRow(new string[] { "a-2-3", "1", "8", "3" }, "a-2-3", true)]
        public void ModelFindTest(string[] add, string query, bool status)
        {
            for (var i = 0; i < add.Length; ++i)
                controller.Gmsh.Model.Add(add[i]);
            var list = controller.Gmsh.Model.List();
            Assert.AreEqual(status, list.Where(v => v == query).Any());
        }
        [DataTestMethod]
        [DataRow("", "", "", "", true, new string[] { "", "", "" })]
        [DataRow("0", "1", "7", "", true, new string[] { "0", "1", "7" })]
        [DataRow("1", "2", "3", "4", false, new string[] { "", "1", "2", "3" })]
        [DataRow("0", "7", "9", "5", false, new string[] { "", "0", "7", "9" })]
        [DataRow("1", "2", "6", "1", true, new string[] { "", "2", "6" })]
        public void ModelRemoveOneByNameTest(string add1, string add2, string add3,
                                             string rem, bool status, string[] expected)
        {
            controller.Gmsh.Model.Add(add1);
            controller.Gmsh.Model.Add(add2);
            controller.Gmsh.Model.Add(add3);
            controller.Gmsh.Model.SetCurrent(rem);
            var error = controller.Gmsh.Logger.GetLastError();
            Assert.AreEqual(status, string.IsNullOrEmpty(error));
            if (string.IsNullOrEmpty(error))
                controller.Gmsh.Model.Remove();
            var list = controller.Gmsh.Model.List();
            CollectionAssert.AreEqual(expected, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "1", "2", "3" }, "")]
        [DataRow(new string[] { "" }, "")]
        public void ModelClearTest(string[] add, string expected)
        {
            for (var i = 0; i < add.Length; ++i)
                controller.Gmsh.Model.Add(add[i]);
            controller.Gmsh.Clear();
            var list = controller.Gmsh.Model.List();
            CollectionAssert.AreEqual(new string[] { expected }, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "1.geo", "2.geo", "3.geo" }, new string[] { "1", "2", "3" })]
        public void ModelSetFileNameGetFileNameTest(string[] expected, string[] add)
        {
            for (var i = 0; i < add.Length; ++i)
            {
                controller.Gmsh.Model.Add(add[i]);
                controller.Gmsh.Model.SetFileName(add[i]);
                var name = controller.Gmsh.Model.GetFileName();
                Assert.AreEqual(add[i], name);
            }
        }
        [DataTestMethod]
        [Timeout(60000)]
        [DataRow(10000)]
        public void ModelTest(int count)
        {
            var ierr = 0;
            controller.Gmsh.Clear();
            for (var i = 0; i < count; ++i)
                controller.Gmsh.Model.Add(i.ToString());
            for (var i = 0; i < count; ++i)
            {
                controller.Gmsh.Model.SetCurrent(i.ToString());
                Assert.AreEqual(0, ierr);
                controller.Gmsh.Model.Remove();
                Assert.AreEqual(0, ierr);
            }
            var list = controller.Gmsh.Model.List();
            Assert.AreEqual(true, list.Length == 1 && list[0] == string.Empty);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        [DataRow("Korpus_Nasosa.step", "2")]
        [DataRow("model.igs", "3")]
        [DataRow("sphere.geo", "4")]
        public void ModelTestLoad(string fileName, string modelName)
        {
            controller.Gmsh.Model.Add(modelName);
            var list = controller.Gmsh.Model.List();
            CollectionAssert.AreEqual(new string[] { "", modelName }, list);
            controller.Gmsh.Merge(modelFolder + fileName);
            controller.Gmsh.Model.SetFileName(fileName);
            var fName = controller.Gmsh.Model.GetFileName();
            Assert.AreEqual(fileName, fName);
        }

        [DataTestMethod]
        [DataRow("test")]
        public void LoadModelAndCheckLog(string name)
        {
            controller.Gmsh.Model.SetCurrent(name);
            var log = controller.Gmsh.Logger.GetLastError();
            Assert.AreEqual(false, log.Length == 0);
        }
    }
}
