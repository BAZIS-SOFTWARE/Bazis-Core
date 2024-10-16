using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PrTests
{
    [TestClass]
    public class TestSegment2D
    {
        [TestMethod]
        [DataRow(
            new float[] { 1.125f, 0.625f, 1.125f, -0.625f },
            new float[] { -0.625f, 0, 2.625f, 0 })]
        [DataRow(
            new float[] { -1, -1, 1, 1 },
            new float[] { -1, 1, 1, -1 })]
        public void IsIntersectWith_ReturnsTrue_WhenIntersectPerpendicular(float[] segOne, float[] segTwo)
        {
            var segmentOne = new Segment2D(new Point2D(segOne[0], segOne[1]), new Point2D(segOne[2], segOne[3]));
            var segmentTwo = new Segment2D(new Point2D(segTwo[0], segTwo[1]), new Point2D(segTwo[2], segTwo[3]));
            Assert.IsTrue(segmentOne.IsIntersectWith(segmentTwo));
        }

        [TestMethod]
        [DataRow(
            new float[] { 1.125f, 0.625f, 1.125f, -0.625f },
            new float[] { 2.625f, 0, 0.625f, 0 })]
        [DataRow(
            new float[] { -2.5f, -1.25f, 0, 0 },
            new float[] { 1.25f, -0.625f, -3.775f, 0.225f })]
        [DataRow(
            new float[] { -0.55f, -1.1f, 0.55f, 1.1f },
            new float[] { 0.55f, -1.1f, -0.55f, 1.1f })]
        public void IsIntersectWith_ReturnsTrue_WhenIntersectUnderAngle(float[] segOne, float[] segTwo)
        {
            var segmentOne = new Segment2D(new Point2D(segOne[0], segOne[1]), new Point2D(segOne[2], segOne[3]));
            var segmentTwo = new Segment2D(new Point2D(segTwo[0], segTwo[1]), new Point2D(segTwo[2], segTwo[3]));
            Assert.IsTrue(segmentOne.IsIntersectWith(segmentTwo));
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
            Assert.IsFalse(segmentOne.IsIntersectWith(segmentTwo));
        }

        [TestMethod]
        [DataRow(
            new float[] { -3.745f, 0, 1.125f, 0 },
            new float[] { -7.325f, 0, 1.775f, 0 })]
        [DataRow(
            new float[] { -11.555f, 12.765f, -11.555f, -12.995f },
            new float[] { -11.556f, -12.995f, -11.556f, -14.785f })]
        [DataRow(
            new float[] { -14, -7, 14, 7},
            new float[] { -7, -3.5f, 7, 3.5f })]
        [DataRow(
            new float[] { -14, 7, 14, -7 },
            new float[] { -7, 3.5f, 7, -3.5f })]
        public void IsIntersectWith_ReturnsFalse_WhenParallel(float[] segOne, float[] segTwo)
        {
            var segmentOne = new Segment2D(new Point2D(segOne[0], segOne[1]), new Point2D(segOne[2], segOne[3]));
            var segmentTwo = new Segment2D(new Point2D(segTwo[0], segTwo[1]), new Point2D(segTwo[2], segTwo[3]));
            Assert.IsFalse(segmentOne.IsIntersectWith(segmentTwo));
        }

        [TestMethod]
        [DataRow(
            new float[] { -1, 0, 1, 0 },
            new float[] { -1, 0, 1, 0 })]
        [DataRow(
            new float[] { 1.175f, 0.995f, 11.125f, -99.675f },
            new float[] { 1.175f, 0.995f, 11.125f, -99.675f })]
        public void IsIntersectWith_ReturnsFalse_WhenOneCoverOther(float[] segOne, float[] segTwo)
        {
            var segmentOne = new Segment2D(new Point2D(segOne[0], segOne[1]), new Point2D(segOne[2], segOne[3]));
            var segmentTwo = new Segment2D(new Point2D(segTwo[0], segTwo[1]), new Point2D(segTwo[2], segTwo[3]));
            Assert.IsFalse(segmentOne.IsIntersectWith(segmentTwo));
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
            Assert.IsTrue(segmentOne.IsIntersectWith(segmentTwo));
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
        public void IsPointBelongsSegment_ReturnsTrue_WhenPointInsideDiagonalSegment(float[] segStart, float[] segEnd, float[] testPoint, bool expected)
        {
            var segment = new Segment2D(new Point2D(segStart[0], segStart[1]), new Point2D(segEnd[0], segEnd[1]));
            Assert.AreEqual(expected, segment.IsPointBelongSegment(new Point2D(testPoint[0], testPoint[1])));
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
