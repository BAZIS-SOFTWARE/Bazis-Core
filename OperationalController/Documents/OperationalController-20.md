# OperationalController-20 — Специализированные контракты контроллера

[Диаграмма](OperationalController-20.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IMeshCommands`

Пространство имён: `OperationalController`.

Исходники: [IMeshCommands.cs](../../OperationalController/Interfaces/IMeshCommands.cs).

Части: [OperationalController-20](OperationalController-20.mdpuml), [OperationalController-21](OperationalController-21.mdpuml).

```text
ChangeMeshSetName(int elemDim, string oldName, string newName) : void
ChangeMeshSetOrder(int elemDim, string name, int order) : void
ClearEmptySet() : void
ClearGeometryMesh() : void
ClearGeometryMesh(int dimension) : void
ClearModelCollection(ObjType objType) : void
ClearNotExistedModelData() : void
ConnectByBeams(string masterGroup, string slaveGroup, double radius, int maxBeams) : void
Create1DFrom2D() : bool
Create2DForm3D() : bool
CreateBoundaryEdges(List<string> linesNodes) : IEnumerable<ILineObject<Node>>
CreateElement(string setName, int dim, int[] nodeNumbers) : void
CreateElementsSet(string setName, int dim) : void
CreateNode(int number, float x, float y, float z) : void
CreateQuadMeshOnPoligon(List<Point2D> points, int numberOfElem) : void
DeleteModelObjects(ObjType objType, IEnumerable<int> objNumbs) : void
DeleteModelSet(ObjType objType, string name) : void
Extrude3DForm2D(Point3D vector, int layers, IEnumerable<IElement2D> elements) : void
ExtrudeElement1DAlongCurve(int[] curveTags, int startPointTag, int startNodeTag, double step) : string
ExtrudeElement3DAlongCurve(int surfaceTag, int[] curveTags, int startPointTag, double step, bool transfinite) : string
ExtrudeElement3DAlongCurve(string setName, int[] curveTags, int startPointTag, double step, bool transfinite) : string
ExtrudeElement3DRotate(int surfaceTag, float angle, Vector3 originPoint, Vector3 rotAxis, bool transfinite) : string
GenerateMesh(int elemDim) : void
HideMesh(int elemDim) : void
```
