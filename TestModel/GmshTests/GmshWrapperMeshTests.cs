using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;
using System.Linq;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GmshWrapperMeshTests
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
        //[DataTestMethod]
        //[DataRow("Cilindr.step", "1")]
        //[DataRow("Korpus_Nasosa.step", "2")]
        //[DataRow("model.igs", "3")]
        //[DataRow("sphere.geo", "4")]
        //public void GenerateMeshTest(string fileName, string modelName)
        //{
        //    GmshWrapperModelTests.ModelLoad(fileName, modelName);
        //    GmshWrapperMesh.Generate(2);
        //}
        //[DataTestMethod]
        //[DataRow("Cilindr.step", "1")]
        //[DataRow("Korpus_Nasosa.step", "2")]
        //[DataRow("model.igs", "3")]
        //[DataRow("sphere.geo", "4")]
        //public void GenerateMeshGetElementsInternal(string fileName, string modelName)
        //{
        //    GmshWrapperModelTests.ModelLoad(fileName, modelName);
        //    GmshWrapperMesh.Generate(2);
        //    ElementInfo[] info;
        //    long[][] elemsTags, nodeTags;
        //    GmshWrapperMesh.GetElementTags(-1, -1, out info, out elemsTags, out nodeTags);
        //    GmshWrapperMesh.GetElementTags(1, -1, out info, out elemsTags, out nodeTags);
        //    GmshWrapperMesh.GetElementTags(2, -1, out info, out elemsTags, out nodeTags);
        //}
        //[DataTestMethod]
        //[DataRow("Cilindr.step", "1")]
        //[DataRow("Korpus_Nasosa.step", "2")]
        //[DataRow("model.igs", "3")]
        //[DataRow("sphere.geo", "4")]
        //public void GenerateMeshGetElementsTest(string fileName, string modelName)
        //{
        //    GmshWrapperModelTests.ModelLoad(fileName, modelName);
        //    GmshWrapperMesh.Generate(2);
        //    var all = GmshWrapperMesh.GetElements(-1, -1);
        //    var e1d = GmshWrapperMesh.GetElements(1, -1);
        //    var e2d = GmshWrapperMesh.GetElements(2, -1);
        //}
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { 7, 8, 1, 3, 6 }, new int[] { 0, 0, 5, 5, 9 })]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 700, 720, 670, 667, 663 }, new int[] { 0, 0, 5, 2, 9 })]
        [DataRow("model.igs", "3", new int[] { 124, 126, 140, 136, 137 }, new int[] { 1, 1, 0, 5, 6 })]
        [DataRow("sphere.geo", "4", new int[] { 13, 14, 1, 2, 3 }, new int[] { 0, 0, 3, 3, 3 })]

        public void GenerateMeshGet1DElementsByIdTest(string fileName, string modelId, 
                                                        int[] lineTags, int[] expectedLength)
        {
            GmshWrapperModel.ModelAdd(modelId);
            
            CollectionAssert.AreEqual(new string[] { "", modelId }, GmshWrapperModel.ModelList());
            GmshWrapperIO.Load(modelFolder + fileName);

            GmshWrapperIO.SetCurrentModelFileName(fileName);
            Assert.AreEqual(fileName, GmshWrapperIO.GetCurrentModelFileName());

            GmshWrapperMesh.Generate(1);
            GmshWrapperMesh.Generate(2);
            for (var i = 0; i < lineTags.Length; ++i)
            {
                var lines = GmshWrapperMesh.GetElements(1, lineTags[i]).ToArray();
                Assert.AreEqual(expectedLength[i], lines.Length);
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { 1, 2, 4, 5, 6 }, new int[] { 112, 112, 72, 0, 0 })]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 1, 2, 7, 292, 293 }, new int[] { 6, 10, 6, 0, 0 })]
        [DataRow("model.igs", "3", new int[] { 3, 4, 5, 29, 30 }, new int[] { 2, 28, 2, 0, 0 })]
        [DataRow("sphere.geo", "4", new int[] { 1, 2, 3, 14, 20 }, new int[] { 0, 0, 0, 13, 15 })]
        public void GenerateMeshGet2DElementsByIdTest(string fileName, string modelName,
                                                        int[] surfTags, int[] expectedLength)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            for (var i = 0; i < surfTags.Length; ++i)
            {
                var surfs = GmshWrapperMesh.GetElements(2, surfTags[i]).ToArray();
                Assert.AreEqual(expectedLength[i], surfs.Length);
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { -1, 1, 2}, new int[] { 0, 1 , 2 } )]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow("model.igs", "3", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow("sphere.geo", "4", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        public void GenerateMeshGetAllNodesTest(string fileName, string modelName, int[] nodesDim, int[] paramsCount)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            long[] nodes;
            double[] local, parametric;
            for (var i = 0; i < nodesDim.Length; ++i)
            {
                GmshWrapperMesh.GetNodes(nodesDim[i], -1, out nodes, out local, out parametric, true);
                if(nodesDim[i] == -1)
                    Assert.AreEqual(0, parametric.Length);
                else
                {
                    Assert.AreEqual(0, local.Length % 3);
                    var totalPoints = local.Length / 3;
                    Assert.AreNotEqual(0, parametric.Length);
                    Assert.AreEqual(parametric.Length, totalPoints * paramsCount[i]);
                }
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { 1, 2, 3 }, new int[] {6, 10, 6 } )]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 1, 2, 3 }, new int[] { 3, 3, 3 })]
        [DataRow("model.igs", "3", new int[] { 1, 2, 3 }, new int[] { 8, 2, 8 })]
        [DataRow("sphere.geo", "4", new int[] { 1, 2, 3 }, new int[] { 4, 4, 4 })]
        public void GenerateMeshGetLineNodesByTag(string fileName, string modelName, int[] tags, int[] expectedLength)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            long[] nodes;
            double[] local, parametric;
            for (var i = 0; i < tags.Length; ++i)
            {
                GmshWrapperMesh.GetNodes(1, tags[i], out nodes, out local, out parametric);
                Assert.AreEqual(expectedLength[i], nodes.Length);
            }
        }
        [DataTestMethod]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 170, 168, 166 }, new int[] { 4, 8, 8 })]
        [DataRow("model.igs", "3", new int[] { 5, 3, 12 }, new int[] { 4, 4, 9 })]
        public void GenerateMeshGetSurfacesNodesByTag(string fileName, string modelName, int[] tags, int[] expectedLength)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            long[] nodes;
            double[] local, parametric;
            for (var i = 0; i < tags.Length; ++i)
            {
                GmshWrapperMesh.GetNodes(2, tags[i], out nodes, out local, out parametric);
                Assert.AreEqual(expectedLength[i], nodes.Length);
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        public void GenerateMeshGetLocalCoordinatesInElement(string fileName, string modelName)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            var point = new double[] { 3.16450111806, 18.4094425392, 5.84848849947 };
            var temp = GmshWrapperMesh.GetLocalCoordinatesInElement(75, point[0], point[1], point[2]);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", 75)]
        public void GenerateMeshGetElementsByCoordinates(string fileName, string modelName, int expected)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            var point = new double[] { 3.16450111806, 18.4094425392, 5.84848849947 };
            var point2 = new double[] { -6.03049135332, 11.5693911823, 8.52002954716 };
            var actual = GmshWrapperMesh.GetElementsByCoordinates(2, point2[0], point2[1], point2[2]);
            Assert.AreEqual(expected, actual[0]);
        }
    }
}
