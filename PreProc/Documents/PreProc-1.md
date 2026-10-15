# PreProc-1 — API подготовки параметров

[Диаграмма](PreProc-1.mdpuml) · [Обзор](PreProc.md) · [Очередь и имена](PreProc-2.md) · [TCF и пути](PreProc-3.md).

## IPreProc

Пространство имён `PreProc.Interfaces`. [Исходник](../Interfaces/IPreProc.cs).

```text
CalcCompDataV2(IEnumerable<ICondData> data, ProcessProperty property) : IReadOnlyList<GeneralParameters>
WriteCommandFile(string projectPath, IEnumerable<string> queueRows) : string
SetDraftParameters<ResultParam>(ResultParam parameter) : void where ResultParam : GeneralParameters, new()
SortCompDataByTimeAndType(string[] dataStrs) : List<string>
```

Двухаргументный `CalcCompDataV2` материализует параметры и возвращает доступный только для чтения список ссылок на `GeneralParameters`. Объекты параметров остаются изменяемыми. Метод присваивает индекс каждому интервалу, устанавливает относительные ссылки на ожидаемые DB и возвращает задачи в порядке числового индекса, с теплом перед механикой. TSF и TCF не записывает.

`WriteCommandFile` делегирует запись TcfWriter. Клиент вызывает её отдельно по актуальной очереди при «Старт». Входная строка имеет вид `термическая InputData/термическая_1_0_0.25.tsf выполнить`; путь отсчитывается от корня комплекта. Возвращается путь ComputationData/computation.tcf; порядок и статусы сохраняются, TSF не изменяются.

Запись TSF в InputData относится к клиенту Gerbera и будет подключена отдельно. PreProc подготавливает параметры и не сериализует их.

Историческое название `SortCompDataByTimeAndType` сохранено, но сортировка теперь идёт по индексу. Вход — пути TSF; результат — строки вида «вид путь статус» для существующего клиента. Это представление списка, а не новый TCF. Неверное имя оборачивается в DraftException.

## Классы и параметры

`PreProc.PreProc` реализует IPreProc; на диаграмме показан только собственный публичный метод. [Исходник](../PreProc.cs).

```text
GetParameters<LoadData, BoundData, ResultParam>(IEnumerable<ICondData> data) : List<ResultParam>
  where ResultParam : GeneralParameters, new()
  where LoadData : CondData
  where BoundData : CondData
```

ProcessProperty — структура с публичными полями `TaskKind : Project.Interfaces.Tasks.TaskKind` и `CommonTaskType : PreProc.Interfaces.ProcessType`. [Исходник](../ProcessProperty.cs).

ProcessType: Welding, Heating, Quenching, Tempering. [Исходник](../Interfaces/IPreProc.cs).

DraftException : Exception предоставляет конструкторы без параметров, с `string message`, с `string message, Exception innerException`. [Исходник](../DraftException.cs). Тексты новых исключений — на английском.
