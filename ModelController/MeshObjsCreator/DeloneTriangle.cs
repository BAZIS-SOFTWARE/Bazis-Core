using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelController.MeshObjsCreator
{
    internal class DeloneTriangle
    {
        public Node2D A { get; }

        public Node2D B { get; }

        public Node2D C { get; }

        public DeloneTriangle(Node2D a, Node2D b, Node2D c)
        {
            A = a;
            B = b;
            C = c;
        }

        public bool IsNodeInsideTriangle(Node2D node)
        {
            var sourceArea = GetTriangleArea(A.Position, B.Position, C.Position);
            var areaSumResult = GetTriangleArea(node.Position, A.Position, B.Position) +
                                GetTriangleArea(node.Position, B.Position, C.Position) +
                                GetTriangleArea(node.Position, A.Position, C.Position);
            return Math.Abs(sourceArea - areaSumResult) < 1e-3;
        }

        public IEnumerable<int> GetIntersectionWith(DeloneTriangle other)
        {
            var vertexesOne = new HashSet<int> { A.Number, B.Number, C.Number };
            var vertexesTwo = new HashSet<int> { other.A.Number, other.B.Number, other.C.Number };
            return vertexesOne.Intersect(vertexesTwo);
        }

        public IEnumerable<int> GetPoints() => new[] { A.Number, B.Number, C.Number };

        public override bool Equals(object obj)
        {
            if (!(obj is DeloneTriangle triangle))
                return false;
            return GetPoints().ToHashSet().SetEquals(triangle.GetPoints());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ((A.Number + B.Number + C.Number) * 397) ^ (A.Number * B.Number * C.Number);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{A.Number}-{B.Number}-{C.Number}";
        }

        private float GetTriangleArea(Point2D a, Point2D b, Point2D c)
        {
            var ab = b.Sub(a);
            var ac = c.Sub(a);
            return 0.5f * Math.Abs(ab._x * ac._y - ac._x * ab._y);
        }
    }
}
