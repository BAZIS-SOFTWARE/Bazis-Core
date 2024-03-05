using System;
using System.Linq;
using System.Reflection;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.MeshObjects;
using ModelController.MeshObjsCreator;

namespace TestModel
{
    [TestClass]
    public class TestMap3DFrom2D
    {
        [TestMethod]
        [DataRow(0f, 0f, 0f, 0f, 1f, 0f, 1f, 0f, 0f)]
        [DataRow(5f, 7f, 8f, 6f, 15f, 20f, 11f, 20f, 6f)]
        public void TestCalcCircumcenter(
            float x1, float y1, float z1,
            float x2, float y2, float z2,
            float x3, float y3, float z3)
        {
            Node[] nodes = new Node[3]
            {
                new Node(1, new Point3D( x1, y1, z1)),
                new Node(2, new Point3D( x2, y2, z2)), 
                new Node(3, new Point3D( x3, y3, z3)),
            };
            var triangle = new Triangle(4, nodes);

            var m = typeof(Sew3DFrom2D).GetMethod("CalcCircumcenter", BindingFlags.NonPublic | BindingFlags.Static);
            var circumcenter = m.Invoke(null, new object[] { triangle }) as Point3D;

            var r1 = circumcenter.Sub(nodes[0].Position).GetSquareSumCoords();
            var r2 = circumcenter.Sub(nodes[1].Position).GetSquareSumCoords();
            var r3 = circumcenter.Sub(nodes[2].Position).GetSquareSumCoords();
            Assert.IsTrue(Math.Abs(r1 - r2) < 10e-6);
            Assert.IsTrue(Math.Abs(r1 - r3) < 10e-6);
            Assert.IsTrue(Math.Abs(r3 - r2) < 10e-6);*/

            //assert
        }
    }
}