using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Geometry.Interfaces;
using OperationalController.ModelScenePresentator.GlObjsVertexCreators;
using OperationalController.ModelScenePresentator.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Собирает буферы поверхностных объектов с учётом состояния представления.
    /// </summary>
    public class SurfaceObjsPresenter<T, U> : ISurfaceObjsPresenter
        where T : ISurface<U> where U : IPoint
    {
        readonly List<ISurfaceObject<T, U>> surfaceObjects = new List<ISurfaceObject<T, U>>();
        readonly IModelView modelView;

        /// <summary>
        /// Создаёт presenter без состояния представления.
        /// </summary>
        public SurfaceObjsPresenter() : this(new NullModelView(null)) { }

        /// <summary>
        /// Создаёт presenter с заданным состоянием представления.
        /// </summary>
        public SurfaceObjsPresenter(IModelView modelView)
        {
            this.modelView = modelView ?? throw new ArgumentNullException(nameof(modelView));
        }

        /// <inheritdoc/>
        public PresenterType PresenterType { get; } = PresenterType.Surface;

        /// <inheritdoc/>
        public ViewMode ViewMode { get; set; } = ViewMode.LineSurface;

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public Tuple<int, int, int, int> CreateIndexes()
        {
            var pointers = 0;
            var vertexes = 0;
            var edges = 0;
            var colors = 0;

            foreach (var surfaceObject in surfaceObjects)
            {
                if (!modelView.GetVisible(surfaceObject.ObjType, surfaceObject.Number))
                    continue;

                foreach (var surface in surfaceObject.GetSurfaces())
                {
                    vertexes += (surface.NumberOfPoints - 2) * 3 * 3;
                    pointers += (surface.NumberOfPoints - 2) * 3;
                    edges += (surface.NumberOfPoints - 2) * 3;
                    colors += (surface.NumberOfPoints - 2) * 3 * 4;
                }
            }

            return new Tuple<int, int, int, int>(pointers, vertexes, colors, edges);
        }

        /// <inheritdoc/>
        public int[] CreatePointers(int indexLength)
        {
            var pointers = new int[indexLength];
            var stride = 0;
            foreach (var surfaceObject in surfaceObjects)
            {
                if (!modelView.GetVisible(surfaceObject.ObjType, surfaceObject.Number))
                    continue;

                foreach (var surface in surfaceObject.GetSurfaces())
                {
                    var length = (surface.NumberOfPoints - 2) * 3;
                    for (var index = 0; index < length; index++)
                        pointers[stride + index] = stride + index;
                    stride += length;
                }
            }

            return pointers;
        }

        /// <inheritdoc/>
        public float[] CreateVertexes(int vertexLength, string vertexType)
        {
            var vertexArray = new float[vertexLength];
            var stride = 0;
            ISurfaceObjsVertexCreator<T, U> vertexCreator;
            if (vertexType == "цвет")
                vertexCreator = new ColorSurfaceObjsCreator<T, U>();
            else if (vertexType == "координаты")
                vertexCreator = new CoordsSurfaceObjsCreator<T, U>();
            else
                vertexCreator = new NormalsSurfaceObjsCreator<T, U>();

            foreach (var surfaceObject in surfaceObjects)
                if (modelView.GetVisible(surfaceObject.ObjType, surfaceObject.Number))
                    vertexCreator.Create(ref stride, vertexArray, surfaceObject, modelView);

            return vertexArray;
        }

        /// <inheritdoc/>
        public bool[] CreateEdgeFlags(int edgesLength)
        {
            var edges = new bool[edgesLength];
            var stride = 0;
            var vertexCreator = new EdgesSurfaceObjsCreator<T, U>();
            vertexCreator.Create(ref stride, edges, surfaceObjects, modelView);
            return edges;
        }

        /// <inheritdoc/>
        public int Count()
        {
            return surfaceObjects.Count;
        }

        /// <inheritdoc/>
        public IEnumerator<ICoordinated> GetEnumerator()
        {
            foreach (var surfaceObject in surfaceObjects)
                yield return surfaceObject;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Добавляет поверхностные объекты.
        /// </summary>
        public void Add(IEnumerable<ISurfaceObject<T, U>> objects)
        {
            surfaceObjects.AddRange(objects);
        }

        /// <inheritdoc/>
        public int[] CreateSeparators()
        {
            if (surfaceObjects.Count == 0)
                return Array.Empty<int>();

            var separators = new int[surfaceObjects.Count + 1];
            var stride = 0;
            var index = 0;
            foreach (var element in surfaceObjects)
            {
                separators[index] = stride;
                if (modelView.GetVisible(element.ObjType, element.Number))
                    foreach (var surface in element.GetSurfaces())
                        stride += surface.NumberOfPoints - 2;
                index++;
            }
            separators[index] = stride;
            return separators;
        }
    }
}
