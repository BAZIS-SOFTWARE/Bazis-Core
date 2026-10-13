using SurfaceMesher;

namespace TestModeCore_test.SurfaceMesher
{
    /// <summary>
    /// Тесты на получение смешанной квадратно-треугольной сетки (GetQuadsTriangles).
    /// Во всех тестах домен - выпуклый четырёхугольник ABCD, заданный только
    /// граничными узлами (без внутренних точек), поэтому у триангуляции всего
    /// два треугольника, разделённых одной из диагоналей. Слияние этих двух
    /// треугольников всегда восстанавливает исходный четырёхугольник ABCD
    /// (по вершинам, противолежащим общему ребру), поэтому качество получившегося
    /// квада не зависит от того, какую именно диагональ выбрала триангуляция Делоне.
    /// </summary>
    [TestClass]
    public class QuadsFromTriangles
    {
        private static Node2D N(int index, double x, double y) => new Node2D(index, x, y);

        private static DeloneTriangulation_m2 BuildQuadDomain(Node2D a, Node2D b, Node2D c, Node2D d, IEnumerable<Edge> constraints = null)
        {
            var sourceNodes = new List<Node2D> { a, b, c, d };
            var boundary = new List<List<Node2D>> { new List<Node2D> { a, b, c, d } };
            var triangulation = new DeloneTriangulation_m2();
            triangulation.BuildTriangulation(sourceNodes, constraints ?? Enumerable.Empty<Edge>(), boundary);
            return triangulation;
        }

        [TestMethod]
        public void Rectangle_MergesIntoSingleQuad()
        {
            // Прямоугольник: все углы 90 градусов, качество квада = 1
            var a = N(0, 0, 0);
            var b = N(1, 4, 0);
            var c = N(2, 4, 2);
            var d = N(3, 0, 2);
            var triangulation = BuildQuadDomain(a, b, c, d);

            var result = triangulation.GetQuadsTriangles().ToList();

            Assert.AreEqual(0, result.OfType<Triangle2D>().Count());
            var quads = result.OfType<Quad2D>().ToList();
            Assert.AreEqual(1, quads.Count);
            var quadNodeIndices = new[] { quads[0].A.Index, quads[0].B.Index, quads[0].C.Index, quads[0].D.Index };
            CollectionAssert.AreEquivalent(new[] { 0, 1, 2, 3 }, quadNodeIndices);
        }

        [TestMethod]
        public void GoodQualityTrapezoid_MergesWithDefaultThreshold()
        {
            // Трапеция ABCD: углы ~90/63/117/90 градусов, качество ~0.70 (выше порога 0.5 по умолчанию)
            var a = N(0, 0, 0);
            var b = N(1, 4, 0);
            var c = N(2, 3.5, 1);
            var d = N(3, 0, 1);
            var triangulation = BuildQuadDomain(a, b, c, d);

            var result = triangulation.GetQuadsTriangles().ToList();

            Assert.AreEqual(1, result.OfType<Quad2D>().Count());
            Assert.AreEqual(0, result.OfType<Triangle2D>().Count());
        }

        [TestMethod]
        public void GoodQualityTrapezoid_DoesNotMergeWhenDiagonalIsConstrained()
        {
            // Та же трапеция, но диагональ AC явно закреплена как ограничение -
            // слияние через неё запрещено, несмотря на хорошее качество
            var a = N(0, 0, 0);
            var b = N(1, 4, 0);
            var c = N(2, 3.5, 1);
            var d = N(3, 0, 1);
            var constraints = new[] { new Edge(a, c) };
            var triangulation = BuildQuadDomain(a, b, c, d, constraints);

            var result = triangulation.GetQuadsTriangles().ToList();

            Assert.AreEqual(0, result.OfType<Quad2D>().Count());
            Assert.AreEqual(2, result.OfType<Triangle2D>().Count());
        }

        [TestMethod]
        public void BadQualityTrapezoid_DoesNotMergeWithDefaultThreshold()
        {
            // Сильно скошенная трапеция: углы ~90/18/162/90 градусов, качество ~0.2 (ниже порога 0.5)
            var a = N(0, 0, 0);
            var b = N(1, 4, 0);
            var c = N(2, 1, 1);
            var d = N(3, 0, 1);
            var triangulation = BuildQuadDomain(a, b, c, d);

            var result = triangulation.GetQuadsTriangles().ToList();

            Assert.AreEqual(0, result.OfType<Quad2D>().Count());
            Assert.AreEqual(2, result.OfType<Triangle2D>().Count());
        }

        [TestMethod]
        public void BadQualityTrapezoid_MergesWhenThresholdLowered()
        {
            // Та же скошенная трапеция, но с порогом качества 0 - слияние всё равно происходит,
            // потому что четырёхугольник остаётся выпуклым
            var a = N(0, 0, 0);
            var b = N(1, 4, 0);
            var c = N(2, 1, 1);
            var d = N(3, 0, 1);
            var triangulation = BuildQuadDomain(a, b, c, d);

            var result = triangulation.GetQuadsTriangles(qualityThreshold: 0).ToList();

            Assert.AreEqual(1, result.OfType<Quad2D>().Count());
            Assert.AreEqual(0, result.OfType<Triangle2D>().Count());
        }
    }
}
