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
            RegisterSetChange(set, created ? ChangeAction.Created : ChangeAction.Updated);
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
                RegisterSetChange(set, created ? ChangeAction.Created : ChangeAction.Updated);
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
        /// <summary>Регистрирует изменённые наборы и добавленные группы после успешного добавления модели.</summary>
        private void RegisterAppendedModelChanges(IModelData appendedModel, IReadOnlyList<string> appendedNames, IReadOnlyCollection<ISetInfo> surfaceChanges)
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
                RegisterSetChange(set, ChangeAction.Created);
            foreach (var set in updatedSets)
                RegisterSetChange(set, ChangeAction.Updated);

            foreach (var group in appendedModel.GroupData)
                RegisterGroupChange(group, ChangeAction.Created);
        }

        /// <summary>
        /// Регистрирует типы геометрических сущностей, изменённых после синхронизации CAD.
        /// </summary>
        private void RegisterGeometryChange(GeoChange.GeometryEntityKind entityKind)
        {
            isModified = true;
            RegisterChange(new GeoChange(entityKind));
        }

        /// <summary>
        /// Регистрирует изменение набора модели по устойчивому номеру.
        /// </summary>
        private void RegisterSetChange(ISetInfo setInfo, ChangeAction action)
        {
            if (setInfo == null)
                return;
            isModified = true;
            if (action == ChangeAction.Deleted)
                RegisterChange(new SetDelete(setInfo.ObjType, setInfo.Number, setInfo.Name));
            else
                RegisterChange(new MeshChange(MeshChange.MeshDataKind.Set, action, setInfo.ObjType, setInfo.Number));
        }

        /// <summary>
        /// Регистрирует изменение группы модели по устойчивому номеру.
        /// </summary>
        private void RegisterGroupChange(IGroup group, ChangeAction action)
        {
            if (group == null)
                return;
            isModified = true;
            RegisterChange(new MeshChange(MeshChange.MeshDataKind.Group, action, group.ObjType, group.Number));
        }

        /// <summary>
        /// Регистрирует изменение условия после обновления списка.
        /// </summary>
        private void RegisterConditionChange(DataKind kind, ChangeAction action, int index)
        {
            isModified = true;
            RegisterChange(new CondChange(kind, action, index));
        }
    }
}
