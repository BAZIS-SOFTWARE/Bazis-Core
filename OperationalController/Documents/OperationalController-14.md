# OperationalController-14 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-14.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `SelectionHelper`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [SelectionHelper.cs](../../OperationalController/MeshObjsUtility/SelectionHelper.cs).

Базовые типы: `ISelectionHelper`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `EmptyCollectionException`

Пространство имён: `OperationalController.Methods.Exceptions`.

Исходники: [EmptyCollectionException.cs](../../OperationalController/Methods/Exceptions/EmptyCollectionException.cs).

Базовые типы: `Exception`.

```text
EmptyCollectionException()
EmptyCollectionException(string message)
EmptyCollectionException(string message, Exception innerException)
```

## `ObjectsDimensionException`

Пространство имён: `OperationalController.Methods.Exceptions`.

Исходники: [ObjectsDimensionException.cs](../../OperationalController/Methods/Exceptions/ObjectsDimensionException.cs).

Базовые типы: `Exception`.

```text
ObjectsDimensionException()
ObjectsDimensionException(string message)
ObjectsDimensionException(string message, Exception innerException)
```

## `PresentersCreator`

Пространство имён: `OperationalController.ModelScenePresentator`.

Исходники: [PresentersCreator.cs](../../OperationalController/ModelScenePresentator/PresentersCreator.cs).

Базовые типы: `IPresentersCreator`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `ElementSurfaceObjsPresenter`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [ElementSurfaceObjsPresenter.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/ElementSurfaceObjsPresenter.cs).

Базовые типы: `SurfaceObjsPresenter<ElementSurface, Node>`.

```text
ElementSurfaceObjsPresenter(IReadOnlyList<ISurfaceObject<ElementSurface, Node>> elements, IReadOnlyList<Color> colors, IModelView modelView)
GetDrawnSurfaces(int index) : IEnumerable<ElementSurface>
```

## `LineObjsPresenter<T>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [LineObjsPresenter.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/LineObjsPresenter.cs).

Базовые типы: `ObjsPresenter<T>`.

```text
LineObjsPresenter(IReadOnlyList<T> lineObjects, Color color)
LineObjsPresenter(IReadOnlyList<T> lineObjects, IObjsColors colors)
LineObjsPresenter(IReadOnlyList<T> lineObjects, IReadOnlyList<Color> colors)
```
