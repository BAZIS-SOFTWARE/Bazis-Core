using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Events;
using ModelController.ModelScenePresentator;
using ModelParcer;
using SceneInterface;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerASCII
    {
        [TestMethod]
        public void GlVertexesCreationSurf_fromSTLText()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromSTLFile();
            //model.SetLoader(loader);
            loader.LoadEvent += (ar1, ar2) => { };
            loader.Load(@"C:\BazisComponents\ModelSolution\TestModel\Examples\STL\ACII.stl");

            //act
            var surfs = model.ObjectData.FindMany<Surface>().ToArray();

            var presentator = new ModelScenePresentator(model);

            //var surfObjsCreator = presentator.CreateSurfaceObjsCreator("",true);

            //var indexes = surfObjsCreator.CreateIndexes();

            //var ptrs = surfObjsCreator.CreatePointers(indexes.Item1);
            //var coords = surfObjsCreator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = surfObjsCreator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = surfObjsCreator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());

            //assert

        }

        [DataTestMethod]
        [DataRow(new string[]
{
            "Элементы3D", "24", "72", "96", "72"
})]
        [DataRow(new string[]
{
            "Узлы", "7", "21", "28", "21"
})]
        public void GlVertexesCreation3DObjs(string[] array)
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += Model_LoadEvent;
            var modelASC = loader.Load(@"C:\BazisComponents\ModelSolution\TestModel\Examples\VIsual-Mesh\rhombDATA1.ASC");
            model.Load(modelASC);
            //act
            var presenter = new ModelScenePresentator(model);

            presenter.HideInsideObjects(array[0]);

            var inds = presenter.CreateVBOIndexes(array[0]);
            var ptrs = presenter.CreateVBOPointers(array[0], inds.Item1);
            var coords = presenter.CreateVBOVertexes(array[0], inds.Item2, "координаты");
            var colors = presenter.CreateVBOVertexes(array[0], inds.Item3, "цвет");
            var normals = presenter.CreateVBOVertexes(array[0], inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, int.Parse(array[1]));
            Assert.AreEqual(coords.Length, int.Parse(array[2]));
            Assert.AreEqual(colors.Length, int.Parse(array[3]));
            Assert.AreEqual(normals.Length, int.Parse(array[4]));
        }   

        [TestMethod]
        public void GlVertexesCreation2D3DObjs_Elem2D3D()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += Model_LoadEvent;
            var modelASC = loader.Load(@"C:\Projs\ENERGOMASH\MESH\SECTOR_DATA1.ASC");
            model.Load(modelASC);

            //act
            var elems3D = model.ObjectData.FindMany<Element3D>().ToArray();
            var presentator = new ModelScenePresentator(model);

            //var surfObjsCreator = presentator.Cre("Элементы3D", true);

            //var indexes = surfObjsCreator.CreateIndexes();

            //var ptrs = surfObjsCreator.CreatePointers(indexes.Item1);
            //var coords = surfObjsCreator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = surfObjsCreator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = surfObjsCreator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());

            //assert

        }

        private void Model_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void GlVertexesCreation2DObjs_Elem2D()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += Model_LoadEvent;
            var modelASC = loader.Load(@"C:\Bazis_newGUI\MESH\Test_DATA1.ASC");
            model.Load(modelASC);

            //act
            var elems2D = model.ObjectData.FindMany<Element2D>().ToArray();
            var presentator = new ModelScenePresentator(model);

            //var surfObjsCreator = presentator.CreateSurfaceObjsCreator("Элементы2D", true);

            //var indexes = surfObjsCreator.CreateIndexes();

            //var ptrs = surfObjsCreator.CreatePointers(indexes.Item1);
            //var coords = surfObjsCreator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = surfObjsCreator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = surfObjsCreator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());

            //assert

        }
    }
}
