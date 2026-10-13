# Model-6 — Геометрические и сеточные сущности

[Диаграмма](Model-6.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `VolumeData`

Пространство имён: `Model.GeometryObjects`.

Исходники: [VolumeData.cs](../../Model/GeometryObjects/VolumeData.cs).

Базовые типы: `IList<VolumeFigure>`.

```text
Count : int [get]
IsReadOnly : bool [get]
this[int index] : VolumeFigure [get, set]
Add(VolumeFigure item) : void
Clear() : void
ClearNotExisted() : void
Contains(VolumeFigure item) : bool
CopyTo(VolumeFigure[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<VolumeFigure>
IndexOf(VolumeFigure item) : int
Insert(int index, VolumeFigure item) : void
Remove(VolumeFigure item) : bool
RemoveAt(int index) : void
VolumeData()
```

## `VolumeFigure`

Пространство имён: `Model.GeometryObjects`.

Исходники: [VolumeFigure.cs](../../Model/GeometryObjects/VolumeFigure.cs).

Базовые типы: `IVolume`.

```text
Name : string [get, set]
Number : int [get]
NumberOfPoints : int [get]
NumberOfSides : int [get]
GetCoordinates() : IEnumerable<Point3D>
GetSurfaceFigures() : IEnumerable<SurfaceFigure>
GetSurfaces() : IEnumerable<GeometrySurface>
GetVertexes() : IEnumerable<GeometryPoint>
ToString() : string
VolumeFigure(int number, SurfaceFigure[] surfaceFigures)
```

## `ObjType`

Пространство имён: `Model.Interfaces`.

Исходники: [IModelObject.cs](../../Model/Interfaces/IModelObject.cs).

```text
Кривая
Поверхность
Точка
Узел
Элемент1D
Элемент2D
Элемент3D
```

## `ElementKind`

Пространство имён: `Model.Interfaces.MeshObjects`.

Исходники: [IElement.cs](../../Model/Interfaces/MeshObjects/IElement.cs).

```text
Гекса
Квадрат
Пента
Стержень
Тетра
Треугольник
```

## `Beam`

Пространство имён: `Model.MeshObjects`.

Исходники: [Beam.cs](../../Model/MeshObjects/Beam.cs).

Базовые типы: `Element`, `ILineObject<Node>`.

```text
Beam(int number, Node[] nodes)
Beam(int number, Node[] nodes, int level)
CreateIntegrationPoint(double w, double ksi) : void
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-3](Model-3.mdpuml) | `ILineObject<T>` |
| [Model-5](Model-5.mdpuml) | `GeometryPoint`, `GeometrySurface`, `SurfaceFigure` |
| [Model-7](Model-7.mdpuml) | `Element` |
| [Model-8](Model-8.mdpuml) | `Node` |
| [Model-16](Model-16.mdpuml) | `IVolume` |
| [Geometry-4](../../PrGeometry/Documents/Geometry-4.mdpuml) | `Point3D` |
