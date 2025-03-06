using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerTest
    {
        private Controller controller = new Controller(); 

        [TestMethod]
        public void GmshInitializeTest()
        {
            controller.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            controller.Gmsh.Open(@"D:\Cilindr.step");
            controller.Gmsh.Model.Mesh.Generate(3);
            var objects = controller.GetMeshObjects();
            controller.Gmsh.Clear();
            objects = controller.GetMeshObjects();
            controller.Unload();
        }
    }
}
