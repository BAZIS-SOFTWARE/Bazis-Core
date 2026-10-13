# Geometry-4 — Координаты и системы отсчёта

[Диаграмма](Geometry-4.mdpuml). [Обзор проекта](Geometry.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Point3D`

Пространство имён: `Geometry`.

Исходники: [Point3D.cs](../../PrGeometry/Point3D.cs).

Базовые типы: `IPoint3D`, `IPointFunctions<Point3D>`.

```text
Equals(Point3D other) : bool
Equals(object obj) : bool
GetHashCode() : int
Point3D()
Point3D(float x, float y, float z)
ToString() : string
```

## `Vector`

Пространство имён: `Geometry`.

Исходники: [Vector.cs](../../PrGeometry/Vector.cs).

```text
CheckCollinearityVectors(Point3D vector1, Point3D vector2) : bool
CrossProd(Point3D v1, Point3D v2) : Point3D
DotProd(Point3D v1, Point3D v2) : float
GetCosAngleVectors(Point3D n1, Point3D n2) : float
GetVectorLength(Point2D p) : double
GetVectorLength(Point3D p) : float
GetVectorNorm(Point3D p) : Point3D
GetVectorRotation(Point3D vec, Point3D axis, float angle) : Point3D
GetVectorsSumm(Point3D v1, Point3D v2) : Point3D
IsVectorsCoDirectional(List<Point3D> temp) : bool
```
