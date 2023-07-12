using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GmshWrapper;

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
        public void GenerateMeshGetElements(string fileName, string modelName)
        {
            GmshWrapperModelTests.ModelLoad(fileName, modelName);
            GmshWrapperMesh.Generate(2);
            ElementInfo[] info;
            int[][] elemsTags, nodeTags;
            GmshWrapperMesh.GetElements(-1, -1, out info, out elemsTags, out nodeTags);
            GmshWrapperMesh.GetElements(1, -1, out info, out elemsTags, out nodeTags);
            GmshWrapperMesh.GetElements(2, -1, out info, out elemsTags, out nodeTags);
        }
    }
}
