using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using System.Collections.Generic;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void ClearGroupData()
        {
            foreach (var group in ModelData.GroupData.ToArray())
            {
                ModelData.GroupData.Remove(group);
                PublishGroupChange(group, ChangeAction.Deleted);
                DeleteCond(group);
            }
            ClearTaskData();
        }

        /// <summary>Удаляет условия указанной группы после регистрации каждого удаления.</summary>
        public void DeleteCond(IGroup group)
        {
            for (var index = TaskData.Count - 1; index >= 0; index--)
                if (ReferenceEquals(TaskData[index].Group, group))
                    DeleteCond(index);
        }

        /// <inheritdoc/>
        public void ClearNotExistedCondData()
        {
            for (var index = TaskData.Count - 1; index >= 0; index--)
                if (!ModelData.GroupData.Contains(TaskData[index].Group))
                    DeleteCond(index);
        }

        /// <inheritdoc/>
        public void ClearNotExistedModelData()
        {
            var data = ModelData.ObjectData;
            var changedSets = new List<ISetInfo>();
            if (RemoveDeadObjects(data.PointsSet))
                changedSets.Add(data.PointsSet);
            if (RemoveDeadObjects(data.CurveSet))
                changedSets.Add(data.CurveSet);
            if (RemoveDeadObjects(data.SurfaceSet))
                changedSets.Add(data.SurfaceSet);
            if (RemoveDeadObjects(data.NodesSet))
                changedSets.Add(data.NodesSet);
            var changed1D = RemoveDeadObjects(data.E1DCollection);
            var changed2D = RemoveDeadObjects(data.E2DCollection);
            var changed3D = RemoveDeadObjects(data.E3DCollection);
            changedSets.AddRange(changed1D);
            changedSets.AddRange(changed2D);
            changedSets.AddRange(changed3D);
            foreach (var set in changedSets)
                PublishSetChange(set, ChangeAction.Updated);
            ClearEmptySet();
            ClearNotExistedGroupData();
            ModelView.Prune();
        }

        /// <inheritdoc/>
        public void ClearNotExistedGroupData()
        {
            foreach (var group in ModelData.GroupData.ToArray())
            {
                var changed = false;
                for (var index = group.Count - 1; index >= 0; index--)
                    if (!group[index].ExistState)
                    {
                        group.RemoveAt(index);
                        changed = true;
                    }
                if (group.Count == 0)
                {
                    ModelData.GroupData.Remove(group);
                    PublishGroupChange(group, ChangeAction.Deleted);
                    DeleteCond(group);
                }
                else if (changed)
                    PublishGroupChange(group, ChangeAction.Updated);
            }
            ClearNotExistedCondData();
        }

        /// <inheritdoc/>
        public void ClearEmptySet()
        {
            var data = ModelData.ObjectData;
            RemoveEmptySets(data.E1DCollection);
            RemoveEmptySets(data.E2DCollection);
            RemoveEmptySets(data.E3DCollection);
        }

        /// <inheritdoc/>
        public void ClearTaskData()
        {
            for (var index = TaskData.Count - 1; index >= 0; index--)
                DeleteCond(index);
        }

        /// <inheritdoc/>
        public void ClearAllData()
        {
            ClearModelCollection(ObjType.Точка);
            ClearModelCollection(ObjType.Узел);
            ClearGroupData();
            ModelView.Prune();
        }

        /// <inheritdoc/>
        public void ClearModelCollection(ObjType objType)
        {
            var data = ModelData.ObjectData;
            switch (objType)
            {
                case ObjType.Точка:
                    ClearSet(data.PointsSet);
                    ClearSet(data.CurveSet);
                    ClearSet(data.SurfaceSet);
                    break;
                case ObjType.Кривая:
                    ClearSet(data.CurveSet);
                    ClearSet(data.SurfaceSet);
                    break;
                case ObjType.Поверхность:
                    ClearSet(data.SurfaceSet);
                    break;
                case ObjType.Узел:
                    ClearSet(data.NodesSet);
                    ClearCollection(data.E1DCollection);
                    ClearCollection(data.E2DCollection);
                    ClearCollection(data.E3DCollection);
                    break;
                case ObjType.Элемент1D:
                    ClearCollection(data.E1DCollection);
                    break;
                case ObjType.Элемент2D:
                    ClearCollection(data.E2DCollection);
                    break;
                case ObjType.Элемент3D:
                    ClearCollection(data.E3DCollection);
                    break;
            }
            ClearNotExistedGroupData();
            ModelView.Prune();
        }

        /// <inheritdoc/>
        public void DeleteModelGroup(string grName)
        {
            var group = ModelData.GroupData.Find(grName);
            if (group == null || !ModelData.GroupData.Remove(group))
                return;
            PublishGroupChange(group, ChangeAction.Deleted);
            DeleteCond(group);
        }

        /// <inheritdoc/>
        public bool DeleteCond(int index)
        {
            var data = TaskData[index];
            TaskData.RemoveAt(index);
            PublishConditionChange(data.Kind, ChangeAction.Deleted, index);
            return true;
        }

        /// <inheritdoc/>
        public void DeleteModelSet(ObjType objType, string name)
        {
            var set = ModelData.ObjectData.GetSetInfo(objType, name);
            if (set == null)
                return;
            ModelData.ObjectData.Remove(objType, name);
            PublishSetChange(set, ChangeAction.Deleted);
            ClearNotExistedGroupData();
            ModelView.Prune();
        }

        /// <inheritdoc/>
        public void DeleteModelObjects(ObjType objType, IEnumerable<int> objNumbs)
        {
            foreach (var number in objNumbs)
            {
                var obj = ModelData.ObjectData.Find(objType, number);
                if (obj != null && obj.ExistState)
                {
                    obj.ExistState = false;
                }
            }
            ClearNotExistedModelData();
        }

        /// <summary>Очищает постоянный набор и регистрирует его изменение.</summary>
        private void ClearSet<T>(IObjectsSet<T> set) where T : IModelObject
        {
            if (set.Count == 0)
                return;
            set.ClearWithExistState();
            PublishSetChange(set, ChangeAction.Updated);
        }

        /// <summary>Удаляет наборы коллекции и затем сообщает об удалении каждого набора.</summary>
        private void ClearCollection<T>(IObjectsCollection<T> collection) where T : IModelObject
        {
            var sets = collection.Objects.ToArray();
            foreach (var set in sets)
                collection.Remove(set.Name);
            collection.Clear();
            foreach (var set in sets)
                PublishSetChange(set, ChangeAction.Deleted);
        }

        /// <summary>Удаляет пустые наборы коллекции с регистрацией.</summary>
        private void RemoveEmptySets<T>(IObjectsCollection<T> collection) where T : IModelObject
        {
            var sets = collection.Objects.Where(set => set.Count == 0).ToArray();
            foreach (var set in sets)
                collection.Remove(set.Name);
            foreach (var set in sets)
                PublishSetChange(set, ChangeAction.Deleted);
        }

        /// <summary>Удаляет несуществующие объекты из коллекции наборов.</summary>
        private IReadOnlyCollection<ISetInfo> RemoveDeadObjects<T>(IObjectsCollection<T> collection) where T : IModelObject
        {
            var changedSets = new List<ISetInfo>();
            foreach (var set in collection.Objects)
                if (RemoveDeadObjects(set) && set.Count != 0)
                    changedSets.Add(set);
            return changedSets;
        }

        /// <summary>Удаляет несуществующие объекты набора и возвращает признак изменения.</summary>
        private bool RemoveDeadObjects<T>(IObjectsSet<T> set) where T : IModelObject
        {
            var keys = set.Where(pair => !pair.Value.ExistState).Select(pair => pair.Key).ToArray();
            foreach (var key in keys)
                set.Remove(key);
            return keys.Length != 0;
        }
    }
}
