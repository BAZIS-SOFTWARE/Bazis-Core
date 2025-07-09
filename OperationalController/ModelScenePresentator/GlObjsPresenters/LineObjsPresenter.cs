using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using OperationalController.ModelScenePresentator.GlObjsVertexCreators;
using OperationalController.ModelScenePresentator.Interfaces;
using OperationalController;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// LineObjsPresenter
    /// </summary>
    public class LineObjsPresenter<T> : IObjsPresenter
        where T : IPoint
    {

        /// <inheritdoc/>
        public string Name { get; set; }

        List<ILineObject<T>> lineObjects = new List<ILineObject<T>>();

        /// <inheritdoc/>

        public PresenterType PresenterType { get; } = PresenterType.Line;
/// <inheritdoc/>

        public bool IsVolumeObjs { get; } = false;

        /// <inheritdoc/>
        public ViewMode ViewMode { get; set; } = ViewMode.Line;
        /// <inheritdoc/>

        public Tuple<int, int, int,int> CreateIndexes()
        {
            var indexes = 0;
            var coords = 0;
            var colors = 0;
            var edges = 0;
            foreach (var lineObj in lineObjects)
            {
                if (lineObj.ViewState)
                {
                    coords = coords + lineObj.NumberOfPoints * 3;
                    indexes = indexes + lineObj.NumberOfPoints;
                    colors = colors + lineObj.NumberOfPoints * 4;
                    edges++;
                }
            }

            return new Tuple<int, int, int,int>(indexes, coords, colors, edges);
        }
/// <inheritdoc/>



        public int[] CreatePointers(int indexLength)
        {
            var stride = 0;
            var ptr = new int[indexLength];

            foreach (var lineObject in lineObjects)
            {
                if (lineObject.ViewState)
                {
                    for (int m = 0; m < lineObject.NumberOfPoints; m++)
                    {
                        ptr[stride + m] = stride + m;
                    }
                    stride = stride + lineObject.NumberOfPoints;
                }
            }
            return ptr;
        }
/// <inheritdoc/>

        public float[] CreateVertexes(int vertexLength, string vertexType)
        {
            var vertexArray = new float[vertexLength];
            var stride = 0;

            ILineObjsVertexCreator<T> vertexCreator;

            if (vertexType == "цвет")
                vertexCreator = new ColorLineObjsCreator<T>();
            else if (vertexType == "координаты")
                vertexCreator = new CoordsLineObjsCreator<T>();
            else vertexCreator = new NormalsLineObjsCreator<T>();

            foreach (var lineObject in lineObjects)
            {
                if(lineObject.ViewState)
                    vertexCreator.Create(ref stride, vertexArray, lineObject);
            }

            return vertexArray;
        }
/// <inheritdoc/>

        public int Count()
        {
            return lineObjects.Count;
        } 

/// <inheritdoc/>

        public bool[] CreateEdgeFlags(int edgesLength)
        {
            var edgesArray = new bool[edgesLength];

            edgesArray.Where(x => x = true);

            return edgesArray;
        }

        public IEnumerator<IModelObject> GetEnumerator()
        {
            foreach (var lineObject in lineObjects)
            {
                yield return lineObject;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public void ClearNotExisted()
        {
            var existObjs = lineObjects.Where(x => x.ExistState == true).ToList();
            lineObjects.Clear();
            lineObjects.AddRange(existObjs);
        }
/// <inheritdoc/>

        public void Clear()
        {
            lineObjects.Clear();
        }
        /// <summary>
        /// Add
        /// </summary>
        /// <typeparam name="ILineObject"></typeparam>
        /// <param name="objs"></param>
        public void Add(IEnumerable<ILineObject<T>> objs)
        {
            lineObjects.AddRange(objs);
        }
    }
}
