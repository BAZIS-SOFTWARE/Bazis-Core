using OperationalController;
using OperationalController.GmshController;

namespace GmshTests
{
    [TestClass]
    public class ChamferTests
    {
        private GmshController gmshController = null!;

        [TestInitialize]
        public void Initialize()
        {
            gmshController = new GmshController();
            gmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
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

        [TestMethod]
        public void PreviewChamfer_ReturnsFourLinesPerCurveWithoutChangingGeometry()
        {
            gmshController.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTags = new[] { 12, 3 };
            var entitiesBefore = gmshController.Gmsh.Model.GetEntities();
            var projectController = CreateProjectController();

            var previews = projectController.PreviewChamfer(curveTags, 0.1, 0.2, false, false);

            Assert.AreEqual(curveTags.Length, previews.Length);
            Assert.IsTrue(previews.All(preview => preview.CreateSegments().Length == 4));
            Assert.IsTrue(previews
                .SelectMany(preview => preview.CreateSegments())
                .All(segment => segment.GetLength() > 0));
            CollectionAssert.AreEqual(entitiesBefore, gmshController.Gmsh.Model.GetEntities());
        }

        private ProjectController CreateProjectController()
        {
            var projectController = new ProjectController
            {
                GmshController = gmshController
            };
            projectController.CreateProject("ChamferTests.bpf2");
            return projectController;
        }
    }
}
