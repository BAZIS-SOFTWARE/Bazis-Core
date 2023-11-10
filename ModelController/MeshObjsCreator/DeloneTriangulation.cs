using Functions.Extensions;
using Geometry;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace ModelController.MeshObjsCreator
{
    public class DeloneTriangulation
    {
        private readonly List<Point2D> points;
        private readonly Dictionary<DeloneTriangle, List<DeloneTriangle>> adjacency;
        private DeloneTriangle lastRecentlyAdd;

        public DeloneTriangulation(List<Point2D> source)
        {
            var boundaryRectangle = GetBoundaries(source);
            var convertedPoints = MapPoints(source, boundaryRectangle).ToList();
            points = SortPoints(convertedPoints).ToList();
            adjacency = new Dictionary<DeloneTriangle, List<DeloneTriangle>>();
        }

        public IEnumerable<DeloneTriangle> GetTriangles()
        {
            points.Add(new Point2D(-100, -100));
            points.Add(new Point2D(100, -100));
            points.Add(new Point2D(0, 100));
            var superTriangle = new DeloneTriangle(points.Count - 3, points.Count - 2, points.Count - 1);
            adjacency.Add(superTriangle, new List<DeloneTriangle>());
            lastRecentlyAdd = superTriangle;
            for (var i = 0; i < points.Count - 3; i++)
            {
                var locatedTriangles = FindContainTriangle(points[i]).ToList();
                if (locatedTriangles.Count == 1)
                {
                    var locatedTriangle = locatedTriangles.First();
                    var triangleOne = new DeloneTriangle(i, locatedTriangle.C, locatedTriangle.A);
                    var triangleTwo = new DeloneTriangle(i, locatedTriangle.A, locatedTriangle.B);
                    var triangleThree = new DeloneTriangle(i, locatedTriangle.B, locatedTriangle.C);
                    var newTriangles = new List<DeloneTriangle> { triangleOne, triangleTwo, triangleThree };
                    InsertThreeTriangles(locatedTriangle, newTriangles);
                    LegalizeEdge(triangleOne, new Edge(locatedTriangle.C, locatedTriangle.A));
                    LegalizeEdge(triangleTwo, new Edge(locatedTriangle.A, locatedTriangle.B));
                    LegalizeEdge(triangleThree, new Edge(locatedTriangle.B, locatedTriangle.C));
                }
                else if (locatedTriangles.Count == 2)
                {
                    var triangleOne = locatedTriangles[0];
                    var triangleTwo = locatedTriangles[1];
                    var intersection = triangleOne.GetIntersectionWith(triangleTwo).ToList();
                    var pi = intersection[0];
                    var pj = intersection[1];
                    var pl = triangleOne.GetPoints().Except(intersection).First();
                    var pk = triangleTwo.GetPoints().Except(intersection).First();
                    var newTriangleOneUp = new DeloneTriangle(i, pi, pl);
                    var newTriangleOneDown = new DeloneTriangle(i, pl, pj);
                    var newTriangleTwoUp = new DeloneTriangle(i, pk, pi);
                    var newTriangleTwoDown = new DeloneTriangle(i, pj, pk);
                    var adjacentByEdgePiPl = FindAdjacentTriangleByEdge(triangleOne, new Edge(pi, pl));
                    var adjacentByEdgePlPj = FindAdjacentTriangleByEdge(triangleOne, new Edge(pl, pj));
                    var adjacentByEdgePjPk = FindAdjacentTriangleByEdge(triangleTwo, new Edge(pj, pk));
                    var adjacentByEdgePkPi = FindAdjacentTriangleByEdge(triangleTwo, new Edge(pk, pi));
                    adjacency.Add(newTriangleOneUp, new List<DeloneTriangle> { newTriangleOneDown, newTriangleTwoUp });
                    if (adjacentByEdgePiPl != null)
                    {
                        adjacency[newTriangleOneUp].Add(adjacentByEdgePiPl);
                        adjacency[adjacentByEdgePiPl].Add(newTriangleOneUp);
                        adjacency[adjacentByEdgePiPl].Remove(triangleOne);
                    }
                    adjacency.Add(newTriangleOneDown, new List<DeloneTriangle> { newTriangleOneUp, newTriangleTwoDown });
                    if (adjacentByEdgePlPj != null)
                    {
                        adjacency[newTriangleOneDown].Add(adjacentByEdgePlPj);
                        adjacency[adjacentByEdgePlPj].Add(newTriangleOneDown);
                        adjacency[adjacentByEdgePlPj].Remove(triangleOne);
                    }
                    adjacency.Add(newTriangleTwoDown, new List<DeloneTriangle> { newTriangleOneDown, newTriangleTwoUp });
                    if (adjacentByEdgePjPk != null)
                    {
                        adjacency[newTriangleTwoDown].Add(adjacentByEdgePjPk);
                        adjacency[adjacentByEdgePjPk].Add(newTriangleTwoDown);
                        adjacency[adjacentByEdgePjPk].Remove(triangleTwo);
                    }
                    adjacency.Add(newTriangleTwoUp, new List<DeloneTriangle> { newTriangleOneUp, newTriangleTwoDown });
                    lastRecentlyAdd = newTriangleTwoUp;
                    if (adjacentByEdgePkPi != null)
                    {
                        adjacency[newTriangleTwoUp].Add(adjacentByEdgePkPi);
                        adjacency[adjacentByEdgePkPi].Add(newTriangleTwoUp);
                        adjacency[adjacentByEdgePkPi].Remove(triangleTwo);
                    }
                    adjacency.Remove(triangleOne);
                    adjacency.Remove(triangleTwo);
                    LegalizeEdge(newTriangleOneUp, new Edge(pi, pl));
                    LegalizeEdge(newTriangleOneDown, new Edge(pl, pj));
                    LegalizeEdge(newTriangleTwoDown, new Edge(pj, pk));
                    LegalizeEdge(newTriangleTwoUp, new Edge(pk, pi));
                }
                else
                    throw new Exception("Unable to locate point in triangulation");
            }
            var triangles = DeleteSuperTriangleNodes(superTriangle);
            return triangles;
        }

        private IEnumerable<DeloneTriangle> DeleteSuperTriangleNodes(DeloneTriangle superTriangle)
        {
            foreach (var triangle in adjacency.Keys)
            {
                if (!triangle.GetPoints().Intersect(superTriangle.GetPoints()).Any())
                    yield return triangle;
            }
        }

        private bool IsSatisfyDeloneCondition(Point2D p, Point2D v1, Point2D v2, Point2D v3)
        {
            var a = v2.Sub(v1);
            var b = v3.Sub(v1);
            if (a._x * b._y - a._y * b._x > 0)
                (v2, v3) = (v3, v2);

            if (Math.Abs(a._x * b._y - a._y * b._x) <= 1e-4)
                throw new ArgumentException("Degenerate triangle given");

            var matrix = Matrix<float>.Build.DenseOfArray(new[,]
            {
                { v1._x, v1._y, v1._x * v1._x + v1._y * v1._y, 1  },
                { v2._x, v2._y, v2._x * v2._x + v2._y * v2._y, 1  },
                { v3._x, v3._y, v3._x * v3._x + v3._y * v3._y, 1  },
                { p._x, p._y, p._x * p._x + p._y * p._y, 1  }
            });
            var det = matrix.Determinant();
            return det >= 0;
        }

        private void LegalizeEdge(DeloneTriangle triangle, Edge edge)
        {
            var adjacentTriangle = FindAdjacentTriangleByEdge(triangle, edge);
            if (adjacentTriangle == null)
                return;
            var pk = adjacentTriangle.GetPoints().Except(triangle.GetPoints()).First();
            var pr = triangle.GetPoints().Except(adjacentTriangle.GetPoints()).First();
            if (IsSatisfyDeloneCondition(points[pk], points[edge.A], points[edge.B], points[pr]))
                return;
            var newTriangleOne = new DeloneTriangle(edge.A, pk, pr);
            var newTriangleTwo = new DeloneTriangle(pk, edge.B, pr);
            var newTriangleOneAdjacent = new List<DeloneTriangle>
            {
                FindAdjacentTriangleByEdge(triangle, new Edge(edge.A, pr)),
                FindAdjacentTriangleByEdge(adjacentTriangle, new Edge(edge.A, pk)),
                newTriangleTwo
            }.Where(x => x != null).ToList();
            var newTriangleTwoAdjacent = new List<DeloneTriangle>
            {
                FindAdjacentTriangleByEdge(triangle, new Edge(pr, edge.B)),
                FindAdjacentTriangleByEdge(adjacentTriangle, new Edge(pk, edge.B)),
                newTriangleOne
            }.Where(x => x != null).ToList();
            adjacency.Add(newTriangleOne, newTriangleOneAdjacent);
            adjacency.Add(newTriangleTwo, newTriangleTwoAdjacent);
            lastRecentlyAdd = newTriangleTwo;
            foreach (var t in newTriangleOneAdjacent.Except(new List<DeloneTriangle> { newTriangleTwo }))
            {
                adjacency[t].Remove(triangle);
                adjacency[t].Remove(adjacentTriangle);
                adjacency[t].Add(newTriangleOne);
            }
            foreach (var t in newTriangleTwoAdjacent.Except(new List<DeloneTriangle> { newTriangleOne }))
            {
                adjacency[t].Remove(adjacentTriangle);
                adjacency[t].Remove(triangle);
                adjacency[t].Add(newTriangleTwo);
            }
            adjacency.Remove(adjacentTriangle);
            adjacency.Remove(triangle);
            LegalizeEdge(newTriangleOne, new Edge(edge.A, pk));
            LegalizeEdge(newTriangleTwo, new Edge(pk, edge.B));
        }

        private DeloneTriangle FindAdjacentTriangleByEdge(DeloneTriangle triangle, Edge edge)
        {
            var adjacentTriangles = adjacency[triangle];
            foreach (var adjacentTriangle in adjacentTriangles)
            {
                var intersection = triangle.GetIntersectionWith(adjacentTriangle).ToList();
                var intersectionEdge = new Edge(intersection[0], intersection[1]);
                if (intersectionEdge.Equals(edge))
                    return adjacentTriangle;
            }
            return null;
        }

        private void InsertThreeTriangles(DeloneTriangle old, List<DeloneTriangle> newTriangles)
        {
            var adjacentTriangles = adjacency[old];
            foreach (var newTriangle in newTriangles)
            {
                if (!adjacency.ContainsKey(newTriangle))
                {
                    adjacency.Add(newTriangle, new List<DeloneTriangle>());
                    lastRecentlyAdd = newTriangle;
                }
                foreach (var adjacentTriangle in adjacentTriangles)
                {
                    var intersection = newTriangle.GetIntersectionWith(adjacentTriangle);
                    if (intersection.Count() != 2)
                        continue;
                    adjacency[newTriangle].Add(adjacentTriangle);
                    adjacency[adjacentTriangle].Remove(old);
                    adjacency[adjacentTriangle].Add(newTriangle);
                }
            }
            adjacency[newTriangles[0]].Add(newTriangles[1]);
            adjacency[newTriangles[0]].Add(newTriangles[2]);
            adjacency[newTriangles[1]].Add(newTriangles[0]);
            adjacency[newTriangles[1]].Add(newTriangles[2]);
            adjacency[newTriangles[2]].Add(newTriangles[0]);
            adjacency[newTriangles[2]].Add(newTriangles[1]);
            adjacency.Remove(old);
        }

        private IEnumerable<DeloneTriangle> FindContainTriangle(Point2D point)
        {
            foreach (var triangle in adjacency.Keys)
            {
                if (DeloneTriangle.IsPointInsideTriangle(point, points[triangle.A], points[triangle.B], points[triangle.C]))
                    yield return triangle;
            }
        }

        private IEnumerable<Point2D> SortPoints(List<Point2D> source)
        {
            var rows = (int)Math.Ceiling(Math.Pow(source.Count, 0.25));
            var bins = new Dictionary<Point2D, int>();
            foreach (var point in source)
            {
                var p = (int)(point._y * rows * 0.999); // row
                var q = (int)(point._x * rows * 0.999); // column
                var binIndex = (p % 2 == 1)
                    ? (p + 1) * rows - q
                    : p * rows + q + 1;
                bins.Add(point, binIndex);
            }
            return bins.OrderBy(p => p.Value).Select(p => p.Key);
        }

        private Rectangle GetBoundaries(List<Point2D> source)
        {
            var xMax = source.Select(p => p._x).Max();
            var xMin = source.Select(p => p._x).Min();
            var yMax = source.Select(p => p._y).Max();
            var yMin = source.Select(p => p._y).Min();
            return new Rectangle(xMin, xMax, yMin, yMax);
        }

        private IEnumerable<Point2D> MapPoints(List<Point2D> source, Rectangle rectangle)
        {
            var largestDimension = Math.Max(rectangle.Height, rectangle.Width);
            return source.Select(p => new Point2D(
                    (p._x - rectangle.Left) / largestDimension,
                    (p._y - rectangle.Bottom) / largestDimension));
        }

        private IEnumerable<Point2D> UnMapPoints(Rectangle rectangle)
        {
            var largestDimension = Math.Max(rectangle.Height, rectangle.Width);
            return points.Select(p => new Point2D(
                (p._x * largestDimension + rectangle.Left),
                (p._y * largestDimension + rectangle.Bottom)));
        }
    }
}