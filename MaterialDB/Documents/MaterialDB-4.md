# MaterialDB-4 — Контракты и данные материалов

[Диаграмма](MaterialDB-4.mdpuml). [Обзор проекта](MaterialDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `MaterialDBItem`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [MaterialDBItem.cs](../../MaterialDB/MaterialData/MaterialDBItem.cs).

```text
CategoryData : CategoryData [get, set]
CategoryDic : Dictionary<CategoryEnum, string> [field]
Name : string [get, set]
this[CategoryEnum key] : Category [get, set]
this[string key] : Category [get, set]
AddCrystallization(double ts, double tl) : void
AddGeneralProps() : void
AddHardeningModel(int mode) : void
AddMaterialModel(int mode) : void
AddMechProps() : void
AddMetallurgicalProps() : void
AddPhaseData(PhaseData phases) : void
AddThermalProps() : void
CheckMechanicalProps() : bool
CheckPhaseData() : bool
CheckThermalProps() : bool
Copy(string copyName) : MaterialDBItem
GetHashCode() : int
GetPhases() : IEnumerable<string>
MaterialDBItem()
MaterialDBItem(string name)
```
