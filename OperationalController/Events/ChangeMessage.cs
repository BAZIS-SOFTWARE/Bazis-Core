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

    public enum ChangeAction { Created, Updated, Deleted }

    /// <summary>
    /// Передаёт типы изменённых геометрических сущностей.
    /// </summary>
    public sealed class GeoChange : ChangeMessage
    {
        [Flags]
        public enum GeometryEntityKind
        {
            None = 0,
            Point = 1,
            Curve = 2,
            Surface = 4,
            Volume = 8
        }

        /// <summary>
        /// Описывает изменённые типы геометрических сущностей.
        /// </summary>
        public GeoChange(GeometryEntityKind entityKind)
        {
            var allKinds = GeometryEntityKind.Point | GeometryEntityKind.Curve | GeometryEntityKind.Surface | GeometryEntityKind.Volume;
            if (entityKind == GeometryEntityKind.None || (entityKind & ~allKinds) != GeometryEntityKind.None)
                throw new ArgumentOutOfRangeException(nameof(entityKind), "At least one valid geometry entity kind is required.");
            EntityKind = entityKind;
        }

        public GeometryEntityKind EntityKind { get; }
    }

    /// <summary>
    /// Адресует изменение набора или группы в пределах текущего проекта.
    /// </summary>
    public sealed class MeshChange : ChangeMessage
    {
        public enum MeshDataKind { Set, Group }

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
    /// Описывает удаление набора модели.
    /// </summary>
    public sealed class SetDelete : ChangeMessage
    {
        /// <summary>
        /// Передаёт имя удалённого набора и его устойчивый адрес.
        /// </summary>
        public SetDelete(ObjType objectType, int entityId, string name)
        {
            ObjectType = objectType;
            EntityId = entityId;
            Name = name;
        }

        public ObjType ObjectType { get; }
        public int EntityId { get; }
        public string Name { get; }
    }

    /// <summary>
    /// Описывает переименование набора модели.
    /// </summary>
    public sealed class SetRename : ChangeMessage
    {
        /// <summary>
        /// Передаёт прежнее и новое имя набора по его устойчивому номеру.
        /// </summary>
        public SetRename(ObjType objectType, int entityId, string oldName, string newName)
        {
            ObjectType = objectType;
            EntityId = entityId;
            OldName = oldName;
            NewName = newName;
        }

        public ObjType ObjectType { get; }
        public int EntityId { get; }
        public string OldName { get; }
        public string NewName { get; }
    }

    /// <summary>
    /// Описывает переименование группы модели.
    /// </summary>
    public sealed class GroupRename : ChangeMessage
    {
        /// <summary>
        /// Передаёт прежнее и новое имя группы по её устойчивому номеру.
        /// </summary>
        public GroupRename(ObjType objectType, int entityId, string oldName, string newName)
        {
            ObjectType = objectType;
            EntityId = entityId;
            OldName = oldName;
            NewName = newName;
        }

        public ObjType ObjectType { get; }
        public int EntityId { get; }
        public string OldName { get; }
        public string NewName { get; }
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
        public enum ProjectChangeKind { Loaded, Created, PropertyChanged }
        public enum ProjectProperty { Name, FilePath, TaskKind, TaskType, MaterialsDB, FunctionsDB }

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
