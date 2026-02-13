using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.GeometryObjects;
using Model.GroupsData;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ModelTests
{
    [TestClass]
    public class TestGroupData
    {
        [TestMethod]

        public void CroupCreation()
        {
            var objs = new List<Node>() { new Node(0, new Point3D()), new Node(1, new Point3D()) };

            var grData = new GroupData();

            grData.Create(objs);

            Assert.AreNotEqual(grData.Count, 0);
        }

        [TestMethod]

        public void SeveralCroupsCreation()
        {
            var objs = new List<ModelObject>() { new Node(0, new Point3D()), new Node(1, new Point3D()) };

            var grData = new GroupData();

            grData.Create(objs);
            grData.Create(objs);

            Assert.AreNotEqual(grData[0].Name, grData[1].Name);
        }

        [TestMethod]
        [DataRow("a", "A")]
        [DataRow("WA", "W1")]
        [DataRow("Wa", "W1")]
        [DataRow("W1_b", "W1_a")]
        [DataRow("Wa_1", "Wa")]
        [DataRow("name", null)]
        [DataRow("A111", "A2")]

        public void PositiveComparationResult(string x, string y)
        {
            var xGroup = new Group(x, Model.Interfaces.ObjType.Элемент1D);
            var yGroup = new Group(y, Model.Interfaces.ObjType.Элемент1D);
            var comparator = new StringNumberGroupComparer();
            Assert.AreEqual(1, comparator.Compare(xGroup, yGroup));
        }

        [TestMethod]
        [DataRow("", null)]
        [DataRow("", "")]
        [DataRow(null, null)]
        [DataRow("w", "w")]
        [DataRow("W1", "W1")]
        [DataRow("wa", "wa")]
        [DataRow("1", "1")]
        public void NeutralComparationResult(string x, string y)
        {
            var xGroup = new Group(x, Model.Interfaces.ObjType.Элемент1D);
            var yGroup = new Group(y, Model.Interfaces.ObjType.Элемент1D);
            var comparator = new StringNumberGroupComparer();
            Assert.AreEqual(0, comparator.Compare(xGroup, yGroup));
        }

        [TestMethod]
        [DataRow(new[] { "A", "a", "A1", "a1", "W1", "A111", "A2" },
            new[] { "A", "a", "A1", "a1", "A2", "A111", "W1" })]
        [DataRow(new[] { "b_01", "a", "a_1", "A1", "A2_b", "A111", "A" },
            new[] { "A", "a", "a_1", "A1", "A2_b", "A111", "b_01" })]
        [DataRow(new[] { "A_1", "A_b", "A1", "A_S", "A_s", "A2", "A2" },
            new[] { "A_1", "A_b", "A_S", "A_s", "A1", "A2", "A2" })]
        public void CheckGroupSortWithComparator(string[] input, string[] expected)
        {
            var groupData = new GroupData();
            var counter = 0;
            foreach (var name in input)
                groupData.Add(new Group(counter++,name, Model.Interfaces.ObjType.Элемент1D));

            groupData.Sort();
            CollectionAssert.AreEqual(expected, groupData.Select(x => x.Name).ToArray());
        }
    }
}
