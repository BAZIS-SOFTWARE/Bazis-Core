using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
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
    public class SurfaceObjsPresenter<T,U> : ISurfaceObjsPresenter
        where T : ISurface<U> where U : IPoint
    {
        List<ISurfaceObject<T,U>> surfaceObjects = new List<ISurfaceObject<T, U>>();
/// <inheritdoc/>

        public PresenterType PresenterType { get; } = PresenterType.Surface;
/// <inheritdoc/>

        public bool IsVolumeObjs { get; } = false;
        /// <inheritdoc/>
        public ViewMode ViewMode { get; set; } = ViewMode.LineSurface;

        /// <inheritdoc/>

        /// <summary>
        /// SurfaceObjsPresenter
        /// </summary>
        /// <param name="isVolumeObjs"></param>
        public SurfaceObjsPresenter(bool isVolumeObjs)
        {
            IsVolumeObjs = isVolumeObjs;
        }

        /// <inheritdoc/>

        public void HideInsideSurfaces()
        {
            var surfDict = new Dictionary<string, List<ISurface<U>>>();

            foreach (var element in surfaceObjects)
            {
                if (element.ViewState & element.NumberOfSides > 1)
                    foreach (var surface in element.GetSurfaces())
                    {
                        var numbers = surface.GetVertexes().Select(x => x.Number).ToArray();

                        Array.Sort(numbers);
                        var surfInd = string.Join("", numbers);   //numbers[0].ToString() + numbers[1].ToString() + numbers[2].ToString();

                        if (!surfDict.ContainsKey(surfInd))
                        {
                            surface.ViewState = true;
                            surfDict.Add(surfInd, new List<ISurface<U>> () { surface });
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

        /// <inheritdoc/>

        public void ShowInsideSurfaces()
        {
            foreach (var element in surfaceObjects)
            {
                if (element.ViewState)
                    foreach (var surface in element.GetSurfaces())
                        surface.ViewState = true;
            }
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
                vertexCreator = new ColorSurfaceObjsCreator<T, U>();
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
            return surfaceObjects.Count;
        }    
/// <inheritdoc/>

        public IEnumerator<IModelObject> GetEnumerator()
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


        /// <summary>
        /// Add
        /// </summary>
        /// <param name="objs"></param>
        public void Add(IEnumerable<ISurfaceObject<T, U>> objs)
        {
                surfaceObjects.AddRange(objs);
        }
/// <inheritdoc/>

        public int[] CreateSeparators()
        {
            var surfObjs = surfaceObjects.ToArray();
            var separators = new int[surfObjs.Length + 1];

            var stride = 0;
            var i = 0;

            for(; i < surfObjs.Length; ++i)
            {
                separators[i] = stride;
                foreach(var surface in surfObjs[i].GetSurfaces())
                    stride += surface.NumberOfPoints - 2;
            }
            separators[i] = stride;

            return separators;
        }
    }
}
