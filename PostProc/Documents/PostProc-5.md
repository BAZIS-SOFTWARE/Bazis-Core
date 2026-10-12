# PostProc-5 — Построение полей и шкала результатов

[Диаграмма](PostProc-5.mdpuml). [Обзор проекта](PostProc.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ResultsFieldsCreator`

Пространство имён: `PostProc.ScenePresenter`.

Исходники: [ResultsFieldsCreator.cs](../../PostProc/ScenePresenter/ResultsFieldsCreator.cs).

```text
ScaleFactor : int [get, set]
CreateLineObjects(Result result, ObjType objsType, string resDescr, ILineObject<Node> beamElement) : IEnumerable<ILineObject<IPoint>>
CreateSurfaceObjects(Result result, string table, string resDescr, IEnumerable<ISurfaceElement> surfaceObjects) : IEnumerable<ResultFigure>
SetScaleItems(ItemRange[] itemRanges) : void
```

## `SurfaceGradientsCreator`

Пространство имён: `PostProc.ScenePresenter`.

Исходники: [SurfaceGradientsCreator.cs](../../PostProc/ScenePresenter/SurfaceGradientsCreator.cs).

```text
CreateGradientObjects(ItemRange[] itemsRanges, List<ResultItem> resItems) : IEnumerable<ResultFigure>
TryIntervalsIntersec(ItemRange itemRange, ResultItem res0, ResultItem res1, out List<Point3D> edCoords) : bool
```

## `SwapValues`

Пространство имён: `PostProc.ScenePresenter`.

Исходники: [SwapValues.cs](../../PostProc/ScenePresenter/SwapValues.cs).

```text
Sort<T, V>(T p1, T p2, V p1V, V p2V, out V[] ndRes, out T[] ndCoords) : void where V : IComparable<V>
```

## `FieldCreator`

Пространство имён: `Results.ScenePresenter`.

Исходники: [FieldCreator.cs](../../PostProc/ScenePresenter/FieldCreator.cs).

```text
CreateFieldObjects(IResult result, string objsType, string resDescr) : IEnumerable<ModelObject>
SetFieldCreator(IResultFieldCreator resultFieldCreator) : void
```

## `IsoLevelsCreator`

Пространство имён: `Results.ScenePresenter`.

Исходники: [IsoLevelFieldsCreator.cs](../../PostProc/ScenePresenter/IsoLevelFieldsCreator.cs).

```text
CreateInSidePoligons(IResult results, string resultType, ISurfaceElement[] elements, List<float[]> values, Color[] colors) : Dictionary<Color, Dictionary<int, List<Point3D>>>
FindCrossElemCoords(float value, ISurfaceElement element, float[] nodesValues) : List<Point3D>
IsoLevelsCreator()
```

## `ResultPoint`

Пространство имён: `Results.ScenePresenter`.

Исходники: [ResultPoint.cs](../../PostProc/ScenePresenter/ResultPoint.cs).

Базовые типы: `ICoordinated`, `IPosition3D`.

```text
Normal : Point3D [get, set]
Position : Point3D [get, set]
CalcCentr() : Point3D
ResultPoint(Point3D point)
ToString() : string
```
