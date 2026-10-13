using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TestModeCore_test.Geometry
{
    [TestClass]
    public class TestSegment2D
    {
        public static IEnumerable<object[]> AdditionData
        {
            get
            {
                return new[]
                {
                    // перпендикулярные
                    new object[]
                    {
                        new Segment2D(new Point2D(1.125f, 0.625f),new Point2D(1.125f, -0.625f)),
                        new Segment2D(new Point2D(-0.625f, 0),new Point2D(2.625f, 0)),
                        true
                    },
                    new object[]
                    {
                        new Segment2D(new Point2D(-1, -1),new Point2D(1, 1)),
                        new Segment2D(new Point2D(-1, 1),new Point2D(1, -1)),
                        true
                    },
                    new object[]
                    {
                        new Segment2D(new Point2D(1.125f, 0.625f),new Point2D(1.125f, -0.625f)),
                        new Segment2D(new Point2D(2.625f, 0),new Point2D(0.625f, 0)),
                        true
                    },
                    // под углом
                    new object[]
                    {
                        new Segment2D(new Point2D(-2.5f, -1.25f),new Point2D(0, 0)),
                        new Segment2D(new Point2D(1.25f, -0.625f),new Point2D(-3.775f, 0.225f)),
                        true
                    },
                    new object[]
                    {
                        new Segment2D(new Point2D(-0.55f, -1.1f),new Point2D(0.55f, 1.1f)),
                        new Segment2D(new Point2D(0.55f, -1.1f),new Point2D(-0.55f, 1.1f)),
                        true
                    },
                    new object[]
                    {
                        new Segment2D(new Point2D(-0.55f, -1.1f),new Point2D(0.55f, 1.1f)),
                        new Segment2D(new Point2D(-0.55f, -1.1f),new Point2D(-0.55f, 1.1f)),
                        true
                    },
                    // параллельные и пересекаются
                    new object[]
                    {
                        new Segment2D(new Point2D(0, 5),new Point2D(1, 5)),
                        new Segment2D(new Point2D(0.5f, 5),new Point2D(-1, 5)),
                        false
                    },

                    new object[]
                    {
                        new Segment2D(new Point2D(0, 5),new Point2D(1, 5)),
                        new Segment2D(new Point2D(1, 5),new Point2D(3, 5)),
                        false
                    },
                    // параллельные и совпадают
                    new object[]
                    {
                        new Segment2D(new Point2D(-1, 0),new Point2D(1, 0)),
                        new Segment2D(new Point2D(-1, 0),new Point2D(1, 0)),
                        false
                    },
                    new object[]
                    {
                        new Segment2D(new Point2D(1.175f, 0.995f),new Point2D(11.125f, -99.675f)),
                        new Segment2D(new Point2D(1.175f, 0.995f),new Point2D(11.125f, -99.675f)),
                        false
                    },
                };
            }
        }


        [TestMethod]
        [DynamicData(nameof(AdditionData))]
        public void IsIntersectWith(Segment2D segOne, Segment2D segTwo,bool res)
        {
            //assert
            Assert.AreEqual(res,segOne.HasIntersectedPoint(segTwo));
        }


        [TestMethod]
        [DataRow(
            new float[] { -3.745f, 0, 1.125f, 0 },
            new float[] { -7.325f, -1.775f, 0, -1 })]
        [DataRow(
            new float[] { -1, 0.5f, 1, 1 },
            new float[] { -1.25f, -1, 9.15f, -2.25f })]
        [DataRow(
            new float[] { -1.125f, 0.5f, 1.125f, 0.5f },
            new float[] { 1, 0.505f, 1, 2.25f })]
        public void IsIntersectWith_ReturnsFalse_WhenNoIntersect(float[] segOne, float[] segTwo)
        {
            var segmentOne = new Segment2D(new Point2D(segOne[0], segOne[1]), new Point2D(segOne[2], segOne[3]));
            var segmentTwo = new Segment2D(new Point2D(segTwo[0], segTwo[1]), new Point2D(segTwo[2], segTwo[3]));
            Assert.IsFalse(segmentOne.HasIntersectedPoint(segmentTwo));
        }

        [TestMethod]
        [DataRow(
            new float[] { -3.745f, 0, 1.125f, 0 },
            new float[] { -7.325f, 0, 1.775f, 0 },
            false)]
        [DataRow(
            new float[] { -11.555f, 12.765f, -11.555f, -12.995f },
            new float[] { -11.556f, -12.995f, -11.556f, -14.785f },
            false)]
        [DataRow(
            new float[] { -14, -7, 14, 7},
            new float[] { -7, -3.5f, 7, 3.5f },
            false)]
        [DataRow(
            new float[] { -14, 7, 14, -7 },
            new float[] { -7, 3.5f, 7, -3.5f },
            false)]
        public void IsIntersectWith_ReturnsFalse_WhenParallel(float[] segOne, float[] segTwo,bool res)
        {
            var segmentOne = new Segment2D(new Point2D(segOne[0], segOne[1]), new Point2D(segOne[2], segOne[3]));
            var segmentTwo = new Segment2D(new Point2D(segTwo[0], segTwo[1]), new Point2D(segTwo[2], segTwo[3]));
            Assert.AreEqual(res,segmentOne.HasIntersectedPoint(segmentTwo));
        }

        [TestMethod]
        [DataRow(
            new float[] { 1.125f, 0.625f, 11.125f, 5.125f },
            new float[] { 1.125f, 0.625f, -2.625f, -9.995f })]
        [DataRow(
            new float[] { -1.115f, -12.85f, 1.15f, 1.15f },
            new float[] { 21.65f, 8.45f, 1.15f, 1.15f })]
        public void IsIntersectWith_ReturnsTrue_WhenIntersectOnesStartAndOthersEnd(float[] segOne, float[] segTwo)
        {
            var segmentOne = new Segment2D(new Point2D(segOne[0], segOne[1]), new Point2D(segOne[2], segOne[3]));
            var segmentTwo = new Segment2D(new Point2D(segTwo[0], segTwo[1]), new Point2D(segTwo[2], segTwo[3]));
            Assert.IsTrue(segmentOne.HasIntersectedPoint(segmentTwo));
        }

        [TestMethod]
        [DataRow(new float[] { 0, 0 },
                new float[] { 0, 1.25f },
                new float[] { 0, 0.125f },
                true)]
        [DataRow(new float[] { 0, 0 },
                new float[] { 0, 0.9998f },
                new float[] { 0, 0.9999f },
                false)]
        [DataRow(new float[] { 0.0005f, 0.0005f },
                new float[] { -0.0005f, -0.0005f },
                new float[] { 0.0001f, 0.0001f },
                true)]
        public void IsPointBelongsSegment_ReturnsTrue_WhenPointInsideSegment(float[] segStart, float[] segEnd, float[] testPoint, bool expected)
        {
            var segment = new Segment2D(new Point2D(segStart[0], segStart[1]), new Point2D(segEnd[0], segEnd[1]));
            Assert.AreEqual(expected, segment.IsPointBelongSegment(new Point2D(testPoint[0], testPoint[1])));
        }

        [TestMethod]
        [DataRow(
                new float[] { -1.125f, -1.125f },
                new float[] { 0, 0 },
                new float[] { -0.005f, -0.005f },
                true)]
        [DataRow(
                new float[] { -1, -1 },
                new float[] { 1, 1 },
                new float[] { 0, 0 },
                true)]
        [DataRow(
                new float[] { -1, 1 },
                new float[] { 1, -1 },
                new float[] { 0, 0 },
                true)]
        [DataRow(
                new float[] { -486.2834f, 104.7089f },
                new float[] { -493.2167f, 43.6507f },
                new float[] { -486.29f, 74 },
                true)]
        public void IsPointBelongsSegment_ReturnsTrue_WhenPointInsideDiagonalSegment(float[] segStart, float[] segEnd, float[] testPoint, bool expected)
        {
            var segment = new Segment2D(new Point2D(segStart[0], segStart[1]), new Point2D(segEnd[0], segEnd[1]));
            Assert.AreEqual(expected, segment.IsPointBelongSegment(new Point2D(testPoint[0], testPoint[1]),5));
        }

        [TestMethod]
        [DataRow(
                new float[] { -2.5f, -1.25f },
                new float[] { 0, 0 },
                new float[] { -0.1f, -0.05f },
                true)]
        [DataRow(
                new float[] { 1.25f, -0.625f },
                new float[] { -2, 1 },
                new float[] { -0.125f, 0.0625f },
                true)]
        public void IsPointBelongsSegment_ReturnsTrue_WhenPointInside30DegreesAngleSegment(float[] segStart, float[] segEnd, float[] testPoint, bool expected)
        {
            var segment = new Segment2D(new Point2D(segStart[0], segStart[1]), new Point2D(segEnd[0], segEnd[1]));
            Assert.AreEqual(expected, segment.IsPointBelongSegment(new Point2D(testPoint[0], testPoint[1])));
        }

        [TestMethod]
        [DataRow(
                new float[] { -1.25f, -2.5f },
                new float[] { 0, 0 },
                new float[] { -0.05f, -0.1f },
                true)]
        [DataRow(
                new float[] { 1.25f, -2.5f },
                new float[] { -2.5f, 5f },
                new float[] { 0.125f, -0.25f },
                true)]
        public void IsPointBelongsSegment_ReturnsTrue_WhenPointInside60DegreesAngleSegment(float[] segStart, float[] segEnd, float[] testPoint, bool expected)
        {
            var segment = new Segment2D(new Point2D(segStart[0], segStart[1]), new Point2D(segEnd[0], segEnd[1]));
            Assert.AreEqual(expected, segment.IsPointBelongSegment(new Point2D(testPoint[0], testPoint[1])));
        }

        [TestMethod]
        [DataRow(
                new float[] { 1.111f, 2.222f },
                new float[] { 1.111f, 2.236f },
                new float[] { 1.111f, 2.231f },
                true)]
        [DataRow(
                new float[] { -1.995f, 1.125f },
                new float[] { -1.995f, 2.725f },
                new float[] { -1.995f, 2 },
                true)]
        [DataRow(
                new float[] { -1.437f, 0 },
                new float[] { -1.789f, 0 },
                new float[] { -1.5f, 0 },
                true)]
        [DataRow(
                new float[] { 2.175f, 0 },
                new float[] { 4.445f, 0 },
                new float[] { 3.795f, 0 },
                true)]
        public void IsPointInsideSegment_ReturnsTrue_WhenSegmentParallelBase(float[] segStart, float[] segEnd, float[] testPoint, bool expected)
        {
            var segment = new Segment2D(new Point2D(segStart[0], segStart[1]), new Point2D(segEnd[0], segEnd[1]));
            Assert.AreEqual(expected, segment.IsPointBelongSegment(new Point2D(testPoint[0], testPoint[1])));
        }
    }
}
