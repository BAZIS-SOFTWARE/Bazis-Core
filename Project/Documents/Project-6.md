# Project-6 — Проект, задача и граничные условия

[Диаграмма](Project-6.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `GeneralData`

Пространство имён: `Project`.

Исходники: [GeneralData.cs](../../Project/GeneralData.cs).

Базовые типы: `IGeneralData`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `ProjectData`

Пространство имён: `Project`.

Исходники: [ProjectData.cs](../../Project/ProjectData.cs).

Базовые типы: `IProjectData`.

```text
ProjectData(string projectFile)
```

## `DataKind`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [IData.cs](../../Project/Interfaces/Tasks/IData.cs).

```text
Закрепление
Материал
Нагрев
Нагрузка
Среда
```

## `Direction`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ICondData.cs](../../Project/Interfaces/Tasks/ICondData.cs).

Базовые типы: `int`.

```text
None
X
Y
Z
```

## `TaskKind`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ITaskData.cs](../../Project/Interfaces/Tasks/ITaskData.cs).

Базовые типы: `int`.

```text
механическая
термическая
химическая
```

## `TaskType`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ITaskData.cs](../../Project/Interfaces/Tasks/ITaskData.cs).

```text
AxiPlain
Linear
Plain
Volume
Volume_mixed
```
