# Project-3 — Функции, материалы и ввод/вывод

[Диаграмма](Project-3.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ILoadEventArgs`

Пространство имён: `Project.Interfaces.IO`.

Исходники: [ILoadEventArgs.cs](../../Project/Interfaces/IO/ILoadEventArgs.cs).

Архивный интерфейс: исходник сохранён для `Project/Results`, но исключён из [Project.csproj](../Project.csproj)
и отсутствует в публичной сборке Project. Его продолжает использовать архивный
[Project.Results.IO.LoadEventArgs](Project-14.md). Загрузчики проекта и задачи от него не зависят.

```text
Message : string [get]
```

## `IProjectLoader`

Пространство имён: `Project.Interfaces.IO`.

Исходники: [IProjectLoader.cs](../../Project/Interfaces/IO/IProjectLoader.cs).

```text
Load(string fullPath, IProgress<int> progress = null) : IProjectData
```

Реализация — [LoadProjectFromTextFormat](Project-13.md). Событий загрузки нет.
Необязательный `progress` сообщает процент чтения через `IProgress<int>.Report`.
Для `.bpf` и `.bpf2` проход модели занимает 0–50%, проход данных задачи — 50–100%.

## `IProjectSaver`

Пространство имён: `Project.Interfaces.IO`.

Исходники: [IProjectSaver.cs](../../Project/Interfaces/IO/IProjectSaver.cs).

```text
Save(IProjectData project, string fullPath) : void
```

## `ISaveEventArgs`

Пространство имён: `Project.Interfaces.IO`.

Исходники: [ISaveEventArgs.cs](../../Project/Interfaces/IO/ISaveEventArgs.cs).

```text
Message : string [get]
```

## `ITaskDataLoader`

Пространство имён: `Project.Interfaces.IO`.

Исходники: [ITaskDataLoader.cs](../../Project/Interfaces/IO/ITaskDataLoader.cs).

```text
Load(string path, IGroupData groupData, IProgress<int> progress = null) : ITaskData
```

Реализация — [LoadTaskDataTextFormat](Project-20.md). Событий загрузки нет.
Необязательный `progress` сообщает целый процент прочитанных строк.

Вызовы обоих загрузчиков без `progress` сохраняются через значение `null` по умолчанию.
100% относится к учитываемому чтению; успешное открытие подтверждает завершение метода
и затем `ProjectChangeKind.Loaded` от [ProjectController](../../OperationalController/Documents/ProjectController.md).

## `ITaskDataSaver`

Пространство имён: `Project.Interfaces.IO`.

Исходники: [ITaskDataSaver.cs](../../Project/Interfaces/IO/ITaskDataSaver.cs).

```text
Save(ITaskData project, string path) : void
```
