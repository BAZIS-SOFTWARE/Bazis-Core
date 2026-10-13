using GmshApi;
using Model.Interfaces;
using OperationalController;

namespace TestProject_test.Geometry
{
    [TestClass]
    public class ProjectControllerGeometryPathTests
    {
        private ProjectController projectController = null!;

        [TestInitialize]
        public void Initialize()
        {
            projectController = new ProjectController();
            projectController.InitializeGeometry(GmshLibrary.FilePath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            projectController.UnloadGeometry();
        }

        [TestMethod]
        public void GetGeometryFilePath_ShouldReturnImportedGeometryPath()
        {
            var filePath = GetGmshGeometryPath("simple.geo");
            projectController.ImportCAD(filePath);

            var actualPath = projectController.GetGeometryFilePath();

            Assert.AreEqual(filePath, actualPath);
        }

        private string GetGmshGeometryPath(string fileName)
        {
            var examplesPath = GetExamplesPath();
            return Path.Combine(examplesPath, "GMSHAPI", fileName);
        }

        private string GetExamplesPath()
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "TestModelCore", "Examples"));
        }
    }
}
