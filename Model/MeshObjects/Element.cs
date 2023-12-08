using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces;
using ModelInterfaces.IntegralObject;

namespace Model.MeshObjects
{
    public abstract class Element : ModelObject, IElement
    {
        protected List<Node> nodes = new List<Node>();

        protected List<IIntegralPoint> intPoints = new List<IIntegralPoint>();

        protected List<float> jacobians = new List<float>();


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

            return $"{Number} {ObjType} {ElementKind} {intPoints.Count} : {str}";
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

        public abstract float DetJ(IIntegralPoint integralPoint);
/// <inheritdoc/>

        public IEnumerable<IIntegralPoint> GetIntegralPoints()
        {
            foreach (var point in intPoints)
            {
                yield return point;
            }
        }

/// <inheritdoc/>

        public abstract float[] GetDerivativesFormFunctions(IIntegralPoint intPoint);
/// <inheritdoc/>

        public abstract float[] GetFormFunctions(IIntegralPoint intPoint);

        /// <inheritdoc/>

        public void AddDetJ(float j)
        {
            jacobians.Add(j);
        }
        /// <inheritdoc/>

        public IEnumerable<float> GetDetsJ()
        {
            foreach (var jacobian in jacobians)
            {
                yield return jacobian;
            }
        }
    }
}
