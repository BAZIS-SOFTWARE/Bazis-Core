using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GmshWrapper;
using System.Linq;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GmshWrapperGeometryTests
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
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { 4, 6, 4, 1 })]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 376, 684, 291, 1 })]
        [DataRow("model.igs", "3", new int[] { 136, 138, 28 })]
        public void GetEntitiesGetAllDimensionTest(string fileName, string modelName, int[] expTags)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            var dim = GmshWrapperGeometry.GetDimension();
            Assert.AreNotEqual(0, dim);
            var tagDims = new int[expTags.Length][];
            for (var i = 0; i < tagDims.GetLength(0); ++i)
                tagDims[i] = Enumerable.Range(1, expTags[i]).ToArray();
            var expectedTags = tagDims.SelectMany(v => v).ToArray();
            var actualTags = GmshWrapperGeometry.GetEntities(-1).Where((v, i) => (i & 1) == 1).ToArray();
            CollectionAssert.AreEqual(expectedTags, actualTags);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { 4, 6, 4, 1 }, 
                  new string[] { "Point", "Curve", "Surface", "Volume"})]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 376, 684, 291, 1 }, 
                  new string[] { "Point", "Curve", "Surface", "Volume" })]
        [DataRow("model.igs", "3", new int[] { 136, 138, 28 }, new string[] { "Point", "Curve", "Surface"})]
        public void SetGetRemoveEntityNameTest(string fileName, string modelName, int[] entCount, string[] entName)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            var ent = GmshWrapperGeometry.GetEntities(-1);
            for(var i = 1; i < ent.Length; i+=2)
            {
                var dim = ent[i - 1];
                var tag = ent[i];
                var expectedName = entName[dim] + tag.ToString();
                GmshWrapperGeometry.SetEntityName(dim, tag, expectedName);
                Assert.AreEqual(expectedName, GmshWrapperGeometry.GetEntityName(dim, tag));
            }
            //Remove Test: 1) Remove exists name 2) Remove not exists name
            RemoveNameTest(ref entName[0], 0, entCount[0] - 2, entCount[0]);
            RemoveNameTest(ref entName[0], 0, entCount[0] + 2, entCount[0]);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new double[] { -14.2188, -2.7094, -13.2231 },
                                      new double[] { 10.6366, 19.599, 10.7593 })]
        [DataRow("Korpus_Nasosa.step", "2", new double[] { -88.2608, -0.679586, -61.2934 },
                                            new double[] { 88.2608, 273.279, 61.2934 })]
        [DataRow("model.igs", "3", new double[] { -1.72724e-05, -179.902, -0.0020751 },
                                   new double[] { 2591.2, 2540, 508 })]
        [DataRow("sphere.geo", "4", new double[] { -1, -1, -1 },
                                    new double[] { 1, 1, 1 })]
        public void ModelBoundingBoxTest(string fileName, string modelName, double[] minExp, double[] maxExp)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            double[] min, max;
            GmshWrapperGeometry.GetBoundingBox(-1, -1, out min, out max);
            for(var i = 0; i < 3; ++i)
                Assert.AreEqual(minExp[i], min[i], 1e-02);
            for (var i = 0; i < 3; ++i)
                Assert.AreEqual(maxExp[i], max[i], 1e-02);
        }

        private void RemoveNameTest(ref string entName, int dim, int delId, int maxId)
        {
            var status = maxId >= delId;
            var delName = entName + delId.ToString();
            GmshWrapperGeometry.RemoveEntityName(delName);
            if (status)
                Assert.AreEqual("",GmshWrapperGeometry.GetEntityName(dim, delId));
        }
    }
}
