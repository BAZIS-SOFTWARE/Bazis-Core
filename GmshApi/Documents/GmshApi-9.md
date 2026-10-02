# GmshApi-9 — Сетка и настройки

[Диаграмма](GmshApi-9.mdpuml). [Обзор проекта](GmshApi.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `FieldType`

Пространство имён: `GmshApi`.

Исходники: [FieldType.cs](../../GmshApi/Wrapper/Enums/FieldType.cs).

Части: [GmshApi-2](GmshApi-2.mdpuml), [GmshApi-9](GmshApi-9.mdpuml).

```text
PostView
Restrict
Structured
Threshold
```

## `Mesh`

Пространство имён: `GmshApi`.

Исходники: [Mesh.cs](../../GmshApi/Wrapper/Mesh.cs).

Части: [GmshApi-9](GmshApi-9.mdpuml), [GmshApi-10](GmshApi-10.mdpuml).

```text
Field : Field [get, private set]
AddElementsByType(int tag, int elementType, IntPtr[] elementTags, IntPtr[] nodeTags) : void
AddNodes(int dim, int tag, IntPtr[] nodeTags, double[] coord, double[] parametricCoord= null) : void
ClassifySurfaces(double angle, bool boundary= true, bool forReparametrization= false, double curveAngle= Math.PI, bool exportDiscrete= true) : void
Clear(int[] dimTags= null) : void
CreateGeometry(int[] dimTags= null) : void
Embed(int dim, int[] tags, int inDim, int inTag) : void
Generate(int dim) : void
GetElement(IntPtr elementTag) : Tuple<int, IntPtr[], int, int>
GetElementProperties(int elementType) : ElementProperties
GetElementQualities(IntPtr[] elementTags, QualityName qualityName= QualityName.minSICN, int task= 0, int numTasks= 1) : double[]
GetElementTypes(int dim= -1, int tag= -1) : int[]
GetElements(int dim= -1, int tag= -1) : Tuple<int[], IntPtr[][], IntPtr[][]>
GetElementsByCoordinates(double x, double y, double z, int dim= -1, bool strict= false) : IntPtr[]
GetEmbedded(int dim, int tag) : int[]
GetNodes(int dim= -1, int tag= -1, bool includeBoundary= false, bool returnParametricCoord= true) : Tuple<IntPtr[], double[], double[]>
GetNodesByElementType(int elementType, int tag= -1, bool returnParametricCoord= true) : Tuple<IntPtr[], double[], double[]>
GetSizes(int[] dimTags) : double[]
ImportStl() : void
Mesh()
ReclassifyNodes() : void
Recombine() : void
Refine() : void
RemoveConstraints(int[] dimTags= null) : void
```
