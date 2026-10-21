# PreProc-1 — API подготовки параметров

[Диаграмма](PreProc-1.mdpuml) · [Обзор](PreProc.md) · [Очередь и имена](PreProc-2.md) · [TCF и пути](PreProc-3.md).

## IPreProc

Пространство имён `PreProc.Interfaces`. [Исходник](../Interfaces/IPreProc.cs).

```text
CalcCompDataV2(IEnumerable<ICondData> data, TaskKind taskKinds, bool synchronizeTimeSteps = false) : IReadOnlyList<GeneralParameters>
Load(IEnumerable<GeneralParameters> parameters) : void
LoadInstructions(string projectPath) : void
Add(GeneralParameters parameters) : void
Remove(GeneralParameters parameters) : bool
Clear() : void
GetEnumerator() : IEnumerator<GeneralParameters>
WriteInstructions(string projectPath) : string
SetDraftParameters<ResultParam>(ResultParam parameter) : void where ResultParam : GeneralParameters, new()
```

`CalcCompDataV2` материализует параметры и возвращает доступный только для чтения список ссылок на `GeneralParameters`. Объекты параметров остаются изменяемыми. Метод присваивает индекс каждому интервалу начиная с нуля, устанавливает относительные ссылки на ожидаемые DB и возвращает задачи в порядке числового индекса, с теплом перед механикой. TSF и TCF не записывает.

`Load` материализует переданное перечисление и заменяет текущую очередь только после успешного чтения всех параметров. `LoadInstructions` передаёт ему ленивое перечисление TcfReader; TCF и TSF читаются один раз. Методы `Add`, `Remove`, `Clear` меняют очередь объектов; перечислитель возвращает те же объекты, чтобы клиент мог отображать и донастраивать параметры.

`WriteInstructions` делегирует запись объектов `ISolverInstructionWriter`, по умолчанию `TcfWriter`, и возвращает путь к `ComputationData/computation.tcf`. Писатель обновляет TSF всех объектов, включая пропущенные, и сохраняет порядок и статусы в TCF. Клиент Gerbera вызывает запись при «Старт» и показывает `ToString()` объектов в TreeView.

## Классы и параметры

`PreProc.PreProc` реализует IPreProc; на диаграмме методы интерфейса не повторяются в классе. [Исходник](../PreProc.cs).

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
