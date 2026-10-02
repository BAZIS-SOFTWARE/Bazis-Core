# Model-18 — Модель, наборы и группы

[Диаграмма](Model-18.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `NumberGroupComparer`

Пространство имён: `Model.GroupsData`.

Исходники: [NumberGroupComparer.cs](../../Model/GroupsData/NumberGroupComparer.cs).

Базовые типы: `IEqualityComparer<IGroup>`.

```text
Equals(IGroup x, IGroup y) : bool
GetHashCode(IGroup obj) : int
```

## `StringNumberGroupComparer`

Пространство имён: `Model.GroupsData`.

Исходники: [StringNumberGroupComparer.cs](../../Model/GroupsData/StringNumberGroupComparer.cs).

Базовые типы: `IComparer<IGroup>`.

```text
Compare(IGroup x, IGroup y) : int
```

## `WarningAttribute`

Пространство имён: `Model.Interfaces.Attributes`.

Исходники: [WarningAttribute.cs](../../Model/Interfaces/Attributes/WarningAttribute.cs).

Базовые типы: `System.Attribute`.

```text
Message : string [get]
WarningAttribute(string message)
```

## `ObjectsCollection<T>`

Пространство имён: `Model.ObjectsCollections`.

Исходники: [ObjectsCollection.cs](../../Model/ObjectsCollections/ObjectsCollection.cs).

Базовые типы: `IObjectsCollection<T>`.

```text
this[int key] : ObjectsSet<T> [get]
this[string key] : ObjectsSet<T> [get]
ToString() : string
```

## `ObjectsSet<T>`

Пространство имён: `Model.ObjectsCollections`.

Исходники: [ObjectsSet.cs](../../Model/ObjectsCollections/ObjectsSet.cs).

Базовые типы: `IObjectsSet<T>`.

```text
Count : int [get]
IsReadOnly : bool [get]
Keys : ICollection<int> [get]
Values : ICollection<T> [get]
this[int key] : T [get, set]
Add(KeyValuePair<int, T> item) : void
Add(int key, T value) : void
Clear() : void
Contains(KeyValuePair<int, T> item) : bool
ContainsKey(int key) : bool
CopyTo(KeyValuePair<int, T>[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<KeyValuePair<int, T>>
ObjectsSet(string name)
Remove(KeyValuePair<int, T> item) : bool
Remove(int key) : bool
Renumber(ref int v) : bool
ToString() : string
TryGetValue(int key, out T value) : bool
```

## `SurfacesSet`

Пространство имён: `Model.ObjectsCollections`.

Исходники: [SurfacesSet .cs](../../Model/ObjectsCollections/SurfacesSet .cs).

Базовые типы: `ObjectsSet<SurfaceFigure>`.

```text
SurfacesSet(string name)
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-3](Model-3.mdpuml) | `IGroup` |
| [Model-5](Model-5.mdpuml) | `SurfaceFigure` |
| [Model-16](Model-16.mdpuml) | `IObjectsCollection<T>`, `IObjectsSet<T>`, `ISetInfo` |
