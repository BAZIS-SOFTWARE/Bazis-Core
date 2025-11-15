using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GeometryObjects;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModel.ModelTests
{
    [TestClass]
    public class TestGeometryObjects
    {
        [TestMethod]
        public void TestOnPointsContains()
        {
            var points = new GeometryPoint[]
{
                new GeometryPoint(0, new Point3D(0.0f,0.0f,0.0f)),
                new GeometryPoint(1, new Point3D(1.0f,0.0f,0.0f)),
                new GeometryPoint(2, new Point3D(1.0f,1.0f,0.0f)),
                new GeometryPoint(3, new Point3D(0.0f,1.0f,0.0f)),
                new GeometryPoint(4, new Point3D(-1.0f,1.0f,0.0f)),
                new GeometryPoint(5, new Point3D(-1.0f,0.0f,0.0f))
};
            var surface0 = new GeometrySurface(0, new GeometryPoint[] { points[0], points[1], points[2] });
            var surface1 = new GeometrySurface(0, new GeometryPoint[] { points[0], points[2], points[3] });
            var surface2 = new GeometrySurface(0, new GeometryPoint[] { points[0], points[3], points[4] });
            var figure = new SurfaceFigure(0, new GeometrySurface[] { surface0, surface1, surface2 });

            Assert.AreEqual(figure.GetVertexes().Count(), 5);
            Assert.AreEqual(figure.CalcCentr(),new Point3D(0.2f,0.6f,0));
        }
    }
}
