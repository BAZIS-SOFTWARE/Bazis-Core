using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController;
using ModelParcer;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerGMSH_inp
    {
        [TestMethod]
        public void GlVertexesCreation_2D3D()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromGMSHTextFile();

            loader.LoadEvent += (ar1, ar2) => { };
            var modelINP = loader.Load(@"C:\BazisComponents\ModelSolution\Examples\3D\Korpus.inp");
            model.Load(modelINP);

            //act
            var presentator = new ModelScenePresentator(model);

            //var surfObjsCreator = presentator.CreateSurfaceObjsCreator("Элементы3D", true);

            //var indexes = surfObjsCreator.CreateIndexes();

            //var ptrs = surfObjsCreator.CreatePointers(indexes.Item1);
            //var coords = surfObjsCreator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = surfObjsCreator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = surfObjsCreator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());

            //assert

        }

        [TestMethod]
        public void GlVertexesCreation_1D2D()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromGMSHTextFile();

            loader.LoadEvent += (ar1, ar2) => { };
            var modelINP = loader.Load(@"C:\BazisComponents\ModelSolution\Examples\2D_axi\2Dval.inp");
            model.Load(modelINP);

            //act
            //var elems3D = model.ObjectData.FindMany<Element3D>().ToArray();
            var presentator = new ModelScenePresentator(model);

            var indexes_nodes = presentator.CreateVBOIndexes("Узлы");
            var ptrs_nodes = presentator.CreateVBOPointers("Узлы", indexes_nodes.Item1);
            var coords_nodes = presentator.CreateVBOVertexes("Узлы", indexes_nodes.Item2, "координаты");
            var colors_nodes = presentator.CreateVBOVertexes("Узлы", indexes_nodes.Item2, "цвет");
            var normals_nodes = presentator.CreateVBOVertexes("Узлы", indexes_nodes.Item2, "нормаль");
            var indexes = presentator.CreateVBOIndexes("Элементы1D");

            var ptrs = presentator.CreateVBOPointers("Элементы1D",indexes.Item1);
            var coords = presentator.CreateVBOVertexes("Элементы1D", indexes.Item2, "координаты");
            var colors = presentator.CreateVBOVertexes("Элементы1D", indexes.Item2, "цвет");
            var normals = presentator.CreateVBOVertexes("Элементы1D", indexes.Item2,"нормаль");

            //assert

        }
    }
}
