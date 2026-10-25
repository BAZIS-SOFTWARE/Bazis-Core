# Geometry-5 — Геометрические построения и операции

[Диаграмма](Geometry-5.mdpuml). [Обзор проекта](Geometry.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `RectangleBox`

Пространство имён: `Geometry`.

Исходники: [RectangleBox.cs](../../PrGeometry/RectangleBox.cs).

```text
Bottom : int [get, set]
Left : int [get, set]
Right : int [get, set]
Top : int [get, set]
GetPoints() : Point2D[]
IsInnerOther(RectangleBox other) : bool
IsIntersectWithOther(RectangleBox other) : bool
IsPointInside(Point2D point) : bool
IsPointsInside(Point2D[] points) : bool
RectangleBox(IEnumerable<Point2D> points)
RectangleBox(int left, int right, int bottom, int top)
```

## `Segment2D`

Пространство имён: `Geometry`.

Исходники: [Segment2D.cs](../../PrGeometry/Segment2D.cs).

```text
P1 : Point2D [get]
P2 : Point2D [get]
GetIntersection(Segment2D other) : Point2D
GetLength() : float
GetOrientationSign(Point2D p0) : int
GetPointOnLine(float subLineLength) : Point2D
HasIntersectedPoint(Segment2D other) : bool
IsPointBelongLine(Point2D point) : bool
IsPointBelongSegment(Point2D point, double delta= 0.0001) : bool
Segment2D(Point2D p1, Point2D p2)
ToString() : string
```

## `Segment3D`

Пространство имён: `Geometry`.

Исходники: [Segment3D.cs](../../PrGeometry/Segment3D.cs).

```text
P0 : Point3D [get]
P1 : Point3D [get]
GetDirectVector() : Point3D
GetLength() : double
GetPlaneIntersection(ref bool state, Plane plane) : Point3D
GetPointOnLine(double subLineLength) : Point3D
Segment3D(Point3D p0, Point3D p1)
```

## `WarningAttribute`

Пространство имён: `Geometry.Attribute`.

Исходники: [WarningAttribute.cs](../../PrGeometry/Attribute/WarningAttribute.cs).

Базовые типы: `System.Attribute`.

```text
Message : string [get]
WarningAttribute(string message)
```

## `EnumerableExtensions`

Пространство имён: `Geometry.Exteisions`.

Исходники: [EnumerableExtensions.cs](../../PrGeometry/Exteisions/EnumerableExtensions.cs).

```text
Bigrams<T>(this IEnumerable<T> items, bool returFirstObj) : IEnumerable<Tuple<T, T>>
```
