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
using System.Drawing;

namespace ModelController.MeshObjsCreator
{
    /// <summary>
    /// Map3DFrom2D
    /// </summary>
    public class Map3DFrom2D : IMap3DFrom2D
    {
        private static Vector3 ToVector3(Node n) => new Vector3(n.Position._x, n.Position._y, n.Position._z);
        private static Vector3 ToVector3(Point3D n) => new Vector3(n._x, n._y, n._z);
        private class TriangleFacet
        {
            public readonly Vector3 a;
            public readonly Vector3 b;
            public readonly Vector3 c;

            public readonly Node na;
            public readonly Node nb;
            public readonly Node nc;


            public readonly Vector3 circumcenter;

            public Vector3 normal { get; private set; } // внешняя нормаль грани

            public TriangleFacet((Vector3 v, Node n) a, (Vector3 v, Node n) b, (Vector3 v, Node n) c, Vector3 normal)
            {
                this.a = a.v;
                this.b = b.v;
                this.c = c.v;

                na = a.n;
                nb = b.n;
                nc = c.n;

                this.normal = normal;
                circumcenter = CalcCircumcenter();
            }

            public TriangleFacet(Triangle triange)
            {
                var v = triange.GetVertexes().ToArray();
                na = v[0] as Node;
                nb = v[1] as Node;
                nc = v[2] as Node;
                this.a = ToVector3(na); 
                this.b = ToVector3(nb); 
                this.c = ToVector3(nc); 
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
                if (obj is TriangleFacet triangleEdge)
                    return Equals(triangleEdge);
                return false;
            }

            public bool Equals(TriangleFacet obj)
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

        private class EdgeQueue
        {
            // посещенные грани
            private HashSet<TriangleFacet> visited = new HashSet<TriangleFacet>();
            // очередь граней
            private Queue<TriangleFacet> queue = new Queue<TriangleFacet>();

            public bool TryPushEdgesOfTetra(Node[] nodes)
            {
                bool rsl = false;
                foreach (var edge in GetEdgesOfTetra(nodes))
                {
                    rsl |= AddIfNotExist(edge);
                }
                return rsl;
            }
            internal void AddAsVisited(TriangleFacet facet)
            {
                visited.Add(facet);
            }

            /// <summary>
            /// грани достаются в порядке FIFO, отбрасывая уже обработанные
            /// </summary>
            /// <param name="facet"></param>
            /// <returns></returns>
            public bool TryPop(out TriangleFacet facet)
            {
                facet = null;
                while (queue.Any())
                {
                    facet = queue.Dequeue();

                    if (!visited.Contains(facet))
                    {
                        visited.Add(facet);
                        return true;
                    }
                }
                return false;
            }

            private bool AddIfNotExist(TriangleFacet facet)
            {
                if (visited.Contains(facet))
                    return false;

                queue.Enqueue(facet);

                return true;
            }
            private IEnumerable<TriangleFacet> GetEdgesOfTetra(Node[] nodes)
            {
                var a = (ToVector3(nodes[0]), nodes[0]);
                var b = (ToVector3(nodes[1]), nodes[0]);
                var c = (ToVector3(nodes[2]), nodes[0]);
                var d = (ToVector3(nodes[3]), nodes[0]);

                yield return GetEdgeOfTetra(a, b, c, d);
                yield return GetEdgeOfTetra(d, b, c, a);
                yield return GetEdgeOfTetra(a, d, c, b);
                yield return GetEdgeOfTetra(a, b, d, c);
            }
            private TriangleFacet GetEdgeOfTetra((Vector3 v, Node n) a, (Vector3 v, Node n) b, (Vector3 v, Node n) c, (Vector3 v, Node n) d)
            {
                var nabc = Vector3.Cross(a.v - b.v, a.v - c.v);
                nabc *= Math.Sign(Vector3.Dot(nabc, a.v - d.v));
                return new TriangleFacet(a, b, c, nabc);
            }

        }

        interface ISearcher
        {
            Node GetNearestPoint(Point3D target);
            Node GetNearestPoint(TriangleFacet facet);
            void AddPoints(IEnumerable<IElement2D> set);
            void Add(Tetra tetra);
        }


        // пока оставил для отладки
        private class SimpleSearcher : ISearcher
        {
            private const double EPS = 10E-4;
            private List<Node> points;
            private Dictionary<int, List<Tetra>> mapOfTetra = new Dictionary<int, List<Tetra>>();
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
                    if (mapOfTetra.TryGetValue(id, out var lst))
                        lst.Add(tetra);
                    else
                        mapOfTetra[id] = new List<Tetra>() { tetra };
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
            public Node GetNearestPoint(TriangleFacet facet)
            {
                Vector3 target = facet.circumcenter;
                var dist = float.MaxValue;
                Node rsl = null;


                var a = facet.a - target;
                var a2 = Vector3.Dot(a, a);
                var an = Vector3.Dot(a, facet.normal);


                foreach (var point in points)
                {
                    var p = ToVector3(point);
                    var r = p - target;
                    var ra = p - facet.a;
                    var rb = p - facet.b;
                    var rc = p - facet.c;
                    var r2 = Vector3.Dot(r, r);
                    var rn = Vector3.Dot(r, facet.normal);
                    //var x = (r2 - a2) / (an - rn);
                    //var x = Math.Abs(-(r2 - a2) / rn);
                    var x = (r2 - a2) / rn;
                    //var sumSq = float.MaxValue;

                    if (Vector3.Dot(r, facet.normal) > EPS) // проверка, что точка в нужной полуплоскости
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
                if (!mapOfTetra.ContainsKey(id))
                    return false;

                foreach (var tetra in mapOfTetra[id])
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
                    ToVector3(basis[0]),
                    ToVector3(basis[1]),
                    ToVector3(basis[2]));
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

        private class OctreeSearcher : ISearcher
        {
            private const double EPS = 10E-4;
            private PointOctree<Node> _tree;
            private Dictionary<Vector3, Node> map;
            private Dictionary<int, List<Tetra>> mapOfTetra = new Dictionary<int, List<Tetra>>();
            private float _initialSize;
            public OctreeSearcher(IReadOnlyList<IElement2D> set)
            {
                _initialSize = GetModelSize(set); 
                var initialPosition = ToVector3(set.First().GetCoordinates().First());
                _tree = new PointOctree<Node>(_initialSize, initialPosition, (float)EPS);
                AddPoints(set);
            }

            public void Add(Tetra tetra)
            {
                foreach (var id in tetra.GetVertexes().Select(x => x.Number))
                {
                    if (mapOfTetra.TryGetValue(id, out var lst))
                        lst.Add(tetra);
                    else
                        mapOfTetra[id] = new List<Tetra>() { tetra };
                }
            }

            public void AddPoints(IEnumerable<IElement2D> set)
            {
                map = new Dictionary<Vector3, Node>();
                foreach (var elem in set)
                {
                    var points = elem.GetVertexes().ToArray();
                    foreach (var point in points)
                    {
                        var v = ToVector3(point as Node);
                        if (!map.ContainsKey(v))
                            map[v] = point as Node;
                    }
                }
                foreach (var p in map)
                {
                    _tree.Add(p.Value, p.Key);
                }
            }

            private float GetModelSize(IReadOnlyList<IElement2D> secSet)
            {
                var points = secSet.First().GetCoordinates().ToArray();
                var size = points[1].Sub(points[0]);
                return (Math.Abs(size._x) + Math.Abs(size._y) + Math.Abs(size._z)) * 0.33f;
            }

            public Node GetNearestPoint(Point3D target)
            {
                if (_tree.Count == 0)
                    return null;

                var dist = float.MaxValue;
                Node rsl = null;
                var size = _initialSize;

                while (true)
                {
                    var points = _tree.GetNearby(ToVector3(target), size);

                    foreach (var point in points)
                    {
                        var sumSq = point.Position.Sub(target).GetSquareSumCoords();
                        if (sumSq < dist)
                        {
                            dist = sumSq;
                            rsl = point;
                        }
                    }

                    if(rsl!= null) 
                        return rsl;

                    size *= 2;
                }
            }


            /// <summary>
            /// Точка, находящаяся со стороны нормали и такая, что:
            ///   1. можно достроить грань до тетраэдра не пересекая добавленных тетраэдров
            ///   2. описанная сфера тетраэдра включает наименьшее число точек со стороны нормали.
            /// 
            /// </summary>
            /// <param name="facet"></param>
            /// <returns></returns>
            public Node GetNearestPoint(TriangleFacet facet)
            {
                Vector3 target = facet.circumcenter;
                Vector3 centre = (facet.a + facet.b + facet.c) / 3;
                var dist = float.MaxValue;
                Node rsl = null;


                var a = facet.a - target;
                var a2 = Vector3.Dot(a, a);
                var an = Vector3.Dot(a, facet.normal);

                var size = _initialSize;
                var points = _tree.GetNearby(target, size);
                while (true)
                {
                    foreach (var point in points)
                    {
                        var p = ToVector3(point.Position);
                        var r = p - target;
                        var ro = p - centre; // к центру треугольника
                        var r2 = Vector3.Dot(r, r);
                        var rn = Vector3.Dot(r, facet.normal);

                        if (Math.Abs(rn) < EPS)
                            continue;

                        var x = (r2 - a2) / rn;

                        if (x < dist)
                            if (Vector3.Dot(r, facet.normal) > EPS) // проверка, что точка в нужной полуплоскости
                                if (!CrossAnyTetra(-ro, point.Number)) // проверка, что вектор ro не пересекает уже существующий тетраэдр
                                {
                                    var ra = p - facet.a;
                                    var rb = p - facet.b;
                                    var rc = p - facet.c;
                                    if (!CrossAnyTetra(-ra, point.Number) && !CrossAnyTetra(-rb, point.Number) && !CrossAnyTetra(-rc, point.Number))
                                    {
                                        dist = x;
                                        rsl = point;
                                    }
                                }


                    }

                    if (rsl != null)
                    {
                        var p = ToVector3(rsl.Position);
                        if ((p - target).LengthSquared() * 4 < size * size)
                            return rsl;
                    }
                    else
                    {
                        if(size > _initialSize * 4)
                            return null;
                    }

                    size *= 2;
                }
            }

            private bool CrossAnyTetra(Vector3 r, int id)
            {
                if (!mapOfTetra.ContainsKey(id))
                    return false;

                foreach (var tetra in mapOfTetra[id])
                {
                    var (a, b, c) = GetBasis(tetra, id);
                    if (IsVectorInTetra(r, a, b, c))
                        return true;
                }

                return false;
            }

            /// <summary>
            /// true, усли вектор r содержится в секторе, образованном векторами a, b, c
            /// </summary>
            /// <param name="r"></param>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="c"></param>
            /// <returns></returns>
            private bool IsVectorInTetra(Vector3 r, Vector3 a, Vector3 b, Vector3 c)
            {
                var matrix = new double[,]
                {
                    { a.X, b.X, c.X },
                    { a.Y, b.Y, c.Y },
                    { a.Z, b.Z, c.Z }
                };

                var vec = new double[] { r.X, r.Y, r.Z };

                var koeff = SolveSlau(matrix, vec);
                
                return koeff[0] > EPS && koeff[1] > EPS && koeff[2] > EPS;
            }

            private (Vector3 a, Vector3 b, Vector3 c) GetBasis(Tetra tetra, int id)
            {
                var p = tetra.GetVertexes().Single(w => w.Number == id).Position;
                var basis = tetra.GetVertexes().Where(w => w.Number != id).Select(w => w.Position.Sub(p)).ToArray();
                return (
                    ToVector3(basis[0]),
                    ToVector3(basis[1]),
                    ToVector3(basis[2]));
            }

            private static double[] SolveSlau(double[,] matrix, double[] vec)
            {
                Matrix<double> A = DenseMatrix.OfArray(matrix);
                MathNet.Numerics.LinearAlgebra.Vector<double> B = DenseVector.OfArray(vec);

                var c = A.Solve(B);
                return new double[] { c[0], c[1], c[2] };
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
            if (fistSet.Count < secSet.Count)
                return CreateTetras(secSet, fistSet);
            else
                return CreateTetras(fistSet, secSet);
        }

        private List<IElement3D> CreateTetras(IReadOnlyList<IElement2D> set1, IReadOnlyList<IElement2D> set2)
        {
            List<IElement3D> rsl = new List<IElement3D>();

            var points = set2.First().GetCoordinates().ToArray();
            var plane2 = new Geometry.Plane(points[0], points[1], points[2]);

            EdgeQueue edgeQueue = new EdgeQueue();

            ISearcher searcher = new OctreeSearcher(set2);

            foreach (var elem in set1)
            {
                // 1. находим центр описанной окружности 
                var triange = elem as Triangle;
                var edge = new TriangleFacet(triange);
                Point3D circumcenter = CalcCircumcenter(triange);

                edgeQueue.AddAsVisited(edge);
                // 2. находим проекцию центра описанной окружности на плоскость второго множества
                Point3D projection = circumcenter.GetPointProectionOnPlane(plane2);
                // 3. Находим вершину, ближайшую к p
                Node[] nodes = new Node[4];
                Array.Copy(elem.GetVertexes().ToArray(), nodes, 3);
                nodes[3] = searcher.GetNearestPoint(projection);
                // 4. добавляем грани тетраэдра в очередь граней
                edgeQueue.TryPushEdgesOfTetra(nodes);

                var tetra = new Tetra(_number++, nodes);

                rsl.Add(tetra);
                searcher.Add(tetra);

            }
            searcher.AddPoints(set1);

            // 5. По одной достаем грани и строим для них тетраэдры
            while (edgeQueue.TryPop(out var edge))
            {
                var node = searcher.GetNearestPoint(edge);
                if (node != null)
                {
                    var nodes = new Node[4] { edge.na, edge.nb, edge.nc, node };

                    var tetra = new Tetra(_number++, nodes);

                    if (edgeQueue.TryPushEdgesOfTetra(nodes))
                    {
                       rsl.Add(tetra);
                       searcher.Add(tetra);
                    }
                }
            }

            return rsl;
        }

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