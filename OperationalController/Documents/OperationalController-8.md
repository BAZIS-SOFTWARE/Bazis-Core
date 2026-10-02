# OperationalController-8 — Создание объектов, операции и презентаторы

[Диаграмма](OperationalController-8.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `VolumeMeshingSettings`

Пространство имён: `OperationalController`.

Исходники: [VolumeMeshingSettings.cs](../../OperationalController/Meshing/VolumeMeshingSettings.cs).

```text
GradientSettings : GradientVolumeMeshingSettings [get]
Type : VolumeMeshingType [get]
VolumeMeshingSettings(VolumeMeshingType type, GradientVolumeMeshingSettings gradientSettings= null)
```

## `VolumeMeshingType`

Пространство имён: `OperationalController`.

Исходники: [VolumeMeshingSettings.cs](../../OperationalController/Meshing/VolumeMeshingSettings.cs).

```text
Gradient
Regular
Undefined
```

## `ChamferGeometryBuilder`

Пространство имён: `OperationalController.GeomObjsCreator`.

Исходники: [ChamferGeometryBuilder.cs](../../OperationalController/GeomObjsCreator/ChamferGeometryBuilder.cs).

```text
Build(IEnumerable<int> curveTags, double firstLength, double secondLengthOrAngle, bool isAngle, bool reflected= false) : int[]
ChamferGeometryBuilder(IGmshController controller)
```

## `Convert2DMeshTo2DGeo`

Пространство имён: `OperationalController.GeomObjsCreator`.

Исходники: [Convert2DMeshTo2DGeo.cs](../../OperationalController/GeomObjsCreator/Convert2DMeshTo2DGeo.cs).

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `CurveLoop`

Пространство имён: `OperationalController.GeomObjsCreator`.

Исходники: [CurveLoop.cs](../../OperationalController/GeomObjsCreator/CurveLoop.cs).

```text
Loop : LinkedList<CurveLoopNode> [get, private set]
Orientation : Orient [get, private set]
Surface : int [get, private set]
CheckLoop() : void
CurveLoop(IGmshController controller, int surface, int index, int pointsPerCurve= 16)
MakeSimilar(CurveLoop src) : void
Rebuild(double[] coords) : void
Reverse(double[] coords= null) : void
```

## `CurveLoopNode`

Пространство имён: `OperationalController.GeomObjsCreator`.

Исходники: [CurveLoop.cs](../../OperationalController/GeomObjsCreator/CurveLoop.cs).

```text
AverageCoords : double[] [get, private set]
Begin : int [get, private set]
End : int [get, private set]
Tag : int [get, private set]
CurveLoopNode(int begin, int end, int tag)
GetParametrization(IGmshController controller, int pointsPerCurve= 16) : double[]
Reverse() : void
SetAverageCoords(double[] coords) : void
```
