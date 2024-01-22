using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.MeshObjects;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IModelPresenter
    /// </summary>
    public interface IModelScenePresenter
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
    }
}
