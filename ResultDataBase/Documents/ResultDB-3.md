# ResultDB-3 — Чтение и сохранение

[Диаграмма](ResultDB-3.mdpuml). [Обзор проекта](ResultDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `SaveResultsFileDb`

Пространство имён: `ResultDB.IO`.

Исходники: [SaveResultsFileDb.cs](../../ResultDataBase/IO/SaveResultsFileDb.cs).

Базовые типы: `IResultsSaver`.

```text
AddData(ResultDataBase db, DataTable table, float time) : void
```

## `TaskKind`

Пространство имён: `ResultDB.IO`.

Исходники: [LoadResultsFileDB.cs](../../ResultDataBase/IO/LoadResultsFileDB.cs).

```text
механическая
термическая
```
