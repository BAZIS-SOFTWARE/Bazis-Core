using System;
using System.Collections.Generic;
using System.Drawing;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.ModelScenePresentator;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerPoligon
    {
        [TestMethod]
        public void TestPoligon4Points()
        {
            //assing
            var poligons = new List<Surface>()
            {
                               new Surface(0,
                new Node[]
                {
                    new Node(0,new Point3D(0 ,1, 0)),
                    new Node(3,new Point3D(0 , 0 , 1)),
                    new Node(2,new Point3D(0.3650039f , 0 , 0.6349962f)),
                    new Node(1,new Point3D(0.3650039f, 0.6349962f , 0))
                },
                Model.Interfaces.ObjKind.ElementSurface,Color.Orange)
            };

            var model = new ModelData();
            model.ObjectData.AddRange(poligons);

            var presentator = new ModelScenePresentator(model);

            //presentator.CreateSurfaceObjsCreator("Поверхность");

            //var indexes = surfaceObjsCreator.CreateIndexes();

            //assert
            //if (indexes.Item1 != 4)
                //throw new Exception();
        }

        [TestMethod]
        public void TestPoligons()
        {
            //assing
            var poligons = new List<Surface>()
            {
                               new Surface(0,
                new Node[]
                {
                    new Node(0,new Point3D(0 ,1, 0)),
                    new Node(1,new Point3D(0.3650039f, 0.6349962f , 0)),
                    new Node(2,new Point3D(0.3650039f , 0 , 0.6349962f)),
                    new Node(3,new Point3D(0 , 0 , 1))
                },
                Model.Interfaces.ObjKind.ElementSurface,Color.Orange),
                new Surface(1,
                new Node[]
                {
                    new Node(0,new Point3D(0.3650039f , 0.6349962f , 0)),
                    new Node(1,new Point3D(0.6829777f , 0.3170223f , 0)),
                    new Node(2,new Point3D(0.6829777f , 0 , 0.3170223f)),
                    new Node(3,new Point3D(0.3650039f , 0 , 0.6349962f)),
                },
                Model.Interfaces.ObjKind.ElementSurface,Color.Red),
                new Surface(2,
                new Node[]
                {
                    new Node(0,new Point3D(0.6829777f , 0.3170223f , 0)),
                    new Node(1,new Point3D(1 , 0 , 0)),
                    new Node(2,new Point3D(0.6829777f , 0 , 0.3170223f)),
                },
                Model.Interfaces.ObjKind.ElementSurface,Color.Orange)

            };
            var model = new ModelData();
            model.ObjectData.AddRange(poligons);

            //act
            var presentator = new ModelScenePresentator(model);
            var inds = presentator.CreateVBOIndexes("Поверхность");
            var ptrs = presentator.CreateVBOPointers("Поверхность", inds.Item1);
            var coords = presentator.CreateVBOVertexes("Поверхность", inds.Item2, "координаты");
            var colors = presentator.CreateVBOVertexes("Поверхность", inds.Item2, "цвет");
            var normals = presentator.CreateVBOVertexes("Поверхность", inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, 24);
            Assert.AreEqual(coords.Length, 72);
            Assert.AreEqual(colors.Length, 72);
            Assert.AreEqual(normals.Length, 72);
        }
    }
}
