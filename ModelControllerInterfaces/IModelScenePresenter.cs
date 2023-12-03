using ModelInterfaces;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IModelPresenter
    /// </summary>
    public interface IModelScenePresenter : IDictionary<ObjType, IObjsPresenter>
    {
        /// <summary>
        /// HideInsideSurfaces
        /// </summary>
        /// <param name="surfaceObjects"></param>
        void HideInsideSurfaces(IEnumerable<ISurfaceObject<IElementSurface,INode>> surfaceObjects);
        /// <summary>
        /// ShowInsideSurfaces
        /// </summary>
        /// <param name="surfaceElements"></param>
        void ShowInsideSurfaces(IEnumerable<ISurfaceObject<IElementSurface, INode>> surfaceElements);
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
        IObjsPresenter CreateBeamPresenter(IEnumerable<ILineObject<INode>> lineObjs);
        /// <summary>
        /// CreateGeometryLinePresenter
        /// </summary>
        /// <param name="lineObjs"></param>
        /// <returns></returns>
        IObjsPresenter CreateGeometryLinePresenter(IEnumerable<ILineObject<IPoint>> lineObjs);
        /// <summary>
        /// CreateSurfaceElementPresenter
        /// </summary>
        /// <param name="surfaceElems"></param>
        /// <returns></returns>
        IObjsPresenter CreateSurfaceElementsPresenter(IEnumerable<ISurfaceObject<IElementSurface,INode>> surfaceElems);
        /// <summary>
        /// CreateSurfaceElementPresenter
        /// </summary>
        /// <param name="surfaceGeom"></param>
        /// <returns></returns>
        IObjsPresenter CreateSurfaceGeometryPresenter(IEnumerable<ISurfaceObject<IGeometrySurface, IPoint>> surfaceGeom);
        /// <summary>
        /// CreatePresenter
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <param name="objsType"></param>
        void CreatePresenter(List<IModelObject> modelObjects, ObjType objsType);
    }
}
