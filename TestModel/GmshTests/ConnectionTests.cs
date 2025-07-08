using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectController.GmshController;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerConnectionTests
    {
        private GmshController controller = new GmshController();

        [TestMethod]
        public void GmshInitializeFinalize()
        {
            controller.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            var ierr = 0;
            Assert.AreEqual(1, controller.Gmsh.IsInitialized());

            controller.Unload();
            Assert.AreEqual(0, controller.Gmsh.IsInitialized());
        }
    }
}
