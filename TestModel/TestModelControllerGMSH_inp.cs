using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.ModelScenePresentator;
using ModelParcer;
using System;

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
            var modelINP = loader.Load(@"C:\BazisComponents\ModelSolution\TestModel\Examples\3D\Korpus.inp");
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
            var modelINP = loader.Load(@"C:\BazisComponents\ModelSolution\TestModel\Examples\2D_axi\2Dval.inp");
            model.Load(modelINP);

            var presenter = new ModelScenePresentator(model);

            foreach (string item in presenter?.GetObjectsTypes())
            {
                presenter.HideInsideObjects(item);

                Tuple<int, int, int> tuple = presenter.CreateVBOIndexes(item);
                int[] ptrs = presenter.CreateVBOPointers(item, tuple.Item1);
                float[] glCoords = presenter.CreateVBOVertexes(item, tuple.Item2, "координаты");
                float[] glColors = presenter.CreateVBOVertexes(item, tuple.Item2, "цвет");
                float[] glNormals = presenter.CreateVBOVertexes(item, tuple.Item2, "нормаль");
                bool[] edges = presenter.CreateVBOEdges(item, tuple.Item3);

                Assert.AreEqual(tuple.Item3, edges.Length);
            }

        }
    }
}
