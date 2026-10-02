# OperationalController-13 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-13.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `FreeNodesFinder`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [FreeNodesFinder.cs](../../OperationalController/MeshObjsUtility/FreeNodesFinder.cs).

Базовые типы: `IFreeNodesFinder`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `InterfacedNodesFinder`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [InterfacedNodesFinder.cs](../../OperationalController/MeshObjsUtility/InterfacedNodesFinder.cs).

Базовые типы: `IInterfacedNodesFinder`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `MergeObjects`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [MergeObjects.cs](../../OperationalController/MeshObjsUtility/MergeObjects.cs).

Базовые типы: `IObjectsMerger`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `NormalsCalculator`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [NormalsCalculator.cs](../../OperationalController/MeshObjsUtility/NormalsCalculator.cs).

Базовые типы: `INormalCalculator`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `NormalsChecker`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [NormalsChecker.cs](../../OperationalController/MeshObjsUtility/NormalsChecker.cs).

```text
Inverted : int [get, private set]
Log : string [get, set]
NotOthogonals : int [get, private set]
ZeroLengths : int [get, private set]
Check() : void
IdentifyByTriangles(int triangles) : ElementKind
NormalsChecker(SurfaceObjsPresenter<ElementSurface, Node> presenter)
```

## `ObjectsRenumber`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [ObjectsRenumber.cs](../../OperationalController/MeshObjsUtility/ObjectsRenumber.cs).

Базовые типы: `IObjectsRenumber`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.
