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
SetMaterialModel(MaterialModel model) : void
AddPhase(string phaseName, double massFraction) : void
RemovePhase(string phaseName) : void
CheckPhaseData() : bool
CheckProps() : bool [override]
AddProps() : void [protected, override]
```

Конструктор проверяет настройки ([MaterialDB-11](MaterialDB-11.mdpuml)) и записывает в «Общие сведения» «Структуру», «Кристаллизацию» (строки TS и TL), «Модель упрочнения» и «Модель твердости». Затем вызывает `AddProps()`: тепловые свойства, «Предел текучести», «Предел прочности», «Коэффициент упрочнения», «Модуль Юнга» и «ТКЛР» (X/Y/Z у ортотропного). Таблицы строятся как `TemperaturePhase` с колонкой на каждую фазу. В конце создаётся пустая категория «Металлургия»; фазовые переходы задаются отдельно. `ArgumentException`, если: нет фаз; TS не меньше TL; «Модель твердости» = 1 без индексов фаз. `CheckProps()` проверяет тепловые и механические свойства. Конструктор только с именем создаёт изотропный материал с одной фазой «phase», кристаллизацией 1400–1500, «Моделью упрочнения» 2 и выключенной «Моделью твердости».

`SetMaterialModel` согласованно меняет запись «Модель материала» и состав тепловых и механических свойств. При переходе к `Orthotropic` скалярные таблицы теплопроводности, модуля Юнга и ТКЛР копируются независимо для X/Y/Z; при возврате к `Isotropic` скалярная таблица копируется из X, а таблицы X/Y/Z удаляются. Остальные свойства не меняются. Клиентский редактор должен вызывать этот метод вместо прямого изменения ячейки модели и обновлять привязку таблиц после вызова. При отсутствии исходного свойства операция завершается `InvalidOperationException` до изменения материала.

Фазовый состав меняется после создания:

* `AddPhase` добавляет строку в конец «Структуры» и колонку фазы, заполненную нулями, в каждую таблицу с раскладкой `TemperaturePhase`. Если фаза уже есть, бросает `ArgumentException`; если у материала нет «Структуры» (старый файл), бросает `InvalidOperationException`. Сумму долей проверяет валидатор (V10).
* `RemovePhase` удаляет строку «Структуры», колонки фазы, процессы «Металлургии», где фаза исходная или конечная (имя вида «Нагрев F-A»). Индекс фазы в «Модели твердости» становится −1, индексы следующих фаз уменьшаются на 1. Если фазы нет, бросает `ArgumentException`; удалить последнюю фазу нельзя — `InvalidOperationException`.

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
UseIdealGasDensity(double molarMass) : void
UseSutherlandViscosity(SutherlandCoefficients coefficients) : void
UseTableDensity() : void
UseTableViscosity() : void
CheckProps() : bool [override]
AddProps() : void [protected, override]
```

Модели газа хранятся в «Общих сведениях» записями «Модель плотности» и «Модель вязкости» с колонками «Параметр» (string) и «Значение» (double), вид `Descriptor`. Первая строка — «Модель» с номером модели; параметры модели — строки ниже:

| Запись | Строка «Модель» | Строки параметров |
| --- | --- | --- |
| «Модель плотности» | 0 — Table, 1 — IdealGas | при IdealGas: «M» — молярная масса, кг/моль |
| «Модель вязкости» | 0 — Table, 1 — Sutherland | при Sutherland: «μ0» (МПа·с), «T0», «S» (К) |

Отдельных записей «Молярная масса» и «Коэффициенты Сазерленда» нет. Калькулятор свойств по строке «Модель» выбирает способ расчёта: при табличной модели берёт таблицу, иначе — параметры из строк ниже.

Конструктор записывает обе модели (параметры — только для выбранной нетабличной модели), «Опорное давление» (по умолчанию 1 МПа) и вызывает `AddProps()`: тепловые свойства (без «Плотности» при `IdealGas`) и «Газодинамические свойства» (с «Динамической вязкостью» `TemperaturePressure` только при `ViscosityModel.Table`). Если выбрана модель `IdealGas` без молярной массы или `Sutherland` без коэффициентов, конструктор бросает `ArgumentException`. `CheckProps()` проверяет тепловые свойства, строку «M» при `IdealGas` и вязкость: таблицу или строки μ0, T0, S. Конструктор только с именем создаёт газ с табличными плотностью и вязкостью.

Модели меняются после создания. Запись модели меняется на месте: строка «Модель» перезаписывается, строки ниже удаляются и при нетабличной модели добавляются заново. Таблицы свойств при переходе на нетабличную модель не удаляются:

| Метод | Запись модели | Таблица свойства |
| --- | --- | --- |
| `UseTableDensity()` | «Модель» = 0; строка «M» удаляется | «Плотность» создаётся пустой, если её нет; существующая сохраняется |
| `UseIdealGasDensity(molarMass)` | «Модель» = 1; строка «M» записывается или заменяется | «Плотность» сохраняется (валидатор предупреждает V11) |
| `UseTableViscosity()` | «Модель» = 0; строки μ0, T0, S удаляются | «Динамическая вязкость» создаётся пустой, если её нет; существующая сохраняется |
| `UseSutherlandViscosity(coefficients)` | «Модель» = 1; строки μ0, T0, S записываются или заменяются | «Динамическая вязкость» сохраняется (валидатор предупреждает V12) |

Повторный вызов безопасен. «Опорное давление» не меняется. Если записи модели нет или она в старом формате (одна числовая колонка), метод создаёт её заново в формате «Параметр»/«Значение».

Единицы: давление — МПа, молярная масса — кг/моль, μ0 — МПа·с, T0 и S — К.

## `MaterialDBItemConverter`

Исходники: [MaterialDBItemConverter.cs](../../MaterialDB/MaterialData/MaterialDBItemConverter.cs). Базовый тип: `JsonConverter`.

```text
CanConvert(Type objectType) : bool
ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) : object?
WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) : void
```

Читает `Name` и `CategoryData` и по записи «Общие сведения / Агрегатное состояние» создаёт нужный класс: 0 — `SolidMaterialItem`, 1 — `LiquidMaterialItem`, 2 — `GasMaterialItem`. Нет записи — `SolidMaterialItem` (формат старых баз не меняется), неизвестное значение — `JsonSerializationException`. После создания вызывает `ArgumentKindResolver` ([MaterialDB-10](MaterialDB-10.mdpuml)). Пишет те же два поля без `$type`.
