# Project-21 — Функции, материалы и ввод/вывод

[Диаграмма](Project-21.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `MovedFrame`

Пространство имён: `Project.Tasks.LocalFrames`. Исходники: [MovedFrame.cs](../../Project/Tasks/LocalFrames/MovedFrame.cs). Базовые типы: `LocalFrame`.

```text
BaseLine : IGroup [get, set]
Frame : Frame [get]
RefLine : IGroup [get, set]
StartPoint : IModelObject [get]
StopPoint : IModelObject [get]
Time : float [get, set]
Velocity : float [get, set]
CalcMotionTime() : float
CalcPosition() : void
MovedFrame()
MovedFrame(IGroup baseLine, IGroup refLine, float _velocity)
ToString() : string
```

## `MovedFrameBuilder`

Пространство имён: `Project.Tasks.LocalFrames`. Исходники: [MovedFrameBuilder.cs](../../Project/Tasks/LocalFrames/MovedFrameBuilder.cs).

```text
Build(IGroup startPoints, IGroup baseLine, IGroup refLine, float velocity) : MovedFrame
CheckStartPoints(IGroup startPoints, IGroup line) : void
SortTrajNodes(IGroup startPoints, IGroup traj) : void
```

## `StaticFrame`

Пространство имён: `Project.Tasks.LocalFrames`. Исходники: [StaticFrame.cs](../../Project/Tasks/LocalFrames/StaticFrame.cs). Базовые типы: `LocalFrame`.

```text
BaseGroup : IGroup [get, set]
Frame : Frame [get]
CalcPosition() : void
StaticFrame()
StaticFrame(IGroup baseGr)
StaticFrame(Point3D a, Point3D b, Point3D c)
ToString() : string
```

## `BeamMatData`

Пространство имён: `Project.Tasks.Materials`. Исходники: [BeamMatData.cs](../../Project/Tasks/Materials/BeamMatData.cs). Базовые типы: `MatData`.

```text
Diameter : double [get, set]
BeamMatData(float diam, MaterialDBItem material, IGroup group, float start, float stop)
ToString() : string
```

## `PlateMatData`

Пространство имён: `Project.Tasks.Materials`. Исходники: [PlateMatData.cs](../../Project/Tasks/Materials/PlateMatData.cs). Базовые типы: `MatData`.

```text
Thickness : double [get, set]
PlateMatData(MaterialDBItem material, IGroup group, float start, float stop)
ToString() : string
```

## `Interpolation`

Пространство имён: `Project.Tasks.Utilities`. Исходники: [Interpolation.cs](../../Project/Tasks/Utilities/Interpolation.cs).

```text
InterpolatedValue(float[] xAr, float[] yAr, float xn) : float
InterpolatedValueTwoPoints(float x1, float x2, float y1, float y2, float xn) : float
```
