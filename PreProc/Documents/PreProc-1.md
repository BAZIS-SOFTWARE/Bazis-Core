# PreProc-1 — API подготовки параметров

[Диаграмма](PreProc-1.mdpuml) · [Обзор](PreProc.md) · [Очередь и имена](PreProc-2.md) · [TCF и пути](PreProc-3.md).

## IPreProc

Пространство имён `PreProc.Interfaces`. [Исходник](../Interfaces/IPreProc.cs).

```text
CalcCompDataV2(IEnumerable<ICondData> data, TaskKind taskKinds, bool synchronizeTimeSteps = false) : IReadOnlyList<GeneralParameters>
WriteCommandFile(string projectPath, IEnumerable<string> queueRows) : string
SetDraftParameters<ResultParam>(ResultParam parameter) : void where ResultParam : GeneralParameters, new()
```

`CalcCompDataV2` материализует параметры и возвращает доступный только для чтения список ссылок на `GeneralParameters`. Объекты параметров остаются изменяемыми. Метод присваивает индекс каждому интервалу, устанавливает относительные ссылки на ожидаемые DB и возвращает задачи в порядке числового индекса, с теплом перед механикой. TSF и TCF не записывает.

`WriteCommandFile` делегирует запись TcfWriter. Клиент вызывает её отдельно по актуальной очереди при «Старт». Входная строка имеет вид `термическая InputData/термическая_1_0_0.25.tsf выполнить`; путь отсчитывается от корня комплекта. Возвращается путь ComputationData/computation.tcf; порядок и статусы сохраняются, TSF не изменяются.

Запись TSF в InputData относится к клиенту Gerbera. PreProc подготавливает параметры и не сериализует их. Для открытия готового комплекта клиент читает TCF, который задаёт порядок TSF и статусы; PreProc не восстанавливает очередь по каталогу TSF.

## Классы и параметры

`PreProc.PreProc` реализует IPreProc; на диаграмме показан только собственный публичный метод. [Исходник](../PreProc.cs).

```text
GetParameters<LoadData, BoundData, ResultParam>(IEnumerable<ICondData> data) : List<ResultParam>
  where ResultParam : GeneralParameters, new()
  where LoadData : CondData
  where BoundData : CondData
SetTimeStep<LoadData>(GeneralParameters parameter, IEnumerable<ICondData> data, float start, float stop) : void
GetMovingSourceTimeStep<LoadData>(IEnumerable<ICondData> data, float start, float stop) : float?
PrepareComplexProcess<SourceParameter, TargetParameter>(List<SourceParameter> sourceParameters, List<TargetParameter> targetParameters, IEnumerable<ICondData> data, bool synchronizeTimeSteps) : IEnumerable<GeneralParameters>
SetMetallurgicalProcesses(TermalParameters thermal, IEnumerable<ICondData> data, float start, float stop) : void
SetThermalDraftParameters(TermalParameters term) : void
SetMechanicalDraftParameters(MechanicalParameters mech) : void
```

`TaskKind` указывает состав физических задач. Тепловые и механические параметры получают базовые численные настройки по своему виду, а шаг времени вычисляется из активных движущихся источников: при их отсутствии максимум равен 1, при наличии выбирается минимум `FrameFunction.CalcMaxTimeStep(velocity)`. Начальный шаг равен половине максимального. Для тепловых параметров названия металлургических реакций берутся из категории `CategoryEnum.Metallurgical` активных `MatData`.

Если `synchronizeTimeSteps = true`, для каждого интервала термомеханической связки выбирается минимум шагов всех движущихся тепловых и механических источников. Если их нет, максимум равен 1. Обе инструкции получают этот максимум и половину его значения как начальный шаг; значение проверяется относительно `MinTimeStep` обеих задач. При `false` каждая задача выбирает шаг по своим условиям.

DraftException : Exception предоставляет конструкторы без параметров, с `string message`, с `string message, Exception innerException`. [Исходник](../DraftException.cs). Тексты новых исключений — на английском.
