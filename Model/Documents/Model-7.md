# Model-7 — Геометрические и сеточные сущности

[Диаграмма](Model-7.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Element`

Пространство имён: `Model.MeshObjects`.

Исходники: [Element.cs](../../Model/MeshObjects/Element.cs).

Базовые типы: `ModelObject`, `IElement`.

```text
Element(int number, Node[] nodes)
Element(int number, Node[] nodes, int intPoints)
GetEnumerator() : IEnumerator<Node>
ToString() : string
```

## `Element2D`

Пространство имён: `Model.MeshObjects`.

Исходники: [Element2D.cs](../../Model/MeshObjects/Element2D.cs).

Базовые типы: `Element`, `IElement2D`.

```text
Element2D(int number, Node[] nodes)
Element2D(int number, Node[] nodes, int intPoints)
```

## `Element3D`

Пространство имён: `Model.MeshObjects`.

Исходники: [Element3D.cs](../../Model/MeshObjects/Element3D.cs).

Базовые типы: `Element`, `IElement3D`.

```text
BuildGraph(IEnumerable<ElementSurface> surfaces) : Dictionary<Node, HashSet<Node>>
Element3D(int number, Node[] nodes)
GetMasterSurface(int masterSurfaceNodesNumber, IEnumerable<ElementSurface> surfaces) : ElementSurface
SortNodes(int mainSurfaceIndex, List<ElementSurface> surfaces) : List<Node>
```

## `ElementSurface`

Пространство имён: `Model.MeshObjects`.

Исходники: [ElementSurface.cs](../../Model/MeshObjects/ElementSurface.cs).

Базовые типы: `ISurface<Node>`.

```text
ElementNumber : int [get]
Owner : IElement [get]
CalcCentr() : Point3D
CreateNodesKey() : int[]
ElementSurface(IElement owner, int surfaceNumber, Node[] nodes)
Equals(ElementSurface other) : bool
GetElements() : IEnumerable<IElement>
SetNeighbour(IElement element) : bool
ToString() : string
```

## `Hexa`

Пространство имён: `Model.MeshObjects`.

Исходники: [Hexa.cs](../../Model/MeshObjects/Hexa.cs).

Базовые типы: `Element3D`.

```text
Hexa(int number, Node[] nodes)
Hexa(int number, Node[] nodes, int level)
```

## `IntegralPoint`

Пространство имён: `Model.MeshObjects`.

Исходники: [IntegralPoint.cs](../../Model/MeshObjects/IntegralPoint.cs).

Базовые типы: `IIntegralPoint`.

```text
this[string dirName] : double [get]
ToString() : string
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-1](Model-1.mdpuml) | `IElement`, `IElement2D`, `IIntegralPoint` |
| [Model-5](Model-5.mdpuml) | `IElement3D` |
| [Model-8](Model-8.mdpuml) | `Node` |
| [Model-15](Model-15.mdpuml) | `ISurface<T>` |
| [Model-17](Model-17.mdpuml) | `ModelObject` |
| [Geometry-4](../../PrGeometry/Documents/Geometry-4.mdpuml) | `Point3D` |
