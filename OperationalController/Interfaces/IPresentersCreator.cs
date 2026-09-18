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
        /// CreatePointObjectsPresenter
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <returns></returns>
        IObjsPresenter CreatePointObjectsPresenter(IEnumerable<IModelObject> modelObjects, IModelView modelView);
        /// <summary>
        /// CreateBeamPresenter
        /// </summary>
        /// <param name="lineObjs"></param>
        /// <returns></returns>
        IObjsPresenter CreateLineObjectsPresenter<T>(IEnumerable<ILineObject<T>> lineObjs, IModelView modelView)
            where T : IPoint;
        /// <summary>
        /// CreateOtherSurfacePresenter
        /// </summary>
        /// <param name="surfaceGeom"></param>
        /// <returns></returns>
        ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T, U>(IEnumerable<ISurfaceObject<T, U>> surfaceGeom, IModelView modelView)
                    where T : ISurface<U> where U : IPoint;
        /// <summary>
        /// Создаёт presenter поверхностей, не принадлежащих модели.
        /// </summary>
        /// <param name="surfaceObjects">Поверхности с координатами.</param>
        /// <param name="getColor">Функция получения цвета поверхности.</param>
        /// <returns>Presenter поверхностей.</returns>
        ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T>(IEnumerable<T> surfaceObjects, Func<T, Color> getColor)
            where T : ICoordinated;
    }
}
