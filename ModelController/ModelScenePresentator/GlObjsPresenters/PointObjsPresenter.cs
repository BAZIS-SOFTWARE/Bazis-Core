using ModelController.ModelScenePresentator.GlObjsVertexCreators;
using ModelController.ModelScenePresentator.Interfaces;
using ModelControllerInterfaces;
using Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ModelController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// PointObjsPresenter
    /// </summary>
    public class PointObjsPresenter : IObjsPresenter
    {

        List<IModelObject> pointObjects = new List<IModelObject>();
/// <inheritdoc/>
        public bool IsVolumeObjs { get; } = false;
        /// <inheritdoc/>

        public PresenterType PresenterType { get; } = PresenterType.Point;

        /// <inheritdoc/>


        public Tuple<int, int, int,int> CreateIndexes()
        {
            var indexes = 0;
            var coords = 0;
            var colors = 0;

            foreach (var pointObject in pointObjects)
            {
                //Закоментировано т.к вызывает exception
                if (pointObject.ViewState)
                {
                    coords += 3;
                    indexes++;
                    colors += 4;
                }
            }


            return new Tuple<int, int, int,int>(indexes, coords, colors,0);
        }
/// <inheritdoc/>


        public int[] CreatePointers(int indexLength)
        {
            var ptr = new int[indexLength];
            var stride = 0;

            foreach (var pointObject in pointObjects)
            {
                if (pointObject.ViewState)
                {
                    ptr[stride] = stride;
                    stride++;
                }
            }

            return ptr;
        }
        /// <inheritdoc/>

        public float[] CreateVertexes(int vertexLength, string vertexType)
        {
            var vertexArray = new float[vertexLength];
            var stride = 0;

            IPointObjsVertexCreator vertexCreator;

            if (vertexType == "цвет")
                vertexCreator = new ColorPointObjsCreator();
            else if (vertexType == "координаты")
                vertexCreator = new CoordsPointObjsCreator();
            else vertexCreator = new NormalsPointObjsCreator();


            foreach (var pointObject in pointObjects)
            {
                vertexCreator.Create(ref stride, vertexArray, pointObject);
            }

            return vertexArray;
        }
/// <inheritdoc/>

        public int Count()
        {
            return pointObjects.Count;
        }

/// <inheritdoc/>

        public bool[] CreateEdgeFlags(int edgesLength)
        {
            return new bool[0];
        }

/// <inheritdoc/>

        public IEnumerator<IModelObject> GetEnumerator()
        {
            foreach (var pointObject in pointObjects)
            {
                yield return pointObject;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public void Clear()
        {
            pointObjects.Clear();
        }

/// <inheritdoc/>

        public void ClearNotExisted()
        {
            var existObjs = pointObjects.Where(x => x.ExistState == true).ToList();
            pointObjects.Clear();
            pointObjects.AddRange(existObjs);
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <typeparam name="ISurfaceObject"></typeparam>
        /// <param name="objs"></param>
        public void Add(IEnumerable<IModelObject> objs)
        {
            pointObjects.AddRange(objs);
        }
    }
}
