using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Model.Interfaces;

namespace Model
{
    public static class IEnumerableExtentions
    {
        public static IEnumerable<Tuple<T, T>> Bigrams<T>(this IEnumerable<T> items)
        {
            var enumerator = items.GetEnumerator();
            enumerator.MoveNext();
            var first = enumerator.Current;
            var head = items.First();
            while (enumerator.MoveNext())
            {
                var second = enumerator.Current;
                yield return Tuple.Create(first, second);
                first = second;
            }
        }
    }

    public class Curve : ModelObject, ILineObject
    {
        Node[] nodes;
        public Curve(int number, Node [] nodes) : base(number)
        {
            ObjType = "Линия";
            NumberOfPoints = nodes.Length;
            this.nodes = nodes;
        }

        public override ObjKind ObjKind { get;  }
        public override string ObjType { get; }

        public override int NumberOfPoints { get; }

        public override Point3D CalcCentralPoint()
        {
            var center = new Point3D();

            foreach (var node in nodes)
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }

        public float CalcLength()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Node> GetNodes()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }

        public override IEnumerable<Point3D> GetPoints()
        {
            foreach (var node in nodes)
            {
                yield return node.Position;
            }
        }

        public IEnumerable<Element1D> GetPolygonalChain()
        {
            return nodes.Bigrams()
                .Select((pair, index) => 
                new Element1D(index, new Node[] {pair.Item1, pair.Item2}, ObjKind.Beam));
        }
    }
}
