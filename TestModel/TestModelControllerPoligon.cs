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
        public void TestPoligonOneSurface()
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

            //act
            var presentator = new ModelScenePresentator(model.ObjectData);
            var inds = presentator["Поверхности"].CreateIndexes();
            var ptrs = presentator["Поверхности"].CreatePointers(inds.Item1);
            var coords = presentator["Поверхности"].CreateVertexes(inds.Item2, "координаты");
            var colors = presentator["Поверхности"].CreateVertexes(inds.Item3, "цвет");
            var normals = presentator["Поверхности"].CreateVertexes(inds.Item2, "нормаль");
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
            var presentator = new ModelScenePresentator(model.ObjectData);
            var inds = presentator["Поверхности"].CreateIndexes();
            var ptrs = presentator["Поверхности"].CreatePointers(inds.Item1);
            var coords = presentator["Поверхности"].CreateVertexes(inds.Item2, "координаты");
            var colors = presentator["Поверхности"].CreateVertexes(inds.Item3, "цвет");
            var normals = presentator["Поверхности"].CreateVertexes(inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, 15);
            Assert.AreEqual(coords.Length, 45);
            Assert.AreEqual(colors.Length, 60);
            Assert.AreEqual(normals.Length, 45);
        }
    }
}
