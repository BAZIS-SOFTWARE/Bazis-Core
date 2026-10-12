# OperationalController-2 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-2.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `CurveMeshingSettings`

Пространство имён: `OperationalController`.

Исходники: [CurveMeshingSettings.cs](../../OperationalController/Meshing/CurveMeshingSettings.cs).

```text
Coefficient : double [get]
IsTransfinite : bool [get]
MeshType : MeshType [get]
NodesCount : int [get]
CurveMeshingSettings(bool isTransfinite, int nodesCount, MeshType meshType, double coefficient)
```

## `GradientVolumeMeshingSettings`

Пространство имён: `OperationalController`.

Исходники: [VolumeMeshingSettings.cs](../../OperationalController/Meshing/VolumeMeshingSettings.cs).

```text
CoreElementSize : double [get]
DistanceMaximum : double [get]
Power : double [get]
SurfaceElementSize : double [get]
GradientVolumeMeshingSettings(double power, double distanceMaximum, double surfaceElementSize, double coreElementSize)
```

## `MeshSettings`

Пространство имён: `OperationalController`.

Исходники: [GetMeshSettings.cs](../../OperationalController/Methods/GetMeshSettings.cs).

```text
Algorithm2D : MeshAlgorithm2D [get]
Algorithm3D : MeshAlgorithm3D [get]
MaximumSize : double [get]
MinimumSize : double [get]
SizeFactor : double [get]
MeshSettings(double minimumSize, double maximumSize, MeshAlgorithm2D algorithm2D, MeshAlgorithm3D algorithm3D, double sizeFactor)
```

## `ModelLoaderFactory`

Пространство имён: `OperationalController`.

Исходники: [ModelLoaderFactory.cs](../../OperationalController/ModelLoaderFactory.cs).

```text
Create(string extension) : IModelLoader
```

## `PresenterType`

Пространство имён: `OperationalController`.

Исходники: [IObjsPresenter.cs](../../OperationalController/Interfaces/IObjsPresenter.cs).

```text
Line
Point
Surface
```

## `SurfaceMeshingSettings`

Пространство имён: `OperationalController`.

Исходники: [SurfaceMeshingSettings.cs](../../OperationalController/Meshing/SurfaceMeshingSettings.cs).

```text
Arrangement : Arrangement [get]
BoundaryPointNumbers : IReadOnlyList<int> [get]
CornerPointNumbers : IReadOnlyList<int> [get]
EmbeddedCurveNumbers : IReadOnlyList<int> [get]
IsRecombined : bool [get]
IsTransfinite : bool [get]
SurfaceMeshingSettings(bool isTransfinite, IEnumerable<int> cornerPointNumbers, Arrangement arrangement, bool isRecombined, IEnumerable<int> embeddedCurveNumbers, IEnumerable<int> boundaryPointNumbers)
```
