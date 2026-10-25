# Model-15 — Модель, наборы и группы

[Диаграмма](Model-15.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IObjectsData`

Пространство имён: `Model.Interfaces`.

Исходники: [IObjectsData.cs](../../Model/Interfaces/IObjectsData.cs).

Части: [Model-14](Model-14.mdpuml), [Model-15](Model-15.mdpuml).

```text
Remove(ObjType objType, int objNumber) : void
Remove(ObjType objType, string objSet) : void
Rotate(ObjType objType, Point3D axis, float angle) : void
```

## `IPoint`

Пространство имён: `Model.Interfaces`.

Исходники: [IPoint.cs](../../Model/Interfaces/IPoint.cs).

Базовые типы: `IModelObject`.

```text
Normal : Point3D [get, set]
Position : Point3D [get, set]
```

## `ISquare`

Пространство имён: `Model.Interfaces`.

Исходники: [ISquare.cs](../../Model/Interfaces/ISquare.cs).

```text
CalcSquare() : double
```

## `ISurface<T>`

Пространство имён: `Model.Interfaces`.

Исходники: [ISurface.cs](../../Model/Interfaces/ISurface.cs).

Базовые типы: `ICoordinated`, `INormal`, `ISquare`.

```text
IsInner : bool [get]
Number : int [get]
this[int ind] : T [get, set]
GetDistance(Point3D point3D) : double
GetLinesFromSurface() : Segment3D[]
GetVertexIndex(T vertex) : int
GetVertexes() : IEnumerable<T>
ReverseVertexes() : void
```

## `ISurfaceObject<T,U>`

Пространство имён: `Model.Interfaces`.

Исходники: [ISurfaceObject.cs](../../Model/Interfaces/ISurfaceObject.cs).

Базовые типы: `IModelObject`, `IVertexObject<U>`.

```text
NumberOfSides : int [get]
GetSurfaces() : IEnumerable<T>
```

## `IVertexObject<T>`

Пространство имён: `Model.Interfaces`.

Исходники: [IVertexObject.cs](../../Model/Interfaces/IVertexObject.cs).

Базовые типы: `IModelObject`.

```text
GetVertexes() : IEnumerable<T>
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-1](Model-1.mdpuml) | `IModelObject` |
| [Model-3](Model-3.mdpuml) | `INormal` |
| [Model-6](Model-6.mdpuml) | `ObjType` |
| [Geometry-1](../../PrGeometry/Documents/Geometry-1.mdpuml) | `ICoordinated` |
| [Geometry-4](../../PrGeometry/Documents/Geometry-4.mdpuml) | `Point3D` |
| [Geometry-5](../../PrGeometry/Documents/Geometry-5.mdpuml) | `Segment3D` |
