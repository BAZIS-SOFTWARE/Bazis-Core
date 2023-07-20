using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi.Wrapper;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GmshWrapperConnectionTests
    {
        [TestMethod]
        public void GmshIsNotInitialized()
        {
            Assert.AreEqual(false, GmshWrapperConnection.IsInitialized());
        }
        [TestMethod]
        public void GmshIsInitialized()
        {
            Assert.AreEqual(false, GmshWrapperConnection.IsInitialized());
            GmshWrapperConnection.Initialize();
            Assert.AreEqual(true, GmshWrapperConnection.IsInitialized());
            GmshWrapperConnection.Finalize();
            Assert.AreEqual(false, GmshWrapperConnection.IsInitialized());
        }
    }
}
