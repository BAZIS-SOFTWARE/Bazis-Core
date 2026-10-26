using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TestModeCore_test.Geometry
{
    [TestClass]
    public class TestSelection
    {
        private double GetPolygonAreaByPoints(int[][] pointValues)
        {
            var creator = new Hull2DCreator();
            var polygon = creator.CreateHullGraham(pointValues.Select(x => new Point2D(x[0], x[1])));
            return polygon.Area;
        }

        [DataTestMethod]
        [DataRow(-0.8f, 0.5f)]
        [DataRow(-0.5f, 0.6f)]
        [DataRow(-0.6f, 0.65f)]
        public void IsPointBelongCurve_ShouldBeTrue_WhenPointBelongCurveStrictly(float x, float y)
        {
            var curve = new Curve2D(new List<Point2D>
            {
                new Point2D(-0.8f, 0.5f),
                new Point2D(-0.7f, 0.7f),
                new Point2D(-0.5f, 0.6f),
                new Point2D(-0.3f, 0.8f)
            });

            var actual = curve.IsPointBelongCurve(new Point2D(x, y));
            Assert.IsTrue(actual);
        }

        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(-0.8f, 0.3f)]
        [DataRow(-0.7f, 0.5f)]
        public void IsPointBelongCurve_ShouldBeFalse_WhenPointOutOfCurve(float x, float y)
        {
            var curve = new Curve2D(new List<Point2D>
            {
                new Point2D(-0.8f, 0.5f),
                new Point2D(-0.7f, 0.7f),
                new Point2D(-0.5f, 0.6f),
                new Point2D(-0.3f, 0.8f)
            });
            var actual = curve.IsPointBelongCurve(new Point2D(x, y));
            Assert.IsFalse(actual);
        }

        [TestMethod]
        [DataRow(-0.6f, 0.655f)]
        public void IsPointBelongCurve_ShouldBeTrue_WhenPointBelongCurveApproximately(float x, float y)
        {
            var curve = new Curve2D(new List<Point2D>
            {
                new Point2D(-0.8f, 0.5f),
                new Point2D(-0.7f, 0.7f),
                new Point2D(-0.5f, 0.6f),
                new Point2D(-0.3f, 0.8f)
            });
            var actual = curve.IsPointBelongCurve(new Point2D(x, y));
            Assert.IsTrue(actual);
        }

        public static IEnumerable<object[]> AdditionData
        {
            get
            {
                var creator = new Hull2DCreator();
                return new[] 
                { 
                    new object[] 
                    {
                        creator.CreateHullGraham(new List<Point2D>
                        {
                            new Point2D(0, 0),
                            new Point2D(0, 1),
                            new Point2D(1, 0),
                        }),
                        new Point2D(0.3f, 0.3f),
                        true
                    },
                    new object[]
                    {
                        creator.CreateHullGraham(new List<Point2D>
                        {
                            new Point2D(0, 0),
                            new Point2D(0, 1),
                            new Point2D(1, 0),
                        }),
                        new Point2D(0.4f, 0),
                        true
                    },
                    new object[]
                    {
                        creator.CreateHullGraham(new List<Point2D>
                        {
                            new Point2D(0, 0),
                            new Point2D(0, 1),
                            new Point2D(1, 0),
                        }),
                        new Point2D(0, 0),
                        true
                    },

                    new object[]
                    {
                        creator.CreateHullGraham(new List<Point2D>
                        {
                            new Point2D(0, 0),
                            new Point2D(0, 1),
                            new Point2D(1, 0),
                        }),
                        new Point2D(0.8f, 0.8f),
                        false
                    },
                    new object[]
                    {
                        creator.CreateHullGraham(new List<Point2D>
                        {
                            new Point2D(0, 0),
                            new Point2D(0, 1),
                            new Point2D(1, 0),
                        }),
                        new Point2D(0, -0.5f),
                        false
                    },
                    new object[]
                    {
                        creator.CreateHullGraham(new List<Point2D>
                        {
                            new Point2D(0, 0),
                            new Point2D(0, 1),
                            new Point2D(1, 0),
                        }),
                        new Point2D(0.51f, 0.51f),
                        false
                    },
                    new object[]
                    {
                        creator.CreateHullGraham(new List<Point2D>
                        {
                            new Point2D(-341.5122f, -77.0802f),
                            new Point2D(-377.1383f, 277.8025f),
                            new Point2D(-526.7075f, -166.2407f),
                            new Point2D(-82.39f, -97.79f),
                        }),
                        new Point2D(-377, 8),
                        true
                    },

                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(AdditionData))]
        public void IsPointInsidePolygon(Polygon poly,Point2D point, bool expected)
        {
            var actual = poly.IsPointInsidePolygon(point);
            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        [DataRow(0.5f, new[] { -1, 0 }, new[] { 0, 1 }, new[] { 0, 0 })]
        [DataRow(50f, new[] { 0, 0 }, new[] { 0, 1 }, new[] { 100, 0 })]
        [DataRow(0.5f, new[] { 0, 0 }, new[] { 1, 0 }, new[] { 6, 1 })]
        public void GetPolygonArea_ReturnsCorrectResult_WhenCountTriangleArea(float expected, params int[][] pointValues)
        {
            var result = GetPolygonAreaByPoints(pointValues);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(4f, new[] {-1, -1}, new[] {-1, 1}, new[] {1, 1}, new[] {1, -1})]
        [DataRow(5f, new[] {-1, 0}, new[] {-2, 1}, new[] {0, 3}, new[] {1, 0})]
        [DataRow(5f, new[] {0, 0}, new[] {-1, 2}, new[] {1, 3}, new[] {2, 1})]
        [DataRow(50.5f, new[] {0, 0}, new[] {0, 1}, new[] {1, 1}, new[] {100, 0})]
        [DataRow(38f, new[] { 2, -2 }, new[] { 3, 1 }, new[] { -4, 3 }, new[] { -5, -5 })]
        public void GetPolygonArea_ReturnsCorrectResult_WhenCountTetragonArea(float expected, params int[][] pointValues)
        {
            var result = GetPolygonAreaByPoints(pointValues);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(23f, new[] {1, -3}, new[] {-2, -1}, new[] {-1, 3}, new[] {5, 0}, new[] {3, -2})]
        [DataRow(25f, new[] { 0, -2 }, new[] { -3, -1 }, new[] { -4, 1 }, new[] { 0, 3 }, new[] { 3, 2 }, new[] {4, 0})]
        [DataRow(50f, new[] { 0, -6 }, new[] { 5, -4 }, new[] { 4, -1 }, new[] { 2, 1 },
            new[] { -1, 1 }, new[] { -4, 0 }, new[] { -5, -3 }, new[] { -3, -5 })]
        public void GetPolygonArea_ReturnsCorrectResult_WhenConvexPolygon(float expected, params int[][] pointValues)
        {
            var result = GetPolygonAreaByPoints(pointValues);
            Assert.AreEqual(expected, result);
        }
    }
}
