# Project-4 — Проект, задача и граничные условия

[Диаграмма](Project-4.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ICondData`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ICondData.cs](../../Project/Interfaces/Tasks/ICondData.cs).

Базовые типы: `IData`, `IGroupDependent`, `ITimeDependentData`.

```text
Direction : Direction [get]
Function : Function [get, set]
LocalFrame : LocalFrame [get, set]
Value : double [get, set]
TrySetDirection(string v) : void
TrySetValue(string v) : void
```

## `IData`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [IData.cs](../../Project/Interfaces/Tasks/IData.cs).

```text
Kind : DataKind [get]
```

## `IFrameData`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [IFrameData.cs](../../Project/Interfaces/Tasks/IFrameData.cs).

Базовые типы: `IData`, `IGroupDependent`, `ITimeDependentData`.

```text
FrameFunction : IFrameFunction [get]
LocalFrame : ILocalFrame [get, set]
```

## `IFrameFunction`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [IFrameFunction.cs](../../Project/Interfaces/Tasks/IFrameFunction.cs).

```text
CalcValue(Point3D elementCenter) : double
GetIntersectedObjects<T>(List<T> elements) : List<T> where T : IModelObject
IsOverlappingSelf(float velocity) : bool
```

## `IGroupDependent`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [IGroupDependent.cs](../../Project/Interfaces/Tasks/IGroupDependent.cs).

```text
Group : IGroup [get, set]
```

## `ILocalFrame`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ILocalFrame.cs](../../Project/Interfaces/Tasks/ILocalFrame.cs).

```text
Frame : Frame [get, set]
Rotation : float [get, set]
Shifting : Point3D [get, set]
```
