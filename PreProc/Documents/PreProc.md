# PreProc

Формирует инструкции расчёта по условиям и параметрам задачи. `CalcCompDataV2` использует `ProcessProperty`; `CalcCompDataV1` сохраняется в текущем контракте. Событий нет. Правила физических полей описаны отдельно.

[Обзорная диаграмма](PreProc.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [PreProc-1](PreProc-1.mdpuml) | Подготовка расчётных инструкций; IPreProc, DraftException, PreProc, ProcessProperty, TaskStatus, ProcessType |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `PreProc.DraftException` | [PreProc-1](PreProc-1.mdpuml) | [DraftException.cs](../../PreProc/DraftException.cs) |
| `PreProc.PreProc` | [PreProc-1](PreProc-1.mdpuml) | [PreProc.cs](../../PreProc/PreProc.cs) |
| `PreProc.ProcessProperty` | [PreProc-1](PreProc-1.mdpuml) | [ProcessProperty.cs](../../PreProc/ProcessProperty.cs) |
| `PreProc.TaskStatus` | [PreProc-1](PreProc-1.mdpuml) | [PreProc.cs](../../PreProc/PreProc.cs) |
| `PreProc.Interfaces.IPreProc` | [PreProc-1](PreProc-1.mdpuml) | [IPreProc.cs](../../PreProc/Interfaces/IPreProc.cs) |
| `PreProc.Interfaces.ProcessType` | [PreProc-1](PreProc-1.mdpuml) | [IPreProc.cs](../../PreProc/Interfaces/IPreProc.cs) |

## Физические поля

[Общий контракт физических полей](../../Documents/PhysicalFields.md) описывает наборы, источники и формат значений. Правила заданы в [AGENTS.md](../../AGENTS.md). Реализованные типы хранения: [GeneralParameters](../../Project/TaskParameters/GeneralParameters.cs) и [PhysicalSet](../../Project/TaskParameters/PhysicalSet.cs). Поддержка каждой проверки формата не предполагается только на основании наличия этих DTO.

## Источники и статус

Состав типов и сигнатуры сверены с текущими `.cs` и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.
