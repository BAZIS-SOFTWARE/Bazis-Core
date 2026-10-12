# Geometry-3 — Координаты и системы отсчёта

[Диаграмма](Geometry-3.mdpuml). [Обзор проекта](Geometry.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IPosition3D`

Пространство имён: `Geometry.Interfaces`.

Исходники: [IPosition3D.cs](../../PrGeometry/Interfaces/IPosition3D.cs).

```text
_x : float [get]
_y : float [get]
_z : float [get]
```

## `IPositionFunctions<T>`

Пространство имён: `Geometry.Interfaces`.

Исходники: [IPositionFunctions.cs](../../PrGeometry/Interfaces/IPositionFunctions.cs).

```text
Div(float divide) : T
Mult(float multiplier) : T
Sub(T SubstractValue) : T
Sum(T AddedValue) : T
```

## `Frame`

Пространство имён: `Geometry`.

Исходники: [Frame.cs](../../PrGeometry/Frame.cs).

```text
Centre : Point3D [get, private set]
Dir_X : Point3D [get, private set]
Dir_Y : Point3D [get, private set]
Dir_Z : Point3D [get, private set]
InverseTransitionMatrix : Matrix<float> [get, private set]
DetermineInverseMatrix() : void
Frame()
Frame(Point3D p0, Point3D p1, Point3D p2)
GetCoordsInFrame(Point3D point) : Point3D
Rotate(Point3D axis, float angle) : void
Shift(Point3D shifting) : void
ToString() : string
```

## `Frame2D`

Пространство имён: `Geometry`.

Исходники: [Frame2D.cs](../../PrGeometry/Frame2D.cs).

```text
Frame2D(Point3D p0, Point3D p1, Point3D p2)
MapToLocalPlaneCoordinates(Point3D point) : Point2D
UnmapToGlobalCoordinates(Point2D point) : Point3D
```

## `Plane`

Пространство имён: `Geometry`.

Исходники: [Plane.cs](../../PrGeometry/Plane.cs).

Базовые типы: `IPlane`.

```text
Plane(Point3D normal, float shifting)
Plane(Point3D p0, Point3D normal)
Plane(Point3D p0, Point3D p1, Point3D p2)
```

## `Point2D`

Пространство имён: `Geometry`.

Исходники: [Point2D.cs](../../PrGeometry/Point2D.cs).

Базовые типы: `IPoint2D`, `IPointFunctions<Point2D>`.

```text
Equals(Point2D other) : bool
GetHashCode() : int
Point2D()
Point2D(double x, double y)
ToString() : string
```
