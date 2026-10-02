# OperationalController-17 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-17.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `CoordsPointObjsCreator`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsVertexCreators`.

Исходники: [CoordsPointObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/CoordsPointObjsCreator.cs).

Базовые типы: `IPointObjsVertexCreator`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `CoordsSurfaceObjsCreator<T, U>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsVertexCreators`.

Исходники: [CoordsSurfaceObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/CoordsSurfaceObjsCreator.cs).

Базовые типы: `ISurfaceObjsVertexCreator<T, U>`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `EdgesSurfaceObjsCreator<T, U>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsVertexCreators`.

Исходники: [EdgesSurfaceObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/EdgesSurfaceObjsCreator.cs).

```text
Create(ref int stride, bool[] vertexAtributes, IEnumerable<T> surfaces) : void
```

## `NormalsLineObjsCreator<T>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsVertexCreators`.

Исходники: [NormalsLineObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/NormalsLineObjsCreator.cs).

Базовые типы: `ILineObjsVertexCreator<T>`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `NormalsPointObjsCreator`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsVertexCreators`.

Исходники: [NormalsPointObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/NormalsPointObjsCreator.cs).

Базовые типы: `IPointObjsVertexCreator`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `NormalsSurfaceObjsCreator<T, U>`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsVertexCreators`.

Исходники: [NormalsSurfaceObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/NormalsSurfaceObjsCreator.cs).

Базовые типы: `ISurfaceObjsVertexCreator<T, U>`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.
