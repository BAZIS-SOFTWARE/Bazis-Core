using Geometry;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.MeshObjects
{
    /// <summary>
    /// Surface class
    /// </summary>
    public class ElementSurface : ISurface<Node>
    {
        Node[] nodes;
        readonly IElement owner;
        IElement neighbour;

        /// <summary>
        /// Surface constructor
        /// </summary>
        /// <param name="owner">Элемент, которому принадлежит поверхность</param>
        /// <param name="surfaceNumber">Номер поверхности внутри элемента</param>
        /// <param name="nodes">Узлы поверхности</param>
        public ElementSurface(IElement owner, int surfaceNumber, Node[] nodes)
        {
            if (nodes.Length < 3)
                throw new Exception("Для создания поверхности требуется не меньше 3-х узлов!");

            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            //nodes.RoundSort(); //Anticlockwise(nodes);
            this.nodes = nodes;
            this.owner = owner;

            Number = surfaceNumber;
        }
        
        /// <summary>
        /// this
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public Node this[int ind]
        {
            get { return nodes[ind]; }
            set { nodes[ind] = value; }
        }
        /// <inheritdoc/>

        public override string ToString()
        {
            var strArr = nodes.Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", strArr);

            return $"{base.ToString()} : {str}";
        }


        /// <inheritdoc/>

        public int NumberOfPoints { get { return nodes.Length; } }
/// <inheritdoc/>

        public int ElementNumber => owner.Number;

        /// <inheritdoc/>
        public int Number { get; }

        /// <summary>
        /// Поверхность внутренняя, если у неё есть живой сосед.
        /// Ничего не хранит: гашение соседа само делает поверхность внешней.
        /// </summary>
        public bool IsInner => neighbour != null && neighbour.ExistState;

        /// <summary>
        /// Перечисляет владельцев поверхности: сначала собственный элемент,
        /// затем соседний, если он есть.
        /// </summary>
        public IEnumerable<IElement> GetElements()
        {
            yield return owner;

            if (neighbour != null)
                yield return neighbour;
        }

        /// <summary>
        /// Строит ключ сопоставления совпадающих поверхностей -
        /// отсортированные номера узлов. Не кэшируется: номера узлов изменяемы.
        /// </summary>
        public int[] CreateNodesKey()
        {
            var key = new int[nodes.Length];
            for (var index = 0; index < nodes.Length; index++)
                key[index] = nodes[index].Number;

            Array.Sort(key);
            return key;
        }

        /// <summary>
        /// Элемент, которому принадлежит поверхность.
        /// </summary>
        internal IElement Owner => owner;

        /// <summary>
        /// Задаёт соседний элемент. Вызывается только связыванием поверхностей.
        /// </summary>
        internal bool SetNeighbour(IElement element)
        {
            if (ReferenceEquals(neighbour, element))
                return false;
            neighbour = element;
            return true;
        }

        /// <inheritdoc/>

        public Point3D CalcNormal()
        {
            var fvectors = nodes[1].Position.Sub(nodes[0].Position);
            var svectors = nodes[2].Position.Sub(nodes[0].Position);
            return Vector.CrossProd(fvectors, svectors);
        }
/// <inheritdoc/>

        public double CalcSquare()
        {
            var square = 0.0f;
            for (int i = 0; i < NumberOfPoints - 2; i++)
            {
                var v1 = this[i + 1].Position.Sub(this[0].Position);
                var v2 = this[i + 2].Position.Sub(this[0].Position);

                var vProd = Vector.CrossProd(v1, v2);
                square += Math.Abs(Vector.GetVectorLength(vProd) / 2);
            }


            return square;
        }
        /// <inheritdoc/>

        public IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var node in nodes)
            {
                yield return node.Position;
            }
        }
        /// <inheritdoc/>

        public bool Equals(ElementSurface other)
        {
            var points = other.GetCoordinates().ToArray();
            if (points.Contains(nodes[0].Position, new Point3DEqualityComparer()) &&
points.Contains(nodes[1].Position, new Point3DEqualityComparer()) &&
points.Contains(nodes[2].Position, new Point3DEqualityComparer()))
                return true;
            else
                return false;
        }
        /// <summary>
        /// GetDistance
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public float GetDistance(Point3D point)
        {
            var normal = CalcNormal();
            var nNormal = Vector.GetVectorNorm(normal);
            var temp = point.Sub(nodes[0].Position);

            return nNormal._x * temp._x + nNormal._y * temp._y + nNormal._z * temp._z;
        }
        /// <inheritdoc/>

        public Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var point in GetCoordinates())
            {
                center = center.Sum(point);
            }
            return center.Div(NumberOfPoints);
        }
/// <inheritdoc/>

        public Segment3D[] GetLinesFromSurface()
        {
            var lines = new Segment3D[nodes.Length];
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                lines[i] = new Segment3D(nodes[i].Position, nodes[i + 1].Position);
            }
            lines[nodes.Length - 1] = new Segment3D(nodes[nodes.Length - 1].Position, nodes[0].Position);
            return lines;
        }

        /// <inheritdoc/>

        public IEnumerable<Node> GetVertexes()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }
/// <inheritdoc/>

        public void ReverseVertexes()
        {
            nodes = nodes.Reverse().ToArray();
        }
/// <inheritdoc/>

        public int GetVertexIndex(Node vertex)
        {
            return Array.IndexOf(nodes, vertex);
        }
    }
}
