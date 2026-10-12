# Model-4 — Грани и вспомогательные алгоритмы

[Диаграмма](Model-4.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ListExtentions`

Пространство имён: `Model.Extensions`.

Исходники: [ListExtentions.cs](../../Model/Extensions/ListExtentions.cs).

```text
SwapList<T>(this List<T> objectArray, int firstIndex, int secondIndex) : bool
```

## `ElementSurfaceEqualityComparer<T>`

Пространство имён: `Model.Interfaces.Comparers`.

Исходники: [EqualityComparerBySurface.cs](../../Model/Interfaces/ObjectsComparers/EqualityComparerBySurface.cs).

Базовые типы: `IEqualityComparer<ISurface<T>>`.

```text
Equals(ISurface<T> s1, ISurface<T> s2) : bool
GetHashCode(ISurface<T> s) : int
```

## `ComparerBySqCoords`

Пространство имён: `Model.Interfaces.ObjectsComparers`.

Исходники: [ComparerBySqCoords.cs](../../Model/Interfaces/ObjectsComparers/ComparerBySqCoords.cs).

Базовые типы: `IComparer<IModelObject>`.

```text
Compare(IModelObject x, IModelObject y) : int
```

## `EqualityComparerByCentralPoint`

Пространство имён: `Model.Interfaces.ObjectsComparers`.

Исходники: [EqualityComparerByCentralPoint.cs](../../Model/Interfaces/ObjectsComparers/EqualityComparerByCentralPoint.cs).

Базовые типы: `IEqualityComparer<IModelObject>`.

```text
Equals(IModelObject x, IModelObject y) : bool
GetHashCode(IModelObject obj) : int
```

## `EqualityComparerByExistState`

Пространство имён: `Model.Interfaces.ObjectsComparers`.

Исходники: [EqualityComparerByExistState.cs](../../Model/Interfaces/ObjectsComparers/EqualityComparerByExistState.cs).

Базовые типы: `IEqualityComparer<IModelObject>`.

```text
Equals(IModelObject x, IModelObject y) : bool
GetHashCode(IModelObject obj) : int
```

## `EqualityComparerByNumber<T>`

Пространство имён: `Model.Interfaces.ObjectsComparers`.

Исходники: [EqualityComparerByNumber.cs](../../Model/Interfaces/ObjectsComparers/EqualityComparerByNumber.cs).

Базовые типы: `IEqualityComparer<T>`.

```text
Equals(T x, T y) : bool
GetHashCode(T obj) : int
```

## Переходы к связанным типам

[Основная диаграмма](Model.mdpuml). Указатели соответствуют типам `reference`, базовым типам и типам в сигнатурах; стандартные типы .NET не раскрываются.

| Диаграмма | Типы |
| --- | --- |
| [Model-1](Model-1.mdpuml) | `IModelObject` |
| [Model-15](Model-15.mdpuml) | `ISurface<T>` |
