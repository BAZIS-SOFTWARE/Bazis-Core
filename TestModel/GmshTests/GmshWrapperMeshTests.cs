using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.GmshWrapper;
using System.Management.Instrumentation;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GmshWrapperMeshTests
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
        [DataRow("Cilindr.step", "1")]
        [DataRow("Korpus_Nasosa.step", "2")]
        [DataRow("model.igs", "3")]
        [DataRow("sphere.geo", "4")]
        public void GenerateMeshTest(string fileName, string modelName)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        [DataRow("Korpus_Nasosa.step", "2")]
        [DataRow("model.igs", "3")]
        [DataRow("sphere.geo", "4")]
        public void GenerateMeshGetElementsInternal(string fileName, string modelName)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            ElementInfo[] info;
            long[][] elemsTags, nodeTags;
            GmshWrapperMesh.GetElements(-1, -1, out info, out elemsTags, out nodeTags);
            GmshWrapperMesh.GetElements(1, -1, out info, out elemsTags, out nodeTags);
            GmshWrapperMesh.GetElements(2, -1, out info, out elemsTags, out nodeTags);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        [DataRow("Korpus_Nasosa.step", "2")]
        [DataRow("model.igs", "3")]
        [DataRow("sphere.geo", "4")]
        public void GenerateMeshGetElementsTest(string fileName, string modelName)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            var all = GmshWrapperMesh.GetElements<ModelObject>(-1, -1);
            var e1d = GmshWrapperMesh.GetElements<ModelObject>(1, -1);
            var e2d = GmshWrapperMesh.GetElements<ModelObject>(2, -1);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        [DataRow("Korpus_Nasosa.step", "2")]
        [DataRow("model.igs", "3")]
        [DataRow("sphere.geo", "4")]
        public void GenerateMeshGetElementsByIdTest(string fileName, string modelName)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            var e11 = GmshWrapperMesh.GetElements<ModelObject>(1, 1);
            var e21 = GmshWrapperMesh.GetElements<ModelObject>(2, 1);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { -1, 1, 2}, new int[] { 0, 1 , 2 } )]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow("model.igs", "3", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow("sphere.geo", "4", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        public void GenerateMeshGetAllNodesPerDimTest(string fileName, string modelName, int[] nodesDim, int[] paramsCount)
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
        [DataRow("Cilindr.step", "1", new int[] { 1, 2 } ,new int[] { 1, 2 })]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 1, 2 }, new int[] { 1, 2 })]
        [DataRow("model.igs", "3", new int[] { 1, 2 }, new int[] { 1, 2 })]
        [DataRow("sphere.geo", "4", new int[] { 1, 2 }, new int[] { 1, 2 })]
        public void GenerateMeshGetNodesByTag(string fileName, string modelName, int[] dim ,int[] tags)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            long[] nodes;
            double[] local, parametric;
            for(var i = 0; i < dim.Length; ++i)
                for (var j = 0; j < tags.Length; ++j)
                {
                    GmshWrapperMesh.GetNodes(dim[i], tags[j], out nodes, out local, out parametric, true);
                }
        }
    }
}
