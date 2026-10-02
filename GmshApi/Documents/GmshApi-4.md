# GmshApi-4 — Жизненный цикл и CAD API

[Диаграмма](GmshApi-4.mdpuml). [Обзор проекта](GmshApi.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Model`

Пространство имён: `GmshApi`.

Исходники: [Model.cs](../../GmshApi/Wrapper/Model.cs).

Части: [GmshApi-4](GmshApi-4.mdpuml), [GmshApi-5](GmshApi-5.mdpuml).

```text
Geo : Geo [get, private set]
Mesh : Mesh [get, private set]
Occ : Occ [get, private set]
Add(string name) : void
AddDiscreteEntity(int dim, int tag= -1, int[] boundary= null) : int
GetAdjacencies(int dim, int tag) : Tuple<int[], int[]>
GetAttribute(string name) : string[]
GetAttributeNames() : string[]
GetBoundary(int[] dimTags, bool combined= true, bool oriented= true, bool recursive= false) : int[]
GetBoundingBox(int dim, int tag) : Tuple<double, double, double, double, double, double>
GetClosestPoint(int dim, int tag, double[] coord) : Tuple<double[], double[]>
GetCurrent() : string
GetCurvature(int dim, int tag, double[] parametricCoord) : double[]
GetDerivative(int dim, int tag, double[] parametricCoord) : double[]
GetDimension() : int
GetEntities(int dim= -1) : int[]
GetEntityName(int dim, int tag) : string
GetFileName() : string
GetNormal(int tag, double[] parametricCoord) : double[]
GetParametrization(int dim, int tag, double[] coord) : double[]
GetParametrizationBounds(int dim, int tag) : Tuple<double[], double[]>
GetSecondDerivative(int dim, int tag, double[] parametricCoord) : double[]
GetType(int dim, int tag) : string
GetValue(int dim, int tag, double[] parametricCoord) : double[]
```
