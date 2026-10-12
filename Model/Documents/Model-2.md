# Model-2 — Загрузка и сохранение

[Диаграмма](Model-2.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ILoader`

Пространство имён: `Model.Interfaces`. Исходники: [ILoader.cs](../../Model/Interfaces/ILoader.cs).

```text
Load(string path) : IModelData
```

## `IModelLoader`

Пространство имён: `Model.Interfaces`. Исходники: [IModelLoader.cs](../../Model/Interfaces/IModelLoader.cs).

```text
Load(string path, IProgress<int> progress = null) : IModelData
```

`progress` необязателен; загрузчик передаёт целые проценты чтения через `Report`.
Событий загрузки и текстовых сообщений у `IModelLoader` и старого `ILoader` нет.
`ILoader.Load(path)` сохраняет прежнюю сигнатуру; загрузчики модели реализуют `IModelLoader`.
100% означает завершение учитываемого чтения, а результат загрузки подтверждается возвратом метода без исключения.

## `IModelSaver`

Пространство имён: `Model.Interfaces`. Исходники: [IModelSaver.cs](../../Model/Interfaces/IModelSaver.cs).

```text
Save(IModelData model, string path) : void
```

## `ISaver`

Пространство имён: `Model.Interfaces`. Исходники: [ISaver.cs](../../Model/Interfaces/ISaver.cs).

```text
Save(IModelData model, string path) : void
```

## `ISaverEventArgs`

Пространство имён: `Model.Interfaces`. Исходники: [ISaverEventArgs.cs](../../Model/Interfaces/ISaverEventArgs.cs).

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-3](Model-3.mdpuml) | `IModelData` |
