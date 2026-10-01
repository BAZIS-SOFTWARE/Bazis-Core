using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Project.Interfaces.Tasks;
using System.Collections.Generic;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Публикует актуальные количества геометрических сущностей после синхронизации CAD.
        /// </summary>
        private void PublishGeometryChange()
        {
            var counts = new Dictionary<GeometryEntityKind, int>
            {
                [GeometryEntityKind.Point] = GmshController.Gmsh.Model.GetEntities(0).Length / 2,
                [GeometryEntityKind.Curve] = GmshController.Gmsh.Model.GetEntities(1).Length / 2,
                [GeometryEntityKind.Surface] = GmshController.Gmsh.Model.GetEntities(2).Length / 2,
                [GeometryEntityKind.Volume] = GmshController.Gmsh.Model.GetEntities(3).Length / 2
            };
            isModified = true;
            Publish(new GeoChange(counts));
        }

        /// <summary>
        /// Публикует изменение набора модели по устойчивому номеру.
        /// </summary>
        private void PublishSetChange(ISetInfo setInfo, ChangeAction action)
        {
            if (setInfo == null)
                return;
            isModified = true;
            Publish(new MeshChange(MeshDataKind.Set, action, setInfo.ObjType, setInfo.Number));
        }

        /// <summary>
        /// Публикует изменение группы модели по устойчивому номеру.
        /// </summary>
        private void PublishGroupChange(IGroup group, ChangeAction action)
        {
            if (group == null)
                return;
            isModified = true;
            Publish(new MeshChange(MeshDataKind.Group, action, group.ObjType, group.Number));
        }

        /// <summary>
        /// Публикует изменение условия после обновления списка.
        /// </summary>
        private void PublishConditionChange(DataKind kind, ChangeAction action, int index)
        {
            isModified = true;
            Publish(new CondChange(kind, action, index));
        }
    }
}
