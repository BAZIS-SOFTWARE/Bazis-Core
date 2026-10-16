# Project-19 — Функции, материалы и ввод/вывод

[Диаграмма](Project-19.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `SPH`

Для `SPH` максимальный шаг движения вычисляется по `Width / (3 × velocity)`.

Пространство имён: `Project.Tasks.Functions.FrameFunctions`.

Исходники: [SPH.cs](../../Project/Tasks/Functions/FrameFunctions/SPH.cs).

Базовые типы: `FrameFunction`.

```text
Width : double [get, set]
CalcValue() : double
IsPointInside(Point3D elementCenter, out double rad, out double halfBead) : bool
SPH()
```

## `SphereBuilder`

Пространство имён: `Project.Tasks.Functions.FrameFunctions`.

Исходники: [SphereBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/SphereBuilder.cs).

```text
SetWidth(string width) : SphereBuilder
SphereBuilder()
```

## `FSWPin`

Пространство имён: `Project.Tasks.Functions.Welding`.

Исходники: [FSWPin.cs](../../Project/Tasks/Functions/FrameFunctions/FSWPin.cs).

Базовые типы: `CillindricalFunction`.

```text
RotSpeed : float [get, set]
CalcValue(Point3D elementCenter) : float
ToString() : string
```

## `FSWPinBuilder`

Пространство имён: `Project.Tasks.Functions.Welding`.

Исходники: [FSWPinBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/FSWPinBuilder.cs).

```text
FSWPinBuilder()
SetBottomDiam(string diam) : FSWPinBuilder
SetFrame(Frame frame) : FSWPinBuilder
SetLength(string length) : FSWPinBuilder
SetRotSpeed(string rotSpeed) : FSWPinBuilder
SetUpperDiam(string diam) : FSWPinBuilder
SetYieldFunc(string yieldFunc) : FSWPinBuilder
```

## `FSWShoulder`

Пространство имён: `Project.Tasks.Functions.Welding`.

Исходники: [FSWShoulder.cs](../../Project/Tasks/Functions/FrameFunctions/FSWShoulder.cs).

Базовые типы: `CillindricalFunction`.

```text
AxisForce : float [get, set]
RotSpeed : float [get, set]
CalcValue(Point3D elementCenter) : float
ToString() : string
```

## `FSWShoulderBuilder`

Пространство имён: `Project.Tasks.Functions.Welding`.

Исходники: [FSWShoulderBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/FSWShoulderBuilder.cs).

```text
FSWShoulderBuilder()
SetAxisForce(string axisForce) : FSWShoulderBuilder
SetBottomDiam(string diam) : FSWShoulderBuilder
SetFrame(Frame frame) : FSWShoulderBuilder
SetFricModuleFunc(string fricModuleFunc) : FSWShoulderBuilder
SetLength(string length) : FSWShoulderBuilder
SetRotSpeed(string rotSpeed) : FSWShoulderBuilder
SetUpperDiam(string diam) : FSWShoulderBuilder
```
