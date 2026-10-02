# SurfaceMesher-3 — Сущности двумерной сетки

[Диаграмма](SurfaceMesher-3.mdpuml). [Обзор проекта](SurfaceMesher.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Quad2D`

Пространство имён: `SurfaceMesher`.

Исходники: [Quad2D.cs](../../SurfaceMesher/Quad2D.cs).

Базовые типы: `Element`.

```text
A : Node2D [get]
B : Node2D [get]
C : Node2D [get]
D : Node2D [get]
Index : int [get]
Quad2D(int index, Node2D a, Node2D b, Node2D c, Node2D d)
```

## `Triangle2D`

Пространство имён: `SurfaceMesher`.

Исходники: [Triangle2D.cs](../../SurfaceMesher/Triangle2D.cs).

Базовые типы: `Element`.

```text
A : Node2D [get]
B : Node2D [get]
C : Node2D [get]
Index : int [get]
GetArea() : double
GetCentroid() : Point2D
GetNodes() : IEnumerable<Node2D>
GetQuality() : double
ToString() : string
Triangle2D(int index, Node2D a, Node2D b, Node2D c)
```

## `Vector`

Пространство имён: `SurfaceMesher`.

Исходники: [Vector.cs](../../SurfaceMesher/Vector.cs).

```text
CrossProduct(Point2D a, Point2D b) : double
DotProduct(Point2D a, Point2D b) : double
GetSquaredVectorLength(Point2D vec) : double
GetVectorLength(Point2D vec) : double
```
