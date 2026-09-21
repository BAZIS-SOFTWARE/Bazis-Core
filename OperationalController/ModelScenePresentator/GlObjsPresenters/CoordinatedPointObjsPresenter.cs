using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Собирает буферы точек, не принадлежащих модели.
    /// </summary>
    public class CoordinatedPointObjsPresenter<T> : IObjsPresenter
        where T : ICoordinated
    {
        readonly List<T> pointObjects;
        readonly Color color;

        /// <summary>
        /// Создаёт presenter независимых точек.
        /// </summary>
        public CoordinatedPointObjsPresenter(IEnumerable<T> pointObjects, Color color)
        {
            this.pointObjects = pointObjects?.ToList() ?? throw new ArgumentNullException(nameof(pointObjects));
            this.color = color;
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public ViewMode ViewMode { get; set; } = ViewMode.Point;

        /// <inheritdoc/>
        public PresenterType PresenterType => PresenterType.Point;

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
            foreach (var pointObject in pointObjects)
            {
                var point = pointObject.GetCoordinates().FirstOrDefault();
                if (vertexType == "цвет")
                    WriteColor(vertexes, ref stride);
                else
                    WritePoint(vertexes, ref stride, point);
            }

            return vertexes;
        }

        /// <inheritdoc/>
        public bool[] CreateEdgeFlags(int edgesLength) => new bool[0];

        /// <inheritdoc/>
        public int Count() => pointObjects.Count;

        /// <inheritdoc/>
        public IEnumerator<ICoordinated> GetEnumerator()
        {
            foreach (var pointObject in pointObjects)
                yield return pointObject;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Записывает координаты точки.
        /// </summary>
        private void WritePoint(float[] vertexes, ref int stride, Point3D point)
        {
            vertexes[stride++] = point._x;
            vertexes[stride++] = point._y;
            vertexes[stride++] = point._z;
        }

        /// <summary>
        /// Записывает цвет точки.
        /// </summary>
        private void WriteColor(float[] vertexes, ref int stride)
        {
            vertexes[stride++] = color.R / 255.0f;
            vertexes[stride++] = color.G / 255.0f;
            vertexes[stride++] = color.B / 255.0f;
            vertexes[stride++] = color.A / 255.0f;
        }
    }
}
