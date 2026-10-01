# PreProc

[PreProc.mdpuml](PreProc.mdpuml) показывает публичный интерфейс `IPreProc`, его реализацию
`PreProc` и входные типы из сборки `Project`. Основной путь — `CalcCompDataV2`: он получает
условия `ICondData`, `ProcessProperty` и каталог для файлов расчёта. `ProcessProperty` хранит
вид физической задачи и тип процесса; `SetDraftParameters` принимает производные
`GeneralParameters`.

`CalcCompDataV1` остаётся в интерфейсе и строит составную тепловую и механическую задачу.
`TaskStatus` применяется при формировании последовательности задач, а `DraftException`
представляет ошибку подготовки чернового расчёта. Входные условия и параметры подробнее
показаны в [Project.mdpuml](../../Project/Documents/Project.mdpuml).

Исходники: [IPreProc.cs](../Interfaces/IPreProc.cs), [PreProc.cs](../PreProc.cs).
