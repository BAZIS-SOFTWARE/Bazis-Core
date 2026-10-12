# OperationalController-16 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-16.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `SurfaceObjsPresenter<T>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [SurfaceObjsPresenter.Coordinated.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/SurfaceObjsPresenter.Coordinated.cs).

Базовые типы: `ObjsPresenter<T>`, `ISurfaceObjsPresenter`.

```text
SurfaceObjsPresenter(IReadOnlyList<T> surfaceObjects, Color color)
SurfaceObjsPresenter(IReadOnlyList<T> surfaceObjects, IObjsColors colors)
SurfaceObjsPresenter(IReadOnlyList<T> surfaceObjects, IReadOnlyList<Color> colors)
```

## `SurfaceObjsPresenter<T, U>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [SurfaceObjsPresenter.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/SurfaceObjsPresenter.cs).

Базовые типы: `ObjsPresenter<ISurfaceObject<T, U>>`, `ISurfaceObjsPresenter`.

```text
GetDrawnSurfaces(int index) : IEnumerable<T>
SurfaceObjsPresenter(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, Color color, bool drawInnerSurfaces= true)
SurfaceObjsPresenter(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, IObjsColors colors, bool drawInnerSurfaces= true)
SurfaceObjsPresenter(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, IReadOnlyList<Color> colors, bool drawInnerSurfaces= true)
```

## `ColorLineObjsCreator<T>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsVertexCreators`.

Исходники: [ColorLineObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/ColorLineObjsCreator.cs).

Базовые типы: `ILineObjsVertexCreator<T>`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `ColorPointObjsCreator`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsVertexCreators`.

Исходники: [ColorPointObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/ColorPointObjsCreator.cs).

Базовые типы: `IPointObjsVertexCreator`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `ColorSurfaceObjsCreator<T,U>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsVertexCreators`.

Исходники: [ColorSurfaceObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/ColorSurfaceObjsCreator.cs).

Базовые типы: `ISurfaceObjsVertexCreator<T,U>`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `CoordsLineObjsCreator<T>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsVertexCreators`.

Исходники: [CoordsLineObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/CoordsLineObjsCreator.cs).

Базовые типы: `ILineObjsVertexCreator<T>`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.
