using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Geometry;
using Geometry.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Презентатор независимых точек.
    /// </summary>
    public class PointObjsPresenter<T> : IObjsPresenter where T : ICoordinated
    {
        private readonly IReadOnlyList<T> pointObjects;
        private readonly Color color;

        /// <summary>
        /// Создает презентатор с единым цветом для всех объектов.
        /// </summary>
        public PointObjsPresenter(IReadOnlyList<T> pointObjects, Color color)
        {
            this.pointObjects = pointObjects ?? throw new ArgumentNullException(nameof(pointObjects));
            this.color = color;
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public PresenterType PresenterType { get; } = PresenterType.Point;

        /// <inheritdoc />
        public ViewMode ViewMode { get; set; } = ViewMode.Point;

        /// <inheritdoc />
        public Tuple<int, int, int, int> CreateIndexes()
        {
            var count = 0;

            foreach (var pointObject in pointObjects)
                count += pointObject.NumberOfPoints;

            return new Tuple<int, int, int, int>(count, count * 3, count * 4, 0);
        }

        /// <inheritdoc />
        public int[] CreatePointers(int indexLength)
        {
            var pointers = new int[indexLength];
            for (var index = 0; index < indexLength; index++)
                pointers[index] = index;

            return pointers;
        }

        /// <inheritdoc />
        public float[] CreateVertexes(int vertexLength, string vertexType)
        {
            var vertexes = new float[vertexLength];
            var stride = 0;

            foreach (var pointObject in pointObjects)
            {
                var coordinates = pointObject.GetCoordinates();

                foreach (var coordinate in coordinates)
                    AddVertex(ref stride, vertexes, vertexType, coordinate);
            }

            return vertexes;
        }

        /// <inheritdoc />
        public bool[] CreateEdgeFlags(int edgesLength) => Array.Empty<bool>();

        /// <inheritdoc />
        public int Count() => pointObjects.Count;

        /// <inheritdoc />
        public IEnumerator<ICoordinated> GetEnumerator()
        {
            foreach (var pointObject in pointObjects)
                yield return pointObject;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void AddVertex(ref int stride, float[] vertexes, string vertexType, Point3D coordinate)
        {
            if (vertexType == "координаты")
            {
                vertexes[stride++] = coordinate._x;
                vertexes[stride++] = coordinate._y;
                vertexes[stride++] = coordinate._z;
                return;
            }

            if (vertexType == "нормаль")
            {
                vertexes[stride++] = 0;
                vertexes[stride++] = 0;
                vertexes[stride++] = 0;
                return;
            }

            vertexes[stride++] = color.R / 255f;
            vertexes[stride++] = color.G / 255f;
            vertexes[stride++] = color.B / 255f;
            vertexes[stride++] = color.A / 255f;
        }
    }
}
