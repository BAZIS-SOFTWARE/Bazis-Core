using Model.Interfaces;
using System.Collections.Generic;

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
        IObjsPresenter CreatePointObjectsPresenter(IEnumerable<IModelObject> modelObjects);
        /// <summary>
        /// CreateBeamPresenter
        /// </summary>
        /// <param name="lineObjs"></param>
        /// <returns></returns>
        IObjsPresenter CreateLineObjectsPresenter<T>(IEnumerable<ILineObject<T>> lineObjs)
            where T : IPoint;
        /// <summary>
        /// CreateOtherSurfacePresenter
        /// </summary>
        /// <param name="surfaceGeom"></param>
        /// <returns></returns>
        ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T, U>(IEnumerable<ISurfaceObject<T, U>> surfaceGeom)
                    where T : ISurface<U> where U : IPoint;
    }
}
