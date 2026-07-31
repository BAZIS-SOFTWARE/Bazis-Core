using Geometry;
using Model;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsFinders;
using Model.MeshObjects;
using OperationalController;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OperationalController.MeshObjsUtility
{
    /// <summary>
    /// FindBoundaryEdges
    /// </summary>
    public class FindBoundaryEdges : IBoundaryEdgesFinder
    {
        /// <inheritdoc/>
        public IEnumerable<ILineObject<Node>> CreateBoundaryEdges(List<string> linesNodes ,IModelData modelData)
        {
            var counter = 0;
            foreach (var lineNodes in linesNodes)
            {
                var numbers = lineNodes.Split(' ');
                var po = Convert.ToInt32(numbers[0]);
                var p1 = Convert.ToInt32(numbers[1]);
                var node0 = modelData.ObjectData.NodesSet[po];
                var node1 = modelData.ObjectData.NodesSet[p1];
                var beam = new Beam(counter, new Node[] { node0, node1 })
                { Color = Color.Red };
                yield return beam;
                counter++;
            }
        }

        /// <summary>
        /// Ищет на сетке ограниченным контуром граничные ребра, возвращая в виде сортированных узлов ребра.
        /// </summary>
        /// <param name="surfaceElements">Элементы</param>
        /// <returns>Список граничных сортированных узлов</returns>
        public List<Node> GetSortedBoundaryEdges(IEnumerable<ISurfaceElement> surfaceElements)
        {
            var nodes = new Dictionary<int, Node>();
            var edges = new Dictionary<string, List<(int,int)>>();

            Action<int, int> registerEdge = (f, l) => 
            {
                var min = Math.Min(f, l);
                var max = Math.Max(f, l);
                var key = $"{min} {max}";
                if (!edges.ContainsKey(key))
                    edges.Add(key, new List<(int, int)>());
                edges[key].Add((min, max));
            };

            foreach (var surf in surfaceElements)
                foreach (var surface in surf.GetSurfaces())
                {
                    var vertices = surface.GetVertexes().ToList();
                    var first = vertices[0];
                    if (!nodes.ContainsKey(first.Number))
                        nodes.Add(first.Number, first);
                    for (var i = 1; i < vertices.Count; ++i)
                    {
                        if (!nodes.ContainsKey(vertices[i].Number))
                            nodes.Add(vertices[i].Number, vertices[i]);
                        registerEdge(first.Number, vertices[i].Number);
                        first = vertices[i];
                    }
                    registerEdge(first.Number, vertices[0].Number);
                }

            var adjNodes = new Dictionary<int, List<int>>();
            var orderedEdges = edges.OrderBy(v => v.Value.Count).Where(v => v.Value.Count == 1).ToList();
            foreach (var item in orderedEdges)
            {
                var value = item.Value[0];
                var first = value.Item1;
                var second = value.Item2;

                if (!adjNodes.ContainsKey(first))
                    adjNodes.Add(first, new List<int>());
                adjNodes[first].Add(second);

                if (!adjNodes.ContainsKey(second))
                    adjNodes.Add(second, new List<int>());
                adjNodes[second].Add(first);
            }

            var result = new List<Node>();
            if (orderedEdges.Count > 0)
            {
                var visited = new HashSet<int>();
                var key = orderedEdges[0].Value[0].Item1;
                for (var i = 0; i < adjNodes.Count; ++i)
                {
                    var adj = adjNodes[key];
                    if (adj.Count > 2)
                        throw new Exception($"У узла с номером {key} {adj.Count} соседей, узел не является граничным");
                    if (!visited.Contains(adj[0]))
                    {
                        visited.Add(adj[0]);
                        key = adj[0];
                        result.Add(nodes[adj[0]]);
                    }
                    else if (!visited.Contains(adj[1]))
                    {
                        visited.Add(adj[1]);
                        key = adj[1];
                        result.Add(nodes[adj[1]]);
                    }
                    else
                        throw new Exception($"Найден цикл, узел {adj[0]} и {adj[1]} уже были посещены");
                }
            }
            return result;
        }

        /// <inheritdoc/>


        public List<string> Find(IEnumerable<ISurfaceElement> surfaceElements)
        {
            var boundedEdges = new List<string>();

            var els = surfaceElements.OrderBy(x => x.Number).ToArray();

            var finder = new InterfacedNodesFinder();
            var interfacedNodes = finder.Find(els);

            foreach (var interfacedNode in interfacedNodes)
            {
                //edge, <el,surf>
                var sharedEdges = new Dictionary<string, Dictionary<IElement, List<ElementSurface>>>();
                foreach (var sharedElement in interfacedNode.Value)
                {
                    var element = ByNumberFinder.Find(els,sharedElement);

                    foreach (var surface in element.GetSurfaces())
                    {
                        if (surface.ViewState)
                        {
                            var numberOfNodes = surface.NumberOfPoints;
                            for (int j = 0; j < numberOfNodes; j++)
                            {
                                var edge = "";
                                if (j < numberOfNodes - 1)
                                {
                                    if (surface[j].Number > surface[j + 1].Number)
                                        edge = surface[j + 1].Number.ToString() + " " + surface[j].Number.ToString();
                                    else
                                        edge = surface[j].Number.ToString() + " " + surface[j + 1].Number.ToString();
                                }
                                else
                                {
                                    if (surface[j].Number > surface[0].Number)
                                        edge = surface[0].Number.ToString() + " " + surface[j].Number.ToString();
                                    else
                                        edge = surface[j].Number.ToString() + " " + surface[0].Number.ToString();
                                }


                                if (!sharedEdges.Keys.Contains(edge))
                                    sharedEdges.Add(edge, new Dictionary<IElement, List<ElementSurface>>()
                                    { { element, new List<ElementSurface>() { surface } } });
                                else
                                {
                                    if (!sharedEdges[edge].ContainsKey(element))
                                        sharedEdges[edge].Add(element, new List<ElementSurface>() { surface });
                                    else sharedEdges[edge][element].Add(surface);

                                    if (CheckAdjacentAngle(sharedEdges[edge]))
                                    {
                                        if (!boundedEdges.Contains(edge))
                                            boundedEdges.Add(edge);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return boundedEdges;
        }

        private bool CheckAdjacentAngle(Dictionary<IElement, List<ElementSurface>> adjElems)
        {
            var vec = new List<Point3D>();

            foreach (var adjElem in adjElems)
            {
                var elNumb = adjElem.Key;
                foreach (var elSurf in adjElem.Value)
                {
                    var normalsCoords = elSurf.CalcNormal();
                    vec.Add(normalsCoords);
                }
            }

            var angle = Math.Abs(Vector.GetCosAngleVectors(vec[0], vec[1]));
            if (angle < 0.8f)
            {
                return true;
            }
            else return false;
        }
    }
}
