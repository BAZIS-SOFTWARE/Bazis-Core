using Geometry;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Model.Utilities;
using Geometry.Exteisions;

namespace Model.MeshObjects
{

    public abstract class Element3D : Element, IElement3D
    {
        protected List<ElementSurface> surfaces = new List<ElementSurface>();
        public Element3D(int number, Node[] nodes) : base(number, nodes)
        {
            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);


            surfaces = SurfacesCreator.CreateSurfacesFromSetOfNodes(number, nodes);
        }



        public Element3D(int number, Node[] nodes,int point) : this(number, nodes)
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
        public abstract double CalcVolume();
/// <inheritdoc/>


/// <inheritdoc/>

        public IEnumerable<ElementSurface> GetSurfaces()
        {
            foreach (var surface in surfaces)
            {
                yield return surface;
            }
        }


/// <inheritdoc/>

        public static List<Node> SortNodes(int mainSurfaceIndex, List<ElementSurface> surfaces)
        {
            var result = new List<Node>();
            var graph = BuildGraph(surfaces);
            var masterSurface = surfaces[mainSurfaceIndex];          

            result.AddRange(masterSurface.GetVertexes());

            foreach (var item in masterSurface.GetVertexes())
            {
                var slaveNode = graph[item].Where(x => !masterSurface.GetVertexes().Contains(x)).First();
                result.Add(slaveNode);
            }

            return result;
        }
        /// <summary>
        /// Создать граф элемента
        /// </summary>
        /// <param name="surfaces"></param>
        /// <returns></returns>
        public static Dictionary<Node, HashSet<Node>> BuildGraph(IEnumerable<ElementSurface> surfaces)
        {
            var graph = new Dictionary<Node, HashSet<Node>>();
            foreach (var surface in surfaces)
            {
                foreach (var pair in surface.GetVertexes().Bigrams(true))
                {
                    if (!graph.ContainsKey(pair.Item1))
                        graph.Add(pair.Item1, new HashSet<Node>());
                    if (!graph.ContainsKey(pair.Item2))
                        graph.Add(pair.Item2, new HashSet<Node>());
                    graph[pair.Item1].Add(pair.Item2);
                    graph[pair.Item2].Add(pair.Item1);
                }
            }
            return graph;
        }
        /// <summary>
        /// GetMasterSurface
        /// </summary>
        /// <param name="masterSurfaceNodesNumber"></param>
        /// <param name="surfaces"></param>
        /// <returns></returns>
        public static ElementSurface GetMasterSurface(int masterSurfaceNodesNumber, IEnumerable<ElementSurface> surfaces)
        {

            ElementSurface masterSurface = null;
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
