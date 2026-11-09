# MaterialDB-11 — Настройки создания материалов

[Диаграмма](MaterialDB-11.mdpuml). [Обзор проекта](MaterialDB.md).

Объекты настроек передаются в конструкторы материалов ([MaterialDB-8](MaterialDB-8.mdpuml)) и определяют состав свойств: какие записи появятся в «Общих сведениях» и какие таблицы создаст `AddProps()`. Классы содержат только данные, без логики. Пространство имён всех типов: `MaterialDB.MaterialData.Settings`.

| Настройки | Конструктор |
| --- | --- |
| `SolidMaterialSettings` | `SolidMaterialItem(string name, SolidMaterialSettings settings)` |
| `FluidMaterialSettings` | `LiquidMaterialItem(string name, FluidMaterialSettings settings)` |
| `GasMaterialSettings` | `GasMaterialItem(string name, GasMaterialSettings settings)` |

Конструктор каждого материала, принимающий только имя, передаёт настройки по умолчанию (`new ...Settings()`).

## `MaterialSettings`

Исходники: [MaterialSettings.cs](../../MaterialDB/MaterialData/Settings/MaterialSettings.cs).

```text
MaterialModel : MaterialModel [get, set; по умолчанию Isotropic]
```

Записывается в «Общие сведения / Модель материала». При `Orthotropic` теплопроводность (у всех сред), модуль Юнга и ТКЛР (у твёрдого тела) создаются тройками X/Y/Z. Перечисление — [MaterialDB-9](MaterialDB-9.mdpuml).

## `SolidMaterialSettings`

Исходники: [SolidMaterialSettings.cs](../../MaterialDB/MaterialData/Settings/SolidMaterialSettings.cs). Базовый тип: `MaterialSettings`.

```text
Phases : PhaseData? [get, set; по умолчанию null]
```

Если задано, записывается «Структура», и таблицы тепловых и механических свойств строятся как `TemperaturePhase` с колонкой на каждую фазу. Иначе — `Argument`. `PhaseData` — [MaterialDB-6](MaterialDB-6.mdpuml).

## `FluidMaterialSettings`

Исходники: [FluidMaterialSettings.cs](../../MaterialDB/MaterialData/Settings/FluidMaterialSettings.cs). Базовый тип: `MaterialSettings`. Используется для жидкости и как база настроек газа.

```text
ReferencePressure : double [get, set; МПа; по умолчанию 1]
```

Записывается в «Общие сведения / Опорное давление».

## `GasMaterialSettings`

Исходники: [GasMaterialSettings.cs](../../MaterialDB/MaterialData/Settings/GasMaterialSettings.cs). Базовый тип: `FluidMaterialSettings`.

```text
DensityModel : DensityModel [get, set; по умолчанию Table]
ViscosityModel : ViscosityModel [get, set; по умолчанию Table]
MolarMass : double? [get, set; кг/моль]
Sutherland : SutherlandCoefficients? [get, set]
```

Правила:

* `DensityModel.IdealGas`: таблица «Плотность» не создаётся, `MolarMass` обязательна.
* `ViscosityModel.Sutherland`: таблица «Динамическая вязкость» не создаётся, `Sutherland` обязателен.
* Если обязательное значение не задано, конструктор газа бросает `ArgumentException`.
* Заданные `MolarMass` и `Sutherland` записываются в «Общие сведения» при любой модели.

## `SutherlandCoefficients`

Исходники: [SutherlandCoefficients.cs](../../MaterialDB/MaterialData/Settings/SutherlandCoefficients.cs). Класс `sealed`, неизменяемый.

```text
Mu0 : double [get; МПа·с]
T0 : double [get; К]
S : double [get; К]
SutherlandCoefficients(double mu0, double t0, double s)
```

Записывается в «Общие сведения / Коэффициенты Сазерленда» таблицей `Коэффициент`/`Значение` со строками μ0, T0, S (`ArgumentKind.Descriptor`).

## Пример

```csharp
var settings = new GasMaterialSettings
{
    DensityModel = DensityModel.IdealGas,
    ViscosityModel = ViscosityModel.Sutherland,
    MolarMass = 0.029,
    ReferencePressure = 0.101325,
    Sutherland = new SutherlandCoefficients(1.716e-11, 273.15, 110.4)
};
var air = new GasMaterialItem("Воздух", settings);
```
