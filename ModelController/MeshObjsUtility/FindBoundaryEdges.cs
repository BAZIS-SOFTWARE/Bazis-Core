using Geometry;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelController.MeshObjsUtility
{
    public class FindBoundaryEdges
    {
        ModelData modelData;

        public FindBoundaryEdges(ModelData modelData)
        {
            this.modelData = modelData;
        }

        public List<string> Find()
        {
            var boundedEdges = new List<string>();

            var els = modelData.ObjectData.FindMany<SurfaceElement>();

            var finder = new FindInterfacedNodes(els.ToArray());
            var interfacedNodes = finder.Find();

            //List<int> openSurfNodes = FindOpenSurfacedNodes(objects);

            //var interfacedNodes = FindInterfaceNodes(objects, openSurfacedNodes);

            foreach (var interfacedNode in interfacedNodes)
            {
                //edge, <el,surf>
                var sharedEdges = new Dictionary<string, Dictionary<Element, List<Surface>>>();
                foreach (var sharedElement in interfacedNode.Value)
                {
                    var element = (SurfaceElement)modelData.ObjectData.Find(sharedElement);

                    foreach (var surface in element.GetSurface())
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
                                    sharedEdges.Add(edge, new Dictionary<Element, List<Surface>>()
                                    { { element, new List<Surface>() { surface } } });
                                else
                                {
                                    if (!sharedEdges[edge].ContainsKey(element))
                                        sharedEdges[edge].Add(element, new List<Surface>() { surface });
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



        private bool CheckAdjacentAngle(Dictionary<Element, List<Surface>> adjElems)
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
