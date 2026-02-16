using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeometryTests
{
    [TestClass]
    public class TestPoint2D
    {
        private bool ComparePointsHash(float[] p1, float[] p2)
        {
            var hash1 = new Point2D(p1[0], p1[1]).GetHashCode();
            var hash2 = new Point2D(p2[0], p2[1]).GetHashCode();
            return hash1 == hash2;
        }

        [TestMethod]
        public void FindMinMax()
        {
            //assign
            var range = new List<Point2D>()
            {
                new Point2D(0.001f, 2.5f),
                new Point2D(0.01f, 2.51f),
                new Point2D(-0.01f, 2.5005f),
            };
            //act

            var xMax = range.Max(x => x._x);
            var xMin = range.Min(x => x._x);
            var yMax = range.Max(x => x._y);
            var yMin = range.Min(x => x._y);

            //assert
            Assert.AreEqual(xMin, -0.01f);
            Assert.AreEqual(xMax, 0.01f);
            Assert.AreEqual(yMin, 2.5f);
            Assert.AreEqual(yMax, 2.51f);
        }
                
        [DataTestMethod]
        [DataRow(
            new float []
            {
                1, 2, 
                1.00003f, 2,                  
            }, true)]
        [DataRow(
            new float[]
            {
                -1, -2,
                -1.00003f, -2,               
            }, true)]
        [DataRow(
            new float[]
            {
                0.0001f, 2,
                -0.0001f, 2,                
            }, false)]
        [DataRow(
            new float[]
            {
                2, -0.0001f, 
                2, 0.0001f,               
            }, false)]
        [DataRow(
            new float[]
            {
                -0.00005f, 2,
                0.00005f, 2,                
            }, true)]
        [DataRow(
            new float[]
            {
               2, 0.00005f, 
               2, -0.00005f,                
            }, true)]
        [DataRow(
            new float[]
            {
               0, 0,
               0, 0,              
            }, true)]       
        public void ComparePoint(float[] ar, bool state)
        {
            //assign
            var point1 = new Point2D (ar[0],ar[1]);
            var point2 = new Point2D(ar[2], ar[3]);            

            //act
            var result = point1.Equals(point2);            

            //assert
            Assert.AreEqual(state, result);

        }

        [TestMethod]
        [DataRow(new float[] { 0.0005f, 0.0005f }, new float[] { 0, 0 })]
        [DataRow(new float[] { -0.0005f, 0.0005f }, new float[] { 0, 0 })]
        [DataRow(new float[] { 0.0005f, -0.0005f }, new float[] { 0, 0 })]
        [DataRow(new float[] { -0.0005f, -0.0005f }, new float[] { 0, 0 })]
        public void GetHashCode_HashShouldBeEqual_WhenPointsClose(float[] p1, float[] p2) => 
            Assert.IsTrue(ComparePointsHash(p1, p2));

        [TestMethod]
        [DataRow(new[] { 1f, 2f }, new[] { 0f, 0f })]
        [DataRow(new[] { -1f, 2f }, new[] { 0f, 0f })]
        [DataRow(new[] { 1f, -2f }, new[] { 0f, 0f })]
        [DataRow(new[] { -1f, -2f }, new[] { 0f, 0f })]
        public void GetHashCode_HashShouldntBeEqual_WhenPointsFar(float[] p1, float[] p2) =>
            Assert.IsFalse(ComparePointsHash(p1, p2));

        [TestMethod]
        [DataRow(new float[] { 1f, 1f }, new float[] { -1, -1f })]
        [DataRow(new float[] { -1f, 1f }, new float[] { 1, -1f })]
        [DataRow(new float[] { 1f, -1f }, new float[] { -1, 1f })]
        [DataRow(new float[] { -1f, -1f }, new float[] { 1, 1f })]
        public void GetHashCode_HashShouldntBeEqual_WhenPointsOppositeBySign(float[] p1, float[] p2) =>
            Assert.IsFalse(ComparePointsHash(p1, p2));

        [TestMethod]
        [DataRow(new float[] { 1f, 1f }, new float[] { 1f, -1f })]
        [DataRow(new float[] { -1f, 1f }, new float[] { -1f, -1f })]
        [DataRow(new float[] { 0f, 1f }, new float[] { 0f, -1f })]
        [DataRow(new float[] { 1f, 1f }, new float[] { -1f, 1f })]
        [DataRow(new float[] { 1f, -1f }, new float[] { -1f, -1f })]
        [DataRow(new float[] { 1f, 0f }, new float[] { -1f, 0f })]
        public void GetHashCode_HashShouldntBeEqual_WhenPointsHaveOnlyOneSameCord(float[] p1, float[] p2) =>
            Assert.IsFalse(ComparePointsHash(p1, p2));

        [TestMethod]
        [DataRow(new float[] { 1f, 0f }, new float[] { 0, 1f })]
        [DataRow(new float[] { 10f, 2f }, new float[] { 2, 10f })]
        [DataRow(new float[] { 0.005f, 2.225f }, new float[] { 2.225f, 0.005f })]
        public void GetHashCode_HashShouldntBeEqual_WhenPointsSwitchXYCords(float[] p1, float[] p2) =>
            Assert.IsFalse(ComparePointsHash(p1, p2));
    }
}
