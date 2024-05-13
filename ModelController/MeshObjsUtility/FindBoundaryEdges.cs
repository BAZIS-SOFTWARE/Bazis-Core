using Geometry;
using Model;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using ModelInterfaces.MeshObjects;
using ModelControllerInterfaces;
using Model.MeshObjects;
using System.Drawing;
using Model.GeometryObjects;

namespace ModelController.MeshObjsUtility
{
    /// <summary>
    /// FindBoundaryEdges
    /// </summary>
    public class FindBoundaryEdges : IBoundaryEdgesFinder
    {
        /// <inheritdoc/>
        public IEnumerable<ILineObject<INode>> CreateBoundaryEdges(List<string> linesNodes ,IModelData modelData)
        {
            var counter = 0;
            foreach (var lineNodes in linesNodes)
            {
                var numbers = lineNodes.Split(' ');
                var po = Convert.ToInt32(numbers[0]);
                var p1 = Convert.ToInt32(numbers[1]);
                var node0 = modelData.ObjectData.NodeCollection.Find(po);
                var node1 = modelData.ObjectData.NodeCollection.Find(p1);
                var beam = new Beam(counter, new INode[] { node0, node1 })
                { MasterColor = Color.Red };
                yield return beam;
                counter++;
            }
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
                var sharedEdges = new Dictionary<string, Dictionary<IElement, List<IElementSurface>>>();
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
                                    sharedEdges.Add(edge, new Dictionary<IElement, List<IElementSurface>>()
                                    { { element, new List<IElementSurface>() { surface } } });
                                else
                                {
                                    if (!sharedEdges[edge].ContainsKey(element))
                                        sharedEdges[edge].Add(element, new List<IElementSurface>() { surface });
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

        private bool CheckAdjacentAngle(Dictionary<IElement, List<IElementSurface>> adjElems)
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
