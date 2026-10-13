# Project-17 — Функции, материалы и ввод/вывод

[Диаграмма](Project-17.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `PT`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [PT.cs](../../Project/Tasks/Functions/PT.cs).

Базовые типы: `Function`.

```text
CalcValue() : double
PT()
```

## `Parameter`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [Parameter.cs](../../Project/Tasks/Functions/Parameter.cs).

```text
Name : string [get, set]
ParameterKind : ParameterKind [get, set]
ParameterType : ParameterType [get, set]
GetValue() : double
Parameter(string name)
Parameter(string name, ParameterType type, double value)
SetValue(double value) : void
ToString() : string
```

## `ParameterKind`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [Parameter.cs](../../Project/Tasks/Functions/Parameter.cs).

```text
Digit
Table
```

## `ParameterType`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [Parameter.cs](../../Project/Tasks/Functions/Parameter.cs).

```text
Constant
Variable
```

## `TT`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [TT.cs](../../Project/Tasks/Functions/TT.cs).

Базовые типы: `Function`.

```text
CalcValue() : double
TT()
```

## `TableParameter`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [TableParameter.cs](../../Project/Tasks/Functions/TableParameter.cs).

Базовые типы: `Parameter`.

```text
Parameter : Parameter [get, set]
Table : Property [get]
GetValue() : double
TableParameter(Property table, string name, Parameter parameter)
ToString() : string
```
