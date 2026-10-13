# Project-14 — Функции, материалы и ввод/вывод

[Диаграмма](Project-14.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

Типы из `Project/Results` на этой схеме описывают архивные исходники, исключённые из сборки
[Project.csproj](../Project.csproj). Архивный [ILoadEventArgs](Project-3.md) также исключён из сборки,
его исходник сохранён для этих типов. Загрузчики результатов и их события не входят
в демонтаж событий загрузки модели, проекта и данных задачи.

## `SaveProjectTextFormat`

Пространство имён: `Project.IO`.

Исходники: [SaveProjectFileTextFormat.cs](../../Project/IO/SaveProjectFileTextFormat.cs).

Базовые типы: `IProjectSaver`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `DataTableEx`

Пространство имён: `Project.Results`.

Исходники: [DataTableEx.cs](../../Project/Results/DataTableEx.cs).

```text
GetTableData(this DataTable table, int objNumber, string resDescr) : float
GetTableSchema(this DataTable table) : string[]
```

## `Result`

Пространство имён: `Project.Results`.

Исходники: [Result.cs](../../Project/Results/Result.cs).

```text
Data : DataSet [get, private set]
TaskKind : TaskKind [get]
Time : float [get]
AddDataTable(DataTable dbDataTable) : void
GetValue(int tableInd, int objNumber, string resDescr) : float
GetValue(string tableName, int objNumber, string resDescr) : float
Result(DataSet dataSet, float time, TaskKind taskKind)
Result(float time, TaskKind taskKind)
ToString() : string
```

## `ResultDataBase`

Пространство имён: `Project.Results`.

Исходники: [ResultDataBase.cs](../../Project/Results/ResultDataBase.cs).

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

## `CSVToDataTableConverter`

Пространство имён: `Project.Results.IO`.

Исходники: [CSVToDataTableConverter.cs](../../Project/Results/IO/CSVToDataTableConverter.cs).

```text
ConvertCSVtoDataTable(string strFilePath) : DataTable
```

## `LoadEventArgs`

Пространство имён: `Project.Results.IO`.

Исходники: [LoadEventArgs.cs](../../Project/Results/IO/LoadEventArgs.cs).

Базовые типы: `ILoadEventArgs`.

```text
LoadEventArgs(string message)
```
