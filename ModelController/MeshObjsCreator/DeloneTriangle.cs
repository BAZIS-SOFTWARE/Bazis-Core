using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsCreator
{
    public class DeloneTriangle
    {
        public int A { get; }

        public int B { get; }

        public int C { get; }

        public DeloneTriangle(int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
        }

        public static bool IsPointInsideTriangle(Point2D point, Point2D a, Point2D b, Point2D c)
        {
            var sourceArea = GetTriangleArea(a, b, c);
            var areaSumResult = GetTriangleArea(point, a, b) +
                                GetTriangleArea(point, b, c) +
                                GetTriangleArea(point, a, c);
            return Math.Abs(sourceArea - areaSumResult) < 1e-3;
        }

        public static float GetTriangleArea(Point2D a, Point2D b, Point2D c)
        {
            var ab = b.Sub(a);
            var ac = c.Sub(a);
            return 0.5f * Math.Abs(ab._x * ac._y - ac._x * ab._y);
        }

        public IEnumerable<int> GetIntersectionWith(DeloneTriangle other)
        {
            var vertexesOne = new HashSet<int> { A, B, C };
            var vertexesTwo = new HashSet<int> { other.A, other.B, other.C };
            return vertexesOne.Intersect(vertexesTwo);
        }

        public IEnumerable<int> GetPoints() => new[] { A, B, C };

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
                var hashCode = ((A + B + C) * 397) ^ (A * B * C);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{A}-{B}-{C}";
        }
    }
}
