using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Events;
using ModelController.ModelScenePresentator;
using Model.ModelParcer;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerASCII
    {
       
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
      

        private void Model_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new NotImplementedException();
        }
    }
}
