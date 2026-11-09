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
MolarMass
PoissonRatio
ReferencePressure
Structure
SutherlandCoefficients
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
