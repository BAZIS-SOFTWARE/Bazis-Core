# Project-9 — Проект, задача и граничные условия

[Диаграмма](Project-9.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `TaskData`

Пространство имён: `Project.Tasks`.

Исходники: [TaskData.cs](../../Project/Tasks/TaskData.cs).

Базовые типы: `ITaskData`.

```text
Count : int [get]
IsReadOnly : bool [get]
this[int ind] : ICondData [get, set]
Add(ICondData data) : void
Clear() : void
Contains(ICondData item) : bool
CopyTo(ICondData[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<ICondData>
IndexOf(ICondData item) : int
Insert(int index, ICondData item) : void
Load(ITaskData taskData) : void
Remove(ICondData item) : bool
RemoveAll(DataKind kind) : int
RemoveAt(int index) : void
TaskData()
TaskData(MaterialDBData materials, FunctionDBData functions)
```
