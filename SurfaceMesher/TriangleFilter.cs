using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace SurfaceMesher
{
    [Obsolete]
    public class TriangleFilter
    {
        private DeloneTriangulation_m2 triangulationInstance;
        private HashSet<Node2D> inflictedNodes;
        private GeometryFactory geometryFactory;

        public TriangleFilter()
        {
            triangulationInstance = new DeloneTriangulation_m2();
            geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        }

        public IEnumerable<Triangle2D> GetInflictedTriangles(List<Node2D> layerPreviousNodes, 
            List<Triangle2D> trianglesPreviousLayer, 
            List<List<Node2D>> layerNextContour, 
            List<Node2D> layerNextNodes)
        {
            inflictedNodes = GetInflictedNodes(layerPreviousNodes, layerNextNodes, layerNextContour);
            
            foreach(var triangle in trianglesPreviousLayer)
            {
                if(inflictedNodes.Contains(triangle.A) && inflictedNodes.Contains(triangle.B) && inflictedNodes.Contains(triangle.C))
                {
                    yield return triangle;
                }
            }
        }

        public IEnumerable<Triangle2D> GetInflictedTriangles(List<Triangle2D> triangles,
            List<Node2D> outerContour, 
            List<Node2D>[] innerContours,
            bool specialSelection = true)
        {
            var polygon = GetPolygon(outerContour, innerContours);
            foreach (var triangle in triangles)
            {
                if (IsTriangleInsidePolygon(triangle, polygon))
                {
                    var minDistance = GetMinDistanceToPolygonByNodes(triangle, polygon);
                    var circumdiameter = GetCircumdiameter(triangle);
                    if (minDistance < 1e-4 || !specialSelection || minDistance > circumdiameter)
                        yield return triangle;
                }
            }
        }

        private HashSet<Node2D> GetInflictedNodes(List<Node2D> layerPreviousNodes, List<Node2D> layerNextNodes, List<List<Node2D>> layerNextContour)
        {
            var inflictedNodes = new HashSet<Node2D>();
            triangulationInstance.BuildTriangulation(layerNextNodes, Enumerable.Empty<Edge>(), layerNextContour);
            foreach (var node in layerPreviousNodes)
            {
                if (triangulationInstance.IsNodeInsideContour(node))
                    inflictedNodes.Add(node);
            }
            return inflictedNodes;
        }

        private Polygon GetPolygon(List<Node2D> outerContour, List<Node2D>[] innerContours)
        {
            var shell = GetLinearRing(outerContour);
            var holes = innerContours.Select(GetLinearRing)
                                     .ToArray();
            return geometryFactory.CreatePolygon(shell, holes);
        }

        private LinearRing GetLinearRing(List<Node2D> contour)
        {
            var coordinateSequence = contour.Select(n => new Coordinate(n.Position.X, n.Position.Y))
                                            .Append(new Coordinate(contour[0].Position.X, contour[0].Position.Y))
                                            .ToArray();
            return geometryFactory.CreateLinearRing(coordinateSequence);
        }

        private double GetMinDistanceToPolygonByNodes(Triangle2D triangle, Polygon polygon)
        {
            var distanceA = GetDistanceToPolygon(triangle.A, polygon);
            var distanceB = GetDistanceToPolygon(triangle.B, polygon);
            var distanceC = GetDistanceToPolygon(triangle.C, polygon);
            return Math.Min(distanceA, Math.Min(distanceB, distanceC));
        }

        private double GetDistanceToPolygon(Node2D node, Polygon polygon)
        {
            var point = new Point(node.Position.X, node.Position.Y);
            return polygon.Boundary.Distance(point);
        }

        private bool IsTriangleInsidePolygon(Triangle2D triangle, Polygon polygon)
        {
            var lr = new LinearRing(
            [
                new Coordinate(triangle.A.X, triangle.A.Y),
                new Coordinate(triangle.B.X, triangle.B.Y),
                new Coordinate(triangle.C.X, triangle.C.Y),
                new Coordinate(triangle.A.X, triangle.A.Y),
            ]);
            var p = new Polygon(lr);
            var intersection = polygon.Intersection(p);
            return Math.Abs(intersection.Area - p.Area) < 1e-4;
        }

        private double GetCircumdiameter(Triangle2D triangle)
        {
            var c = triangle.B.Position.Sub(triangle.A.Position).GetLength();
            var a = triangle.C.Position.Sub(triangle.B.Position).GetLength();
            var b = triangle.A.Position.Sub(triangle.C.Position).GetLength();
            var S = triangle.GetArea();
            return a * b * c / (2 * S);
        }
    }
} 