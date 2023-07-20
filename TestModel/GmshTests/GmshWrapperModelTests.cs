using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi.Wrapper;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GmshWrapperModelTests
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
        [DataRow(new string[] { "" })]
        public void TestEmptyModelList(string[] expected)
        {
            CollectionAssert.AreEqual(expected, GmshWrapperModel.ModelList());
        }
        [DataTestMethod]
        [DataRow(new string[] { "", "1" }, "1")]
        public void AddSingleModelInListAndCheck(string[] expected, string added)
        {
            GmshWrapperModel.ModelAdd(added);
            CollectionAssert.AreEqual(expected, GmshWrapperModel.ModelList());
        }
        [DataTestMethod]
        [DataRow(new string[] { "", "1", "2", "1" }, "1", "2", "1")]//Возможны не уникальные значения
        [DataRow(new string[] { "", "ABCD", "cde", "1" }, "ABCD", "cde", "1")]
        [DataRow(new string[] { "", "", "", "" }, "", "", "")]
        public void AddSeveralModelsInListAndCheck(string[] expected, string add1, string add2, string add3)
        {
            GmshWrapperModel.ModelAdd(add1);
            GmshWrapperModel.ModelAdd(add2);
            GmshWrapperModel.ModelAdd(add3);
            CollectionAssert.AreEqual(expected, GmshWrapperModel.ModelList());
        }
        [TestMethod]
        public void RemoveInEmptyList()
        {
            GmshWrapperModel.ModelRemove();
            CollectionAssert.AreEqual(new[] { "" }, GmshWrapperModel.ModelList());
        }
        [DataTestMethod]
        [DataRow("1")]
        [DataRow("2")]
        [DataRow("3")]
        public void AddRemoveTest(string add)
        {
            GmshWrapperModel.ModelAdd(add);
            GmshWrapperModel.ModelRemove();
            CollectionAssert.AreEqual(new[] { "" }, GmshWrapperModel.ModelList());
        }
        [DataTestMethod]
        [DataRow(new string[] { "", "2", "8" }, "2", "8", "15", "16")]
        public void AddRemoveTwoTest(string[] expected, string add1, string add2, string add3, string add4)
        {
            GmshWrapperModel.ModelAdd(add1);
            GmshWrapperModel.ModelAdd(add2);
            GmshWrapperModel.ModelAdd(add3);
            GmshWrapperModel.ModelAdd(add4);
            GmshWrapperModel.ModelRemove();
            GmshWrapperModel.ModelRemove();
            CollectionAssert.AreEqual(expected, GmshWrapperModel.ModelList());
        }
        [DataTestMethod]
        [DataRow(new string[] { "1", "7", "15" }, "0", "15")]
        [DataRow(new string[] { "", "", "6" }, "100", "6")]
        [DataRow(new string[] { "1", "7", "15" }, "9", "")]
        [DataRow(new string[] { "2", "", "" }, "1", "2")]
        public void ModelSetCurrentFalseTrueTest(string[] add, string unsuccess, string success)
        {
            for (var i = 0; i < add.Length; ++i)
                GmshWrapperModel.ModelAdd(add[i]);
            Assert.AreEqual(false, GmshWrapperModel.ModelSetCurrent(unsuccess));
            Assert.AreEqual(true, GmshWrapperModel.ModelSetCurrent(success));
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
                GmshWrapperModel.ModelAdd(add[i]);
            GmshWrapperModel.ModelSetCurrent(set);
            Assert.AreEqual(get, GmshWrapperModel.ModelGetCurrent());
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
                GmshWrapperModel.ModelAdd(add[i]);
            Assert.AreEqual(status, GmshWrapperModel.ModelFind(query));
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
            GmshWrapperModel.ModelAdd(add1);
            GmshWrapperModel.ModelAdd(add2);
            GmshWrapperModel.ModelAdd(add3);
            Assert.AreEqual(status, GmshWrapperModel.ModelRemoveByName(rem));
            CollectionAssert.AreEqual(expected, GmshWrapperModel.ModelList());
        }
        [DataTestMethod]
        [DataRow(new string[] { "1", "2", "3" }, "")]
        [DataRow(new string[] { "" }, "")]
        public void ModelClearTest(string[] add, string expected)
        {
            for (var i = 0; i < add.Length; ++i)
                GmshWrapperModel.ModelAdd(add[i]);
            GmshWrapperModel.ModelClear();
            CollectionAssert.AreEqual(new string[] { expected }, GmshWrapperModel.ModelList());
        }
        [DataTestMethod]
        [DataRow(new string[] { "1.geo", "2.geo", "3.geo" }, new string[] { "1", "2", "3" })]
        public void ModelSetFileNameGetFileNameTest(string[] expected, string[] add)
        {
            for (var i = 0; i < add.Length; ++i)
            {
                GmshWrapperModel.ModelAdd(add[i]);
                GmshWrapperIO.ModelSetFileName(add[i]);
                Assert.AreEqual(add[i], GmshWrapperIO.ModelGetFileName());
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        [DataRow("Korpus_Nasosa.step", "2")]
        [DataRow("model.igs", "3")]
        [DataRow("sphere.geo", "4")]
        public void ModelTestLoad(string fileName, string modelName)
        {
            ModelLoad(fileName, modelName);
        }
        public static void ModelLoad(string fileName, string modelName)
        {
            GmshWrapperModel.ModelAdd(modelName);
            CollectionAssert.AreEqual(new string[] { "", modelName }, GmshWrapperModel.ModelList());
            GmshWrapperIO.Merge(modelFolder + fileName);
            GmshWrapperIO.ModelSetFileName(fileName);
            Assert.AreEqual(fileName, GmshWrapperIO.ModelGetFileName());
        }
    }
}
