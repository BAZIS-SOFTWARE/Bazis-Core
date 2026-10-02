# Расхождения реализации и целевой архитектуры

Основание: [AGENTS.md](../AGENTS.md), [инструкции архитектора](../.codex/agents/core-architect.md) и согласованное требование пользователя: события объявляются и вызываются только операционным контроллером. Обзор — [Architecture.md](Architecture.md). Демонтаж событий загрузки сетки, проекта и данных задачи выполнен 3 октября 2026 года; остальные участки миграции перечислены ниже. Отдельный откат делегатов уведомления из `Append` уже внесён в реализацию и документацию.

## Необходимые изменения реализации

| ID | Текущее состояние и источник | Целевое состояние | Необходимое изменение |
| --- | --- | --- | --- |
| EVT-01 | `LoadEvent` удалён из контрактов и загрузчиков `Model`; остаются события сохранения: [IModelSaver](../Model/Interfaces/IModelSaver.cs), [ISaver](../Model/Interfaces/ISaver.cs). | В библиотеке модели событий нет. | Демонтаж событий сохранения остаётся отдельной задачей. [IModelLoader](../Model/Interfaces/IModelLoader.cs) сохраняет необязательный `IProgress<int>`; изменения модели публикует `ProjectController`. |
| EVT-02 | `LoadEvent` и ретрансляция удалены из загрузчиков проекта и данных задачи. Их интерфейсы принимают необязательный `IProgress<int>`. События сохранения остаются. | Проект, задача и их IO не имеют событий. | Демонтаж событий сохранения остаётся отдельной задачей; завершение изменения состояния сообщает контроллер. [IProjectLoader](../Project/Interfaces/IO/IProjectLoader.cs), [ITaskDataLoader](../Project/Interfaces/IO/ITaskDataLoader.cs). |
| EVT-03 | Сохранители поверхностной сетки в `PostProc` объявляют `SaveEvent`: [IGridSurfacesSaver](../PostProc/IO/GridExport/IGridSurfacesSaver.cs), [SaveGridSurfacesToTxtSTL](../PostProc/IO/GridExport/SaveGridSurfacesToTxtSTL.cs). | Экспортёры не публикуют событий. | Удалить объявления и вызовы `SaveEvent`, обновить потребителей. Успех определяется завершением вызова, ошибка — исключением; экспорт сам по себе не требует сообщения об изменении проекта. |
| EVT-04 | В `ResultDB` есть события загрузки и сохранения: [IResultsLoader](../ResultDataBase/Interfaces/IResultsLoader.cs), [IResultsSaver](../ResultDataBase/Interfaces/IResultsSaver.cs), [LoadResultsFileDB](../ResultDataBase/IO/LoadResultsFileDB.cs). | Библиотека результатов не объявляет событий. | Удалить события IO. Если изменение активных результатов станет операцией контроллера, отдельно определить его команды и данные уведомления; не создавать второй источник событий в `ResultDB`. |
| EVT-05 | `ModelView` и внутренний `IModelView` объявляют `Changed`; контроллер подписывается и преобразует его в `ViewChange`: [IModelView](../OperationalController/Interfaces/IModelView.cs), [ModelView](../OperationalController/ModelView/ModelView.cs), [ProjectSession](../OperationalController/Methods/ProjectSession.cs). | Только `ProjectController` является источником событий, включая изменения представления. | Удалить `Changed`, подписки и `OnViewChanged`. Передавать контроллеру данные выполненной команды/пакета без событий и постоянных подписок. Сохранить сведения по каждому набору, отсутствие уведомления при отсутствии изменения и объединение внутри `BeginViewUpdate`. Конкретные сигнатуры внутренней передачи результата требуют отдельного проектирования. |
| PHY-01 | `PhysicalSetName` содержит `Hydrodynamic`, но `TaskKind` и `PreProc.GetNativeFieldSet` поддерживают только тепловую, механическую и химическую задачи: [PhysicalSet](../Project/TaskParameters/PhysicalSet.cs), [ITaskData](../Project/Interfaces/Tasks/ITaskData.cs), [PreProc](../PreProc/PreProc.cs). | Интеграционные правила описывают четыре вида задач. | Для полноценной гидродинамической задачи отдельно расширить контракты задачи и путь формирования инструкций. Наличие имени набора не считать готовой поддержкой этой задачи. |

Удаление объявлений — изменение библиотечных контрактов. До реализации требуется найти потребителей и подписки, включая клиентские приложения. Данная ревизия не проверяет совместимость этих клиентов.

## Соответствующие правилу участки

`IModelData`, `IObjectsData`, `IGroupData`, `IObjectsSet` и `IObjectsCollection` уже не объявляют событий изменения объектов, наборов или групп. По решению пользователя удалены перегрузки с делегатами у `ModelData.Append`, `ObjectsCollection.Append` и `ObjectsSet.Append`. Контроллер сообщает результаты успешного добавления через `PublishAppendedModel` и `PublishSetChange`, по одному сообщению на затронутый набор или добавленную группу. Ошибки нумерации выходят исключениями. `ObjectsSet.Renumber` возвращает `bool`, а `MarkSurfaces.Mark` — уникальных изменённых владельцев через `out`. `IObjectsMerger.Merge` возвращает изменённые элементы и признак удаления узлов. Делегаты уведомления удалены из этих операций; сообщения формирует контроллер после обхода данных. Единственный вызов внешнего `Message` находится в [ProjectSession.cs](../OperationalController/Methods/ProjectSession.cs).

Источники: [IModelData](../Model/Interfaces/IModelData.cs), [IObjectsSet](../Model/Interfaces/ObjectsCollections/IObjectsSet.cs), [IObjectsCollection](../Model/Interfaces/ObjectsCollections/IObjectsCollection.cs), [ModelData](../Model/ModelData.cs), [MarkSurfaces](../Model/Utilities/MarkSurfaces.cs), [PublishChanges](../OperationalController/Methods/PublishChanges.cs). Сведения об ошибке не требуют делегата; вызовы уведомления из горячих циклов `Append` исключены.

## Исправленные предложения документации

Прежняя схема `ResultController.mdpuml` предлагала несуществующий `ResultController` и его события `ResultsReplaced`/`CurrentChanged`. Это предложение отменено: обновлённая схема показывает библиотеку результатов без событий. Она не утверждает, что управление активными результатами уже добавлено в `IProjectController`.

В активных диаграммах событий нижних библиотек и `ModelView.Changed` нет. Исторические заметки о прежнем устройстве представления сохранены в явно обозначенном архивном разделе [ModelView.md](../OperationalController/Documents/ModelView.md). Старые изображения не используются как подтверждение актуального контракта.

## Ограничения, не являющиеся изменениями событий

| Наблюдение | Что отражено в документации |
| --- | --- |
| [PostProcController](../PostProc/PostProcController.cs) не объявляет реализацию [IPostProcController](../PostProc/IPostProcController.cs). | Связь реализации на диаграмме не добавлена. Решение о согласовании этих типов требует отдельной задачи. |
| [ResultsFieldsCreator.CreateLineObjects](../PostProc/ScenePresenter/ResultsFieldsCreator.cs) выбрасывает `NotImplementedException`. | Наличие метода не представлено как готовое построение линейного поля. |
| Комментарии `PhysicalSet.Values` описывают элементные группы `MatData`, а правило допускает также узловые группы. | [Контракт полей](PhysicalFields.md) допускает оба вида групп; сам строковый ключ DTO не подтверждает поддержку их ввода и разрешения в потребителях. |
| В просмотренных DTO нет полного валидатора строкового формата физических полей. | Число компонент, одиночный `0`, принадлежность полей наборам и разрешение групп описаны как нормативные требования, реализация которых должна проверяться отдельно. |
| `PreProc.GetParameters` явно задаёт `T = 20` для групп `MatData`. | Это явно заданное начальное условие, а не доказательство нарушения правила нулевого значения отсутствующего поля. |

## Критерии завершения миграции событий

После отдельного разрешения на реализацию должны быть удалены все объявления событий из нижних библиотек и `ModelView`, а постоянные подписки между ними — заменены передачей результатов операций. `Message` остаётся в `IProjectEvents` и реализующем `ProjectController`; вызывает его только контроллер.

Проверка поведения должна охватывать геометрию, наборы, группы, каскадное удаление условий, изменение связности без изменения количества объектов, частично выполненную операцию, замену проекта и завершение пакета представления. Повторная команда без фактического изменения не создаёт уведомления; пакет сохраняет точные изменения каждого набора. Поддерживаемый процент чтения не должен исчезнуть при удалении событий IO.


## Полный перечень оставшихся объявлений событий

Список отражает оставшиеся объявления нетестовых проектов; строки `Project/Results` относятся к архивным исходникам, исключённым из `Project.csproj`. Эти объявления остаются в коде; целевые диаграммы их исключают.

| Проект | Объявление | Источник |
| --- | --- | --- |
| Model | `IModelSaver.SaveEvent` | [IModelSaver.cs](../Model/Interfaces/IModelSaver.cs) |
| Model | `ISaver.SaveEvent` | [ISaver.cs](../Model/Interfaces/ISaver.cs) |
| Model | `SaveModelToASCIITextFile.SaveEvent` | [SaveModelToASCIITextFile.cs](../Model/IO/SaveModelToASCIITextFile.cs) |
| Model | `SaveModelToBPF2TextFile.SaveEvent` | [SaveModelToBPF2TextFile.cs](../Model/IO/SaveModelToBPF2TextFile.cs) |
| Model | `SaveModelToBPFTextFile.SaveEvent` | [SaveModelToBPFTextFile.cs](../Model/IO/SaveModelToBPFTextFile.cs) |
| Model | `SaveModelToBinarySTLFile.SaveEvent` | [SaveToTxtSTLFile.cs](../Model/IO/STL/SaveToTxtSTLFile.cs) |
| Model | `SaveToTxtSTLFile.SaveEvent` | [SaveToTxtSTLFile.cs](../Model/IO/STL/SaveToTxtSTLFile.cs) |
| OperationalController | `IModelView.Changed` | [IModelView.cs](../OperationalController/Interfaces/IModelView.cs) |
| OperationalController | `ModelView.Changed` | [ModelView.cs](../OperationalController/ModelView/ModelView.cs) |
| PostProc | `IGridSurfacesSaver.SaveEvent` | [IGridSurfacesSaver.cs](../PostProc/IO/GridExport/IGridSurfacesSaver.cs) |
| PostProc | `SaveGridSurfacesToBPF.SaveEvent` | [SaveGridSurfacesToBPF.cs](../PostProc/IO/GridExport/SaveGridSurfacesToBPF.cs) |
| PostProc | `SaveGridSurfacesToBinSTL.SaveEvent` | [SaveGridSurfacesToBinSTL.cs](../PostProc/IO/GridExport/SaveGridSurfacesToBinSTL.cs) |
| PostProc | `SaveGridSurfacesToSTLBase.SaveEvent` | [SaveGridSurfacesToSTLBase.cs](../PostProc/IO/GridExport/SaveGridSurfacesToSTLBase.cs) |
| PostProc | `SaveGridSurfacesToTxtSTL.SaveEvent` | [SaveGridSurfacesToTxtSTL.cs](../PostProc/IO/GridExport/SaveGridSurfacesToTxtSTL.cs) |
| Project | `IProjectSaver.SaveEvent` | [IProjectSaver.cs](../Project/Interfaces/IO/IProjectSaver.cs) |
| Project | `IResultsExportSaver.SaveEvent` | [IResultsExportSaver.cs](../Project/Interfaces/Results/IResultsExportSaver.cs) |
| Project | `ITaskDataSaver.SaveEvent` | [ITaskDataSaver.cs](../Project/Interfaces/IO/ITaskDataSaver.cs) |
| Project | `LoadResultsFileBrfTextFormat.LoadEvent` | [LoadResultsFileBrfTextFormat.cs](../Project/Results/IO/LoadResultsFileBrfTextFormat.cs) |
| Project | `LoadResultsFileDB.LoadEvent` | [LoadResultsFileDB.cs](../Project/Results/IO/LoadResultsFileDB.cs) |
| Project | `SaveProjectTextFormat.SaveEvent` | [SaveProjectFileTextFormat.cs](../Project/IO/SaveProjectFileTextFormat.cs) |
| Project | `SaveResultsFileDb.SaveEvent` | [SaveResultsFileDb.cs](../Project/Results/IO/SaveResultsFileDb.cs) |
| Project | `SaveResultsFileTextFormat.SaveEvent` | [SaveResultsFileTextFormat.cs](../Project/Results/IO/SaveResultsFileTextFormat.cs) |
| Project | `SaveTaskDataTextFormat.SaveEvent` | [SaveTaskDataTextFormat.cs](../Project/Tasks/IO/SaveTaskDataTextFormat.cs) |
| ResultDB | `IResultsLoader.LoadEvent` | [IResultsLoader.cs](../ResultDataBase/Interfaces/IResultsLoader.cs) |
| ResultDB | `IResultsSaver.SaveEvent` | [IResultsSaver.cs](../ResultDataBase/Interfaces/IResultsSaver.cs) |
| ResultDB | `LoadResultsFileDB.LoadEvent` | [LoadResultsFileDB.cs](../ResultDataBase/IO/LoadResultsFileDB.cs) |
| ResultDB | `SaveResultsFileDb.SaveEvent` | [SaveResultsFileDb.cs](../ResultDataBase/IO/SaveResultsFileDb.cs) |

## Исправленные ошибки уведомлений в нагруженных циклах

Закрыты нарушения для `MoveMesh`, `RotateMesh`, `ChangeMeshSetOrder`, `RenumberElements`, `MergeNodes`, `MarkModelSurfaces`, массового добавления узлов/элементов, построения границ и удаления объектов. Делегаты `Renumber`, `MarkSurfaces.Mark` и `IObjectsMerger.Merge` заменены результатами операций. Сведения о гранях объединяются с прочими изменениями набора до вызова `Message`. Очистка зависимостей при массовом удалении перенесена за цикл выбранных объектов, рекурсивное удаление геометрии публикует уникальные затронутые наборы.

Сигнатуры: [Model-18](../Model/Documents/Model-18.mdpuml), [Model-20](../Model/Documents/Model-20.mdpuml), [IObjectsMerger](../OperationalController/Documents/OperationalController-23.mdpuml), [операции контроллера](../OperationalController/Documents/OperationalController-3.mdpuml). Границы публикации и сохранённый порядок сообщений разных групп/условий описаны в [ProjectController](../OperationalController/Documents/ProjectController.md). Постоянных подписок и нового общего накопителя событий не добавлено. Миграция оставшихся событий сохранения, загрузки результатов и `ModelView` остаётся открытой.
