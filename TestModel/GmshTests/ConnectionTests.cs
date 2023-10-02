using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;
using GmshApi.GmshController;
using System.ComponentModel;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerConnectionTests
    {
        private GmshController controller = new GmshController("C:\\Windows\\System32\\gmsh.dll");
        private static string modelFolder = @"..\..\..\Examples\GMSHConnector\";
        public void Init()
        {
            /*var argv = string.Empty;
            var ierr = 0;
            controller.gmshInitialize(0, ref argv, 0, 0, ref ierr);*/
        }
        [TestCleanup]
        public void Cleanup()
        {
            var ierr = 0;
            //controller.gmshFinalize(ref ierr);
            controller.gmshClear(ref ierr);
        }
        [TestMethod]
        public void GmshInitializeFinalize()
        {
            var ierr = 0;
            Assert.AreEqual(1, controller.gmshIsInitialized(ref ierr));
            controller.gmshFinalize(ref ierr);
            Assert.AreEqual(0, controller.gmshIsInitialized(ref ierr));
        }
    }
}
