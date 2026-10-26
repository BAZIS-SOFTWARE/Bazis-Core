using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Презентатор линейных объектов.
    /// </summary>
    public class LineObjsPresenter<T> : ObjsPresenter<T>
        where T : ICoordinated
    {
        /// <summary>
        /// Создаёт презентатор с индивидуальным цветом для каждого объекта.
        /// </summary>
        public LineObjsPresenter(IReadOnlyList<T> lineObjects, IReadOnlyList<Color> colors)
            : this(lineObjects, new ManyColorsObjsColors(colors))
        {
        }

        /// <summary>
        /// Создаёт презентатор с единым цветом для всех объектов.
        /// </summary>
        public LineObjsPresenter(IReadOnlyList<T> lineObjects, Color color)
            : this(lineObjects, new SingleColorObjsColors(color))
        {
        }

        /// <summary>
        /// Создаёт презентатор с заданным источником цветов.
        /// </summary>
        public LineObjsPresenter(IReadOnlyList<T> lineObjects, IObjsColors colors)
            : base(lineObjects, colors)
        {
        }

        /// <inheritdoc />
        public override PresenterType PresenterType { get; } = PresenterType.Line;

        /// <inheritdoc />
        public override ViewMode ViewMode { get; set; } = ViewMode.Line;

        /// <inheritdoc />
        public override Tuple<int, int, int, int> CreateIndexes()
        {
            var numberOfSegments = 0;

            foreach (var lineObject in Objects)
                numberOfSegments += lineObject.NumberOfPoints - 1;

            return new Tuple<int, int, int, int>(numberOfSegments * 2, numberOfSegments * 6, numberOfSegments * 8, 0);
        }

        /// <inheritdoc />
        public override float[] CreateVertexes(int vertexLength, string vertexType)
        {
            var vertexes = new float[vertexLength];
            var stride = 0;

            var objectIndex = 0;

            foreach (var color in CreateColors())
            {
                if (objectIndex == Objects.Count)
                    throw new InvalidOperationException("More colors were provided than objects.");

                var lineObject = Objects[objectIndex];
                var coordinates = lineObject.GetCoordinates();
                Point3D previous = null;

                foreach (var coordinate in coordinates)
                {
                    if (!ReferenceEquals(previous, null))
                    {
                        AddVertex(ref stride, vertexes, vertexType, previous, color);
                        AddVertex(ref stride, vertexes, vertexType, coordinate, color);
                    }

                    previous = coordinate;
                }

                objectIndex++;
            }

            if (objectIndex != Objects.Count)
                throw new InvalidOperationException("A color was not provided for every object.");

            return vertexes;
        }

        /// <inheritdoc />
        public override bool[] CreateEdgeFlags(int edgesLength)
        {
            return Array.Empty<bool>();
        }

        /// <summary>
        /// Записывает данные вершины в буфер.
        /// </summary>
        private void AddVertex(ref int stride, float[] vertexes, string vertexType, Point3D coordinate, Color color)
        {
            if (vertexType == "координаты")
            {
                vertexes[stride++] = (float)coordinate._x;
                vertexes[stride++] = (float)coordinate._y;
                vertexes[stride++] = (float)coordinate._z;
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
