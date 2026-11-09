# MaterialDB

Хранит материалы, категории, свойства, фазовый состав и функции. Материал — абстрактный `MaterialDBItem` с наследниками по среде (твёрдое тело, жидкость, газ); состав его свойств задаётся в конструкторе объектом настроек. Контракты загрузки и сохранения показаны вместе с данными. Событий нет.

[Обзорная диаграмма](MaterialDB.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [MaterialDB-1](MaterialDB-1.mdpuml) | Контракты и данные материалов; ICategory, IDataInformer, ILoader, IMaterialDB, IProperty, ISaver |
| [MaterialDB-2](MaterialDB-2.mdpuml) | Свойства, фазовый состав и функции; FunctionDBData, FunctionItem, CategoryEnum, ChemElement |
| [MaterialDB-3](MaterialDB-3.mdpuml) | Контракты и данные материалов; Category, CategoryData, MaterialDBData |
| [MaterialDB-4](MaterialDB-4.mdpuml) | Контракты и данные материалов; абстрактный MaterialDBItem |
| [MaterialDB-5](MaterialDB-5.mdpuml) | Свойства, фазовый состав и функции; ChemicalData, GeneralAbsentException, DataItem, PropertyItem, PropertyData |
| [MaterialDB-6](MaterialDB-6.mdpuml) | Свойства, фазовый состав и функции; PropertyEnum, Phase, PhaseData, Process |
| [MaterialDB-7](MaterialDB-7.mdpuml) | Свойства, фазовый состав и функции; ProcessData, InterpolationSearch |
| [MaterialDB-8](MaterialDB-8.mdpuml) | Материалы по средам; SolidMaterialItem, LiquidMaterialItem, GasMaterialItem, MaterialDBItemConverter |
| [MaterialDB-9](MaterialDB-9.mdpuml) | Перечисления и DTO формата 1.2.0; ArgumentKind, MediumKind, DensityModel, ViscosityModel, MaterialModel, HardnessPhaseIndices, PressureColumnConvention |
| [MaterialDB-10](MaterialDB-10.mdpuml) | Совместимость и проверка; IArgumentKindResolver, ArgumentKindResolver, IMaterialValidator, MaterialValidator, ValidationIssue, IssueSeverity |
| [MaterialDB-11](MaterialDB-11.mdpuml) | Настройки создания материалов; MaterialSettings, SolidMaterialSettings, FluidMaterialSettings, GasMaterialSettings, SutherlandCoefficients |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `MaterialDB.FunctionData.FunctionDBData` | [MaterialDB-2](MaterialDB-2.mdpuml) | [FunctionDBData.cs](../../MaterialDB/FunctionData/FunctionDBData.cs) |
| `MaterialDB.FunctionData.FunctionItem` | [MaterialDB-2](MaterialDB-2.mdpuml) | [FunctionItem.cs](../../MaterialDB/FunctionData/FunctionItem.cs) |
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
| `MaterialDB.MaterialData.MaterialDBItem` (абстрактный) | [MaterialDB-4](MaterialDB-4.mdpuml), [MaterialDB-8](MaterialDB-8.mdpuml) | [MaterialDBItem.cs](../../MaterialDB/MaterialData/MaterialDBItem.cs) |
| `MaterialDB.MaterialData.SolidMaterialItem` | [MaterialDB-8](MaterialDB-8.mdpuml) | [SolidMaterialItem.cs](../../MaterialDB/MaterialData/SolidMaterialItem.cs) |
| `MaterialDB.MaterialData.LiquidMaterialItem` | [MaterialDB-8](MaterialDB-8.mdpuml) | [LiquidMaterialItem.cs](../../MaterialDB/MaterialData/LiquidMaterialItem.cs) |
| `MaterialDB.MaterialData.GasMaterialItem` | [MaterialDB-8](MaterialDB-8.mdpuml) | [GasMaterialItem.cs](../../MaterialDB/MaterialData/GasMaterialItem.cs) |
| `MaterialDB.MaterialData.MaterialDBItemConverter` | [MaterialDB-8](MaterialDB-8.mdpuml) | [MaterialDBItemConverter.cs](../../MaterialDB/MaterialData/MaterialDBItemConverter.cs) |
| `MaterialDB.ArgumentKind` | [MaterialDB-9](MaterialDB-9.mdpuml) | [ArgumentKind.cs](../../MaterialDB/ArgumentKind.cs) |
| `MaterialDB.MaterialData.MediumKind` | [MaterialDB-9](MaterialDB-9.mdpuml) | [MediumKind.cs](../../MaterialDB/MaterialData/MediumKind.cs) |
| `MaterialDB.MaterialData.DensityModel` | [MaterialDB-9](MaterialDB-9.mdpuml) | [DensityModel.cs](../../MaterialDB/MaterialData/DensityModel.cs) |
| `MaterialDB.MaterialData.ViscosityModel` | [MaterialDB-9](MaterialDB-9.mdpuml) | [ViscosityModel.cs](../../MaterialDB/MaterialData/ViscosityModel.cs) |
| `MaterialDB.MaterialData.MaterialModel` | [MaterialDB-9](MaterialDB-9.mdpuml) | [MaterialModel.cs](../../MaterialDB/MaterialData/MaterialModel.cs) |
| `MaterialDB.MaterialData.Settings.MaterialSettings` | [MaterialDB-11](MaterialDB-11.mdpuml) | [MaterialSettings.cs](../../MaterialDB/MaterialData/Settings/MaterialSettings.cs) |
| `MaterialDB.MaterialData.Settings.SolidMaterialSettings` | [MaterialDB-11](MaterialDB-11.mdpuml) | [SolidMaterialSettings.cs](../../MaterialDB/MaterialData/Settings/SolidMaterialSettings.cs) |
| `MaterialDB.MaterialData.Settings.FluidMaterialSettings` | [MaterialDB-11](MaterialDB-11.mdpuml) | [FluidMaterialSettings.cs](../../MaterialDB/MaterialData/Settings/FluidMaterialSettings.cs) |
| `MaterialDB.MaterialData.Settings.GasMaterialSettings` | [MaterialDB-11](MaterialDB-11.mdpuml) | [GasMaterialSettings.cs](../../MaterialDB/MaterialData/Settings/GasMaterialSettings.cs) |
| `MaterialDB.MaterialData.Settings.SutherlandCoefficients` | [MaterialDB-11](MaterialDB-11.mdpuml) | [SutherlandCoefficients.cs](../../MaterialDB/MaterialData/Settings/SutherlandCoefficients.cs) |
| `MaterialDB.MaterialData.HardnessPhaseIndices` | [MaterialDB-9](MaterialDB-9.mdpuml) | [HardnessPhaseIndices.cs](../../MaterialDB/MaterialData/HardnessPhaseIndices.cs) |
| `MaterialDB.MaterialData.PressureColumnConvention` | [MaterialDB-9](MaterialDB-9.mdpuml) | [PressureColumnConvention.cs](../../MaterialDB/MaterialData/PressureColumnConvention.cs) |
| `MaterialDB.Compatibility.IArgumentKindResolver` | [MaterialDB-10](MaterialDB-10.mdpuml) | [IArgumentKindResolver.cs](../../MaterialDB/Compatibility/IArgumentKindResolver.cs) |
| `MaterialDB.Compatibility.ArgumentKindResolver` | [MaterialDB-10](MaterialDB-10.mdpuml) | [ArgumentKindResolver.cs](../../MaterialDB/Compatibility/ArgumentKindResolver.cs) |
| `MaterialDB.Validation.IMaterialValidator` | [MaterialDB-10](MaterialDB-10.mdpuml) | [IMaterialValidator.cs](../../MaterialDB/Validation/IMaterialValidator.cs) |
| `MaterialDB.Validation.MaterialValidator` | [MaterialDB-10](MaterialDB-10.mdpuml) | [MaterialValidator.cs](../../MaterialDB/Validation/MaterialValidator.cs) |
| `MaterialDB.Validation.ValidationIssue`, `IssueSeverity` | [MaterialDB-10](MaterialDB-10.mdpuml) | [ValidationIssue.cs](../../MaterialDB/Validation/ValidationIssue.cs) |
| `MaterialDB.DataItem` | [MaterialDB-5](MaterialDB-5.mdpuml) | [DataItem.cs](../../MaterialDB/DataItem.cs) |
| `MaterialDB.MaterialData.PropertyItem` | [MaterialDB-5](MaterialDB-5.mdpuml) | [PropertyItem.cs](../../MaterialDB/MaterialData/PropertyItem.cs) |
| `MaterialDB.MaterialData.PropertyData` | [MaterialDB-5](MaterialDB-5.mdpuml) | [PropertyData.cs](../../MaterialDB/MaterialData/PropertyData.cs) |
| `MaterialDB.MaterialData.PropertyEnum` | [MaterialDB-6](MaterialDB-6.mdpuml) | [MaterialDBData.cs](../../MaterialDB/MaterialData/MaterialDBData.cs) |
| `MaterialDB.MaterialData.MetallurgicalData.Phase` | [MaterialDB-6](MaterialDB-6.mdpuml) | [Phase.cs](../../MaterialDB/MaterialData/MetallurgicalData/Phase.cs) |
| `MaterialDB.MaterialData.MetallurgicalData.PhaseData` | [MaterialDB-6](MaterialDB-6.mdpuml) | [PhaseData.cs](../../MaterialDB/MaterialData/MetallurgicalData/PhaseData.cs) |
| `MaterialDB.MaterialData.MetallurgicalData.Process<T>` | [MaterialDB-6](MaterialDB-6.mdpuml) | [Process.cs](../../MaterialDB/MaterialData/MetallurgicalData/Process.cs) |
| `MaterialDB.MaterialData.MetallurgicalData.ProcessData` | [MaterialDB-7](MaterialDB-7.mdpuml) | [ProcessData.cs](../../MaterialDB/MaterialData/MetallurgicalData/ProcessData.cs) |
| `MaterialDB.Utilities.InterpolationSearch` | [MaterialDB-7](MaterialDB-7.mdpuml) | [InterpolationSearch.cs](../../MaterialDB/Utilities/InterpolationSearch.cs) |

## Источники и статус

Состав типов и сигнатуры сверены с `.cs` версии 1.2.0 (абстрактный `MaterialDBItem`, `ArgumentKind`, категории Fluid и Gas, ортотропные свойства, проверка и резолвер) и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Целевое состояние 2.0.0 (единый `sealed DataItem`, удаление `PropertyItem`, `FunctionItem`, `IProperty`, `CalcProp` и `InterpolationSearch`) описано в [Spec/MaterialDB-Spec.md](Spec/MaterialDB-Spec.md). Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.
