using ModelInterfaces;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IModelPresenter
    /// </summary>
    public interface IModelScenePresenter : IDictionary<string, IObjsPresenter>
    {
        /// <summary>
        /// HideInsideSurfaces
        /// </summary>
        /// <param name="surfaceObjects"></param>
        void HideInsideSurfaces(IEnumerable<ISurfaceObject> surfaceObjects);
        /// <summary>
        /// ShowInsideSurfaces
        /// </summary>
        /// <param name="surfaceElements"></param>
        void ShowInsideSurfaces(IEnumerable<ISurfaceObject> surfaceElements);
        /// <summary>
        /// CreatePointObjectsPresenter
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <returns></returns>
        IObjsPresenter CreatePointObjectsPresenter(IEnumerable<IModelObject> modelObjects);
        /// <summary>
        /// CreateLineObjectsPresenter
        /// </summary>
        /// <param name="lineObjs"></param>
        /// <returns></returns>
        IObjsPresenter CreateLineObjectsPresenter(IEnumerable<ILineObject> lineObjs);
        /// <summary>
        /// CreateSurfaceObjectsPresenter
        /// </summary>
        /// <param name="surfaceObjs"></param>
        /// <returns></returns>
        IObjsPresenter CreateSurfaceObjectsPresenter(IEnumerable<ISurfaceObject> surfaceObjs);
    }
}
