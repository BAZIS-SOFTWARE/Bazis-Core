using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Events;
using ModelController.IO;

namespace TestModel
{
    [TestClass]
    public class TestSorter
    {
        [TestMethod]
        public void SortObjectsByNumber()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromProjectTextFile();
            loader.LoadEvent += Model_LoadEvent;

            //act
            var modelBPF = loader.Load(@"C:\Projs\TestProjTerm\Cub\cub_unsorted.bpf");
            model.Load(modelBPF);

            //assert
            var number = 0;
            foreach (var modelObject in model.ObjectData)
            {
                Assert.AreEqual(number, modelObject.Number);
                number++;
            }
        }

        private void Model_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new NotImplementedException();
        }
    }
}
