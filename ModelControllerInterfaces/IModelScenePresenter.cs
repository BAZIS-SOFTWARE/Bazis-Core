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
        IObjsPresenter CreateGeometryLinePresenter(IEnumerable<ILineObject<IGeometryPoint>> lineObjs);
        /// <summary>
        /// CreateSurfaceElementsPresenter
        /// </summary>
        /// <param name="surfaceElems"></param>
        /// <param name="isVolumeObjs"></param>
        /// <returns></returns>
        IObjsPresenter CreateSurfaceElementsPresenter(IEnumerable<ISurfaceObject<IElementSurface,INode>> surfaceElems,bool isVolumeObjs);
        /// <summary>
        /// CreateSurfaceGeometryPresenter
        /// </summary>
        /// <param name="surfaceGeom"></param>
        /// <param name="isVolumeObjs"></param>
        /// <returns></returns>
        IObjsPresenter CreateSurfaceGeometryPresenter(IEnumerable<ISurfaceObject<IGeometrySurface, IGeometryPoint>> surfaceGeom, bool isVolumeObjs);
        /// <summary>
        /// CreatePresenter
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <param name="objsType"></param>
        void CreatePresenter(List<IModelObject> modelObjects, ObjType objsType);
    }
}
