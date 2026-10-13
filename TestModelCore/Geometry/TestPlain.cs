using Microsoft.VisualStudio.TestTools.UnitTesting;
using Geometry;
using System;

namespace TestModeCore_test.Geometry
{
    [TestClass]
    public class TestPlain
    {
        [DataTestMethod]
        [DataRow(
            new float[]
            {
                1, 1, 1, 0,
                -2.5f,1.5f,0.862069f,
                -0.07963449997959426f
            })]
        [DataRow(
            new float[]
            {
                1, 1, 1, 0,
                -2.5f,5.5f,0.862069f,
                2.2297665767789088f
            })]
        [DataRow(
            new float[]
            {
                10, 1, 1, 0,
                -2.5f,1.5f,0.862069f,
                -2.2414891757725486f
            })]
        [DataRow(
            new float[]
            {
                10, 1, 1, -10,
                -2.5f,1.5f,0.862069f,
                -3.231636718749223f
            })]

        public void CalcPointPositToPlane(float[] ar)
        {
            //arrange
            var plane = new Plane(new Point3D(ar[0], ar[1], ar[2]), ar[3]);

            //act
            var res = plane.PointPosition(new Point3D(ar[4], ar[5], ar[6]));
            //assert
            Assert.AreEqual(ar[7], res, 0.001f);
        }

        [TestMethod]
        public void GetDistanceToParallelPlane()
        {
            //arrange
            var points0 = new Point3D[]
            {
                new Point3D(1, 0, 0),
                new Point3D(0, 1, 0),
                new Point3D(0, 0, 0)
            };

            var plane0 = new Plane(points0[0], points0[1], points0[2]);

            var points1 = new Point3D[]
            {
                new Point3D(1, 0, -5),
                new Point3D(0, 1, -5),
                new Point3D(0, 0, -5)
            };

            var plane1 = new Plane(points1[0], points1[1], points1[2]);
            //act
            var dist = Math.Abs(plane0.PlanePosition(plane1));
            //assert
            Assert.AreEqual(dist, 5);
        }

        [DataTestMethod]
        [DataRow(
            new float[] 
            { 
                -1, 0, 0, 0, 2, 0,0,0,3,
                6,- 3,- 2,6
            })]
        [DataRow(
            new float[]
            {
                -1.05f, 0, 0, 0, 20, 0,0,0,0.3f,
                6,- 0.315f,- 21,6.3f
            })]
        public void SetPlainByPoints(float[] ar)
        {
            //arrange
            var points = new Point3D[]
            {
                new Point3D(ar[0], ar[1], ar[2]),
                new Point3D(ar[3], ar[4],ar[5]),
                new Point3D(ar[6], ar[7], ar[8])
            };

            //act
            var plane = new Plane(points[0], points[1], points[2]);

            //assert
            Assert.AreEqual(plane.Normal, new Point3D(ar[9], ar[10], ar[11]));
            Assert.IsTrue(Math.Abs(plane.Shifting - ar[12]) < 1e-4);
        }

        [DataTestMethod]
        [DataRow(
            new float[]
            {
                -1.05f, 1, 0, 0, 1, 0,
                0,1,0,-1
            })]
        [DataRow(
            new float[]
            {
                -1.05f, 1, 0, 10, 1, 0,
                10,1,0,9.5f
            })]
        public void SetPlainByPointAndNormal(float[] ar)
        {
            //arrange
            var point = new Point3D(ar[0], ar[1], ar[2]);
            var normal = new Point3D(ar[3], ar[4], ar[5]);

            //act
            var plane = new Plane(point, normal);

            //assert
            Assert.AreEqual(plane.Normal, new Point3D(ar[6], ar[7], ar[8]));
            Assert.IsTrue(Math.Abs(plane.Shifting - ar[9]) < 1e-4);
        }
    }
}
