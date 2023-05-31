using System.Collections.Generic;
using System.Drawing;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.ModelScenePresentator;
using SceneInterface;

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
            var presentator = new ModelScenePresentator(model);
            var inds = presentator.CreateVBOIndexes("Поверхность");
            var ptrs = presentator.CreateVBOPointers("Поверхность", inds.Item1);
            var coords = presentator.CreateVBOVertexes("Поверхность", inds.Item2, "координаты");
            var colors = presentator.CreateVBOVertexes("Поверхность", inds.Item3, "цвет");
            var normals = presentator.CreateVBOVertexes("Поверхность", inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, 6);
            Assert.AreEqual(coords.Length, 18);
            Assert.AreEqual(colors.Length, 24);
            Assert.AreEqual(normals.Length, 18);
        }

        //[TestMethod]
        //public void TestComplexPoligons()
        //{
        //    var model = new ModelData();
        //    var loader = new LoadModelFromProjectTextFile();
        //    loader.LoadEvent += (ar1, ar2) => { };
        //    //var project = loader.Load(@"C:\Bazis_newGUI\PROJ\Test_3D\Term\two_tetra_term_cycle_multiStep.bpf");
        //    var modelLoad = loader.Load(@"C:\Projs\TestProjTerm\tube\test.bpf");

        //    model.Load(modelLoad);

        //    var loaderDB = new LoadResultsFileDB();
        //    loaderDB.LoadEvent += (ar1, ar2) => { };
        //    var results = loaderDB.Load(@"C:\Projs\TestProjTerm\tube\термическая_0_0_10.db");

        //    var elements = model.ObjectData.FindMany<Element>().ToArray();

        //    var scale = new RainbowScale(4, 150, 0, 10);

        //    var gradientFieldsCreator = new GradientFieldsCreator(elements, scale.ValueRange().ToArray(), scale.ColorRange().ToArray(), 1);

        //    var reultObjects = gradientFieldsCreator.CreateFieldObjects(results[24], "Узлы", "T");

        //    var resModel = new ModelData();
        //    resModel.ObjectData.AddRange(reultObjects);
        //    var presenter = new ModelScenePresentator(resModel);

        //    presenter.HideInsideObjects("Поверхность");

        //    var inds = presenter.CreateVBOIndexes("Поверхность");
        //    var ptrs = presenter.CreateVBOPointers("Поверхность", inds.Item1);
        //    var coords = presenter.CreateVBOVertexes("Поверхность", inds.Item2, "координаты");
        //    var colors = presenter.CreateVBOVertexes("Поверхность", inds.Item2, "цвет");
        //    var normals = presenter.CreateVBOVertexes("Поверхность", inds.Item2, "нормаль");
        //}

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
            var presentator = new ModelScenePresentator(model);
            var inds = presentator.CreateVBOIndexes("Поверхность");
            var ptrs = presentator.CreateVBOPointers("Поверхность", inds.Item1);
            var coords = presentator.CreateVBOVertexes("Поверхность", inds.Item2, "координаты");
            var colors = presentator.CreateVBOVertexes("Поверхность", inds.Item3, "цвет");
            var normals = presentator.CreateVBOVertexes("Поверхность", inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, 15);
            Assert.AreEqual(coords.Length, 45);
            Assert.AreEqual(colors.Length, 60);
            Assert.AreEqual(normals.Length, 45);
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
            Assert.AreEqual(ptrs.Length, 15);
            Assert.AreEqual(coords.Length, 45);
            Assert.AreEqual(colors.Length, 45);
            Assert.AreEqual(normals.Length, 45);
        }
    }
}
