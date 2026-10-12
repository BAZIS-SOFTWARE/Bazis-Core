# Model-14 — Модель, наборы и группы

[Диаграмма](Model-14.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IObjectsData`

Пространство имён: `Model.Interfaces`.

Исходники: [IObjectsData.cs](../../Model/Interfaces/IObjectsData.cs).

Части: [Model-14](Model-14.mdpuml), [Model-15](Model-15.mdpuml).

```text
CurveSet : IObjectsSet<Curve> [get]
E1DCollection : IObjectsCollection<Beam> [get]
E2DCollection : IObjectsCollection<Element2D> [get]
E3DCollection : IObjectsCollection<Element3D> [get]
NodesSet : IObjectsSet<Node> [get]
PointsSet : IObjectsSet<GeometryPoint> [get]
SurfaceSet : IObjectsSet<SurfaceFigure> [get]
Clear(ObjType objType) : void
ClearAll() : void
ClearEmptySet() : void
ClearNotExisted() : void
Count(ObjType objType) : int
Find(ObjType objType, int objNumbr) : IModelObject
FindMany(int objNumbr) : IEnumerable<IModelObject>
GetAllElements() : IEnumerable<IElement>
GetAllObjects() : IEnumerable<IModelObject>
GetMaxElementNumber() : int
GetMaxNumber(ObjType objType) : int
GetObjects(ObjType objType) : IEnumerable<IModelObject>
GetObjects(ObjType objType, string setName) : IEnumerable<IModelObject>
GetSetInfo(ObjType objType, int objNumber) : ISetInfo
GetSetInfo(ObjType objType, string setName) : ISetInfo
GetSetsInfo(ObjType objType) : IEnumerable<ISetInfo>
Move(ObjType objType, Point3D vector) : void
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-1](Model-1.mdpuml) | `IElement`, `IModelObject` |
| [Model-5](Model-5.mdpuml) | `Curve`, `GeometryPoint`, `SurfaceFigure` |
| [Model-6](Model-6.mdpuml) | `Beam`, `ObjType` |
| [Model-7](Model-7.mdpuml) | `Element2D`, `Element3D` |
| [Model-8](Model-8.mdpuml) | `Node` |
| [Model-16](Model-16.mdpuml) | `IObjectsCollection<T>`, `IObjectsSet<T>`, `ISetInfo` |
| [Geometry-4](../../PrGeometry/Documents/Geometry-4.mdpuml) | `Point3D` |
