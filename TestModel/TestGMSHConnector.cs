using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelParcer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModel
{
    [TestClass]
    public class TestGMSHConnector
    {
        [TestMethod]
        public void TestCreateGMSHConnector()
        {
            var connector = new GMSHConnector();
            connector.Dispose();
        }

        [TestMethod]
        public void TestScriptGMSHConnector() 
        {
            var connector = new GMSHConnector();
            connector.GMSHExtractModelFromScript("sphere.geo","sphere.inp");
            connector.Dispose();
        }

    }
}
