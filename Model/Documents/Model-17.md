# Model-17 — Модель, наборы и группы

[Диаграмма](Model-17.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ModelObject`

Пространство имён: `Model`.

Исходники: [ModelObject.cs](../../Model/ModelObject.cs).

Базовые типы: `IModelObject`.

```text
CompareTo(IModelObject other) : int
Equals(IModelObject other) : bool
Equals(IModelObject x, IModelObject y) : bool
GetHashCode(IModelObject obj) : int
ModelObject(int number)
ToString() : string
```

## `ObjectsData`

Пространство имён: `Model`.

Исходники: [ObjectsData.cs](../../Model/ObjectsData.cs).

Базовые типы: `IObjectsData`.

```text
IsReadOnly : bool [get]
ObjectsData()
```

## `Group`

Пространство имён: `Model.GroupsData`.

Исходники: [Group.cs](../../Model/GroupsData/Group.cs).

Базовые типы: `IGroup`.

```text
Count : int [get]
IsReadOnly : bool [get]
this[int key] : IModelObject [get, set]
Add(IModelObject item) : void
Clear() : void
Contains(IModelObject item) : bool
CopyTo(IModelObject[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<IModelObject>
Group(int number, string groupName, ObjType objType)
Group(string groupName, ObjType objType)
IndexOf(IModelObject item) : int
Insert(int index, IModelObject item) : void
Remove(IModelObject item) : bool
RemoveAt(int index) : void
ToString() : string
```

## `GroupData`

Пространство имён: `Model.GroupsData`.

Исходники: [GroupData.cs](../../Model/GroupsData/GroupData.cs).

Базовые типы: `IGroupData`.

```text
Count : int [get]
IsReadOnly : bool [get]
this[int index] : IGroup [get, set]
Add(IGroup item) : void
Clear() : void
Contains(IGroup item) : bool
CopyTo(IGroup[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<IGroup>
GroupData()
IndexOf(IGroup item) : int
Insert(int index, IGroup item) : void
Remove(IGroup item) : bool
RemoveAt(int index) : void
```

## `NameAndTypeGroupComparer`

Пространство имён: `Model.GroupsData`.

Исходники: [NameAndTypeGroupComparer.cs](../../Model/GroupsData/NameAndTypeGroupComparer.cs).

Базовые типы: `IEqualityComparer<IGroup>`.

```text
Equals(IGroup x, IGroup y) : bool
GetHashCode(IGroup obj) : int
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-1](Model-1.mdpuml) | `IModelObject` |
| [Model-3](Model-3.mdpuml) | `IGroup`, `IGroupData` |
| [Model-6](Model-6.mdpuml) | `ObjType` |
| [Model-14](Model-14.mdpuml) | `IObjectsData` |
| [Model-15](Model-15.mdpuml) | `IObjectsData` |
