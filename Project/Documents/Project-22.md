# Project-22 — Функции, материалы и ввод/вывод

[Диаграмма](Project-22.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ResultData`

Пространство имён: `Results.ResultsData`.

Исходники: [ResultData.cs](../../Project/Results/ResultData.cs).

Базовые типы: `IResultData`.

```text
Count : int [get]
IsReadOnly : bool [get]
this[int ind] : Result [get, set]
Add(Result results) : void
Clear() : void
Contains(Result item) : bool
CopyTo(Result[] array, int arrayIndex) : void
GetEnumerator() : IEnumerator<Result>
GetResults() : IEnumerable<Result>
IndexOf(Result item) : int
Insert(int index, Result item) : void
Remove(Result item) : bool
RemoveAt(int index) : void
RemoveByTaskKind(TaskKind taskKind) : void
RemoveRange(int startInd, int stopInd) : void
RemoveRange(string taskKind, float firstTime, float secondTime) : void
ResultData()
```

## `ResultsComparer`

Пространство имён: `Results.ResultsData`.

Исходники: [ResultsComparer.cs](../../Project/Results/ResultsComparer.cs).

Базовые типы: `IEqualityComparer<Result>`.

```text
Equals(Result x, Result y) : bool
GetHashCode(Result obj) : int
```
