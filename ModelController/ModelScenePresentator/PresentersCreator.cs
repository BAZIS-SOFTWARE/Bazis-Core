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

        public IObjsPresenter CreateBeamPresenter(IEnumerable<ILineObject<INode>> lineObjs)
        {
            var els1dPresentor = new LineObjsPresenter<INode>();
            els1dPresentor.Add(lineObjs);
            return els1dPresentor;
        }
/// <inheritdoc/>

        public IObjsPresenter CreateGeometryLinePresenter(IEnumerable<ILineObject<IGeometryPoint>> lineObjs)
        {
            var linePres = new LineObjsPresenter<IGeometryPoint>();
            linePres.Add(lineObjs);
            return linePres;
        }
/// <inheritdoc/>

        public IObjsPresenter CreateSurfaceElementsPresenter(IEnumerable<ISurfaceObject<IElementSurface, INode>> surfaceElems, bool isVolumeObjs)
        {
            var elsPresentor = new SurfaceObjsPresenter<IElementSurface, INode>(isVolumeObjs);
            elsPresentor.Add(surfaceElems);
            return elsPresentor;
        }
/// <inheritdoc/>

        public IObjsPresenter CreateSurfaceGeometryPresenter(IEnumerable<ISurfaceObject<IGeometrySurface, IGeometryPoint>> surfaceGeom, bool isVolumeObjs)
        {
            var fig2dPresentor = new SurfaceObjsPresenter<IGeometrySurface, IGeometryPoint>(isVolumeObjs);
            fig2dPresentor.Add(surfaceGeom);
            return fig2dPresentor;
        }
/// <inheritdoc/>

        public IObjsPresenter CreateOtherSurfacePresenter(IEnumerable<ISurfaceObject<ISurface<IPoint>, IPoint>> surfaceGeom, bool isVolumeObjs)
        {
            var fig2dPresentor = new SurfaceObjsPresenter<ISurface<IPoint>, IPoint>(isVolumeObjs);
            fig2dPresentor.Add(surfaceGeom);
            return fig2dPresentor;
        }
    }
}
