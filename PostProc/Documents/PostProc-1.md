# PostProc-1 — Экспорт данных

[Диаграмма](PostProc-1.mdpuml). [Обзор проекта](PostProc.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IGridExporter`

Пространство имён: `PostProc.IO.GridExport`.

Исходники: [IGridExporter.cs](../../PostProc/IO/GridExport/IGridExporter.cs).

```text
ExportGridSurfaces(IEnumerable<ResultFigure> figures, string savePath, string format) : void
```

## `IGridSurfacesSaver`

Пространство имён: `PostProc.IO.GridExport`.

Исходники: [IGridSurfacesSaver.cs](../../PostProc/IO/GridExport/IGridSurfacesSaver.cs).

```text
SaveGrid(IEnumerable<ResultFigure> figures, string savePath) : void
```

## `IResultsExportSaver`

Пространство имён: `PostProc.IO.ResultExport`.

Исходники: [IResultsExportSaver.cs](../../PostProc/IO/ResultExport/IResultsExportSaver.cs).

```text
SaveResults(IEnumerable<IModelObject> objects, Result result, string resDescr, string savePath) : void
```

## `IResultsExporter`

Пространство имён: `PostProc.IO.ResultExport`.

Исходники: [IResultsExporter.cs](../../PostProc/IO/ResultExport/IResultsExporter.cs).

```text
ExportObjectsResults(IEnumerable<IModelObject> objects, Result result, string resDesc, string savePath, string format) : void
```

## `GridExporter`

Пространство имён: `PostProc.IO.GridExport`.

Исходники: [GridExporter.cs](../../PostProc/IO/GridExport/GridExporter.cs).

Базовые типы: `IGridExporter`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `SaveGridSurfacesToBPF`

Пространство имён: `PostProc.IO.GridExport`.

Исходники: [SaveGridSurfacesToBPF.cs](../../PostProc/IO/GridExport/SaveGridSurfacesToBPF.cs).

Базовые типы: `IGridSurfacesSaver`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.
