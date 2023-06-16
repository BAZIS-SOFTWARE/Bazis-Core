using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.ModelScenePresentator;
using Model.ModelParcer;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerCDB
    {
        [DataTestMethod]
        [DataRow(new string[]
{
            "Элементы3D"
})]
        [DataRow(new string[]
{
            "Узлы"
})]
        public void GlVertexesCreation_2D3D(string[] array)
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromCDBTextFile();

            loader.LoadEvent += (ar1, ar2) => { };
            var modelCDB = loader.Load(@"..\..\Examples\CDB\longModel.cdb");

            model.Load(modelCDB);

            //act
            var presentator = new ModelScenePresentator(model);

            var indexes_nodes = presentator.CreateVBOIndexes(array[0]);
            var ptrs_nodes = presentator.CreateVBOPointers(array[0], indexes_nodes.Item1);
            var coords_nodes = presentator.CreateVBOVertexes(array[0], indexes_nodes.Item2, "координаты");
            var colors_nodes = presentator.CreateVBOVertexes(array[0], indexes_nodes.Item3, "цвет");
            var normals_nodes = presentator.CreateVBOVertexes(array[0], indexes_nodes.Item2, "нормаль");
           
            //assert

        }
    }
}
