using Model.Interfaces;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IModelPresenter
    /// </summary>
    public interface IPresentersCreator
    {
        ///// <summary>
        ///// TransparencyValue
        ///// </summary>
        //int TransparencyValue { get; set; }
        ///// <summary>
        ///// SetView
        ///// </summary>
        ///// <param name="objsType"></param>
        ///// <param name="objsView"></param>
        void SetView(string objsType, PresenterView objsView);
        /// <summary>
        /// GetView
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        PresenterView GetView(string objType);
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
        /// <param name="isVolumeObjs"></param>
        /// <returns></returns>
        IObjsPresenter CreateSurfaceObjectsPresenter<T, U>(IEnumerable<ISurfaceObject<T, U>> surfaceGeom, bool isVolumeObjs)
                    where T : ISurface<U> where U : IPoint;
    }
}
