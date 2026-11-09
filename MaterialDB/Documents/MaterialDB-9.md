# MaterialDB-9 — Перечисления и DTO формата 1.2.0

[Диаграмма](MaterialDB-9.mdpuml). [Обзор проекта](MaterialDB.md).

Новые типы, на которых строятся классы материалов, проверка и совместимость.

## `ArgumentKind`

Пространство имён: `MaterialDB`. Исходники: [ArgumentKind.cs](../../MaterialDB/ArgumentKind.cs). В файле хранится строкой.

```text
Undefined = 0         поля нет в файле; после загрузки заменяется резолвером, не записывается
Constant = 1          одна строка
Argument = 2          одна переменная X: у свойства материала — температура, у функции — любая
TemperaturePhase = 3  «Температура» и колонка на каждую фазу «Структуры»
TemperaturePressure = 4  «Температура» и колонки давлений «P_<МПа>»
Descriptor = 5        описательная нечисловая таблица («Структура», химсостав, модели плотности и вязкости газа…)
Process = 6           процесс металлургии («Температура» × «Масс.Доли_…»)
```

## `MediumKind`, `DensityModel`, `ViscosityModel`, `MaterialModel`

Пространство имён: `MaterialDB.MaterialData`. Исходники: [MediumKind.cs](../../MaterialDB/MaterialData/MediumKind.cs), [DensityModel.cs](../../MaterialDB/MaterialData/DensityModel.cs), [ViscosityModel.cs](../../MaterialDB/MaterialData/ViscosityModel.cs), [MaterialModel.cs](../../MaterialDB/MaterialData/MaterialModel.cs). В «Общих сведениях» хранятся как `int`.

```text
MediumKind:     Solid = 0, Liquid = 1, Gas = 2
DensityModel:   Table = 0 (таблица «Плотность»), IdealGas = 1 (ρ = p·M/(R·T))
ViscosityModel: Table = 0 (таблица «Динамическая вязкость»), Sutherland = 1 (формула Сазерленда)
MaterialModel:  Isotropic = 1 (скалярные λ, E, ТКЛР), Orthotropic = 2 (тройки X/Y/Z)
```

Если записи нет: среда — `Solid`, модели плотности и вязкости — `Table`, модель материала — `Isotropic`. У газа «Модель плотности» и «Модель вязкости» — записи «Параметр»/«Значение»: номер модели в строке «Модель», параметры (M или μ0, T0, S) — в строках ниже ([MaterialDB-8](MaterialDB-8.md)); старый формат с одной числовой колонкой тоже читается. Значения задаются при создании материала через настройки ([MaterialDB-11](MaterialDB-11.mdpuml)).

## `HardnessPhaseIndices`

Пространство имён: `MaterialDB.MaterialData`. Исходники: [HardnessPhaseIndices.cs](../../MaterialDB/MaterialData/HardnessPhaseIndices.cs).

```text
AusteniteIndex : int [get]
BeiniteIndex : int [get]
FerriteIndex : int [get]
MartensiteIndex : int [get]
PerliteIndex : int [get]
HardnessPhaseIndices(int ferriteIndex, int perliteIndex, int beiniteIndex, int martensiteIndex, int austeniteIndex)
```

Индекс — номер строки фазы в «Структуре» с нуля, −1 — фазы нет. Тип задаётся при создании твёрдого тела (`SolidMaterialSettings.HardnessIndices`, [MaterialDB-11](MaterialDB-11.mdpuml)); `RemovePhase` пересчитывает индексы в записи. Типом пользуется только запись; читают индексы валидатор и PropertiesCalculator напрямую по колонкам записи «Модель твердости».

## `PressureColumnConvention`

Пространство имён: `MaterialDB.MaterialData`. Исходники: [PressureColumnConvention.cs](../../MaterialDB/MaterialData/PressureColumnConvention.cs).

```text
Prefix : string [get]
Format(double pressure) : string
GetPressures(DataTable table) : IEnumerable<double>
TryParse(string columnName, out double pressure) : bool
```

Давление в МПа, число в `InvariantCulture`: `Format(0.101325)` → `P_0.101325`.