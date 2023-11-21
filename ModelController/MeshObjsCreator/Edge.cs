using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsCreator
{
    internal class Edge
    {
        public DeloneNode2D A { get; }

        public DeloneNode2D B { get; }

        public Edge(DeloneNode2D a, DeloneNode2D b)
        {
            A = a;
            B = b;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge edge))
                return false;
            return (A == edge.A && B == edge.B) || (A == edge.B && B == edge.A);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((A.Number + B.Number) * 397) ^ (A.Number * B.Number);
            }
        }

        public override string ToString()
        {
            return $"{A.Number}-{B.Number}";
        }
    }
}
