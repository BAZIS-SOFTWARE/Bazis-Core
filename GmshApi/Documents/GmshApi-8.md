# GmshApi-8 — Жизненный цикл и CAD API

[Диаграмма](GmshApi-8.mdpuml). [Обзор проекта](GmshApi.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `PluginName`

Пространство имён: `GmshApi`.

Исходники: [PluginName.cs](../../GmshApi/Wrapper/Enums/PluginName.cs).

Части: [GmshApi-6](GmshApi-6.mdpuml), [GmshApi-7](GmshApi-7.mdpuml), [GmshApi-8](GmshApi-8.mdpuml).

```text
SimplePartition
Skin
Smooth
SpanningTree
SphericalRaise
StreamLines
Summation
Tetrahedralize
Transform
Triangulate
VoroMetal
Warp
```
