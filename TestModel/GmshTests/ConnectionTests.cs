using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;
using GmshApi.GmshController;
using System.ComponentModel;
using ModelControllerInterfaces.GmshController;

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
                //controller.gmshFinalize(ref ierr);
            Assert.AreEqual(0, controller.gmshIsInitialized(ref ierr));
        }
    }
}
