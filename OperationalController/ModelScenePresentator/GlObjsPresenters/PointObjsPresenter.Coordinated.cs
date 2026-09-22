using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Презентатор независимых точек.
    /// </summary>
    public class PointObjsPresenter<T> : ObjsPresenter<T>
        where T : ICoordinated
    {
        /// <summary>
        /// Создаёт презентатор с единым цветом для всех объектов.
        /// </summary>
        public PointObjsPresenter(IReadOnlyList<T> pointObjects, Color color)
            : this(pointObjects, new SingleColorObjsColors(color))
        {
        }

        /// <summary>
        /// Создаёт презентатор с индивидуальными цветами объектов.
        /// </summary>
        public PointObjsPresenter(IReadOnlyList<T> pointObjects, IReadOnlyList<Color> colors)
            : this(pointObjects, new ManyColorsObjsColors(colors))
        {
        }

        /// <summary>
        /// Создаёт презентатор с заданным источником цветов.
        /// </summary>
        public PointObjsPresenter(IReadOnlyList<T> pointObjects, IObjsColors colors)
            : base(pointObjects, colors)
        {
        }

        /// <inheritdoc />
        public override PresenterType PresenterType { get; } = PresenterType.Point;

        /// <inheritdoc />
        public override ViewMode ViewMode { get; set; } = ViewMode.Point;

        /// <inheritdoc />
        public override Tuple<int, int, int, int> CreateIndexes()
        {
            var count = 0;

            foreach (var pointObject in Objects)
                count += pointObject.NumberOfPoints;

            return new Tuple<int, int, int, int>(count, count * 3, count * 4, 0);
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

                var pointObject = Objects[objectIndex];

                foreach (var coordinate in pointObject.GetCoordinates())
                    AddVertex(ref stride, vertexes, vertexType, coordinate, color);

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
