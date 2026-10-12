using Model.Interfaces;
using OperationalController.ModelScenePresentator.GlObjsVertexCreators;
using OperationalController.ModelScenePresentator.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Собирает буферы точечных объектов с готовыми цветами.
    /// </summary>
    public class PointObjsPresenter : ObjsPresenter<IModelObject>
    {
        /// <summary>
        /// Создаёт presenter точечных объектов с индивидуальными цветами.
        /// </summary>
        public PointObjsPresenter(IReadOnlyList<IModelObject> pointObjects, IReadOnlyList<Color> colors)
            : this(pointObjects, new ManyColorsObjsColors(colors))
        {
        }

        /// <summary>
        /// Создаёт presenter точечных объектов с одним цветом.
        /// </summary>
        public PointObjsPresenter(IReadOnlyList<IModelObject> pointObjects, Color color)
            : this(pointObjects, new SingleColorObjsColors(color))
        {
        }

        /// <summary>
        /// Создаёт presenter точечных объектов с заданным источником цветов.
        /// </summary>
        public PointObjsPresenter(IReadOnlyList<IModelObject> pointObjects, IObjsColors colors)
            : base(pointObjects, colors)
        {
        }

        /// <inheritdoc/>
        public override PresenterType PresenterType { get; } = PresenterType.Point;

        /// <inheritdoc/>
        public override ViewMode ViewMode { get; set; } = ViewMode.Point;

        /// <inheritdoc/>
        public override Tuple<int, int, int, int> CreateIndexes()
        {
            var count = Objects.Count;
            return new Tuple<int, int, int, int>(count, count * 3, count * 4, 0);
        }

        /// <inheritdoc/>
        public override float[] CreateVertexes(int vertexLength, string vertexType)
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

            var objectIndex = 0;

            foreach (var color in CreateColors())
            {
                if (objectIndex == Objects.Count)
                    throw new InvalidOperationException("More colors were provided than objects.");

                var pointObject = Objects[objectIndex];
                vertexCreator.Create(ref stride, vertexes, pointObject, color);
                objectIndex++;
            }

            if (objectIndex != Objects.Count)
                throw new InvalidOperationException("A color was not provided for every object.");

            return vertexes;
        }

        /// <inheritdoc/>
        public override bool[] CreateEdgeFlags(int edgesLength)
        {
            return Array.Empty<bool>();
        }
    }
}
