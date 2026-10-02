# OperationalController-19 — Специализированные контракты контроллера

[Диаграмма](OperationalController-19.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IGeometryCommands`

Пространство имён: `OperationalController`.

Исходники: [IGeometryCommands.cs](../../OperationalController/Interfaces/IGeometryCommands.cs).

```text
CreateChamfer(IEnumerable<int> curveTags, double firstLength, double secondLengthOrAngle, bool isAngle, bool reflected) : int[]
CreateLine(int startPointNumber, int endPointNumber) : int
CreatePoint(double x, double y, double z, double meshSize= 0) : int
CreatePointByVector(int basePointTag, int directionPointTag, double offset) : int
CreatePointProjectionOntoGeometry(int basePointTag, int dim, int geometryTag) : int
CreateSurface(int[] lineNumbers) : int
DeleteGeometryObject(int dim, int number, bool recursive= false) : void
InitializeGeometry(string gmshDllPath) : void
SetEmbeddedGeometryEntities(int hostDimension, int hostNumber, int embeddedDimension, IEnumerable<int> embeddedNumbers) : void
UnloadGeometry() : void
```

## `IGeometryQueries`

Пространство имён: `OperationalController`.

Исходники: [IGeometryQueries.cs](../../OperationalController/Interfaces/IGeometryQueries.cs).

```text
GetAdjacentGeometryObjects(IModelObject obj, int direction) : IEnumerable<IModelObject>
GetAdjacentGeometryObjects(int dim, int number) : Tuple<int[], int[]>
GetBoundary(int[] dimTags, bool combined= true, bool oriented= true, bool recursive= false) : int[]
GetCenterOfMass(int dim, int tag) : (double X, double Y, double Z)
GetCurveLength(int curveTag) : double
GetGeoObjPoints(int dim, int tag, double[] parametricCoord) : double[]
GetGeometryFilePath() : string
GetGeometryLastError() : string
GetModelCurve(int objNumber) : Curve
GetModelPoint(int objNumber) : GeometryPoint
GetModelSurface(int objNumber) : SurfaceFigure
GetModelVolumes() : IEnumerable<VolumeFigure>
GetParametrization(int dim, int tag, double[] coordinates) : double[]
GetParametrizationBounds(int dim, int tag) : Tuple<double[], double[]>
GetSurfaceNormal(int surfaceTag, double[] parametricCoordinates) : double[]
GetType(int dim, int tag) : string
SelectByScope(int startDim, IEnumerable<int> selectedNumbers, int targetDim) : HashSet<int>
```

## `IGroupCommands`

Пространство имён: `OperationalController`.

Исходники: [IGroupCommands.cs](../../OperationalController/Interfaces/IGroupCommands.cs).

```text
ClearGroupData() : void
ClearNotExistedGroupData() : void
CreateGroup(IEnumerable<IModelObject> objs) : void
CreateGroup(string grName, IEnumerable<IModelObject> objs) : void
CreateGroupByGeoObjs(int meshDim, int geoDim, int tag) : string
CreateGroupByMeshObjs(int dim, string setName) : string
CreateOpenSurfaceNodesGroup(string setName) : string
DeleteModelGroup(string grName) : void
```

## `IGroupQueries`

Пространство имён: `OperationalController`.

Исходники: [IGroupQueries.cs](../../OperationalController/Interfaces/IGroupQueries.cs).

```text
ContainsConditions(string grName) : bool
GetAllModelGroups() : IEnumerable<IGroup>
GetModelGroup(int grIndex) : IGroup
GetModelGroup(string grName) : IGroup
```

## `IInterfacedNodesFinder`

Пространство имён: `OperationalController`.

Исходники: [IInterfacedNodesFinder.cs](../../OperationalController/Interfaces/IInterfacedNodesFinder.cs).

```text
Find(IEnumerable<IElement> elements) : Dictionary<int, List<int>>
Find(List<int> nodes, IEnumerable<IElement> elements) : Dictionary<int, List<int>>
```
