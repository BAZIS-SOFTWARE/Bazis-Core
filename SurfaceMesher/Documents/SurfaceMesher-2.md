# SurfaceMesher-2 — Триангуляция и экспорт

[Диаграмма](SurfaceMesher-2.mdpuml). [Обзор проекта](SurfaceMesher.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `DeloneTriangulation_m2`

Пространство имён: `SurfaceMesher`.

Исходники: [DeloneTriangulation_m2.cs](../../SurfaceMesher/DeloneTriangulation_m2.cs).

```text
BuildTriangulation(List<Node2D> sourceNodes, IEnumerable<Edge> constraints, List<List<Node2D>> boundaries) : void
GetQuadsTriangles(double qualityThreshold= defaultQuadQualityThreshold) : IEnumerable<Element>
GetTriangles() : IEnumerable<Triangle2D>
IsNodeInsideContour(Node2D node) : bool
RefineMesh(int iterations= 1) : IEnumerable<Node2D>
```

## `IEnumerableExtensions`

Пространство имён: `SurfaceMesher`.

Исходники: [IEnumerableExtensions.cs](../../SurfaceMesher/IEnumerableExtensions.cs).

```text
Bigrams<T>(this IEnumerable<T> source, bool isRing) : IEnumerable<Tuple<T, T>>
```

## `QuadCandidate`

Пространство имён: `SurfaceMesher`.

Исходники: [DeloneTriangulation_m2.cs](../../SurfaceMesher/DeloneTriangulation_m2.cs).

```text
A : int [get]
B : int [get]
C : int [get]
D : int [get]
Quality : double [get]
TriangleOne : int [get]
TriangleTwo : int [get]
QuadCandidate(double quality, int triangleOne, int triangleTwo, int a, int b, int c, int d)
```

## `SaveToBPF`

Пространство имён: `SurfaceMesher`.

Исходники: [SaveToBPF.cs](../../SurfaceMesher/SaveToBPF.cs).

```text
Save(string filepath, IEnumerable<Triangle2D> triangles) : void
```

## `SaveToSTL`

Пространство имён: `SurfaceMesher`.

Исходники: [SaveToSTL.cs](../../SurfaceMesher/SaveToSTL.cs).

```text
Save(string filepath, IEnumerable<Triangle2D> triangles) : void
```

## `TriangleFilter`

Пространство имён: `SurfaceMesher`.

Исходники: [TriangleFilter.cs](../../SurfaceMesher/TriangleFilter.cs).

```text
GetInflictedTriangles(List<Node2D> layerPreviousNodes, List<Triangle2D> trianglesPreviousLayer, List<List<Node2D>> layerNextContour, List<Node2D> layerNextNodes) : IEnumerable<Triangle2D>
GetInflictedTriangles(List<Triangle2D> triangles, List<Node2D> outerContour, List<Node2D>[] innerContours, bool specialSelection= true) : IEnumerable<Triangle2D>
TriangleFilter()
```
