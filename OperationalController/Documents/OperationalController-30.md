# OperationalController-30 — Специализированные контракты контроллера

[Диаграмма](OperationalController-30.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ISurfaceObjsVertexCreator<T,U>`

Пространство имён: `OperationalController.ModelScenePresentator.Interfaces`.

Исходники: [ISurfaceObjsVertexCreator.cs](../../OperationalController/ModelScenePresentator/Interfaces/ISurfaceObjsVertexCreator.cs).

```text
Create(ref int stride, float[] vertexAtributes, IEnumerable<T> surfaces, System.Drawing.Color color) : void
```

## `IExtractor1DFrom2D`

Пространство имён: `OperationalController.ObjsCreator`.

Исходники: [IExtractor1DFrom2D.cs](../../OperationalController/Interfaces/ObjsCreator/IExtractor1DFrom2D.cs).

```text
Create(int startNumber, IElement2D[] elements) : IEnumerable<Beam>
```

## `IExtractor2DFrom3D`

Пространство имён: `OperationalController.ObjsCreator`.

Исходники: [IExtractor2DFrom3D.cs](../../OperationalController/Interfaces/ObjsCreator/IExtractor2DFrom3D.cs).

```text
Create(int StartNumber, IEnumerable<IElement3D> elements) : IEnumerable<Element2D>
FindOpenSurfaces(IEnumerable<IElement3D> elements) : IEnumerable<ElementSurface>
```

## `ISurfaceMeshCreator`

Пространство имён: `OperationalController.SurfaceMeshCreator`.

Исходники: [ISurfaceMeshCreator.cs](../../OperationalController/Interfaces/SurfaceMeshCreator/ISurfaceMeshCreator.cs).

```text
DeloneTriangulation(IEnumerable<Node> nodes) : IEnumerable<IElement2D>
```

## `IMesh2DRefiner`

Пространство имён: `OperationalController.Utilities`.

Исходники: [IMesh2DRefiner.cs](../../OperationalController/Interfaces/Utilities/IMesh2DRefiner.cs).

```text
Refine(IEnumerable<IElement2D> elements, float elementSize) : void
```
