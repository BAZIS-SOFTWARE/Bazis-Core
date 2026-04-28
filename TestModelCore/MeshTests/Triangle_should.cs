using SurfaceMesher;

namespace Tests
{
    [TestClass]
    public class TriangleTests
    {
        [TestMethod]
        public void GetArea_ShouldReturnCorrectValue()
        {
            var triangle = new Triangle2D(0, new Node2D(1, 0, 0), new Node2D(2, 4, 0), new Node2D(3, 2, 3));
            Assert.AreEqual(6, triangle.GetArea());
        }

        [TestMethod]
        [DataRow(
            0, 0, 
            0, 0, 
            0, 0, DisplayName = "Three coincident nodes")]
        [DataRow(
            0, 0,
            0, 1,
            0, 2, DisplayName = "Three nodes on the line")]
        public void TriangleConstructor_ShouldThrowException_WhenZeroArea(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Triangle2D(0, new Node2D(1, x1, y1), new Node2D(2, x2, y2), new Node2D(3, x3, y3))
            );
        }

        [TestMethod]
        public void TriangleConstructor_ShouldThrowException_WhenSmallTriangleExpected()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Triangle2D(0, new Node2D(1, 0, 0), new Node2D(2, 1e-2, 0), new Node2D(3, 0, 1e-2))
            );
        }

        [TestMethod]
        public void TriangleConstructor_ShouldThrowException_WhenNodesOrderedClockwise()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                new Triangle2D(0, new Node2D(1, 0, 0), new Node2D(2, 0, 1), new Node2D(3, 1, 0))
            );
        }
    }
}