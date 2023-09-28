using GmshApi.GmshController;
using GmshApi.GmshKernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GmshApi.GmshController.ControllerGeneral;

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
            var isInitialized = controller.GetFunction<gmshIsInitialized>("gmshIsInitialized");
            Assert.AreEqual(1, isInitialized(ref ierr));
        }
    }
}
