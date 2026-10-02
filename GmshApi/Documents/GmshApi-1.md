# GmshApi-1 — Жизненный цикл и CAD API

[Диаграмма](GmshApi-1.mdpuml). [Обзор проекта](GmshApi.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `BoundaryLayerOptions`

Пространство имён: `GmshApi`.

Исходники: [BoundaryLayerOptions.cs](../../GmshApi/Wrapper/Enums/FieldTypeOptions/BoundaryLayerOptions.cs).

```text
AnisoMax
Beta
BetaLaw
CurvesList
ExcludedSurfacesList
FanPointsList
FanPointsSizesList
IntersectMetrics
NbLayers
PointsList
Quads
Ratio
Size
SizeFar
SizesList
Thickness
```

## `ElementProperties`

Пространство имён: `GmshApi`.

Исходники: [ElementProperties.cs](../../GmshApi/ElementProperties.cs).

```text
coords : double[] [field]
coords_n : int [field]
dim : int [field]
elementName : string [field]
numNodes : int [field]
numPrimaryNodes : int [field]
order : int [field]
```

## `ExtendOptions`

Пространство имён: `GmshApi`.

Исходники: [ExtendOptions.cs](../../GmshApi/Wrapper/Enums/FieldTypeOptions/ExtendOptions.cs).

```text
CurvesList
DistMax
Power
SizeMax
SurfacesList
```

## `Geo`

Пространство имён: `GmshApi`.

Исходники: [Geo.cs](../../GmshApi/Wrapper/Geo.cs).

```text
AddSurfaceLoop(int[] surfaceTags, int tag= -1) : int
AddVolume(int[] shellTags, int tag= -1) : int
Synchronize() : void
```
