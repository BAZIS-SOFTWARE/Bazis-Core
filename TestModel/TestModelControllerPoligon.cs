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
                new Point[]
                {
                    new Point(0,new Point3D(0 ,1, 0)),
                    new Point(3,new Point3D(0 , 0 , 1)),
                    new Point(2,new Point3D(0.3650039f , 0 , 0.6349962f)),
                    new Point(1,new Point3D(0.3650039f, 0.6349962f , 0))
                };

            var fig2D = new Figure2D(0, points);
            var model = new ModelData();
            model.ObjectData.Add(fig2D);

            //act
            var presentator = new ModelScenePresentator(model.ObjectData);
            var inds = presentator[ObjType.Фигура2D].CreateIndexes();
            var ptrs = presentator[ObjType.Фигура2D].CreatePointers(inds.Item1);
            var coords = presentator[ObjType.Фигура2D].CreateVertexes(inds.Item2, "координаты");
            var colors = presentator[ObjType.Фигура2D].CreateVertexes(inds.Item3, "цвет");
            var normals = presentator[ObjType.Фигура2D].CreateVertexes(inds.Item2, "нормаль");
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
                new Point[]
                {
                    new Point(0,new Point3D(0 ,1, 0)),
                    new Point(1,new Point3D(0.3650039f, 0.6349962f , 0)),
                    new Point(2,new Point3D(0.3650039f , 0 , 0.6349962f)),
                    new Point(3,new Point3D(0 , 0 , 1))
                }),
                new Figure2D(1,
                new Point[]
                {
                    new Point(0,new Point3D(0.3650039f , 0.6349962f , 0)),
                    new Point(1,new Point3D(0.6829777f , 0.3170223f , 0)),
                    new Point(2,new Point3D(0.6829777f , 0 , 0.3170223f)),
                    new Point(3,new Point3D(0.3650039f , 0 , 0.6349962f)),
                }),
                new Figure2D(2,
                new Point[]
                {
                    new Point(0,new Point3D(0.6829777f , 0.3170223f , 0)),
                    new Point(1,new Point3D(1 , 0 , 0)),
                    new Point(2,new Point3D(0.6829777f , 0 , 0.3170223f)),
                })

            };
            var model = new ModelData();
            model.ObjectData.AddRange(figs2D);

            //act
            var presentator = new ModelScenePresentator(model.ObjectData);
            var inds = presentator[ObjType.Фигура2D].CreateIndexes();
            var ptrs = presentator[ObjType.Фигура2D].CreatePointers(inds.Item1);
            var coords = presentator[ObjType.Фигура2D].CreateVertexes(inds.Item2, "координаты");
            var colors = presentator[ObjType.Фигура2D].CreateVertexes(inds.Item3, "цвет");
            var normals = presentator[ObjType.Фигура2D].CreateVertexes(inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, 15);
            Assert.AreEqual(coords.Length, 45);
            Assert.AreEqual(colors.Length, 60);
            Assert.AreEqual(normals.Length, 45);
        }
    }
}
