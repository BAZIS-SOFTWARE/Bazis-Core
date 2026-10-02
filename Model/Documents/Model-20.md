# Model-20 — Грани и вспомогательные алгоритмы

[Диаграмма](Model-20.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `DefaultSetInfoComparer`

Пространство имён: `Model.Utilities`.

Исходники: [DefaultSetInfoComparer.cs](../../Model/Utilities/DefaultSetInfoComparer.cs).

Базовые типы: `IEqualityComparer<ISetInfo>`.

```text
Equals(ISetInfo x, ISetInfo y) : bool
GetHashCode(ISetInfo obj) : int
```

## `MarkSurfaces`

Пространство имён: `Model.Utilities`.

Исходники: [MarkSurfaces.cs](../../Model/Utilities/MarkSurfaces.cs).

```text
Mark(IEnumerable<ISurfaceElement> elements) : IReadOnlyList<ElementSurface>
Mark(IEnumerable<ISurfaceElement> elements, out IReadOnlyCollection<IElement> changedElements) : IReadOnlyList<ElementSurface>
```

## `SurfaceNodesComparer`

Пространство имён: `Model.Utilities`.

Исходники: [SurfaceNodesComparer.cs](../../Model/Utilities/SurfaceNodesComparer.cs).

Базовые типы: `IEqualityComparer<int[]>`.

```text
Equals(int[] first, int[] second) : bool
GetHashCode(int[] nodeNumbers) : int
```

## `SurfacesCreator`

Пространство имён: `Model.Utilities`.

Исходники: [SurfacesCreator.cs](../../Model/Utilities/SurfacesCreator.cs).

```text
CreateSurfacesFromSetOfNodes(IElement owner, Node[] nodes) : List<ElementSurface>
```

## `SurfacesFinder`

Пространство имён: `Model.Utilities`.

Исходники: [SurfacesFinder.cs](../../Model/Utilities/SurfacesFinder.cs).

```text
FindSurfaceNodes<T>(T[] nodes) : IEnumerable<T[]> where T : IPoint
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-1](Model-1.mdpuml) | `IElement` |
| [Model-5](Model-5.mdpuml) | `ISurfaceElement` |
| [Model-7](Model-7.mdpuml) | `ElementSurface` |
| [Model-8](Model-8.mdpuml) | `Node` |
| [Model-15](Model-15.mdpuml) | `IPoint` |
| [Model-16](Model-16.mdpuml) | `ISetInfo` |
