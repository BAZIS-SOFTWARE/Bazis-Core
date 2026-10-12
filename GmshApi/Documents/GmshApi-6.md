# GmshApi-6 — Жизненный цикл и CAD API

[Диаграмма](GmshApi-6.mdpuml). [Обзор проекта](GmshApi.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Occ`

Пространство имён: `GmshApi`.

Исходники: [Occ.cs](../../GmshApi/Wrapper/Occ.cs).

Части: [GmshApi-5](GmshApi-5.mdpuml), [GmshApi-6](GmshApi-6.mdpuml).

```text
GetSurfaceLoops(int volumeTag) : Tuple<int[], int[][]>
ImportShapes(string fileName, bool highestDimOnly= true, string format= "") : int[]
Intersect(int[] objectDimTags, int[] toolDimTags, int tag= -1, bool removeObject= true, bool removeTool= true) : Tuple<int[], int[][]>
Remove(int[] dimTags, bool recursive= false) : void
RemoveAllDuplicates() : void
Synchronize() : void
```

## `Option`

Пространство имён: `GmshApi`.

Исходники: [Option.cs](../../GmshApi/Wrapper/Option.cs).

```text
GetNumber(string name) : double
GetString(string name) : string
SetNumber(string name, double value) : void
SetString(string name, string value) : void
```

## `Plugin`

Пространство имён: `GmshApi`.

Исходники: [Plugin.cs](../../GmshApi/Wrapper/Plugin.cs).

```text
Run(PluginName name) : int
SetNumber(PluginName name, string option, double value) : void
SetString(PluginName name, string option, string value) : void
```

## `PluginName`

Пространство имён: `GmshApi`.

Исходники: [PluginName.cs](../../GmshApi/Wrapper/Enums/PluginName.cs).

Части: [GmshApi-6](GmshApi-6.mdpuml), [GmshApi-7](GmshApi-7.mdpuml), [GmshApi-8](GmshApi-8.mdpuml).

```text
AnalyseMeshQuality
Annotate
BoundaryAngles
Bubbles
Crack
Curl
CurvedBndDist
CutBox
CutGrid
CutMesh
CutParametric
CutPlane
CutSphere
DiscretizationError
Distance
Divergence
Eigenvalues
Eigenvectors
ExtractEdges
ExtractElements
FieldFromAmplitudePhase
GaussPoints
Gradient
HarmonicToTime
```
