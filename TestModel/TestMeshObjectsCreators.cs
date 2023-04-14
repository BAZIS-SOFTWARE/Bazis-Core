using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.IO;
using ModelController.MeshObjsUtility;

namespace TestModel
{
    [TestClass]
    public class TestMeshObjectsCreators
    {
        [TestMethod]
        public void FindFreeNodes()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromProjectTextFile();
            loader.LoadEvent += Model_LoadEvent;
            //act
            var modelINP = loader.Load(@"C:\Projs\TestProjTerm\Cub\cub_unsorted_freeNodes.bpf");
            model.Load(modelINP);

            var freeNodesFinder = new FreeNodesFinder(model.ObjectData);

            var freeNodes = freeNodesFinder.Find<Element>();

            //assert
            freeNodes.ToArray()[0] = 14;
            freeNodes.ToArray()[1] = 15;
        }

        private void Model_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new NotImplementedException();
        }
    }
}
