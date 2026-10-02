# OperationalController-29 — Специализированные контракты контроллера

[Диаграмма](OperationalController-29.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IGmshController`

Пространство имён: `OperationalController.GmshController`.

Исходники: [IGmshController.cs](../../OperationalController/GmshController/IGmshController.cs).

Части: [OperationalController-28](OperationalController-28.mdpuml), [OperationalController-29](OperationalController-29.mdpuml).

```text
SetRecombineSurface(int tag) : void
SetSize(int tag, double meshDize) : void
SetTransfiniteCurve(int tag, int numNodes, MeshType meshType= MeshType.Progression, double coef= 1.0) : void
SetTransfiniteSurface(int tag, Arrangement arrangement= Arrangement.Left, int[] cornerTags= null) : void
SetTransfiniteVolume(int tag, int[] cornerTags= null) : void
Unload() : void
```

## `IExtrusionStrategy<TResult>`

Пространство имён: `OperationalController.MeshObjsCreator.Extrude`.

Исходники: [IExtrusionStrategy.cs](../../OperationalController/MeshObjsCreator/Extrude/IExtrusionStrategy.cs).

```text
AddElement(CurveExtruder extruder, Matrix4x4 correctMatrix, int outIndex, int inIndex) : void
GetResult() : TResult
Initialize(CurveExtruder extruder, object extrudeElements) : void
PostProcess(CurveExtruder extruder, bool rotateStatus, int curveIndex, int lastCurveIndex, Matrix4x4 lastCorrectMatrix) : void
PreProcess(CurveExtruder extruder, Matrix4x4 firstMatrix) : void
```

## `INormalCalculator`

Пространство имён: `OperationalController.MeshObjsUtility`.

Исходники: [INormalCalculator.cs](../../OperationalController/Interfaces/Utilities/INormalCalculator.cs).

```text
CalcElemsNormals(IEnumerable<ISurfaceElement> elements) : IEnumerable<Curve>
CalcNodeNormals(IEnumerable<IElement3D> elements) : Dictionary<int, Point3D>
```

## `IObjsColors`

Пространство имён: `OperationalController.ModelScenePresentator.GlObjsPresenters`.

Исходники: [IObjsColors.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/IObjsColors.cs).

```text
Type : ObjsColorsType [get]
CreateColors(int objectsCount) : IEnumerable<Color>
```

## `ILineObjsVertexCreator<T>`

Пространство имён: `OperationalController.ModelScenePresentator.Interfaces`.

Исходники: [ILineObjsVertexCreator.cs](../../OperationalController/ModelScenePresentator/Interfaces/ILineObjsVertexCreator.cs).

```text
Create(ref int stride, float[] vertexAtributes, IVertexObject<T> nodeObject, System.Drawing.Color color) : void
```

## `IPointObjsVertexCreator`

Пространство имён: `OperationalController.ModelScenePresentator.Interfaces`.

Исходники: [IPointObjsVertexCreator.cs](../../OperationalController/ModelScenePresentator/Interfaces/IPointObjsVertexCreator.cs).

```text
Create(ref int stride, float[] vertexAtributes, IModelObject pointObject, System.Drawing.Color color) : void
```
