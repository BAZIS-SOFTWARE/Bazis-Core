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
        Dictionary<string, PresenterView> ObjsViews = new Dictionary<string, PresenterView>();
        /// <summary>
        /// this
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        public PresenterView this[string objType]
        {
            get { return ObjsViews[objType]; }
            set { ObjsViews[objType] = value; }
        }

        /// <summary>
        /// PresentorsCreator
        /// </summary>
        public PresentersCreator()
        {
            ObjsViews.Add(ObjType.Точка.ToString(), PresenterView.Point);
            ObjsViews.Add(ObjType.Узел.ToString(), PresenterView.Point);
            ObjsViews.Add(ObjType.Линия.ToString(), PresenterView.Line);
            ObjsViews.Add(ObjType.Фигура2D.ToString(), PresenterView.Surface);
            ObjsViews.Add(ObjType.Фигура3D.ToString(), PresenterView.Surface);
            ObjsViews.Add(ObjType.Элемент1D.ToString(), PresenterView.Line);
            ObjsViews.Add(ObjType.Элемент2D.ToString(), PresenterView.Surface);
            ObjsViews.Add(ObjType.Элемент3D.ToString(), PresenterView.Surface);
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
/// <inheritdoc/>

        public void Add(string objsType, PresenterView objsView)
        {
            ObjsViews.Add(objsType, objsView);
        }
/// <inheritdoc/>

        public bool Remove(string objsType)
        {
            return ObjsViews.Remove(objsType);
        }
    }
}
