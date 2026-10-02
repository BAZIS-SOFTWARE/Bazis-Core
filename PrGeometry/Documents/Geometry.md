# Geometry

Геометрические примитивы, координаты и операции построения. Не содержит событий и не зависит от GUI.

[Обзорная диаграмма](Geometry.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [Geometry-1](Geometry-1.mdpuml) | Координаты и системы отсчёта; ICoordinated, IPlane, IPoint2D, IPoint3D, IPointFunctions, IPosition2D |
| [Geometry-2](Geometry-2.mdpuml) | Геометрические построения и операции; Curve2D, Curve3D, Hull2DCreator, Point3DEqualityComparer, PointsSorter, Polygon |
| [Geometry-3](Geometry-3.mdpuml) | Координаты и системы отсчёта; IPosition3D, IPositionFunctions, Frame, Frame2D, Plane, Point2D |
| [Geometry-4](Geometry-4.mdpuml) | Координаты и системы отсчёта; Point3D, Vector |
| [Geometry-5](Geometry-5.mdpuml) | Геометрические построения и операции; RectangleBox, Segment2D, Segment3D, WarningAttribute, EnumerableExtensions |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `Geometry.Curve2D` | [Geometry-2](Geometry-2.mdpuml) | [Curve2D.cs](../../PrGeometry/Curve2D.cs) |
| `Geometry.Curve3D` | [Geometry-2](Geometry-2.mdpuml) | [Curve3D.cs](../../PrGeometry/Curve3D.cs) |
| `Geometry.Frame` | [Geometry-3](Geometry-3.mdpuml) | [Frame.cs](../../PrGeometry/Frame.cs) |
| `Geometry.Frame2D` | [Geometry-3](Geometry-3.mdpuml) | [Frame2D.cs](../../PrGeometry/Frame2D.cs) |
| `Geometry.Hull2DCreator` | [Geometry-2](Geometry-2.mdpuml) | [Hull2DCreator.cs](../../PrGeometry/Hull2DCreator.cs) |
| `Geometry.Plane` | [Geometry-3](Geometry-3.mdpuml) | [Plane.cs](../../PrGeometry/Plane.cs) |
| `Geometry.Point2D` | [Geometry-3](Geometry-3.mdpuml) | [Point2D.cs](../../PrGeometry/Point2D.cs) |
| `Geometry.Point3D` | [Geometry-4](Geometry-4.mdpuml) | [Point3D.cs](../../PrGeometry/Point3D.cs) |
| `Geometry.Point3DEqualityComparer` | [Geometry-2](Geometry-2.mdpuml) | [Point3DEqualityComparer.cs](../../PrGeometry/Point3DEqualityComparer.cs) |
| `Geometry.PointsSorter` | [Geometry-2](Geometry-2.mdpuml) | [PointsSorter.cs](../../PrGeometry/PointsSorter.cs) |
| `Geometry.Polygon` | [Geometry-2](Geometry-2.mdpuml) | [Polygon.cs](../../PrGeometry/Polygon.cs) |
| `Geometry.RectangleBox` | [Geometry-5](Geometry-5.mdpuml) | [RectangleBox.cs](../../PrGeometry/RectangleBox.cs) |
| `Geometry.Segment2D` | [Geometry-5](Geometry-5.mdpuml) | [Segment2D.cs](../../PrGeometry/Segment2D.cs) |
| `Geometry.Segment3D` | [Geometry-5](Geometry-5.mdpuml) | [Segment3D.cs](../../PrGeometry/Segment3D.cs) |
| `Geometry.Vector` | [Geometry-4](Geometry-4.mdpuml) | [Vector.cs](../../PrGeometry/Vector.cs) |
| `Geometry.Attribute.WarningAttribute` | [Geometry-5](Geometry-5.mdpuml) | [WarningAttribute.cs](../../PrGeometry/Attribute/WarningAttribute.cs) |
| `Geometry.Exteisions.EnumerableExtensions` | [Geometry-5](Geometry-5.mdpuml) | [EnumerableExtensions.cs](../../PrGeometry/Exteisions/EnumerableExtensions.cs) |
| `Geometry.Interfaces.ICoordinated` | [Geometry-1](Geometry-1.mdpuml) | [ICoordinated.cs](../../PrGeometry/Interfaces/ICoordinated.cs) |
| `Geometry.Interfaces.IPlane` | [Geometry-1](Geometry-1.mdpuml) | [IPlane.cs](../../PrGeometry/Interfaces/IPlane.cs) |
| `Geometry.Interfaces.IPoint2D` | [Geometry-1](Geometry-1.mdpuml) | [IPoint2D.cs](../../PrGeometry/Interfaces/IPoint2D.cs) |
| `Geometry.Interfaces.IPoint3D` | [Geometry-1](Geometry-1.mdpuml) | [IPoint3D.cs](../../PrGeometry/Interfaces/IPoint3D.cs) |
| `Geometry.Interfaces.IPointFunctions<T>` | [Geometry-1](Geometry-1.mdpuml) | [IPointFunctions.cs](../../PrGeometry/Interfaces/IPointFunctions.cs) |
| `Geometry.Interfaces.IPosition2D` | [Geometry-1](Geometry-1.mdpuml) | [IPosition2D.cs](../../PrGeometry/Interfaces/IPosition2D.cs) |
| `Geometry.Interfaces.IPosition3D` | [Geometry-3](Geometry-3.mdpuml) | [IPosition3D.cs](../../PrGeometry/Interfaces/IPosition3D.cs) |
| `Geometry.Interfaces.IPositionFunctions<T>` | [Geometry-3](Geometry-3.mdpuml) | [IPositionFunctions.cs](../../PrGeometry/Interfaces/IPositionFunctions.cs) |

## Источники и статус

Состав типов и сигнатуры сверены с текущими `.cs` и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.
