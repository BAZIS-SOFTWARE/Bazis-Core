using Model.Interfaces;
using ModelController.ModelScenePresentator.GlObjsPresenters;
using ModelControllerInterfaces;
using System.Collections.Generic;

namespace ModelController.ModelScenePresentator
{
    /// <summary>
    /// ModelScenePresentator
    /// </summary>
    public class PresentersCreator : IPresentersCreator
    {
        /// <inheritdoc/> 
        public IObjsPresenter CreatePointObjectsPresenter(IEnumerable<IModelObject> modelObjects)
        {
            var pointsPresentor = new PointObjsPresenter();
            pointsPresentor.Add(modelObjects);
            return pointsPresentor;
        }

/// <inheritdoc/>

        public IObjsPresenter CreateLineObjectsPresenter<T>(IEnumerable<ILineObject<T>> lineObjs)
            where T : IPoint
        {
            var linePres = new LineObjsPresenter<T>();
            linePres.Add(lineObjs);
            return linePres;
        }

/// <inheritdoc/>

        public ISurfaceObjsPresenter CreateSurfaceObjectsPresenter<T,U>(IEnumerable<ISurfaceObject<T, U>> surfaceGeom, bool isVolumeObjs)
                    where T : ISurface<U> where U : IPoint
        {

            var presentor = new SurfaceObjsPresenter<T, U>(isVolumeObjs);
            presentor.Add(surfaceGeom);
            return presentor;
        }
    }
}
