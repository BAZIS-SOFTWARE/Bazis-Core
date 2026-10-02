# Project-11 — Параметры и физические поля

[Диаграмма](Project-11.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `PhysicalSet`

Пространство имён: `Project.TaskParameters`. Исходники: [PhysicalSet.cs](../../Project/TaskParameters/PhysicalSet.cs).

```text
File : string [get, set]
IsFileEmpty : bool [get]
Name : PhysicalSetName [get, set]
Source : PhysicalSetSource [get, set]
Values : Dictionary<string, List<PhysicalField>> [get, set]
```

## `PhysicalSetName`

Пространство имён: `Project.TaskParameters`. Исходники: [PhysicalSet.cs](../../Project/TaskParameters/PhysicalSet.cs).

```text
Chemical
Hydrodynamic
Mechanical
Thermal
```

## `PhysicalSetSource`

Пространство имён: `Project.TaskParameters`. Исходники: [PhysicalSet.cs](../../Project/TaskParameters/PhysicalSet.cs).

```text
ResultFile
Values
```

## `SolverSettings`

Пространство имён: `Project.TaskParameters`. Исходники: [SolverSettings.cs](../../Project/TaskParameters/SolverSettings.cs).

```text
MatrixStorage : MatrixStorageKind [get]
MaxIter : int [get, set]
MaxRelaxation : float [get, set]
Precision : float [get, set]
Priority : string [get, set]
Relaxation : float [get, set]
Solver : LinearSolverKind [get]
SolverSettings(LinearSolverKind solver, MatrixStorageKind matrixStorage)
```

## `SolverSettingsIO`

Пространство имён: `Project.TaskParameters`.

Исходники: [SolverSettingsIO.cs](../../Project/TaskParameters/SolverSettingsIO.cs).

```text
ReadFromFile(string path) : string
WriteToFile(string path, string settings) : void
```

## `TermalConvergence`

Пространство имён: `Project.TaskParameters`. Исходники: [TermalConvergence.cs](../../Project/TaskParameters/TermalConvergence.cs).

```text
Is_Switched_Tm : bool [get, set]
Tm : float [get, set]
```
