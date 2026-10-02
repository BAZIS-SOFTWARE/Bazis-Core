# OperationalController-12 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-12.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `TranslateExtruder`

Пространство имён: `OperationalController.MeshObjsCreator.Extrude`.

Исходники: [TranslateExtruder.cs](../../OperationalController/MeshObjsCreator/Extrude/TranslateExtruder.cs).

Базовые типы: `MeshExtruder`.

```text
Extrude(IEnumerable<Element2D> curLayer, Point3D dir, int layers= 1, float layerSize= 1) : List<List<Element3D>>
ExtrudeTransfinite(IEnumerable<Element2D> curLayer, Point3D dir, int layers= 1, float layerSize= 1) : List<List<Element3D>>
```

## `CrossSection`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [CrossSection.cs](../../OperationalController/MeshObjsUtility/CrossSection.cs).

Базовые типы: `ICrossSectionMaker`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `ElementData`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [NormalsChecker.cs](../../OperationalController/MeshObjsUtility/NormalsChecker.cs).

```text
center : Point3D [field]
surfaceEdges : List<Point3D> [field]
surfaceNormals : List<Point3D> [field]
surfacePoints : List<Point3D> [field]
ElementData()
```

## `FindBoundaryEdges`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [FindBoundaryEdges.cs](../../OperationalController/MeshObjsUtility/FindBoundaryEdges.cs).

Базовые типы: `IBoundaryEdgesFinder`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `FindCoincidentObjects`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [FindCoincidentObjects.cs](../../OperationalController/MeshObjsUtility/FindCoincidentObjects.cs).

Базовые типы: `ICoincidentObjectsFinder`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `FindOpenSurfaceNodes`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [FindOpenSurfaceNodes.cs](../../OperationalController/MeshObjsUtility/FindOpenSurfaceNodes.cs).

```text
Find(IEnumerable<IElement3D> elements) : IEnumerable<int>
```
