# OperationalController-4 — Специализированные контракты контроллера

[Диаграмма](OperationalController-4.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IBoundaryEdgesFinder`

Пространство имён: `OperationalController`.

Исходники: [IBoundaryEdgesFinder.cs](../../OperationalController/Interfaces/IBoundaryEdgesFinder.cs).

```text
CreateBoundaryEdges(List<string> linesNodesб, IModelData modelData) : IEnumerable<ILineObject<Node>>
Find(IEnumerable<ISurfaceElement> surfaceElements) : List<string>
```

## `ICoincidentObjectsFinder`

Пространство имён: `OperationalController`.

Исходники: [ICoincidentObjectsFinder.cs](../../OperationalController/Interfaces/ICoincidentObjectsFinder.cs).

```text
Find<T>(List<T> modelObjects, float distance) : List<List<int>> where T : IModelObject
```

## `IConditionCommands`

Пространство имён: `OperationalController`.

Исходники: [IConditionCommands.cs](../../OperationalController/Interfaces/IConditionCommands.cs).

```text
AddTaskData(ICondData cond) : void
ClearNotExistedCondData() : void
ClearTaskData() : void
Create(DataKind kind, string info) : ICondData
DeleteCond(IGroup group) : void
DeleteCond(int index) : bool
UpdateCondition(int index, ICondData condition) : void
```

## `IConditionQueries`

Пространство имён: `OperationalController`.

Исходники: [IConditionQueries.cs](../../OperationalController/Interfaces/IConditionQueries.cs).

```text
GetAllCondData() : IEnumerable<ICondData>
GetCondData(int condIndex) : ICondData
```

## `ICrossSectionMaker`

Пространство имён: `OperationalController`.

Исходники: [ICrossSectionMaker.cs](../../OperationalController/Interfaces/ICrossSectionMaker.cs).

```text
GetSectionSurfaces(IEnumerable<IElement3D> elements, Plane plane) : SurfaceFigure
```

## `IFreeNodesFinder`

Пространство имён: `OperationalController`.

Исходники: [IFreeNodesFinder.cs](../../OperationalController/Interfaces/IFreeNodesFinder.cs).

```text
Find(IObjectsData objsData) : IEnumerable<int>
```
