# SurfaceMesher-1 — Сущности двумерной сетки

[Диаграмма](SurfaceMesher-1.mdpuml). [Обзор проекта](SurfaceMesher.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Contour`

Пространство имён: `SurfaceMesher`.

Исходники: [Contour.cs](../../SurfaceMesher/Contour.cs).

```text
NodesCount : int [get]
Contour()
Contour(List<List<Node2D>> boundaries)
GetContourId(Node2D node) : int
GetElementAfter(Node2D node) : Node2D
GetElementBefore(Node2D node) : Node2D
```

## `ContourElementInfo`

Пространство имён: `SurfaceMesher`.

Исходники: [ContourElementInfo.cs](../../SurfaceMesher/ContourElementInfo.cs).

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `Edge`

Пространство имён: `SurfaceMesher`.

Исходники: [Edge.cs](../../SurfaceMesher/Edge.cs).

```text
A : Node2D [get]
B : Node2D [get]
Edge(Node2D a, Node2D b)
Equals(object obj) : bool
GetHashCode() : int
GetMiddlePoint() : Point2D
GetSquaredLength() : double
IsIntersectWith(Edge other) : bool
ToString() : string
```

## `Element`

Пространство имён: `SurfaceMesher`.

Исходники: [Element.cs](../../SurfaceMesher/Element.cs).

```text
GetNodes() : IEnumerable<Node2D>
```

## `Node2D`

Пространство имён: `SurfaceMesher`.

Исходники: [Node2D.cs](../../SurfaceMesher/Node2D.cs).

```text
Index : int [get]
Position : Point2D [get]
X : double [get]
Y : double [get]
Equals(object obj) : bool
GetHashCode() : int
Node2D(int index, Point2D position)
Node2D(int index, double x, double y)
ToString() : string
```

## `Point2D`

Пространство имён: `SurfaceMesher`.

Исходники: [Point2D.cs](../../SurfaceMesher/Point2D.cs).

```text
X : double [get, private set]
Y : double [get, private set]
Add(Point2D b) : Point2D
GetLength() : double
Mult(double factor) : Point2D
Point2D(double x, double y)
Sub(Point2D b) : Point2D
ToString() : string
```
