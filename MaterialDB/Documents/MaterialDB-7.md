# MaterialDB-7 — Свойства, фазовый состав и функции

[Диаграмма](MaterialDB-7.mdpuml). [Обзор проекта](MaterialDB.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ProcessData`

Пространство имён: `MaterialDB.MaterialData.MetallurgicalData`.

Исходники: [ProcessData.cs](../../MaterialDB/MaterialData/MetallurgicalData/ProcessData.cs).

Базовые типы: `IEnumerable<Process<float>>`.

```text
GetEnumerator() : IEnumerator<Process<float>>
ProcessData(IEnumerable<Property> reactions, string[] reacTypes)
ToString() : string
```

## `InterpolationSearch`

Пространство имён: `MaterialDB.Utilities`.

Исходники: [InterpolationSearch.cs](../../MaterialDB/Utilities/InterpolationSearch.cs).

```text
InterpolatedValue(float[] xAr, float[] yAr, float xn) : float
InterpolatedValueTwoPoints(float x1, float x2, float y1, float y2, float xn) : float
```
