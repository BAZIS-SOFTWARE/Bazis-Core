# ResultDB

Предоставляет результаты, доступ к SQLite и операции чтения/сохранения. Отдельный `ResultController` из прежней схемы не реализован. Целевая библиотека результатов не объявляет событий; инициирующий изменения операционный контроллер отвечает за внешние уведомления.

[Обзорная диаграмма](ResultDB.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [ResultDB-1](ResultDB-1.mdpuml) | Результаты и хранилище; DataTableEx, Result, ResultDataBase |
| [ResultDB-2](ResultDB-2.mdpuml) | Чтение и сохранение; IResultsLoader, IResultsSaver, CSVToDataTableConverter, LoadEventArgs, LoadResultsFileDB, SaveEventArgs |
| [ResultDB-3](ResultDB-3.mdpuml) | Чтение и сохранение; SaveResultsFileDb, TaskKind |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `ResultDB.DataTableEx` | [ResultDB-1](ResultDB-1.mdpuml) | [DataTableEx.cs](../../ResultDataBase/DataTableEx.cs) |
| `ResultDB.Result` | [ResultDB-1](ResultDB-1.mdpuml) | [Result.cs](../../ResultDataBase/Result.cs) |
| `ResultDB.ResultDataBase` | [ResultDB-1](ResultDB-1.mdpuml) | [ResultDataBase.cs](../../ResultDataBase/ResultDataBase.cs) |
| `ResultDB.IO.CSVToDataTableConverter` | [ResultDB-2](ResultDB-2.mdpuml) | [CSVToDataTableConverter.cs](../../ResultDataBase/IO/CSVToDataTableConverter.cs) |
| `ResultDB.IO.LoadEventArgs` | [ResultDB-2](ResultDB-2.mdpuml) | [LoadEventArgs.cs](../../ResultDataBase/IO/LoadEventArgs.cs) |
| `ResultDB.IO.LoadResultsFileDB` | [ResultDB-2](ResultDB-2.mdpuml) | [LoadResultsFileDB.cs](../../ResultDataBase/IO/LoadResultsFileDB.cs) |
| `ResultDB.IO.SaveEventArgs` | [ResultDB-2](ResultDB-2.mdpuml) | [SaveEventArgs.cs](../../ResultDataBase/IO/SaveEventArgs.cs) |
| `ResultDB.IO.SaveResultsFileDb` | [ResultDB-3](ResultDB-3.mdpuml) | [SaveResultsFileDb.cs](../../ResultDataBase/IO/SaveResultsFileDb.cs) |
| `ResultDB.IO.TaskKind` | [ResultDB-3](ResultDB-3.mdpuml) | [LoadResultsFileDB.cs](../../ResultDataBase/IO/LoadResultsFileDB.cs) |
| `ResultDB.Interfaces.IResultsLoader` | [ResultDB-2](ResultDB-2.mdpuml) | [IResultsLoader.cs](../../ResultDataBase/Interfaces/IResultsLoader.cs) |
| `ResultDB.Interfaces.IResultsSaver` | [ResultDB-2](ResultDB-2.mdpuml) | [IResultsSaver.cs](../../ResultDataBase/Interfaces/IResultsSaver.cs) |

## Источники и статус

Состав типов и сигнатуры сверены с текущими `.cs` и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.
