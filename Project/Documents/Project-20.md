# Project-20 — Функции, материалы и ввод/вывод

[Диаграмма](Project-20.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `LoadTaskDataTextFormat`

Пространство имён: `Project.Tasks.IO`. Исходники: [LoadTaskDataTextFormat.cs](../../Project/Tasks/IO/LoadTaskDataTextFormat.cs). Базовые типы: `ITaskDataLoader`.

Метод `Load` полностью соответствует [ITaskDataLoader](Project-3.md) с необязательным `IProgress<int>`;
собственных публичных перегрузок и событий загрузки нет. Чтение через
[ProgressTextReader](../../Model/Documents/Model-11.md) сообщает процент прочитанных строк.

Полная сигнатура реализации (на диаграмме наследуется от интерфейса):

```text
Load(string fullPath, IGroupData groupData, IProgress<int> progress = null) : ITaskData
```

```text
private ReadLines(string path, IProgress<int> progress) : IEnumerable<string>
```

## `SaveEventArgs`

Пространство имён: `Project.Tasks.IO`. Исходники: [SaveEventArgs.cs](../../Project/Tasks/IO/SaveEventArgs.cs). Базовые типы: `ISaveEventArgs`.

```text
SaveEventArgs(string message)
```

## `SaveTaskDataTextFormat`

Пространство имён: `Project.Tasks.IO`. Исходники: [SaveTaskDataTextFormat.cs](../../Project/Tasks/IO/SaveTaskDataTextFormat.cs). Базовые типы: `ITaskDataSaver`. Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `FrameType`

Пространство имён: `Project.Tasks.LocalFrames`. Исходники: [LocalFrame.cs](../../Project/Tasks/LocalFrames/LocalFrame.cs).

```text
MovedFrame
StaticFrame
```

## `LocalFrame`

Пространство имён: `Project.Tasks.LocalFrames`. Исходники: [LocalFrame.cs](../../Project/Tasks/LocalFrames/LocalFrame.cs).

```text
Frame : Frame [get]
FrameType : FrameType [get, internal set]
Name : string [get, internal set]
Rotation_X : float [get, set]
Rotation_Y : float [get, set]
Rotation_Z : float [get, set]
Shifting : Point3D [get, set]
CalcPosition() : void
GetGlobalCoordsInLocalFrame(Point3D point) : Point3D
SetShiftingAndRotation(string shiftingData) : void
```
