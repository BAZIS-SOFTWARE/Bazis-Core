using Model.Interfaces;
using Model.MeshObjects;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Синхронизирует представление сетки в модели с геометрическим контроллером.
        /// </summary>
        private void SynchronizeGeometryMesh()
        {
            var before = CaptureData();
            ModelData.ObjectData.Clear(ObjType.Узел);
            ModelData.GroupData.ClearNotExisted();
            TaskData.ClearNotExisted(ModelData.GroupData);

            var nodes = GmshController.GetNodes();
            nodes.ForEach(node =>
                ModelData.ObjectData.NodesSet.Add(node.Number, node));

            foreach (var item in GmshController.GetMeshSets<Beam>(nodes))
                ModelData.ObjectData.E1DCollection.Add(item);

            foreach (var item in GmshController.GetMeshSets<Element2D>(nodes))
                ModelData.ObjectData.E2DCollection.Add(item);

            foreach (var item in GmshController.GetMeshSets<Element3D>(nodes))
                ModelData.ObjectData.E3DCollection.Add(item);

            try
            {
                MarkModelSurfaces(ModelData);
            }
            finally
            {
                PublishDataDifference(before, true);
                ModelView.Prune();
            }
        }
    }
}
