# ResultDB-2 — Чтение и сохранение

[Диаграмма](ResultDB-2.mdpuml). [Обзор проекта](ResultDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IResultsLoader`

Пространство имён: `ResultDB.Interfaces`. Исходники: [IResultsLoader.cs](../../ResultDataBase/Interfaces/IResultsLoader.cs).

```text
GetResult(string fullPath, List<string> tables, float time) : Result
GetResults(string fullPath, List<string> tables) : IEnumerable<Result>
GetTablesNames(string fullPath) : IEnumerable<string>
GetTablesSchemes(string fullPath) : IEnumerable<KeyValuePair<string, List<string>>>
GetValues(string fullPath, string tableName, string column) : IEnumerable<float>
```

## `IResultsSaver`

Пространство имён: `ResultDB.Interfaces`. Исходники: [IResultsSaver.cs](../../ResultDataBase/Interfaces/IResultsSaver.cs).

```text
Save(List<Result> results, string path, bool append) : void
```

## `CSVToDataTableConverter`

Пространство имён: `ResultDB.IO`. Исходники: [CSVToDataTableConverter.cs](../../ResultDataBase/IO/CSVToDataTableConverter.cs).

```text
ConvertCSVtoDataTable(string strFilePath) : DataTable
```

## `LoadEventArgs`

Пространство имён: `ResultDB.IO`. Исходники: [LoadEventArgs.cs](../../ResultDataBase/IO/LoadEventArgs.cs).

```text
Message : string [get]
LoadEventArgs(string message)
```

## `LoadResultsFileDB`

Пространство имён: `ResultDB.IO`. Исходники: [LoadResultsFileDB.cs](../../ResultDataBase/IO/LoadResultsFileDB.cs). Базовые типы: `IResultsLoader`.

```text
Merge(string dbPath1, string dbPath2) : string
```

## `SaveEventArgs`

Пространство имён: `ResultDB.IO`. Исходники: [SaveEventArgs.cs](../../ResultDataBase/IO/SaveEventArgs.cs).

```text
Message : string [get, private set]
SaveEventArgs(string message)
```
