# SurfaceMesher

Сущности и алгоритмы двумерной триангуляции, фильтрация и экспорт сетки. Исключённые из компиляции варианты `DeloneTriangulation.cs` и `DeloneTriangulation_m.cs` не включены в актуальный состав типов. Событий нет.

[Обзорная диаграмма](SurfaceMesher.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [SurfaceMesher-1](SurfaceMesher-1.mdpuml) | Сущности двумерной сетки; Contour, ContourElementInfo, Edge, Element, Node2D, Point2D |
| [SurfaceMesher-2](SurfaceMesher-2.mdpuml) | Триангуляция и экспорт; DeloneTriangulation_m2, IEnumerableExtensions, QuadCandidate, SaveToBPF, SaveToSTL, TriangleFilter |
| [SurfaceMesher-3](SurfaceMesher-3.mdpuml) | Сущности двумерной сетки; Quad2D, Triangle2D, Vector |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `SurfaceMesher.Contour` | [SurfaceMesher-1](SurfaceMesher-1.mdpuml) | [Contour.cs](../../SurfaceMesher/Contour.cs) |
| `SurfaceMesher.ContourElementInfo` | [SurfaceMesher-1](SurfaceMesher-1.mdpuml) | [ContourElementInfo.cs](../../SurfaceMesher/ContourElementInfo.cs) |
| `SurfaceMesher.DeloneTriangulation_m2` | [SurfaceMesher-2](SurfaceMesher-2.mdpuml) | [DeloneTriangulation_m2.cs](../../SurfaceMesher/DeloneTriangulation_m2.cs) |
| `SurfaceMesher.Edge` | [SurfaceMesher-1](SurfaceMesher-1.mdpuml) | [Edge.cs](../../SurfaceMesher/Edge.cs) |
| `SurfaceMesher.Element` | [SurfaceMesher-1](SurfaceMesher-1.mdpuml) | [Element.cs](../../SurfaceMesher/Element.cs) |
| `SurfaceMesher.IEnumerableExtensions` | [SurfaceMesher-2](SurfaceMesher-2.mdpuml) | [IEnumerableExtensions.cs](../../SurfaceMesher/IEnumerableExtensions.cs) |
| `SurfaceMesher.Node2D` | [SurfaceMesher-1](SurfaceMesher-1.mdpuml) | [Node2D.cs](../../SurfaceMesher/Node2D.cs) |
| `SurfaceMesher.Point2D` | [SurfaceMesher-1](SurfaceMesher-1.mdpuml) | [Point2D.cs](../../SurfaceMesher/Point2D.cs) |
| `SurfaceMesher.Quad2D` | [SurfaceMesher-3](SurfaceMesher-3.mdpuml) | [Quad2D.cs](../../SurfaceMesher/Quad2D.cs) |
| `SurfaceMesher.QuadCandidate` | [SurfaceMesher-2](SurfaceMesher-2.mdpuml) | [DeloneTriangulation_m2.cs](../../SurfaceMesher/DeloneTriangulation_m2.cs) |
| `SurfaceMesher.SaveToBPF` | [SurfaceMesher-2](SurfaceMesher-2.mdpuml) | [SaveToBPF.cs](../../SurfaceMesher/SaveToBPF.cs) |
| `SurfaceMesher.SaveToSTL` | [SurfaceMesher-2](SurfaceMesher-2.mdpuml) | [SaveToSTL.cs](../../SurfaceMesher/SaveToSTL.cs) |
| `SurfaceMesher.Triangle2D` | [SurfaceMesher-3](SurfaceMesher-3.mdpuml) | [Triangle2D.cs](../../SurfaceMesher/Triangle2D.cs) |
| `SurfaceMesher.TriangleFilter` | [SurfaceMesher-2](SurfaceMesher-2.mdpuml) | [TriangleFilter.cs](../../SurfaceMesher/TriangleFilter.cs) |
| `SurfaceMesher.Vector` | [SurfaceMesher-3](SurfaceMesher-3.mdpuml) | [Vector.cs](../../SurfaceMesher/Vector.cs) |

## Источники и статус

Состав типов и сигнатуры сверены с текущими `.cs` и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.
