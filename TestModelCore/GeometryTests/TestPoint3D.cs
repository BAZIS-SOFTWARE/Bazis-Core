using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeometryTests
{
    [TestClass]
    public class TestPoint3D
    {
        private bool ComparePointsHash(float[] p1, float[] p2)
        {
            var hash1 = new Point3D(p1[0], p1[1], p1[2]).GetHashCode();
            var hash2 = new Point3D(p2[0], p2[1], p2[2]).GetHashCode();
            return hash1 == hash2;
        }

        [DataTestMethod]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                1.5f, 1, 0,
                5, 0, 0,
                6, 1, 0 
            },
            new int[]{0,1,3,2}
            )]
        [DataRow(
            new float[]
            {
                1,1.5f, 0,
                0.5f, 2, 0,
                -1, -1, 0,
                -2, 1.5f, 0,
                1,-1,0,
                -1,2.5f,0
            },
            new int[] { 0, 1, 5, 3,2,4 }
            )]

        public void RoundNodeList(float[] ar, int[]an)
        {

            //assing
            var ini_points = new List<Point3D>();
            for (int i = 0; i < ar.Length; i += 3)
            {
                ini_points.Add(new Point3D(ar[i], ar[i + 1], ar[i + 2]));
            }

            //act
            var sort_points = new List<Point3D>();
            for (int i = 0; i < an.Length; i++)
                sort_points.Add(ini_points[i]);

            sort_points.RoundSort();

            //assert
            for (int i = 0; i < an.Length; i++)
            {
                Assert.AreEqual(sort_points[i], ini_points[an[i]]);
            }
            
        }

        
        [TestMethod]
        public void SortByDistance()
        {
            //arrange
            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 2.5f, 0),
                new Point3D(5, 5, 0),
                new Point3D(5, 0, 0),
                new Point3D(5, 2.5f, 0)

            };

            //act point on straight line
            points.SortByDistance();

            //assert
            var comparePoints = new Point3D[]
            {
                new Point3D(0, 0, 0),
                new Point3D(5, 0, 0),
                new Point3D(5, 2.5f, 0),
                new Point3D(5, 5, 0),
                new Point3D(10, 2.5f, 0)
            };
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] != comparePoints[i])
                    throw new Exception();
            }
        }

        [TestMethod]
        public void TestPointsEqualityInList()
        {
            //arrange
            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 2.5f, 0),
                new Point3D(5, 5, 0),
                new Point3D(5, 5, 0),
                new Point3D(-15, 0, 0),
                new Point3D(-5, -2.5f, 0),
                new Point3D(-15, -2.5f, 10),
                new Point3D(59003.5f, 12318.13f,    52106.64f),
                new Point3D(59003.5039f, 12318.13f,    52106.64f)
            };

            //act point on straight line
            var distPoints = new List<Point3D>();

            foreach (var point in points)
            {
                if (!distPoints.Contains(point))
                    distPoints.Add(point);
            }

            //assert

            Assert.AreEqual(distPoints.Count, 8);            
        }

        [TestMethod]
        public void TestPointsEqualityInDict()
        {
            //arrange
            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 2.5f, 0),
                new Point3D(5, 5, 0),
                new Point3D(5, 5, 0),
                new Point3D(-15, 0, 0),
                new Point3D(-5, -2.5f, 0),
                new Point3D(-15, -2.5f, 10)
            };

            //act point on straight line
            var distPoints = new Dictionary<Point3D,int>();

            var counter = 0;
            foreach (var point in points)
            {
                if (!distPoints.ContainsKey(point))
                    distPoints.Add(point,counter++);
            }

            //assert

            Assert.AreEqual(distPoints.Count, 6);
        }

        [TestMethod]
        public void TestHeshFunction()
        {
            //arrange
            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 0),
                new Point3D(0, 0.0001f, 0),
                new Point3D(5, 5, 0),
                new Point3D(5, 5.0001f, 0),
            };

            //act point on straight line
            var heshCodes = new List<int>();

            foreach (var point in points)
                heshCodes.Add(point.GetHashCode());

            //assert

            Assert.AreEqual(new Point3D(0, 0, 0).GetHashCode(), new Point3D(0, 0.0001f, 0).GetHashCode());

            Assert.AreNotEqual(new Point3D(0, 0, 0).GetHashCode(), new Point3D(5, 5, 0).GetHashCode());

            Assert.AreEqual(heshCodes.Distinct().Count(), 2);
        }

        [TestMethod]
        public void GetDistanceToLineInCentre()
        {
            //arrange
            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 2.5f, 0),
                new Point3D(5, 5, 0),
                new Point3D(-15, 0, 0),
                new Point3D(-5, -2.5f, 0),
                new Point3D(-15, -2.5f, 10)
            };

            //act point on straight line
            var line = new Segment3D(new Point3D(0, 0, 0), new Point3D(10, 0, 0));

            //assert
            var distances = new float[]
            {
                0,
                2.5f,
                5,
                0,
                2.5f,
                10.3f
            };
            for (int i = 0; i < points.Count; i++)
            {
                var dist = points[i].GetDistanceToLine(line);
                Assert.AreEqual(dist, distances[i], 0.1);
            }
        }

        [TestMethod]
        public void GetDistanceToLineShifted()
        {
            //arrange
            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 0)
            };

            //act point on straight line
            var line = new Segment3D(new Point3D(10, 0, 0), new Point3D(20, 10, 10));

            //assert
            var distances = new float[]
            {
                8.1649f
            };
            for (int i = 0; i < points.Count; i++)
            {
                var dist = points[i].GetDistanceToLine(line);
                Assert.AreEqual(dist, distances[i], 0.01);
            }
        }

        [TestMethod]
        //x cord
        [DataRow(new float[] { 1f, 1f, 1f }, new float[] { 1f, -1f, -1f })]
        [DataRow(new float[] { -1f, 1f, 1f }, new float[] { -1f, -1f, -1f })]
        [DataRow(new float[] { 0f, 1f, 1f }, new float[] { 0f, -1f, -1f })]
        //y cord
        [DataRow(new float[] { 1f, 1f, 1f }, new float[] { -1f, 1f, -1f })]
        [DataRow(new float[] { 1f, -1f, 1f }, new float[] { -1f, -1f, -1f })]
        [DataRow(new float[] { 1f, 0f, 1f }, new float[] { -1f, 0f, -1f })]
        //z cord
        [DataRow(new float[] { 1f, 1f, 1f }, new float[] { -1f, -1f, 1f })]
        [DataRow(new float[] { 1f, 1f, -1f }, new float[] { -1f, -1f, -1f })]
        [DataRow(new float[] { 1f, 1f, 0f }, new float[] { -1f, -1f, 0f })]
        public void GetHashCode_HashShouldntBeEqual_WhenPointsHaveOnlyOneSameCord(float[] p1, float[] p2) =>
            Assert.IsFalse(ComparePointsHash(p1, p2));

        [TestMethod]
        [DataRow(new float[] { 0.0005f, 0.0005f, 0.0005f }, new float[] { 0f, 0f, 0f })]
        [DataRow(new float[] { -0.0005f, 0.0005f, 0.0005f }, new float[] { 0f, 0f, 0f })]
        [DataRow(new float[] { 0.0005f, -0.0005f, 0.0005f }, new float[] { 0f, 0f, 0f })]
        [DataRow(new float[] { 0.0005f, 0.0005f, -0.0005f }, new float[] { 0f, 0f, 0f })]
        [DataRow(new float[] { -0.0005f, -0.0005f, 0.0005f }, new float[] { 0f, 0f, 0f })]
        [DataRow(new float[] { 0.0005f, -0.0005f, -0.0005f }, new float[] { 0f, 0f, 0f })]
        [DataRow(new float[] { -0.0005f, 0.0005f, -0.0005f }, new float[] { 0f, 0f, 0f })]
        [DataRow(new float[] { -0.0005f, -0.0005f, -0.0005f }, new float[] { 0f, 0f, 0f })]
        public void GetHashCode_HashShouldBeEqual_WhenPointsClose(float[] p1, float[] p2) =>
            Assert.IsTrue(ComparePointsHash(p1, p2));

        [TestMethod]
        [DataRow(new float[] { 1f, 1f, 1f }, new float[] { -1f, -1f, -1f })]
        [DataRow(new float[] { -1f, 1f, 1f }, new float[] { 1f, -1f, -1f })]
        [DataRow(new float[] { 1f, -1f, 1f }, new float[] { -1f, 1f, -1f })]
        [DataRow(new float[] { 1f, 1f, -1f }, new float[] { -1f, -1f, 1f })]
        [DataRow(new float[] { -1f, -1f, 1f }, new float[] { 1f, 1f, -1f })]
        [DataRow(new float[] { 1f, -1f, -1f }, new float[] { -1f, 1f, 1f })]
        [DataRow(new float[] { -1f, 1f, -1f }, new float[] { 1f, -1f, 1f })]
        [DataRow(new float[] { -1f, -1f, -1f }, new float[] { 1f, 1f, 1f })]
        public void GetHashCode_HashShouldntBeEqual_WhenPointsOppositeBySign(float[] p1, float[] p2) =>
            Assert.IsFalse(ComparePointsHash(p1, p2));

        [TestMethod]
        // integers
        [DataRow(new[] { 10f, 10f, 10f }, new[] { 0f, 0f, 0f })]
        [DataRow(new[] { -10f, 10f, 10f }, new[] { 0f, 0f, 0f })]
        [DataRow(new[] { 10f, -10f, 10f }, new[] { 0f, 0f, 0f })]
        [DataRow(new[] { 10f, 10f, -10f }, new[] { 0f, 0f, 0f })]
        [DataRow(new[] { -10f, -10f, 10f }, new[] { 0f, 0f, 0f })]
        [DataRow(new[] { 10f, -10f, -10f }, new[] { 0f, 0f, 0f })]
        [DataRow(new[] { -10f, 10f, -10f }, new[] { 0f, 0f, 0f })]
        [DataRow(new[] { -10f, -10f, -10f }, new[] { 0f, 0f, 0f })]
        // floats
        [DataRow(new[] { 10.9994f, 10.0001f, 10.5643f }, new[] { -0.1111f, 0.5555f, 0.9999f })]
        [DataRow(new[] { -10.9994f, 10.0001f, 10.5643f }, new[] { -0.1111f, 0.5555f, 0.9999f })]
        [DataRow(new[] { 10.9994f, -10.0001f, 10.5643f }, new[] { -0.1111f, 0.5555f, 0.9999f })]
        [DataRow(new[] { 10.9994f, 10.0001f, -10.5643f }, new[] { -0.1111f, 0.5555f, 0.9999f })]
        [DataRow(new[] { -10.9994f, -10.0001f, 10.5643f }, new[] { -0.1111f, 0.5555f, 0.9999f })]
        [DataRow(new[] { 10.9994f, -10.0001f, -10.5643f }, new[] { -0.1111f, 0.5555f, 0.9999f })]
        [DataRow(new[] { -10.9994f, 10.0001f, -10.5643f }, new[] { -0.1111f, 0.5555f, 0.9999f })]
        [DataRow(new[] { -10.9994f, -10.0001f, -10.5643f }, new[] { -0.1111f, 0.5555f, 0.9999f })]
        public void GetHashCode_HashShouldntBeEqual_WhenPointsFar(float[] p1, float[] p2) =>
            Assert.IsFalse(ComparePointsHash(p1, p2));

        [TestMethod]
        [DataRow(new float[] { 1f, 0f, -1f }, new float[] { 0, -1f, 1f })]
        [DataRow(new float[] { 10.125f, 0.995f, -1.675f }, new float[] { 0.995f, -1.675f, 10.125f })]
        [DataRow(new float[] { 0.51f, 0.52f, 0.53f }, new float[] { 0.53f, 0.51f, 0.52f })]
        [DataRow(new float[] { -0.01f, -0.02f, -0.03f }, new float[] { -0.01f, -0.03f, -0.02f })]
        public void GetHashCode_HashShouldntBeEqual_WhenPointsSwitchXYZCords(float[] p1, float[] p2) =>
            Assert.IsFalse(ComparePointsHash(p1, p2));
    }
}
