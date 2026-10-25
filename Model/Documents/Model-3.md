# Model-3 — Модель, наборы и группы

[Диаграмма](Model-3.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IGroup`

Пространство имён: `Model.Interfaces`.

Исходники: [IGroup.cs](../../Model/Interfaces/IGroup.cs).

Базовые типы: `IList<IModelObject>`.

```text
Name : string [get, set]
Number : int [get]
ObjType : ObjType [get]
AddRange(IEnumerable<IModelObject> items) : void
ClearRemoved() : void
Reverse() : void
SortByDistance() : void
Swap(int fInd, int sInd) : bool
```

## `IGroupData`

Пространство имён: `Model.Interfaces`.

Исходники: [IGroupData.cs](../../Model/Interfaces/IGroupData.cs).

Базовые типы: `IList<IGroup>`.

```text
ClearNotExisted() : void
Create(IEnumerable<IModelObject> modelObjects) : string
Create(string name, IEnumerable<IModelObject> modelObjects) : void
Create(string name, ObjType objType) : void
Find(int index) : IGroup
Find(string grName) : IGroup
FindMany(ObjType objType) : IEnumerable<IGroup>
GetMaxNumber() : int
Sort() : void
```

## `ILineObject<T>`

Пространство имён: `Model.Interfaces`.

Исходники: [ILineObject.cs](../../Model/Interfaces/ILineObject.cs).

Базовые типы: `IModelObject`, `IVertexObject<T>`.

```text
CalcLength() : double
```

## `IModelData`

Пространство имён: `Model.Interfaces`.

Исходники: [IModelData.cs](../../Model/Interfaces/IModelData.cs).

```text
GroupData : IGroupData [get]
Loader : IModelLoader [get, set]
ObjectData : IObjectsData [get]
Saver : IModelSaver [get, set]
VolumeData : VolumeData [get]
Append(IModelData modelData) : IReadOnlyList<string>
ClearAll() : void
Load(string path) : void
Save(string path) : void
SetGroupData(IGroupData objectData) : void
SetObjectData(IObjectsData objectData) : void
```

## `INormal`

Пространство имён: `Model.Interfaces`.

Исходники: [INormal.cs](../../Model/Interfaces/INormal.cs).

```text
CalcNormal() : Point3D
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-1](Model-1.mdpuml) | `IModelObject` |
| [Model-2](Model-2.mdpuml) | `IModelLoader`, `IModelSaver` |
| [Model-6](Model-6.mdpuml) | `ObjType`, `VolumeData` |
| [Model-14](Model-14.mdpuml) | `IObjectsData` |
| [Model-15](Model-15.mdpuml) | `IObjectsData`, `IVertexObject<T>` |
| [Geometry-4](../../PrGeometry/Documents/Geometry-4.mdpuml) | `Point3D` |
