using OperationalController.ModelScenePresentator.GlObjsVertexCreators;
using OperationalController.ModelScenePresentator.Interfaces;
using OperationalController;
using Geometry.Interfaces;
using Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Model.Interfaces.ObjectsCollections;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// PointObjsPresenter
    /// </summary>
    public class PointObjsPresenter : IObjsPresenter
    {
        List<IModelObject> pointObjects = new List<IModelObject>();
        readonly IModelView modelView;
        readonly ISetInfo setInfo;

        /// <summary>
        /// Создаёт presenter объектов заданного набора.
        /// </summary>
        public PointObjsPresenter(IModelView modelView, ISetInfo setInfo)
        {
            this.modelView = modelView ?? throw new ArgumentNullException(nameof(modelView));
            this.setInfo = setInfo ?? throw new ArgumentNullException(nameof(setInfo));
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public PresenterType PresenterType { get; } = PresenterType.Point;

        /// <inheritdoc/>
        public ViewMode ViewMode { get; set; } = ViewMode.Point;

        /// <inheritdoc/>
        public Tuple<int, int, int,int> CreateIndexes()
        {
            var indexes = 0;
            var coords = 0;
            var colors = 0;

            foreach (var pointObject in pointObjects)
            {
                //Закоментировано т.к вызывает exception
                if (modelView.GetVisible(pointObject.ObjType, pointObject.Number))
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
                if (modelView.GetVisible(pointObject.ObjType, pointObject.Number))
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


            var setColor = GetSetColor(vertexType);
            foreach (var pointObject in pointObjects)
            {
                if (modelView.GetVisible(pointObject.ObjType, pointObject.Number))
                    vertexCreator.Create(ref stride, vertexArray, pointObject, modelView, setColor);
            }

            return vertexArray;
        }

        /// <summary>
        /// Возвращает цвет стиля набора для текущего построения буфера.
        /// </summary>
        private Color? GetSetColor(string vertexType)
        {
            if (vertexType != "цвет")
                return null;

            return modelView.GetSetColor(setInfo);
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

        public IEnumerator<ICoordinated> GetEnumerator()
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
