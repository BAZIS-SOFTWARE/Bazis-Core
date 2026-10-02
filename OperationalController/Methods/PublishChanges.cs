using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Project.Interfaces.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>Удаляет запись геометрии и возвращает признак изменения набора.</summary>
        private bool RemoveGeometryEntry<T>(IObjectsSet<T> set, int number) where T : IModelObject
        {
            if (!set.TryGetValue(number, out var item))
                return false;
            item.ExistState = false;
            set.Remove(number);
            return true;
        }
        /// <summary>Добавляет объект в набор коллекции и регистрирует выполненные изменения.</summary>
        private void AddCollectionObject<T>(IObjectsCollection<T> collection, string name, T item) where T : IModelObject
        {
            var created = !collection.ContainsName(name);
            if (created)
                collection.Add(name);
            collection.Add(name, item);
            var set = collection[name];
            PublishSetChange(set, created ? ChangeAction.Created : ChangeAction.Updated);
        }

        /// <summary>Добавляет набор объектов и регистрирует выполненные изменения.</summary>
        private void AddCollectionObjects<T>(IObjectsCollection<T> collection, string name, IEnumerable<T> items) where T : IModelObject
        {
            var created = !collection.ContainsName(name);
            if (created)
                collection.Add(name);
            var changed = false;
            foreach (var item in items)
            {
                collection.Add(name, item);
                changed = true;
            }
            if (created || changed)
            {
                var set = collection[name];
                PublishSetChange(set, created ? ChangeAction.Created : ChangeAction.Updated);
            }
        }

        /// <summary>Добавляет объекты постоянного набора и возвращает признак изменения.</summary>
        private bool AddSetObjects<T>(IObjectsSet<T> set, IEnumerable<T> items) where T : IModelObject
        {
            var changed = false;
            foreach (var item in items)
            {
                set.Add(item.Number, item);
                changed = true;
            }
            return changed;
        }

        /// <summary>Добавляет подготовленные наборы и возвращает сведения о них.</summary>
        private IReadOnlyCollection<ISetInfo> AddCollectionSets<T>(IObjectsCollection<T> collection, IEnumerable<Model.ObjectsCollections.ObjectsSet<T>> sets) where T : IModelObject
        {
            var added = new List<ISetInfo>();
            foreach (var set in sets)
            {
                collection.Add(set);
                added.Add(set);
            }
            return added;
        }
        /// <summary>Сообщает об изменённых наборах и добавленных группах после успешного добавления модели.</summary>
        private void PublishAppendedModel(IModelData appendedModel, IReadOnlyList<string> appendedNames, IReadOnlyCollection<ISetInfo> surfaceChanges)
        {
            var source = appendedModel.ObjectData;
            var target = ModelData.ObjectData;
            var updatedSets = new HashSet<ISetInfo>(surfaceChanges, ReferenceEqualityComparer.Instance);
            var createdSets = new HashSet<ISetInfo>(ReferenceEqualityComparer.Instance);
            if (source.NodesSet.Count != 0)
                updatedSets.Add(target.NodesSet);

            // Первое имя относится к постоянному набору узлов; далее идут новые наборы элементов.
            var nextName = 1;
            var elementTypes = new[] { ObjType.Элемент1D, ObjType.Элемент2D, ObjType.Элемент3D };
            foreach (var objectType in elementTypes)
                foreach (var sourceSet in source.GetSetsInfo(objectType))
                {
                    var name = appendedNames[nextName++];
                    var addedSet = target.GetSetInfo(objectType, name);
                    createdSets.Add(addedSet);
                }

            if (source.PointsSet.Count != 0)
                updatedSets.Add(target.PointsSet);
            if (source.CurveSet.Count != 0)
                updatedSets.Add(target.CurveSet);
            if (source.SurfaceSet.Count != 0)
                updatedSets.Add(target.SurfaceSet);

            updatedSets.ExceptWith(createdSets);
            foreach (var set in createdSets)
                PublishSetChange(set, ChangeAction.Created);
            foreach (var set in updatedSets)
                PublishSetChange(set, ChangeAction.Updated);

            foreach (var group in appendedModel.GroupData)
                PublishGroupChange(group, ChangeAction.Created);
        }

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
