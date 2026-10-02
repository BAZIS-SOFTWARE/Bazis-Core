# Project

Хранит общие сведения проекта, расчётную задачу, условия и параметры физических полей. Условия связаны с группами модели. Целевые загрузчики и сохранители не содержат событий.

Загрузчики проекта и данных задачи сообщают условный процент через необязательный `IProgress<int>`
в [IProjectLoader и ITaskDataLoader](Project-3.md). Их `LoadEvent` и два соответствующих `LoadEventArgs` удалены.
`Project/Results` и `Project.Interfaces.IO.ILoadEventArgs` сохранены как архивные исходники и исключены
из [активной сборки Project](../Project.csproj). Эти архивные типы показаны на схемах для навигации
по исходникам; они не входят в публичный контракт сборки. Действующая библиотека `ResultDataBase`
и её события загрузки результатов остаются отдельным компонентом.

[Обзорная диаграмма](Project.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [Project-1](Project-1.mdpuml) | Проект, задача и граничные условия; IGeneralData, IProject, IProjectData, IResult, IResultData, ICillindricalFunction |
| [Project-2](Project-2.mdpuml) | Параметры и физические поля; CalculationMode, ChemicalConvergence, ChemicalParameters, ConditionReference, ConditionReferenceJsonConverter, GeneralParameters |
| [Project-3](Project-3.mdpuml) | Функции, материалы и ввод/вывод; архивный ILoadEventArgs, IProjectLoader, IProjectSaver, ISaveEventArgs, ITaskDataLoader, ITaskDataSaver |
| [Project-4](Project-4.mdpuml) | Проект, задача и граничные условия; ICondData, IData, IFrameData, IFrameFunction, IGroupDependent, ILocalFrame |
| [Project-5](Project-5.mdpuml) | Проект, задача и граничные условия; ISphereFunction, ITableFunction, ITaskData, ITimeDependentData, ITrajectory, IPowerParamsFunction |
| [Project-6](Project-6.mdpuml) | Проект, задача и граничные условия; GeneralData, ProjectData, DataKind, Direction, TaskKind, TaskType |
| [Project-7](Project-7.mdpuml) | Проект, задача и граничные условия; ClampData, ClampKind, CondData, Data, HeatData, LoadData |
| [Project-8](Project-8.mdpuml) | Проект, задача и граничные условия; LoadKind, MatData, MediaData, MediaType, ObjKind, ObjectData |
| [Project-9](Project-9.mdpuml) | Проект, задача и граничные условия; TaskData |
| [Project-10](Project-10.mdpuml) | Параметры и физические поля; LinearSolverKind, MatrixStorageKind, MechanicalConvergence, MechanicalParameters, PhysicalField, PhysicalFieldName |
| [Project-11](Project-11.mdpuml) | Параметры и физические поля; PhysicalSet, PhysicalSetName, PhysicalSetSource, SolverSettings, SolverSettingsIO, TermalConvergence |
| [Project-12](Project-12.mdpuml) | Параметры и физические поля; TermalParameters, TimeSettings |
| [Project-13](Project-13.mdpuml) | Функции, материалы и ввод/вывод; IResultsExportSaver, IResultsExporter, LoadProjectFromTextFormat, ProgressRange, SaveEventArgs |
| [Project-14](Project-14.mdpuml) | Функции, материалы и ввод/вывод; SaveProjectTextFormat, DataTableEx, Result, ResultDataBase, CSVToDataTableConverter, LoadEventArgs |
| [Project-15](Project-15.mdpuml) | Функции, материалы и ввод/вывод; LoadResultsFileBrfTextFormat, LoadResultsFileDB, SaveEventArgs, SaveResultsFileDb, SaveResultsFileTextFormat, CustomFunction |
| [Project-16](Project-16.mdpuml) | Функции, материалы и ввод/вывод; DHEX, FHEX, FrameFunction, FuncName, FuncType, Function |
| [Project-17](Project-17.mdpuml) | Функции, материалы и ввод/вывод; PT, Parameter, ParameterKind, ParameterType, TT, TableParameter |
| [Project-18](Project-18.mdpuml) | Функции, материалы и ввод/вывод; CIL, CillindricalBuilder, CustomFrameFunction, DEL, EllipsBuilder, FrameFunctionBuilder |
| [Project-19](Project-19.mdpuml) | Функции, материалы и ввод/вывод; SPH, SphereBuilder, FSWPin, FSWPinBuilder, FSWShoulder, FSWShoulderBuilder |
| [Project-20](Project-20.mdpuml) | Функции, материалы и ввод/вывод; LoadTaskDataTextFormat, SaveEventArgs, SaveTaskDataTextFormat, FrameType, LocalFrame |
| [Project-21](Project-21.mdpuml) | Функции, материалы и ввод/вывод; MovedFrame, MovedFrameBuilder, StaticFrame, BeamMatData, PlateMatData, Interpolation |
| [Project-22](Project-22.mdpuml) | Функции, материалы и ввод/вывод; ResultData, ResultsComparer |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `Project.GeneralData` | [Project-6](Project-6.mdpuml) | [GeneralData.cs](../../Project/GeneralData.cs) |
| `Project.ProjectData` | [Project-6](Project-6.mdpuml) | [ProjectData.cs](../../Project/ProjectData.cs) |
| `Project.IO.LoadProjectFromTextFormat` | [Project-13](Project-13.mdpuml) | [LoadProjectFileTextFormat.cs](../../Project/IO/LoadProjectFileTextFormat.cs) |
| `Project.IO.ProgressRange` | [Project-13](Project-13.mdpuml) | [ProgressRange.cs](../../Project/IO/ProgressRange.cs) |
| `Project.IO.SaveEventArgs` | [Project-13](Project-13.mdpuml) | [SaveEventArgs.cs](../../Project/IO/SaveEventArgs.cs) |
| `Project.IO.SaveProjectTextFormat` | [Project-14](Project-14.mdpuml) | [SaveProjectFileTextFormat.cs](../../Project/IO/SaveProjectFileTextFormat.cs) |
| `Project.Interfaces.IGeneralData` | [Project-1](Project-1.mdpuml) | [IGeneralData.cs](../../Project/Interfaces/IGeneralData.cs) |
| `Project.Interfaces.IProject` | [Project-1](Project-1.mdpuml) | [IProject.cs](../../Project/Interfaces/IProject.cs) |
| `Project.Interfaces.IProjectData` | [Project-1](Project-1.mdpuml) | [IProjectData.cs](../../Project/Interfaces/IProjectData.cs) |
| `Project.Interfaces.IResult` | [Project-1](Project-1.mdpuml) | [IResult.cs](../../Project/Interfaces/IResult.cs) |
| `Project.Interfaces.IResultData` | [Project-1](Project-1.mdpuml) | [IResultData.cs](../../Project/Interfaces/IResultData.cs) |
| `Project.Interfaces.IO.ILoadEventArgs` | [Project-3](Project-3.mdpuml) | [ILoadEventArgs.cs](../../Project/Interfaces/IO/ILoadEventArgs.cs) |
| `Project.Interfaces.IO.IProjectLoader` | [Project-3](Project-3.mdpuml) | [IProjectLoader.cs](../../Project/Interfaces/IO/IProjectLoader.cs) |
| `Project.Interfaces.IO.IProjectSaver` | [Project-3](Project-3.mdpuml) | [IProjectSaver.cs](../../Project/Interfaces/IO/IProjectSaver.cs) |
| `Project.Interfaces.IO.ISaveEventArgs` | [Project-3](Project-3.mdpuml) | [ISaveEventArgs.cs](../../Project/Interfaces/IO/ISaveEventArgs.cs) |
| `Project.Interfaces.IO.ITaskDataLoader` | [Project-3](Project-3.mdpuml) | [ITaskDataLoader.cs](../../Project/Interfaces/IO/ITaskDataLoader.cs) |
| `Project.Interfaces.IO.ITaskDataSaver` | [Project-3](Project-3.mdpuml) | [ITaskDataSaver.cs](../../Project/Interfaces/IO/ITaskDataSaver.cs) |
| `Project.Interfaces.Results.IResultsExportSaver` | [Project-13](Project-13.mdpuml) | [IResultsExportSaver.cs](../../Project/Interfaces/Results/IResultsExportSaver.cs) |
| `Project.Interfaces.Results.IResultsExporter` | [Project-13](Project-13.mdpuml) | [IResultsExporter.cs](../../Project/Interfaces/Results/IResultsExporter.cs) |
| `Project.Interfaces.Tasks.DataKind` | [Project-6](Project-6.mdpuml) | [IData.cs](../../Project/Interfaces/Tasks/IData.cs) |
| `Project.Interfaces.Tasks.Direction` | [Project-6](Project-6.mdpuml) | [ICondData.cs](../../Project/Interfaces/Tasks/ICondData.cs) |
| `Project.Interfaces.Tasks.ICillindricalFunction` | [Project-1](Project-1.mdpuml) | [ICillindricalFunction.cs](../../Project/Interfaces/Tasks/ICillindricalFunction.cs) |
| `Project.Interfaces.Tasks.ICondData` | [Project-4](Project-4.mdpuml) | [ICondData.cs](../../Project/Interfaces/Tasks/ICondData.cs) |
| `Project.Interfaces.Tasks.IData` | [Project-4](Project-4.mdpuml) | [IData.cs](../../Project/Interfaces/Tasks/IData.cs) |
| `Project.Interfaces.Tasks.IFrameData` | [Project-4](Project-4.mdpuml) | [IFrameData.cs](../../Project/Interfaces/Tasks/IFrameData.cs) |
| `Project.Interfaces.Tasks.IFrameFunction` | [Project-4](Project-4.mdpuml) | [IFrameFunction.cs](../../Project/Interfaces/Tasks/IFrameFunction.cs) |
| `Project.Interfaces.Tasks.IGroupDependent` | [Project-4](Project-4.mdpuml) | [IGroupDependent.cs](../../Project/Interfaces/Tasks/IGroupDependent.cs) |
| `Project.Interfaces.Tasks.ILocalFrame` | [Project-4](Project-4.mdpuml) | [ILocalFrame.cs](../../Project/Interfaces/Tasks/ILocalFrame.cs) |
| `Project.Interfaces.Tasks.ISphereFunction` | [Project-5](Project-5.mdpuml) | [ISphereFunction.cs](../../Project/Interfaces/Tasks/ISphereFunction.cs) |
| `Project.Interfaces.Tasks.ITableFunction` | [Project-5](Project-5.mdpuml) | [ITableFunction.cs](../../Project/Interfaces/Tasks/ITableFunction.cs) |
| `Project.Interfaces.Tasks.ITaskData` | [Project-5](Project-5.mdpuml) | [ITaskData.cs](../../Project/Interfaces/Tasks/ITaskData.cs) |
| `Project.Interfaces.Tasks.ITimeDependentData` | [Project-5](Project-5.mdpuml) | [ITimeDependentData.cs](../../Project/Interfaces/Tasks/ITimeDependentData.cs) |
| `Project.Interfaces.Tasks.ITrajectory` | [Project-5](Project-5.mdpuml) | [ITrajectory.cs](../../Project/Interfaces/Tasks/ITrajectory.cs) |
| `Project.Interfaces.Tasks.TaskKind` | [Project-6](Project-6.mdpuml) | [ITaskData.cs](../../Project/Interfaces/Tasks/ITaskData.cs) |
| `Project.Interfaces.Tasks.TaskType` | [Project-6](Project-6.mdpuml) | [ITaskData.cs](../../Project/Interfaces/Tasks/ITaskData.cs) |
| `Project.Results.DataTableEx` | [Project-14](Project-14.mdpuml) | [DataTableEx.cs](../../Project/Results/DataTableEx.cs) |
| `Project.Results.Result` | [Project-14](Project-14.mdpuml) | [Result.cs](../../Project/Results/Result.cs) |
| `Project.Results.ResultDataBase` | [Project-14](Project-14.mdpuml) | [ResultDataBase.cs](../../Project/Results/ResultDataBase.cs) |
| `Project.Results.IO.CSVToDataTableConverter` | [Project-14](Project-14.mdpuml) | [CSVToDataTableConverter.cs](../../Project/Results/IO/CSVToDataTableConverter.cs) |
| `Project.Results.IO.LoadEventArgs` | [Project-14](Project-14.mdpuml) | [LoadEventArgs.cs](../../Project/Results/IO/LoadEventArgs.cs) |
| `Project.Results.IO.LoadResultsFileBrfTextFormat` | [Project-15](Project-15.mdpuml) | [LoadResultsFileBrfTextFormat.cs](../../Project/Results/IO/LoadResultsFileBrfTextFormat.cs) |
| `Project.Results.IO.LoadResultsFileDB` | [Project-15](Project-15.mdpuml) | [LoadResultsFileDB.cs](../../Project/Results/IO/LoadResultsFileDB.cs) |
| `Project.Results.IO.SaveEventArgs` | [Project-15](Project-15.mdpuml) | [SaveEventArgs.cs](../../Project/Results/IO/SaveEventArgs.cs) |
| `Project.Results.IO.SaveResultsFileDb` | [Project-15](Project-15.mdpuml) | [SaveResultsFileDb.cs](../../Project/Results/IO/SaveResultsFileDb.cs) |
| `Project.Results.IO.SaveResultsFileTextFormat` | [Project-15](Project-15.mdpuml) | [SaveResultsFileTextFormat.cs](../../Project/Results/IO/SaveResultsFileTextFormat.cs) |
| `Project.TaskParameters.CalculationMode` | [Project-2](Project-2.mdpuml) | [GeneralParameters.cs](../../Project/TaskParameters/GeneralParameters.cs) |
| `Project.TaskParameters.ChemicalConvergence` | [Project-2](Project-2.mdpuml) | [ChemicalConvergence.cs](../../Project/TaskParameters/ChemicalConvergence.cs) |
| `Project.TaskParameters.ChemicalParameters` | [Project-2](Project-2.mdpuml) | [ChemicalParameters.cs](../../Project/TaskParameters/ChemicalParameters.cs) |
| `Project.TaskParameters.ConditionReference` | [Project-2](Project-2.mdpuml) | [ConditionReference.cs](../../Project/TaskParameters/ConditionReference.cs) |
| `Project.TaskParameters.ConditionReferenceJsonConverter` | [Project-2](Project-2.mdpuml) | [ConditionReference.cs](../../Project/TaskParameters/ConditionReference.cs) |
| `Project.TaskParameters.GeneralParameters` | [Project-2](Project-2.mdpuml) | [GeneralParameters.cs](../../Project/TaskParameters/GeneralParameters.cs) |
| `Project.TaskParameters.LinearSolverKind` | [Project-10](Project-10.mdpuml) | [SolverSettings.cs](../../Project/TaskParameters/SolverSettings.cs) |
| `Project.TaskParameters.MatrixStorageKind` | [Project-10](Project-10.mdpuml) | [SolverSettings.cs](../../Project/TaskParameters/SolverSettings.cs) |
| `Project.TaskParameters.MechanicalConvergence` | [Project-10](Project-10.mdpuml) | [MechanicalConvergence.cs](../../Project/TaskParameters/MechanicalConvergence.cs) |
| `Project.TaskParameters.MechanicalParameters` | [Project-10](Project-10.mdpuml) | [MechanicalParameters.cs](../../Project/TaskParameters/MechanicalParameters.cs) |
| `Project.TaskParameters.PhysicalField` | [Project-10](Project-10.mdpuml) | [PhysicalSet.cs](../../Project/TaskParameters/PhysicalSet.cs) |
| `Project.TaskParameters.PhysicalFieldName` | [Project-10](Project-10.mdpuml) | [PhysicalSet.cs](../../Project/TaskParameters/PhysicalSet.cs) |
| `Project.TaskParameters.PhysicalSet` | [Project-11](Project-11.mdpuml) | [PhysicalSet.cs](../../Project/TaskParameters/PhysicalSet.cs) |
| `Project.TaskParameters.PhysicalSetName` | [Project-11](Project-11.mdpuml) | [PhysicalSet.cs](../../Project/TaskParameters/PhysicalSet.cs) |
| `Project.TaskParameters.PhysicalSetSource` | [Project-11](Project-11.mdpuml) | [PhysicalSet.cs](../../Project/TaskParameters/PhysicalSet.cs) |
| `Project.TaskParameters.SolverSettings` | [Project-11](Project-11.mdpuml) | [SolverSettings.cs](../../Project/TaskParameters/SolverSettings.cs) |
| `Project.TaskParameters.SolverSettingsIO` | [Project-11](Project-11.mdpuml) | [SolverSettingsIO.cs](../../Project/TaskParameters/SolverSettingsIO.cs) |
| `Project.TaskParameters.TermalConvergence` | [Project-11](Project-11.mdpuml) | [TermalConvergence.cs](../../Project/TaskParameters/TermalConvergence.cs) |
| `Project.TaskParameters.TermalParameters` | [Project-12](Project-12.mdpuml) | [TermalParameters.cs](../../Project/TaskParameters/TermalParameters.cs) |
| `Project.TaskParameters.TimeSettings` | [Project-12](Project-12.mdpuml) | [TimeSettings.cs](../../Project/TaskParameters/TimeSettings.cs) |
| `Project.Tasks.ClampData` | [Project-7](Project-7.mdpuml) | [ClampData.cs](../../Project/Tasks/ClampData.cs) |
| `Project.Tasks.ClampKind` | [Project-7](Project-7.mdpuml) | [ClampData.cs](../../Project/Tasks/ClampData.cs) |
| `Project.Tasks.CondData` | [Project-7](Project-7.mdpuml) | [CondData.cs](../../Project/Tasks/CondData.cs) |
| `Project.Tasks.Data` | [Project-7](Project-7.mdpuml) | [Data.cs](../../Project/Tasks/Data.cs) |
| `Project.Tasks.HeatData` | [Project-7](Project-7.mdpuml) | [HeatData.cs](../../Project/Tasks/HeatData.cs) |
| `Project.Tasks.LoadData` | [Project-7](Project-7.mdpuml) | [LoadData.cs](../../Project/Tasks/LoadData.cs) |
| `Project.Tasks.LoadKind` | [Project-8](Project-8.mdpuml) | [LoadData.cs](../../Project/Tasks/LoadData.cs) |
| `Project.Tasks.MatData` | [Project-8](Project-8.mdpuml) | [MatData.cs](../../Project/Tasks/MatData.cs) |
| `Project.Tasks.MediaData` | [Project-8](Project-8.mdpuml) | [MediaData.cs](../../Project/Tasks/MediaData.cs) |
| `Project.Tasks.MediaType` | [Project-8](Project-8.mdpuml) | [MediaData.cs](../../Project/Tasks/MediaData.cs) |
| `Project.Tasks.ObjKind` | [Project-8](Project-8.mdpuml) | [MatData.cs](../../Project/Tasks/MatData.cs) |
| `Project.Tasks.ObjectData` | [Project-8](Project-8.mdpuml) | [ObjectData.cs](../../Project/Tasks/ObjectData.cs) |
| `Project.Tasks.TaskData` | [Project-9](Project-9.mdpuml) | [TaskData.cs](../../Project/Tasks/TaskData.cs) |
| `Project.Tasks.Functions.CustomFunction` | [Project-15](Project-15.mdpuml) | [CustomFunction.cs](../../Project/Tasks/Functions/CustomFunction.cs) |
| `Project.Tasks.Functions.DHEX` | [Project-16](Project-16.mdpuml) | [DHEX.cs](../../Project/Tasks/Functions/DHEX.cs) |
| `Project.Tasks.Functions.FHEX` | [Project-16](Project-16.mdpuml) | [FHEX.cs](../../Project/Tasks/Functions/FHEX.cs) |
| `Project.Tasks.Functions.FrameFunction` | [Project-16](Project-16.mdpuml) | [FrameFunction.cs](../../Project/Tasks/Functions/FrameFunction.cs) |
| `Project.Tasks.Functions.FuncName` | [Project-16](Project-16.mdpuml) | [Function.cs](../../Project/Tasks/Functions/Function.cs) |
| `Project.Tasks.Functions.FuncType` | [Project-16](Project-16.mdpuml) | [Function.cs](../../Project/Tasks/Functions/Function.cs) |
| `Project.Tasks.Functions.Function` | [Project-16](Project-16.mdpuml) | [Function.cs](../../Project/Tasks/Functions/Function.cs) |
| `Project.Tasks.Functions.PT` | [Project-17](Project-17.mdpuml) | [PT.cs](../../Project/Tasks/Functions/PT.cs) |
| `Project.Tasks.Functions.Parameter` | [Project-17](Project-17.mdpuml) | [Parameter.cs](../../Project/Tasks/Functions/Parameter.cs) |
| `Project.Tasks.Functions.ParameterKind` | [Project-17](Project-17.mdpuml) | [Parameter.cs](../../Project/Tasks/Functions/Parameter.cs) |
| `Project.Tasks.Functions.ParameterType` | [Project-17](Project-17.mdpuml) | [Parameter.cs](../../Project/Tasks/Functions/Parameter.cs) |
| `Project.Tasks.Functions.TT` | [Project-17](Project-17.mdpuml) | [TT.cs](../../Project/Tasks/Functions/TT.cs) |
| `Project.Tasks.Functions.TableParameter` | [Project-17](Project-17.mdpuml) | [TableParameter.cs](../../Project/Tasks/Functions/TableParameter.cs) |
| `Project.Tasks.Functions.FrameFunctions.CIL` | [Project-18](Project-18.mdpuml) | [CIL.cs](../../Project/Tasks/Functions/FrameFunctions/CIL.cs) |
| `Project.Tasks.Functions.FrameFunctions.CillindricalBuilder` | [Project-18](Project-18.mdpuml) | [CillindricalBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/CillindricalBuilder.cs) |
| `Project.Tasks.Functions.FrameFunctions.CustomFrameFunction` | [Project-18](Project-18.mdpuml) | [CustomFrameFunction.cs](../../Project/Tasks/Functions/FrameFunctions/CustomFrameFunction.cs) |
| `Project.Tasks.Functions.FrameFunctions.DEL` | [Project-18](Project-18.mdpuml) | [DEL.cs](../../Project/Tasks/Functions/FrameFunctions/DEL.cs) |
| `Project.Tasks.Functions.FrameFunctions.EllipsBuilder` | [Project-18](Project-18.mdpuml) | [EllipsBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/EllipsBuilder.cs) |
| `Project.Tasks.Functions.FrameFunctions.FrameFunctionBuilder` | [Project-18](Project-18.mdpuml) | [FrameFunctionBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/FrameFunctionBuilder.cs) |
| `Project.Tasks.Functions.FrameFunctions.SPH` | [Project-19](Project-19.mdpuml) | [SPH.cs](../../Project/Tasks/Functions/FrameFunctions/SPH.cs) |
| `Project.Tasks.Functions.FrameFunctions.SphereBuilder` | [Project-19](Project-19.mdpuml) | [SphereBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/SphereBuilder.cs) |
| `Project.Tasks.Functions.Welding.FSWPin` | [Project-19](Project-19.mdpuml) | [FSWPin.cs](../../Project/Tasks/Functions/FrameFunctions/FSWPin.cs) |
| `Project.Tasks.Functions.Welding.FSWPinBuilder` | [Project-19](Project-19.mdpuml) | [FSWPinBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/FSWPinBuilder.cs) |
| `Project.Tasks.Functions.Welding.FSWShoulder` | [Project-19](Project-19.mdpuml) | [FSWShoulder.cs](../../Project/Tasks/Functions/FrameFunctions/FSWShoulder.cs) |
| `Project.Tasks.Functions.Welding.FSWShoulderBuilder` | [Project-19](Project-19.mdpuml) | [FSWShoulderBuilder.cs](../../Project/Tasks/Functions/FrameFunctions/FSWShoulderBuilder.cs) |
| `Project.Tasks.IO.LoadTaskDataTextFormat` | [Project-20](Project-20.mdpuml) | [LoadTaskDataTextFormat.cs](../../Project/Tasks/IO/LoadTaskDataTextFormat.cs) |
| `Project.Tasks.IO.SaveEventArgs` | [Project-20](Project-20.mdpuml) | [SaveEventArgs.cs](../../Project/Tasks/IO/SaveEventArgs.cs) |
| `Project.Tasks.IO.SaveTaskDataTextFormat` | [Project-20](Project-20.mdpuml) | [SaveTaskDataTextFormat.cs](../../Project/Tasks/IO/SaveTaskDataTextFormat.cs) |
| `Project.Tasks.LocalFrames.FrameType` | [Project-20](Project-20.mdpuml) | [LocalFrame.cs](../../Project/Tasks/LocalFrames/LocalFrame.cs) |
| `Project.Tasks.LocalFrames.LocalFrame` | [Project-20](Project-20.mdpuml) | [LocalFrame.cs](../../Project/Tasks/LocalFrames/LocalFrame.cs) |
| `Project.Tasks.LocalFrames.MovedFrame` | [Project-21](Project-21.mdpuml) | [MovedFrame.cs](../../Project/Tasks/LocalFrames/MovedFrame.cs) |
| `Project.Tasks.LocalFrames.MovedFrameBuilder` | [Project-21](Project-21.mdpuml) | [MovedFrameBuilder.cs](../../Project/Tasks/LocalFrames/MovedFrameBuilder.cs) |
| `Project.Tasks.LocalFrames.StaticFrame` | [Project-21](Project-21.mdpuml) | [StaticFrame.cs](../../Project/Tasks/LocalFrames/StaticFrame.cs) |
| `Project.Tasks.Materials.BeamMatData` | [Project-21](Project-21.mdpuml) | [BeamMatData.cs](../../Project/Tasks/Materials/BeamMatData.cs) |
| `Project.Tasks.Materials.PlateMatData` | [Project-21](Project-21.mdpuml) | [PlateMatData.cs](../../Project/Tasks/Materials/PlateMatData.cs) |
| `Project.Tasks.Utilities.Interpolation` | [Project-21](Project-21.mdpuml) | [Interpolation.cs](../../Project/Tasks/Utilities/Interpolation.cs) |
| `ProjectInterfaces.Tasks.IPowerParamsFunction` | [Project-5](Project-5.mdpuml) | [IPowerParamsFunction.cs](../../Project/Interfaces/Tasks/IPowerParamsFunction.cs) |
| `Results.ResultsData.ResultData` | [Project-22](Project-22.mdpuml) | [ResultData.cs](../../Project/Results/ResultData.cs) |
| `Results.ResultsData.ResultsComparer` | [Project-22](Project-22.mdpuml) | [ResultsComparer.cs](../../Project/Results/ResultsComparer.cs) |

## Физические поля

[Общий контракт физических полей](../../Documents/PhysicalFields.md) описывает наборы, источники и формат значений. Правила заданы в [AGENTS.md](../../AGENTS.md). Реализованные типы хранения: [GeneralParameters](../../Project/TaskParameters/GeneralParameters.cs) и [PhysicalSet](../../Project/TaskParameters/PhysicalSet.cs). Поддержка каждой проверки формата не предполагается только на основании наличия этих DTO.

## Источники и статус

Состав типов и сигнатуры сверены с текущими `.cs` и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.
