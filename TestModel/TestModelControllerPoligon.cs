using System.Collections.Generic;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.GeometryObjects;
using Model.MeshObjects;
using ModelController.ModelScenePresentator;
using ModelInterfaces;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerPoligon
    {
        [TestMethod]
        public void TestPoligonOneSurface()
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

            var fig2D = new Figure2D(0, points);
            var model = new ModelData();
            model.ObjectData.Fig2DCollection.Add(fig2D);

            //act
            var creator = new PresentorsCreator();
            var presentator = creator.CreateSurfaceGeometryPresenter(model.ObjectData.Fig2DCollection,false);
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
        public void TestPoligonsManySurfaces()
        {
            //assing
            var figs2D = new List<Figure2D>()
            {
                               new Figure2D(0,
                new GeometryPoint[]
                {
                    new GeometryPoint(0,new Point3D(0 ,1, 0)),
                    new GeometryPoint(1,new Point3D(0.3650039f, 0.6349962f , 0)),
                    new GeometryPoint(2,new Point3D(0.3650039f , 0 , 0.6349962f)),
                    new GeometryPoint(3,new Point3D(0 , 0 , 1))
                }),
                new Figure2D(1,
                new GeometryPoint[]
                {
                    new GeometryPoint(0,new Point3D(0.3650039f , 0.6349962f , 0)),
                    new GeometryPoint(1,new Point3D(0.6829777f , 0.3170223f , 0)),
                    new GeometryPoint(2,new Point3D(0.6829777f , 0 , 0.3170223f)),
                    new GeometryPoint(3,new Point3D(0.3650039f , 0 , 0.6349962f)),
                }),
                new Figure2D(2,
                new GeometryPoint[]
                {
                    new GeometryPoint(0,new Point3D(0.6829777f , 0.3170223f , 0)),
                    new GeometryPoint(1,new Point3D(1 , 0 , 0)),
                    new GeometryPoint(2,new Point3D(0.6829777f , 0 , 0.3170223f)),
                })

            };
            var model = new ModelData();
            model.ObjectData.Fig2DCollection.AddRange(figs2D);

            //act
            var creator = new PresentorsCreator();
            var presentator = creator.CreateSurfaceGeometryPresenter(model.ObjectData.Fig2DCollection, false);
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
