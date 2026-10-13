# OperationalController-18 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-18.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `BrigthnessController`

Пространство имён: `OperationalController.SceneBrightnessController`.

Исходники: [BrigthnessController.cs](../../OperationalController/SceneBrightnessController/BrigthnessController.cs).

```text
ApplyBrightness(Color color, float dimFactor, float desaturateFactor) : Color
```
