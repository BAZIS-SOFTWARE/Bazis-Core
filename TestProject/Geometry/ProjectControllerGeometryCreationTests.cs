using Geometry;
using Model;
using Model.Interfaces;
using OperationalController;
using OperationalController.GmshController;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestProject_test.Geometry
{
    [TestClass]
    public class ProjectControllerGeometryCreationTests
    {
        [TestMethod]
        public void CreateGeometryObjectsTest()
        {
            var project = new ProjectController();
            project.CreateProject(@"newProj.bpf2");
            project.InitializeGeometry(GmshLibrary.FilePath);

            var start = project.CreatePoint(0, 0, 0);
            var middle = project.CreatePoint(2, 2, 0);
            var end = project.CreatePoint(2, 0, 0);

            var pointByVector = project.CreatePointByVector(start, middle, 1);

            var line1 = project.CreateLine(start, middle);
            var line2 = project.CreateLine(middle, end);
            var line3 = project.CreateLine(end, start);

            var plane = project.CreateSurface(new int[] { line1, line2, line3 });

            // ===== ASSERT POINTS =====
            var s = project.GetModelPoint(start);
            var pbv = project.GetModelPoint(pointByVector);
            Assert.IsNotNull(s);
            Assert.AreEqual(start, s.Number);
            Assert.IsNotNull(pbv);
            Assert.AreEqual(pointByVector, pbv.Number);

            var m = project.GetModelPoint(middle);
            Assert.IsNotNull(m);
            Assert.AreEqual(middle, m.Number);

            var e = project.GetModelPoint(end);
            Assert.IsNotNull(e);
            Assert.AreEqual(end, e.Number);

            // ===== ASSERT CURVES =====
            var c = project.GetModelCurve(line2);
            Assert.IsNotNull(c);
            Assert.AreEqual(line2, c.Number);

            // ===== ASSERT SURFACE =====
            var p = project.GetModelSurface(plane);
            Assert.IsNotNull(p);
            Assert.AreEqual(plane, p.Number);
        }

        [TestMethod]
        public void CreatePoint_ShouldThrow_WhenGeometryIsNotInitialized()
        {
            var project = new ProjectController();
            project.CreateProject(@"newProj.bpf2");

            Assert.ThrowsException<InvalidOperationException>(() => project.CreatePoint(0, 0, 0));
        }
    }
}
