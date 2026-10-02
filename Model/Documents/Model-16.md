# Model-16 — Модель, наборы и группы

[Диаграмма](Model-16.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IVolume`

Пространство имён: `Model.Interfaces`.

Исходники: [IVolume.cs](../../Model/Interfaces/IVolume.cs).

```text
CalcVolume() : double
```

## `IObjectsCollection<T>`

Пространство имён: `Model.Interfaces.ObjectsCollections`.

Исходники: [IObjectsCollection.cs](../../Model/Interfaces/ObjectsCollections/IObjectsCollection.cs).

Базовые типы: `IEnumerable<KeyValuePair<string, ObjectsSet<T>>>`.

```text
Count : int [get]
Names : ICollection<string> [get]
Objects : ICollection<ObjectsSet<T>> [get]
this[int index] : ObjectsSet<T> [get]
this[string name] : ObjectsSet<T> [get]
Add(ObjectsSet<T> set) : void
Add(string setName) : void
Add(string setName, T addObj) : void
AddRange(string setName, IEnumerable<T> addObjs) : void
Append(IObjectsCollection<T> collection) : IReadOnlyList<string>
ChangeName(string oldName, string newName) : void
Clear() : void
ClearEmptySets() : void
ContainsName(string setName) : bool
ContainsNumber(int objNumbr) : bool
Find(int objNumbr) : T
GetMaxNumber() : int
GetMinNumber() : int
GetObjects() : IEnumerable<T>
Remove(int objNumber) : void
Remove(string name) : bool
RemoveNotExistedObjects() : void
```

## `IObjectsSet<T>`

Пространство имён: `Model.Interfaces.ObjectsCollections`.

Исходники: [IObjectsSet.cs](../../Model/Interfaces/ObjectsCollections/IObjectsSet.cs).

Базовые типы: `ISetInfo`, `IDictionary<int, T>`.

```text
Append(IObjectsSet<T> appSet) : string
ClearNotExisted() : void
ClearWithExistState() : void
```

## `ISetInfo`

Пространство имён: `Model.Interfaces.ObjectsCollections`.

Исходники: [ISetInfo.cs](../../Model/Interfaces/ObjectsCollections/ISetInfo.cs).

```text
Name : string [get]
Number : int [get, set]
NumberOfObjects : int [get]
ObjType : ObjType [get]
GetCentr(int number) : Point3D
GetCoords(int number) : IEnumerable<Point3D>
GetNumbers() : IEnumerable<int>
GetObjectInfo(int number) : string
GetObjectsInfo() : IEnumerable<string>
```

## `ModelData`

Пространство имён: `Model`.

Исходники: [ModelData.cs](../../Model/ModelData.cs).

Базовые типы: `IModelData`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-3](Model-3.mdpuml) | `IModelData` |
| [Model-6](Model-6.mdpuml) | `ObjType` |
| [Model-18](Model-18.mdpuml) | `ObjectsSet<T>` |
| [Geometry-4](../../PrGeometry/Documents/Geometry-4.mdpuml) | `Point3D` |
