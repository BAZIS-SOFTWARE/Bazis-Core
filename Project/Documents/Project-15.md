# Project-15 — Функции, материалы и ввод/вывод

[Диаграмма](Project-15.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `LoadResultsFileBrfTextFormat`

Пространство имён: `Project.Results.IO`. Исходники: [LoadResultsFileBrfTextFormat.cs](../../Project/Results/IO/LoadResultsFileBrfTextFormat.cs). Базовые типы: `IResultsLoader`.

```text
GetResultFromDb(string fullPath, string tableName, float time) : Result
GetResultsFromDb(string fullPath, string table) : IEnumerable<Result>
GetValuesFromDb(string fullPath, string table, string column) : IEnumerable<float>
Load(string path) : IEnumerable<Result>
ReadResults(StreamReader sr) : List<Result>
ReadResultsKind(StreamReader sr, TaskKind taskKind, Dictionary<string, Dictionary<TaskKind, string[]>> format) : List<Result>
```

## `LoadResultsFileDB`

Пространство имён: `Project.Results.IO`. Исходники: [LoadResultsFileDB.cs](../../Project/Results/IO/LoadResultsFileDB.cs). Базовые типы: `IResultsLoader`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `SaveEventArgs`

Пространство имён: `Project.Results.IO`. Исходники: [SaveEventArgs.cs](../../Project/Results/IO/SaveEventArgs.cs). Базовые типы: `ISaveEventArgs`.

```text
SaveEventArgs(string message)
```

## `SaveResultsFileDb`

Пространство имён: `Project.Results.IO`. Исходники: [SaveResultsFileDb.cs](../../Project/Results/IO/SaveResultsFileDb.cs). Базовые типы: `IResultsSaver`.

```text
AddData(ResultDataBase db, DataTable table, float time) : void
```

## `SaveResultsFileTextFormat`

Пространство имён: `Project.Results.IO`. Исходники: [SaveResultsFileTextFormat.cs](../../Project/Results/IO/SaveResultsFileTextFormat.cs). Базовые типы: `IResultsSaver`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `CustomFunction`

Пространство имён: `Project.Tasks.Functions`. Исходники: [CustomFunction.cs](../../Project/Tasks/Functions/CustomFunction.cs). Базовые типы: `Function`.

```text
ExecutedFile : string [get]
CalcValue() : double
CreateEngine(string file) : void
```
