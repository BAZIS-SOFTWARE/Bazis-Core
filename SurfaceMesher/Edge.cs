using System;

namespace SurfaceMesher
{
    /// <summary>
    /// Неориентированное ребро, состоящее из двух узлов
    /// </summary>
    public class Edge
    {
        private const double epsilon = 1e-4;

        public Node2D A { get; }

        public Node2D B { get; }

        public Edge(Node2D a, Node2D b)
        {
            A = a;
            B = b;
            if (GetSquaredLength() < epsilon * epsilon)
                throw new ArgumentException($"Длина ребра ({a.Index}; {b.Index}) меньше .01");
        }

        public Point2D GetMiddlePoint()
        {
            var x = Math.Round((A.Position.X + B.Position.X) * 0.5d, 2);
            var y = Math.Round((A.Position.Y + B.Position.Y) * 0.5d, 2);
            return new Point2D(x, y);
        }

        public bool IsIntersectWith(Edge other)
        {
            var a1b1 = B.Position.Sub(A.Position);
            var a2b2 = other.B.Position.Sub(other.A.Position);
            var b1b2 = other.B.Position.Sub(B.Position);
            var b1a2 = other.A.Position.Sub(B.Position);
            var b2b1 = B.Position.Sub(other.B.Position);
            var b2a1 = A.Position.Sub(other.B.Position);
            return Vector.CrossProduct(a1b1, b1b2) * Vector.CrossProduct(a1b1, b1a2) < 0 //edit
                && Vector.CrossProduct(a2b2, b2b1) * Vector.CrossProduct(a2b2, b2a1) < 0;
        }

        public double GetSquaredLength()
        {
            var vector = B.Position.Sub(A.Position);
            return Vector.GetSquaredVectorLength(vector); //edit
        }

        public override int GetHashCode()
        {
            var hash = 37;
            unchecked
            {
                hash = A.GetHashCode() * hash + B.GetHashCode() + B.GetHashCode() * hash + A.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge edge)
            {
                return edge.A.Index == A.Index && edge.B.Index == B.Index
                    || edge.A.Index == B.Index && edge.B.Index == A.Index;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{A.Index} {B.Index}";
        }
    }
}