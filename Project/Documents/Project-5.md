# Project-5 — Проект, задача и граничные условия

[Диаграмма](Project-5.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ISphereFunction`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ISphereFunction.cs](../../Project/Interfaces/Tasks/ISphereFunction.cs).

Базовые типы: `IFrameFunction`.

```text
Width : float [get]
```

## `ITableFunction`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ITableFunction.cs](../../Project/Interfaces/Tasks/ITableFunction.cs).

```text
DataTable : DataTable [get, set]
Name : string [get, set]
Units : string [get, set]
X_unit : string [get, set]
Y_unit : string [get, set]
CalcProp(float param) : float
```

## `ITaskData`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ITaskData.cs](../../Project/Interfaces/Tasks/ITaskData.cs).

Базовые типы: `IList<ICondData>`.

```text
Functions : FunctionDBData [get, set]
Loader : ITaskDataLoader [get, set]
Materials : MaterialDBData [get, set]
Saver : ITaskDataSaver [get, set]
TaskKind : TaskKind [get, set]
TaskType : TaskType [get, set]
AddRange(IEnumerable<ICondData> datas) : void
ClearNotExisted(IGroup group) : void
ClearNotExisted(IGroupData gropData) : void
Contains(string grName) : bool
Create(DataKind kind, string dataInfo, IGroupData groupData) : ICondData
Find(DataKind kind) : IEnumerable<ICondData>
Find(string grName) : IEnumerable<ICondData>
Find<T>() : IEnumerable<T> where T : ICondData
Load(string projectFile, IGroupData groupData) : void
RemoveRange(IEnumerable<string> dataInfo) : void
Save(string projectFile) : void
```

## `ITimeDependentData`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ITimeDependentData.cs](../../Project/Interfaces/Tasks/ITimeDependentData.cs).

```text
StartTime : float [get, set]
StopTime : float [get, set]
```

## `ITrajectory`

Пространство имён: `Project.Interfaces.Tasks`.

Исходники: [ITrajectory.cs](../../Project/Interfaces/Tasks/ITrajectory.cs).

```text
Name : string [get]
NumberOfPoints : int [get]
this[int ind] : Point3D [get]
GetPoints() : IEnumerable<Point3D>
SetPoints(IEnumerable<Point3D> points) : void
```

## `IPowerParamsFunction`

Пространство имён: `ProjectInterfaces.Tasks`.

Исходники: [IPowerParamsFunction.cs](../../Project/Interfaces/Tasks/IPowerParamsFunction.cs).

Базовые типы: `IPowerFunction`.

```text
GetParameter(string parameter) : float
SetParameter(string parameter, float val) : void
```
