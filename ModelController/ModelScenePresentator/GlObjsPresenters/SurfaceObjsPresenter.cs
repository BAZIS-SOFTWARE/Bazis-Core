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
    public class SurfaceObjsPresenter<T,U> : IObjsPresenter, IEnumerable<ISurfaceObject<T,U>>
        where T : ISurface<U> where U : IPoint
    {
        IEnumerable<ISurfaceObject<T,U>> surfaceObjects;
/// <inheritdoc/>

        public PresenterType PresenterType { get; } = PresenterType.Surface;
/// <inheritdoc/>

        public bool IsVolumeObjs { get; } = false;

        /// <summary>
        /// SurfaceObjsPresenter
        /// </summary>
        /// <param name="surfaceObjects"></param>
        /// <param name="presenterType"></param>
        public SurfaceObjsPresenter(IEnumerable<ISurfaceObject<T,U>> surfaceObjects, bool isVolumeObjs)
        {
            this.surfaceObjects = surfaceObjects;
            IsVolumeObjs = isVolumeObjs;
        }

        
/// <inheritdoc/>

        public Tuple<int, int, int,int> CreateIndexes()
        {
            var pointers = 0;
            var vertexes = 0;
            var edges = 0;
            var colors = 0;

            foreach (var surfaceObj in surfaceObjects)
            {
                if(surfaceObj.ViewState)
                    foreach (var surface in surfaceObj.GetSurfaces())
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

            foreach (var surfaceObj in surfaceObjects)
            {
                if (surfaceObj.ViewState)
                {
                    foreach (var surface in surfaceObj.GetSurfaces())
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

            ISurfaceObjsVertexCreator<T,U> vertexCreator;

            if (vertexType == "цвет")
                vertexCreator = new ColorSurfaceObjsCreator<T,U>();
            else if (vertexType == "координаты")
                vertexCreator = new CoordsSurfaceObjsCreator<T, U>();
            else vertexCreator = new NormalsSurfaceObjsCreator<T, U>();

            foreach (var surfaceElement in surfaceObjects)
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

            var vertexCreator = new EdgesSurfaceObjsCreator<T, U>();
            vertexCreator.Create(ref stride, edgesArray, surfaceObjects);  
            return edgesArray;
        }

/// <inheritdoc/>

        public int Count()
        {
            return surfaceObjects.Count();
        }    
/// <inheritdoc/>

        public IEnumerator<ISurfaceObject<T, U>> GetEnumerator()
        {
            foreach (var surfaceElement in surfaceObjects)
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
