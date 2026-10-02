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

## `Property`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [Property.cs](../../MaterialDB/MaterialData/Property.cs).

Базовые типы: `IProperty`.

```text
this[int row,int col] : double [get, set]
Property(string name, string x_unit, string y_unit, string units, DataTable dataTable)
ToString() : string
```

## `PropertyData`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [PropertyData.cs](../../MaterialDB/MaterialData/PropertyData.cs).

Базовые типы: `IDictionary<string, Property>`.

```text
Count : int [get]
IsReadOnly : bool [get]
Keys : ICollection<string> [get]
Values : ICollection<Property> [get]
this[string key] : Property [get, set]
Add(KeyValuePair<string, Property> item) : void
Add(string key, Property value) : void
Clear() : void
Contains(KeyValuePair<string, Property> item) : bool
ContainsKey(string key) : bool
CopyTo(KeyValuePair<string, Property>[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<KeyValuePair<string, Property>>
Remove(KeyValuePair<string, Property> item) : bool
Remove(string key) : bool
TryGetValue(string key, out Property value) : bool
```
