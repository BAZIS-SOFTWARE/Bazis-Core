using Geometry;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Elements;

namespace ModelController.MeshObjsCreator
{
    public class DeloneTriangulation
    {
        private readonly Dictionary<int, Node2D> nodes;
        private readonly Dictionary<DeloneTriangle, List<DeloneTriangle>> adjacency;
        private readonly Dictionary<int, Node> sourceNodes;
        private DeloneTriangle lastRecentlyAdd;
        private readonly Rectangle boundaryRectangle;
        private readonly ConverterToLocalBazis converter;

        public DeloneTriangulation(List<Node> source)
        {
            sourceNodes = source.ToDictionary(n => n.Number, n => n);
            converter = new ConverterToLocalBazis(source[0].Position, source[1].Position, source[2].Position);
            var localNodes = ConvertToLocalPlaneCoordinates(source).ToList();
            boundaryRectangle = GetBoundaries(localNodes);
            var convertedPoints = MapPoints(localNodes, boundaryRectangle).ToList();
            nodes = SortPoints(convertedPoints).ToDictionary(x => x.Number, x => x);
            adjacency = new Dictionary<DeloneTriangle, List<DeloneTriangle>>();
        }

        public IEnumerable<Triangle> GetTriangles()
        {
            var sourceNodesInPlane = new Node2D[nodes.Count];
            nodes.Values.CopyTo(sourceNodesInPlane, 0);
            var superTriangle = CreateSuperTriangle();
            adjacency.Add(superTriangle, new List<DeloneTriangle>());
            lastRecentlyAdd = superTriangle;
            foreach(var node in sourceNodesInPlane)
            {
                var locatedTriangles = FindContainTriangle(node).ToList();
                if (locatedTriangles.Count == 1)
                {
                    var locatedTriangle = locatedTriangles.First();
                    var triangleOne = new DeloneTriangle(node, locatedTriangle.C, locatedTriangle.A);
                    var triangleTwo = new DeloneTriangle(node, locatedTriangle.A, locatedTriangle.B);
                    var triangleThree = new DeloneTriangle(node, locatedTriangle.B, locatedTriangle.C);
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
                    var newTriangleOneUp = new DeloneTriangle(node, nodes[pi], nodes[pl]);
                    var newTriangleOneDown = new DeloneTriangle(node, nodes[pl], nodes[pj]);
                    var newTriangleTwoUp = new DeloneTriangle(node, nodes[pk], nodes[pi]);
                    var newTriangleTwoDown = new DeloneTriangle(node, nodes[pj], nodes[pk]);
                    var adjacentByEdgePiPl = FindAdjacentTriangleByEdge(triangleOne, new Edge(nodes[pi], nodes[pl]));
                    var adjacentByEdgePlPj = FindAdjacentTriangleByEdge(triangleOne, new Edge(nodes[pl], nodes[pj]));
                    var adjacentByEdgePjPk = FindAdjacentTriangleByEdge(triangleTwo, new Edge(nodes[pj], nodes[pk]));
                    var adjacentByEdgePkPi = FindAdjacentTriangleByEdge(triangleTwo, new Edge(nodes[pk], nodes[pi]));
                    adjacency.Add(newTriangleOneUp, new List<DeloneTriangle> { newTriangleOneDown, newTriangleTwoUp });
                    if (adjacentByEdgePiPl != null)
                    {
                        adjacency[newTriangleOneUp].Add(adjacentByEdgePiPl);
                        adjacency[adjacentByEdgePiPl].Add(newTriangleOneUp);
                        adjacency[adjacentByEdgePiPl].Remove(triangleOne);
                    }
                    adjacency.Add(newTriangleOneDown,
                        new List<DeloneTriangle> { newTriangleOneUp, newTriangleTwoDown });
                    if (adjacentByEdgePlPj != null)
                    {
                        adjacency[newTriangleOneDown].Add(adjacentByEdgePlPj);
                        adjacency[adjacentByEdgePlPj].Add(newTriangleOneDown);
                        adjacency[adjacentByEdgePlPj].Remove(triangleOne);
                    }
                    adjacency.Add(newTriangleTwoDown,
                        new List<DeloneTriangle> { newTriangleOneDown, newTriangleTwoUp });
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
                    LegalizeEdge(newTriangleOneUp, new Edge(nodes[pi], nodes[pl]));
                    LegalizeEdge(newTriangleOneDown, new Edge(nodes[pl], nodes[pj]));
                    LegalizeEdge(newTriangleTwoDown, new Edge(nodes[pj], nodes[pk]));
                    LegalizeEdge(newTriangleTwoUp, new Edge(nodes[pk], nodes[pi]));
                }
                else
                    throw new Exception("Unable to locate point in triangulation");
            }

            var triangles = DeleteSuperTriangleNodes(superTriangle);
            var index = 0;
            foreach (var triangle in triangles)
            {
                yield return new Triangle(index++, triangle.GetPoints().Select(i => sourceNodes[i]).ToArray());
            }
        }

        private DeloneTriangle CreateSuperTriangle()
        {
            var nodeIndex = nodes.Values.Select(n => n.Number).Max();
            nodes.Add(nodeIndex + 1, new Node2D(nodeIndex + 1, new Point2D(-100, -100)));
            nodes.Add(nodeIndex + 2, new Node2D(nodeIndex + 2, new Point2D(100, -100)));
            nodes.Add(nodeIndex + 3, new Node2D(nodeIndex + 3, new Point2D(0, 100)));
            return new DeloneTriangle(nodes[nodeIndex + 1], nodes[nodeIndex + 2], nodes[nodeIndex + 3]);
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
                { v1._x, v1._y, v1._x * v1._x + v1._y * v1._y, 1 },
                { v2._x, v2._y, v2._x * v2._x + v2._y * v2._y, 1 },
                { v3._x, v3._y, v3._x * v3._x + v3._y * v3._y, 1 },
                { p._x, p._y, p._x * p._x + p._y * p._y, 1 }
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
            if (IsSatisfyDeloneCondition(nodes[pk].Position, edge.A.Position, edge.B.Position, nodes[pr].Position))
                return;
            var newTriangleOne = new DeloneTriangle(edge.A, nodes[pk], nodes[pr]);
            var newTriangleTwo = new DeloneTriangle(nodes[pk], edge.B, nodes[pr]);
            var newTriangleOneAdjacent = new List<DeloneTriangle>
            {
                FindAdjacentTriangleByEdge(triangle, new Edge(edge.A, nodes[pr])),
                FindAdjacentTriangleByEdge(adjacentTriangle, new Edge(edge.A, nodes[pk])),
                newTriangleTwo
            }.Where(x => x != null).ToList();
            var newTriangleTwoAdjacent = new List<DeloneTriangle>
            {
                FindAdjacentTriangleByEdge(triangle, new Edge(nodes[pr], edge.B)),
                FindAdjacentTriangleByEdge(adjacentTriangle, new Edge(nodes[pk], edge.B)),
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
            LegalizeEdge(newTriangleOne, new Edge(edge.A, nodes[pk]));
            LegalizeEdge(newTriangleTwo, new Edge(nodes[pk], edge.B));
        }

        private DeloneTriangle FindAdjacentTriangleByEdge(DeloneTriangle triangle, Edge edge)
        {
            var adjacentTriangles = adjacency[triangle];
            foreach (var adjacentTriangle in adjacentTriangles)
            {
                var intersection = triangle.GetIntersectionWith(adjacentTriangle).ToList();
                var begin = intersection[0];
                var end = intersection[1];
                var intersectionEdge = new Edge(nodes[begin], nodes[end]);
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

        private IEnumerable<DeloneTriangle> FindContainTriangle(Node2D node)
        {
            foreach (var triangle in adjacency.Keys)
            {
                if (triangle.IsNodeInsideTriangle(node))
                    yield return triangle;
            }
        }

        private IEnumerable<Node2D> SortPoints(List<Node2D> source)
        {
            var rows = (int)Math.Ceiling(Math.Pow(source.Count, 0.25));
            var bins = new Dictionary<Node2D, int>();
            foreach (var node in source)
            {
                var p = (int)(node.Position._y * rows * 0.999); // row
                var q = (int)(node.Position._x * rows * 0.999); // column
                var binIndex = (p % 2 == 1)
                    ? (p + 1) * rows - q
                    : p * rows + q + 1;
                bins.Add(node, binIndex);
            }
            return bins.OrderBy(p => p.Value).Select(p => p.Key);
        }

        private Rectangle GetBoundaries(List<Node2D> source)
        {
            var xMax = source.Select(p => p.Position._x).Max();
            var xMin = source.Select(p => p.Position._x).Min();
            var yMax = source.Select(p => p.Position._y).Max();
            var yMin = source.Select(p => p.Position._y).Min();
            return new Rectangle(xMin, xMax, yMin, yMax);
        }

        private IEnumerable<Node2D> MapPoints(List<Node2D> source, Rectangle rectangle)
        {
            var largestDimension = Math.Max(rectangle.Height, rectangle.Width);
            var scale = 1f;
            return source.Select(p => new Node2D(p.Number, new Point2D(
                scale * (p.Position._x - rectangle.Left) / largestDimension,
                scale * (p.Position._y - rectangle.Bottom) / largestDimension)));
        }

        private IEnumerable<Node2D> UnMapPoints(List<Node2D> source, Rectangle rectangle)
        {
            var largestDimension = Math.Max(rectangle.Height, rectangle.Width);
            return source.Select(p => new Node2D(p.Number, new Point2D(
                (p.Position._x * largestDimension + rectangle.Left),
                (p.Position._y * largestDimension + rectangle.Bottom))));
        }

        private IEnumerable<Node2D> ConvertToLocalPlaneCoordinates(IEnumerable<Node> source)
        {
            foreach (var node in source)
            {
                var point2D = converter.MapToLocalPlaneCoordinates(node.Position);
                yield return new Node2D(node.Number, new Point2D(point2D._x, point2D._y));
            }
        }

        private IEnumerable<Node> ReconvertToSourceCoordinates(IEnumerable<Node2D> source)
        {
            foreach (var node in source)
            {
                var point3D = converter.UnmapToSourceCoordinates(node.Position);
                yield return new Node(node.Number, point3D);
            }
        }
    }
}