# Geometry-1 — Координаты и системы отсчёта

[Диаграмма](Geometry-1.mdpuml). [Обзор проекта](Geometry.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ICoordinated`

Пространство имён: `Geometry.Interfaces`.

Исходники: [ICoordinated.cs](../../PrGeometry/Interfaces/ICoordinated.cs).

```text
NumberOfPoints : int [get]
GetCoordinates() : IEnumerable<Point3D>
```

## `IPlane`

Пространство имён: `Geometry.Interfaces`.

Исходники: [IPlane.cs](../../PrGeometry/Interfaces/IPlane.cs).

```text
Normal : Point3D [get]
Shifting : double [get, set]
PlanePosition(Plane plane) : double
PointPosition(Point3D point) : double
```

## `IPoint2D`

Пространство имён: `Geometry.Interfaces`.

Исходники: [IPoint2D.cs](../../PrGeometry/Interfaces/IPoint2D.cs).

Базовые типы: `IPosition2D`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `IPoint3D`

Пространство имён: `Geometry.Interfaces`.

Исходники: [IPoint3D.cs](../../PrGeometry/Interfaces/IPoint3D.cs).

Базовые типы: `IPosition3D`.

```text
GetDistanceToLine(Segment3D line) : double
GetPointProectionOnPlane(Plane plane) : Point3D
IsBelongTo(Plane plane) : bool
```

## `IPointFunctions<T>`

Пространство имён: `Geometry.Interfaces`.

Исходники: [IPointFunctions.cs](../../PrGeometry/Interfaces/IPointFunctions.cs).

Базовые типы: `IPositionFunctions<T>`, `IEquatable<T>`.

```text
GetSquareSumCoords() : double
```

## `IPosition2D`

Пространство имён: `Geometry.Interfaces`.

Исходники: [IPosition2D.cs](../../PrGeometry/Interfaces/IPosition2D.cs).

```text
_x : double [get, set]
_y : double [get, set]
```
