using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

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

        [DataTestMethod]
        [DataRow("Cilindr.step", "1", 1, new int[] { }, new int[] { 1, 2, 6 } )]
        [DataRow("Cilindr.step", "1", 2, new int[] { }, new int[] { 1, 4, 5 })]
        [DataRow("Cilindr.step", "1", 3, new int[] { }, new int[] { 2, 3, 6 })]
        [DataRow("Cilindr.step", "1", 4, new int[] { }, new int[] { 3, 4, 5 })]
        [DataRow("sphere.geo", "2",1, new int[] { }, new int[] { })]
        [DataRow("sphere.geo", "2", 2, new int[] { }, new int[] { 1, 4, 9, 12 })]
        [DataRow("sphere.geo", "2", 3, new int[] { }, new int[] { 1, 2, 5, 8 })]
        [DataRow("sphere.geo", "2", 4, new int[] { }, new int[] { 2, 3, 10, 11 })]
        [DataRow("sphere.geo", "2", 5, new int[] { }, new int[] { 3, 4, 6, 7 })]
        [DataRow("sphere.geo", "2", 6, new int[] { }, new int[] { 5, 6, 11, 12 })]
        [DataRow("sphere.geo", "2", 7, new int[] { }, new int[] {7, 8, 9, 10 })]
        public void GetAdjacenciesControlPointTest(string fileName, string modelName, int tag ,int[] downExp, int[] upExp)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            var adj = GmshWrapperGeometry.GetAdjacencies(0, tag);
            CollectionAssert.AreEqual(downExp, adj[0]);
            CollectionAssert.AreEqual(upExp, adj[1]);
        }
        [DataTestMethod]
        [DataRow("sphere.geo", "1", 1, new int[] { 2, 3 }, new int[] { 18, 22 })]
        [DataRow("sphere.geo", "1", 2, new int[] { 3, 4 }, new int[] { 14, 20 })]
        [DataRow("sphere.geo", "1", 3, new int[] { 4, 5 }, new int[] { 16, 24 })]
        [DataRow("sphere.geo", "1", 4, new int[] { 5, 2 }, new int[] { 26, 28 })]
        [DataRow("sphere.geo", "1", 5, new int[] { 3, 6 }, new int[] { 20, 22 })]
        [DataRow("sphere.geo", "1", 6, new int[] { 6, 5 }, new int[] { 24, 28 })]
        [DataRow("sphere.geo", "1", 7, new int[] { 5, 7 }, new int[] { 16, 26 })]
        [DataRow("sphere.geo", "1", 8, new int[] { 7, 3 }, new int[] { 14, 18 })]
        [DataRow("sphere.geo", "1", 9, new int[] { 2, 7 }, new int[] { 18, 26 })]
        [DataRow("sphere.geo", "1", 10, new int[] { 7, 4 }, new int[] { 14, 16 })]
        [DataRow("sphere.geo", "1", 11, new int[] { 4, 6 }, new int[] { 20, 24 })]
        [DataRow("sphere.geo", "1", 12, new int[] { 6, 2 }, new int[] { 22, 28 })]
        public void GetAdjacenciesCurvesTest(string fileName, string modelName, int tag, int[] downExp, int[] upExp)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            var adj = GmshWrapperGeometry.GetAdjacencies(1, tag);
            CollectionAssert.AreEqual(downExp, adj[0]);
            CollectionAssert.AreEqual(upExp, adj[1]);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1", 1, new int[] { 1, 2, 3, 4 }, new int[] { 1 })]
        [DataRow("Cilindr.step", "1", 2, new int[] { 1, 5, 3, 6 }, new int[] { 1 })]
        [DataRow("Cilindr.step", "1", 3, new int[] { 4, 5 }, new int[] { 1 })]
        [DataRow("Cilindr.step", "1", 4, new int[] { 6, 2 }, new int[] { 1 })]
        public void GetAdjacenciesSurfacesTest(string fileName, string modelName, int tag, int[] downExp, int[] upExp)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            var adj = GmshWrapperGeometry.GetAdjacencies(2, tag);
            CollectionAssert.AreEqual(downExp, adj[0]);
            CollectionAssert.AreEqual(upExp, adj[1]);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1", 1, new int[] { 1, 2, 3, 4 }, new int[] { })]
        [DataRow("sphere.geo", "2", 30, new int[] { 28, 26, 16, 14, 20, 24, 22, 18 }, new int[] {})]
        [DataRow("sphere.geo", "2", 1, null, null)]//Not Found Test
        public void GetAdjacenciesVolumeTest(string fileName, string modelName, int tag, int[] downExp, int[] upExp)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            var adj = GmshWrapperGeometry.GetAdjacencies(3, tag);
            CollectionAssert.AreEqual(downExp, adj[0]);
            CollectionAssert.AreEqual(upExp, adj[1]);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1", 1, new double[] { }, new double[] { 7.04956865241, 19.1445358209, -2.46386184958 })]
        [DataRow("Cilindr.step", "1", 2, new double[] { }, new double[] { 10.6317844476, 2.25496193279, 0 })]
        [DataRow("Cilindr.step", "1", 3, new double[] { }, new double[] { -14.2140002427, 14.6346119553, -2.46386184958 })]
        [DataRow("Cilindr.step", "1", 4, new double[] { }, new double[] { -10.6317844476, -2.25496193279, 2.33468147911e-16 })]
        [DataRow("Cilindr.step", "1", 5, new double[] { }, new double[] { })]//Not found test

        public void GetLocalCoordsByParametricTest(string fileName, string modelName, int tag,
                                                   double[] parametric, double[] expected)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            var local = GmshWrapperGeometry.GetLocalCoordByParametric(0, tag, parametric);
            for(var i = 0; i < expected.Length; ++i)
                Assert.AreEqual(expected[i], local[i], 1e-08);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { 1, 2, 3, 4, 5, 6 })]
        [DataRow("sphere.geo", "2", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 })]
        public void ParametrizationCurvesTest(string fileName, string modelName, int[] tags)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            for (var i = 0; i < tags.Length; ++i)
            {
                var expected = GmshWrapperGeometry.GetParametrizationBounds(1, tags[i]);
                var numInsideCount = GmshWrapperGeometry.IsInside(1, tags[i], expected[0]);
                Assert.AreNotEqual(0, numInsideCount);
                numInsideCount = GmshWrapperGeometry.IsInside(1, tags[i], expected[1]);
                Assert.AreNotEqual(0, numInsideCount);
                var list = expected[0].Concat(expected[1]).ToArray();
                var local = GmshWrapperGeometry.GetLocalCoordByParametric(1, tags[i], list);
                var parametric = GmshWrapperGeometry.GetParametricCoordByLocal(1, tags[i], local);
                //parametric = GmshWrapperGeometry.SortAndFilterParametric(1, tags[i], parametric);
                //var localPerParametric = GmshWrapperGeometry.GetLocalCoordByParametric(1, tags[i], parametric);
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { 1/*, 2, 3, 4*/})]
        //[DataRow("sphere.geo", "2", new int[] { 14, 16, 18, 20, 22, 24, 26, 28})]
        public void ParametrizationSurfacesTest(string fileName, string modelName, int[] tags)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            for (var i = 0; i < tags.Length; ++i)
            {
                var expected = GmshWrapperGeometry.GetParametrizationBounds(2, tags[i]);
                var numInsideCount = GmshWrapperGeometry.IsInside(2, tags[i], expected[0]);
                Assert.AreNotEqual(0, numInsideCount);
                numInsideCount = GmshWrapperGeometry.IsInside(2, tags[i], expected[1]);
                Assert.AreNotEqual(0, numInsideCount);
                var list = expected[0].Concat(expected[1]).ToArray();
                var local = GmshWrapperGeometry.GetLocalCoordByParametric(2, tags[i], list);
                var parametric = GmshWrapperGeometry.GetParametricCoordByLocal(2, tags[i], local);
                //parametric = GmshWrapperGeometry.SortAndFilterParametric(2, tags[i], parametric);
                //var localPerParametric = GmshWrapperGeometry.GetLocalCoordByParametric(2, tags[i], parametric);
            }
        }



        /*[DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        public void GetParametricCoordsByLocalTest()
        {

        }*/

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
