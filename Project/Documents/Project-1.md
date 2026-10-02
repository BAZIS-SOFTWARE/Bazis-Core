# Project-1 — Проект, задача и граничные условия

[Диаграмма](Project-1.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IGeneralData`

Пространство имён: `Project.Interfaces`.

Исходники: [IGeneralData.cs](../../Project/Interfaces/IGeneralData.cs).

```text
Comments : string [get, set]
Name : string [get, set]
```

## `IProject`

Пространство имён: `Project.Interfaces`.

Исходники: [IProject.cs](../../Project/Interfaces/IProject.cs).

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `IProjectData`

Пространство имён: `Project.Interfaces`.

Исходники: [IProjectData.cs](../../Project/Interfaces/IProjectData.cs).

```text
GeneralData : IGeneralData [get]
Loader : IProjectLoader [get, set]
ModelData : IModelData [get, set]
Saver : IProjectSaver [get, set]
TaskData : ITaskData [get, set]
Load(string fullPath) : void
Save(string fullPath) : void
```

## `IResult`

Пространство имён: `Project.Interfaces`.

Исходники: [IResult.cs](../../Project/Interfaces/IResult.cs).

Базовые типы: `ISerializable`.

```text
Data : DataTable [get]
TaskKind : TaskKind [get]
Time : float [get]
AddData(DataTable dbDataTable) : void
GetValue(int objNumber, string resDescr) : float
```

## `IResultData`

Пространство имён: `Project.Interfaces`.

Исходники: [IResultData.cs](../../Project/Interfaces/IResultData.cs).

Базовые типы: `IList<Result>`.

```text
Loader : IResultsLoader [get, set]
Saver : IResultsSaver [get, set]
AddRange(IEnumerable<Result> results) : void
AddRange(IEnumerable<Result> results, IEqualityComparer<Result> comparer) : void
FindByTaskKind(TaskKind taskKind) : IEnumerable<Result>
FindByTaskKind(string taskKind) : IEnumerable<Result>
FindByTime(TaskKind taskKind, float time) : IEnumerable<Result>
FindByTime(TaskKind taskKind, float time, float delta) : IEnumerable<Result>
FindByTime(string taskKind, float time) : IEnumerable<Result>
FindByTime(string taskKind, float time, float delta) : IEnumerable<Result>
FindByTimeInterval(string taskKind, float firstTime, float lastTime) : IEnumerable<Result>
GetResultKinds() : List<TaskKind>
Load(string path) : void
```

## `ICillindricalFunction`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ICillindricalFunction.cs](../../Project/Interfaces/Tasks/ICillindricalFunction.cs).

Базовые типы: `IFrameFunction`.

```text
BottomDiam : float [get]
Length : float [get]
UpperDiam : float [get]
```
