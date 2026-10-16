# PreProc — подготовка переносимого расчётного комплекта

[Обзорная диаграмма](PreProc.mdpuml). [Архитектура решения](../../Documents/Architecture.md).

`CalcCompDataV2` формирует и возвращает упорядоченный список `GeneralParameters`, не записывая файлы. Клиент Gerbera записывает TSF и показывает очередь в TreeView. При открытии готового комплекта клиент читает TCF и восстанавливает очередь и статусы в порядке его команд. `WriteCommandFile` принимает строки текущей очереди и записывает TCF при «Старт», сохраняя её порядок и статусы.

Вход `CalcCompDataV2` — условия и набор физических задач `TaskKind`. Базовые численные настройки выбираются по виду физической задачи. Металлургические реакции определяются активными материалами, шаг времени — активными движущимися источниками.

Необязательный флаг `synchronizeTimeSteps` включает общий максимальный и начальный шаги для связанных задач одного интервала. По умолчанию задачи рассчитывают шаг независимо.

## Разделы

| Документ и диаграмма | Содержание |
| --- | --- |
| [PreProc-1](PreProc-1.md) · [UML](PreProc-1.mdpuml) | Контракт IPreProc и подготовка параметров |
| [PreProc-2](PreProc-2.md) · [UML](PreProc-2.mdpuml) | Имена файлов, строки очереди, статусы и сортировка |
| [PreProc-3](PreProc-3.md) · [UML](PreProc-3.mdpuml) | Запись TCF, проверка путей и файловый контракт |

## Указатель типов

| Тип | Подробности | Исходник |
| --- | --- | --- |
| `PreProc.Interfaces.IPreProc` | [PreProc-1](PreProc-1.md) | [IPreProc.cs](../Interfaces/IPreProc.cs) |
| `PreProc.PreProc` | [PreProc-1](PreProc-1.md) | [PreProc.cs](../PreProc.cs) |
| `PreProc.DraftException` | [PreProc-1](PreProc-1.md) | [DraftException.cs](../DraftException.cs) |
| `PreProc.TaskStatus` | [PreProc-2](PreProc-2.md) | [PreProc.cs](../PreProc.cs) |
| `PreProc.TcfWriter` | [PreProc-3](PreProc-3.md) | [TcfWriter.cs](../TcfWriter.cs) |
| `PreProc.CalculationPaths` — internal | [PreProc-3](PreProc-3.md) | [CalculationPaths.cs](../CalculationPaths.cs) |

Сигнатуры соответствуют текущим исходникам. На диаграммах класс PreProc не повторяет операции интерфейса. Типы библиотеки Project обозначены только точками входа; их устройство здесь не раскрывается. Исключённые из компиляции интерфейсы не включены.

## Границы реализации

Подготовка параметров не имеет файловых эффектов; TSF записывает клиент. Совместимость со старыми файлами без обязательных полей `GeneralParameters` не реализуется. Отдельная библиотека Calculation не создаётся.

Контракт физических полей: [PhysicalFields.md](../../Documents/PhysicalFields.md), [GeneralParameters](../../Project/TaskParameters/GeneralParameters.cs), [PhysicalSet](../../Project/TaskParameters/PhysicalSet.cs). Правила документирования: [AGENTS.md](../../AGENTS.md).

Диаграммы разделены на компактные страницы. Проверка исходников и ссылок выполнена структурно; визуальный рендер в текущем окружении недоступен. Итоговый PDF требует отдельного разрешения и здесь не создаётся. Старые схемы алгоритмов и изображения не определяют актуальный API.
