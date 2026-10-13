# PreProc-1 — Подготовка расчётных инструкций

[Диаграмма](PreProc-1.mdpuml). [Обзор проекта](PreProc.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IPreProc`

Пространство имён: `PreProc.Interfaces`.

Исходники: [IPreProc.cs](../../PreProc/Interfaces/IPreProc.cs).

```text
CalcCompDataV1(IEnumerable<ICondData> data, ProcessType processType, string path) : void
CalcCompDataV2(IEnumerable<ICondData> data, ProcessProperty prop, string path) : void
SetDraftParameters<ResultParam>(ResultParam parameter) : void where ResultParam : GeneralParameters, new()
SortCompDataByTimeAndType(string[] dataStrs) : List<string>
```

## `DraftException`

Пространство имён: `PreProc`.

Исходники: [DraftException.cs](../../PreProc/DraftException.cs).

Базовые типы: `Exception`.

```text
DraftException()
DraftException(string message)
DraftException(string message, Exception innerException)
```

## `PreProc`

Пространство имён: `PreProc`.

Исходники: [PreProc.cs](../../PreProc/PreProc.cs).

Базовые типы: `IPreProc`.

```text
GetParameters<LoadData, BoundData, ResultParam>(IEnumerable<ICondData> data) : List<ResultParam> where ResultParam : GeneralParameters, new()
            where LoadData : CondData
            where BoundData : CondData
```

## `ProcessProperty`

Пространство имён: `PreProc`.

Исходники: [ProcessProperty.cs](../../PreProc/ProcessProperty.cs).

```text
CommonTaskType : ProcessType [field]
TaskKind : TaskKind [field]
```

## `TaskStatus`

Пространство имён: `PreProc`.

Исходники: [PreProc.cs](../../PreProc/PreProc.cs).

Базовые типы: `int`.

```text
выполнить
пропустить
```

## `ProcessType`

Пространство имён: `PreProc.Interfaces`.

Исходники: [IPreProc.cs](../../PreProc/Interfaces/IPreProc.cs).

```text
Heating
Quenching
Tempering
Welding
```

TSF-файлы одиночных и связанных задач записываются по пути Path.Combine.
