# OperationalController-21 — Специализированные контракты контроллера

[Диаграмма](OperationalController-21.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IMeshCommands`

Пространство имён: `OperationalController`.

Исходники: [IMeshCommands.cs](../../OperationalController/Interfaces/IMeshCommands.cs).

Части: [OperationalController-20](OperationalController-20.mdpuml), [OperationalController-21](OperationalController-21.mdpuml).

```text
MergeElements(ObjType objType, string setMaster, string setSlave) : void
MergeNodes(List<List<int>> numbers) : void
MoveMesh(ObjType objType, Point3D vector) : void
RecombineMesh() : void
RefineMesh() : void
RenumberElements() : void
RotateMesh(ObjType objType, Point3D axis, float angle) : void
SetCurveMeshingSettings(int curveNumber, CurveMeshingSettings settings) : void
SetMeshAlgorithm2D(MeshAlgorithm2D algorithm) : void
SetMeshAlgorithm3D(MeshAlgorithm3D algorithm) : void
SetMeshMaximumSize(double size) : void
SetMeshMinimumSize(double size) : void
SetMeshPointSize(int pointNumber, double size) : void
SetMeshSizeFactor(double factor) : void
SetSurfaceMeshingSettings(int surfaceNumber, SurfaceMeshingSettings settings) : void
SetVolumeMeshingSettings(int volumeNumber, VolumeMeshingSettings settings) : void
```

## `IMeshQueries`

Пространство имён: `OperationalController`.

Исходники: [IMeshQueries.cs](../../OperationalController/Interfaces/IMeshQueries.cs).

Части: [OperationalController-21](OperationalController-21.mdpuml), [OperationalController-22](OperationalController-22.mdpuml).

```text
CalcElemsNormals(int elemDim) : IEnumerable<Curve>
CalcNodeNormals() : Dictionary<int, Point3D>
ContainsModelObject(ObjType objType, int objNumber) : bool
CreateModelObjectsPresentor(ISetInfo setInfo) : IObjsPresenter
FindBoundaryEdges() : List<string>
FindCoincidentObjects(ObjType objType, float distance) : List<List<int>>
FindFreeNodes() : IEnumerable<int>
FindInterfacedNodes(List<int> nodes, int elemDim) : Dictionary<int, List<int>>
FindInterfacedNodes(int elemDim) : Dictionary<int, List<int>>
GetAllModelElements() : IEnumerable<IElement>
GetAllModelNodes() : IEnumerable<Node>
GetAllModelObjects() : IEnumerable<IModelObject>
GetAllModelSetsInfo() : IEnumerable<ISetInfo>
GetCurveMeshingSettings(int curveNumber) : CurveMeshingSettings
GetGeometryMeshNodes(int dimension, IEnumerable<int> entityNumbers, bool includeBoundary= false) : IEnumerable<Node>
GetMeshPointSize(int pointNumber) : double
GetMeshSettings() : MeshSettings
GetModelElements(int dim, string setName) : IEnumerable<IElement>
GetModelObject(ObjType objType, int objNumber) : IModelObject
GetModelObjects(ObjType objType) : IEnumerable<IModelObject>
GetModelSetInfo(ObjType objType, int objNumber) : ISetInfo
GetModelSetInfo(ObjType objType, string setName) : ISetInfo
GetModelSetsInfo(ObjType objType) : IEnumerable<ISetInfo>
GetModelSurfaceElements(int dim) : IEnumerable<ISurfaceElement>
```
