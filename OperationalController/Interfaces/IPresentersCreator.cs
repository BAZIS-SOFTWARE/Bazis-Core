using System.Collections.Generic;
using System.Drawing;
using Geometry.Interfaces;
using Model.Interfaces;

namespace OperationalController
{
    /// <summary>
    /// Создает презентаторы объектов.
    /// </summary>
    public interface IPresentersCreator
    {
        /// <summary>
        /// Создает презентатор точечных объектов с индивидуальными цветами.
        /// </summary>
        IObjsPresenter CreatePointObjectsPresenter(IReadOnlyList<IModelObject> pointObjects, IReadOnlyList<Color> colors);

        /// <summary>
        /// Создает презентатор точечных объектов с единым цветом.
        /// </summary>
        IObjsPresenter CreatePointObjectsPresenter(IReadOnlyList<IModelObject> pointObjects, Color color);

        /// <summary>
        /// Создает презентатор линейных объектов с индивидуальными цветами.
        /// </summary>
        IObjsPresenter CreateLineObjectsPresenter<T>(IReadOnlyList<T> lineObjects, IReadOnlyList<Color> colors) where T : ICoordinated;

        /// <summary>
        /// Создает презентатор линейных объектов с единым цветом.
        /// </summary>
        IObjsPresenter CreateLineObjectsPresenter<T>(IReadOnlyList<T> lineObjects, Color color) where T : ICoordinated;

        /// <summary>
        /// Создает презентатор поверхностных объектов с индивидуальными цветами.
        /// </summary>
        ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T, U>(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, IReadOnlyList<Color> colors, bool drawInnerSurfaces = true) where T : ISurface<U> where U : IPoint;

        /// <summary>
        /// Создает презентатор поверхностных объектов с единым цветом.
        /// </summary>
        ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T, U>(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, Color color, bool drawInnerSurfaces = true) where T : ISurface<U> where U : IPoint;

        /// <summary>
        /// Создает презентатор поверхностей с координатами и индивидуальными цветами.
        /// </summary>
        ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T>(IReadOnlyList<T> surfaceObjects, IReadOnlyList<Color> colors) where T : ICoordinated;

        /// <summary>
        /// Создает презентатор поверхностей с координатами и единым цветом.
        /// </summary>
        ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T>(IReadOnlyList<T> surfaceObjects, Color color) where T : ICoordinated;

        /// <summary>
        /// Создает презентатор независимых точек с единым цветом.
        /// </summary>
        IObjsPresenter CreatePointObjectsPresenter<T>(IReadOnlyList<T> pointObjects, Color color) where T : ICoordinated;
    }
}
