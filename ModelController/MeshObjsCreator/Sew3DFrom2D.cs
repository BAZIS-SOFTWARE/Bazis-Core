using Model.MeshObjects;
using ModelControllerInterfaces;
using ModelInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using System.Runtime.CompilerServices;
using Octree;
using System.Numerics;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Xml.Linq;
using System.Security.Cryptography;
using MathNet.Numerics.Distributions;


using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ModelController.MeshObjsCreator
{
    /// <summary>
    /// Map3DFrom2D
    /// </summary>
    public class Map3DFrom2D : IMap3DFrom2D
    {

        private class TriangleEdge
        {
            public readonly Vector3 a;
            public readonly Vector3 b;
            public readonly Vector3 c;
            public readonly Vector3 circumcenter;

            public Vector3 normal { get; private set; } // внешняя нормаль грани

            public TriangleEdge(Vector3 a, Vector3 b, Vector3 c, Vector3 normal)
            {
                this.a = a;
                this.b = b;
                this.c = c;
                this.normal = normal;
                circumcenter = CalcCircumcenter();
            }

            public TriangleEdge(Triangle triange)
            {
                var v = triange.GetVertexes().ToArray();
                var na = v[0] as Node;
                var nb = v[1] as Node;
                var nc = v[2] as Node;
                this.a = new Vector3(na._x, na._y, na._z);
                this.b = new Vector3(nb._x, nb._y, nb._z);
                this.c = new Vector3(nc._x, nc._y, nc._z);
            }

            private Vector3 CalcCircumcenter()
            {
                //https://en.wikipedia.org/wiki/Circumcircle
                var bc = (b - c).LengthSquared();
                var ca = (c - a).LengthSquared();
                var ab = (a - b).LengthSquared();
                var fa = bc * (ca + ab - bc);
                var fb = ca * (ab + bc - ca);
                var fc = ab * (bc + ca - ab);
                var invSum = 1f / (fa + fb + fc);
                return new Vector3(
                    (a.X * fa + b.X * fb + c.X * fc) * invSum,
                    (a.Y * fa + b.Y * fb + c.Y * fc) * invSum,
                    (a.Z * fa + b.Z * fb + c.Z * fc) * invSum);
            }



            public override bool Equals(object obj)
            {
                if (obj is TriangleEdge triangleEdge)
                    return Equals(triangleEdge);
                return false;
            }

            public bool Equals(TriangleEdge obj)
            {
                return a.Equals(obj.a) && b.Equals(obj.b) && c.Equals(obj.c)
                       || a.Equals(obj.a) && b.Equals(obj.c) && c.Equals(obj.b)
                       || a.Equals(obj.b) && b.Equals(obj.c) && c.Equals(obj.a)
                       || a.Equals(obj.b) && b.Equals(obj.a) && c.Equals(obj.c)
                       || a.Equals(obj.c) && b.Equals(obj.a) && c.Equals(obj.b)
                       || a.Equals(obj.c) && b.Equals(obj.b) && c.Equals(obj.a);
            }

            public override int GetHashCode()
            {
                return a.GetHashCode()^b.GetHashCode()^c.GetHashCode();
            }
        }

        private class EdgeSet : HashSet<TriangleEdge>
        {
            private HashSet<TriangleEdge> visited = new HashSet<TriangleEdge>();
            private List<TriangleEdge> current = new List<TriangleEdge>();
            public bool TryAddEdgesOfTetra(Node[] nodes)
            {
                bool rsl = false;
                foreach (var edge in GetEdgesOfTetra(nodes))
                {
                    rsl |= AddIfNotExist(edge);
                }
                return rsl;
            }

            public void FixEdges()
            {
                current = this.ToList();
            }

            public TriangleEdge GetNext()
            {
                while (true)
                {
                    var edge = current.FirstOrDefault();


                    if (edge == null)
                    {
                        FixEdges();
                        continue;
                    }     
                    else 
                        current.RemoveAt(0);


                    if (Contains(edge))
                        return edge;
                }
            }

            private bool AddIfNotExist(TriangleEdge triangleEdge)
            {
                if (visited.Contains(triangleEdge))
                {
                    if (Contains(triangleEdge))
                        Remove(triangleEdge);
                    return false;
                }

                visited.Add(triangleEdge);
                Add(triangleEdge);
                return true;
            }
            private IEnumerable<TriangleEdge> GetEdgesOfTetra(Node[] nodes)
            {
                var a = ToVector3(nodes[0]);
                var b = ToVector3(nodes[1]);
                var c = ToVector3(nodes[2]);
                var d = ToVector3(nodes[3]);

                yield return GetEdgeOfTetra(a, b, c, d);
                yield return GetEdgeOfTetra(d, b, c, a);
                yield return GetEdgeOfTetra(a, d, c, b);
                yield return GetEdgeOfTetra(a, b, d, c);
            }
            private TriangleEdge GetEdgeOfTetra(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
            {
                var nabc = Vector3.Cross(a - b, a - c);
                nabc *= Math.Sign(Vector3.Dot(nabc, a - d));
                return new TriangleEdge(a, b, c, nabc);
            }
            private Vector3 ToVector3(Node n) => new Vector3(n.Position._x, n.Position._y, n.Position._z);
        }

        interface ISearcher
        {
            Node GetNearestPoint(Point3D target);
            Node GetNearestPoint(TriangleEdge edge);
            void AddPoints(IEnumerable<IElement2D> set);
            void Add(Tetra tetra);
        }

        private class SimpleSearcher : ISearcher
        {
            private const double EPS = 10E-4;
            private List<Node> points;
            private Dictionary<int, List<Tetra>> map = new Dictionary<int, List<Tetra>>();
            public SimpleSearcher(IEnumerable<IElement2D> set)
            {
                AddPoints(set);
            }

            public void AddPoints(IEnumerable<IElement2D> set)
            {
                if (points is null)
                    points = new List<Node>();
                points.AddRange(set.SelectMany(w => w.GetVertexes().OfType<Node>()).Distinct());
            }

            public void Add(Tetra tetra)
            {
                foreach (var id in tetra.GetVertexes().Select(x => x.Number))
                {
                    if (map.TryGetValue(id, out var lst))
                        lst.Add(tetra);
                    else
                        map[id] = new List<Tetra>() { tetra };
                }
            }

            public Node GetNearestPoint(Point3D target)
            {
                var dist = float.MaxValue;
                Node rsl = null;
                foreach (var point in points)
                {
                    var sumSq = point.Position.Sub(target).GetSquareSumCoords();
                    if (sumSq < dist)
                    {
                        dist = sumSq;
                        rsl = point;
                    }
                }
                return rsl;
            }
            public Node GetNearestPoint(TriangleEdge edge)
            {
                Vector3 target = edge.circumcenter;
                var dist = float.MaxValue;
                Node rsl = null;


                var a = edge.a - target;
                var a2 = Vector3.Dot(a, a);
                var an = Vector3.Dot(a, edge.normal);


                foreach (var point in points)
                {

                    var r = new Vector3(point.Position._x, point.Position._y, point.Position._z) - target;
                    var ra = new Vector3(point.Position._x, point.Position._y, point.Position._z) - edge.a;
                    var rb = new Vector3(point.Position._x, point.Position._y, point.Position._z) - edge.b;
                    var rc = new Vector3(point.Position._x, point.Position._y, point.Position._z) - edge.c;
                    var r2 = Vector3.Dot(r, r);
                    var rn = Vector3.Dot(r, edge.normal);
                    //var x = (r2 - a2) / (an - rn);
                    //var x = Math.Abs(-(r2 - a2) / rn);
                    var x = (r2 - a2) / rn;
                    //var sumSq = float.MaxValue;

                    if (Vector3.Dot(r, edge.normal) > EPS) // проверка, что точка в нужной полуплоскости
                        if (!CrossAnyTetra(-r, point.Number))  // проверка, что вектор r не пересекает уже существующий тетраэдр
                            if (!CrossAnyTetra(-ra, point.Number) && !CrossAnyTetra(-rb, point.Number) && !CrossAnyTetra(-rc, point.Number))
                                if (x < dist)
                                {
                                    dist = x;
                                    rsl = point;
                                }
                }
                return rsl;
            }

            private bool CrossAnyTetra(Vector3 r, int id)
            {
                if (!map.ContainsKey(id))
                    return false;

                foreach (var tetra in map[id])
                {
                    var (a, b, c) = GetBasis(tetra, id);
                    if (IsVectorInTetra(r, a, b, c))
                        return true;
                }

                return false;
            }

            private bool IsVectorInTetra(Vector3 r, Vector3 a, Vector3 b, Vector3 c)
            {
                // если коэффициенты разложения в басис положительные => вектор внутри тетраэдра, образованного базисом
                //var matrix = new double[,]
                //{
                //    { a.X, b.X, c.X, r.X },
                //    { a.Y, b.Y, c.Y, r.Y },
                //    { a.Z, b.Z, c.Z, r.Z }
                //};
                var matrix = new double[,]
                {
                    { a.X, b.X, c.X },
                    { a.Y, b.Y, c.Y },
                    { a.Z, b.Z, c.Z }
                };

                var vec = new double[] { r.X, r.Y, r.Z };
                //var matrix2 = new double[,]
                //{
                //    { a.X, a.Y, a.Z},
                //    { b.X, b.Y, b.Z},
                //    { c.X, c.Y, c.Z},
                //    { r.X, r.Y, r.Z},
                //};

                var koeff = SLAU.Gauss(matrix, vec);


                //var koeff2 = SLAU.Gauss(new double[,]{
                //    {1, 0, 0, 1},
                //    {0, 1, 0, 1},
                //    {0, 0, 1, 1}});

                return koeff[0] > EPS && koeff[1] > EPS && koeff[2] > EPS;
            }

            private (Vector3 a, Vector3 b, Vector3 c) GetBasis(Tetra tetra, int id)
            {
                var p = tetra.GetVertexes().Single(w => w.Number == id).Position;
                var basis = tetra.GetVertexes().Where(w => w.Number != id).Select(w => w.Position.Sub(p)).ToArray();
                return (
                    new Vector3(basis[0]._x, basis[0]._y, basis[0]._z),
                    new Vector3(basis[1]._x, basis[1]._y, basis[1]._z),
                    new Vector3(basis[2]._x, basis[2]._y, basis[2]._z));
            }


            private class SLAU
            {
                public static double[] Gauss(double[,] matrix, double[] vec)
                {
                    Matrix<double> A = DenseMatrix.OfArray(matrix);
                    MathNet.Numerics.LinearAlgebra.Vector<double> B = DenseVector.OfArray(vec);

                    var c = A.Solve(B);
                    return new double[] { c[0], c[1], c[2] };
                }
            }
        }


        public class OctreeSearcher : ISearcher
        {
            private PointOctree<Node> _tree;
            private Dictionary<Vector3, Node> map;
            private float _initialSize;
            public OctreeSearcher(ImmutableList<IElement2D> set)
            {
                _initialSize = GetModelSize(set);
                var initialPosition = GetCentralPoint(set);
                _tree = new PointOctree<Node>(_initialSize, initialPosition, 1e-6f);
                map = new Dictionary<Vector3, Node>();
                foreach (var elem in set)
                {
                    var points = elem.GetVertexes().ToArray();
                    foreach (var point in points)
                    {
                        var v = new Vector3(point._x, point._y, point._z);
                        if (!map.ContainsKey(v))
                            map[v] = point as Node;
                    }
                }
                foreach (var p in map)
                {
                    _tree.Add(p.Value, p.Key);
                }
            }

            public void Add(Tetra tetra)
            {
                throw new NotImplementedException();
            }

            public void AddPoints(IEnumerable<IElement2D> set)
            {
                throw new NotImplementedException();
            }


            private Vector3 GetCentralPoint(ImmutableList<IElement2D> secSet)
            {
                var p = secSet.First().GetCoordinates().First();
                return new Vector3(p._x, p._y, p._z);
            }

            private float GetModelSize(ImmutableList<IElement2D> secSet)
            {
                var points = secSet.First().GetCoordinates().ToArray();
                var size = points[1].Sub(points[0]);
                return (size._x + size._y + size._z) * 0.33f;
            }

            Node ISearcher.GetNearestPoint(Point3D target)
            {
                throw new NotImplementedException();
            }

            Node ISearcher.GetNearestPoint(TriangleEdge normal)
            {
                throw new NotImplementedException();
            }
        }

        private int _number = 0;
        /// <summary>
        /// Создает разбиение пространства на тетраэдры, удовлетворяющие критерию Делоне,
        /// в области ограниченной двумя плоскостими,
        /// не имеющих пересечения в триангулируемой области.
        /// </summary>
        /// <param name="fistSet">триангуляция первой плоскости, удовлетворяющая критерию Делоне</param>
        /// <param name="secSet">триангуляция второй плоскости, удовлетворяющая критерию Делоне</param>
        /// <returns>Список тетраэдров, удовлетворяющие критерию Делоне</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public List<IElement3D> Create(List<IElement2D> fistSet, List<IElement2D> secSet)
        {
            var (set1, set2) = (fistSet, secSet);

            if (set1.Count < set2.Count)
                (set1, set2) = (set2, set1);

            return CreateTetras(set1, set2);
        }

        public class TetraIdComparer : IEqualityComparer<Tetra>
        {
            public bool Equals(Tetra x, Tetra y)
            {
                var ids1 = x.GetVertexes().Select(w => w.Number).OrderBy(w => w);
                var ids2 = y.GetVertexes().Select(w => w.Number).OrderBy(w => w);
                return ids1.SequenceEqual(ids2);
            }

            public int GetHashCode(Tetra obj)
            {
                var vertexes =  obj.GetVertexes().Select(w => w.Number).OrderBy(w => w).ToArray();
                return (((vertexes[0]*41+11) * vertexes[1]*101 + 11) * vertexes[2]*317 + 11) ^ vertexes[3];
            }
            public static TetraIdComparer Instance = new TetraIdComparer();
        }

        private List<IElement3D> CreateTetras(IReadOnlyList<IElement2D> set1, IReadOnlyList<IElement2D> set2)
        {
            HashSet<Tetra> rsl = new HashSet<Tetra>(TetraIdComparer.Instance);
            List<Tetra> rsl2 = new List<Tetra>();

            var ponts2 = set2.First().GetCoordinates().ToArray();
            var plane2 = new Geometry.Plane(ponts2[0], ponts2[1], ponts2[2]);

            EdgeSet edgeSet = new EdgeSet();

            Dictionary<Vector3, Node> map = new Dictionary<Vector3, Node>();
            FillMap(set1, map);

            ISearcher searcher = new SimpleSearcher(set2);

            foreach (var elem in set1)
            {
                // 1. находим центр описанной окружности 
                var triange = elem as Triangle;
                Point3D circumcenter = CalcCircumcenter(triange);

                edgeSet.Add(new TriangleEdge(triange));

                // 2. находим проекцию центра описанной окружности на плоскость второго множества
                Point3D projection = circumcenter.GetPointProectionOnPlane(plane2);
                // 3. Находим вершину, ближайшую к p
                Node[] nodes = new Node[4];
                Array.Copy(elem.GetVertexes().ToArray(), nodes, 3);
                nodes[3] = searcher.GetNearestPoint(projection);
                // 4. добавляем грани тетраэдра в множество граней
                // если грань еже была добавлена, она удаляется
                edgeSet.TryAddEdgesOfTetra(nodes);

                var tetra = new Tetra(_number++, nodes);

                if (rsl.Add(tetra))
                {
                    rsl2.Add(tetra);
                    searcher.Add(tetra);
                }

            }
            searcher.AddPoints(set1);
            FillMap(set2, map);
            edgeSet.FixEdges();

            // 5. По одной достаем грани и строим для нее тетраэдр
            while (edgeSet.Any())
            {
                var edge = edgeSet.GetNext();
                //var edge = edgeSet.First();

                var node = searcher.GetNearestPoint(edge);
                if (node != null)
                {
                    var nodes = new Node[4] { map[edge.a], map[edge.b], map[edge.c], node };

                    if (nodes.Select(w => w.Number).ToHashSet().Count != 4)
                        ;

                    var tetra = new Tetra(_number++, nodes);

                    var ids = nodes.Select(w => w.Number).ToHashSet();
                    if (ids.Contains(4) &&
                        ids.Contains(101) &&
                        ids.Contains(105))
                        ;

                    if (edgeSet.TryAddEdgesOfTetra(nodes))
                    {
                       if(rsl.Add(tetra));
                       {
                           rsl2.Add(tetra);
                           searcher.Add(tetra);
                       }
                    }
                }

                edgeSet.Remove(edge);
            }

            return rsl2.OfType<IElement3D>().ToList();
        }

        private void FillMap(IReadOnlyList<IElement2D> set1, Dictionary<Vector3, Node> map)
        {
            foreach (var p in set1)
            foreach (var node in p.GetVertexes())
            {
                var key = ToVector3(node as Node);
                if (!map.ContainsKey(key))
                    map[key] = node as Node;
            }
        }


        Vector3 ToVector3(Node n) => new Vector3(n.Position._x, n.Position._y, n.Position._z);

        /// <summary>
        /// Вычисление координат центра описанной окружности треугольника
        /// </summary>
        /// <param name="elem"></param>
        /// <returns>координаты центра описанной окружности треугольника</returns>
        private static Point3D CalcCircumcenter(Triangle elem)
        {
            //https://en.wikipedia.org/wiki/Circumcircle
            var vertexes = elem.GetCoordinates().ToList();
            var a2 = vertexes[0].Sub(vertexes[1]).GetSquareSumCoords();
            var b2 = vertexes[1].Sub(vertexes[2]).GetSquareSumCoords();
            var c2 = vertexes[2].Sub(vertexes[0]).GetSquareSumCoords();
            var fa = a2 * (b2 + c2 - a2);
            var fb = b2 * (c2 + a2 - b2);
            var fc = c2 * (a2 + b2 - c2);
            var invSum = 1f / (fa + fb + fc);
            return new Point3D(
                (vertexes[2]._x * fa + vertexes[0]._x * fb + vertexes[1]._x * fc) * invSum,
                (vertexes[2]._y * fa + vertexes[0]._y * fb + vertexes[1]._y * fc) * invSum,
                (vertexes[2]._z * fa + vertexes[0]._z * fb + vertexes[1]._z * fc) * invSum);
        }

    }
}