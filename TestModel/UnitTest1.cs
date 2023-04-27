using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelParcer;
using Model;
using Geometry;
using System;

namespace TestModel
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string path = @"C:\file1.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += this.Import_LoadEvent;
            var model = import.Load(path);
            Assert.AreEqual(8, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(1, model.ObjectData.NumberOfElements);
        }

        private void Import_LoadEvent(object arg1, Model.Events.LoaderEventArgs arg2)
        {

        }
    }
}
