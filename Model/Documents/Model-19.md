# Model-19 — Грани и вспомогательные алгоритмы

[Диаграмма](Model-19.mdpuml). [Обзор проекта](Model.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ByNumberFinder`

Пространство имён: `Model.Interfaces.ObjectsFinders`.

Исходники: [ByNumberFinder.cs](../../Model/Interfaces/ObjectsFinders/ByNumberFinder.cs).

```text
Find<T>(List<T> objs, int objNumbr) : T where T : IModelObject
Find<T>(T[] objs, int objNumbr) : T where T : IModelObject
```

## `DistanceSorter`

Пространство имён: `Model.Interfaces.ObjectsSorters`.

Исходники: [DistanceSorter.cs](../../Model/Interfaces/ObjectsSorters/DistanceSorter.cs).

```text
SortByDistance<T>(this List<T> objs) : void where T : IModelObject
```

## `MergeSort<T>`

Пространство имён: `Model.Sort`.

Исходники: [MergeSorts.cs](../../Model/Sort/MergeSorts.cs).

```text
MergeSortFunction(List<T> array) : void
```

## `MergeSort`

Пространство имён: `Model.Sort`.

Исходники: [MergeSorts.cs](../../Model/Sort/MergeSorts.cs).

```text
MergeSortFunction(List<int> array) : void
```

## `MergeSortWithComparer<T>`

Пространство имён: `Model.Sort`.

Исходники: [MergeSorts.cs](../../Model/Sort/MergeSorts.cs).

```text
MergeSortFunction(List<T> array, IComparer<T> comparer) : void
```

## `DefaultModelObjsComparer<T>`

Пространство имён: `Model.Utilities`.

Исходники: [DefaultModelObjsComparer.cs](../../Model/Utilities/DefaultModelObjsComparer.cs).

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
