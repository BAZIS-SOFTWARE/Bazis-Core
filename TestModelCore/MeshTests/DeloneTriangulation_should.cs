using SurfaceMesher;

namespace MeshTests
{
    [TestClass]
    public class DeloneTriangulationTests
    {
        private DeloneTriangulation instance;
        private List<Node2D> nodes;
        private List<List<Node2D>> contour;

        [TestInitialize]
        public void SetUp()
        {
            instance = new DeloneTriangulation();
            nodes = new List<Node2D>
            {
                new Node2D(0, 0, 7),
                new Node2D(1, -5, 5),
                new Node2D(2, 5, 5),
                new Node2D(3, -2, 3),
                new Node2D(4, 3, 1),
                new Node2D(5, -4, -1),
                new Node2D(6, 1, -2),
                new Node2D(7, -6, -4),
                new Node2D(8, 5, -4)
            };
            contour = new List<List<Node2D>> { 
                new List<Node2D> 
                { 
                    nodes[7], 
                    nodes[8], 
                    nodes[2], 
                    nodes[0], 
                    nodes[1]
                } };
        }

        [TestMethod]
        public void GetTriangles_ShouldReturnCorrectNumberOfTriangles_OnSimplePointSet()
        { 
            instance.BuildTriangulation(nodes, [], []);
            var triangles = instance.GetTriangles().ToList();

            Assert.AreEqual(11, triangles.Count);
        }

        [TestMethod]
        public void GetTriangles_ShouldBeCorrect_WhenConstraintEdgeAlreadyExists()
        {
            var constrainedEdge = new Edge(nodes[6], nodes[3]);
            var constraints = new HashSet<Edge> { constrainedEdge };

            instance.BuildTriangulation(nodes, constraints, []);
            var triangles = instance.GetTriangles();
            var edges = GetEdgesFromTriangulation(triangles);

            Assert.IsTrue(edges.Contains(constrainedEdge));
        }


        [TestMethod]
        public void GetTriangles_ShouldNotReturnTrianglesOutsideBoundary_WhenBoundaryContourGiven()
        {
            var contour = new List<List<Node2D>> { new List<Node2D> { nodes[7], nodes[6], nodes[8], nodes[4], nodes[2], nodes[3], nodes[0], nodes[1], nodes[5] } };

            instance.BuildTriangulation(nodes, new HashSet<Edge>(), contour);
            var triangles = instance.GetTriangles().ToList();

            Assert.AreEqual(7, triangles.Count);
        }

        [TestMethod]
        public void GetTriangles_ShouldNotReturnTrianglesOutsideBoundary_OnDomainWithHole()
        {
            var outerContour = new List<Node2D> { nodes[7], nodes[8], nodes[2], nodes[0], nodes[1] };
            var innerContour = new List<Node2D> { nodes[5], nodes[3], nodes[4], nodes[6] };
            var contours = new List<List<Node2D>> { outerContour, innerContour };

            instance.BuildTriangulation(nodes, [], contours);
            var triangles = instance.GetTriangles().ToList();

            Assert.AreEqual(9, triangles.Count);
        }

        [TestMethod]
        [DataRow(5, 2)]
        [DataRow(7, 2)]
        [DataRow(1, 8)]
        public void GetTriangles_ShouldContainEdge_WhenConstraintGiven(int i, int j)
        {
            var constraintEdge = new Edge(nodes[i], nodes[j]);
            var constraints = new HashSet<Edge> { constraintEdge };

            instance.BuildTriangulation(nodes, constraints, new List<List<Node2D>>());
            var triangles = instance.GetTriangles().ToList();

            var edges = GetEdgesFromTriangulation(triangles);
            Assert.IsTrue(edges.Contains(constraintEdge));
        }

        [TestMethod]
        public void GetTriangles_ShouldBeCorrect_WhenConstrainedEdgeDoubled()
        {
            var constraintEdgeOne = new Edge(nodes[3], nodes[5]);
            var constraintEdgeTwo = new Edge(nodes[5], nodes[3]);
            var constraints = new HashSet<Edge> { constraintEdgeOne, constraintEdgeTwo };

            instance.BuildTriangulation(nodes, constraints, new List<List<Node2D>>());
            var triangles = instance.GetTriangles().ToList();

            Assert.AreEqual(11, triangles.Count);
        }

        [TestMethod]
        public void BuildTriangulation_ShouldBeCorrect_WhenTriangulationGiven_AsConstrainedEdges()
        {
            var constraints = new List<Edge>
            {
                new Edge(nodes[6], nodes[7]),
                new Edge(nodes[7], nodes[8]),
                new Edge(nodes[6], nodes[8]),
                new Edge(nodes[6], nodes[4]),
                new Edge(nodes[4], nodes[8]),
                new Edge(nodes[4], nodes[2]),
                new Edge(nodes[8], nodes[2]),
                new Edge(nodes[3], nodes[4]),
                new Edge(nodes[3], nodes[2]),
                new Edge(nodes[3], nodes[0]),
                new Edge(nodes[0], nodes[2]),
                new Edge(nodes[5], nodes[3]),
                new Edge(nodes[5], nodes[6]),
                new Edge(nodes[5], nodes[4]),
                new Edge(nodes[0], nodes[1]),
                new Edge(nodes[1], nodes[3]),
                new Edge(nodes[1], nodes[5]),
                new Edge(nodes[7], nodes[5]),
                new Edge(nodes[1], nodes[7]),
            };

            instance.BuildTriangulation(nodes, constraints, new List<List<Node2D>>());
            var triangles = instance.GetTriangles().ToList();

            Assert.AreEqual(11, triangles.Count);
        }

        [TestMethod]
        [DataRow(0, 0, false, DisplayName = "Inside inner contour")]
        [DataRow(0, 4, true, DisplayName = "Inside domain")]
        [DataRow(-1, 5, true, DisplayName = "Inside domain, on edge")]
        [DataRow(-4, 7, false, DisplayName = "Outside domain")]
        [DataRow(5, 1, true, DisplayName = "On the outer contour")]
        [DataRow(-5, 5, true, DisplayName = "On existing boundary node")]
        [DataRow(-2, 3, true, DisplayName = "On the inner contour node")]
        [DataRow(-100, 0, false, DisplayName = "Point is far from the outer contour")]
        public void IsNodeInsideContour_ShouldBeCorrect_OnVariousPoints(double x, double y, bool expected)
        {
            var outerContour = new List<Node2D> { nodes[7], nodes[8], nodes[2], nodes[0], nodes[1] };
            var innerContour = new List<Node2D> { nodes[5], nodes[3], nodes[4], nodes[6] };
            var contours = new List<List<Node2D>> { outerContour, innerContour };

            instance.BuildTriangulation(nodes, new HashSet<Edge>(), contours);
            var actual = instance.IsNodeInsideContour(new Node2D(nodes.Count + 3, x, y));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BuildTriangulation_ShouldReturnCorrectTrianglersNumber_WhenConstraintsGiven()
        {
            var nodes = new List<Node2D>
            {
                new Node2D(0, 1, 0),
                new Node2D(1, 1, 1),
                new Node2D(2, 0, 1),
                new Node2D(3, 2, -1),
                new Node2D(4, 2, 2),
                new Node2D(5, -1, 2),
                new Node2D(6, -1, -1)
            };

            var constrainedEdges = new HashSet<Edge>()
            {
                new Edge(nodes[0], nodes[1]),
                new Edge(nodes[1], nodes[2]),
                new Edge(nodes[2], nodes[0])
            };

            instance.BuildTriangulation(nodes, constrainedEdges, new List<List<Node2D>>());
            var triangles = instance.GetTriangles().ToList();

            Assert.AreEqual(8, triangles.Count);
        }

        [TestMethod]
        public void RefineNodes_ShouldRefineMesh_WhenNodesGivenOnly()
        {
            instance.BuildTriangulation(nodes, [], contour);
            var q1 = instance.GetTriangles().Select(t => t.GetQuality()).Average();
            var refinedNodes = instance.RefineMesh().ToList();
            instance.BuildTriangulation(refinedNodes, [], contour);
            var q2 = instance.GetTriangles().Select(t => t.GetQuality()).Average();

            Assert.IsTrue(q1 < q2);
        }

        private HashSet<Edge> GetEdgesFromTriangulation(IEnumerable<Triangle2D> triangles)
        {
            var edges = new HashSet<Edge>();
            foreach (var triangle in triangles)
            {
                foreach (var edge in triangle.GetNodes().Bigrams(true))
                {
                    edges.Add(new Edge(edge.Item1, edge.Item2));
                }
            }
            return edges;
        }
    }
}