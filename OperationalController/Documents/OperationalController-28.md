# OperationalController-28 — Специализированные контракты контроллера

[Диаграмма](OperationalController-28.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IProjectView`

Пространство имён: `OperationalController`.

Исходники: [IProjectView.cs](../../OperationalController/Interfaces/IProjectView.cs).

Части: [OperationalController-27](OperationalController-27.mdpuml), [OperationalController-28](OperationalController-28.mdpuml).

```text
SetSelection(ObjType objType, IEnumerable<int> numbers) : void
SetSelectionColor(Color color) : void
SetTransparency(byte transparency) : void
SetViewMode(ISetInfo setInfo, ViewMode viewMode) : void
SetVisible(ObjType objType, IEnumerable<int> numbers, bool visible) : void
ShowAll() : void
ToggleSelection(ObjType objType, IEnumerable<int> numbers) : void
TryGetSingleSelectedType(out ObjType objType) : bool
```

## `ISelectionHelper`

Пространство имён: `OperationalController`.

Исходники: [ISelectionHelper.cs](../../OperationalController/Interfaces/ISelectionHelper.cs).

```text
SelectE2DInPlane(IObjectsData objectsData, float angleInGradus, int startNumber) : List<int>
SelectNodeInDirection(IObjectsData objectsData, float angleInGradus, int numbFirst, int numbSecond) : List<int>
SelectNodeInPlane(IObjectsData objectsData, Plane plane) : List<int>
```

## `ISurfaceObjsPresenter`

Пространство имён: `OperationalController`.

Исходники: [ISurfaceObjsPresenter.cs](../../OperationalController/Interfaces/ISurfaceObjsPresenter.cs).

Базовые типы: `IObjsPresenter`.

```text
CreateSeparators() : int[]
```

## `IVolumeMeshCreator`

Пространство имён: `OperationalController`.

Исходники: [IVolumeMeshCreator.cs](../../OperationalController/Interfaces/IVolumeMeshCreator.cs).

```text
Sew3Dfrom2D(List<IElement2D> firstSet, List<IElement2D> secSet) : List<IElement3D>
```

## `IGmshController`

Пространство имён: `OperationalController.GmshController`.

Исходники: [IGmshController.cs](../../OperationalController/GmshController/IGmshController.cs).

Части: [OperationalController-28](OperationalController-28.mdpuml), [OperationalController-29](OperationalController-29.mdpuml).

```text
Gmsh : Gmsh [get]
IsInitialized : bool [get]
CalcCurveLength(int curveTag) : double
CalcVolume(int volTag) : double
CreateControlPoints() : List<GeometryPoint>
CreateGeometry(double surfAngle= Math.PI / 2, double curveAngle= Math.PI) : void
CreateLines() : List<Curve>
CreateSurfaceData() : Tuple<IObjectsSet<SurfaceFigure>, VolumeData>
CreateSurfaceFigures() : List<SurfaceFigure>
CreateVolumeFigures() : List<VolumeFigure>
DeleteMeshElements(IntPtr[] elementTags) : void
GetMeshEntities(int dim, int tag, List<Node> nodes) : List<IElement>
GetMeshObjects() : (List<Node>, List<ILineObject<Node>>, List<Element2D>, List<Element3D>)
GetMeshSets<T>(List<Node> nodes) : IEnumerable<ObjectsSet<T>> where T : class, IElement
GetNodes(int dim= -1, int tag= -1, bool includeBoundary= true) : List<Node>
GetRecombineSurface(int tag) : bool
GetSize(int tag) : double
GetSurfaceNodes(int tag) : int[]
GetTransfiniteCurve(int tag) : string[]
GetTransfiniteSurface(int tag) : string[]
GetTransfiniteVolume(int tag) : string[]
Load(string dllToLoad) : void
MergeGeometry() : void
SetGradientVolume(int tag, double power, double distMax, double surfSize, double coreSize) : void
```
