# OperationalController-26 — Специализированные контракты контроллера

[Диаграмма](OperationalController-26.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IProjectEvents`

Пространство имён: `OperationalController`.

Исходники: [IProjectEvents.cs](../../OperationalController/Interfaces/IProjectEvents.cs).

```text
Message : EventHandler<ProjectMessageEventArgs>
```

## `IProjectSession`

Пространство имён: `OperationalController`.

Исходники: [IProjectSession.cs](../../OperationalController/Interfaces/IProjectSession.cs).

```text
FilePath : string [get]
FunctionsDB : FunctionDBData [get, set]
HasProject : bool [get]
IsGeometryInitialized : bool [get]
IsModified : bool [get]
MaterialsDB : MaterialDBData [get, set]
Name : string [get]
TaskKind : TaskKind [get]
TaskType : TaskType [get]
Append(string path, IProgress<int> progress= null) : void
ChangeProjectName(string name) : void
ChangeTaskKind(TaskKind taskKind) : void
ChangeTaskType(TaskType taskType) : void
ClearAllData() : void
Close() : void
Create(string projectFile) : void
CreateTask() : void
CreateTask(MaterialDBData materials, FunctionDBData functions) : void
ImportGeometry(string path) : void
Open(string path, IProgress<int> progress= null) : void
Save(string path) : void
```

## `IProjectStorage`

Пространство имён: `OperationalController`.

Исходники: [IProjectStorage.cs](../../OperationalController/Interfaces/IProjectStorage.cs).

```text
CopyFile(string fileName, string oldFolder, string newFolder) : bool
ExportGeometry(string filePath) : void
ExportMesh(string filePath) : void
LoadSMF(string path) : void
SaveSMF(string path) : void
```
