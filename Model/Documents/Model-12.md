# Model-12 — Загрузка и сохранение

[Диаграмма](Model-12.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `SaveModelToBPF2TextFile`

Пространство имён: `Model.IO`.

Исходники: [SaveModelToBPF2TextFile.cs](../../Model/IO/SaveModelToBPF2TextFile.cs).

Базовые типы: `IModelSaver`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `SaveModelToBPFTextFile`

Пространство имён: `Model.IO`.

Исходники: [SaveModelToBPFTextFile.cs](../../Model/IO/SaveModelToBPFTextFile.cs).

Базовые типы: `IModelSaver`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `LoadFromSTLFile`

Пространство имён: `Model.IO.STL`.

Исходники: [LoadFromSTLFile.cs](../../Model/IO/STL/LoadFromSTLFile.cs).

Базовые типы: `IModelLoader`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `SaveModelToBinarySTLFile`

Пространство имён: `Model.IO.STL`.

Исходники: [SaveToTxtSTLFile.cs](../../Model/IO/STL/SaveToTxtSTLFile.cs).

Базовые типы: `IModelSaver`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `SaveToTxtSTLFile`

Пространство имён: `Model.IO.STL`.

Исходники: [SaveToTxtSTLFile.cs](../../Model/IO/STL/SaveToTxtSTLFile.cs).

Базовые типы: `IModelSaver`.

```text
SaveSurfaces<T>(StreamWriter writer, IEnumerable<ISurface<T>> surfaces) : void where T : IPoint
```

## `StlFileLoadExeption`

Пространство имён: `Model.IO.STL`.

Исходники: [LoadFromSTLFile.cs](../../Model/IO/STL/LoadFromSTLFile.cs).

Базовые типы: `Exception`.

```text
StlFileLoadExeption(string message, Exception inner)
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-2](Model-2.mdpuml) | `IModelLoader`, `IModelSaver` |
| [Model-15](Model-15.mdpuml) | `IPoint`, `ISurface<T>` |
