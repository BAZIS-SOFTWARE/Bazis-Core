using Geometry.Interfaces;
using Model.Interfaces;
using OperationalController.ModelScenePresentator.GlObjsVertexCreators;
using OperationalController.ModelScenePresentator.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Собирает буферы поверхностных объектов с готовыми цветами.
    /// </summary>
    public class SurfaceObjsPresenter<T, U> : ISurfaceObjsPresenter
        where T : ISurface<U>
        where U : IPoint
    {
        readonly IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects;
        readonly IReadOnlyList<Color> colors;
        readonly Color color;
        readonly bool hasSingleColor;

        /// <summary>
        /// Создаёт presenter поверхностных объектов с индивидуальными цветами.
        /// </summary>
        public SurfaceObjsPresenter(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, IReadOnlyList<Color> colors)
        {
            this.surfaceObjects = surfaceObjects ?? throw new ArgumentNullException(nameof(surfaceObjects));
            this.colors = colors ?? throw new ArgumentNullException(nameof(colors));
            if (surfaceObjects.Count != colors.Count)
                throw new ArgumentException("The number of colors must match the number of objects.", nameof(colors));
        }

        /// <summary>
        /// Создаёт presenter поверхностных объектов с одним цветом.
        /// </summary>
        public SurfaceObjsPresenter(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, Color color)
        {
            this.surfaceObjects = surfaceObjects ?? throw new ArgumentNullException(nameof(surfaceObjects));
            this.color = color;
            hasSingleColor = true;
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
            var colorsCount = 0;

            foreach (var surfaceObject in surfaceObjects)
            {
                foreach (var surface in surfaceObject.GetSurfaces())
                {
                    var triangles = surface.NumberOfPoints - 2;
                    vertexes += triangles * 9;
                    pointers += triangles * 3;
                    edges += triangles * 3;
                    colorsCount += triangles * 12;
                }
            }

            return new Tuple<int, int, int, int>(pointers, vertexes, colorsCount, edges);
        }

        /// <inheritdoc/>
        public int[] CreatePointers(int indexLength)
        {
            var pointers = new int[indexLength];
            for (var index = 0; index < indexLength; index++)
                pointers[index] = index;

            return pointers;
        }

        /// <inheritdoc/>
        public float[] CreateVertexes(int vertexLength, string vertexType)
        {
            var vertexes = new float[vertexLength];
            var stride = 0;
            ISurfaceObjsVertexCreator<T, U> vertexCreator;

            if (vertexType == "цвет")
                vertexCreator = new ColorSurfaceObjsCreator<T, U>();
            else if (vertexType == "координаты")
                vertexCreator = new CoordsSurfaceObjsCreator<T, U>();
            else
                vertexCreator = new NormalsSurfaceObjsCreator<T, U>();

            for (var index = 0; index < surfaceObjects.Count; index++)
                vertexCreator.Create(ref stride, vertexes, surfaceObjects[index], GetColor(index));

            return vertexes;
        }

        /// <inheritdoc/>
        public bool[] CreateEdgeFlags(int edgesLength)
        {
            var edges = new bool[edgesLength];
            var stride = 0;
            var creator = new EdgesSurfaceObjsCreator<T, U>();
            creator.Create(ref stride, edges, surfaceObjects);
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
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public int[] CreateSeparators()
        {
            var separators = new int[surfaceObjects.Count + 1];
            var stride = 0;

            for (var index = 0; index < surfaceObjects.Count; index++)
            {
                separators[index] = stride;
                foreach (var surface in surfaceObjects[index].GetSurfaces())
                    stride += surface.NumberOfPoints - 2;
            }

            separators[surfaceObjects.Count] = stride;
            return separators;
        }

        /// <summary>
        /// Возвращает цвет объекта по его позиции в списке.
        /// </summary>
        private Color GetColor(int index)
        {
            return hasSingleColor ? color : colors[index];
        }
    }
}
