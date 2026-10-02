# OperationalController-1 — Представление и сообщения об изменениях

[Диаграмма](OperationalController-1.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ChangeAction`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

```text
Created
Deleted
Updated
```

## `ChangeMessage`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `ChangedSets`

Пространство имён: `OperationalController`.

Исходники: [ChangedSets.cs](../../OperationalController/ModelView/ChangedSets.cs).

```text
IsEmpty : bool [get]
Add(ISetInfo setInfo, ModelViewChange change) : void
Clear() : void
CreateEventArgs() : ModelViewChangedEventArgs
```

## `CondChange`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

Базовые типы: `ChangeMessage`.

```text
Action : ChangeAction [get]
DataKind : DataKind [get]
Id : int [get]
CondChange(DataKind dataKind, ChangeAction action, int id)
```

## `GeoChange`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

Базовые типы: `ChangeMessage`.

```text
Counts : IReadOnlyDictionary<GeometryEntityKind, int> [get]
GeoChange(IReadOnlyDictionary<GeometryEntityKind, int> counts)
```

## `GeometryEntityKind`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

```text
Curve
Point
Surface
Volume
```
