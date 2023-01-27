using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.IO;
using ModelController.MeshObjsUtility;
using ModelInterface;

namespace TestModel
{
    [TestClass]
    public class TestMeshObjectsCreators
    {
        [TestMethod]
        public void Extract2DFrom3DMesh()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromGMSH_InpFile();
            model.SetLoader(loader);

            model.LoadEvent += Model_LoadEvent;
            //act
            model.Load(@"C:\Projs\TestProjTerm\box\box1.inp");


            var els3D = model.ObjectData.FindMany<Element3D>();

            var nodes = model.ObjectData.FindMany<Node>();
            var freeNodesFinder = new FreeNodesFinder(nodes.ToArray());

            var interNodesFinder = new FindInterfacedNodes(els3D.ToArray());

            var freeNodes = freeNodesFinder.Find(interNodesFinder.Find());
            //assert
        }

        private void Model_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new NotImplementedException();
        }
    }
}
