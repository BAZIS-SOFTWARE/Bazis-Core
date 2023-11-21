using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Functions.Extensions;

namespace ModelController.MeshObjsCreator.Delone2D
{
    public class DeloneTriangle
    {
        public DeloneNode2D A { get; }

        public DeloneNode2D B { get; }

        public DeloneNode2D C { get; }

        public DeloneTriangle(DeloneNode2D a, DeloneNode2D b, DeloneNode2D c)
        {
            var p = b.Position.Sub(a.Position);
            var m = c.Position.Sub(b.Position);
            var crossProduct = p._x * m._y - p._y * m._x;
            if (Math.Abs(crossProduct) < 1e-4)
                throw new ArgumentException("All point of the triangle on a line");
            if (crossProduct > 0)
            {
                A = a;
                B = b;
                C = c;
            }
            else
            {
                A = a;
                B = c;
                C = b;
            }
        }

        public bool IsNodeInsideTriangle(DeloneNode2D node)
        {
            foreach (var edge in GetNodes().Bigrams(true))
            {
                var a = edge.Item2.Position.Sub(edge.Item1.Position);
                var b = node.Position.Sub(edge.Item2.Position);
                var crossProduct = a._x * b._y - a._y * b._x;
                if (Math.Abs(crossProduct) < 1e-4)
                    continue;
                if (crossProduct < 0)
                    return false;
            }
            return true;
        }

        public IEnumerable<int> GetIntersectionWith(DeloneTriangle other)
        {
            var vertexesOne = new HashSet<int> { A.Number, B.Number, C.Number };
            var vertexesTwo = new HashSet<int> { other.A.Number, other.B.Number, other.C.Number };
            return vertexesOne.Intersect(vertexesTwo);
        }

        public IEnumerable<int> GetNodeIndexes() => new[] { A.Number, B.Number, C.Number };

        public IEnumerable<DeloneNode2D> GetNodes() => new[] { A, B, C };

        public override bool Equals(object obj)
        {
            if (!(obj is DeloneTriangle triangle))
                return false;
            return GetNodeIndexes().ToHashSet().SetEquals(triangle.GetNodeIndexes());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (A.Number + B.Number + C.Number) * 397 ^ A.Number * B.Number * C.Number;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{A.Number}-{B.Number}-{C.Number}";
        }
    }
}
