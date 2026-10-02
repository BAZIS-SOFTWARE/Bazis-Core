# MaterialDB-3 — Контракты и данные материалов

[Диаграмма](MaterialDB-3.mdpuml). [Обзор проекта](MaterialDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Category`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [Category.cs](../../MaterialDB/MaterialData/Category.cs).

Базовые типы: `ICategory`.

```text
PropertyDic : Dictionary<PropertyEnum, string> [field]
this[PropertyEnum key] : Property [get, set]
```

## `CategoryData`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [CategoryData.cs](../../MaterialDB/MaterialData/CategoryData.cs).

Базовые типы: `IDictionary<string, Category>`.

```text
Count : int [get]
IsReadOnly : bool [get]
Keys : ICollection<string> [get]
Values : ICollection<Category> [get]
this[PropertyEnum key] : Category [get, set]
this[string key] : Category [get, set]
Add(KeyValuePair<string, Category> item) : void
Add(string key, Category value) : void
Clear() : void
Contains(KeyValuePair<string, Category> item) : bool
ContainsKey(string key) : bool
CopyTo(KeyValuePair<string, Category>[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<KeyValuePair<string, Category>>
Remove(KeyValuePair<string, Category> item) : bool
Remove(string key) : bool
TryGetValue(string key, out Category value) : bool
```

## `MaterialDBData`

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [MaterialDBData.cs](../../MaterialDB/MaterialData/MaterialDBData.cs).

Базовые типы: `IMaterialDB`.

```text
Count : int [get]
IsReadOnly : bool [get]
Keys : ICollection<string> [get]
Values : ICollection<MaterialDBItem> [get]
this[string key] : MaterialDBItem [get, set]
Add(KeyValuePair<string, MaterialDBItem> item) : void
Add(string key, MaterialDBItem value) : void
Clear() : void
Contains(KeyValuePair<string, MaterialDBItem> item) : bool
ContainsKey(string key) : bool
CopyTo(KeyValuePair<string, MaterialDBItem>[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<KeyValuePair<string, MaterialDBItem>>
MaterialDBData()
MaterialDBData(string dbName, string dbFolder)
Remove(KeyValuePair<string, MaterialDBItem> item) : bool
Remove(string key) : bool
TryGetValue(string key, out MaterialDBItem value) : bool
```
