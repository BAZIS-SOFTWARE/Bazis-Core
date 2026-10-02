# GmshApi-5 — Жизненный цикл и CAD API

[Диаграмма](GmshApi-5.mdpuml). [Обзор проекта](GmshApi.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Model`

Пространство имён: `GmshApi`.

Исходники: [Model.cs](../../GmshApi/Wrapper/Model.cs).

Части: [GmshApi-4](GmshApi-4.mdpuml), [GmshApi-5](GmshApi-5.mdpuml).

```text
IsInside(int dim, int tag, double[] coord, bool parametric= false) : int
List() : string[]
Model()
Remove() : void
RemoveAttribute(string name) : void
RemoveEntities(int[] dimTags, bool recursive= false) : void
RemoveEntityName(string name) : void
ReparametrizeOnSurface(int dim, int tag, double[] parametricCoord, int surfaceTag, int which= 0) : double[]
SetAttribute(string name, string[] values) : void
SetCoordinates(int tag, double x, double y, double z) : void
SetCurrent(string name) : void
SetEntityName(int dim, int tag, string name) : void
SetFileName(string fileName) : void
```

## `Occ`

Пространство имён: `GmshApi`.

Исходники: [Occ.cs](../../GmshApi/Wrapper/Occ.cs).

Части: [GmshApi-5](GmshApi-5.mdpuml), [GmshApi-6](GmshApi-6.mdpuml).

```text
AddBSplineSurface(int[] pointTags, int numPointsU, int tag= -1, int degreeU= 3, int degreeV= 3, double[] weights= null, double[] knotsU= null, double[] knotsV= null, int[] multiplicitiesU= null, int[] multiplicitiesV= null, int[] wireTags= null, bool wire3D= false) : int
AddBezier(int[] pointTags, int tag= -1) : int
AddBox(double x, double y, double z, double dx, double dy, double dz, int tag= -1) : int
AddChamfer(int[] volumeTags, int[] curveTags, int[] surfaceTags, double[] distances, bool removeVolume= true) : int[]
AddCircle(double x, double y, double z, double r, int tag= -1, double angle1= 0, double angle2= 2 * Math.PI, double[] zAxis= null, double[] xAxis= null) : int
AddCircleArc(int startTag, int centerTag, int endTag, int tag= -1) : int
AddCurveLoop(int[] curveTags, int tag= -1) : int
AddLine(int startTag, int endTag, int tag= -1) : int
AddPipe(int[] dimTags, int wireTag, string trihedron= "") : int[]
AddPlaneSurface(int[] wireTags, int tag= -1) : int
AddPoint(double x, double y, double z, double meshSize= 0, int tag= -1) : int
AddRectangle(double x, double y, double z, double dx, double dy, int tag= -1, double roundedRadius= 0) : int
AddSpline(int[] pointTags, int tag= -1, double[] tangents= null) : int
AddSurfaceFilling(int wireTag, int tag= -1, int[] pointTags= null, int degree= 3, int numPointsOnCurves= 15, int numIter= 2, bool anisotropic= false, double tol2d= 0.00001, double tol3d= 0.0001, double tolAng= 0.01, double tolCurv= 0.1, int maxDegree= 8, int maxSegments= 9) : int
AddThruSections(int[] wireTags, int tag= -1, bool makeSolid= true, bool makeRuled= false, int maxDegree= -1, string continuity= "", string parametrization= "", bool smoothing= false) : int[]
AddTrimmedSurface(int surfaceTag, int[] wiretTags= null, bool wire3D= false, int tag= -1) : int
AddWedge(double x, double y, double z, double dx, double dy, double dz, int tag= -1, double ltx= 0, double[] zAxis= null) : int
AddWire(int[] curveTags, int tag= -1, bool checkClosed= false) : int
Fragment(int[] objectDimTags, int[] toolDimTags, int tag= -1, bool removeObject= true, bool removeTool= true) : Tuple<int[],int[][]>
Fuse(int[] objectDimTags, int[] toolDimTags, int tag= -1, bool removeObject= true, bool removeTool= true) : Tuple<int[], int[][]>
GetCenterOfMass(int dim, int tag) : Tuple<double, double, double>
GetCurveLoops(int surfaceTag) : Tuple<int[], int[][]>
GetEntities(int dim= -1) : int[]
GetMass(int dim, int tag) : double
```
