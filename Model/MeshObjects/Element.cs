using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces.IntegralObject;
using ModelInterfaces.MeshObjects;

namespace Model.MeshObjects
{
    public abstract class Element : ModelObject, IElement
    {
        protected List<INode> nodes = new List<INode>();

        protected List<IIntegralPoint> intPoints = new List<IIntegralPoint>();

        protected List<double> jacobians = new List<double>();


        /// <summary>
        /// Element
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        /// <exception cref="Exception"></exception>
        public Element(int number, INode[] nodes) : base(number)
        {
            if (nodes.Length == 0)
                throw new Exception("Массив узлов не может быть пустой!");

            foreach (var node in nodes)
            {
                node.AddElement(this);
                this.nodes.Add(node);
            }
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
        public INode this[int nodeInd] => nodes[nodeInd];

        /// <inheritdoc/>

        public override int NumberOfPoints { get { return nodes.Count(); } }
/// <inheritdoc/>

        public abstract ElementKind ElementKind { get; }


        /// <inheritdoc/>
        public IEnumerator<INode> GetEnumerator()
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

            return $"{base.ToString()} {ElementKind} {intPoints.Count} : {str}";
        }
/// <inheritdoc/>

        public IEnumerable<INode> GetVertexes()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }
        /// <inheritdoc/>

        public abstract double DetJ(IIntegralPoint integralPoint);
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

        public void AddDetJ(double j)
        {
            jacobians.Add(j);
        }
        /// <inheritdoc/>

        public IEnumerable<double> GetDetsJ()
        {
            foreach (var jacobian in jacobians)
            {
                yield return jacobian;
            }
        }
    }
}
