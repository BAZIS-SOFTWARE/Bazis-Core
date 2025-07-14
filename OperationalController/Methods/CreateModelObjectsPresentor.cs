using Model;
using Model.Interfaces;
using OperationalController;
using OperationalController.MeshObjsUtility;
using OperationalController.ModelScenePresentator;
using System.Linq;

namespace OperationalController
{
    public partial class Controller
    {
        PresentersCreator presentersCreator = new PresentersCreator();
        ChangeInsideSurface changeInsideSurface => new ChangeInsideSurface();
/// <inheritdoc/>

        public void ChangeInsideSurfacesState(bool state)
        {
            var e3D = ModelData.ObjectData.E3DCollection.GetObjects();
            
            if(state)
                changeInsideSurface.ShowInsideSurfaces(e3D);
            else
                changeInsideSurface.HideInsideSurfaces(e3D);
        }
/// <inheritdoc/>

        public IObjsPresenter CreateModelObjectsPresentor(ObjType objType)
        {
            IObjsPresenter pre;
            switch (objType)
            {
                case ObjType.Узел:
                    pre = presentersCreator.CreatePointObjectsPresenter(ModelData.ObjectData.NodesSet.Values);
                    break;
                case ObjType.Кривая:
                    pre = presentersCreator?.CreateLineObjectsPresenter(ModelData.ObjectData.CurveCollection.GetObjects());
                    break;
                case ObjType.Поверхность:
                    pre = presentersCreator.CreateSurfaceObjectsPresenter(ModelData.ObjectData.SurfaceCollection.GetObjects());
                    break;
                case ObjType.Объем:
                    pre = presentersCreator.CreateSurfaceObjectsPresenter(ModelData.ObjectData.VolumeCollection.GetObjects());
                    break;
                case ObjType.Элемент1D:
                    pre = presentersCreator.CreateLineObjectsPresenter(ModelData.ObjectData.E1DCollection.GetObjects());
                    break;
                case ObjType.Элемент2D:
                    pre = presentersCreator.CreateSurfaceObjectsPresenter(ModelData.ObjectData.E2DCollection.GetObjects());
                    break;
                case ObjType.Элемент3D:
                    pre = presentersCreator.CreateSurfaceObjectsPresenter(ModelData.ObjectData.E3DCollection.GetObjects());
                    break;
                default:
                    pre = presentersCreator.CreatePointObjectsPresenter(ModelData.ObjectData.PointsSet.Values);
                    break;
            }
            // изменить в будущем на массив viewMode
            var setInfo = ModelData.ObjectData.GetSetsInfo(objType).FirstOrDefault();
            if (setInfo != null)
                pre.ViewMode = setInfo.ViewMode;
            pre.Name = objType.ToString();
            return pre;
        }
    }
}
