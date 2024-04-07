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
        Dictionary<string, int> ObjsTransparency = new Dictionary<string, int>();
         

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

            ObjsTransparency.Add(ObjType.Точка.ToString(), 100);
            ObjsTransparency.Add(ObjType.Узел.ToString(), 100);
            ObjsTransparency.Add(ObjType.Линия.ToString(), 100);
            ObjsTransparency.Add(ObjType.Фигура2D.ToString(), 100);
            ObjsTransparency.Add(ObjType.Фигура3D.ToString(), 100);
            ObjsTransparency.Add(ObjType.Элемент1D.ToString(), 100);
            ObjsTransparency.Add(ObjType.Элемент2D.ToString(), 100);
            ObjsTransparency.Add(ObjType.Элемент3D.ToString(), 100);
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

        public void SetTransparency(string objType, int alpha)
        {
            ObjsTransparency[objType] = alpha;
        }
/// <inheritdoc/>

        public int GetTransparency(string objType)
        {
            return ObjsTransparency[objType];
        }
/// <inheritdoc/>


        public void SetView(string objsType, PresenterView objsView)
        {
            ObjsViews.Add(objsType, objsView);
        }
/// <inheritdoc/>

        public PresenterView GetView(string objType)
        {
            return ObjsViews[objType];
        }
    }
}
