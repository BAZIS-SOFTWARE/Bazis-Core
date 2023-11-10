using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsCreator
{
    internal class Edge
    {
        public int A { get; }

        public int B { get; }

        public Edge(int a, int b)
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
                return ((A + B) * 397) ^ (A * B);
            }
        }

        public override string ToString()
        {
            return $"{A}-{B}";
        }
    }
}
