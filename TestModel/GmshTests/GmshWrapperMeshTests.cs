using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;
using System.Linq;
using GmshApi.GmshController;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerMeshTests
    {
        private static string modelFolder = @"..\..\Examples\GMSHConnector\";
        [TestInitialize]
        public void Init()
        {
            ControllerConnection.Initialize();
        }
        [TestCleanup]
        public void Cleanup()
        {
            ControllerConnection.Finalize();
        }
        //[DataTestMethod]
        //[DataRow("Cilindr.step", "1")]
        //[DataRow("Korpus_Nasosa.step", "2")]
        //[DataRow("model.igs", "3")]
        //[DataRow("sphere.geo", "4")]
        //public void GenerateMeshTest(string fileName, string modelName)
        //{
        //    GmshWrapperModelTests.ModelLoad(fileName, modelName);
        //    ControllerMesh.Generate(2);
        //}
        //[DataTestMethod]
        //[DataRow("Cilindr.step", "1")]
        //[DataRow("Korpus_Nasosa.step", "2")]
        //[DataRow("model.igs", "3")]
        //[DataRow("sphere.geo", "4")]
        //public void GenerateMeshGetElementsInternal(string fileName, string modelName)
        //{
        //    GmshWrapperModelTests.ModelLoad(fileName, modelName);
        //    ControllerMesh.Generate(2);
        //    ElementInfo[] info;
        //    long[][] elemsTags, nodeTags;
        //    ControllerMesh.GetElementTags(-1, -1, out info, out elemsTags, out nodeTags);
        //    ControllerMesh.GetElementTags(1, -1, out info, out elemsTags, out nodeTags);
        //    ControllerMesh.GetElementTags(2, -1, out info, out elemsTags, out nodeTags);
        //}
        //[DataTestMethod]
        //[DataRow("Cilindr.step", "1")]
        //[DataRow("Korpus_Nasosa.step", "2")]
        //[DataRow("model.igs", "3")]
        //[DataRow("sphere.geo", "4")]
        //public void GenerateMeshGetElementsTest(string fileName, string modelName)
        //{
        //    GmshWrapperModelTests.ModelLoad(fileName, modelName);
        //    ControllerMesh.Generate(2);
        //    var all = ControllerMesh.GetElements(-1, -1);
        //    var e1d = ControllerMesh.GetElements(1, -1);
        //    var e2d = ControllerMesh.GetElements(2, -1);
        //}
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { 7, 8, 1, 3, 6 }, new int[] { 0, 0, 5, 5, 9 })]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 700, 720, 670, 667, 663 }, new int[] { 0, 0, 5, 2, 9 })]
        [DataRow("model.igs", "3", new int[] { 124, 126, 140, 136, 137 }, new int[] { 1, 1, 0, 5, 6 })]
        [DataRow("sphere.geo", "4", new int[] { 13, 14, 1, 2, 3 }, new int[] { 0, 0, 3, 3, 3 })]

        public void GenerateMeshGet1DElementsByIdTest(string fileName, string modelId, 
                                                        int[] lineTags, int[] expectedLength)
        {
            ModelLoad(fileName, modelId);

            ControllerMesh.Generate(1);
            ControllerMesh.Generate(2);
            for (var i = 0; i < lineTags.Length; ++i)
            {
                var lines = ControllerMesh.GetElements(1, lineTags[i]).ToArray();
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
            ModelLoad(fileName, modelName);
            ControllerMesh.Generate(2);
            for (var i = 0; i < surfTags.Length; ++i)
            {
                var surfs = ControllerMesh.GetElements(2, surfTags[i]).ToArray();
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
            ModelLoad(fileName, modelName);
            ControllerMesh.Generate(2);
            long[] nodes;
            double[] local, parametric;
            for (var i = 0; i < nodesDim.Length; ++i)
            {
                ControllerMesh.GetNodes(nodesDim[i], -1, out nodes, out local, out parametric, true);
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
            ModelLoad(fileName, modelName);
            ControllerMesh.Generate(2);
            long[] nodes;
            double[] local, parametric;
            for (var i = 0; i < tags.Length; ++i)
            {
                ControllerMesh.GetNodes(1, tags[i], out nodes, out local, out parametric);
                Assert.AreEqual(expectedLength[i], nodes.Length);
            }
        }
        [DataTestMethod]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 170, 168, 166 }, new int[] { 4, 8, 8 })]
        [DataRow("model.igs", "3", new int[] { 5, 3, 12 }, new int[] { 4, 4, 9 })]
        public void GenerateMeshGetSurfacesNodesByTag(string fileName, string modelName, int[] tags, int[] expectedLength)
        {
            ModelLoad(fileName, modelName);
            ControllerMesh.Generate(2);
            long[] nodes;
            double[] local, parametric;
            for (var i = 0; i < tags.Length; ++i)
            {
                ControllerMesh.GetNodes(2, tags[i], out nodes, out local, out parametric);
                Assert.AreEqual(expectedLength[i], nodes.Length);
            }
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        public void GenerateMeshGetLocalCoordinatesInElement(string fileName, string modelName)
        {
            ModelLoad(fileName, modelName);
            ControllerMesh.Generate(2);
            var point = new double[] { 3.16450111806, 18.4094425392, 5.84848849947 };
            var temp = ControllerMesh.GetLocalCoordinatesInElement(75, point[0], point[1], point[2]);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", 75)]
        public void GenerateMeshGetElementsByCoordinates(string fileName, string modelName, int expected)
        {
            ModelLoad(fileName, modelName);
            ControllerMesh.Generate(2);
            //point - известные барицентрические координаты центра элемента 75
            var point = new double[] { -8.74248599355, 1.39424515076, 6.51327932777 };
            var actual = ControllerMesh.GetElementsByCoordinates(2, point[0], point[1], point[2]);
            Assert.AreEqual(expected, actual[0]);
        }


        private void ModelLoad(string fileName, string modelID)
        {
            ControllerModel.ModelAdd(modelID);
            ControllerIO.Load(modelFolder + fileName);
            ControllerIO.SetCurrentModelFileName(fileName);
        }
    }
}
