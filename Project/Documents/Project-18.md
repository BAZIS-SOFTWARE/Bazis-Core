# Project-18 — Функции, материалы и ввод/вывод

[Диаграмма](Project-18.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `CIL`

Пространство имён: `Project.Tasks.Functions.FrameFunctions`.

Исходники: [CIL.cs](../../Project/Tasks/Functions/FrameFunctions/CIL.cs).

Базовые типы: `FrameFunction`.

```text
BottomDiam : double [get, set]
Length : double [get, set]
UpperDiam : double [get, set]
CIL()
CalcValue() : double
IsPointInside(Point3D elementCenter, out double r, out double r0, out double r1, out double r2) : bool
```

## `CillindricalBuilder`

Для `CIL` максимальный шаг движения вычисляется по `UpperDiam / (3 × velocity)`.

Пространство имён: `Project.Tasks.Functions.FrameFunctions`.

Исходники: [CillindricalBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/CillindricalBuilder.cs).

```text
CillindricalBuilder()
SetBottomDiam(string diam) : CillindricalBuilder
SetLength(string length) : CillindricalBuilder
SetUpperDiam(string diam) : CillindricalBuilder
```

## `CustomFrameFunction`

Пространство имён: `Project.Tasks.Functions.FrameFunctions`.

Исходники: [CustomFrameFunction.cs](../../Project/Tasks/Functions/FrameFunctions/CustomFrameFunction.cs).

Базовые типы: `FrameFunction`.

```text
ExecutedFile : string [get]
MotionLength : double [get, set]
CalcValue() : double
CreateEngine(string file) : void
CustomFrameFunction()
```

`MotionLength` задаётся явно для пользовательской функции с движущейся системой координат и сохраняется как параметр функции.

## `DEL`

Для `DEL` максимальный шаг движения вычисляется по `(FrontPart + BackPart) / (3 × velocity)`.

Пространство имён: `Project.Tasks.Functions.FrameFunctions`.

Исходники: [DEL.cs](../../Project/Tasks/Functions/FrameFunctions/DEL.cs).

Базовые типы: `FrameFunction`.

```text
BackPart : float [get, set]
Depth : float [get, set]
FrontPart : float [get, set]
Width : float [get, set]
CalcValue() : double
DEL()
IsPointInside(Point3D elementCenter) : bool
MaxAndMinSemiaxis() : void
```

## `EllipsBuilder`

Пространство имён: `Project.Tasks.Functions.FrameFunctions`.

Исходники: [EllipsBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/EllipsBuilder.cs).

```text
EllipsBuilder()
SetBackPart(float backPart) : EllipsBuilder
SetDepth(float depth) : EllipsBuilder
SetFrontPart(float frontPart) : EllipsBuilder
SetWidth(float width) : EllipsBuilder
```

## `FrameFunctionBuilder`

Пространство имён: `Project.Tasks.Functions.FrameFunctions`.

Исходники: [FrameFunctionBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/FrameFunctionBuilder.cs).

```text
FrameFunctionBuilder(string[] heatSourceAr)
```
