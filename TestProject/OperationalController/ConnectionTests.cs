using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalController.GmshController;

namespace TestProject_test.OperationalController
{
    [TestClass]
    public class ConnectionTests
    {
        private GmshController controller = new GmshController();

        [TestMethod]
        public void GmshInitializeFinalize()
        {
            controller.Load(GmshLibrary.FilePath);
            var ierr = 0;
            Assert.AreEqual(1, controller.Gmsh.IsInitialized());

            controller.Unload();
            Assert.AreEqual(0, controller.Gmsh.IsInitialized());
        }
    }
}
