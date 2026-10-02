# OperationalController-7 — Представление и сообщения об изменениях

[Диаграмма](OperationalController-7.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ViewChange`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

Базовые типы: `ChangeMessage`.

```text
AffectedSets : IEnumerable<ISetInfo> [get]
ChangeKind : ModelViewChange [get]
GetChangeKind(ISetInfo setInfo) : ModelViewChange
ViewChange(IReadOnlyDictionary<ISetInfo, ModelViewChange> changes)
```

## `ViewMode`

Пространство имён: `OperationalController`.

Исходники: [ViewMode.cs](../../OperationalController/ModelView/ViewMode.cs).

```text
Line
LineSurface
Point
Surface
```
