using Microsoft.VisualStudio.TestTools.UnitTesting;
using Geometry;
using System;
using Model.MeshObjects;

namespace TestModel
{
    [TestClass]
    public class TestCalcDistance
    {
        [DataTestMethod]
        [DataRow(new float[]
        {
            -50, 6, 0, 3, 56, 0,50, 6, 100,3, 56, 100,72.86277f
        })]
        [DataRow(new float[]
        {
            3, 56, 100, 50, 6, 0,50, 0, 0,-50, 0, 0,100
        })]
        [DataRow(new float[]
        {
            50, 6, 41.37931f, 3, 56, 0,50, 6, 0,50, 6, 100,0
        })]
        public void TestDistanceBetweenNodeAndPlaneCase_1(float[] array)
        {
            var node = new Node(0, new Point3D(array[0], array[1], array[2]));
            Node[] nodes = new Node[4];
            nodes[0] = new Node(1, new Point3D(array[3], array[4], array[5]));
            nodes[1] = new Node(1, new Point3D(array[6], array[7], array[8]));
            nodes[2] = new Node(1, new Point3D(array[9], array[10], array[11]));

            var plane = new Plane(nodes[0].Position, nodes[1].Position, nodes[2].Position);
            //act
            var calcDist = Math.Abs(plane.PointPosition(node.Position));
            var dist = Math.Round(calcDist, 5);
            var res = Math.Round(array[12], 5);
            Assert.AreEqual(dist, res);
        }
    }
}