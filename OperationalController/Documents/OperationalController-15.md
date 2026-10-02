# OperationalController-15 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-15.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ManyColorsObjsColors`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [IObjsColors.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/IObjsColors.cs).

Базовые типы: `IObjsColors`.

```text
ManyColorsObjsColors(IReadOnlyList<Color> colors)
```

## `ObjsColorsType`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [IObjsColors.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/IObjsColors.cs).

```text
Many
Single
```

## `ObjsPresenter<T>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [ObjsPresenter.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/ObjsPresenter.cs).

Базовые типы: `IObjsPresenter`.

```text
Objects : IReadOnlyList<T> [get]
CreateColors() : IEnumerable<Color>
GetEnumerator() : IEnumerator<ICoordinated>
ObjsPresenter(IReadOnlyList<T> objects, IObjsColors colors)
```

## `PointObjsPresenter<T>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [PointObjsPresenter.Coordinated.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/PointObjsPresenter.Coordinated.cs).

Базовые типы: `ObjsPresenter<T>`.

```text
PointObjsPresenter(IReadOnlyList<T> pointObjects, Color color)
PointObjsPresenter(IReadOnlyList<T> pointObjects, IObjsColors colors)
PointObjsPresenter(IReadOnlyList<T> pointObjects, IReadOnlyList<Color> colors)
```

## `PointObjsPresenter`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [PointObjsPresenter.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/PointObjsPresenter.cs).

Базовые типы: `ObjsPresenter<IModelObject>`.

```text
PointObjsPresenter(IReadOnlyList<IModelObject> pointObjects, Color color)
PointObjsPresenter(IReadOnlyList<IModelObject> pointObjects, IObjsColors colors)
PointObjsPresenter(IReadOnlyList<IModelObject> pointObjects, IReadOnlyList<Color> colors)
```

## `SingleColorObjsColors`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [IObjsColors.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/IObjsColors.cs).

Базовые типы: `IObjsColors`.

```text
SingleColorObjsColors(Color color)
```
