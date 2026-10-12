using SurfaceMesher;

namespace TestModeCore_test.SurfaceMesher
{
    [TestClass]
    public class TriangleFilter_should
    {
        private TriangleFilter filterInstance;
        private DeloneTriangulation_m2 triangulationInstance;

        [TestInitialize]
        public void Setup()
        {
            filterInstance = new TriangleFilter();
            triangulationInstance = new DeloneTriangulation_m2();
        }

        [DataTestMethod]
        [DataRow(8, new int[]
            {
                0, -1, -3,
                1, 3, -1,
                2, 2, 4,
                3, -3, 4,
                4, -5, 1,
                5, -3, 3,
                6, -1, 3,
                7, 1, 3,
                8, -3, 1,
                9, -1, 1,
                10, 1, 1,
                11, -1, -1,
                12, 1, -1
            },
            new int[]
            {
                13, -2, -1,
                14, 3, -3,
                15, 6, 3,
                16, 1, 6,
                17, -2, 3
            }, DisplayName = "Произвольный контур")]
        [DataRow(19, new int[]
            {
                0, -1, -3,
                1, 3, -1,
                2, 2, 4,
                3, -3, 4,
                4, -5, 1,
                5, -3, 3,
                6, -1, 3,
                7, 1, 3,
                8, -3, 1,
                9, -1, 1,
                10, 1, 1,
                11, -1, -1,
                12, 1, -1
            },
            new int[]
            {
                13, -1, -4,
                14, 3, -3,
                15, 6, 3,
                16, 1, 6,
                17, -5, 4,
                18, -6, -3
            }, DisplayName = "Все элементы в контуре")]
        [DataRow(0, new int[]
            {
                0, -1, -3,
                1, 3, -1,
                2, 2, 4,
                3, -3, 4,
                4, -5, 1,
                5, -3, 3,
                6, -1, 3,
                7, 1, 3,
                8, -3, 1,
                9, -1, 1,
                10, 1, 1,
                11, -1, -1,
                12, 1, -1
            },
            new int[]
            {
                13, 0, 2,
                14, -2, 2,
                15, -2, 0,
                16, 1, 0
            }, DisplayName = "Не одного элемента в конутре")]
        [DataRow(1, new int[]
            {
                0, -1, -3,
                1, 3, -1,
                2, 2, 4,
                3, -3, 4,
                4, -5, 1,
                5, -3, 3,
                6, -1, 3,
                7, 1, 3,
                8, -3, 1,
                9, -1, 1,
                10, 1, 1,
                11, -1, -1,
                12, 1, -1
            },
            new int[]
            {
                13, 0, 0,
                14, 5, 0,
                15, 5, 5,
                16, 0, 5
            }, DisplayName = "Контур первая четверть")]
        [DataRow(5, new int[]
            {
                0, -1, -3,
                1, 3, -1,
                2, 2, 4,
                3, -3, 4,
                4, -5, 1,
                5, -3, 3,
                6, -1, 3,
                7, 1, 3,
                8, -3, 1,
                9, -1, 1,
                10, 1, 1,
                11, -1, -1,
                12, 1, -1
            },
            new int[]
            {
                13, 0, 0,
                14, 0, 5,
                15, -6, 5,
                16, -6, 0
            }, DisplayName = "Контур вторая четверть")]
        [DataRow(0, new int[]
            {
                0, -1, -3,
                1, 3, -1,
                2, 2, 4,
                3, -3, 4,
                4, -5, 1,
                5, -3, 3,
                6, -1, 3,
                7, 1, 3,
                8, -3, 1,
                9, -1, 1,
                10, 1, 1,
                11, -1, -1,
                12, 1, -1
            },
            new int[]
            {
                13, 0, 0,
                14, -6, 0,
                15, -6, -4,
                16, 0, -4
            }, DisplayName = "Контур третья четверть")]
        [DataRow(0, new int[]
            {
                0, -1, -3,
                1, 3, -1,
                2, 2, 4,
                3, -3, 4,
                4, -5, 1,
                5, -3, 3,
                6, -1, 3,
                7, 1, 3,
                8, -3, 1,
                9, -1, 1,
                10, 1, 1,
                11, -1, -1,
                12, 1, -1
            },
            new int[]
            {
                13, 0, 0,
                14, 0, -4,
                15, 4, -4,
                16, 4, 0
            }, DisplayName = "Контур четвертая четверть")]
        [DataRow(0, new int[]
            {
                4, 1, 0,
                5, 2, 0,
                6, 2, 1,
                7, 1, 1
            },
            new int[]
            {
                8, -2, -1,
                9, -1, -1,
                10, -1, 0,
                11, -1, 1,
                12, -2, 1,
                13, -2, 0
            }, DisplayName = "Два независимых контура")]

        public void GetInflictedTriangles_ShouldBeCorrect_WhenLayersIntersect(int result, int[] previousLayer, int[] nextLayer)
        {
            var nodesLayerBottom = GetNode2Ds(previousLayer);
            triangulationInstance.BuildTriangulation(nodesLayerBottom, [], []);

            var layerBottomTriangles = triangulationInstance.GetTriangles().ToList();

            var nodesLayerTop = GetNode2Ds(nextLayer);
            var layerTopContour = new List<List<Node2D>> { new List<Node2D>() };
            for (int i = 0; i < nodesLayerTop.Count; i++)
            {
                layerTopContour[0].Add(nodesLayerTop[i]);
            }

            var inflictedTriangles = filterInstance.GetInflictedTriangles(nodesLayerBottom, layerBottomTriangles, layerTopContour, nodesLayerTop).ToList();
         
            Assert.AreEqual(result, inflictedTriangles.Count);
        }

        [TestMethod]
        [DataRow(false, new double[] { 1, 1 }, new double[] { 3, 3 }, new double[] { 1, 3 })]
        [DataRow(false, new double[] { 2.5, 0 }, new double[] { 5, 2.5 }, new double[] { 1.5, 2.5 })]
        [DataRow(true, new double[] { 3, 3 }, new double[] { 5, 2.5 }, new double[] { 5, 4 })]
        [DataRow(false, new double[] { 0.5, 2.5 }, new double[] { 1.5, 3.5 }, new double[] { 0.5, 3.5 })]
        public void GetInflictedTriangles_ShouldSelectTriangle_WhenItInsideDomain(bool result, double[] nodeOne, 
            double[] nodeTwo, double[] nodeThree)
        {
            var outerContour = new List<Node2D>()
            {
                new Node2D(0, 0, 0), new Node2D(1, 1.5, 0), new Node2D(2, 1.5, 2.5), new Node2D(3, 5, 2.5),
                new Node2D(4, 5, 4), new Node2D(5, 0, 4)
            };
            var a = new Node2D(6, nodeOne[0], nodeOne[1]);
            var b = new Node2D(7, nodeTwo[0], nodeTwo[1]);
            var c = new Node2D(8, nodeThree[0], nodeThree[1]);
            var triangle = new Triangle2D(0, a, b, c);

            var selected = filterInstance.GetInflictedTriangles([triangle], outerContour, [])
                                         .ToList();

            Assert.AreEqual(result, selected.Count != 0);
        }

        public List<Node2D> GetNode2Ds(int[] nodeData)
        {
            var result = new List<Node2D>();
            for (int i = 0; i < nodeData.Length; i = i + 3)
            {
                result.Add(new Node2D(nodeData[i], nodeData[i + 1], nodeData[i + 2]));
            }
            return result;
        }
    }
}
