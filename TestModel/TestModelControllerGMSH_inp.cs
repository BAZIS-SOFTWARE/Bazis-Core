using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.ModelScenePresentator;
using Model.IO;
using System;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerGMSH_inp
    {       
        [DataTestMethod]
        [DataRow(new string[]
{
            "2D_axi\\2Dval.inp"
})]
        [DataRow(new string[]
{
            "3D\\Korpus.inp"
})]
        public void GlCoordsColorsEdgesIndexesCreation(string[] array)
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromGMSHTextFile();

            loader.LoadEvent += (ar1, ar2) => { };
            var modelINP = loader.Load($@"C:\BazisComponents\ModelSolution\TestModel\Examples\{array[0]}");
            model.Load(modelINP);

            var presenter = new ModelScenePresentator(model);

            foreach (string item in presenter?.GetObjectsTypes())
            {
                presenter.HideInsideObjects(item);

                var tuple = presenter.CreateVBOIndexes(item);
                int[] ptrs = presenter.CreateVBOPointers(item, tuple.Item1);
                float[] glCoords = presenter.CreateVBOVertexes(item, tuple.Item2, "координаты");
                float[] glColors = presenter.CreateVBOVertexes(item, tuple.Item3, "цвет");
                float[] glNormals = presenter.CreateVBOVertexes(item, tuple.Item2, "нормаль");
                bool[] edges = presenter.CreateVBOEdges(item, tuple.Item4);

                Assert.AreEqual(tuple.Item4, edges.Length);
                Assert.AreEqual(tuple.Item1, ptrs.Length);
                Assert.AreEqual(tuple.Item2, glCoords.Length);
                Assert.AreEqual(tuple.Item3, glColors.Length);
                Assert.AreEqual(tuple.Item2, glNormals.Length);
            }

        }
    }
}
