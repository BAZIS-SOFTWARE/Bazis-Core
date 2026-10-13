using SurfaceMesher;

namespace TestModeCore_test.SurfaceMesher
{
    [TestClass]
    public class Edge_should
    {
        [TestMethod]
        [DataRow(new[] { -1d, -1d }, new[] { 1d, 1d }, new[] { -1d, 1d }, new[] { 1d, -1d }, true, DisplayName = "intersected edges")]
        [DataRow(new[] { 0d, 0d }, new[] { 0d, 1d }, new[] { 0d, 0d }, new[] { 1d, 0d }, false, DisplayName = "intersected by one of bound nodes")]
        [DataRow(new[] { 0d, 0d }, new[] { 0d, 1d }, new[] { 1d, 0d }, new[] { 1d, 1d }, false, DisplayName = "parallel edges")]
        [DataRow(new[] { 0d, 0d }, new[] { 0d, 1d }, new[] { 0d, 0d }, new[] { 0d, 1d }, false, DisplayName = "coincident codirected edges")]
        [DataRow(new[] { 0d, 0d }, new[] { 0d, 1d }, new[] { 0d, 1d }, new[] { 0d, 0d }, false, DisplayName = "coincident oppositely-directed edges")]
        [DataRow(new[] { 0d, 0d }, new[] { 0d, 1d }, new[] { 0d, 2d }, new[] { 0d, 3d }, false, DisplayName = "not coincident not parallel codirected edges")]

        public void IsIntersectWith_ShouldBeCorrect_OnGivenEdges(double[] nodeOne, double[] nodeTwo, double[] nodeThree, double[] nodeFour, bool result)
        {
            var A1 = new Node2D(1, nodeOne[0], nodeOne[1]);
            var B1 = new Node2D(2, nodeTwo[0], nodeTwo[1]);
            var A2 = new Node2D(3, nodeThree[0], nodeThree[1]);
            var B2 = new Node2D(4, nodeFour[0], nodeFour[1]);

            var edgeOne = new Edge(A1, B1);
            var edgeTwo = new Edge(A2, B2);

            Assert.AreEqual(result, edgeOne.IsIntersectWith(edgeTwo));
        }

        [TestMethod]
        public void EdgeConstructor_ShouldThrowException_WhenCoincidentNodesGiven()
        {
            var A = new Node2D(1, 0, 1);
            var B = new Node2D(2, 0, 1);

            Assert.ThrowsException<ArgumentException>(() => new Edge(A, B));
        }
    }
}