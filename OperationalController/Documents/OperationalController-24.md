# OperationalController-24 — Специализированные контракты контроллера

[Диаграмма](OperationalController-24.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IPresentersCreator`

Пространство имён: `OperationalController`.

Исходники: [IPresentersCreator.cs](../../OperationalController/Interfaces/IPresentersCreator.cs).

```text
CreateLineObjectsPresenter<T>(IReadOnlyList<T> lineObjects, Color color) : IObjsPresenter where T : ICoordinated
CreateLineObjectsPresenter<T>(IReadOnlyList<T> lineObjects, IReadOnlyList<Color> colors) : IObjsPresenter where T : ICoordinated
CreatePointObjectsPresenter(IReadOnlyList<IModelObject> pointObjects, Color color) : IObjsPresenter
CreatePointObjectsPresenter(IReadOnlyList<IModelObject> pointObjects, IReadOnlyList<Color> colors) : IObjsPresenter
CreatePointObjectsPresenter<T>(IReadOnlyList<T> pointObjects, Color color) : IObjsPresenter where T : ICoordinated
CreateSurfaceObjectsPresenter<T, U>(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, Color color, bool drawInnerSurfaces= true) : ISurfaceObjsPresenter where T : ISurface<U> where U : IPoint
CreateSurfaceObjectsPresenter<T, U>(IReadOnlyList<ISurfaceObject<T, U>> surfaceObjects, IReadOnlyList<Color> colors, bool drawInnerSurfaces= true) : ISurfaceObjsPresenter where T : ISurface<U> where U : IPoint
CreateSurfaceObjectsPresenter<T>(IReadOnlyList<T> surfaceObjects, Color color) : ISurfaceObjsPresenter where T : ICoordinated
CreateSurfaceObjectsPresenter<T>(IReadOnlyList<T> surfaceObjects, IReadOnlyList<Color> colors) : ISurfaceObjsPresenter where T : ICoordinated
```
