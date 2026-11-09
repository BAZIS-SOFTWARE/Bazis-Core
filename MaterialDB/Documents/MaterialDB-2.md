# MaterialDB-2 — Свойства, фазовый состав и функции

[Диаграмма](MaterialDB-2.mdpuml). [Обзор проекта](MaterialDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `FunctionDBData`

Пространство имён: `MaterialDB.FunctionData`.

Исходники: [FunctionDBData.cs](../../MaterialDB/FunctionData/FunctionDBData.cs).

Базовые типы: `IDictionary<string, FunctionItem>`.

```text
Count : int [get]
IsReadOnly : bool [get]
Keys : ICollection<string> [get]
Name : string [get, set]
Values : ICollection<FunctionItem> [get]
this[string key] : FunctionItem [get, set]
Add(KeyValuePair<string, FunctionItem> item) : void
Add(string key, FunctionItem value) : void
Clear() : void
Contains(KeyValuePair<string, FunctionItem> item) : bool
ContainsKey(string key) : bool
CopyTo(KeyValuePair<string, FunctionItem>[] array, int arrayIndex) : void
FunctionDBData()
FunctionDBData(string dbName, string dbFolder)
GetEnumerator() : IEnumerator<KeyValuePair<string, FunctionItem>>
OnDeserialized(StreamingContext context) : void [internal]
Remove(KeyValuePair<string, FunctionItem> item) : bool
Remove(string key) : bool
TryGetValue(string key, out FunctionItem value) : bool
```

## `FunctionItem`

Пространство имён: `MaterialDB.FunctionData`.

Исходники: [FunctionItem.cs](../../MaterialDB/FunctionData/FunctionItem.cs).

Базовый тип: `DataItem`. Тип помечен `[Obsolete]`: будет удалён в MaterialDB 2.0.0, вместо него используется `DataItem`.

```text
FunctionItem(string name, string x_unit, string y_unit, string units, DataTable dataTable)
FunctionItem(string name, string x_unit, string y_unit, string units, DataTable dataTable, ArgumentKind argumentKind) [JsonConstructor]
Copy(string copyName) : FunctionItem
CalcProp(float variable) : float [Obsolete; будет удалён в 2.0.0]
```

## `CategoryEnum`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [MaterialDBData.cs](../../MaterialDB/MaterialData/MaterialDBData.cs).

```text
Fluid
Gas
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

Загрузка базы функций читает найденный путь без Windows-разделителей. После десериализации `FunctionDBData.OnDeserialized` вызывает `ArgumentKindResolver` ([MaterialDB-10](MaterialDB-10.mdpuml)): записи без `ArgumentKind` получают `Constant` (одна строка) или `Argument`.

`CategoryEnum.Fluid` — «Гидродинамические свойства» жидкости, `CategoryEnum.Gas` — «Газодинамические свойства» газа.
