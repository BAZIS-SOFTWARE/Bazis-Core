using Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using ModelInterfaces;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ExtensionsLib;

namespace Model.MeshObjects
{

    public abstract class Element3D : Element, IElement3D
    {
        private List<ElementSurface> surfaces = new List<ElementSurface>();
        public Element3D(int number, INode[] nodes) : base(number, nodes)
        {
            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);
            
            var centr = CalcCentr();
            var surfNumber = 0;
            foreach (var surfNodes in SurfaceFinder.FindSurfaceNodes(nodes))
            {
                var surface = new ElementSurface(number,surfNumber, surfNodes);

                var normal = surface.CalcNormal();
                var plane = new Plane(surface[0].Position, normal);
                var proj = centr.GetPointProectionOnPlane(plane);
                var vec = proj.Sub(centr);
                var res = Geometry.Vector.DotProd(normal, vec);

                if (res < 0)
                    surface.ReverseVertexes();

                surfaces.Add(surface);
                surfNumber++;
            }
        }

        public Element3D(int number, INode[] nodes,int point) : this(number, nodes)
        {


        }
        /// <inheritdoc/>


        public override bool ViewState
        {
            get => base.ViewState;
            set
            {
                base.ViewState = value;
                foreach (var surface in surfaces)
                {
                    surface.ViewState = value;
                }
            }
        }


        /// <inheritdoc/>
        public override ObjType ObjType { get; } = ObjType.Элемент3D;
        /// <inheritdoc/>

        public int NumberOfSides { get { return surfaces.Count(); } }

        /// <inheritdoc/>

        public override Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var node in nodes)
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }
/// <inheritdoc/>

        public IEnumerable<Point3D> GetSurfacesNormals()
        {
            foreach (var surface in surfaces)
            {
                yield return surface.CalcNormal();
            }
        }

        /// <summary>
        /// Calc volume
        /// </summary>
        /// <returns></returns>
        public abstract float CalcVolume();
/// <inheritdoc/>


/// <inheritdoc/>

        public IEnumerable<IElementSurface> GetSurfaces()
        {
            foreach (var surface in surfaces)
            {
                yield return surface;
            }
        }

        protected IEnumerable<INode> SortNodes(int mainSurfaceNodesNumber)
        {
            var result = new List<INode>();
            var edges = new HashSet<Tuple<INode, INode>>();
            INode masterStartNode = null;
            var masterSurface = new HashSet<INode>();
            var graph = new Dictionary<INode, HashSet<INode>>();
            foreach (var surface in surfaces)
            {
                foreach(var pair in surface.GetVertexes().Bigrams(true))
                {
                    if (!graph.ContainsKey(pair.Item1))
                        graph.Add(pair.Item1, new HashSet<INode>());
                    if (!graph.ContainsKey(pair.Item2))
                        graph.Add(pair.Item2, new HashSet<INode>());
                    graph[pair.Item1].Add(pair.Item2);
                    graph[pair.Item2].Add(pair.Item1);
                }
                if(surface.NumberOfPoints == mainSurfaceNodesNumber && masterStartNode == null)
                {
                    masterStartNode = surface[0];
                    masterSurface = surface.GetVertexes().ToHashSet();
                }
            }
            var currentNode = masterStartNode;
            result.Add(currentNode);
            for (var i = 0; i < mainSurfaceNodesNumber - 1; i++)
            {
                var nextNode = graph[currentNode].Where(x => masterSurface.Contains(x) && !result.Contains(x)).First();
                result.Add(nextNode);
                currentNode = nextNode;
            }
            currentNode = graph[masterStartNode].Where(x => !result.Contains(x)).First();
            result.Add(currentNode);
            for (var i = 0; i < mainSurfaceNodesNumber - 1; i++)
            {
                var nextNode = graph[currentNode].Where(x => !result.Contains(x) && graph[result[i + 1]].Contains(x)).First();
                result.Add(nextNode);
                currentNode = nextNode;
            }
            return result;
        }
    }
}
