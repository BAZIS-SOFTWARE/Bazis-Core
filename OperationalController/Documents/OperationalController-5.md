# OperationalController-5 — Представление и сообщения об изменениях

[Диаграмма](OperationalController-5.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `MeshChange`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

Базовые типы: `ChangeMessage`.

```text
Action : ChangeAction [get]
DataKind : MeshDataKind [get]
EntityId : int [get]
ObjectType : ObjType [get]
MeshChange(MeshDataKind dataKind, ChangeAction action, ObjType objectType, int entityId)
```

## `MeshDataKind`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

```text
Group
Set
```

## `ModelView`

Пространство имён: `OperationalController`.

Исходники: [ModelView.cs](../../OperationalController/ModelView/ModelView.cs).

Базовые типы: `IModelView`.

```text
HasOpenUpdate : bool [get]
EndUpdate() : void
ModelView(IModelData model)
```

## `ModelViewChange`

Пространство имён: `OperationalController`.

Исходники: [ModelViewChange.cs](../../OperationalController/ModelView/ModelViewChange.cs).

```text
InsideSurfaces
None
ObjectColor
Selection
SelectionColor
SetColor
Transparency
ViewMode
Visibility
```

## `ModelViewChangedEventArgs`

Пространство имён: `OperationalController`.

Исходники: [ModelViewChangedEventArgs.cs](../../OperationalController/ModelView/ModelViewChangedEventArgs.cs).

Базовые типы: `EventArgs`.

```text
Count : int [get]
GetChangedSets() : IEnumerable<ISetInfo>
GetChanges(ISetInfo setInfo) : ModelViewChange
HasAny(ISetInfo setInfo, ModelViewChange changes) : bool
ModelViewChangedEventArgs(IReadOnlyDictionary<ISetInfo, ModelViewChange> sets)
```

## `ObjectsView`

Пространство имён: `OperationalController`.

Исходники: [ObjectsView.cs](../../OperationalController/ModelView/ObjectsView.cs).

Базовые типы: `IObjectsView`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.
