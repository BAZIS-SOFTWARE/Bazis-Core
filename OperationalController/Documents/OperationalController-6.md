# OperationalController-6 — Представление и сообщения об изменениях

[Диаграмма](OperationalController-6.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ProjectChange`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

Базовые типы: `ChangeMessage`.

```text
ChangeKind : ProjectChangeKind [get]
Property : ProjectProperty? [get]
ProjectChange(ProjectChangeKind changeKind, ProjectProperty? property= null)
```

## `ProjectChangeKind`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

```text
Closed
Created
Loaded
PropertyChanged
```

## `ProjectMessageEventArgs`

Пространство имён: `OperationalController`.

Исходники: [ProjectMessageEventArgs.cs](../../OperationalController/Events/ProjectMessageEventArgs.cs).

Базовые типы: `EventArgs`.

```text
Change : ChangeMessage [get]
ProjectMessageEventArgs(ChangeMessage change)
```

## `ProjectProperty`

Пространство имён: `OperationalController`.

Исходники: [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs).

```text
FilePath
FunctionsDB
MaterialsDB
Name
TaskKind
TaskType
```

## `SetStyle`

Пространство имён: `OperationalController`.

Исходники: [SetStyle.cs](../../OperationalController/ModelView/SetStyle.cs).

```text
Color : Color? [get, set]
ViewMode : ViewMode? [get, set]
```

## `UpdateScope`

Пространство имён: `OperationalController`.

Исходники: [UpdateScope.cs](../../OperationalController/ModelView/UpdateScope.cs).

Базовые типы: `IDisposable`.

```text
Dispose() : void
UpdateScope(ModelView view)
```
