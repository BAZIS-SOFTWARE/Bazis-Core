# GmshApi

Обёртка нативного CAD и сеточного API Gmsh. `GmshKernel` отвечает за загрузку функций, `Gmsh` предоставляет модель, настройки, плагины и журнал. Событий для внешних потребителей CAE ядра нет.

[Обзорная диаграмма](GmshApi.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [GmshApi-1](GmshApi-1.mdpuml) | Жизненный цикл и CAD API; BoundaryLayerOptions, ElementProperties, ExtendOptions, Geo |
| [GmshApi-2](GmshApi-2.mdpuml) | Сетка и настройки; Arrangement, Field, FieldType |
| [GmshApi-3](GmshApi-3.mdpuml) | Жизненный цикл и CAD API; Gmsh, GmshKernel, InvisibleOptions, Logger, LoggerLevel, Memory |
| [GmshApi-4](GmshApi-4.mdpuml) | Жизненный цикл и CAD API; Model |
| [GmshApi-5](GmshApi-5.mdpuml) | Жизненный цикл и CAD API; Model, Occ |
| [GmshApi-6](GmshApi-6.mdpuml) | Жизненный цикл и CAD API; Occ, Option, Plugin, PluginName |
| [GmshApi-7](GmshApi-7.mdpuml) | Жизненный цикл и CAD API; PluginName |
| [GmshApi-8](GmshApi-8.mdpuml) | Жизненный цикл и CAD API; PluginName |
| [GmshApi-9](GmshApi-9.mdpuml) | Сетка и настройки; FieldType, Mesh |
| [GmshApi-10](GmshApi-10.mdpuml) | Сетка и настройки; Mesh, MeshAlgorithm2D, MeshAlgorithm3D, MeshType, QualityName |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `GmshApi.Arrangement` | [GmshApi-2](GmshApi-2.mdpuml) | [Arrangement.cs](../../GmshApi/Wrapper/Enums/Arrangement.cs) |
| `GmshApi.BoundaryLayerOptions` | [GmshApi-1](GmshApi-1.mdpuml) | [BoundaryLayerOptions.cs](../../GmshApi/Wrapper/Enums/FieldTypeOptions/BoundaryLayerOptions.cs) |
| `GmshApi.ElementProperties` | [GmshApi-1](GmshApi-1.mdpuml) | [ElementProperties.cs](../../GmshApi/ElementProperties.cs) |
| `GmshApi.ExtendOptions` | [GmshApi-1](GmshApi-1.mdpuml) | [ExtendOptions.cs](../../GmshApi/Wrapper/Enums/FieldTypeOptions/ExtendOptions.cs) |
| `GmshApi.Field` | [GmshApi-2](GmshApi-2.mdpuml) | [Field.cs](../../GmshApi/Wrapper/Field.cs) |
| `GmshApi.FieldType` | [GmshApi-2](GmshApi-2.mdpuml), [GmshApi-9](GmshApi-9.mdpuml) | [FieldType.cs](../../GmshApi/Wrapper/Enums/FieldType.cs) |
| `GmshApi.Geo` | [GmshApi-1](GmshApi-1.mdpuml) | [Geo.cs](../../GmshApi/Wrapper/Geo.cs) |
| `GmshApi.Gmsh` | [GmshApi-3](GmshApi-3.mdpuml) | [Gmsh.cs](../../GmshApi/Wrapper/Gmsh.cs) |
| `GmshApi.GmshKernel` | [GmshApi-3](GmshApi-3.mdpuml) | [GmshKernel.cs](../../GmshApi/GmshKernel.cs) |
| `GmshApi.InvisibleOptions` | [GmshApi-3](GmshApi-3.mdpuml) | [InvisibleOptions.cs](../../GmshApi/Wrapper/Enums/PluginNameOptions/InvisibleOptions.cs) |
| `GmshApi.Logger` | [GmshApi-3](GmshApi-3.mdpuml) | [Logger.cs](../../GmshApi/Wrapper/Logger.cs) |
| `GmshApi.LoggerLevel` | [GmshApi-3](GmshApi-3.mdpuml) | [LoggerLevel.cs](../../GmshApi/Wrapper/Enums/LoggerLevel.cs) |
| `GmshApi.Memory` | [GmshApi-3](GmshApi-3.mdpuml) | [Memory.cs](../../GmshApi/Wrapper/Memory.cs) |
| `GmshApi.Mesh` | [GmshApi-9](GmshApi-9.mdpuml), [GmshApi-10](GmshApi-10.mdpuml) | [Mesh.cs](../../GmshApi/Wrapper/Mesh.cs) |
| `GmshApi.MeshAlgorithm2D` | [GmshApi-10](GmshApi-10.mdpuml) | [MeshAlgorithm.cs](../../GmshApi/Wrapper/Enums/MeshAlgorithm.cs) |
| `GmshApi.MeshAlgorithm3D` | [GmshApi-10](GmshApi-10.mdpuml) | [MeshAlgorithm.cs](../../GmshApi/Wrapper/Enums/MeshAlgorithm.cs) |
| `GmshApi.MeshType` | [GmshApi-10](GmshApi-10.mdpuml) | [MeshType.cs](../../GmshApi/Wrapper/Enums/MeshType.cs) |
| `GmshApi.Model` | [GmshApi-4](GmshApi-4.mdpuml), [GmshApi-5](GmshApi-5.mdpuml) | [Model.cs](../../GmshApi/Wrapper/Model.cs) |
| `GmshApi.Occ` | [GmshApi-5](GmshApi-5.mdpuml), [GmshApi-6](GmshApi-6.mdpuml) | [Occ.cs](../../GmshApi/Wrapper/Occ.cs) |
| `GmshApi.Option` | [GmshApi-6](GmshApi-6.mdpuml) | [Option.cs](../../GmshApi/Wrapper/Option.cs) |
| `GmshApi.Plugin` | [GmshApi-6](GmshApi-6.mdpuml) | [Plugin.cs](../../GmshApi/Wrapper/Plugin.cs) |
| `GmshApi.PluginName` | [GmshApi-6](GmshApi-6.mdpuml), [GmshApi-7](GmshApi-7.mdpuml), [GmshApi-8](GmshApi-8.mdpuml) | [PluginName.cs](../../GmshApi/Wrapper/Enums/PluginName.cs) |
| `GmshApi.QualityName` | [GmshApi-10](GmshApi-10.mdpuml) | [QualityName.cs](../../GmshApi/Wrapper/Enums/QualityName.cs) |

## Источники и статус

Состав типов и сигнатуры сверены с текущими `.cs` и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.
