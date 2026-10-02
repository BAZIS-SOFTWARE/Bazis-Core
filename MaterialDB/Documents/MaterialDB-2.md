# MaterialDB-2 — Свойства, фазовый состав и функции

[Диаграмма](MaterialDB-2.mdpuml). [Обзор проекта](MaterialDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `FunctionDBData`

Пространство имён: `MaterialDB.FunctionData`.

Исходники: [FunctionDBData.cs](../../MaterialDB/FunctionData/FunctionDBData.cs).

Базовые типы: `IDictionary<string, Property>`.

```text
Count : int [get]
IsReadOnly : bool [get]
Keys : ICollection<string> [get]
Name : string [get, set]
Values : ICollection<Property> [get]
this[string key] : Property [get, set]
Add(KeyValuePair<string, Property> item) : void
Add(string key, Property value) : void
Clear() : void
Contains(KeyValuePair<string, Property> item) : bool
ContainsKey(string key) : bool
CopyTo(KeyValuePair<string, Property>[] array, int arrayIndex) : void
FunctionDBData()
FunctionDBData(string dbName, string dbFolder)
GetEnumerator() : IEnumerator<KeyValuePair<string, Property>>
Remove(KeyValuePair<string, Property> item) : bool
Remove(string key) : bool
TryGetValue(string key, out Property value) : bool
```

## `CategoryEnum`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [MaterialDBData.cs](../../MaterialDB/MaterialData/MaterialDBData.cs).

```text
General
Mechanical
Metallurgical
Thermal
```

## `ChemElement`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [ChemicalData.cs](../../MaterialDB/MaterialData/ChemicalData.cs).

Базовые типы: `int`.

```text
Al
As
C
Cr
Cu
Mn
Mo
Nb
Ni
None
P
Si
Ti
V
W
```
