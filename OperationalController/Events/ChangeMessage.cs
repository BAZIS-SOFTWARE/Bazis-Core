using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OperationalController
{
    /// <summary>
    /// Базовый тип одного изменения проекта.
    /// </summary>
    public abstract class ChangeMessage { }

    public enum GeometryEntityKind { Point, Curve, Surface, Volume }
    public enum MeshDataKind { Set, Group }
    public enum ChangeAction { Created, Updated, Deleted }
    public enum ProjectChangeKind { Loaded, Created, Closed, PropertyChanged }
    public enum ProjectProperty { Name, FilePath, TaskKind, TaskType, MaterialsDB, FunctionsDB }

    /// <summary>
    /// Передаёт снимок количества геометрических сущностей после изменения.
    /// </summary>
    public sealed class GeoChange : ChangeMessage
    {
        /// <summary>
        /// Создаёт снимок количеств геометрических сущностей.
        /// </summary>
        public GeoChange(IReadOnlyDictionary<GeometryEntityKind, int> counts)
        {
            if (counts == null)
                throw new ArgumentNullException(nameof(counts));
            foreach (var kind in Enum.GetValues<GeometryEntityKind>())
                if (!counts.TryGetValue(kind, out var count) || count < 0)
                    throw new ArgumentException("Counts must contain every geometry kind with a nonnegative value.", nameof(counts));
            Counts = new ReadOnlyDictionary<GeometryEntityKind, int>(new Dictionary<GeometryEntityKind, int>(counts));
        }

        public IReadOnlyDictionary<GeometryEntityKind, int> Counts { get; }
    }

    /// <summary>
    /// Адресует изменение набора или группы в пределах текущего проекта.
    /// </summary>
    public sealed class MeshChange : ChangeMessage
    {
        /// <summary>
        /// Описывает изменение набора или группы по устойчивому номеру.
        /// </summary>
        public MeshChange(MeshDataKind dataKind, ChangeAction action, ObjType objectType, int entityId)
        {
            DataKind = dataKind;
            Action = action;
            ObjectType = objectType;
            EntityId = entityId;
        }

        public MeshDataKind DataKind { get; }
        public ChangeAction Action { get; }
        public ObjType ObjectType { get; }
        public int EntityId { get; }
    }

    /// <summary>
    /// Адресует изменение условия индексом его списка.
    /// </summary>
    public sealed class CondChange : ChangeMessage
    {
        /// <summary>
        /// Описывает изменение условия по индексу его списка.
        /// </summary>
        public CondChange(DataKind dataKind, ChangeAction action, int id)
        {
            DataKind = dataKind;
            Action = action;
            Id = id;
        }

        public DataKind DataKind { get; }
        public ChangeAction Action { get; }
        public int Id { get; }
    }

    /// <summary>
    /// Передаёт затронутые наборы и изменения их представления.
    /// </summary>
    public sealed class ViewChange : ChangeMessage
    {
        private readonly IReadOnlyDictionary<ISetInfo, ModelViewChange> changes;

        /// <summary>
        /// Копирует изменения представления затронутых наборов.
        /// </summary>
        public ViewChange(IReadOnlyDictionary<ISetInfo, ModelViewChange> changes)
        {
            if (changes == null)
                throw new ArgumentNullException(nameof(changes));
            this.changes = new ReadOnlyDictionary<ISetInfo, ModelViewChange>(new Dictionary<ISetInfo, ModelViewChange>(changes, ReferenceEqualityComparer.Instance));
            var kind = ModelViewChange.None;
            foreach (var change in changes.Values)
                kind |= change;
            ChangeKind = kind;
        }

        public ModelViewChange ChangeKind { get; }
        public IEnumerable<ISetInfo> AffectedSets => changes.Keys;

        /// <summary>
        /// Возвращает изменение представления заданного набора.
        /// </summary>
        public ModelViewChange GetChangeKind(ISetInfo setInfo)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));
            changes.TryGetValue(setInfo, out var change);
            return change;
        }
    }

    /// <summary>
    /// Описывает замену проекта или изменение его свойства.
    /// </summary>
    public sealed class ProjectChange : ChangeMessage
    {
        /// <summary>
        /// Описывает замену проекта или изменение его свойства.
        /// </summary>
        public ProjectChange(ProjectChangeKind changeKind, ProjectProperty? property = null)
        {
            if ((changeKind == ProjectChangeKind.PropertyChanged) != property.HasValue)
                throw new ArgumentException("Property is required only for property changes.", nameof(property));
            ChangeKind = changeKind;
            Property = property;
        }

        public ProjectChangeKind ChangeKind { get; }
        public ProjectProperty? Property { get; }
    }
}
