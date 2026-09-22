using System.Collections.Generic;
using System.Drawing;
using Geometry.Interfaces;
using Model.Interfaces;
using OperationalController.ModelScenePresentator.GlObjsPresenters;

namespace OperationalController.ModelScenePresentator
{
    /// <summary>
    /// Создает презентаторы объектов.
    /// </summary>
    public class PresentersCreator : IPresentersCreator
    {
        /// <inheritdoc />
        public IObjsPresenter CreatePointObjectsPresenter(IReadOnlyList<IModelObject> pointObjects, IReadOnlyList<Color> colors) => new PointObjsPresenter(pointObjects, colors);

        /// <inheritdoc />
        public IObjsPresenter CreatePointObjectsPresenter(IReadOnlyList<IModelObject> pointObjects, Color color) => new PointObjsPresenter(pointObjects, color);

        /// <inheritdoc />
        public IObjsPresenter CreateLineObjectsPresenter<T>(IReadOnlyList<T> lineObjects, IReadOnlyList<Color> colors) where T : ICoordinated => new LineObjsPresenter<T>(lineObjects, colors);

        /// <inheritdoc />
        public IObjsPresenter CreateLineObjectsPresenter<T>(IReadOnlyList<T> lineObjects, Color color) where T : ICoordinated => new LineObjsPresenter<T>(lineObjects, color);

        /// <inheritdoc />
        public ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T, U>(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, IReadOnlyList<Color> colors) where T : ISurface<U> where U : IPoint => new SurfaceObjsPresenter<T, U>(surfaceObjects, colors);

        /// <inheritdoc />
        public ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T, U>(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, Color color) where T : ISurface<U> where U : IPoint => new SurfaceObjsPresenter<T, U>(surfaceObjects, color);

        /// <inheritdoc />
        public ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T>(IReadOnlyList<T> surfaceObjects, IReadOnlyList<Color> colors) where T : ICoordinated => new SurfaceObjsPresenter<T>(surfaceObjects, colors);

        /// <inheritdoc />
        public ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T>(IReadOnlyList<T> surfaceObjects, Color color) where T : ICoordinated => new SurfaceObjsPresenter<T>(surfaceObjects, color);

        /// <inheritdoc />
        public IObjsPresenter CreatePointObjectsPresenter<T>(IReadOnlyList<T> pointObjects, Color color) where T : ICoordinated => new PointObjsPresenter<T>(pointObjects, color);
    }
}
