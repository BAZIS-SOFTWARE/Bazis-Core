# PostProc-2 — Построение полей и шкала результатов

[Диаграмма](PostProc-2.mdpuml). [Обзор проекта](PostProc.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IPostProcController`

Пространство имён: `PostProc`.

Исходники: [IPostProcController.cs](../../PostProc/IPostProcController.cs).

```text
GridExporter : IGridExporter [get]
ResultsExporter : IResultsExporter [get]
ResultsFieldsCreator : IResultsFieldsCreator [get]
ResultsMerger : IResultsMerger [get]
```

## `IResultsMerger`

Пространство имён: `PostProc`.

Исходники: [IResultsMerger.cs](../../PostProc/IResultsMerger.cs).

```text
Merge(Dictionary<int, List<int>> interfaceNodes, string resultDescr, IEnumerable<Result> results) : void
```

## `IGradientsCreator<T>`

Пространство имён: `PostProc.ScenePresenter.Interfaces`.

Исходники: [IGradientsCreator.cs](../../PostProc/ScenePresenter/Interfaces/IGradientsCreator.cs).

```text
CreateGradientObjects(ItemRange[] itemsValues, Color[] itemsColors, List<Point3D> nodesCoords, List<float> nodesResults) : IEnumerable<T>
```

## `IResultsFieldsCreator`

Пространство имён: `PostProc.ScenePresenter.Interfaces`.

Исходники: [IResultsFieldsCreator.cs](../../PostProc/ScenePresenter/Interfaces/IResultsFieldsCreator.cs).

```text
ScaleFactor : int [get, set]
CreateSurfaceObjects(Result result, ObjType objsType, string resDescr, IEnumerable<ISurfaceElement> surfaceObjects) : IEnumerable<ResultFigure>
SetScaleItems(ItemRange[] rangesValues) : void
```

## `IResultsFieldsCreator`

Пространство имён: `ProjectInterfaces.Results`.

Исходники: [IResultsFieldsCreator.cs](../../PostProc/ScenePresenter/IResultsFieldsCreator.cs).

```text
ScaleFactor : int [get, set]
CreateSurfaceObjects(IResult result, ObjType objsType, string resDescr, IEnumerable<ISurfaceElement> surfaceObjects) : IEnumerable<IResultFigure2D>
SetScaleItems(Color[] rangesColors, ItemRange[] rangesValues) : void
```

## `ItemRange`

Пространство имён: `PostProc`.

Исходники: [ItemRange.cs](../../PostProc/ItemRange.cs).

```text
Color : Color [get, set]
Max : float [get, set]
Min : float [get, set]
ToString() : string
```
