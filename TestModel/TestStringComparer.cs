using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GroupsData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TestModel
{
    [TestClass]
    public class TestStringComparer
    {
        [TestMethod]
        [DataRow("a", "A")]
        [DataRow("W1", "WA")]
        [DataRow("W1", "Wa")]
        [DataRow("W1_b", "W1_a")]
        [DataRow("Wa_1", "Wa")]
        public void PositiveComparationResult(string x, string y)
        {
            var xGroup = new Group(x, ModelInterfaces.ObjType.Элемент1D);
            var yGroup = new Group(y, ModelInterfaces.ObjType.Элемент1D);
            var comparator = new StringNumberGroupComparer();
            Assert.AreEqual(1, comparator.Compare(xGroup, yGroup));
        }

        [TestMethod]
        [DataRow("w", "w")]
        [DataRow("W1", "W1")]
        [DataRow("wa", "wa")]
        [DataRow("1", "1")]
        public void NeutralComparationResult(string x, string y)
        {
            var xGroup = new Group(x, ModelInterfaces.ObjType.Элемент1D);
            var yGroup = new Group(y, ModelInterfaces.ObjType.Элемент1D);
            var comparator = new StringNumberGroupComparer();
            Assert.AreEqual(0, comparator.Compare(xGroup, yGroup));
        }

        [TestMethod]
        [DataRow(null, null)]
        [DataRow("name", null)]
        [DataRow(null, "")]
        
        public void ShouldFail_NullReferenceException(string x, string y)
        {
            var xGroup = new Group(x, ModelInterfaces.ObjType.Элемент1D);
            var yGroup = new Group(y, ModelInterfaces.ObjType.Элемент1D);
            var comparator = new StringNumberGroupComparer();
            Assert.ThrowsException<NullReferenceException>(() =>  comparator.Compare(xGroup, yGroup));
        }

        [TestMethod]
        [DataRow("", "")]
        public void ShoulFail_ArgumentNullException(string x, string y)
        {
            var xGroup = new Group(x, ModelInterfaces.ObjType.Элемент1D);
            var yGroup = new Group(y, ModelInterfaces.ObjType.Элемент1D);
            var comparator = new StringNumberGroupComparer();
            Assert.ThrowsException<ArgumentNullException>(() => comparator.Compare(xGroup, yGroup));
        }
    }
}
