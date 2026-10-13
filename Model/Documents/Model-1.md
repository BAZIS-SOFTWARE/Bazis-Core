# Model-1 — Геометрические и сеточные сущности

[Диаграмма](Model-1.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `IModelObject`

Пространство имён: `Model.Interfaces`.

Исходники: [IModelObject.cs](../../Model/Interfaces/IModelObject.cs).

Базовые типы: `IComparable<IModelObject>`, `IEqualityComparer<IModelObject>`, `IEquatable<IModelObject>`, `IPosition3D`, `ICoordinated`.

```text
Dim : int [get]
ExistState : bool [get, set]
Number : int [get]
ObjType : ObjType [get]
CalcCentr() : Point3D
SetNumber(int number) : void
```

## `IIntegralObject`

Пространство имён: `Model.Interfaces.IntegralObject`.

Исходники: [IIntegralObject.cs](../../Model/Interfaces/IntegralObject/IIntegralObject.cs).

```text
GetIntegralPoints() : IEnumerable<IIntegralPoint>
```

## `IIntegralPoint`

Пространство имён: `Model.Interfaces.IntegralObject`.

Исходники: [IIntegralPoint.cs](../../Model/Interfaces/IntegralObject/IIntegralPoint.cs).

```text
Index : int [get, set]
Jacobi : Matrix<double> [get]
Weigt : double [get, set]
this[string dirInd] : double [get]
AddCoordinate(string name, double value) : void
GetCoordinates() : IEnumerable<KeyValuePair<string, double>>
```

## `IElement`

Пространство имён: `Model.Interfaces.MeshObjects`.

Исходники: [IElement.cs](../../Model/Interfaces/MeshObjects/IElement.cs).

Базовые типы: `IVertexObject<Node>`, `IIntegralObject`.

```text
ElementKind : ElementKind [get]
IncidentMatrix : int[][] [get]
Level : int [get, set]
this[int nodeInd] : Node [get, set]
CalcJacobi(IIntegralPoint integralPoint) : Matrix<double>
CreateIntegrationPoints(int level) : IEnumerable<IntegralPoint>
GetDerivativesFormFunctions(IIntegralPoint intPoint) : double[,]
GetFormFunctions(IIntegralPoint intPoint) : double[]
GetNodeIndex(Node node) : int
```

## `IElement1D`

Пространство имён: `Model.Interfaces.MeshObjects`.

Исходники: [IElement1D.cs](../../Model/Interfaces/MeshObjects/IElement1D.cs).

Базовые типы: `IElement`, `ILineObject<Node>`.

Собственных членов сверх показанного интерфейсного контракта нет; связи показаны на диаграмме.

## `IElement2D`

Пространство имён: `Model.Interfaces.MeshObjects`.

Исходники: [IElement2D.cs](../../Model/Interfaces/MeshObjects/IElement2D.cs).

Базовые типы: `ISurfaceElement`, `ISquare`.

```text
TransitionMatrix : Matrix<double> [get, set]
CalcTransitionMatrix(IPoint3D[] points) : double[,]
CreateIntegrationPoint(double w, double ksi, double eta) : IntegralPoint
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-3](Model-3.mdpuml) | `ILineObject<T>` |
| [Model-5](Model-5.mdpuml) | `ISurfaceElement` |
| [Model-6](Model-6.mdpuml) | `ElementKind`, `ObjType` |
| [Model-7](Model-7.mdpuml) | `IntegralPoint` |
| [Model-8](Model-8.mdpuml) | `Node` |
| [Model-15](Model-15.mdpuml) | `ISquare`, `IVertexObject<T>` |
| [Geometry-1](../../PrGeometry/Documents/Geometry-1.mdpuml) | `ICoordinated`, `IPoint3D` |
| [Geometry-3](../../PrGeometry/Documents/Geometry-3.mdpuml) | `IPosition3D` |
| [Geometry-4](../../PrGeometry/Documents/Geometry-4.mdpuml) | `Point3D` |
