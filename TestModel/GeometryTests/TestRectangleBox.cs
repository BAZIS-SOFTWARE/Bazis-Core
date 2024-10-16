using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace PrTests
{
    [TestClass]
    public class TestRectangleBox
    {
        [TestMethod]
        [DataRow(-3, -2, 2, 3)]
        [DataRow(-3, -2, -3, -2)]
        [DataRow(2, 3, 2, 3)]
        [DataRow(2, 3, -3, -2)]
        public void IsIntersectWithOther_ShouldBeTrue_WhenIntersectsInPointInSide(int left, int right, int bottom, int top)
        {
            var rectangle = new RectangleBox(-2, 2, -2, 2);
            var otherRectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsIntersectWithOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-3, 3, 2, 3)]
        [DataRow(-3, 3, -3, -2)]
        [DataRow(-3, -2, -3, 3)]
        [DataRow(2, 3, -3, 3)]
        public void IsIntersectWithOther_ShouldBeTrue_WhenIntersectsInTwoPointsInSide(int left, int right, int bottom, int top)
        {
            var rectangle = new RectangleBox(-2, 2, -2, 2);
            var otherRectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsIntersectWithOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-3, -2, 2, -2)]
        [DataRow(2, 3, 2, -2)]
        [DataRow(-2, 2, 2, 3)]
        [DataRow(-2, 2, -2, -3)]
        public void IsIntersectWithOther_ShouldBeTrue_WhenIntersectsInSide(int left, int right, int bottom, int top)
        {
            var rectangle = new RectangleBox(-2, 2, -2, 2);
            var otherRectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsIntersectWithOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(1, 3, 1, 3)]
        [DataRow(-3, -1, 1, 3)]
        [DataRow(1, 3, -3, -1)]
        [DataRow(-3, -1, -3, -1)]
        public void IsIntersectWithOther_ShouldBeTrue_WhenOthersCornerInsideRectangle(int left, int right, int bottom, int top)
        {
            var rectangle = new RectangleBox(-2, 2, -2, 2);
            var otherRectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsIntersectWithOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(1, 4, -1, 1)]
        [DataRow(-1, -4, -1, 1)]
        [DataRow(-1, 1, 1, 4)]
        [DataRow(-1, 1, -4, -1)]
        public void IsIntersectWithOther_ShouldBeTrue_WhenOthersTwoCornersInsideRectangle(int left, int right, int bottom, int top)
        {
            var rectangle = new RectangleBox(-2, 2, -2, 2);
            var otherRectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsIntersectWithOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-1, 1, -1, 1)]
        [DataRow(-2, 0, 0, 2)]
        [DataRow(0, 2, 0, 2)]
        [DataRow(-2, 0, -2, 0)]
        [DataRow(0, 2, -2, 0)]
        public void IsIntersectWithOther_ShouldBeTrue_WhenOtherInsideRectangle(int left, int right, int bottom, int top)
        {
            var rectangle = new RectangleBox(-3, 3, -3, 3);
            var otherRectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsIntersectWithOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-3, 3, 8, 12)]
        [DataRow(-3, 3, -10, -6)]
        [DataRow(-15, -10, -3, 3)]
        [DataRow(12, 15, -3, 3)]
        [DataRow(6, 9, 6, 9)]
        [DataRow(-9, -6, 6, 9)]
        [DataRow(6, 9, -9, -6)]
        [DataRow(-9, -6, -9, -6)]
        public void IsIntersectWithOther_ShouldBeFalse_WhenNoIntersection(int left, int right, int bottom, int top)
        {
            var rectangle = new RectangleBox(-3, 3, -3, 3);
            var otherRectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsFalse(rectangle.IsIntersectWithOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-2, 0, 0, 2)]
        [DataRow(0, 2, 0, 2)]
        [DataRow(-2, 0, -2, 0)]
        [DataRow(0, 2, -2, 0)]
        [DataRow(-3, 3, -3, 3)]
        [DataRow(-1, 1, -1, 1)]
        [DataRow(-10, 10, -10, 10)]
        public void IsInnerOther_ShouldBeTrue_WhenRectanglesEquals(int left, int right, int bottom, int top)
        {
            var rectangle = new RectangleBox(left, right, bottom, top);
            var otherRectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsInnerOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-2, 0, 0, 2)]
        [DataRow(0, 2, 0, 2)]
        [DataRow(-2, 0, -2, 0)]
        [DataRow(0, 2, -2, 0)]
        [DataRow(-1, 1, -1, 1)]
        public void IsInnerOther_ShouldBeFalse_WhenOtherInsideRectangle(int left, int right, int bottom, int top)
        {
            var rectangle = new RectangleBox(-3, 3, -3, 3);
            var otherRectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsFalse(rectangle.IsInnerOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-3, 3, -2, 2)]
        [DataRow(-2, 2, -3, 3)]
        public void IsInnerOther_ShouldBeTrue_WhenRectanglesSideInsideOthers(int left, int right, int bottom, int top)
        {
            var otherRectangle = new RectangleBox(-3, 3, -3, 3);
            var rectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsInnerOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-3, 0, 0, 3)]
        [DataRow(0, 3, 0, 3)]
        [DataRow(-3, 0, -3, 0)]
        [DataRow(0, 3, -3, 0)]
        public void IsInnerOther_ShouldBeTrue_WhenRectanglesTwoSidesInsideOthers(int left, int right, int bottom, int top)
        {
            var otherRectangle = new RectangleBox(-3, 3, -3, 3);
            var rectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsInnerOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-3, 3, 0, 3)]
        [DataRow(-3, 3, -3, 0)]
        [DataRow(-3, 0, -3, 3)]
        [DataRow(0, 3, -3, 3)]
        public void IsInnerOther_ShouldBeTrue_WhenRectanglesThreeSidesInsideOthers(int left, int right, int bottom, int top)
        {
            var otherRectangle = new RectangleBox(-3, 3, -3, 3);
            var rectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsInnerOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-1, 1, -1, 1)]
        [DataRow(-2, 2, -2, 2)]
        [DataRow(-1, 0, 0, 1)]
        [DataRow(-1, 0, -1, 0)]
        [DataRow(0, 1, -1, 0)]
        [DataRow(0, 1, 0, 1)]
        [DataRow(-2, 0, 0, 2)]
        [DataRow(-2, 0, -2, 0)]
        [DataRow(0, 2, -2, 0)]
        [DataRow(0, 2, 0, 2)]
        public void IsInnerOther_ShouldBeTrue_WhenRectanglesInsideOtherWithoutIntersection(int left, int right, int bottom, int top)
        {
            var otherRectangle = new RectangleBox(-3, 3, -3, 3);
            var rectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsTrue(rectangle.IsInnerOther(otherRectangle));
        }

        [TestMethod]
        [DataRow(-5, -4, 4, 5)]
        [DataRow(3, 4, 4, 5)]
        [DataRow(-3, -2, -3, -2)]
        [DataRow(5, 6, -5, -4)]
        [DataRow(-1, 1, -5, -4)]
        [DataRow(-1, 1, 4, 5)]
        [DataRow(-5, -3, -1, 1)]
        [DataRow(3, 5, -1, 1)]
        public void IsInnerOther_ShouldBeFalse_WhenOtherOutside(int left, int right, int bottom, int top)
        {
            var otherRectangle = new RectangleBox(-1, 1, -1, 1);
            var rectangle = new RectangleBox(left, right, bottom, top);

            Assert.IsFalse(rectangle.IsInnerOther(otherRectangle));
        }
    }
}
