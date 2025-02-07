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
        Dictionary<string, PresenterView> ObjsViews = new Dictionary<string, PresenterView>();

        Dictionary<string, ObjectsItem> ObjsItems = new Dictionary<string, ObjectsItem>();
        ///// <inheritdoc/>

        //public int TransparencyValue { get; set; }

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


        public void SetView(string objsType, PresenterView objsView)
        {
            ObjsViews[objsType] = objsView;
        }
/// <inheritdoc/>

        public PresenterView GetView(string objType)
        {
            return ObjsViews[objType];
        }
///// <inheritdoc/>

//        public void SetColor(ObjType objsType, Color objsColor)
//        {
//            ObjsColors[objsType] = objsColor;
//        }
///// <inheritdoc/>

//        public Color GetColor(ObjType objsType)
//        {
//            return ObjsColors[objsType];
//        }
    }
}
