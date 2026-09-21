using Model.Interfaces;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController
{
    /// <summary>
    /// IModelPresenter
    /// </summary>
    public interface IPresentersCreator
    {
        ///// <summary>
        ///// SetColor
        ///// </summary>
        ///// <param name="objsType"></param>
        ///// <param name="objsColor"></param>
        //void SetColor(ObjType objsType, Color objsColor);
        ///// <summary>
        ///// GetColor
        ///// </summary>
        ///// <param name="objType"></param>
        ///// <returns></returns>
        //Color GetColor(ObjType objType);
        /// <summary>
        /// Создаёт presenter точечных объектов набора модели.
        /// </summary>
        IObjsPresenter CreatePointObjectsPresenter(IEnumerable<IModelObject> modelObjects, IModelView modelView, Model.Interfaces.ObjectsCollections.ISetInfo setInfo);

        /// <summary>
        /// Создаёт presenter линейных объектов набора модели.
        /// </summary>
        IObjsPresenter CreateLineObjectsPresenter<T>(IEnumerable<ILineObject<T>> lineObjs, IModelView modelView, Model.Interfaces.ObjectsCollections.ISetInfo setInfo)
            where T : IPoint;

        /// <summary>
        /// Создаёт presenter поверхностных объектов набора модели.
        /// </summary>
        ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T, U>(IEnumerable<ISurfaceObject<T, U>> surfaceGeom, IModelView modelView, Model.Interfaces.ObjectsCollections.ISetInfo setInfo)
                    where T : ISurface<U> where U : IPoint;
        /// <summary>
        /// Создаёт presenter поверхностей, не принадлежащих модели.
        /// </summary>
        /// <param name="surfaceObjects">Поверхности с координатами.</param>
        /// <param name="getColor">Функция получения цвета поверхности.</param>
        /// <returns>Presenter поверхностей.</returns>
        ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T>(IEnumerable<T> surfaceObjects, Func<T, Color> getColor)
            where T : ICoordinated;

        /// <summary>
        /// Создаёт presenter независимых точек.
        /// </summary>
        IObjsPresenter CreateIndependentPointObjectsPresenter<T>(IEnumerable<T> pointObjects, Color color)
            where T : ICoordinated;

        /// <summary>
        /// Создаёт presenter независимых линий.
        /// </summary>
        IObjsPresenter CreateIndependentLineObjectsPresenter<T>(IEnumerable<T> lineObjects, Color color)
            where T : ICoordinated;
    }
}
