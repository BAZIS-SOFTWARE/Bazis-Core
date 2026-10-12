# OperationalController-27 — Специализированные контракты контроллера

[Диаграмма](OperationalController-27.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IProjectView`

Пространство имён: `OperationalController`.

Исходники: [IProjectView.cs](../../OperationalController/Interfaces/IProjectView.cs).

Части: [OperationalController-27](OperationalController-27.mdpuml), [OperationalController-28](OperationalController-28.mdpuml).

```text
HideInsideSurfaces : bool [get]
SelectedCount : int [get]
SelectedTypes : IEnumerable<ObjType> [get]
SelectionColor : Color [get]
Transparency : byte [get]
BeginViewUpdate() : IDisposable
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
Select(ObjType objType, IEnumerable<int> numbers) : void
SetColor(ISetInfo setInfo, Color color) : void
SetColor(ObjType objType, IEnumerable<int> numbers, Color color) : void
SetHideInsideSurfaces(bool hide) : void
```
