# GmshApi-2 — Сетка и настройки

[Диаграмма](GmshApi-2.mdpuml). [Обзор проекта](GmshApi.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Arrangement`

Пространство имён: `GmshApi`.

Исходники: [Arrangement.cs](../../GmshApi/Wrapper/Enums/Arrangement.cs).

```text
AlternateLeft
AlternateRight
Left
Right
```

## `Field`

Пространство имён: `GmshApi`.

Исходники: [Field.cs](../../GmshApi/Wrapper/Field.cs).

```text
Add(FieldType fieldType, int tag= -1) : int
GetNumber(int tag, string option) : double
GetNumbers(int tag, string option) : double[]
GetString(int tag, string option) : string
GetType(int tag) : string
List() : int[]
Remove(int tag) : void
SetAsBackgroundMesh(int tag) : void
SetAsBoundaryLayer(int tag) : void
SetNumber(int tag, string option, double value) : void
SetNumbers(int tag, string option, double[] values) : void
SetString(int tag, string option, string value) : void
```

## `FieldType`

Пространство имён: `GmshApi`.

Исходники: [FieldType.cs](../../GmshApi/Wrapper/Enums/FieldType.cs).

Части: [GmshApi-2](GmshApi-2.mdpuml), [GmshApi-9](GmshApi-9.mdpuml).

```text
AttractorAnisoCurve
AutomaticMeshSizeField
Ball
BoundaryLayer
Box
Constant
Curvature
Cylinder
Distance
Extend
ExternalProcess
Frustum
Gradient
IntersectAniso
Laplacian
LonLat
MathEval
MathEvalAniso
MaxEigenHessian
Mean
Min
MinAniso
Octree
Param
```
