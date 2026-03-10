using System.Collections.Generic;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.GeometryObjects;
using Model.Interfaces;
using OperationalController.ModelScenePresentator;

namespace ModelTests
{
    [TestClass]
    public class TestModelControllerPoligon
    {
        [TestMethod]
        public void TestOneSurfaceFigurePresentation()
        {
            //assing
            var points =
                new GeometryPoint[]
                {
                    new GeometryPoint(0,new Point3D(0 ,1, 0)),
                    new GeometryPoint(3,new Point3D(0 , 0 , 1)),
                    new GeometryPoint(2,new Point3D(0.3650039f , 0 , 0.6349962f)),
                    new GeometryPoint(1,new Point3D(0.3650039f, 0.6349962f , 0))
                };
            var surface = new GeometrySurface(0, points);
            var fig2D = new SurfaceFigure(0, new GeometrySurface[] { surface });

            var model = new ModelData();
            model.ObjectData.SurfaceSet.Add(0, fig2D);

            //act
            var creator = new PresentersCreator();
            var presentator = creator.CreateSurfaceObjectsPresenter(model.ObjectData.SurfaceSet.Values);
            var inds = presentator.CreateIndexes();
            var ptrs = presentator.CreatePointers(inds.Item1);
            var coords = presentator.CreateVertexes(inds.Item2, "координаты");
            var colors = presentator.CreateVertexes(inds.Item3, "цвет");
            var normals = presentator.CreateVertexes(inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, 6);
            Assert.AreEqual(coords.Length, 18);
            Assert.AreEqual(colors.Length, 24);
            Assert.AreEqual(normals.Length, 18);
        }       

        [TestMethod]
        public void TestManySurfaceFigurePresentation()
        {
            //assing
            var surfaces = new List<GeometrySurface>()
            {
                               new GeometrySurface(0,
                new GeometryPoint[]
                {
                    new GeometryPoint(0,new Point3D(0 ,1, 0)),
                    new GeometryPoint(1,new Point3D(0.3650039f, 0.6349962f , 0)),
                    new GeometryPoint(2,new Point3D(0.3650039f , 0 , 0.6349962f)),
                    new GeometryPoint(3,new Point3D(0 , 0 , 1))
                }),
                new GeometrySurface(1,
                new GeometryPoint[]
                {
                    new GeometryPoint(0,new Point3D(0.3650039f , 0.6349962f , 0)),
                    new GeometryPoint(1,new Point3D(0.6829777f , 0.3170223f , 0)),
                    new GeometryPoint(2,new Point3D(0.6829777f , 0 , 0.3170223f)),
                    new GeometryPoint(3,new Point3D(0.3650039f , 0 , 0.6349962f)),
                }),
                new GeometrySurface(2,
                new GeometryPoint[]
                {
                    new GeometryPoint(0,new Point3D(0.6829777f , 0.3170223f , 0)),
                    new GeometryPoint(1,new Point3D(1 , 0 , 0)),
                    new GeometryPoint(2,new Point3D(0.6829777f , 0 , 0.3170223f)),
                })

            };
            var model = new ModelData();
            model.ObjectData.SurfaceSet.Add(0, new SurfaceFigure(0,surfaces.ToArray()));

            //act
            var creator = new PresentersCreator();
            var presentator = creator.CreateSurfaceObjectsPresenter(model.ObjectData.SurfaceSet.Values);
            var inds = presentator.CreateIndexes();
            var ptrs = presentator.CreatePointers(inds.Item1);
            var coords = presentator.CreateVertexes(inds.Item2, "координаты");
            var colors = presentator.CreateVertexes(inds.Item3, "цвет");
            var normals = presentator.CreateVertexes(inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, 15);
            Assert.AreEqual(coords.Length, 45);
            Assert.AreEqual(colors.Length, 60);
            Assert.AreEqual(normals.Length, 45);
        }
    }
}
