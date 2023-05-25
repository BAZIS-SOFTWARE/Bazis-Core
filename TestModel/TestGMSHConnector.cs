using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelParcer;
using System.IO;
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
            GMSHConnector.Init();
            GMSHConnector.Dispose();
        }

        [TestMethod]
        public void TestScriptGMSHConnector1D()
        {
            GMSHConnector.Init();
            GMSHConnector.GMSHCreateFromScript(@"..\..\Examples\GMSHConnector\sphere1D.geo", ".inp", "Элементы1D");
            CompareContentOfFiles(@"..\..\Examples\GMSHConnector\sphere1D.inp",
                                  @"..\..\Examples\GMSHConnector\expected1D.inp");
            GMSHConnector.Dispose();
        }
        [TestMethod]
        public void TestScriptGMSHConnector2D()
        {
            GMSHConnector.Init();
            GMSHConnector.GMSHCreateFromScript(@"..\..\Examples\GMSHConnector\sphere2D.geo", ".inp", "Элементы2D");
            CompareContentOfFiles(@"..\..\Examples\GMSHConnector\sphere2D.inp",
                                  @"..\..\Examples\GMSHConnector\expected2D.inp");
            GMSHConnector.Dispose();
        }
        [TestMethod]
        public void TestScriptGMSHConnector3D()
        {
            GMSHConnector.Init();
            GMSHConnector.GMSHCreateFromScript(@"..\..\Examples\GMSHConnector\sphere3D.geo", ".inp", "Элементы3D");
            CompareContentOfFiles(@"..\..\Examples\GMSHConnector\sphere3D.inp",
                                  @"..\..\Examples\GMSHConnector\expected3D.inp");
            GMSHConnector.Dispose();
        }

        [TestMethod]
        public void TestScriptSTPConnector1D()
        {
            GMSHConnector.Init();
            GMSHConnector.GMSHCreateFromScript(@"..\..\Examples\GMSHConnector\model11_3_stp1D.stp", ".inp", "Элементы1D");
            CompareContentOfFiles(@"..\..\Examples\GMSHConnector\model11_3_stp1D.inp",
                                  @"..\..\Examples\GMSHConnector\expected_model11_3_stp1D.inp");
            GMSHConnector.Dispose();
        }

        [TestMethod]
        public void TestScriptSTPConnector2D()
        {
            GMSHConnector.Init();
            GMSHConnector.GMSHCreateFromScript(@"..\..\Examples\GMSHConnector\model11_3_stp2D.stp", ".inp", "Элементы2D");
            CompareContentOfFiles(@"..\..\Examples\GMSHConnector\model11_3_stp2D.inp",
                                  @"..\..\Examples\GMSHConnector\expected_model11_3_stp2D.inp");
            GMSHConnector.Dispose();
        }

        [TestMethod]
        public void TestScriptSTPConnector3D()
        {
            GMSHConnector.Init();
            GMSHConnector.GMSHCreateFromScript(@"..\..\Examples\GMSHConnector\model11_3_stp3D.stp", ".inp", "Элементы3D");
            CompareContentOfFiles(@"..\..\Examples\GMSHConnector\model11_3_stp3D.inp",
                                  @"..\..\Examples\GMSHConnector\expected_model11_3_stp3D.inp");
            GMSHConnector.Dispose();
        }

        private string CuttedString(string path)
        {
            var lines = File.ReadAllLines(path);
            var result = lines.Skip(2).Aggregate(new StringBuilder(1000),(s, v) => s.Append(v), r => r.ToString());
            return result;
        }

        private void CompareContentOfFiles(string actualPath, string expectedPath)
        {
            var expected = CuttedString(expectedPath);
            var actual = CuttedString(actualPath);
            Assert.AreEqual(expected, actual);
        }

    }
}
