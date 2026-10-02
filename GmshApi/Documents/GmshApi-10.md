# GmshApi-10 — Сетка и настройки

[Диаграмма](GmshApi-10.mdpuml). [Обзор проекта](GmshApi.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Mesh`

Пространство имён: `GmshApi`.

Исходники: [Mesh.cs](../../GmshApi/Wrapper/Mesh.cs).

Части: [GmshApi-9](GmshApi-9.mdpuml), [GmshApi-10](GmshApi-10.mdpuml).

```text
RemoveEmbedded(int[] dimTags, int dim= -1) : void
SetAlgorithm(int dim, int tag, MeshAlgorithm2D val= MeshAlgorithm2D.Automatic) : void
SetRecombine(int dim, int tag, double angle= 45.0) : void
SetSize(int[] dimTags, double size) : void
SetTransfiniteAutomatic(int[] dimTags, double cornerAngle= 2.35, bool recombine= true) : void
SetTransfiniteCurve(int tag, int numNodes, MeshType meshType= MeshType.Progression, double coef= 1) : void
SetTransfiniteSurface(int tag, Arrangement arrangement= Arrangement.Left, int[] cornerTags= null) : void
SetTransfiniteVolume(int tag, int[] cornerTags= null) : void
SetVisibility(IntPtr[] elementTags, int value) : void
```

## `MeshAlgorithm2D`

Пространство имён: `GmshApi`.

Исходники: [MeshAlgorithm.cs](../../GmshApi/Wrapper/Enums/MeshAlgorithm.cs).

```text
Automatic
BAMG
Delaunay
FrontalDelaunay
FrontalDelaunayQuads
InitialMeshOnly
MeshAdapt
PackingOfParallelograms
QuasiStructuredQuad
```

## `MeshAlgorithm3D`

Пространство имён: `GmshApi`.

Исходники: [MeshAlgorithm.cs](../../GmshApi/Wrapper/Enums/MeshAlgorithm.cs).

```text
Delaunay
Frontal
HXT
InitialMeshOnly
MMG3D
RTree
```

## `MeshType`

Пространство имён: `GmshApi`.

Исходники: [MeshType.cs](../../GmshApi/Wrapper/Enums/MeshType.cs).

```text
Beta
Bump
Progression
```

## `QualityName`

Пространство имён: `GmshApi`.

Исходники: [QualityName.cs](../../GmshApi/Wrapper/Enums/QualityName.cs).

```text
angleShape
gamma
innerRadius
maxDetJac
maxEdge
minDetJac
minEdge
minIsotropy
minSICN
minSIGE
minSJ
outerRadius
```
