using Geometry;
using Model;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using ModelInterfaces.MeshObjects;

namespace ModelController.MeshObjsUtility
{
    public class FindBoundaryEdges
    {
        IModelData modelData;

        public FindBoundaryEdges(IModelData modelData)
        {
            this.modelData = modelData;
        }
        /// <summary>
        /// Find
        /// </summary>
        /// <returns></returns>
        public List<string> Find()
        {
            var boundedEdges = new List<string>();

            var els = modelData.ObjectData.GetObjects(ObjType.Элемент).
                Select(x => (ISurfaceElement)x).ToArray();

            var finder = new InterfacedNodesFinder();
            var interfacedNodes = finder.Find(els);

            //List<int> openSurfNodes = FindOpenSurfacedNodes(objects);

            //var interfacedNodes = FindInterfaceNodes(objects, openSurfacedNodes);

            foreach (var interfacedNode in interfacedNodes)
            {
                //edge, <el,surf>
                var sharedEdges = new Dictionary<string, Dictionary<IElement, List<IElementSurface>>>();
                foreach (var sharedElement in interfacedNode.Value)
                {
                    var element = els.Find(sharedElement);

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
                    var normalsCoords = elSurf.CalcNormal();//GetSideNormal(elNumb, elSurf);
                    vec.Add(normalsCoords);
                }
            }

            var angle = Math.Abs(Vector.GetAngleVectors(vec[0], vec[1]));
            if (angle < 0.8f)
            {
                return true;
            }
            else return false;
        }
    }
}
