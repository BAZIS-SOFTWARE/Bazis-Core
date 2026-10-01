using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Запоминает адреса и размеры данных до составной операции.
        /// </summary>
        private sealed class DataSnapshot
        {
            public Dictionary<(ObjType Type, int Id), int> Sets { get; } = new Dictionary<(ObjType, int), int>();
            public Dictionary<(ObjType Type, int Id), int> Groups { get; } = new Dictionary<(ObjType, int), int>();
            public List<ICondData> Conditions { get; } = new List<ICondData>();
        }

        /// <summary>
        /// Снимает адреса наборов, групп и условий перед изменением данных.
        /// </summary>
        private DataSnapshot CaptureData()
        {
            var snapshot = new DataSnapshot();
            foreach (var setInfo in GetAllModelSetsInfo())
                snapshot.Sets.Add((setInfo.ObjType, setInfo.Number), setInfo.NumberOfObjects);
            foreach (var group in ModelData.GroupData)
                snapshot.Groups.Add((group.ObjType, group.Number), group.Count);
            foreach (var condition in TaskData)
                snapshot.Conditions.Add(condition);
            return snapshot;
        }

        /// <summary>
        /// Публикует созданные, удалённые и изменившиеся по размеру данные после операции.
        /// </summary>
        private void PublishDataDifference(DataSnapshot before, bool updateExistingMeshSets = false)
        {
            var after = CaptureData();
            foreach (var pair in before.Sets)
                if (!after.Sets.ContainsKey(pair.Key))
                    Publish(new MeshChange(MeshDataKind.Set, ChangeAction.Deleted, pair.Key.Type, pair.Key.Id));
            foreach (var pair in after.Sets)
            {
                if (!before.Sets.TryGetValue(pair.Key, out var oldCount))
                    Publish(new MeshChange(MeshDataKind.Set, ChangeAction.Created, pair.Key.Type, pair.Key.Id));
                else if (oldCount != pair.Value || updateExistingMeshSets &&
                         (pair.Key.Type == ObjType.Узел || pair.Key.Type == ObjType.Элемент1D ||
                          pair.Key.Type == ObjType.Элемент2D || pair.Key.Type == ObjType.Элемент3D))
                    Publish(new MeshChange(MeshDataKind.Set, ChangeAction.Updated, pair.Key.Type, pair.Key.Id));
            }

            foreach (var pair in before.Groups)
                if (!after.Groups.ContainsKey(pair.Key))
                    Publish(new MeshChange(MeshDataKind.Group, ChangeAction.Deleted, pair.Key.Type, pair.Key.Id));
            foreach (var pair in after.Groups)
            {
                if (!before.Groups.TryGetValue(pair.Key, out var oldCount))
                    Publish(new MeshChange(MeshDataKind.Group, ChangeAction.Created, pair.Key.Type, pair.Key.Id));
                else if (oldCount != pair.Value)
                    Publish(new MeshChange(MeshDataKind.Group, ChangeAction.Updated, pair.Key.Type, pair.Key.Id));
            }

            for (var index = before.Conditions.Count - 1; index >= 0; index--)
                if (!after.Conditions.Contains(before.Conditions[index]))
                    Publish(new CondChange(before.Conditions[index].Kind, ChangeAction.Deleted, index));
            for (var index = 0; index < after.Conditions.Count; index++)
                if (!before.Conditions.Contains(after.Conditions[index]))
                    Publish(new CondChange(after.Conditions[index].Kind, ChangeAction.Created, index));
        }
    }
}
