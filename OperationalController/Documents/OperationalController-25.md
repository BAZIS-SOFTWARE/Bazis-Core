# OperationalController-25 — Специализированные контракты контроллера

[Диаграмма](OperationalController-25.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IProjectController`

Пространство имён: `OperationalController`.

Исходники: [IProjectController.cs](../../OperationalController/Interfaces/IProjectController.cs).

Базовые типы: `IProjectSession`, `IProjectEvents`, `IProjectView`, `IProjectStorage`, `IMeshQueries`, `IMeshCommands`, `IGeometryQueries`, `IGeometryCommands`, `IGroupQueries`, `IGroupCommands`, `IConditionQueries`, `IConditionCommands`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.
