using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GroupsData;
using Model.Interfaces;
using Model.MeshObjects;
using OperationalController.MeshObjsCreator;
using OperationalController.MeshObjsCreator.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestModeCore_test.OperationalController
{
    [TestClass]
    public class BeamGroupConnection_should
    {
        private static IEnumerable<Type> typesToTest;

        [ClassInitialize]
        public static void SetUp(TestContext testContext)
        {
            typesToTest = new List<Type>()
            {
                typeof(SimpleBeamGroupConnection),
                typeof(OctreeBeamGroupConnection)
            };
        }

        [TestMethod]
        [ExpectedException(typeof(CollectionEmptyArgumentException))]
        [DataRow(0, 0, DisplayName = "radius is zero")]
        [DataRow(1, 0, DisplayName = "radius is cube edge")]
        [DataRow(0.5, 0, DisplayName = "radius is less than cube edge")]
        [DataRow(2, 0, DisplayName = "radius is more than the largest cube diagonal")]
        public void Create_ShouldReturnEmptyEnumerable_WhenGroupsAreEmpty(double searchRadius, int expected)
        {
            foreach (var type in typesToTest)
            {
                var instance = GetInstanceOf(type);

                var master = new Group("master", ObjType.Узел) { };

                var slave = new Group("slave", ObjType.Узел) { };

                var beams = instance.Create(master.Cast<Node>(), slave.Cast<Node>(), searchRadius).ToList();

                Assert.AreEqual(expected, beams.Count);
            }
        }

        [TestMethod]
        [DataRow(1, 4, DisplayName = "radius is cube edge")]
        [DataRow(0.5, 0, DisplayName = "radius is less than cube edge")]
        [DataRow(2, 4, DisplayName = "radius is more than the largest cube diagonal")]
        public void Create_ShouldBeCorrect_OnIdentityCube_WhenBeamsCountNotSpecified(double searchRadius, int expected)
        {
            foreach(var type in typesToTest)
            {
                var instance = GetInstanceOf(type);

                var master = new Group("master", ObjType.Узел)
                    {
                        new Node(0, new Point3D(0, 0, 0)),
                        new Node(1, new Point3D(1, 0, 0)),
                        new Node(2, new Point3D(1, 1, 0)),
                        new Node(3, new Point3D(0, 1, 0))
                    };

                var slave = new Group("slave", ObjType.Узел)
                {
                    new Node(4, new Point3D(0, 0, 1)),
                    new Node(5, new Point3D(1, 0, 1)),
                    new Node(6, new Point3D(1, 1, 1)),
                    new Node(7, new Point3D(0, 1, 1))
                };

                var beams = instance.Create(master.Cast<Node>(), slave.Cast<Node>(), searchRadius).ToList();

                Assert.AreEqual(expected, beams.Count);
            }
        }

        [TestMethod]
        // не будем разрешать нулевой радиус [DataRow(0, 0, DisplayName = "zero radius, default incidence")]
        [DataRow(1, 1, DisplayName = "radius is cube edge")]
        [DataRow(0.5, 0, DisplayName = "radius is less than cube edge")]
        [DataRow(1.42, 3, DisplayName = "radius is the smallest cube diagonal")]
        [DataRow(1.74, 4, DisplayName = "radius is the largest cube diagonal")]
        public void Create_ShouldBeCorrect_OnDifferentSizeGroups_WhenBeamsCountNotSpecified(double searchRadius, int expected)
        {
            foreach (var type in typesToTest)
            {
                var instance = GetInstanceOf(type);

                var master = new Group("master", ObjType.Узел)
                {
                    new Node(0, new Point3D(0, 0, 0)),
                };

                var slave = new Group("slave", ObjType.Узел)
                {
                    new Node(4, new Point3D(0, 0, 1)),
                    new Node(5, new Point3D(1, 0, 1)),
                    new Node(6, new Point3D(1, 1, 1)),
                    new Node(7, new Point3D(0, 1, 1))
                };

                var beams = instance.Create(master.Cast<Node>(), slave.Cast<Node>(), searchRadius).ToList();

                Assert.AreEqual(expected, beams.Count);
            }
        }

        [TestMethod]
        [DataRow(0.5, 2, 0, DisplayName = "radius is less than cube edge, incidence is 2")]
        [DataRow(1, 2, 2, DisplayName = "radius is cube edge, incidence is 2")]
        [DataRow(1, 3, 2, DisplayName = "radius is cube edge, incidence is 3")]
        [DataRow(1.42, 1, 2, DisplayName = "radius is minimal cube diagonal, incidence is 1")]
        [DataRow(1.42, 2, 4, DisplayName = "radius is minimal cube diagonal, incidence is 2")]
        [DataRow(1.42, 3, 6, DisplayName = "radius is minimal cube diagonal, incidence is 3")]
        [DataRow(1.42, 4, 6, DisplayName = "radius is minimal cube diagonal, incidence is 4")]
        [DataRow(1.74, 4, 8, DisplayName = "radius is more than maximal cube diagonal, incidence is 4")]
        [DataRow(2, 4, 8, DisplayName = "radius is 2, incidence is 4")]
        public void Create_ShouldBeCorrect_OnDifferentSize_WhenBeamsCountSpecified(double searchRadius, int beamsCount, int expected)
        {
            foreach(var type in typesToTest)
            {
                var instance = GetInstanceOf(type);

                var master = new Group("master", ObjType.Узел)
                {
                    new Node(0, new Point3D(0, 0, 0)),
                    new Node(2, new Point3D(1, 1, 0))
                };

                var slave = new Group("slave", ObjType.Узел)
                {
                    new Node(4, new Point3D(0, 0, 1)),
                    new Node(5, new Point3D(1, 0, 1)),
                    new Node(6, new Point3D(1, 1, 1)),
                    new Node(7, new Point3D(0, 1, 1))
                };

                var beams = instance.Create(master.Cast<Node>(), slave.Cast<Node>(), searchRadius, beamsCount).ToList();

                Assert.AreEqual(expected, beams.Count);
            }
        }

        private BeamGroupConnection GetInstanceOf(Type type)
        {
            return (BeamGroupConnection)type.GetConstructor(new Type[] { }).Invoke(new object[] { });
        }
    }
}
