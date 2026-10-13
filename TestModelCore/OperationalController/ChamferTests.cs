using OperationalController;
using OperationalController.GmshController;

namespace TestModeCore_test.OperationalController
{
    [TestClass]
    public class ChamferTests
    {
        private GmshController gmshController = null!;

        [TestInitialize]
        public void Initialize()
        {
            gmshController = new GmshController();
            gmshController.Load(GmshLibrary.FilePath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            gmshController.Gmsh.Clear();
        }

        [DataTestMethod]
        [DataRow(0.2, false)]
        [DataRow(30.0, true)]
        public void CreateChamfer_AddsSurfacesToBox(double secondLengthOrAngle, bool isAngle)
        {
            gmshController.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTags = new[] { 12, 3 };
            var surfacesBefore = gmshController.Gmsh.Model.GetEntities(2).Length / 2;
            var projectController = CreateProjectController();

            var result = projectController.CreateChamfer(
                curveTags,
                0.1,
                secondLengthOrAngle,
                isAngle,
                false);
            var surfacesAfter = gmshController.Gmsh.Model.GetEntities(2).Length / 2;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
            Assert.AreEqual(surfacesBefore + curveTags.Length, surfacesAfter);
        }

        private ProjectController CreateProjectController()
        {
            var projectController = new ProjectController();
            projectController.InitializeGeometry(GmshLibrary.FilePath);
            projectController.CreateProject("ChamferTests.bpf2");
            return projectController;
        }
    }
}
