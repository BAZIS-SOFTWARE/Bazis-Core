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
    /// Собирает буферы точечных объектов с готовыми цветами.
    /// </summary>
    public class PointObjsPresenter : IObjsPresenter
    {
        readonly IReadOnlyList<IModelObject> pointObjects;
        readonly IReadOnlyList<Color> colors;
        readonly Color color;
        readonly bool hasSingleColor;

        /// <summary>
        /// Создаёт presenter точечных объектов с индивидуальными цветами.
        /// </summary>
        public PointObjsPresenter(IReadOnlyList<IModelObject> pointObjects, IReadOnlyList<Color> colors)
        {
            this.pointObjects = pointObjects ?? throw new ArgumentNullException(nameof(pointObjects));
            this.colors = colors ?? throw new ArgumentNullException(nameof(colors));
            if (pointObjects.Count != colors.Count)
                throw new ArgumentException("The number of colors must match the number of objects.", nameof(colors));
        }

        /// <summary>
        /// Создаёт presenter точечных объектов с одним цветом.
        /// </summary>
        public PointObjsPresenter(IReadOnlyList<IModelObject> pointObjects, Color color)
        {
            this.pointObjects = pointObjects ?? throw new ArgumentNullException(nameof(pointObjects));
            this.color = color;
            hasSingleColor = true;
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public PresenterType PresenterType { get; } = PresenterType.Point;

        /// <inheritdoc/>
        public ViewMode ViewMode { get; set; } = ViewMode.Point;

        /// <inheritdoc/>
        public Tuple<int, int, int, int> CreateIndexes()
        {
            var count = pointObjects.Count;
            return new Tuple<int, int, int, int>(count, count * 3, count * 4, 0);
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
            IPointObjsVertexCreator vertexCreator;

            if (vertexType == "цвет")
                vertexCreator = new ColorPointObjsCreator();
            else if (vertexType == "координаты")
                vertexCreator = new CoordsPointObjsCreator();
            else
                vertexCreator = new NormalsPointObjsCreator();

            for (var index = 0; index < pointObjects.Count; index++)
                vertexCreator.Create(ref stride, vertexes, pointObjects[index], GetColor(index));

            return vertexes;
        }

        /// <inheritdoc/>
        public bool[] CreateEdgeFlags(int edgesLength)
        {
            return Array.Empty<bool>();
        }

        /// <inheritdoc/>
        public int Count()
        {
            return pointObjects.Count;
        }

        /// <inheritdoc/>
        public IEnumerator<ICoordinated> GetEnumerator()
        {
            foreach (var pointObject in pointObjects)
                yield return pointObject;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
