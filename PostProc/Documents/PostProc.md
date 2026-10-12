# PostProc

Строит поля результатов, шкалы цветов и экспортирует данные. В текущем коде `PostProcController` не объявляет реализацию `IPostProcController`, поэтому связь реализации не добавлена. Экспортёры в целевом API не содержат событий.

[Обзорная диаграмма](PostProc.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [PostProc-1](PostProc-1.mdpuml) | Экспорт данных; IGridExporter, IGridSurfacesSaver, IResultsExportSaver, IResultsExporter, GridExporter, SaveGridSurfacesToBPF |
| [PostProc-2](PostProc-2.mdpuml) | Построение полей и шкала результатов; IPostProcController, IResultsMerger, IGradientsCreator, IResultsFieldsCreator, ItemRange |
| [PostProc-3](PostProc-3.mdpuml) | Экспорт данных; SaveGridSurfacesToBinSTL, SaveGridSurfacesToSTLBase, SaveGridSurfacesToTxtSTL, ResultsExporter, SaveObjectsResultsToCSV, SaveObjectsResultsToTXT |
| [PostProc-4](PostProc-4.mdpuml) | Построение полей и шкала результатов; MergeResults, PostProcController, LineGradientsCreator, ResultFigure, ResultItem, ResultSurface |
| [PostProc-5](PostProc-5.mdpuml) | Построение полей и шкала результатов; ResultsFieldsCreator, SurfaceGradientsCreator, SwapValues, FieldCreator, IsoLevelsCreator, ResultPoint |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `PostProc.IPostProcController` | [PostProc-2](PostProc-2.mdpuml) | [IPostProcController.cs](../../PostProc/IPostProcController.cs) |
| `PostProc.IResultsMerger` | [PostProc-2](PostProc-2.mdpuml) | [IResultsMerger.cs](../../PostProc/IResultsMerger.cs) |
| `PostProc.ItemRange` | [PostProc-2](PostProc-2.mdpuml) | [ItemRange.cs](../../PostProc/ItemRange.cs) |
| `PostProc.MergeResults` | [PostProc-4](PostProc-4.mdpuml) | [MergeResults.cs](../../PostProc/MergeResults.cs) |
| `PostProc.PostProcController` | [PostProc-4](PostProc-4.mdpuml) | [PostProcController.cs](../../PostProc/PostProcController.cs) |
| `PostProc.IO.GridExport.GridExporter` | [PostProc-1](PostProc-1.mdpuml) | [GridExporter.cs](../../PostProc/IO/GridExport/GridExporter.cs) |
| `PostProc.IO.GridExport.IGridExporter` | [PostProc-1](PostProc-1.mdpuml) | [IGridExporter.cs](../../PostProc/IO/GridExport/IGridExporter.cs) |
| `PostProc.IO.GridExport.IGridSurfacesSaver` | [PostProc-1](PostProc-1.mdpuml) | [IGridSurfacesSaver.cs](../../PostProc/IO/GridExport/IGridSurfacesSaver.cs) |
| `PostProc.IO.GridExport.SaveGridSurfacesToBPF` | [PostProc-1](PostProc-1.mdpuml) | [SaveGridSurfacesToBPF.cs](../../PostProc/IO/GridExport/SaveGridSurfacesToBPF.cs) |
| `PostProc.IO.GridExport.SaveGridSurfacesToBinSTL` | [PostProc-3](PostProc-3.mdpuml) | [SaveGridSurfacesToBinSTL.cs](../../PostProc/IO/GridExport/SaveGridSurfacesToBinSTL.cs) |
| `PostProc.IO.GridExport.SaveGridSurfacesToSTLBase` | [PostProc-3](PostProc-3.mdpuml) | [SaveGridSurfacesToSTLBase.cs](../../PostProc/IO/GridExport/SaveGridSurfacesToSTLBase.cs) |
| `PostProc.IO.GridExport.SaveGridSurfacesToTxtSTL` | [PostProc-3](PostProc-3.mdpuml) | [SaveGridSurfacesToTxtSTL.cs](../../PostProc/IO/GridExport/SaveGridSurfacesToTxtSTL.cs) |
| `PostProc.IO.ResultExport.IResultsExportSaver` | [PostProc-1](PostProc-1.mdpuml) | [IResultsExportSaver.cs](../../PostProc/IO/ResultExport/IResultsExportSaver.cs) |
| `PostProc.IO.ResultExport.IResultsExporter` | [PostProc-1](PostProc-1.mdpuml) | [IResultsExporter.cs](../../PostProc/IO/ResultExport/IResultsExporter.cs) |
| `PostProc.IO.ResultExport.ResultsExporter` | [PostProc-3](PostProc-3.mdpuml) | [ResultsExporter.cs](../../PostProc/IO/ResultExport/ResultsExporter.cs) |
| `PostProc.IO.ResultExport.SaveObjectsResultsToCSV` | [PostProc-3](PostProc-3.mdpuml) | [SaveObjectsResultsToCSV.cs](../../PostProc/IO/ResultExport/SaveObjectsResultsToCSV.cs) |
| `PostProc.IO.ResultExport.SaveObjectsResultsToTXT` | [PostProc-3](PostProc-3.mdpuml) | [SaveObjectsResultsToTXT.cs](../../PostProc/IO/ResultExport/SaveObjectsResultsToTXT.cs) |
| `PostProc.ScenePresenter.LineGradientsCreator` | [PostProc-4](PostProc-4.mdpuml) | [LineGradientsCreator.cs](../../PostProc/ScenePresenter/LineGradientsCreator.cs) |
| `PostProc.ScenePresenter.ResultFigure` | [PostProc-4](PostProc-4.mdpuml) | [ResultFigure.cs](../../PostProc/ScenePresenter/ResultFigure.cs) |
| `PostProc.ScenePresenter.ResultItem` | [PostProc-4](PostProc-4.mdpuml) | [ResultItem.cs](../../PostProc/ScenePresenter/ResultItem.cs) |
| `PostProc.ScenePresenter.ResultSurface` | [PostProc-4](PostProc-4.mdpuml) | [ResultSurface.cs](../../PostProc/ScenePresenter/ResultSurface.cs) |
| `PostProc.ScenePresenter.ResultsFieldsCreator` | [PostProc-5](PostProc-5.mdpuml) | [ResultsFieldsCreator.cs](../../PostProc/ScenePresenter/ResultsFieldsCreator.cs) |
| `PostProc.ScenePresenter.SurfaceGradientsCreator` | [PostProc-5](PostProc-5.mdpuml) | [SurfaceGradientsCreator.cs](../../PostProc/ScenePresenter/SurfaceGradientsCreator.cs) |
| `PostProc.ScenePresenter.SwapValues` | [PostProc-5](PostProc-5.mdpuml) | [SwapValues.cs](../../PostProc/ScenePresenter/SwapValues.cs) |
| `PostProc.ScenePresenter.Interfaces.IGradientsCreator<T>` | [PostProc-2](PostProc-2.mdpuml) | [IGradientsCreator.cs](../../PostProc/ScenePresenter/Interfaces/IGradientsCreator.cs) |
| `PostProc.ScenePresenter.Interfaces.IResultsFieldsCreator` | [PostProc-2](PostProc-2.mdpuml) | [IResultsFieldsCreator.cs](../../PostProc/ScenePresenter/Interfaces/IResultsFieldsCreator.cs) |
| `ProjectInterfaces.Results.IResultsFieldsCreator` | [PostProc-2](PostProc-2.mdpuml) | [IResultsFieldsCreator.cs](../../PostProc/ScenePresenter/IResultsFieldsCreator.cs) |
| `Results.ScenePresenter.FieldCreator` | [PostProc-5](PostProc-5.mdpuml) | [FieldCreator.cs](../../PostProc/ScenePresenter/FieldCreator.cs) |
| `Results.ScenePresenter.IsoLevelsCreator` | [PostProc-5](PostProc-5.mdpuml) | [IsoLevelFieldsCreator.cs](../../PostProc/ScenePresenter/IsoLevelFieldsCreator.cs) |
| `Results.ScenePresenter.ResultPoint` | [PostProc-5](PostProc-5.mdpuml) | [ResultPoint.cs](../../PostProc/ScenePresenter/ResultPoint.cs) |

## Источники и статус

Состав типов и сигнатуры сверены с текущими `.cs` и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.


## Ограничение построения линейного поля

`ResultsFieldsCreator.CreateLineObjects` сейчас выбрасывает `NotImplementedException`.
Схема показывает существующий метод и связи, но не подтверждает готовую функциональность.
Источник: [ResultsFieldsCreator.cs](../ScenePresenter/ResultsFieldsCreator.cs).
