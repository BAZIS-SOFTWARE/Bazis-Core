# PostProc-3 — Экспорт данных

[Диаграмма](PostProc-3.mdpuml). [Обзор проекта](PostProc.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `SaveGridSurfacesToBinSTL`

Пространство имён: `PostProc.IO.GridExport`.

Исходники: [SaveGridSurfacesToBinSTL.cs](../../PostProc/IO/GridExport/SaveGridSurfacesToBinSTL.cs).

Базовые типы: `SaveGridSurfacesToSTLBase`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `SaveGridSurfacesToSTLBase`

Пространство имён: `PostProc.IO.GridExport`.

Исходники: [SaveGridSurfacesToSTLBase.cs](../../PostProc/IO/GridExport/SaveGridSurfacesToSTLBase.cs).

Базовые типы: `IGridSurfacesSaver`.

```text
SplitToTriangles(ResultSurface surface) : IEnumerable<List<Point3D>>
```

## `SaveGridSurfacesToTxtSTL`

Пространство имён: `PostProc.IO.GridExport`.

Исходники: [SaveGridSurfacesToTxtSTL.cs](../../PostProc/IO/GridExport/SaveGridSurfacesToTxtSTL.cs).

Базовые типы: `SaveGridSurfacesToSTLBase`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `ResultsExporter`

Пространство имён: `PostProc.IO.ResultExport`.

Исходники: [ResultsExporter.cs](../../PostProc/IO/ResultExport/ResultsExporter.cs).

Базовые типы: `IResultsExporter`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `SaveObjectsResultsToCSV`

Пространство имён: `PostProc.IO.ResultExport`.

Исходники: [SaveObjectsResultsToCSV.cs](../../PostProc/IO/ResultExport/SaveObjectsResultsToCSV.cs).

Базовые типы: `IResultsExportSaver`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `SaveObjectsResultsToTXT`

Пространство имён: `PostProc.IO.ResultExport`.

Исходники: [SaveObjectsResultsToTXT.cs](../../PostProc/IO/ResultExport/SaveObjectsResultsToTXT.cs).

Базовые типы: `IResultsExportSaver`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.
