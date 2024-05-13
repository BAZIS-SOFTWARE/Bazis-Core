using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi.GmshController;

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
            Assert.AreEqual(1, controller.gmshIsInitialized(ref ierr));

            controller.Finalize(ref ierr);
            Assert.AreEqual(0, controller.gmshIsInitialized(ref ierr));
        }
    }
}
