using OperationalController;
using OperationalController.GeomObjsCreator;
using OperationalController.GmshController;
using Model.Interfaces;
using Geometry;

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

        [TestMethod]
        public void CreateChamfer_AddsSurfaceToBox()
        {
            gmshController.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTag = gmshController.Gmsh.Model.GetEntities(1)[1];
            var surfacesBefore = gmshController.Gmsh.Model.GetEntities(2).Length / 2;
            var projectController = CreateProjectController();

            var result = projectController.CreateChamfer(curveTag, 0.1, 0.2, false, false);
            var surfacesAfter = gmshController.Gmsh.Model.GetEntities(2).Length / 2;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
            Assert.AreEqual(surfacesBefore + 1, surfacesAfter);
            Assert.AreEqual(
                surfacesAfter,
                projectController.GetModelObjects(ObjType.Поверхность).Count());
        }

        [TestMethod]
        public void CreateChamferByAngle_AddsSurfaceToBox()
        {
            gmshController.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTag = gmshController.Gmsh.Model.GetEntities(1)[1];
            var surfacesBefore = gmshController.Gmsh.Model.GetEntities(2).Length / 2;
            var projectController = CreateProjectController();

            var result = projectController.CreateChamfer(curveTag, 0.1, 30, true, false);
            var surfacesAfter = gmshController.Gmsh.Model.GetEntities(2).Length / 2;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
            Assert.AreEqual(surfacesBefore + 1, surfacesAfter);
        }

        [TestMethod]
        public void CreateChamferByAngle_WorksForNonPerpendicularSurfaces()
        {
            gmshController.Gmsh.Model.Occ.AddWedge(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTag = FindCurveBetweenNonPerpendicularSurfaces();
            var surfacesBefore = gmshController.Gmsh.Model.GetEntities(2).Length / 2;
            var projectController = CreateProjectController();

            var result = projectController.CreateChamfer(curveTag, 0.05, 20, true, false);
            var surfacesAfter = gmshController.Gmsh.Model.GetEntities(2).Length / 2;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
            Assert.AreEqual(surfacesBefore + 1, surfacesAfter);
        }

        [TestMethod]
        public void CreateChamfer_MultipleCurvesAddsMultipleSurfaces()
        {
            gmshController.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTags = new List<int> { 12, 3 };
            var surfacesBefore = gmshController.Gmsh.Model.GetEntities(2).Length / 2;
            var projectController = CreateProjectController();

            var result = projectController.CreateChamfer(curveTags, 0.1, 0.2, false, false);
            var surfacesAfter = gmshController.Gmsh.Model.GetEntities(2).Length / 2;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
            Assert.AreEqual(surfacesBefore + curveTags.Count, surfacesAfter);
        }

        [TestMethod]
        public void CreateChamferByAngle_MultipleCurvesAddsMultipleSurfaces()
        {
            gmshController.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTags = new[] { 12, 3 };
            var surfacesBefore = gmshController.Gmsh.Model.GetEntities(2).Length / 2;
            var projectController = CreateProjectController();

            projectController.CreateChamfer(curveTags, 0.1, 30, true, false);

            Assert.AreEqual(
                surfacesBefore + curveTags.Length,
                gmshController.Gmsh.Model.GetEntities(2).Length / 2);
        }

        [TestMethod]
        public void PreviewChamfer_ReturnsFourLinesWithoutChangingGeometry()
        {
            gmshController.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTag = gmshController.Gmsh.Model.GetEntities(1)[1];
            var entitiesBefore = gmshController.Gmsh.Model.GetEntities();
            var builder = new ChamferGeometryBuilder(gmshController);

            var preview = builder.PreviewChamfer(curveTag, 0.1, 0.2, false, false);
            var segments = preview.CreateSegments();

            Assert.AreEqual(0.1, Vector.GetVectorLength(preview.FirstOffset), 1e-6);
            Assert.AreEqual(0.2, Vector.GetVectorLength(preview.SecondOffset), 1e-6);
            Assert.AreEqual(4, segments.Length);
            Assert.IsTrue(segments.All(segment => segment.GetLength() > 0));
            CollectionAssert.AreEqual(entitiesBefore, gmshController.Gmsh.Model.GetEntities());
        }

        [TestMethod]
        public void PreviewChamfer_ReflectedSwapsSurfaceLengths()
        {
            gmshController.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTag = gmshController.Gmsh.Model.GetEntities(1)[1];
            var builder = new ChamferGeometryBuilder(gmshController);

            var preview = builder.PreviewChamfer(curveTag, 0.1, 0.2, false, true);

            Assert.AreEqual(0.2, Vector.GetVectorLength(preview.FirstOffset), 1e-6);
            Assert.AreEqual(0.1, Vector.GetVectorLength(preview.SecondOffset), 1e-6);
        }

        [TestMethod]
        public void PreviewChamfer_MultipleCurvesReturnsPreviewForEachCurve()
        {
            gmshController.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTags = gmshController.Gmsh.Model.GetEntities(1)
                .Where((_, index) => index % 2 == 1)
                .Take(2)
                .ToArray();
            var entitiesBefore = gmshController.Gmsh.Model.GetEntities();
            var builder = new ChamferGeometryBuilder(gmshController);

            var previews = builder.PreviewChamfer(curveTags, 0.1, 0.2, false, false);

            Assert.AreEqual(curveTags.Length, previews.Length);
            Assert.IsTrue(previews.All(preview => preview.CreateSegments().Length == 4));
            CollectionAssert.AreEqual(entitiesBefore, gmshController.Gmsh.Model.GetEntities());
        }

        [TestMethod]
        public void CreateChamfer_ThrowsForDuplicateCurveTags()
        {
            var builder = new ChamferGeometryBuilder(gmshController);

            Assert.ThrowsException<ArgumentException>(
                () => builder.Build(new[] { 1, 1 }, 0.1, 0.2, false));
        }

        [DataTestMethod]
        [DataRow(0.0)]
        [DataRow(-0.1)]
        [DataRow(double.NaN)]
        [DataRow(double.PositiveInfinity)]
        public void CreateChamfer_ThrowsForInvalidLength(double length)
        {
            var projectController = CreateProjectController();

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => projectController.CreateChamfer(1, length, 0.1, false, false));
        }

        [DataTestMethod]
        [DataRow(0.0)]
        [DataRow(-1.0)]
        [DataRow(90.0)]
        [DataRow(double.NaN)]
        [DataRow(double.PositiveInfinity)]
        public void CreateChamferByAngle_ThrowsForInvalidAngle(double angleInDegrees)
        {
            gmshController.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            gmshController.Gmsh.Model.Occ.Synchronize();

            var curveTag = gmshController.Gmsh.Model.GetEntities(1)[1];
            var projectController = CreateProjectController();

            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => projectController.CreateChamfer(curveTag, 0.1, angleInDegrees, true, false));
        }

        private int FindCurveBetweenNonPerpendicularSurfaces()
        {
            var curves = gmshController.Gmsh.Model.GetEntities(1);
            for (var index = 1; index < curves.Length; index += 2)
            {
                var curveTag = curves[index];
                var (surfaces, _) = gmshController.Gmsh.Model.GetAdjacencies(1, curveTag);
                if (surfaces.Length != 2)
                    continue;

                var (minimum, maximum) = gmshController.Gmsh.Model.GetParametrizationBounds(1, curveTag);
                var point = gmshController.Gmsh.Model.GetValue(1, curveTag, new[] { (minimum[0] + maximum[0]) / 2.0 });
                var firstNormal = GetNormal(surfaces[0], point);
                var secondNormal = GetNormal(surfaces[1], point);
                var absoluteDotProduct = Math.Abs(
                    firstNormal.Zip(secondNormal, (first, second) => first * second).Sum());

                if (absoluteDotProduct > 0.01 && absoluteDotProduct < 0.99)
                    return curveTag;
            }

            Assert.Fail("У клиновидной геометрии не найдено ребро между неперпендикулярными поверхностями.");
            return 0;
        }

        private double[] GetNormal(int surfaceTag, double[] point)
        {
            var parameters = gmshController.Gmsh.Model.GetParametrization(2, surfaceTag, point);
            var normal = gmshController.Gmsh.Model.GetNormal(surfaceTag, parameters);
            var length = Math.Sqrt(normal.Sum(component => component * component));
            return normal.Select(component => component / length).ToArray();
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
