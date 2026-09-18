using Model.Interfaces;
using OperationalController.ModelScenePresentator.GlObjsPresenters;
using Geometry.Interfaces;
using OperationalController;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController.ModelScenePresentator
{
    /// <summary>
    /// ModelScenePresentator
    /// </summary>
    public class PresentersCreator : IPresentersCreator
    {
        /// <summary>
        /// Создаёт presenter точечных объектов без состояния представления.
        /// </summary>
        public IObjsPresenter CreatePointObjectsPresenter(IEnumerable<IModelObject> modelObjects)
        {
            return CreatePointObjectsPresenter(modelObjects, new NullModelView(null));
        }

        /// <inheritdoc/>
        public IObjsPresenter CreatePointObjectsPresenter(IEnumerable<IModelObject> modelObjects, IModelView modelView)
        {
            var pointsPresentor = new PointObjsPresenter(modelView);
            pointsPresentor.Add(modelObjects);
            return pointsPresentor;
        }

        /// <summary>
        /// Создаёт presenter линейных объектов без состояния представления.
        /// </summary>
        public IObjsPresenter CreateLineObjectsPresenter<T>(IEnumerable<ILineObject<T>> lineObjs)
            where T : IPoint
        {
            return CreateLineObjectsPresenter(lineObjs, new NullModelView(null));
        }

/// <inheritdoc/>

        public IObjsPresenter CreateLineObjectsPresenter<T>(IEnumerable<ILineObject<T>> lineObjs, IModelView modelView)
            where T : IPoint
        {
            var linePres = new LineObjsPresenter<T>(modelView);
            linePres.Add(lineObjs);
            return linePres;
        }

        /// <summary>
        /// Создаёт presenter поверхностных объектов без состояния представления.
        /// </summary>
        public ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T, U>(IEnumerable<ISurfaceObject<T, U>> surfaceGeom)
            where T : ISurface<U> where U : IPoint
        {
            return CreateSurfaceObjectsPresenter(surfaceGeom, new NullModelView(null));
        }

/// <inheritdoc/>

        public ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T,U>(IEnumerable<ISurfaceObject<T, U>> surfaceGeom, IModelView modelView)
                    where T : ISurface<U> where U : IPoint
        {

            var presentor = new SurfaceObjsPresenter<T, U>(modelView);
            presentor.Add(surfaceGeom);
            return presentor;
        }

        /// <inheritdoc/>
        public ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T>(IEnumerable<T> surfaceObjects, Func<T, Color> getColor)
            where T : ICoordinated
        {
            return new CoordinatedSurfaceObjsPresenter<T>(surfaceObjects, getColor);
        }
    }
}
