# OperationalController-10 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-10.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `OctreeBeamGroupConnection`

Пространство имён: `OperationalController.MeshObjsCreator`.

Исходники: [OctreeBeamGroupConnection.cs](../../OperationalController/MeshObjsCreator/OctreeBeamGroupConnection.cs).

Базовые типы: `BeamGroupConnection`.

```text
Connect(IEnumerable<Node> master, IEnumerable<Node> slave) : IEnumerable<Beam>
```

## `QuadMeshCreator`

Пространство имён: `OperationalController.MeshObjsCreator`.

Исходники: [QuadMeshCreator.cs](../../OperationalController/MeshObjsCreator/QuadMeshCreator.cs).

```text
CreateQuadMeshOnPoligon(List<Point2D> points, int numberOfElem) : ObjectsCollection<Element2D>
```

## `SimpleBeamGroupConnection`

Пространство имён: `OperationalController.MeshObjsCreator`.

Исходники: [SimpleBeamGroupConnection.cs](../../OperationalController/MeshObjsCreator/SimpleBeamGroupConnection.cs).

Базовые типы: `BeamGroupConnection`.

```text
Connect(IEnumerable<Node> smaller, IEnumerable<Node> larger) : IEnumerable<Beam>
```

## `VolumeMeshCreator`

Пространство имён: `OperationalController.MeshObjsCreator`.

Исходники: [VolumeMeshCreator.cs](../../OperationalController/MeshObjsCreator/VolumeMeshCreator.cs).

Базовые типы: `IVolumeMeshCreator`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `CollectionEmptyArgumentException`

Пространство имён: `OperationalController.MeshObjsCreator.Exceptions`.

Исходники: [CollectionArgumentException.cs](../../OperationalController/MeshObjsCreator/Exceptions/CollectionArgumentException.cs).

Базовые типы: `Exception`.

```text
CollectionEmptyArgumentException()
CollectionEmptyArgumentException(string message)
CollectionEmptyArgumentException(string message, Exception innerException)
```

## `NegativeRadiusArgumentException`

Пространство имён: `OperationalController.MeshObjsCreator.Exceptions`.

Исходники: [RadiusArgumentException.cs](../../OperationalController/MeshObjsCreator/Exceptions/RadiusArgumentException.cs).

Базовые типы: `Exception`.

```text
NegativeRadiusArgumentException()
NegativeRadiusArgumentException(string message)
NegativeRadiusArgumentException(string message, Exception innerException)
```
