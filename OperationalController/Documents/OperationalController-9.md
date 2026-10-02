# OperationalController-9 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-9.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `MiddleSurfaceBuilder`

Пространство имён: `OperationalController.GeomObjsCreator`.

Исходники: [MiddleSurfaceBuilder.cs](../../OperationalController/GeomObjsCreator/MiddleSurfaceBuilder.cs).

```text
RemoveDuplicates : bool [get, set]
UseSeparateModel : bool [get, set]
Build(IEnumerable<int> surfaces) : void
MiddleSurfaceBuilder(IGmshController controller)
```

## `Orient`

Пространство имён: `OperationalController.GeomObjsCreator`.

Исходники: [CurveLoop.cs](../../OperationalController/GeomObjsCreator/CurveLoop.cs).

```text
CCW
CW
```

## `GmshController`

Пространство имён: `OperationalController.GmshController`.

Исходники: [GmshController.cs](../../OperationalController/GmshController/GmshController.cs).

Базовые типы: `IGmshController`.

```text
CreateSurfaceFigure(List<GeometryPoint> points, int surfaceTag) : SurfaceFigure
GetPointsCountByElementSize(int curveTag, float elemSize) : int
RemoveGeometryObjects(int[] dimTags, bool delChild= false) : bool
```

## `BeamGroupConnection`

Пространство имён: `OperationalController.MeshObjsCreator`.

Исходники: [BeamGroupConnection.cs](../../OperationalController/MeshObjsCreator/BeamGroupConnection.cs).

```text
Connect(IEnumerable<Node> smaller, IEnumerable<Node> larger) : IEnumerable<Beam>
Create(IEnumerable<Node> master, IEnumerable<Node> slave, double radius, int beamsCount= 0) : IEnumerable<Beam>
GetSquaredDistance(Node one, Node two) : double
```

## `Extract1DFrom2D`

Пространство имён: `OperationalController.MeshObjsCreator`.

Исходники: [Extract1DFrom2D.cs](../../OperationalController/MeshObjsCreator/Extract1DFrom2D.cs).

Базовые типы: `IExtractor1DFrom2D`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `Extract2DFrom3D`

Пространство имён: `OperationalController.MeshObjsCreator`.

Исходники: [Extract2DFrom3D.cs](../../OperationalController/MeshObjsCreator/Extract2DFrom3D.cs).

Базовые типы: `IExtractor2DFrom3D`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.
