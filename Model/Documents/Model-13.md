# Model-13 — Загрузка и сохранение

[Диаграмма](Model-13.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `StlFileWriteExeption`

Пространство имён: `Model.IO.STL`.

Исходники: [SaveToTxtSTLFile.cs](../../Model/IO/STL/SaveToTxtSTLFile.cs).

Базовые типы: `Exception`.

```text
StlFileWriteExeption(string message)
StlFileWriteExeption(string message, Exception inner)
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.
