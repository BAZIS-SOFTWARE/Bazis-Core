# MaterialDB-6 — Свойства, фазовый состав и функции

[Диаграмма](MaterialDB-6.mdpuml). [Обзор проекта](MaterialDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `PropertyEnum`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [MaterialDBData.cs](../../MaterialDB/MaterialData/MaterialDBData.cs).

```text
Capacity
Compressibility
Density
DensityModel
DynamicViscosity
HardeningFactor
HardeningModel
HardnessModel
MaterialModel
MediumKind
MolarMass [Obsolete]
PoissonRatio
ReferencePressure
Structure
SutherlandCoefficients [Obsolete]
TensileStrength
ThermalConductivity
ThermalConductivityX
ThermalConductivityY
ThermalConductivityZ
ThermalExpansion
ThermalExpansionX
ThermalExpansionY
ThermalExpansionZ
ViscosityModel
VolumeExpansion
YieldStrength
YoungModule
YoungModuleX
YoungModuleY
YoungModuleZ
```

## `Phase`

Пространство имён: `MaterialDB.MaterialData.MetallurgicalData`.

Исходники: [Phase.cs](../../MaterialDB/MaterialData/MetallurgicalData/Phase.cs).

```text
Name : string [get, set]
Value : float [get, set]
ToString() : string
```

## `PhaseData`

Пространство имён: `MaterialDB.MaterialData.MetallurgicalData`.

Исходники: [PhaseData.cs](../../MaterialDB/MaterialData/MetallurgicalData/PhaseData.cs).

Базовые типы: `IList<Phase>`.

```text
Count : int [get]
IsReadOnly : bool [get]
this[int index] : Phase [get, set]
Add(Phase item) : void
AddRange(IEnumerable<Phase> items) : void
Clear() : void
Contains(Phase item) : bool
CopyTo(Phase[] array, int arrayIndex) : void
Find(string phaseName) : Phase
GetEnumerator() : IEnumerator<Phase>
IndexOf(Phase item) : int
Insert(int index, Phase item) : void
PhaseData()
PhaseData(DataTable phaseTable)
PhaseData(List<Phase> phases)
Remove(Phase item) : bool
RemoveAt(int index) : void
ToString() : string
```

## `Process<T>`

Пространство имён: `MaterialDB.MaterialData.MetallurgicalData`.

Исходники: [Process.cs](../../MaterialDB/MaterialData/MetallurgicalData/Process.cs).

```text
DataTable : DataTable [get]
Name : string [get]
PhaseMax : T [get, internal set]
PhaseMin : T [get, internal set]
Reaction : PropertyItem [get, internal set]
TempMax : T [get, internal set]
TempMin : T [get, internal set]
Process(PropertyItem reaction)
ToString() : string
```

Значения 1.2.0 (после `MaterialModel` в коде) соответствуют записям: «Агрегатное состояние», «Модель плотности», «Молярная масса», «Опорное давление», «Модель вязкости», «Коэффициенты Сазерленда», «Динамическая вязкость», «Сжимаемость», «Коэффициент объёмного расширения», «Теплопроводность», «Модуль Юнга» и «ТКЛР» по осям X/Y/Z и «Модель твердости» (в той же записи лежат индексы фаз модели Куркина).

`MolarMass` и `SutherlandCoefficients` помечены `[Obsolete]`: отдельных записей «Молярная масса» и «Коэффициенты Сазерленда» нет. Молярная масса — строка «M» в «Модели плотности», коэффициенты Сазерленда — строки μ0, T0, S в «Модели вязкости» ([MaterialDB-8](MaterialDB-8.md)). Значения перечисления оставлены, чтобы не сдвигать номера следующих.
