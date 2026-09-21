using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using OperationalController.MeshObjsUtility;
using OperationalController.ModelScenePresentator;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        PresentersCreator presentersCreator = new PresentersCreator();
        public IObjsPresenter CreateModelObjectsPresentor(ISetInfo setInfo)
        {
            IObjsPresenter pre;
            //var objType = ;
            switch (setInfo.ObjType)
            {
                case ObjType.Узел:
                    pre = presentersCreator.CreatePointObjectsPresenter(ModelData.ObjectData.NodesSet.Values, ModelView, setInfo);
                    break;
                case ObjType.Кривая:
                    pre = presentersCreator?.CreateLineObjectsPresenter(ModelData.ObjectData.CurveSet.Values, ModelView, setInfo);
                    break;
                case ObjType.Поверхность:
                    pre = presentersCreator.CreateSurfaceObjectsPresenter(ModelData.ObjectData.SurfaceSet.Values, ModelView, setInfo);
                    break;
                case ObjType.Элемент1D:
                    pre = presentersCreator.CreateLineObjectsPresenter(ModelData.ObjectData.E1DCollection[setInfo.Name].Values, ModelView, setInfo);
                    break;
                case ObjType.Элемент2D:
                    pre = presentersCreator.CreateSurfaceObjectsPresenter(ModelData.ObjectData.E2DCollection[setInfo.Name].Values, ModelView, setInfo);
                    break;
                case ObjType.Элемент3D:
                    pre = presentersCreator.CreateSurfaceObjectsPresenter(ModelData.ObjectData.E3DCollection[setInfo.Name].Values, ModelView, setInfo);
                    break;
                default:
                    pre = presentersCreator.CreatePointObjectsPresenter(ModelData.ObjectData.PointsSet.Values, ModelView, setInfo);
                    break;
            }
            // изменить в будущем на массив viewMode
            //var setInfo = ModelData.ObjectData.GetSetsInfo(objType).FirstOrDefault();
            //if (setInfo != null)
            pre.ViewMode = ModelView.GetViewMode(setInfo);
            pre.Name = setInfo.Name;
            return pre;
        }
    }
}
