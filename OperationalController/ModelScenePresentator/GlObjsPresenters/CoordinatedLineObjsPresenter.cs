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
    /// Собирает буферы линий, не принадлежащих модели.
    /// </summary>
    public class CoordinatedLineObjsPresenter<T> : IObjsPresenter
        where T : ICoordinated
    {
        readonly List<T> lineObjects;
        readonly Color color;

        /// <summary>
        /// Создаёт presenter независимых линий.
        /// </summary>
        public CoordinatedLineObjsPresenter(IEnumerable<T> lineObjects, Color color)
        {
            this.lineObjects = lineObjects?.ToList() ?? throw new ArgumentNullException(nameof(lineObjects));
            this.color = color;
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public ViewMode ViewMode { get; set; } = ViewMode.Line;

        /// <inheritdoc/>
        public PresenterType PresenterType => PresenterType.Line;

        /// <inheritdoc/>
        public Tuple<int, int, int, int> CreateIndexes()
        {
            var points = lineObjects.Sum(lineObject => lineObject.NumberOfPoints);
            return new Tuple<int, int, int, int>(points, points * 3, points * 4, lineObjects.Count);
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
            foreach (var lineObject in lineObjects)
            {
                foreach (var point in lineObject.GetCoordinates())
                {
                    if (vertexType == "цвет")
                        WriteColor(vertexes, ref stride);
                    else
                        WritePoint(vertexes, ref stride, point);
                }
            }

            return vertexes;
        }

        /// <inheritdoc/>
        public bool[] CreateEdgeFlags(int edgesLength) => new bool[edgesLength];

        /// <inheritdoc/>
        public int Count() => lineObjects.Count;

        /// <inheritdoc/>
        public IEnumerator<ICoordinated> GetEnumerator()
        {
            foreach (var lineObject in lineObjects)
                yield return lineObject;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Записывает координаты точки линии.
        /// </summary>
        private void WritePoint(float[] vertexes, ref int stride, Point3D point)
        {
            vertexes[stride++] = point._x;
            vertexes[stride++] = point._y;
            vertexes[stride++] = point._z;
        }

        /// <summary>
        /// Записывает цвет точки линии.
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
