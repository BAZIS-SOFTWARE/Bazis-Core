# Project-8 — Проект, задача и граничные условия

[Диаграмма](Project-8.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `LoadKind`

Пространство имён: `Project.Tasks`.

Исходники: [LoadData.cs](../../Project/Tasks/LoadData.cs).

```text
Давление
Сила
```

## `MatData`

Пространство имён: `Project.Tasks`.

Исходники: [MatData.cs](../../Project/Tasks/MatData.cs).

Базовые типы: `CondData`.

```text
Material : MaterialDBItem [get, set]
MatData(MaterialDBItem material, IGroup group, float start, float stop)
ToString() : string
```

## `MediaData`

Пространство имён: `Project.Tasks`.

Исходники: [MediaData.cs](../../Project/Tasks/MediaData.cs).

Базовые типы: `CondData`.

```text
MediaType : MediaType [get]
MediaData(IGroup group, float start, float stop)
ToString() : string
```

## `MediaType`

Пространство имён: `Project.Tasks`.

Исходники: [MediaData.cs](../../Project/Tasks/MediaData.cs).

```text
ConstantTemp
HeatFlux
```

## `ObjKind`

Пространство имён: `Project.Tasks`.

Исходники: [MatData.cs](../../Project/Tasks/MatData.cs).

```text
Объемы
Пластины
Стержни
```

## `ObjectData`

Пространство имён: `Project.Tasks`.

Исходники: [ObjectData.cs](../../Project/Tasks/ObjectData.cs).

Базовые типы: `Data`, `IGroupDependent`, `ITimeDependentData`.

```text
ObjectData(IGroup group, string info)
```
