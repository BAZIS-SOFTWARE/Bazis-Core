# Project-13 — Функции, материалы и ввод/вывод

[Диаграмма](Project-13.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IResultsExportSaver`

Пространство имён: `Project.Interfaces.Results`.

Исходники: [IResultsExportSaver.cs](../../Project/Interfaces/Results/IResultsExportSaver.cs).

```text
SaveResults(IEnumerable<IModelObject> objects, Result result, string resDescr, string savePath) : void
```

## `IResultsExporter`

Пространство имён: `Project.Interfaces.Results`.

Исходники: [IResultsExporter.cs](../../Project/Interfaces/Results/IResultsExporter.cs).

```text
ExportObjectsResults(IEnumerable<IModelObject> objects, Result result, string resDesc, string savePath, string format) : void
```

## `LoadProjectFromTextFormat`

Пространство имён: `Project.IO`.

Исходники: [LoadProjectFileTextFormat.cs](../../Project/IO/LoadProjectFileTextFormat.cs).

Базовые типы: `IProjectLoader`.

Метод `Load` полностью соответствует [IProjectLoader](Project-3.md) с необязательным `IProgress<int>`;
собственных публичных перегрузок и событий загрузки нет. `ProgressRange` распределяет
прогресс загрузчиков модели и задачи по двум последовательным диапазонам 0–50% и 50–100%.

Полная сигнатура реализации (на диаграмме наследуется от интерфейса):

```text
Load(string fullPath, IProgress<int> progress = null) : IProjectData
```

## `ProgressRange`

Пространство имён: `Project.IO`.

Исходники: [ProgressRange.cs](../../Project/IO/ProgressRange.cs).

Базовые типы: `IProgress<int>`.

```text
ProgressRange(IProgress<int> progress, int start, int length)
Report(int value) : void
```

## `SaveEventArgs`

Пространство имён: `Project.IO`.

Исходники: [SaveEventArgs.cs](../../Project/IO/SaveEventArgs.cs).

Базовые типы: `ISaveEventArgs`.

```text
SaveEventArgs(string message)
```
