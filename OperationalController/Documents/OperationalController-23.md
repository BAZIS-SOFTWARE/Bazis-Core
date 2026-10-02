# OperationalController-23 — Специализированные контракты контроллера

[Диаграмма](OperationalController-23.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IModelView`

Пространство имён: `OperationalController`.

Исходники: [IModelView.cs](../../OperationalController/Interfaces/IModelView.cs).

Части: [OperationalController-22](OperationalController-22.mdpuml), [OperationalController-23](OperationalController-23.mdpuml).

```text
SetColor(ISetInfo setInfo, Color color) : void
SetColor(ObjType objType, IEnumerable<int> numbers, Color color) : void
SetSelection(ObjType objType, IEnumerable<int> numbers) : void
SetViewMode(ISetInfo setInfo, ViewMode viewMode) : void
SetVisible(ObjType objType, IEnumerable<int> numbers, bool visible) : void
ShowAll() : void
ToggleSelection(ObjType objType, IEnumerable<int> numbers) : void
TryGetSingleSelectedType(out ObjType objType) : bool
```

## `IObjectsMerger`

Пространство имён: `OperationalController`.

Исходники: [IObjectsMerger.cs](../../OperationalController/Interfaces/IObjectsMerger.cs).

```text
Merge(List<List<int>> numbers, IObjectsSet<Node> modelObjects) : void
Merge(List<List<int>> numbers, IObjectsSet<Node> modelObjects, out IReadOnlyCollection<IElement> changedElements, out bool nodesRemoved) : void
```

## `IObjectsRenumber`

Пространство имён: `OperationalController`.

Исходники: [IObjectsRenumber.cs](../../OperationalController/Interfaces/IObjectsRenumber.cs).

```text
Renumber(IObjectsData objectsData, ObjType objType) : void
```

## `IObjectsView`

Пространство имён: `OperationalController`.

Исходники: [IObjectsView.cs](../../OperationalController/Interfaces/IObjectsView.cs).

```text
HiddenCount : int [get]
SelectedCount : int [get]
ClearObjectColor(IEnumerable<int> numbers) : bool
ClearObjectColors() : bool
ClearSelection() : bool
ClearSelectionAndHidden() : bool
Deselect(IEnumerable<int> numbers) : bool
GetSelected() : IEnumerable<int>
IsHidden(int number) : bool
IsSelected(int number) : bool
Prune(Predicate<int> exists) : bool
PruneSetStyles(IEnumerable<ISetInfo> existingSets) : bool
Select(IEnumerable<int> numbers) : bool
SetHidden(IEnumerable<int> numbers, bool hidden) : bool
SetObjectColor(IEnumerable<int> numbers, Color color) : bool
SetSelection(IEnumerable<int> numbers) : bool
SetSetColor(ISetInfo setInfo, Color color) : bool
SetViewMode(ISetInfo setInfo, ViewMode viewMode) : bool
ShowAll() : bool
ToggleSelection(IEnumerable<int> numbers) : bool
TryGetObjectColor(int number, out Color color) : bool
TryGetSetColor(ISetInfo setInfo, out Color color) : bool
TryGetViewMode(ISetInfo setInfo, out ViewMode viewMode) : bool
```

## `IObjsPresenter`

Пространство имён: `OperationalController`.

Исходники: [IObjsPresenter.cs](../../OperationalController/Interfaces/IObjsPresenter.cs).

Базовые типы: `IEnumerable<ICoordinated>`.

```text
Name : string [get, set]
PresenterType : PresenterType [get]
ViewMode : ViewMode [get, set]
Count() : int
CreateEdgeFlags(int edgesLength) : bool[]
CreateIndexes() : Tuple<int, int, int, int>
CreatePointers(int indexLength) : int[]
CreateVertexes(int vertexLength, string vertexType) : float[]
```
