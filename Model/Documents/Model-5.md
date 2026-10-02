# Model-5 — Геометрические и сеточные сущности

[Диаграмма](Model-5.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IElement3D`

Пространство имён: `Model.Interfaces.MeshObjects`.

Исходники: [IElement3D.cs](../../Model/Interfaces/MeshObjects/IElement3D.cs).

Базовые типы: `ISurfaceElement`, `IVolume`.

```text
CreateIntegrationPoint(double w, double ksi, double eta, double dze) : IntegralPoint
```

## `ISurfaceElement`

Пространство имён: `Model.Interfaces.MeshObjects`.

Исходники: [ISurfaceElement.cs](../../Model/Interfaces/MeshObjects/ISurfaceElement.cs).

Базовые типы: `IElement`, `ISurfaceObject<ElementSurface, Node>`.

```text
GetSurfacesNormals() : IEnumerable<Point3D>
```

## `Curve`

Пространство имён: `Model.GeometryObjects`.

Исходники: [Curve.cs](../../Model/GeometryObjects/Curve.cs).

Базовые типы: `ModelObject`, `ILineObject<GeometryPoint>`.

```text
Curve(int number, GeometryPoint[] points)
ToString() : string
```

## `GeometryPoint`

Пространство имён: `Model.GeometryObjects`.

Исходники: [GeometryPoint.cs](../../Model/GeometryObjects/GeometryPoint.cs).

Базовые типы: `ModelObject`, `IPoint`.

```text
AddLine(ILineObject<IPoint> line) : void
GeometryPoint(int number, Point3D position)
GetLines() : IEnumerable<ILineObject<IPoint>>
ToString() : string
```

## `GeometrySurface`

Пространство имён: `Model.GeometryObjects`.

Исходники: [GeometrySurface.cs](../../Model/GeometryObjects/GeometrySurface.cs).

Базовые типы: `ISurface<GeometryPoint>`.

```text
CalcCentr() : Point3D
Equals(Node other) : bool
GeometrySurface(int number, GeometryPoint[] points)
ToString() : string
```

## `SurfaceFigure`

Пространство имён: `Model.GeometryObjects`.

Исходники: [SurfaceFigure.cs](../../Model/GeometryObjects/SurfaceFigure.cs).

Базовые типы: `ModelObject`, `ISurfaceObject<GeometrySurface, GeometryPoint>`, `ISquare`.

```text
Volume : VolumeFigure [get, internal set]
SurfaceFigure(int number, GeometrySurface[] surfaces)
ToString() : string
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-1](Model-1.mdpuml) | `IElement` |
| [Model-3](Model-3.mdpuml) | `ILineObject<T>` |
| [Model-6](Model-6.mdpuml) | `VolumeFigure` |
| [Model-7](Model-7.mdpuml) | `ElementSurface`, `IntegralPoint` |
| [Model-8](Model-8.mdpuml) | `Node` |
| [Model-15](Model-15.mdpuml) | `IPoint`, `ISquare`, `ISurface<T>`, `ISurfaceObject<T,U>` |
| [Model-16](Model-16.mdpuml) | `IVolume` |
| [Model-17](Model-17.mdpuml) | `ModelObject` |
| [Geometry-4](../../PrGeometry/Documents/Geometry-4.mdpuml) | `Point3D` |
