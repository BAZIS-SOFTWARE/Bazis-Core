# Project-7 — Проект, задача и граничные условия

[Диаграмма](Project-7.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ClampData`

Пространство имён: `Project.Tasks`.

Исходники: [ClampData.cs](../../Project/Tasks/ClampData.cs).

Базовые типы: `CondData`.

```text
ClampKind : ClampKind [get, private set]
ClampData(IGroup group, float start, float stop)
ToString() : string
TrySetKind(string v) : void
```

## `ClampKind`

Пространство имён: `Project.Tasks`.

Исходники: [ClampData.cs](../../Project/Tasks/ClampData.cs).

```text
Жесткое
Контакт
Симметрия
Упругое
```

## `CondData`

Пространство имён: `Project.Tasks`.

Исходники: [CondData.cs](../../Project/Tasks/CondData.cs).

Базовые типы: `ICondData`.

```text
CondData(IGroup group, float startTime, float stopTime)
CondData(IGroup group, string info)
ToString() : string
```

## `Data`

Пространство имён: `Project.Tasks`.

Исходники: [Data.cs](../../Project/Tasks/Data.cs).

Базовые типы: `IData`.

```text
GetInfo : string [get]
Data(string info)
SetInfo(string info) : void
```

## `HeatData`

Пространство имён: `Project.Tasks`.

Исходники: [HeatData.cs](../../Project/Tasks/HeatData.cs).

Базовые типы: `CondData`.

```text
HeatData(IGroup group, float start, float stop)
```

## `LoadData`

Пространство имён: `Project.Tasks`.

Исходники: [LoadData.cs](../../Project/Tasks/LoadData.cs).

Базовые типы: `CondData`.

```text
LoadKind : LoadKind [get, set]
LoadData(IGroup group, float start, float stop)
ToString() : string
TrySetKind(string v) : void
```
