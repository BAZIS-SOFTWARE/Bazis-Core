using GmshApi.GmshController;
using GmshApi.GmshKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerTests
    {
        private ControllerGeneral controller;
        [TestInitialize]
        public void Init()
        {
            controller = new ControllerGeneral("C:\\Windows\\System32\\gmsh.dll");
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void TestInitializeMethod()
        {
            var ierr = 0;
            String argv = String.Empty;
            controller.gmshInitialize(0,ref argv, 0, 0,ref ierr);
            Assert.AreEqual(1,controller.gmshIsInitialized(ref ierr));
            controller.gmshFinalize(ref ierr);
            Assert.AreEqual(0, controller.gmshIsInitialized(ref ierr));
        }

    }
}
