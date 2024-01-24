using Model;
using ModelInterfaces;
using ModelController.ModelScenePresentator.GlObjsPresenters;
using ModelControllerInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Model.MeshObjects;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.MeshObjects;

namespace ModelController.ModelScenePresentator
{
    /// <summary>
    /// ModelScenePresentator
    /// </summary>
    public class PresentersCreator : IPresentersCreator
    {
        Dictionary<ObjType, PresenterView> ObjsViews = new Dictionary<ObjType, PresenterView>();
        /// <summary>
        /// this
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        public PresenterView this[ObjType objType]
        {
            get { return ObjsViews[objType]; }
            set { ObjsViews[objType] = value; }
        }

        /// <summary>
        /// PresentorsCreator
        /// </summary>
        public PresentersCreator()
        {
            ObjsViews.Add(ObjType.Точка, PresenterView.Point);
            ObjsViews.Add(ObjType.Узел, PresenterView.Point);
            ObjsViews.Add(ObjType.Линия, PresenterView.Line);
            ObjsViews.Add(ObjType.Фигура2D, PresenterView.Surface);
            ObjsViews.Add(ObjType.Фигура3D, PresenterView.Surface);
            ObjsViews.Add(ObjType.Элемент1D, PresenterView.Line);
            ObjsViews.Add(ObjType.Элемент2D, PresenterView.Surface);
            ObjsViews.Add(ObjType.Элемент3D, PresenterView.Surface);
        }
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

        public IObjsPresenter CreateSurfaceObjectsPresenter<T,U>(IEnumerable<ISurfaceObject<T, U>> surfaceGeom, bool isVolumeObjs)
                    where T : ISurface<U> where U : IPoint
        {

            var presentor = new SurfaceObjsPresenter<T, U>(isVolumeObjs);
            presentor.Add(surfaceGeom);
            return presentor;
        }
    }
}
