using Model.Interfaces;
using ModelControllerInterfaces;
using ProjectController.MeshObjsUtility;
using ProjectController.ModelScenePresentator;
using System.Linq;

namespace ProjectController
{
    public partial class ProjectController
    {
        PresentersCreator presentersCreator = new PresentersCreator();
        ChangeInsideSurface changeInsideSurface => new ChangeInsideSurface();

        public IObjsPresenter CreateModelObjectsPresentor(ObjType objType, bool isInsideObjectsShown)
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
                    if (isInsideObjectsShown)
                        changeInsideSurface.HideInsideSurfaces(ModelData.ObjectData.E3DCollection.GetObjects());
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
