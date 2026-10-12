# Project-12 — Параметры и физические поля

[Диаграмма](Project-12.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `TermalParameters`

Пространство имён: `Project.TaskParameters`.

Исходники: [TermalParameters.cs](../../Project/TaskParameters/TermalParameters.cs).

Базовые типы: `GeneralParameters`.

```text
MetallurgicalProcesses : List<string> [get, set]
TermalConvergence : TermalConvergence [get, set]
TermalParameters()
```

## `TimeSettings`

Пространство имён: `Project.TaskParameters`.

Исходники: [TimeSettings.cs](../../Project/TaskParameters/TimeSettings.cs).

```text
InitTimeStep : float [get, set]
MaxTimeStep : float [get, set]
MinTimeStep : float [get, set]
StartTime : float [get, set]
StopTime : float [get, set]
```
