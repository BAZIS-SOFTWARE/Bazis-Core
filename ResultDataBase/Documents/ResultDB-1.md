# ResultDB-1 — Результаты и хранилище

[Диаграмма](ResultDB-1.mdpuml). [Обзор проекта](ResultDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `DataTableEx`

Пространство имён: `ResultDB`.

Исходники: [DataTableEx.cs](../../ResultDataBase/DataTableEx.cs).

```text
GetTableData(this DataTable table, int objNumber, string resDescr) : float
GetTableSchema(this DataTable table) : string[]
```

## `Result`

Пространство имён: `ResultDB`.

Исходники: [Result.cs](../../ResultDataBase/Result.cs).

```text
Data : DataSet [get, private set]
Name : string [get]
Time : float [get]
AddDataTable(DataTable dbDataTable) : void
ContainsTable(string tableName) : bool
GetValue(int tableInd, int objNumber, string resDescr) : float
GetValue(string tableName, int objNumber, string resDescr) : float
Result(DataSet dataSet, float time, string name)
Result(float time, string name)
ToString() : string
TryGetValue<T>(string tableName, int objNumber, string resDescr, out T resu) : bool
```

## `ResultDataBase`

Пространство имён: `ResultDB`.

Исходники: [ResultDataBase.cs](../../ResultDataBase/ResultDataBase.cs).

```text
AddData(string queStr) : int
ClearTable(string tableName) : void
CloseConnection() : void
CreateAddCommand(string table, string columns, string values) : string
CreateTables(string tableName, string[] columnsName) : void
DeleteTable(string tableName) : void
GetResults(string table, string column, string filter) : DataTable
GetTableShema(string tableName) : IEnumerable<string>
OpenConnection() : void
ResultDataBase(string fullName)
```
