using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Geometry.Interfaces;
using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;

namespace OperationalController
{
    /// <summary>
    /// Вспомогательные методы совместимости тестов буферов.
    /// </summary>
    public static class PresenterTestExtensions
    {
        /// <summary>
        /// Создает маркер устаревшего состояния для вызовов, не влияющий на буферы.
        /// </summary>
        public static PresenterBuffers GetDrawState(this IModelView modelView, ISetInfo setInfo) => new PresenterBuffers();

        /// <summary>
        /// Создает презентатор точек с тестовым единым цветом.
        /// </summary>
        public static IObjsPresenter CreatePointObjectsPresenter(this IPresentersCreator creator, IEnumerable<IModelObject> objects) => creator.CreatePointObjectsPresenter(objects.ToList(), Color.Blue);

        /// <summary>
        /// Создает презентатор линий с тестовым единым цветом.
        /// </summary>
        public static IObjsPresenter CreateLineObjectsPresenter<T>(this IPresentersCreator creator, IEnumerable<T> objects) where T : ICoordinated => creator.CreateLineObjectsPresenter(objects.ToList(), Color.Blue);

        /// <summary>
        /// Создает презентатор поверхностей с тестовым единым цветом.
        /// </summary>
        public static ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T, U>(this IPresentersCreator creator, IEnumerable<ISurfaceObject<T, U>> objects) where T : ISurface<U> where U : IPoint => creator.CreateSurfaceObjectsPresenter(objects.ToList(), Color.Blue);

        /// <summary>
        /// Создает презентатор поверхностей с координатами и тестовым единым цветом.
        /// </summary>
        public static ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T>(this IPresentersCreator creator, IEnumerable<T> objects) where T : ICoordinated => creator.CreateSurfaceObjectsPresenter(objects.ToList(), Color.Blue);

        /// <summary>
        /// Создает размеры буферов без состояния представления.
        /// </summary>
        public static Tuple<int, int, int, int> CreateIndexes(this IObjsPresenter presenter, PresenterBuffers buffers) => presenter.CreateIndexes();

        /// <summary>
        /// Создает указатели без состояния представления.
        /// </summary>
        public static int[] CreatePointers(this IObjsPresenter presenter, int length, PresenterBuffers buffers) => presenter.CreatePointers(length);

        /// <summary>
        /// Создает вершины без состояния представления.
        /// </summary>
        public static float[] CreateVertexes(this IObjsPresenter presenter, int length, string type, PresenterBuffers buffers) => presenter.CreateVertexes(length, type);

        /// <summary>
        /// Создает флаги ребер без состояния представления.
        /// </summary>
        public static bool[] CreateEdgeFlags(this IObjsPresenter presenter, int length, PresenterBuffers buffers) => presenter.CreateEdgeFlags(length);

        /// <summary>
        /// Создает разделители без состояния представления.
        /// </summary>
        public static int[] CreateSeparators(this ISurfaceObjsPresenter presenter, PresenterBuffers buffers) => presenter.CreateSeparators();
    }

    /// <summary>
    /// Маркер совместимости старых тестов буферов.
    /// </summary>
    public sealed class PresenterBuffers
    {
    }
}
