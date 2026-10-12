using Geometry;
using Geometry.Exteisions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestModeCore_test.Geometry
{
    [TestClass]
    public class TestSelection
    {
        /// <summary>
        /// General test for testing Polygon Selection.
        /// Takes parameters of rectangle sides and set of points to build polygon
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="expected"></param>
        /// <param name="pointValues"></param>
        /// array of arrays to create the points of polygon
        public void TestIsSelectedByRectangleBase(int left, int right, int bottom, int top, bool expected, params int[][] pointValues)
        {
            var creator = new Hull2DCreator();
            var polygon = creator.CreateHullGraham(pointValues.Select(x => new Point2D(x[0], x[1])));

            var points = polygon.GetPoints();
            
            var polygonBox = new RectangleBox(points);
            var rectangleBox = new RectangleBox(left, right, bottom, top);


            Assert.AreEqual(expected, IsSelectedByRectangle(polygonBox, rectangleBox));
            //Assert.AreEqual(expected, IsSelectedByCrossing(polygon, rectangleBox));
        }

        public bool IsSelectedByRectangle(RectangleBox polygonBox, RectangleBox rectangle)
        {

            if (polygonBox.IsIntersectWithOther(rectangle))
                return true;
            if (polygonBox.IsInnerOther(rectangle))
                return true;

            return false;
        }

        public bool IsSelectedByCrossing(Polygon polygon, RectangleBox rectangle)
        {

            var rectanglePoints = rectangle.GetPoints();

            if (rectanglePoints.Any(p => polygon.IsPointInsidePolygon(p)))
                return true;
            // Вопрос, насколько нужен этот цикл проверки?
            //foreach (var rectangleSegment in rectanglePoints.Bigrams(true).Select(p => new Segment2D(p.Item1, p.Item2)))
            //{
            //    foreach (var contourSegment in points.Bigrams(true).Select(p => new Segment2D(p.Item1, p.Item2)))
            //    {
            //        if (contourSegment.HasIntersectedPoint(rectangleSegment))
            //            return true;
            //    }
            //}
            return false;
        }

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
        [DataRow(1, 3, 2, 3, false, new[] { 0, 0 }, new[] { 1, 1 }, new[] { 1, 0 }, new[] { 0, 1 })]
        [DataRow(1, 2, -3, -1, false, new[] { 0, 0 }, new[] { 1, 1 }, new[] { 1, 0 }, new[] { 0, 1 })]
        [DataRow(-2, -1, -2, -1, false, new[] { 0, 0 }, new[] { 1, 1 }, new[] { 1, 0 }, new[] { 0, 1 })]
        [DataRow(-3, -1, 1, 3, false, new[] { 0, 0 }, new[] { 1, 1 }, new[] { 1, 0 }, new[] { 0, 1 })]
        public void SelectionBox_IsSelectedByRectangle_ShouldBeFalse_WhenEmptyIntersection
            (int left, int right, int bottom, int top, bool expected, params int[][] pointValues) =>
            TestIsSelectedByRectangleBase(left, right, bottom, top, expected, pointValues);

        [TestMethod]
        [DataRow(new[] { -3, 0 }, new[] { 0, 2 }, new[] { -1, 6 }, new[] { -3, 7 }, new[] { -5, 4 })]
        [DataRow(new[] { 3, 0 }, new[] { 0, 4 }, new[] { 1, 6 }, new[] { 3, 7 }, new[] { 5, 4 })]
        [DataRow(new[] { 3, 0 }, new[] { 0, -4 }, new[] { 1, -6 }, new[] { 3, -7 }, new[] { 5, -4 })]
        [DataRow(new[] { -3, 0 }, new[] { 0, -4 }, new[] { -1, -6 }, new[] { -3, -7 }, new[] { -5, -4 })]
        [DataRow(new[] { -2, 3 }, new[] { -3, 4 }, new[] { -5, 2 }, new[] { -2, -7 }, new[] { -3, -4 })]
        [DataRow(new[] { -2, -3 }, new[] { -3, -4 }, new[] { -5, -2 }, new[] { -2, 7 }, new[] { -3, 4 })]
        [DataRow(new[] { 2, 3 }, new[] { 3, 4 }, new[] { 5, -2 }, new[] { 2, -7 }, new[] { 3, -4 })]
        [DataRow(new[] { 2, -3 }, new[] { 3, -4 }, new[] { 5, 2 }, new[] { 2, 7 }, new[] { 3, 4 })]
        public void SelectionBox_IsSelectedByRectangle_ShouldBeTrue_WhenOneRectanglePointInsidePolygon
            (params int[][] pointValues) => TestIsSelectedByRectangleBase(-2, 2, -3, 3, true, pointValues);

        [TestMethod]
        [DataRow(new[] { -3, 2 }, new[] { -4, 6 }, new[] { 0, 8 }, new[] { 1, 7 }, new[] { 5, 4 }, new[] { 3, 2 })]
        [DataRow(new[] { 1, -4 }, new[] { 1, 4 }, new[] { 2, 6 }, new[] { 5, 0 }, new[] { -4, -7 })]
        [DataRow(new[] { -3, -2 }, new[] { -4, -6 }, new[] { 0, -8 }, new[] { 1, -7 }, new[] { 5, -4 }, new[] { 3, -2 })]
        [DataRow(new[] { -1, -4 }, new[] { -1, 4 }, new[] { -2, 6 }, new[] { -5, 0 }, new[] { -4, -7 })]
        [DataRow(new[] { -1, -4 }, new[] { 1, 4 }, new[] { 2, 6 }, new[] { 5, 0 }, new[] { 4, 7 })]
        public void SelectionBox_IsSelectedByRectangle_ShouldBeTrue_WhenTwoRectanglePointInsidePolygon
            (params int[][] pointValues) => TestIsSelectedByRectangleBase(-2, 2, -3, 3, true, pointValues);

        [TestMethod]
        [DataRow(new[] { -4, -4 }, new[] { 2, 5 }, new[] { 5, 1 }, new[] { 3, -7 }, new[] { -3, -6 })]
        [DataRow(new[] { 4, -4 }, new[] { -2, 5 }, new[] { -5, 1 }, new[] { -3, -7 }, new[] { 3, -6 })]
        public void SelectionBox_IsSelectedByRectangle_ShouldBeTrue_WhenThreeRectanglePointInsidePolygon
            (params int[][] pointValues) => TestIsSelectedByRectangleBase(-2, 2, -3, 3, true, pointValues);

        [TestMethod]
        [DataRow(new[] { 0, 0 }, new[] { 2, 2 }, new[] { 2, 0 }, new[] { 0, 2 })]
        [DataRow(new[] { -1, 0 }, new[] { 1, 2 }, new[] { 1, 0 })]
        [DataRow(
            new[] { -1, 0 }, new[] { 1, 2 }, new[] { 1, 1 },
            new[] { 2, 1 }, new[] { 0, 1 }, new[] { 0, 0 }
            )]
        [DataRow(new[] { 2, 2 }, new[] { 2, -2 }, new[] { -2, -2 }, new[] { -2, 2 })]
        public void SelectionBox_IsSelectedByRectangle_ShouldBeTrue_WhenRectangleInsidePolygon
            (params int[][] pointValues) => TestIsSelectedByRectangleBase(0, 1, 0, 1, true, pointValues);

        [TestMethod]
        [DataRow(new[] { 0, 0 }, new[] { 1, 1 }, new[] { 1, 0 }, new[] { 0, 1 })]
        [DataRow(new[] { -5, 0 }, new[] { -2, 1 }, new[] { 1, 3 }, new[] { 2, -2 }, new[] { -4, -3 })]
        [DataRow(new[] { -5, 4 }, new[] { 0, 4 }, new[] { 0, 0 }, new[] { -5, 0 })]
        [DataRow(new[] { 0, 0 }, new[] { 0, 4 }, new[] { 5, 4 }, new[] { 5, 0 })]
        [DataRow(new[] { 0, 0 }, new[] { 5, 0 }, new[] { 5, -4 }, new[] { 0, -4 })]
        [DataRow(new[] { 0, 0 }, new[] { 0, -4 }, new[] { -5, -4 }, new[] { -5, 0 })]
        [DataRow(new[] { 0, 0 }, new[] { -5, 0 }, new[] { -5, 4 }, new[] { 0, 4 })]
        public void SelectionBox_IsSelectedByRectangle_ShouldBeTrue_WhenPolygonInsideRectangle
            (params int[][] pointValues) => TestIsSelectedByRectangleBase(-5, 5, -4, 4, true, pointValues);

        [TestMethod]
        [DataRow(new[] { 0, 0 }, new[] { 2, 2 }, new[] { 2, 0 }, new[] { 0, 2 })]
        [DataRow(new[] { 3, 2 }, new[] { 2, 2 }, new[] { 2, 5 }, new[] { 1, 5 }, new[] { 1, 6 }, new[] { 3, 5 })]
        [DataRow(new[] { 3, 1 }, new[] { 3, 2 }, new[] { 5, 3 }, new[] { 5, 2 })]
        [DataRow(new[] { 2, 2 }, new[] { 3, 2 }, new[] { 3, -3 }, new[] { 2, -3 })]
        public void SelectionBox_IsSelectedByRectangle_ShouldBeTrue_WhenRectangleIntersectsWithPolygon
            (params int[][] pointValues) => TestIsSelectedByRectangleBase(1, 4, -1, 3, true, pointValues);

        [TestMethod]
        [DataRow(-2, 2, -2, 2, true, new[] { -2, -2 }, new[] { -2, 2 }, new[] { 2, -2 }, new[] { 2, 2 })]
        [DataRow(-20, 25, -12, 10, true, new[] { -20, 10 }, new[] { -20, -12 }, new[] { 25, -12 }, new[] { 25, 10 })]
        public void SelectionBox_IsSelectedByRectangle_ShouldBeTrue_WhenPolygonEqualsRectangle
            (int left, int right, int bottom, int top, bool expected, params int[][] pointValues) =>
            TestIsSelectedByRectangleBase(left, right, bottom, top, expected, pointValues);

        [TestMethod]
        [DataRow(new[] { 1, 1 }, new[] { 1, 0 }, new[] { 2, 1 }, new[] { 2, 0 })]
        [DataRow(new[] { -1, 0 }, new[] { -1, 1 }, new[] { 0, 0 }, new[] { 0, 1 })]
        [DataRow(new[] { 0, 1 }, new[] { 0, 2 }, new[] { 1, 1 }, new[] { 1, 2 })]
        [DataRow(new[] { 0, -1 }, new[] { 0, 0 }, new[] { 1, -1 }, new[] { 1, 0 })]
        public void SelectionBox_IsSelectedByRectangle_ShouldBeTrue_WhenRectangleHasCommonSideWithPolygon
            (params int[][] pointValues) => TestIsSelectedByRectangleBase(0, 1, 0, 1, true, pointValues);

        [TestMethod]
        //convex octagon
        [DataRow(
            new[] { -1, 4 }, new[] { -4, 1 }, new[] { -4, -1 }, new[] { -1, -4 },
            new[] { 1, -4 }, new[] { 4, -1 }, new[] { 4, 1 }, new[] { 1, 4 }
            )]
        //rhombus
        [DataRow(new[] { 0, 4 }, new[] { -4, 0 }, new[] { 0, -4 }, new[] { 4, 0 })]
        //hexagon
        [DataRow(
            new[] { 0, 4 }, new[] { -3, 2 }, new[] { -3, -2 },
            new[] { 0, -4 }, new[] { 3, -2 }, new[] { 3, 2 }
            )]
        public void SelectionBox_IsSelectedByRectangle_ShouldBeTrue_WhenConvexPolygon
            (params int[][] pointsValues) => TestIsSelectedByRectangleBase(-4, 4, -4, 4, true, pointsValues);

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
