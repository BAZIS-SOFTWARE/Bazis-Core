# Geometry-2 — Геометрические построения и операции

[Диаграмма](Geometry-2.mdpuml). [Обзор проекта](Geometry.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Curve2D`

Пространство имён: `Geometry`.

Исходники: [Curve2D.cs](../../PrGeometry/Curve2D.cs).

```text
Curve2D(List<Point2D> points)
IsPointBelongCurve(Point2D point, double delta= 1e-2) : bool
```

## `Curve3D`

Пространство имён: `Geometry`.

Исходники: [Curve3D.cs](../../PrGeometry/Curve3D.cs).

Базовые типы: `IList<Point3D>`.

```text
Count : int [get]
IsReadOnly : bool [get]
this[int index] : Point3D [get, set]
Add(Point3D point) : void
AddRange(IEnumerable<Point3D> points) : void
Clear() : void
Contains(Point3D item) : bool
CopyTo(Point3D[] array, int arrayIndex) : void
Curve3D()
Curve3D(List<Point3D> points)
GetEnumerator() : IEnumerator<Point3D>
IndexOf(Point3D point) : int
Insert(int index, Point3D item) : void
Remove(Point3D point) : bool
RemoveAt(int index) : void
```

## `Hull2DCreator`

Пространство имён: `Geometry`.

Исходники: [Hull2DCreator.cs](../../PrGeometry/Hull2DCreator.cs).

```text
CreateHullGraham(IEnumerable<Point2D> points) : Polygon
GetHullAndrew(IEnumerable<Point2D> points) : IEnumerable<Point2D>
GetHullGraham(IEnumerable<Point2D> points) : IEnumerable<Point2D>
TryCreateHullGraham(IEnumerable<Point2D> input, out Polygon output) : bool
```

## `Point3DEqualityComparer`

Пространство имён: `Geometry`.

Исходники: [Point3DEqualityComparer.cs](../../PrGeometry/Point3DEqualityComparer.cs).

Базовые типы: `IEqualityComparer<Point3D>`.

```text
Equals(Point3D a, Point3D b) : bool
GetHashCode(Point3D obj) : int
```

## `PointsSorter`

Пространство имён: `Geometry`.

Исходники: [PointsSorter.cs](../../PrGeometry/PointsSorter.cs).

```text
RoundSort<T>(this List<T> points) : void where T : IPosition3D
RoundSort<T>(this T[] objects) : void where T : IPosition3D
SortByDistance<T>(this List<T> points) : void where T : IPosition3D
SwapArray<T>(T[] objectArray, int x, int y) : bool
SwapList<T>(List<T> objectArray, int firstIndex, int secondIndex) : bool
```

## `Polygon`

Пространство имён: `Geometry`.

Исходники: [Polygon.cs](../../PrGeometry/Polygon.cs).

```text
Area : double [get]
GetPoints() : IEnumerable<Point2D>
IsPointInsidePolygon(Point2D point, float epsilon= 0.01f) : bool
Polygon(List<Point2D> points)
```
