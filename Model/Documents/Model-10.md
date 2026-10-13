# Model-10 — Загрузка и сохранение

[Диаграмма](Model-10.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `SaverEventArgs`

Пространство имён: `Model.Events`.

Исходники: [SaverEventArgs.cs](../../Model/Events/SaverEventArgs.cs).

Базовые типы: `EventArgs`.

```text
Message : string [get, private set]
SaverEventArgs(string message)
```

## `LoadModelFromASCIITextFile`

Пространство имён: `Model.IO`.

Исходники: [LoadModelFromASCIITextFile.cs](../../Model/IO/LoadModelFromASCIITextFile.cs).

Базовые типы: `IModelLoader`.

```text
ReplaceQuotes(string source) : string
```

## `LoadModelFromASCIITextFile_v2`

Пространство имён: `Model.IO`.

Исходники: [LoadModelFromASCIITextFile_v2.cs](../../Model/IO/LoadModelFromASCIITextFile_v2.cs).

Базовые типы: `IModelLoader`.

```text
ReplaceQuotes(string source) : string
```

## `LoadModelFromBPF2TextFile`

Пространство имён: `Model.IO`.

Исходники: [LoadModelFromBPF2TextFile.cs](../../Model/IO/LoadModelFromBPF2TextFile.cs).

Базовые типы: `IModelLoader`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `LoadModelFromBPFTextFile`

Пространство имён: `Model.IO`.

Исходники: [LoadModelFromBPFTextFile.cs](../../Model/IO/LoadModelFromBPFTextFile.cs).

Базовые типы: `IModelLoader`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-2](Model-2.mdpuml) | `IModelLoader` |
