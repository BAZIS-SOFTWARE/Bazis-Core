# OperationalController-11 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-11.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `CurveExtruder`

Пространство имён: `OperationalController.MeshObjsCreator.Extrude`.

Исходники: [CurveExtruder.cs](../../OperationalController/MeshObjsCreator/Extrude/CurveExtruder.cs).

Базовые типы: `MeshExtruder`.

```text
CreateBeam(Node lastNode, Node curNode) : Beam
CreateNextLayer(IEnumerable<Element2D> curLayer, Matrix4x4 curBasis) : List<Element2D>
CreateNode(Vector4 curCenter) : Node
CurveExtruder(IGmshController controller, int curveTag, double step, int pointTag)
CurveExtruder(IGmshController controller, int[] curveTags, double step, int pointTag, float maxAngle= MathF.PI / 2)
CurveExtruder(IGmshController controller, int[] curveTags, double[] steps, int pointTag, float maxAngle= MathF.PI / 2)
Extrude(IEnumerable<Element2D> firstLayer) : List<List<Element3D>>
ExtrudeFromPoint(Node firstNode) : List<Beam>
ExtrudeTransfinite(IEnumerable<Element2D> firstLayer) : List<List<Element3D>>
GetTransformedVector(Vector4 coords, Matrix4x4 matrix) : Vector4
```

## `Element2DSorter`

Пространство имён: `OperationalController.MeshObjsCreator.Extrude`.

Исходники: [MeshExtruder.cs](../../OperationalController/MeshObjsCreator/Extrude/MeshExtruder.cs).

Базовые типы: `IComparer<Element2D>`.

```text
Center : Point3D [get, set]
Compare(Element2D x, Element2D y) : int
```

## `Extrude3DStrategy`

Пространство имён: `OperationalController.MeshObjsCreator.Extrude`.

Исходники: [Extrude3DStrategy.cs](../../OperationalController/MeshObjsCreator/Extrude/Extrude3DStrategy.cs).

Базовые типы: `IExtrusionStrategy<List<List<Element3D>>>`.

```text
Transfinite : bool [get, set]
```

## `ExtrudeBeamStrategy`

Пространство имён: `OperationalController.MeshObjsCreator.Extrude`.

Исходники: [ExtrudeBeamStrategy.cs](../../OperationalController/MeshObjsCreator/Extrude/ExtrudeBeamStrategy.cs).

Базовые типы: `IExtrusionStrategy<List<Beam>>`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `MeshExtruder`

Пространство имён: `OperationalController.MeshObjsCreator.Extrude`.

Исходники: [MeshExtruder.cs](../../OperationalController/MeshObjsCreator/Extrude/MeshExtruder.cs).

```text
MissedElements : int [get, private set]
NodeTag : int [get, set]
Nodes : Dictionary<int, Node> [get, private set]
CreateCopy(IEnumerable<Element2D> src) : List<Element2D>
CreateLayerTetrahedrons(IEnumerable<Element2D> curLayer, List<Element2D> nextLayer) : List<Element3D>
CreateTransfiniteLayer(IEnumerable<Element2D> curLayer, List<Element2D> nextLayer) : List<Element3D>
SortFirstLayer(IEnumerable<Element2D> layer) : List<Element2D>
```

## `RotateExtruder`

Пространство имён: `OperationalController.MeshObjsCreator.Extrude`.

Исходники: [RotateExtruder.cs](../../OperationalController/MeshObjsCreator/Extrude/RotateExtruder.cs).

Базовые типы: `MeshExtruder`.

```text
ElementTag : ref int [get]
CreateNextLayer(IEnumerable<Element2D> curLayer, Matrix4x4 transform) : List<Element2D>
Extrude(IEnumerable<Element2D> curLayer, float radAngle, Vector3 origin, Vector3 rotAxis, int segments= 8) : List<List<Element3D>>
ExtrudeTransfinite(IEnumerable<Element2D> curLayer, float radAngle, Vector3 origin, Vector3 rotAxis, int segments= 8) : List<List<Element3D>>
```
