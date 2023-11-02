using ModelInterfaces;
using ModelController.ModelScenePresentator.GlObjsVertexCreators;
using ModelController.ModelScenePresentator.Interfaces;
using ModelControllerInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ModelController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// SurfaceObjsPresenter
    /// </summary>
    public class SurfaceObjsPresenter : IObjsPresenter
    {
        IEnumerable<ISurfaceObject> surfaceElements;
/// <inheritdoc/>

        public PresenterType PresenterType { get; } = PresenterType.Surface;

        /// <summary>
        /// SurfaceObjsPresenter
        /// </summary>
        /// <param name="surfaceElements"></param>
        public SurfaceObjsPresenter(IEnumerable<ISurfaceObject> surfaceElements)
        {
            this.surfaceElements = surfaceElements;
        }

        
/// <inheritdoc/>

        public Tuple<int, int, int,int> CreateIndexes()
        {
            var pointers = 0;
            var vertexes = 0;
            var edges = 0;
            var colors = 0;

            foreach (var surfaceObj in surfaceElements)
            {
                if(surfaceObj.ViewState)
                    foreach (var surface in surfaceObj.GetSurface())
                    {
                        if (surface.ViewState)
                        {
                            vertexes = vertexes + (surface.NumberOfPoints - 2) * 3 * 3;
                            pointers = pointers + (surface.NumberOfPoints - 2) * 3;
                            edges = edges + (surface.NumberOfPoints - 2) * 3;
                            colors = colors + (surface.NumberOfPoints - 2) * 3 * 4;
                        }
                    }
            }


            return new Tuple<int, int, int,int>(pointers, vertexes, colors, edges);
        }
/// <inheritdoc/>

        public int[] CreatePointers(int indexLength)
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
/// <inheritdoc/>

        public float[] CreateVertexes(int vertexLength,string vertexType)
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
/// <inheritdoc/>

        public bool[] CreateEdgeFlags(int edgesLength)
        {
            var edgesArray = new bool[edgesLength];
            var stride = 0;

            var vertexCreator = new EdgesSurfaceObjsCreator();
            vertexCreator.Create(ref stride, edgesArray, surfaceElements);  
            return edgesArray;
        }

/// <inheritdoc/>

        public int Count()
        {
            return surfaceElements.Count();
        }    
/// <inheritdoc/>

        public IEnumerator<IModelObject> GetEnumerator()
        {
            foreach (var surfaceElement in surfaceElements)
            {
                yield return surfaceElement;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
