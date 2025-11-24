using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;

namespace Model.MeshObjects
{
    public abstract class Element : ModelObject, IElement
    {
        protected List<Node> nodes = new List<Node>();

        protected List<IIntegralPoint> intPoints = new List<IIntegralPoint>();

        //protected List<Matrix<double>> jacobiMatrixes = new List<Matrix<double>>();


        /// <summary>
        /// Element
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        /// <exception cref="Exception"></exception>
        public Element(int number, Node[] nodes) : base(number)
        {
            if (nodes.Length == 0)
                throw new Exception("Массив узлов не может быть пустой!");

            foreach (var node in nodes)
            {
                node.AddElement(this);
                this.nodes.Add(node);
            }
        }

        public override bool ExistState
        {
            get => base.ExistState;
            set
            {
                base.ExistState = value;
                if (!value)
                {
                    foreach (var node in nodes)
                        node.RemoveElement(this);
                }
            }
        }
/// <inheritdoc/>

        public int GetNodeIndex(Node node)
        {
            return nodes.IndexOf(node);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        /// <param name="intPoints"></param>
        /// Кол-во точек интегрирования
        /// <exception cref="Exception"></exception>
        public Element(int number, Node[] nodes, int intPoints) : this(number, nodes)
        {

        }

        /// <summary>
        /// this
        /// </summary>
        /// <param name="nodeInd"></param>
        /// <returns></returns>
        public virtual Node this[int nodeInd]
        {
            get { return nodes[nodeInd]; }
            set { nodes[nodeInd] = value; }
        }



        /// <inheritdoc/>

        public override int NumberOfPoints { get { return nodes.Count; } }
/// <inheritdoc/>

        public abstract ElementKind ElementKind { get; }

        /// <inheritdoc/>
        public abstract int Level { get; set; }


        /// <inheritdoc/>
        public IEnumerator<Node> GetEnumerator()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }

        /// <inheritdoc/>

        public override IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var node in nodes)
            {
                yield return node.Position;
            }
        }
/// <inheritdoc/>

        public override string ToString()
        {
            var strArr = nodes.Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", strArr);

            return $"{base.ToString()} {ElementKind} {Level} : {str}";
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

        public abstract Matrix<double> CalcJacobi(IIntegralPoint integralPoint);
/// <inheritdoc/>

        public IEnumerable<IIntegralPoint> GetIntegralPoints()
        {
            foreach (var point in intPoints)
            {
                yield return point;
            }
        }

        /// <inheritdoc/>

        public abstract double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint);
/// <inheritdoc/>

        public abstract double[] GetFormFunctions(IIntegralPoint intPoint);

/// <inheritdoc/>

        public abstract IEnumerable<IntegralPoint> CreateIntegrationPoints(int level);
    }
}
