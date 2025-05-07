
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Geometry;
using System;

namespace PrTests
{
    [TestClass]
    public class TestFrame
    {
        [TestMethod]
        public void YAxisRotation()
        {
            //arrange
            var frame = new Frame(
                new Point3D(0, 0, 0),
                new Point3D(0, 1, 0),
                new Point3D(1, 0, 0)
                );

            //act points on surface

                var frameRot = frame.Rotate(new Point3D(0, 1, 0), 45);
            
            //assert
            var frameRes = new Frame(
                new Point3D(0.7071f, 0, -0.7071f),
                new Point3D(0.7071f, 0, 0.7071f),
                new Point3D(0, 1, 0)
                //new Point3D(0, 0, 0)
                );

            if (frameRot.Dir_X != frameRes.Dir_X)
                throw new Exception();
            if (frameRot.Dir_Y != frameRes.Dir_Y)
                throw new Exception();
            if (frameRot.Dir_Z != frameRes.Dir_Z)
                throw new Exception();
        }

        [TestMethod]
        public void FrameCreation_ShouldFail_When()
        {
            var emptyVector = new Point3D();
            Assert.ThrowsException<Exception>( () => new Frame(emptyVector, emptyVector, emptyVector));
        }

        [TestMethod]
        public void FrameCreation_ShouldFail_WhenEmptyAxis()
        {
            var emptyVector = new Point3D();
            Assert.ThrowsException<Exception>(() => new Frame(emptyVector, emptyVector, emptyVector));
        }

        [DataRow(
            new[] { 1f, 2f, 3f },
            new[] { 1f, 2f, 3f },
            new[] { 1f, 2f, 3f })]
        [DataRow(
            new[] { -1f, -2f, -3f },
            new[] { -1f, -2f, -3f },
            new[] { -1f, -2f, -3f })]
        [DataRow(
            new[] { 1.9995f, 2.0005f, 3.3333f },
            new[] { 1.9995f, 2.0005f, 3.3333f },
            new[] { 1.9995f, 2.0005f, 3.3333f })]
        [DataRow(
            new[] { -1.9995f, 2.0005f, -3.3333f },
            new[] { -1.9995f, 2.0005f, -3.3333f },
            new[] { -1.9995f, 2.0005f, -3.3333f })]
        [DataRow(
            new[] { 0.0f, 0.0f, 0.0f },
            new[] { 0.0f, 0.0f, 0.0f },
            new[] { 0.0f, 0.0f, 0.0f })]
        [TestMethod]
        public void FrameCreation_ShouldFail_WhenSameAxis(float[] p1, float[] p2, float[] p3)
        {
            var axisX = new Point3D(p1[0], p1[1], p1[2]);
            var axisY = new Point3D(p2[0], p2[1], p2[2]);
            var axisZ = new Point3D(p3[0], p3[1], p3[2]);
            var center = new Point3D();

            Assert.ThrowsException<Exception>(() => new Frame(center, axisY,axisX));
        }

        [DataRow(
            new[] { 1f, 2f, 3f },
            new[] { 1f, 2f, 3f },
            new[] { 1f, 2f, 3f })]
        [DataRow(
            new[] { -1f, -2f, -3f },
            new[] { -1f, -2f, -3f },
            new[] { -1f, -2f, -3f })]
        [DataRow(
            new[] { 1.9995f, 2.0005f, 3.3333f },
            new[] { 1.9995f, 2.0005f, 3.3333f },
            new[] { 1.9995f, 2.0005f, 3.3333f })]
        [DataRow(
            new[] { -1.9995f, 2.0005f, -3.3333f },
            new[] { -1.9995f, 2.0005f, -3.3333f },
            new[] { -1.9995f, 2.0005f, -3.3333f })]
        [DataRow(
            new[] { 0.0f, 0.0f, 0.0f },
            new[] { 0.0f, 0.0f, 0.0f },
            new[] { 0.0f, 0.0f, 0.0f })]
        [TestMethod]
        public void FrameCreation_ShouldFail_SamePoints(float[] p1, float[] p2, float[] p3)
        {
            var axisX = new Point3D(p1[0], p1[1], p1[2]);
            var axisY = new Point3D(p2[0], p2[1], p2[2]);
            var axisZ = new Point3D(p3[0], p3[1], p3[2]);

            Assert.ThrowsException<Exception>(() => new Frame(axisX, axisY, axisZ));
        }

        [DataRow(
            new[] { 0.5f, 0.2f, 0.9f },
            new[] { 1.2f, 2.3f, 4.8f },
            new[] { 0.5f, 0.2f, 0.9f })]
        [DataRow(
            new[] { 1.2f, 2.3f, 3.4f },
            new[] { 1.2f, 2.3f, 3.4f },
            new[] { 0.0f, 0.0f, 0.0f })]
        [DataRow(
            new[] { -1.2f, -2.3f, -3.4f },
            new[] { -1.2f, -2.3f, -3.4f },
            new[] { 0.0f, 0.0f, 0.0f })]
        [DataRow(
            new[] { -1.2f, 2.3f, -3.4f },
            new[] { -1.2f, 2.3f, -3.4f },
            new[] { 0.0f, 0.0f, 0.0f })]
        [TestMethod]
        public void FrameCreation_ShouldFail_WhenTwoSamePoints(float[] p1, float[] p2, float[] p3)
        {
            var axisX = new Point3D(p1[0], p1[1], p1[2]);
            var axisY = new Point3D(p2[0], p2[1], p2[2]);
            var axisZ = new Point3D(p3[0], p3[1], p3[2]);

            Assert.ThrowsException<Exception>(() => new Frame(axisX, axisY, axisZ));
        }

        [DataRow(
             new[] { 0.5f, 0.2f, 0.9f },
             new[] { 1.2f, 2.3f, 4.8f },
             new[] { 0.5f, 0.2f, 0.9f })]
        [DataRow(
             new[] { 1.2f, 2.3f, 3.4f },
             new[] { 1.2f, 2.3f, 3.4f },
             new[] { 0.0f, 0.0f, 0.0f })]
        [DataRow(
             new[] { -1.2f, -2.3f, -3.4f },
             new[] { -1.2f, -2.3f, -3.4f },
             new[] { 0.0f, 0.0f, 0.0f })]
        [DataRow(
             new[] { -1.2f, 2.3f, -3.4f },
             new[] { -1.2f, 2.3f, -3.4f },
             new[] { 0.0f, 0.0f, 0.0f })]
        [TestMethod]
        public void FrameCreation_ShouldFail_WhenTwoSameAxis(float[] p1, float[] p2, float[] p3)
        {
            var axisX = new Point3D(p1[0], p1[1], p1[2]);
            var axisY = new Point3D(p2[0], p2[1], p2[2]);
            var axisZ = new Point3D(p3[0], p3[1], p3[2]);
            var center = new Point3D();

            Assert.ThrowsException<Exception>(() => new Frame(center, axisY, axisX));
        }

        [DataRow(
             new[] { -1f, 0f, 0f },
             new[] { 0f, -1f, 0f },
             new[] { 0f, 0f, -1f })]
        [DataRow(
             new[] { 1f, 0f, 0f },
             new[] { 0f, 1f, 0f },
             new[] { 0f, 0f, 1f })]
        [TestMethod]
        public void FrameCreation_ShouldPass_WhenCorrectAxisInput(float[] p1, float[] p2, float[] p3)
        {
            var axisX = new Point3D(p1[0], p1[1], p1[2]);
            var axisY = new Point3D(p2[0], p2[1], p2[2]);
            var axisZ = new Point3D(p3[0], p3[1], p3[2]);
            var center = new Point3D();
            new Frame(center, axisY, axisX);
        }

        [DataRow(
             new[] { -1.2f, 4.3f, -8.412f },
             new[] { -0.992f, 11.3f, -3.4f },
             new[] { 1.0f, 0.12f, 0.33f })]
        [DataRow(
             new[] { 11f, 22f, 33f },
             new[] { 12f, 23f, 34f },
             new[] { 5f, 6f, 7f })]
        [DataRow(
             new[] { -11f, 22f, 33f },
             new[] { 12f, -23f, 34f },
             new[] { 5f, 6f, -7f })]
        [TestMethod]
        public void FrameCreation_ShouldPass_WhenCorrectPointInput(float[] p1, float[] p2, float[] p3)
        {
            var axisX = new Point3D(p1[0], p1[1], p1[2]);
            var axisY = new Point3D(p2[0], p2[1], p2[2]);
            var axisZ = new Point3D(p3[0], p3[1], p3[2]);
            new Frame(axisX, axisY, axisZ);
        }

        [DataRow(
             new[] { -1f, 0f, 0f },
             new[] { 0f, -1f, 0f },
             new[] { 0f, 0f, -1f },
             new[] { 0f, 0f, 0f})]
        [DataRow(
             new[] { 1f, 0f, 0f },
             new[] { 0f, 1f, 0f },
             new[] { 0f, 0f, 1f },
             new[] { 0f, 0f, 0f })]
        [TestMethod]
        public void FrameCreation_ShouldPass_WhenBaseNotNullable(float[] p1, float[] p2, float[] p3, float[] c)
        {
            var axisX = new Point3D(p1[0], p1[1], p1[2]);
            var axisY = new Point3D(p2[0], p2[1], p2[2]);
            var axisZ = new Point3D(p3[0], p3[1], p3[2]);
            var cen = new Point3D(c[0], c[1], c[2]);
            new Frame(cen, axisY, axisX);
        }
        [TestMethod]
        [DataRow( new[] { 0.0f, 0.0f, 0.0f })]
        [DataRow(new[] { 1.0f, 0.0f, 0.0f })]
        public void FrameCreation_ShouldPass_WhenShifting(float[] points)
        {
            var shifting = new Point3D(points[0], points[1], points[2]);
            var frame = new Frame(new Point3D(),new Point3D(0, 1, 0), new Point3D(1, 0, 0));
            frame.Shift(shifting);
        }

        [TestMethod]
        public void FrameCreation_ShouldPass_OnTestCase24_09_24()
        {
            var posit = new Point3D(59005.25f, 12276.25f, 52050.02f);
            var dirY = new Point3D(0, 0, -2.539063f);
            var dirX = new Point3D(-1.75f, 1.75f, 0);
            var shifting = new Point3D(0, 0, 0);

            var frame = new Frame(posit, posit.Sum(dirY), posit.Sum(dirX));
            var sFrame = frame.Shift(shifting);
            var axis = sFrame.Dir_Y.Sub(sFrame.Centre);
            axis = Vector.GetVectorNorm(axis);
            var res = sFrame.Rotate(axis, 0);
        }
    }
}
