# MaterialDB

Хранит материалы, категории, свойства, фазовый состав и функции. Контракты загрузки и сохранения показаны вместе с данными. Событий нет.

[Обзорная диаграмма](MaterialDB.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [MaterialDB-1](MaterialDB-1.mdpuml) | Контракты и данные материалов; ICategory, IDataInformer, ILoader, IMaterialDB, IProperty, ISaver |
| [MaterialDB-2](MaterialDB-2.mdpuml) | Свойства, фазовый состав и функции; FunctionDBData, CategoryEnum, ChemElement |
| [MaterialDB-3](MaterialDB-3.mdpuml) | Контракты и данные материалов; Category, CategoryData, MaterialDBData |
| [MaterialDB-4](MaterialDB-4.mdpuml) | Контракты и данные материалов; MaterialDBItem |
| [MaterialDB-5](MaterialDB-5.mdpuml) | Свойства, фазовый состав и функции; ChemicalData, GeneralAbsentException, Property, PropertyData |
| [MaterialDB-6](MaterialDB-6.mdpuml) | Свойства, фазовый состав и функции; PropertyEnum, Phase, PhaseData, Process |
| [MaterialDB-7](MaterialDB-7.mdpuml) | Свойства, фазовый состав и функции; ProcessData, InterpolationSearch |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `MaterialDB.FunctionData.FunctionDBData` | [MaterialDB-2](MaterialDB-2.mdpuml) | [FunctionDBData.cs](../../MaterialDB/FunctionData/FunctionDBData.cs) |
| `MaterialDB.Interfaces.ICategory` | [MaterialDB-1](MaterialDB-1.mdpuml) | [ICategory.cs](../../MaterialDB/Interfaces/ICategory.cs) |
| `MaterialDB.Interfaces.IDataInformer` | [MaterialDB-1](MaterialDB-1.mdpuml) | [IDataInformer.cs](../../MaterialDB/Interfaces/IDataInformer.cs) |
| `MaterialDB.Interfaces.ILoader` | [MaterialDB-1](MaterialDB-1.mdpuml) | [ILoader.cs](../../MaterialDB/Interfaces/ILoader.cs) |
| `MaterialDB.Interfaces.IMaterialDB` | [MaterialDB-1](MaterialDB-1.mdpuml) | [IMaterialDB.cs](../../MaterialDB/Interfaces/IMaterialDB.cs) |
| `MaterialDB.Interfaces.IProperty` | [MaterialDB-1](MaterialDB-1.mdpuml) | [IProperty.cs](../../MaterialDB/Interfaces/IProperty.cs) |
| `MaterialDB.Interfaces.ISaver` | [MaterialDB-1](MaterialDB-1.mdpuml) | [ISaver.cs](../../MaterialDB/Interfaces/ISaver.cs) |
| `MaterialDB.MaterialData.Category` | [MaterialDB-3](MaterialDB-3.mdpuml) | [Category.cs](../../MaterialDB/MaterialData/Category.cs) |
| `MaterialDB.MaterialData.CategoryData` | [MaterialDB-3](MaterialDB-3.mdpuml) | [CategoryData.cs](../../MaterialDB/MaterialData/CategoryData.cs) |
| `MaterialDB.MaterialData.CategoryEnum` | [MaterialDB-2](MaterialDB-2.mdpuml) | [MaterialDBData.cs](../../MaterialDB/MaterialData/MaterialDBData.cs) |
| `MaterialDB.MaterialData.ChemElement` | [MaterialDB-2](MaterialDB-2.mdpuml) | [ChemicalData.cs](../../MaterialDB/MaterialData/ChemicalData.cs) |
| `MaterialDB.MaterialData.ChemicalData` | [MaterialDB-5](MaterialDB-5.mdpuml) | [ChemicalData.cs](../../MaterialDB/MaterialData/ChemicalData.cs) |
| `MaterialDB.MaterialData.GeneralAbsentException` | [MaterialDB-5](MaterialDB-5.mdpuml) | [GeneralAbsentException.cs](../../MaterialDB/MaterialData/GeneralAbsentException.cs) |
| `MaterialDB.MaterialData.MaterialDBData` | [MaterialDB-3](MaterialDB-3.mdpuml) | [MaterialDBData.cs](../../MaterialDB/MaterialData/MaterialDBData.cs) |
| `MaterialDB.MaterialData.MaterialDBItem` | [MaterialDB-4](MaterialDB-4.mdpuml) | [MaterialDBItem.cs](../../MaterialDB/MaterialData/MaterialDBItem.cs) |
| `MaterialDB.MaterialData.Property` | [MaterialDB-5](MaterialDB-5.mdpuml) | [Property.cs](../../MaterialDB/MaterialData/Property.cs) |
| `MaterialDB.MaterialData.PropertyData` | [MaterialDB-5](MaterialDB-5.mdpuml) | [PropertyData.cs](../../MaterialDB/MaterialData/PropertyData.cs) |
| `MaterialDB.MaterialData.PropertyEnum` | [MaterialDB-6](MaterialDB-6.mdpuml) | [MaterialDBData.cs](../../MaterialDB/MaterialData/MaterialDBData.cs) |
| `MaterialDB.MaterialData.MetallurgicalData.Phase` | [MaterialDB-6](MaterialDB-6.mdpuml) | [Phase.cs](../../MaterialDB/MaterialData/MetallurgicalData/Phase.cs) |
| `MaterialDB.MaterialData.MetallurgicalData.PhaseData` | [MaterialDB-6](MaterialDB-6.mdpuml) | [PhaseData.cs](../../MaterialDB/MaterialData/MetallurgicalData/PhaseData.cs) |
| `MaterialDB.MaterialData.MetallurgicalData.Process<T>` | [MaterialDB-6](MaterialDB-6.mdpuml) | [Process.cs](../../MaterialDB/MaterialData/MetallurgicalData/Process.cs) |
| `MaterialDB.MaterialData.MetallurgicalData.ProcessData` | [MaterialDB-7](MaterialDB-7.mdpuml) | [ProcessData.cs](../../MaterialDB/MaterialData/MetallurgicalData/ProcessData.cs) |
| `MaterialDB.Utilities.InterpolationSearch` | [MaterialDB-7](MaterialDB-7.mdpuml) | [InterpolationSearch.cs](../../MaterialDB/Utilities/InterpolationSearch.cs) |

## Источники и статус

Состав типов и сигнатуры сверены с текущими `.cs` и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.
