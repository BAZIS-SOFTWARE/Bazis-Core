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
    /// Презентатор линейных объектов.
    /// </summary>
    public class LineObjsPresenter<T> : IObjsPresenter where T : ICoordinated
    {
        private readonly IReadOnlyList<T> lineObjects;
        private readonly IReadOnlyList<Color> colors;
        private readonly Color color;
        private readonly bool hasSingleColor;

        /// <summary>
        /// Создает презентатор с индивидуальным цветом для каждого объекта.
        /// </summary>
        public LineObjsPresenter(IReadOnlyList<T> lineObjects, IReadOnlyList<Color> colors)
        {
            this.lineObjects = lineObjects ?? throw new ArgumentNullException(nameof(lineObjects));
            this.colors = colors ?? throw new ArgumentNullException(nameof(colors));

            if (lineObjects.Count != colors.Count)
                throw new ArgumentException("The number of colors must match the number of objects.", nameof(colors));
        }

        /// <summary>
        /// Создает презентатор с единым цветом для всех объектов.
        /// </summary>
        public LineObjsPresenter(IReadOnlyList<T> lineObjects, Color color)
        {
            this.lineObjects = lineObjects ?? throw new ArgumentNullException(nameof(lineObjects));
            this.color = color;
            hasSingleColor = true;
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public PresenterType PresenterType { get; } = PresenterType.Line;

        /// <inheritdoc />
        public ViewMode ViewMode { get; set; } = ViewMode.Line;

        /// <inheritdoc />
        public Tuple<int, int, int, int> CreateIndexes()
        {
            var numberOfSegments = 0;

            foreach (var lineObject in lineObjects)
                numberOfSegments += lineObject.NumberOfPoints - 1;

            return new Tuple<int, int, int, int>(numberOfSegments * 2, numberOfSegments * 6, numberOfSegments * 8, 0);
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

            for (var objectIndex = 0; objectIndex < lineObjects.Count; objectIndex++)
            {
                var lineObject = lineObjects[objectIndex];
                var currentColor = hasSingleColor ? color : colors[objectIndex];
                var coordinates = lineObject.GetCoordinates();
                Point3D previous = null;

                foreach (var coordinate in coordinates)
                {
                    if (!ReferenceEquals(previous, null))
                    {
                        AddVertex(ref stride, vertexes, vertexType, previous, currentColor);
                        AddVertex(ref stride, vertexes, vertexType, coordinate, currentColor);
                    }

                    previous = coordinate;
                }
            }

            return vertexes;
        }

        /// <inheritdoc />
        public bool[] CreateEdgeFlags(int edgesLength) => Array.Empty<bool>();

        /// <inheritdoc />
        public int Count() => lineObjects.Count;

        /// <inheritdoc />
        public IEnumerator<ICoordinated> GetEnumerator()
        {
            foreach (var lineObject in lineObjects)
                yield return lineObject;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static void AddVertex(ref int stride, float[] vertexes, string vertexType, Point3D coordinate, Color color)
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
