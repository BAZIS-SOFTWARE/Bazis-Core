# MaterialDB-1 — Контракты и данные материалов

[Диаграмма](MaterialDB-1.mdpuml). [Обзор проекта](MaterialDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ICategory`

Пространство имён: `MaterialDB.Interfaces`.

Исходники: [ICategory.cs](../../MaterialDB/Interfaces/ICategory.cs).

```text
Name : string [get, set]
PropertyData : PropertyData [get, set]
this[string key] : Property [get, set]
```

## `IDataInformer`

Пространство имён: `MaterialDB.Interfaces`.

Исходники: [IDataInformer.cs](../../MaterialDB/Interfaces/IDataInformer.cs).

```text
GetDataNames(DataSet dataSet) : List<string>
```

## `ILoader`

Пространство имён: `MaterialDB.Interfaces`.

Исходники: [ILoader.cs](../../MaterialDB/Interfaces/ILoader.cs).

```text
LoadDataBase(string path) : DataSet
```

## `IMaterialDB`

Пространство имён: `MaterialDB.Interfaces`.

Исходники: [IMaterialDB.cs](../../MaterialDB/Interfaces/IMaterialDB.cs).

Базовые типы: `IDictionary<string, MaterialDBItem>`.

```text
Name : string [get, set]
GetPhases(List<string> materialsNames) : HashSet<string>
```

## `IProperty`

Пространство имён: `MaterialDB.Interfaces`.

Исходники: [IProperty.cs](../../MaterialDB/Interfaces/IProperty.cs).

```text
DataTable : DataTable [get, set]
Name : string [get, set]
Units : string [get, set]
X_unit : string [get, set]
Y_unit : string [get, set]
this[int row, int col] : double [get, set]
AddRow() : void
CalcProp(DataTable phaseData, float temp) : float
CalcProp(PhaseData phaseData, float temp) : float
CalcProp(float variable) : float
Copy(string copyName) : Property
```

## `ISaver`

Пространство имён: `MaterialDB.Interfaces`.

Исходники: [ISaver.cs](../../MaterialDB/Interfaces/ISaver.cs).

```text
SaveDataBase(DataSet data, string path) : void
```
