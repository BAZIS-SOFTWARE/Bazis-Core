using Geometry;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using Functions.Extensions;
using Model;
using Model.Elements;

namespace ModelController.MeshObjsCreator.Delone2D
{
    public class DeloneTriangulation2D
    {
        private readonly Dictionary<int, DeloneNode2D> nodes;
        private readonly Dictionary<DeloneTriangle2D, List<DeloneTriangle2D>> adjacency;
        private readonly Dictionary<int, Node> sourceNodes;
        private DeloneTriangle2D lastRecentlyAdd;

        public DeloneTriangulation2D(List<Node> source)
        {
            sourceNodes = source.ToDictionary(n => n.Number, n => n);
            var localNodes = ConvertToLocalPlaneCoordinates(source).ToList();
            var boundaryRectangle = GetBoundaries(localNodes);
            var convertedPoints = MapPoints(localNodes, boundaryRectangle).ToList();
            nodes = SortPoints(convertedPoints).ToDictionary(x => x.Number, x => x);
            adjacency = new Dictionary<DeloneTriangle2D, List<DeloneTriangle2D>>();
        }

        public IEnumerable<Triangle> GetTriangles()
        {
            var sourceNodesInPlane = new DeloneNode2D[nodes.Count];
            nodes.Values.CopyTo(sourceNodesInPlane, 0);
            var superTriangle = CreateSuperTriangle();
            adjacency.Add(superTriangle, new List<DeloneTriangle2D>());
            lastRecentlyAdd = superTriangle;
            foreach (var node in sourceNodesInPlane)
            {
                var locatedTriangles = FindContainTriangle(node).ToList();
                if (locatedTriangles.Count == 1)
                {
                    var locatedTriangle = locatedTriangles.First();
                    var triangleOne = new DeloneTriangle2D(node, locatedTriangle.C, locatedTriangle.A);
                    var triangleTwo = new DeloneTriangle2D(node, locatedTriangle.A, locatedTriangle.B);
                    var triangleThree = new DeloneTriangle2D(node, locatedTriangle.B, locatedTriangle.C);
                    var newTriangles = new List<DeloneTriangle2D> { triangleOne, triangleTwo, triangleThree };
                    InsertThreeTriangles(locatedTriangle, newTriangles);
                    LegalizeEdge(triangleOne, new DeloneEdge2D(locatedTriangle.C, locatedTriangle.A));
                    LegalizeEdge(triangleTwo, new DeloneEdge2D(locatedTriangle.A, locatedTriangle.B));
                    LegalizeEdge(triangleThree, new DeloneEdge2D(locatedTriangle.B, locatedTriangle.C));
                }
                else if (locatedTriangles.Count == 2)
                {
                    var triangleOne = locatedTriangles[0];
                    var triangleTwo = locatedTriangles[1];
                    var intersection = triangleOne.GetIntersectionWith(triangleTwo).ToList();
                    var pi = intersection[0];
                    var pj = intersection[1];
                    var pl = triangleOne.GetNodeIndexes().Except(intersection).First();
                    var pk = triangleTwo.GetNodeIndexes().Except(intersection).First();
                    var newTriangleOneUp = new DeloneTriangle2D(node, nodes[pi], nodes[pl]);
                    var newTriangleOneDown = new DeloneTriangle2D(node, nodes[pl], nodes[pj]);
                    var newTriangleTwoUp = new DeloneTriangle2D(node, nodes[pk], nodes[pi]);
                    var newTriangleTwoDown = new DeloneTriangle2D(node, nodes[pj], nodes[pk]);
                    var adjacentByEdgePiPl = FindAdjacentTriangleByEdge(triangleOne, new DeloneEdge2D(nodes[pi], nodes[pl]));
                    var adjacentByEdgePlPj = FindAdjacentTriangleByEdge(triangleOne, new DeloneEdge2D(nodes[pl], nodes[pj]));
                    var adjacentByEdgePjPk = FindAdjacentTriangleByEdge(triangleTwo, new DeloneEdge2D(nodes[pj], nodes[pk]));
                    var adjacentByEdgePkPi = FindAdjacentTriangleByEdge(triangleTwo, new DeloneEdge2D(nodes[pk], nodes[pi]));
                    adjacency.Add(newTriangleOneUp, new List<DeloneTriangle2D> { newTriangleOneDown, newTriangleTwoUp });
                    lastRecentlyAdd = newTriangleOneUp;
                    if (adjacentByEdgePiPl != null)
                    {
                        adjacency[newTriangleOneUp].Add(adjacentByEdgePiPl);
                        adjacency[adjacentByEdgePiPl].Add(newTriangleOneUp);
                        adjacency[adjacentByEdgePiPl].Remove(triangleOne);
                    }
                    adjacency.Add(newTriangleOneDown,
                        new List<DeloneTriangle2D> { newTriangleOneUp, newTriangleTwoDown });
                    lastRecentlyAdd = newTriangleOneDown;
                    if (adjacentByEdgePlPj != null)
                    {
                        adjacency[newTriangleOneDown].Add(adjacentByEdgePlPj);
                        adjacency[adjacentByEdgePlPj].Add(newTriangleOneDown);
                        adjacency[adjacentByEdgePlPj].Remove(triangleOne);
                    }
                    adjacency.Add(newTriangleTwoDown,
                        new List<DeloneTriangle2D> { newTriangleOneDown, newTriangleTwoUp });
                    lastRecentlyAdd = newTriangleTwoDown;
                    if (adjacentByEdgePjPk != null)
                    {
                        adjacency[newTriangleTwoDown].Add(adjacentByEdgePjPk);
                        adjacency[adjacentByEdgePjPk].Add(newTriangleTwoDown);
                        adjacency[adjacentByEdgePjPk].Remove(triangleTwo);
                    }
                    adjacency.Add(newTriangleTwoUp, new List<DeloneTriangle2D> { newTriangleOneUp, newTriangleTwoDown });
                    lastRecentlyAdd = newTriangleTwoUp;
                    if (adjacentByEdgePkPi != null)
                    {
                        adjacency[newTriangleTwoUp].Add(adjacentByEdgePkPi);
                        adjacency[adjacentByEdgePkPi].Add(newTriangleTwoUp);
                        adjacency[adjacentByEdgePkPi].Remove(triangleTwo);
                    }
                    adjacency.Remove(triangleOne);
                    adjacency.Remove(triangleTwo);
                    LegalizeEdge(newTriangleOneUp, new DeloneEdge2D(nodes[pi], nodes[pl]));
                    LegalizeEdge(newTriangleOneDown, new DeloneEdge2D(nodes[pl], nodes[pj]));
                    LegalizeEdge(newTriangleTwoDown, new DeloneEdge2D(nodes[pj], nodes[pk]));
                    LegalizeEdge(newTriangleTwoUp, new DeloneEdge2D(nodes[pk], nodes[pi]));
                }
                else if (locatedTriangles.Count == 0)
                    throw new Exception("Unable to locate point in triangulation");
                else
                    throw new Exception("Point is located on 3 or more triangles");
            }
            var triangles = DeleteSuperTriangleNodes(superTriangle);
            var index = 0;
            foreach (var triangle in triangles)
            {
                yield return new Triangle(index++, triangle.GetNodeIndexes().Select(i => sourceNodes[i]).ToArray());
            }
        }

        private DeloneTriangle2D CreateSuperTriangle()
        {
            var nodeIndex = nodes.Values.Select(n => n.Number).Max();
            nodes.Add(nodeIndex + 1, new DeloneNode2D(nodeIndex + 1, new Point2D(-1000, -1000)));
            nodes.Add(nodeIndex + 2, new DeloneNode2D(nodeIndex + 2, new Point2D(1000, -1000)));
            nodes.Add(nodeIndex + 3, new DeloneNode2D(nodeIndex + 3, new Point2D(0, 1000)));
            return new DeloneTriangle2D(nodes[nodeIndex + 1], nodes[nodeIndex + 2], nodes[nodeIndex + 3]);
        }

        private IEnumerable<DeloneTriangle2D> DeleteSuperTriangleNodes(DeloneTriangle2D superTriangle)
        {
            foreach (var triangle in adjacency.Keys)
            {
                if (!triangle.GetNodeIndexes().Intersect(superTriangle.GetNodeIndexes()).Any())
                    yield return triangle;
            }
        }

        private bool IsSatisfyDeloneCondition(Point2D p, Point2D v1, Point2D v2, Point2D v3)
        {
            var a = v2.Sub(v1);
            var b = v3.Sub(v1);
            if (a._x * b._y - a._y * b._x > 0)
                (v2, v3) = (v3, v2);

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

        private void LegalizeEdge(DeloneTriangle2D triangle, DeloneEdge2D edge)
        {
            var adjacentTriangle = FindAdjacentTriangleByEdge(triangle, edge);
            if (adjacentTriangle == null)
                return;
            var pk = adjacentTriangle.GetNodeIndexes().Except(triangle.GetNodeIndexes()).First();
            var pr = triangle.GetNodeIndexes().Except(adjacentTriangle.GetNodeIndexes()).First();
            if (IsSatisfyDeloneCondition(nodes[pk].Position, edge.A.Position, edge.B.Position, nodes[pr].Position))
                return;
            var newTriangleOne = new DeloneTriangle2D(edge.A, nodes[pk], nodes[pr]);
            var newTriangleTwo = new DeloneTriangle2D(nodes[pk], edge.B, nodes[pr]);
            var newTriangleOneAdjacent = new List<DeloneTriangle2D>
            {
                FindAdjacentTriangleByEdge(triangle, new DeloneEdge2D(edge.A, nodes[pr])),
                FindAdjacentTriangleByEdge(adjacentTriangle, new DeloneEdge2D(edge.A, nodes[pk])),
                newTriangleTwo
            }.Where(x => x != null).ToList();
            var newTriangleTwoAdjacent = new List<DeloneTriangle2D>
            {
                FindAdjacentTriangleByEdge(triangle, new DeloneEdge2D(nodes[pr], edge.B)),
                FindAdjacentTriangleByEdge(adjacentTriangle, new DeloneEdge2D(nodes[pk], edge.B)),
                newTriangleOne
            }.Where(x => x != null).ToList();
            adjacency.Add(newTriangleOne, newTriangleOneAdjacent);
            adjacency.Add(newTriangleTwo, newTriangleTwoAdjacent);
            lastRecentlyAdd = newTriangleTwo;
            foreach (var t in newTriangleOneAdjacent.Except(new List<DeloneTriangle2D> { newTriangleTwo }))
            {
                adjacency[t].Remove(triangle);
                adjacency[t].Remove(adjacentTriangle);
                adjacency[t].Add(newTriangleOne);
            }
            foreach (var t in newTriangleTwoAdjacent.Except(new List<DeloneTriangle2D> { newTriangleOne }))
            {
                adjacency[t].Remove(adjacentTriangle);
                adjacency[t].Remove(triangle);
                adjacency[t].Add(newTriangleTwo);
            }
            adjacency.Remove(adjacentTriangle);
            adjacency.Remove(triangle);
            LegalizeEdge(newTriangleOne, new DeloneEdge2D(edge.A, nodes[pk]));
            LegalizeEdge(newTriangleTwo, new DeloneEdge2D(nodes[pk], edge.B));
        }

        private DeloneTriangle2D FindAdjacentTriangleByEdge(DeloneTriangle2D triangle, DeloneEdge2D edge)
        {
            var adjacentTriangles = adjacency[triangle];
            foreach (var adjacentTriangle in adjacentTriangles)
            {
                var intersection = triangle.GetIntersectionWith(adjacentTriangle).ToList();
                var begin = intersection[0];
                var end = intersection[1];
                var intersectionEdge = new DeloneEdge2D(nodes[begin], nodes[end]);
                if (intersectionEdge.Equals(edge))
                    return adjacentTriangle;
            }
            return null;
        }

        private void InsertThreeTriangles(DeloneTriangle2D old, List<DeloneTriangle2D> newTriangles)
        {
            var adjacentTriangles = adjacency[old];
            foreach (var newTriangle in newTriangles)
            {
                if (!adjacency.ContainsKey(newTriangle))
                {
                    adjacency.Add(newTriangle, new List<DeloneTriangle2D>());
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

        private IEnumerable<DeloneTriangle2D> FindContainTriangle(DeloneNode2D node)
        {
            var currentTriangle = lastRecentlyAdd;
            while (true)
            {
                var found = true;
                foreach (var edge in currentTriangle.GetNodes().Bigrams(true))
                {
                    var a = edge.Item2.Position.Sub(edge.Item1.Position);
                    var b = node.Position.Sub(edge.Item2.Position);
                    var crossProduct = a._x * b._y - a._y * b._x;
                    if (Math.Abs(crossProduct) < 1e-4)
                        continue;
                    if (crossProduct < 0)
                    {
                        currentTriangle = FindAdjacentTriangleByEdge(currentTriangle, new DeloneEdge2D(edge.Item1, edge.Item2));
                        found = false;
                        break;
                    }
                }
                if (found)
                    break;
            }
            var result = new List<DeloneTriangle2D> { currentTriangle };
            foreach (var triangle in adjacency[currentTriangle])
            {
                if (triangle.IsNodeInsideTriangle(node))
                {
                    result.Add(triangle);
                }
            }
            return result;
        }

        private IEnumerable<DeloneNode2D> SortPoints(List<DeloneNode2D> source)
        {
            var rows = (int)Math.Ceiling(Math.Pow(source.Count, 0.25));
            var bins = new Dictionary<DeloneNode2D, int>();
            foreach (var node in source)
            {
                var p = (int)(node.Position._y * rows * 0.999); // row
                var q = (int)(node.Position._x * rows * 0.999); // column
                var binIndex = p % 2 == 1
                    ? (p + 1) * rows - q
                    : p * rows + q + 1;
                bins.Add(node, binIndex);
            }
            return bins.OrderBy(p => p.Value).Select(p => p.Key);
        }

        private Rectangle GetBoundaries(List<DeloneNode2D> source)
        {
            var xMax = source.Select(p => p.Position._x).Max();
            var xMin = source.Select(p => p.Position._x).Min();
            var yMax = source.Select(p => p.Position._y).Max();
            var yMin = source.Select(p => p.Position._y).Min();
            return new Rectangle(xMin, xMax, yMin, yMax);
        }

        private IEnumerable<DeloneNode2D> MapPoints(List<DeloneNode2D> source, Rectangle rectangle)
        {
            var largestDimension = Math.Max(rectangle.Height, rectangle.Width);
            var scale = 100f;
            return source.Select(p => new DeloneNode2D(p.Number, new Point2D(
                scale * (p.Position._x - rectangle.Left) / largestDimension,
                scale * (p.Position._y - rectangle.Bottom) / largestDimension)));
        }

        private IEnumerable<DeloneNode2D> ConvertToLocalPlaneCoordinates(IEnumerable<Node> source)
        {
            return source.Select(node => new DeloneNode2D(node.Number, new Point2D(node.Position._x, node.Position._y)));
        }
    }
}