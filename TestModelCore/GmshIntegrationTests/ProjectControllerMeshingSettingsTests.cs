using GmshApi;
using Model.Interfaces;
using OperationalController;

namespace GmshTests
{
    [TestClass]
    public class ProjectControllerMeshingSettingsTests
    {
        private ProjectController projectController = null!;

        [TestInitialize]
        public void Initialize()
        {
            projectController = new ProjectController();
            projectController.InitializeGeometry(GetGmshDllPath());
        }

        [TestCleanup]
        public void Cleanup()
        {
            projectController.UnloadGeometry();
        }

        [TestMethod]
        public void SetCurveMeshingSettings_ShouldReturnSpecifiedBumpSettings()
        {
            ImportGmshGeometry("simple.geo");
            var settings = new CurveMeshingSettings(true, 7, MeshType.Bump, 0.35);

            projectController.SetCurveMeshingSettings(1, settings);
            var actualSettings = projectController.GetCurveMeshingSettings(1);

            Assert.IsTrue(actualSettings.IsTransfinite);
            Assert.AreEqual(settings.NodesCount, actualSettings.NodesCount);
            Assert.AreEqual(settings.MeshType, actualSettings.MeshType);
            Assert.AreEqual(settings.Coefficient, actualSettings.Coefficient, 1e-12);

            projectController.SetCurveMeshingSettings(1, new CurveMeshingSettings(false, 0, MeshType.Progression, 1));
            actualSettings = projectController.GetCurveMeshingSettings(1);

            Assert.IsFalse(actualSettings.IsTransfinite);
        }

        [TestMethod]
        public void SetSurfaceMeshingSettings_ShouldReturnSpecifiedSettings()
        {
            ImportGmshGeometry("simple.geo");
            var initialSettings = projectController.GetSurfaceMeshingSettings(1);
            var settings = new SurfaceMeshingSettings(
                true,
                initialSettings.BoundaryPointNumbers,
                Arrangement.Right,
                true,
                Array.Empty<int>(),
                initialSettings.BoundaryPointNumbers);

            projectController.SetSurfaceMeshingSettings(1, settings);
            var actualSettings = projectController.GetSurfaceMeshingSettings(1);

            Assert.IsTrue(actualSettings.IsTransfinite);
            CollectionAssert.AreEquivalent(settings.CornerPointNumbers.ToArray(), actualSettings.CornerPointNumbers.ToArray());
            Assert.AreEqual(settings.Arrangement, actualSettings.Arrangement);
            Assert.IsTrue(actualSettings.IsRecombined);

            projectController.SetSurfaceMeshingSettings(1, new SurfaceMeshingSettings(
                false,
                Array.Empty<int>(),
                Arrangement.Left,
                false,
                Array.Empty<int>(),
                initialSettings.BoundaryPointNumbers));
            actualSettings = projectController.GetSurfaceMeshingSettings(1);

            Assert.IsFalse(actualSettings.IsTransfinite);
            Assert.IsFalse(actualSettings.IsRecombined);
        }

        [TestMethod]
        public void SetEmbeddedGeometryEntities_ShouldReplaceEmbeddedEntities()
        {
            ImportGmshGeometry("simple.geo");
            var startPointNumber = projectController.CreatePoint(0.5, 0.25, 0);
            var endPointNumber = projectController.CreatePoint(0.5, 0.75, 0);
            var embeddedCurveNumber = projectController.CreateLine(startPointNumber, endPointNumber);
            var initialSettings = projectController.GetSurfaceMeshingSettings(1);

            projectController.SetEmbeddedGeometryEntities(2, 1, 1, [embeddedCurveNumber]);
            var actualSettings = projectController.GetSurfaceMeshingSettings(1);

            CollectionAssert.AreEquivalent(new[] { embeddedCurveNumber }, actualSettings.EmbeddedCurveNumbers.ToArray());

            projectController.SetEmbeddedGeometryEntities(2, 1, 1, Array.Empty<int>());
            actualSettings = projectController.GetSurfaceMeshingSettings(1);

            Assert.AreEqual(0, actualSettings.EmbeddedCurveNumbers.Count);
        }

        [TestMethod]
        public void SetVolumeMeshingSettings_ShouldSupportAllMeshingTypes()
        {
            ImportCadGeometry("cilindr.step");
            var volumeNumber = GetFirstEntityNumber(3);
            var gradientSettings = new GradientVolumeMeshingSettings(1.2, 0.4, 0.1, 0.3);
            var actualSettings = projectController.GetVolumeMeshingSettings(volumeNumber);

            Assert.AreEqual(VolumeMeshingType.Undefined, actualSettings.Type);

            projectController.SetVolumeMeshingSettings(volumeNumber, new VolumeMeshingSettings(VolumeMeshingType.Regular));
            actualSettings = projectController.GetVolumeMeshingSettings(volumeNumber);

            Assert.AreEqual(VolumeMeshingType.Regular, actualSettings.Type);

            projectController.SetVolumeMeshingSettings(
                volumeNumber,
                new VolumeMeshingSettings(VolumeMeshingType.Gradient, gradientSettings));
            actualSettings = projectController.GetVolumeMeshingSettings(volumeNumber);

            Assert.AreEqual(VolumeMeshingType.Gradient, actualSettings.Type);
            Assert.IsNotNull(actualSettings.GradientSettings);
            Assert.AreEqual(gradientSettings.Power, actualSettings.GradientSettings.Power, 1e-12);
            Assert.AreEqual(gradientSettings.DistanceMaximum, actualSettings.GradientSettings.DistanceMaximum, 1e-12);
            Assert.AreEqual(gradientSettings.SurfaceElementSize, actualSettings.GradientSettings.SurfaceElementSize, 1e-12);
            Assert.AreEqual(gradientSettings.CoreElementSize, actualSettings.GradientSettings.CoreElementSize, 1e-12);

            projectController.SetVolumeMeshingSettings(volumeNumber, new VolumeMeshingSettings(VolumeMeshingType.Undefined));
            actualSettings = projectController.GetVolumeMeshingSettings(volumeNumber);

            Assert.AreEqual(VolumeMeshingType.Undefined, actualSettings.Type);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void GetGeometryMeshNodes_ShouldReturnDistinctNodesForCadGeometry(int dimension)
        {
            ImportCadGeometry("cilindr.step");
            var entityNumbers = GetEntityNumbers(dimension).Take(2).ToArray();

            var nodesWithoutBoundary = projectController.GetGeometryMeshNodes(dimension, entityNumbers).ToArray();
            var nodesWithBoundary = projectController.GetGeometryMeshNodes(dimension, entityNumbers, true).ToArray();
            var nodeNumbers = nodesWithBoundary.Select(x => x.Number).ToArray();

            Assert.IsTrue(nodesWithBoundary.Length > 0);
            Assert.AreEqual(nodeNumbers.Length, nodeNumbers.Distinct().Count());
            Assert.IsTrue(nodesWithBoundary.Length >= nodesWithoutBoundary.Length);
        }

        [TestMethod]
        public void SetMeshPointSize_ShouldReturnSpecifiedSize()
        {
            ImportGmshGeometry("simple.geo");
            const double size = 0.125;

            projectController.SetMeshPointSize(1, size);
            var actualSize = projectController.GetMeshPointSize(1);

            Assert.AreEqual(size, actualSize, 1e-12);
        }

        [TestMethod]
        public void GetGeometryFilePath_ShouldReturnImportedGeometryPath()
        {
            var filePath = GetGmshGeometryPath("simple.geo");
            projectController.ImportCAD(filePath);

            var actualPath = projectController.GetGeometryFilePath();

            Assert.AreEqual(filePath, actualPath);
        }

        [TestMethod]
        public void GetCurveMeshingSettings_ShouldThrowWhenGeometryIsNotInitialized()
        {
            projectController.UnloadGeometry();

            Assert.ThrowsException<InvalidOperationException>(() => projectController.GetCurveMeshingSettings(1));
        }

        [TestMethod]
        public void SetEmbeddedGeometryEntities_ShouldThrowForInvalidDimension()
        {
            ImportGmshGeometry("simple.geo");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                projectController.SetEmbeddedGeometryEntities(4, 1, 1, Array.Empty<int>()));
        }

        [TestMethod]
        public void SetVolumeMeshingSettings_ShouldThrowWhenGradientSettingsAreNotSpecified()
        {
            ImportCadGeometry("cilindr.step");
            var volumeNumber = GetFirstEntityNumber(3);

            Assert.ThrowsException<ArgumentException>(() =>
                projectController.SetVolumeMeshingSettings(volumeNumber, new VolumeMeshingSettings(VolumeMeshingType.Gradient)));
        }

        private void ImportGmshGeometry(string fileName)
        {
            var filePath = GetGmshGeometryPath(fileName);
            projectController.ImportCAD(filePath);
        }

        private void ImportCadGeometry(string fileName)
        {
            var examplesPath = GetExamplesPath();
            var filePath = Path.Combine(examplesPath, "CAD", fileName);
            projectController.ImportCAD(filePath);
        }

        private string GetGmshGeometryPath(string fileName)
        {
            var examplesPath = GetExamplesPath();
            return Path.Combine(examplesPath, "GMSHAPI", fileName);
        }

        private int GetFirstEntityNumber(int dimension)
        {
            return GetEntityNumbers(dimension).First();
        }

        private IEnumerable<int> GetEntityNumbers(int dimension)
        {
            if (dimension == 1)
            {
                foreach (var curve in projectController.GetModelObjects(ObjType.Кривая))
                    yield return curve.Number;

                yield break;
            }

            if (dimension == 2)
            {
                foreach (var surface in projectController.GetModelObjects(ObjType.Поверхность))
                    yield return surface.Number;

                yield break;
            }

            if (dimension == 3)
            {
                foreach (var volume in projectController.GetModelVolumes())
                    yield return volume.Number;

                yield break;
            }

            throw new ArgumentOutOfRangeException(nameof(dimension));
        }

        private string GetExamplesPath()
        {
            return Path.GetFullPath(@"..\..\..\Examples");
        }

        private string GetGmshDllPath()
        {
            return Path.GetFullPath(@"..\..\..\..\GmshApi\gmsh.dll");
        }
    }
}
