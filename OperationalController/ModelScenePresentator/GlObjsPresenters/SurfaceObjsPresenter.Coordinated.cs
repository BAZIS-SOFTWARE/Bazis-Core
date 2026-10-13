using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Собирает буферы координатных поверхностей с готовыми цветами.
    /// </summary>
    public class SurfaceObjsPresenter<T> : ObjsPresenter<T>, ISurfaceObjsPresenter
        where T : ICoordinated
    {
        /// <summary>
        /// Создаёт presenter координатных поверхностей с индивидуальными цветами.
        /// </summary>
        public SurfaceObjsPresenter(IReadOnlyList<T> surfaceObjects, IReadOnlyList<Color> colors)
            : this(surfaceObjects, new ManyColorsObjsColors(colors))
        {
        }

        /// <summary>
        /// Создаёт presenter координатных поверхностей с одним цветом.
        /// </summary>
        public SurfaceObjsPresenter(IReadOnlyList<T> surfaceObjects, Color color)
            : this(surfaceObjects, new SingleColorObjsColors(color))
        {
        }

        /// <summary>
        /// Создаёт presenter координатных поверхностей с заданным источником цветов.
        /// </summary>
        public SurfaceObjsPresenter(IReadOnlyList<T> surfaceObjects, IObjsColors colors)
            : base(surfaceObjects, colors)
        {
        }

        /// <inheritdoc/>
        public override ViewMode ViewMode { get; set; } = ViewMode.LineSurface;

        /// <inheritdoc/>
        public override PresenterType PresenterType => PresenterType.Surface;

        /// <inheritdoc/>
        public override Tuple<int, int, int, int> CreateIndexes()
        {
            var pointers = 0;
            var vertexes = 0;
            var colorsCount = 0;
            var edges = 0;

            foreach (var surfaceObject in Objects)
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
        public override float[] CreateVertexes(int vertexLength, string vertexType)
        {
            var vertexes = new float[vertexLength];
            var stride = 0;

            var objectIndex = 0;

            foreach (var color in CreateColors())
            {
                if (objectIndex == Objects.Count)
                    throw new InvalidOperationException("More colors were provided than objects.");

                var surfaceObject = Objects[objectIndex];
                var coordinates = surfaceObject.GetCoordinates().ToArray();

                if (vertexType == "цвет")
                    CreateColors(ref stride, vertexes, coordinates, color);
                else if (vertexType == "координаты")
                    CreateCoordinates(ref stride, vertexes, coordinates);
                else
                    CreateNormals(ref stride, vertexes, coordinates);

                objectIndex++;
            }

            if (objectIndex != Objects.Count)
                throw new InvalidOperationException("A color was not provided for every object.");

            return vertexes;
        }

        /// <inheritdoc/>
        public override bool[] CreateEdgeFlags(int edgesLength)
        {
            var edges = new bool[edgesLength];
            var stride = 0;

            foreach (var surfaceObject in Objects)
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
        public int[] CreateSeparators()
        {
            var separators = new int[Objects.Count + 1];
            var stride = 0;

            for (var index = 0; index < Objects.Count; index++)
            {
                separators[index] = stride;
                stride += Objects[index].NumberOfPoints - 2;
            }

            separators[Objects.Count] = stride;
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
    }
}
