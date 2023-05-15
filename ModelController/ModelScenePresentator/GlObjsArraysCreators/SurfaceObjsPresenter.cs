using Model;
using Model.Interfaces;
using ModelController.ModelScenePresentator.GlObjsVertexCreators;
using ModelController.ModelScenePresentator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelController.ModelScenePresentator.GlObjsArraysCreators
{
    public class SurfaceObjsPresenter : ObjsPresenter
    {
        ISurfaceObject [] surfaceElements;

        public override IModelObject this[int index]
        {
            get { return surfaceElements[index]; }
        }

        public SurfaceObjsPresenter(ISurfaceObject[] surfaceElements)
        {
            this.surfaceElements = surfaceElements;
        }

        //public override IEnumerable<IModelObject> GetObjs()
        //{
        //    foreach (var surfaceElement in surfaceElements)
        //    {
        //        yield return surfaceElement;
        //    }
        //}

        public void HideInsideSurfaces()
        {
            var surfDict = new Dictionary<string, List<Surface>>();

            foreach (var element in surfaceElements)
            {
                if (element.ViewState)
                    foreach (var surface in element.GetSurface())
                    {
                        var numbers = surface.GetNodes().Select(x => x.Number).ToArray();

                        Array.Sort(numbers);
                        var surfInd = string.Join("", numbers);   //numbers[0].ToString() + numbers[1].ToString() + numbers[2].ToString();

                        if (!surfDict.ContainsKey(surfInd))
                        {
                            surface.ViewState = true;
                            surfDict.Add(surfInd, new List<Surface>() { surface });
                        }

                        else
                        {
                            surface.ViewState = false;
                            surfDict[surfInd][0].ViewState = false;
                            surfDict[surfInd].Add(surface);
                        }
                    }
            }

        }

        public override Tuple<int, int, int> CreateIndexes()
        {
            var indexes = 0;
            var vertexes = 0;
            var edges = 0;

            foreach (var surfaceObj in surfaceElements)
            {
                if(surfaceObj.ViewState)
                    foreach (var surface in surfaceObj.GetSurface())
                    {
                        if (surface.ViewState)
                        {
                            vertexes = vertexes + (surface.NumberOfPoints - 2) * 3 * 3;
                            indexes = indexes + (surface.NumberOfPoints - 2) * 3;
                            edges = edges + (surface.NumberOfPoints - 2) * 3;
                        }
                    }
            }


            return new Tuple<int, int, int>(indexes, vertexes, edges);
        }

        public override int[] CreatePointers(int indexLength)
        {
            var ptr = new int[indexLength];
            var stride = 0;

            foreach (var surfaceObj in surfaceElements)
            {
                if (surfaceObj.ViewState)
                {
                    foreach (var surface in surfaceObj.GetSurface())
                    {
                        if (surface.ViewState)
                        {
                            var length = (surface.NumberOfPoints - 2) * 3;
                            for (int m = 0; m < length; m++)
                            {
                                ptr[stride + m] = stride + m;
                            }
                            stride = stride + length;
                        }
                    }
                }
            }

            return ptr;
        }

        public override float[] CreateVertexes(int vertexLength,string vertexType)
        {
            var vertexArray = new float[vertexLength];
            var stride = 0;

            ISurfaceObjsVertexCreator vertexCreator;

            if (vertexType == "цвет")
                vertexCreator = new ColorSurfaceObjsCreator();
            else if (vertexType == "координаты")
                vertexCreator = new CoordsSurfaceObjsCreator();
            else vertexCreator = new NormalsSurfaceObjsCreator();

            foreach (var surfaceElement in surfaceElements)
            {
                if (surfaceElement.ViewState)
                {
                    vertexCreator.Create(ref stride, vertexArray, surfaceElement);
                }
            }

            return vertexArray;
        }

        public override bool[] CreateEdgeFlags(int edgesLength)
        {
            var edgesArray = new bool[edgesLength];
            var stride = 0;

            var vertexCreator = new EdgesSurfaceObjsCreator();
            vertexCreator.Create(ref stride, edgesArray, surfaceElements);  
            return edgesArray;
        }

        internal void ShowInsideSurfaces()
        {
            foreach (var element in surfaceElements)
            {
                if (element.ViewState)
                    foreach (var surface in element.GetSurface())
                        surface.ViewState = true;
            }
        }

        public override int Count()
        {
            return surfaceElements.Length;
        }    

        public override IEnumerable<IModelObject> GetObjs()
        {
            foreach (var surfaceElement in surfaceElements)
            {
                yield return surfaceElement;
            }
        }
    }
}
