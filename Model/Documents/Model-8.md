# Model-8 — Геометрические и сеточные сущности

[Диаграмма](Model-8.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Node`

Пространство имён: `Model.MeshObjects`.

Исходники: [Node.cs](../../Model/MeshObjects/Node.cs).

Базовые типы: `ModelObject`, `IPoint`.

```text
this[int elemInd] : IElement [get]
AddElement(IElement element) : void
GetElements() : IEnumerable<IElement>
Node(int number, Point3D position)
RemoveElement(IElement element) : void
ToString() : string
```

## `Penta`

Пространство имён: `Model.MeshObjects`.

Исходники: [Penta.cs](../../Model/MeshObjects/Penta.cs).

Базовые типы: `Element3D`.

```text
Penta(int number, Node[] nodes)
Penta(int number, Node[] nodes, int level)
```

## `Quad`

Пространство имён: `Model.MeshObjects`.

Исходники: [Quad.cs](../../Model/MeshObjects/Quad.cs).

Базовые типы: `Element2D`.

```text
Quad(int number, Node[] nodes)
Quad(int number, Node[] nodes, int level)
```

## `Tetra`

Пространство имён: `Model.MeshObjects`.

Исходники: [Tetra.cs](../../Model/MeshObjects/Tetra.cs).

Базовые типы: `Element3D`.

```text
CreateIntegrationPoint(int index, double w, double l1, double l2, double l3, double l4) : IntegralPoint
Tetra(int number, Node[] nodes)
Tetra(int number, Node[] nodes, int level)
```

## `Tetra_c`

Пространство имён: `Model.MeshObjects`.

Исходники: [Tetra_c.cs](../../Model/MeshObjects/Tetra_c.cs).

Базовые типы: `Element3D`.

```text
Tetra_c(int number, Node[] nodes)
Tetra_c(int number, Node[] nodes, int level)
```

## `Triangle`

Пространство имён: `Model.MeshObjects`.

Исходники: [Triangle.cs](../../Model/MeshObjects/Triangle.cs).

Базовые типы: `Element2D`.

```text
CreateIntegrationPoint(double w, double l1, double l2, double l3) : IntegralPoint
Triangle(int number, Node[] nodes)
Triangle(int number, Node[] nodes, int level)
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-1](Model-1.mdpuml) | `IElement` |
| [Model-7](Model-7.mdpuml) | `Element2D`, `Element3D`, `IntegralPoint` |
| [Model-15](Model-15.mdpuml) | `IPoint` |
| [Model-17](Model-17.mdpuml) | `ModelObject` |
| [Geometry-4](../../PrGeometry/Documents/Geometry-4.mdpuml) | `Point3D` |
