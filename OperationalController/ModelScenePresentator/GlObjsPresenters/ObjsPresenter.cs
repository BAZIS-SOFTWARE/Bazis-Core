using Geometry.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Содержит общую логику презентаторов объектов.
    /// </summary>
    public abstract class ObjsPresenter<T> : IObjsPresenter
        where T : ICoordinated
    {
        private readonly IObjsColors colors;

        /// <summary>
        /// Отображаемые объекты.
        /// </summary>
        protected IReadOnlyList<T> Objects { get; }

        /// <summary>
        /// Создаёт базовый презентатор объектов.
        /// </summary>
        protected ObjsPresenter(IReadOnlyList<T> objects, IObjsColors colors)
        {
            Objects = objects ?? throw new ArgumentNullException(nameof(objects));
            this.colors = colors ?? throw new ArgumentNullException(nameof(colors));
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public abstract PresenterType PresenterType { get; }

        /// <inheritdoc/>
        public abstract ViewMode ViewMode { get; set; }

        /// <summary>
        /// Создаёт последовательность цветов отображаемых объектов.
        /// </summary>
        protected IEnumerable<Color> CreateColors()
        {
            return colors.CreateColors(Objects.Count);
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
        public int Count()
        {
            return Objects.Count;
        }

        /// <inheritdoc/>
        public IEnumerator<ICoordinated> GetEnumerator()
        {
            foreach (var modelObject in Objects)
                yield return modelObject;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public abstract Tuple<int, int, int, int> CreateIndexes();

        /// <inheritdoc/>
        public abstract float[] CreateVertexes(int vertexLength, string vertexType);

        /// <inheritdoc/>
        public abstract bool[] CreateEdgeFlags(int edgesLength);
    }
}
