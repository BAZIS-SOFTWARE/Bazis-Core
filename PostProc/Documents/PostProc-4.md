# PostProc-4 — Построение полей и шкала результатов

[Диаграмма](PostProc-4.mdpuml). [Обзор проекта](PostProc.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `MergeResults`

Пространство имён: `PostProc`.

Исходники: [MergeResults.cs](../../PostProc/MergeResults.cs).

```text
Merge(Dictionary<int, List<int>> interfaceNodes, string resultDescr, Result res) : void
```

## `PostProcController`

Пространство имён: `PostProc`.

Исходники: [PostProcController.cs](../../PostProc/PostProcController.cs).

```text
GridExporter : IGridExporter [get]
Intervals : int [get]
MaxValue : float [get]
MinValue : float [get]
ResultsExporter : IResultsExporter [get]
ResultsFieldsCreator : ResultsFieldsCreator [get]
ResultsMerger : MergeResults [get]
FillRange(float min, float max, int intervals, int precision) : void
GetItems() : IEnumerable<ItemRange>
GetValueColor(float resValue) : Color
```

## `LineGradientsCreator`

Пространство имён: `PostProc.ScenePresenter`.

Исходники: [LineGradientsCreator.cs](../../PostProc/ScenePresenter/LineGradientsCreator.cs).

Базовые типы: `IGradientsCreator<ILineObject<IPoint>>`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `ResultFigure`

Пространство имён: `PostProc.ScenePresenter`.

Исходники: [ResultFigure.cs](../../PostProc/ScenePresenter/ResultFigure.cs).

Базовые типы: `ICoordinated`.

```text
Color : Color [get]
NumberOfSides : int [get]
this[int index] : ResultPoint [get]
CalcCentr() : Point3D
CalcNormal() : Point3D
GetSurfaces() : IEnumerable<ResultSurface>
GetVertexes() : IEnumerable<ResultPoint>
ResultFigure(IEnumerable<Point3D> points, Color color)
```

## `ResultItem`

Пространство имён: `PostProc.ScenePresenter`.

Исходники: [ResultItem.cs](../../PostProc/ScenePresenter/ResultItem.cs).

```text
Coord : Point3D [get, set]
Value : float [get, set]
ResultItem()
ResultItem(Point3D coord, float value)
ToString() : string
```

## `ResultSurface`

Пространство имён: `PostProc.ScenePresenter`.

Исходники: [ResultSurface.cs](../../PostProc/ScenePresenter/ResultSurface.cs).

Базовые типы: `ICoordinated`.

```text
this[int index] : ResultPoint [get, set]
CalcCentr() : Point3D
CalcNormal() : Point3D
GetVertexes() : IEnumerable<ResultPoint>
ResultSurface(List<ResultPoint> points)
ReverseVertexes() : void
```
