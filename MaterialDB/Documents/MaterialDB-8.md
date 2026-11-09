# MaterialDB-8 — Материалы по средам

[Диаграмма](MaterialDB-8.mdpuml). [Обзор проекта](MaterialDB.md).

Наследники абстрактного [`MaterialDBItem`](MaterialDB-4.md) и конвертер, который выбирает класс при загрузке. Состав свойств задаётся при создании через объекты настроек ([MaterialDB-11](MaterialDB-11.mdpuml)); методы записи настроек закрыты. Состав категорий по средам:

| Состав | Solid | Liquid | Gas |
| --- | --- | --- | --- |
| «Общие сведения», тепловые свойства | да | да | да |
| Механические свойства, металлургия | да | нет | нет |
| «Гидродинамические свойства» (`CategoryEnum.Fluid`) | нет | да | нет |
| «Газодинамические свойства» (`CategoryEnum.Gas`) | нет | нет | да |

Пространство имён всех типов: `MaterialDB.MaterialData`.

## `SolidMaterialItem`

Исходники: [SolidMaterialItem.cs](../../MaterialDB/MaterialData/SolidMaterialItem.cs). Базовый тип: `MaterialDBItem`.

```text
Medium : MediumKind [get; Solid]
SolidMaterialItem(string name)
SolidMaterialItem(string name, SolidMaterialSettings settings)
SolidMaterialItem() [internal]
AddCrystallization(double ts, double tl) : void
AddHardeningModel(int mode) : void
AddHardnessModel(int mode, HardnessPhaseIndices? indices = null) : void
AddMetallurgicalProps() : void
CheckPhaseData() : bool
CheckProps() : bool [override]
AddProps() : void [protected, override]
```

Конструктор записывает «Структуру» из `settings.Phases` (если задана) и вызывает `AddProps()`: тепловые свойства, затем «Предел текучести», «Предел прочности», «Коэффициент упрочнения», «Модуль Юнга» и «ТКЛР» (X/Y/Z у ортотропного). При фазах таблицы строятся с колонкой на каждую фазу. `CheckProps()` проверяет тепловые и механические свойства. Конструктор только с именем создаёт изотропный материал без металлургии.

`AddCrystallization`, `AddHardeningModel`, `AddHardnessModel`, `AddMetallurgicalProps` состав таблиц не меняют и остаются публичными.

«Модель твердости» хранится в «Общих сведениях». Без индексов — одна колонка с режимом (0 — выключена, 1 — включена). С индексами — одна строка с колонками `Модель твердости`, `FerriteIndex`, `PerliteIndex`, `BeiniteIndex`, `MartensiteIndex`, `AusteniteIndex`; индекс — номер строки фазы в «Структуре» с нуля, −1 — фазы нет.

## `LiquidMaterialItem`

Исходники: [LiquidMaterialItem.cs](../../MaterialDB/MaterialData/LiquidMaterialItem.cs). Базовый тип: `MaterialDBItem`.

```text
Medium : MediumKind [get; Liquid]
LiquidMaterialItem(string name)
LiquidMaterialItem(string name, FluidMaterialSettings settings)
LiquidMaterialItem() [internal]
CheckProps() : bool [override]
AddProps() : void [protected, override]
```

Конструктор записывает «Опорное давление» (по умолчанию 1 МПа) и вызывает `AddProps()`: тепловые свойства и «Гидродинамические свойства / Динамическая вязкость» (`Argument`). `CheckProps()` проверяет тепловые свойства и вязкость.

## `GasMaterialItem`

Исходники: [GasMaterialItem.cs](../../MaterialDB/MaterialData/GasMaterialItem.cs). Базовый тип: `MaterialDBItem`.

```text
Medium : MediumKind [get; Gas]
GasMaterialItem(string name)
GasMaterialItem(string name, GasMaterialSettings settings)
GasMaterialItem() [internal]
CheckProps() : bool [override]
AddProps() : void [protected, override]
```

Конструктор записывает «Модель плотности», «Модель вязкости», «Опорное давление» (по умолчанию 1 МПа), а если заданы — «Молярную массу» и «Коэффициенты Сазерленда», затем вызывает `AddProps()`: тепловые свойства (без «Плотности» при `IdealGas`) и «Газодинамические свойства» (с «Динамической вязкостью» `TemperaturePressure` только при `ViscosityModel.Table`). Если выбрана модель `IdealGas` без молярной массы или `Sutherland` без коэффициентов, конструктор бросает `ArgumentException`. `CheckProps()` проверяет тепловые свойства и вязкость: таблицу или коэффициенты Сазерленда. Конструктор только с именем создаёт газ с табличными плотностью и вязкостью.

Единицы: давление — МПа, молярная масса — кг/моль, μ0 — МПа·с, T0 и S — К.

## `MaterialDBItemConverter`

Исходники: [MaterialDBItemConverter.cs](../../MaterialDB/MaterialData/MaterialDBItemConverter.cs). Базовый тип: `JsonConverter`.

```text
CanConvert(Type objectType) : bool
ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) : object?
WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) : void
```

Читает `Name` и `CategoryData` и по записи «Общие сведения / Агрегатное состояние» создаёт нужный класс: 0 — `SolidMaterialItem`, 1 — `LiquidMaterialItem`, 2 — `GasMaterialItem`. Нет записи — `SolidMaterialItem` (формат старых баз не меняется), неизвестное значение — `JsonSerializationException`. После создания вызывает `ArgumentKindResolver` ([MaterialDB-10](MaterialDB-10.mdpuml)). Пишет те же два поля без `$type`.