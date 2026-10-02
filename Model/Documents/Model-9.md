# Model-9 — Геометрические и сеточные сущности

[Диаграмма](Model-9.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Triangle_c`

Пространство имён: `Model.MeshObjects`.

Исходники: [Triangle_c.cs](../../Model/MeshObjects/Triangle_c.cs).

Базовые типы: `Element2D`.

```text
Triangle_c(int number, Node[] nodes)
Triangle_c(int number, Node[] nodes, int level)
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-7](Model-7.mdpuml) | `Element2D` |
| [Model-8](Model-8.mdpuml) | `Node` |
