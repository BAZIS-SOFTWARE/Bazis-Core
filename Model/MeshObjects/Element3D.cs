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
using ModelInterfaces.ObjectsComparers;

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

        public IEnumerable<INode> SortNodesEx(int mainSurfaceNodesNumber)
        {
            // TO DO 
            // ПРЕДЛОЖИТЬ РЕАЛИЗАЦИЮ СОРТИРОВКИ УЗЛОВ ИСПОЛЬЗУЯ ПРЕИМУЩЕСТВЕННО ГРАФ ЭЛЕМЕНТА
            return null;
        }
/// <inheritdoc/>

        public IEnumerable<INode> SortNodes(int mainSurfaceNodesNumber)
        {
            var result = new List<INode>();
            var graph = BuildGraph();
            var masterSurface = GetMasterSurface(mainSurfaceNodesNumber);          

            result.AddRange(masterSurface.GetVertexes());

            var slaveStartNode = graph[masterSurface[0]].Where(x => !masterSurface.GetVertexes().Contains(x)).First();
            //находим противолеж. поверхность
            var slaveNodes = new List<INode>();
            var slaveNodeIndex = 0;
            foreach (var item in surfaces)
            {
                var res = item.GetVertexes().Intersect(masterSurface.GetVertexes());

                if(res.Count() == 0)
                {
                    slaveNodes = item.GetVertexes().ToList();
                    slaveNodes.Reverse();
                    slaveNodeIndex = slaveNodes.IndexOf(slaveStartNode);
                    break;
                }

            }
          
            // проходим после индекса до конца
            for (int i = slaveNodeIndex; i < slaveNodes.Count; i++)
                result.Add(slaveNodes[i]);
            // проходим с начала до индекса
            for (int i = 0; i < slaveNodeIndex; i++)
                result.Add(slaveNodes[i]);

            return result;
        }

        private Dictionary<INode, HashSet<INode>> BuildGraph()
        {
            var graph = new Dictionary<INode, HashSet<INode>>();
            foreach (var surface in surfaces)
            {
                foreach (var pair in surface.GetVertexes().Bigrams(true))
                {
                    if (!graph.ContainsKey(pair.Item1))
                        graph.Add(pair.Item1, new HashSet<INode>());
                    if (!graph.ContainsKey(pair.Item2))
                        graph.Add(pair.Item2, new HashSet<INode>());
                    graph[pair.Item1].Add(pair.Item2);
                    graph[pair.Item2].Add(pair.Item1);
                }
            }
            return graph;
        }

        private IElementSurface GetMasterSurface(int masterSurfaceNodesNumber)
        {

            IElementSurface masterSurface = null;
            foreach (var surface in surfaces)
            {
                if (surface.NumberOfPoints == masterSurfaceNodesNumber)
                {
                    masterSurface = surface;
                    break;
                }
            }
            return masterSurface;
        }
    }
}
