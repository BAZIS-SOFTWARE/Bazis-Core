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
        /// Создаёт presenter точечных объектов заданного набора.
        /// </summary>
        public IObjsPresenter CreatePointObjectsPresenter(IEnumerable<IModelObject> modelObjects, IModelView modelView, Model.Interfaces.ObjectsCollections.ISetInfo setInfo)
        {
            var pointsPresentor = new PointObjsPresenter(modelView, setInfo);
            pointsPresentor.Add(modelObjects);
            return pointsPresentor;
        }

        /// <summary>
        /// Создаёт presenter линейных объектов заданного набора.
        /// </summary>
        public IObjsPresenter CreateLineObjectsPresenter<T>(IEnumerable<ILineObject<T>> lineObjs, IModelView modelView, Model.Interfaces.ObjectsCollections.ISetInfo setInfo)
            where T : IPoint
        {
            var linePres = new LineObjsPresenter<T>(modelView, setInfo);
            linePres.Add(lineObjs);
            return linePres;
        }

        /// <summary>
        /// Создаёт presenter поверхностных объектов заданного набора.
        /// </summary>
        public ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T,U>(IEnumerable<ISurfaceObject<T, U>> surfaceGeom, IModelView modelView, Model.Interfaces.ObjectsCollections.ISetInfo setInfo)
                    where T : ISurface<U> where U : IPoint
        {
            var presentor = new SurfaceObjsPresenter<T, U>(modelView, setInfo);
            presentor.Add(surfaceGeom);
            return presentor;
        }

        /// <inheritdoc/>
        public ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T>(IEnumerable<T> surfaceObjects, Func<T, Color> getColor)
            where T : ICoordinated
        {
            return new CoordinatedSurfaceObjsPresenter<T>(surfaceObjects, getColor);
        }

        /// <inheritdoc/>
        public IObjsPresenter CreateIndependentPointObjectsPresenter<T>(IEnumerable<T> pointObjects, Color color)
            where T : ICoordinated
        {
            return new CoordinatedPointObjsPresenter<T>(pointObjects, color);
        }

        /// <inheritdoc/>
        public IObjsPresenter CreateIndependentLineObjectsPresenter<T>(IEnumerable<T> lineObjects, Color color)
            where T : ICoordinated
        {
            return new CoordinatedLineObjsPresenter<T>(lineObjects, color);
        }
    }
}
