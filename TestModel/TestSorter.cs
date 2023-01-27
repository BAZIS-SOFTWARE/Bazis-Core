using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.IO;
using ModelInterface;

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

            model.SetLoader(loader);
            model.LoadEvent += Model_LoadEvent; ;

            //act
            model.Load(@"C:\Projs\TestProjTerm\Cub\cub_unsorted.bpf");

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
