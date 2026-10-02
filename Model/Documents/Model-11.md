# Model-11 — Загрузка и сохранение

[Диаграмма](Model-11.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `LoadModelFromCDBTextFile`

Пространство имён: `Model.IO`.

Исходники: [LoadModelFromCDBTextFile.cs](../../Model/IO/LoadModelFromCDBTextFile.cs).

Базовые типы: `IModelLoader`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `LoadModelFromINPTextFile`

Пространство имён: `Model.IO`.

Исходники: [LoadModelFromINPTextFile.cs](../../Model/IO/LoadModelFromINPTextFile.cs).

Базовые типы: `IModelLoader`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `LoadModelFromINPTextFile_v2`

Пространство имён: `Model.IO`.

Исходники: [LoadModelFromINPTextFile_v2.cs](../../Model/IO/LoadModelFromINPTextFile_v2.cs).

Базовые типы: `IModelLoader`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `LoadModelFromSalomeFile`

Пространство имён: `Model.IO`.

Исходники: [LoadModelFromSalomeFile.cs](../../Model/IO/LoadModelFromSalomeFile.cs).

Базовые типы: `IModelLoader`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `ProgressTextReader`

Пространство имён: `Model.IO`.

Исходники: [ProgressTextReader.cs](../../Model/IO/ProgressTextReader.cs).

Базовые типы: `StreamReader`.

```text
ProgressTextReader(string path, IProgress<int> progress)
ReadLine() : string
```

## `SaveModelToASCIITextFile`

Пространство имён: `Model.IO`.

Исходники: [SaveModelToASCIITextFile.cs](../../Model/IO/SaveModelToASCIITextFile.cs).

Базовые типы: `IModelSaver`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-2](Model-2.mdpuml) | `IModelLoader`, `IModelSaver` |
