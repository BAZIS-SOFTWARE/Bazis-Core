# MaterialDB-5 — Свойства, фазовый состав и функции

[Диаграмма](MaterialDB-5.mdpuml). [Обзор проекта](MaterialDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ChemicalData`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [ChemicalData.cs](../../MaterialDB/MaterialData/ChemicalData.cs).

Базовые типы: `IDictionary<ChemElement, float>`.

```text
Count : int [get]
IsReadOnly : bool [get]
Keys : ICollection<ChemElement> [get]
Values : ICollection<float> [get]
this[ChemElement key] : float [get, set]
Add(ChemElement key, float value) : void
Add(KeyValuePair<ChemElement, float> item) : void
Clear() : void
Contains(KeyValuePair<ChemElement, float> item) : bool
ContainsKey(ChemElement key) : bool
CopyTo(KeyValuePair<ChemElement, float>[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<KeyValuePair<ChemElement, float>>
Parce(string el) : ChemElement
Remove(ChemElement key) : bool
Remove(KeyValuePair<ChemElement, float> item) : bool
TryGetValue(ChemElement key, out float value) : bool
```

## `GeneralAbsentException`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [GeneralAbsentException.cs](../../MaterialDB/MaterialData/GeneralAbsentException.cs).

Базовые типы: `Exception`.

```text
GeneralAbsentException()
GeneralAbsentException(string? message)
GeneralAbsentException(string? message, Exception? innerException)
```

## `DataItem`

Пространство имён: `MaterialDB`.

Исходники: [DataItem.cs](../../MaterialDB/DataItem.cs).

```text
DataItem(string name, string x_unit, string y_unit, string units, DataTable dataTable)
DataItem(string copyName, DataItem source)
Name : string [get, set]
X_unit : string [get, set]
Y_unit : string [get, set]
Units : string [get, set]
DataTable : DataTable [get, set]
this[int row, int col] : double [get, set]
Copy(string copyName) : DataItem
AddRow() : void
ToString() : string
```

## `PropertyItem`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [PropertyItem.cs](../../MaterialDB/MaterialData/PropertyItem.cs).

Базовые типы: `DataItem`, `IProperty`.

```text
PropertyItem(string name, string x_unit, string y_unit, string units, DataTable dataTable)
Copy(string copyName) : PropertyItem
CalcProp(PhaseData phaseData, float temp) : float
```

## `PropertyData`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [PropertyData.cs](../../MaterialDB/MaterialData/PropertyData.cs).

Базовые типы: `IDictionary<string, PropertyItem>`.

```text
Count : int [get]
IsReadOnly : bool [get]
Keys : ICollection<string> [get]
Values : ICollection<PropertyItem> [get]
this[string key] : PropertyItem [get, set]
Add(KeyValuePair<string, PropertyItem> item) : void
Add(string key, PropertyItem value) : void
Clear() : void
Contains(KeyValuePair<string, PropertyItem> item) : bool
ContainsKey(string key) : bool
CopyTo(KeyValuePair<string, PropertyItem>[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<KeyValuePair<string, PropertyItem>>
Remove(KeyValuePair<string, PropertyItem> item) : bool
Remove(string key) : bool
TryGetValue(string key, out PropertyItem value) : bool
```
