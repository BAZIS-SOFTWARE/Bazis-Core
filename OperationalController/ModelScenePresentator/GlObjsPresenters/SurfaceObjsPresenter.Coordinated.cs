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
    /// Собирает буферы координатных поверхностей с готовыми цветами.
    /// </summary>
    public class SurfaceObjsPresenter<T> : ISurfaceObjsPresenter
        where T : ICoordinated
    {
        readonly IReadOnlyList<T> surfaceObjects;
        readonly IReadOnlyList<Color> colors;
        readonly Color color;
        readonly bool hasSingleColor;

        /// <summary>
        /// Создаёт presenter координатных поверхностей с индивидуальными цветами.
        /// </summary>
        public SurfaceObjsPresenter(IReadOnlyList<T> surfaceObjects, IReadOnlyList<Color> colors)
        {
            this.surfaceObjects = surfaceObjects ?? throw new ArgumentNullException(nameof(surfaceObjects));
            this.colors = colors ?? throw new ArgumentNullException(nameof(colors));
            if (surfaceObjects.Count != colors.Count)
                throw new ArgumentException("The number of colors must match the number of objects.", nameof(colors));
        }

        /// <summary>
        /// Создаёт presenter координатных поверхностей с одним цветом.
        /// </summary>
        public SurfaceObjsPresenter(IReadOnlyList<T> surfaceObjects, Color color)
        {
            this.surfaceObjects = surfaceObjects ?? throw new ArgumentNullException(nameof(surfaceObjects));
            this.color = color;
            hasSingleColor = true;
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public ViewMode ViewMode { get; set; } = ViewMode.LineSurface;

        /// <inheritdoc/>
        public PresenterType PresenterType => PresenterType.Surface;

        /// <inheritdoc/>
        public Tuple<int, int, int, int> CreateIndexes()
        {
            var pointers = 0;
            var vertexes = 0;
            var colorsCount = 0;
            var edges = 0;

            foreach (var surfaceObject in surfaceObjects)
            {
                var triangles = surfaceObject.NumberOfPoints - 2;
                pointers += triangles * 3;
                vertexes += triangles * 9;
                colorsCount += triangles * 12;
                edges += triangles * 3;
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

            for (var index = 0; index < surfaceObjects.Count; index++)
            {
                var coordinates = surfaceObjects[index].GetCoordinates().ToArray();
                if (vertexType == "цвет")
                    CreateColors(ref stride, vertexes, coordinates, GetColor(index));
                else if (vertexType == "координаты")
                    CreateCoordinates(ref stride, vertexes, coordinates);
                else
                    CreateNormals(ref stride, vertexes, coordinates);
            }

            return vertexes;
        }

        /// <inheritdoc/>
        public bool[] CreateEdgeFlags(int edgesLength)
        {
            var edges = new bool[edgesLength];
            var stride = 0;

            foreach (var surfaceObject in surfaceObjects)
            {
                var triangles = surfaceObject.NumberOfPoints - 2;
                for (var index = 0; index < triangles; index++)
                {
                    if (index == 0)
                    {
                        edges[stride] = true;
                        edges[stride + 1] = true;
                        edges[stride + 2] = true;
                    }
                    else
                    {
                        edges[stride - 1] = false;
                        edges[stride] = false;
                        edges[stride + 1] = true;
                        edges[stride + 2] = true;
                    }

                    stride += 3;
                }
            }

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
                stride += surfaceObjects[index].NumberOfPoints - 2;
            }

            separators[surfaceObjects.Count] = stride;
            return separators;
        }

        /// <summary>
        /// Записывает координаты триангулированной поверхности.
        /// </summary>
        private void CreateCoordinates(ref int stride, float[] vertexes, Point3D[] coordinates)
        {
            for (var index = 0; index < coordinates.Length - 2; index++)
            {
                WritePoint(vertexes, stride, coordinates[0]);
                WritePoint(vertexes, stride + 3, coordinates[index + 1]);
                WritePoint(vertexes, stride + 6, coordinates[index + 2]);
                stride += 9;
            }
        }

        /// <summary>
        /// Записывает цвета триангулированной поверхности.
        /// </summary>
        private void CreateColors(ref int stride, float[] vertexes, Point3D[] coordinates, Color color)
        {
            for (var index = 0; index < coordinates.Length - 2; index++)
            {
                WriteColor(vertexes, stride, color);
                WriteColor(vertexes, stride + 4, color);
                WriteColor(vertexes, stride + 8, color);
                stride += 12;
            }
        }

        /// <summary>
        /// Записывает нормали триангулированной поверхности.
        /// </summary>
        private void CreateNormals(ref int stride, float[] vertexes, Point3D[] coordinates)
        {
            var firstVector = coordinates[1].Sub(coordinates[0]);
            var secondVector = coordinates[2].Sub(coordinates[0]);
            var normal = Vector.CrossProd(firstVector, secondVector);

            for (var index = 0; index < coordinates.Length - 2; index++)
            {
                WritePoint(vertexes, stride, normal);
                WritePoint(vertexes, stride + 3, normal);
                WritePoint(vertexes, stride + 6, normal);
                stride += 9;
            }
        }

        /// <summary>
        /// Записывает пространственную точку в буфер.
        /// </summary>
        private void WritePoint(float[] vertexes, int index, Point3D point)
        {
            vertexes[index] = point._x;
            vertexes[index + 1] = point._y;
            vertexes[index + 2] = point._z;
        }

        /// <summary>
        /// Записывает цвет в буфер.
        /// </summary>
        private void WriteColor(float[] vertexes, int index, Color color)
        {
            vertexes[index] = color.R / 255.0f;
            vertexes[index + 1] = color.G / 255.0f;
            vertexes[index + 2] = color.B / 255.0f;
            vertexes[index + 3] = color.A / 255.0f;
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
