using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using GmshApi.GmshController;
using System;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerModelTests
    {
        private GmshController controller = new GmshController(@"..\..\..\..\GmshApi\gmsh.dll");
        private static string modelFolder = @"..\..\..\Examples\GMSHAPI\";

        [TestInitialize]
        public void Init()
        {
            /*var argv = string.Empty;
            var ierr = 0;
            controller.gmshInitialize(0, ref argv, 0, 0, ref ierr);*/
        }
        [TestCleanup]
        public void Cleanup()
        {
            var ierr = 0;
            //controller.gmshFinalize(ref ierr);
            controller.gmshClear(ref ierr);
        }
        [DataTestMethod]
        [DataRow(new string[] { "" })]
        public void TestEmptyModelList(string[] expected)
        {
            string[] list;
            controller.ModelList(out list);
            CollectionAssert.AreEqual(expected, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "", "1" }, "1")]
        public void AddSingleModelInListAndCheck(string[] expected, string added)
        {
            var ierr = 0;
            string[] list;
            controller.gmshModelAdd(added, ref ierr);
            controller.ModelList(out list);
            CollectionAssert.AreEqual(expected, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "", "1", "2", "1" }, "1", "2", "1")]//Возможны не уникальные значения
        [DataRow(new string[] { "", "ABCD", "cde", "1" }, "ABCD", "cde", "1")]
        [DataRow(new string[] { "", "", "", "" }, "", "", "")]
        public void AddSeveralModelsInListAndCheck(string[] expected, string add1, string add2, string add3)
        {
            var ierr = 0;
            string[] list;
            controller.gmshModelAdd(add1, ref ierr);
            controller.gmshModelAdd(add2, ref ierr);
            controller.gmshModelAdd(add3, ref ierr);
            controller.ModelList(out list);
            CollectionAssert.AreEqual(expected, list);
        }
        [TestMethod]
        public void RemoveInEmptyList()
        {
            var ierr = 0;
            string[] list;
            controller.gmshModelRemove(ref ierr);
            controller.ModelList(out list);
            CollectionAssert.AreEqual(new[] { "" }, list);
        }
        [DataTestMethod]
        [DataRow("1")]
        [DataRow("2")]
        [DataRow("3")]
        public void AddRemoveTest(string add)
        {
            var ierr = 0;
            string[] list;
            controller.gmshModelAdd(add, ref ierr);
            controller.gmshModelRemove(ref ierr);
            controller.ModelList(out list);
            CollectionAssert.AreEqual(new[] { "" }, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "", "2", "8" }, "2", "8", "15", "16")]
        public void AddRemoveTwoTest(string[] expected, string add1, string add2, string add3, string add4)
        {
            var ierr = 0;
            string[] list;
            controller.gmshModelAdd(add1, ref ierr);
            controller.gmshModelAdd(add2, ref ierr);
            controller.gmshModelAdd(add3, ref ierr);
            controller.gmshModelAdd(add4, ref ierr);
            controller.gmshModelRemove(ref ierr);
            controller.gmshModelRemove(ref ierr);
            controller.ModelList(out list);
            CollectionAssert.AreEqual(expected, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "1", "7", "15" }, "0", "15")]
        [DataRow(new string[] { "", "", "6" }, "100", "6")]
        [DataRow(new string[] { "1", "7", "15" }, "9", "")]
        [DataRow(new string[] { "2", "", "" }, "1", "2")]
        public void ModelSetCurrentFalseTrueTest(string[] add, string unsuccess, string success)
        {
            var ierr = 0;
            string[] list;
            for (var i = 0; i < add.Length; ++i)
                controller.gmshModelAdd(add[i], ref ierr);
            controller.gmshModelSetCurrent(unsuccess, ref ierr);
            Assert.AreEqual(1, ierr);
            controller.gmshModelSetCurrent(success, ref ierr);
            Assert.AreEqual(0, ierr);
        }
        [DataTestMethod]
        [DataRow(new string[] { "1", "7", "15" }, "0", "15")]
        [DataRow(new string[] { "", "", "6" }, "", "")]
        [DataRow(new string[] { "1", "7", "15" }, "7", "7")]
        [DataRow(new string[] { "2", "", "" }, "10", "")]
        [DataRow(new string[] { "10", "20", "30" }, "21", "30")]
        public void ModelSetCurrentGetCurrentTest(string[] add, string set, string get)
        {
            var ierr = 0;
            string current;
            for (var i = 0; i < add.Length; ++i)
                controller.gmshModelAdd(add[i], ref ierr);
            controller.gmshModelSetCurrent(set, ref ierr);
            controller.ModelGetCurrent(out current);
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
            var ierr = 0;
            string[] list;
            for (var i = 0; i < add.Length; ++i)
                controller.gmshModelAdd(add[i], ref ierr);
            controller.ModelList(out list);
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
            var ierr = 0;
            string[] list;
            controller.gmshModelAdd(add1, ref ierr);
            controller.gmshModelAdd(add2, ref ierr);
            controller.gmshModelAdd(add3, ref ierr);
            controller.gmshModelSetCurrent(rem, ref ierr);
            var actual = !Convert.ToBoolean(ierr);
            Assert.AreEqual(status, actual);
            if(ierr == 0)
                controller.gmshModelRemove(ref ierr);
            controller.ModelList(out list);
            CollectionAssert.AreEqual(expected, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "1", "2", "3" }, "")]
        [DataRow(new string[] { "" }, "")]
        public void ModelClearTest(string[] add, string expected)
        {
            var ierr = 0;
            string[] list;
            for (var i = 0; i < add.Length; ++i)
                controller.gmshModelAdd(add[i], ref ierr);
            controller.gmshClear(ref ierr);
            controller.ModelList(out list);
            CollectionAssert.AreEqual(new string[] { expected }, list);
        }
        [DataTestMethod]
        [DataRow(new string[] { "1.geo", "2.geo", "3.geo" }, new string[] { "1", "2", "3" })]
        public void ModelSetFileNameGetFileNameTest(string[] expected, string[] add)
        {
            var ierr = 0;
            string name = string.Empty;
            for (var i = 0; i < add.Length; ++i)
            {
                controller.gmshModelAdd(add[i], ref ierr);
                controller.gmshModelSetFileName(add[i], ref ierr);
                controller.ModelGetFileName(out name);
                Assert.AreEqual(add[i], name);
            }
        }
        [DataTestMethod]
        [Timeout(60000)]
        [DataRow(10000)]
        public void ModelTest(int count)
        {
            var ierr = 0;
            controller.gmshClear(ref ierr);
            for (var i = 0; i < count; ++i)
                controller.gmshModelAdd(i.ToString(), ref ierr);
            string[] list;
            for (var i = 0; i < count; ++i)
            {
                controller.gmshModelSetCurrent(i.ToString(), ref ierr);
                Assert.AreEqual(0, ierr);
                controller.gmshModelRemove(ref ierr);
                Assert.AreEqual(0, ierr);
            }
            controller.ModelList(out list);
            Assert.AreEqual(true, list.Length == 1 && list[0] == string.Empty);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        [DataRow("Korpus_Nasosa.step", "2")]
        [DataRow("model.igs", "3")]
        [DataRow("sphere.geo", "4")]
        public void ModelTestLoad(string fileName, string modelName)
        {
            var ierr = 0;
            ModelLoad(fileName, modelName, controller);
        }

        [DataTestMethod]
        [DataRow("test")]
        public void LoadModelAndCheckLog(string name)
        {
            var ierr = 0;
            controller.gmshModelSetCurrent(name, ref ierr);
            string log = string.Empty;
            controller.LoggerGetLastError(out log);
            Assert.AreEqual(false, log.Length == 0);
        }


        public static void ModelLoad(string fileName, string modelName, GmshController controller)
        {
            var ierr = 0;
            string[] list;
            string fName;
            controller.gmshModelAdd(modelName, ref ierr);
            controller.ModelList(out list);
            CollectionAssert.AreEqual(new string[] { "", modelName }, list);
            controller.gmshMerge(modelFolder + fileName, ref ierr);
            controller.gmshModelSetFileName(fileName, ref ierr);
            controller.ModelGetFileName(out fName);
            Assert.AreEqual(fileName, fName);
        }
    }
}
