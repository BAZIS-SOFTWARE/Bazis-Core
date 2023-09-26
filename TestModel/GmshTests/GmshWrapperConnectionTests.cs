using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;
using GmshApi.GmshController;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerConnectionTests
    {
        [TestMethod]
        public void GmshIsNotInitialized()
        {
            Assert.AreEqual(false, ControllerConnection.IsInitialized());
        }
        [TestMethod]
        public void GmshIsInitialized()
        {
            Assert.AreEqual(false, ControllerConnection.IsInitialized());
            ControllerConnection.Initialize();
            Assert.AreEqual(true, ControllerConnection.IsInitialized());
            ControllerConnection.Finalize();
            Assert.AreEqual(false, ControllerConnection.IsInitialized());
        }
    }
}
