# OperationalController-22 — Специализированные контракты контроллера

[Диаграмма](OperationalController-22.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IMeshQueries`

Пространство имён: `OperationalController`.

Исходники: [IMeshQueries.cs](../../OperationalController/Interfaces/IMeshQueries.cs).

Части: [OperationalController-21](OperationalController-21.mdpuml), [OperationalController-22](OperationalController-22.mdpuml).

```text
GetSectionSurfaces(Plane plane) : SurfaceFigure
GetSurfaceMeshingSettings(int surfaceNumber) : SurfaceMeshingSettings
GetVolumeMeshingSettings(int volumeNumber) : VolumeMeshingSettings
SelectE2DInPlane(float angleInGradus, int startNumber, Color color) : List<int>
SelectNodeInDirection(float angleInGradus, int numbFirst, int numbSecond, Color color) : List<int>
SelectNodeInPlane(Plane plane, Color color) : List<int>
```

## `IModelView`

Пространство имён: `OperationalController`.

Исходники: [IModelView.cs](../../OperationalController/Interfaces/IModelView.cs).

Части: [OperationalController-22](OperationalController-22.mdpuml), [OperationalController-23](OperationalController-23.mdpuml).

```text
HideInsideSurfaces : bool [get, set]
Model : IModelData [get]
SelectedCount : int [get]
SelectedTypes : IEnumerable<ObjType> [get]
SelectionColor : Color [get, set]
Transparency : byte [get, set]
BeginUpdate() : IDisposable
ClearColor() : void
ClearColor(ObjType objType, IEnumerable<int> numbers) : void
ClearSelection() : void
ClearSelection(ObjType objType) : void
Deselect(ObjType objType, IEnumerable<int> numbers) : void
GetColor(ISetInfo setInfo) : Color
GetColor(ISetInfo setInfo, int number) : Color
GetColor(ObjType objType, int number) : Color
GetSelected(ObjType objType) : IEnumerable<int>
GetSelection() : IEnumerable<IModelObject>
GetViewMode(ISetInfo setInfo) : ViewMode
GetVisible(ObjType objType, int number) : bool
HideSelected() : void
IsSelected(ObjType objType, int number) : bool
Prune() : void
Reset() : void
Select(ObjType objType, IEnumerable<int> numbers) : void
```
