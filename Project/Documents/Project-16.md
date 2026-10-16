# Project-16 — Функции, материалы и ввод/вывод

[Диаграмма](Project-16.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `DHEX`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [DHEX.cs](../../Project/Tasks/Functions/DHEX.cs).

Базовые типы: `Function`.

```text
CalcValue() : double
DHEX()
```

## `FHEX`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [FHEX.cs](../../Project/Tasks/Functions/FHEX.cs).

Базовые типы: `Function`.

```text
CalcValue() : double
FHEX()
```

## `FrameFunction`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [FrameFunction.cs](../../Project/Tasks/Functions/FrameFunction.cs).

Базовые типы: `Function`, `IFrameFunction`.

```text
X : double [get, set]
Y : double [get, set]
Z : double [get, set]
FrameFunction()
```

`CalcMaxTimeStep(float velocity)` объявлен в `IFrameFunction`; конкретные формы возвращают время перемещения на треть своего размера вдоль направления движения.

## `FuncName`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [Function.cs](../../Project/Tasks/Functions/Function.cs).

```text
CIL
Custom
DEL
DHEX
FHEX
PT
SPH
TT
```

## `FuncType`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [Function.cs](../../Project/Tasks/Functions/Function.cs).

```text
CPF
PF
```

## `Function`

Пространство имён: `Project.Tasks.Functions`.

Исходники: [Function.cs](../../Project/Tasks/Functions/Function.cs).

```text
FunctionType : FuncType [get, set]
Name : string [get, protected set]
this[string key] : Parameter [get, set]
AddParameter(Parameter parameter) : void
CalcValue() : double
ContainsParameter(string name) : bool
GetParameters() : IEnumerable<Parameter>
GetTableParameters() : IEnumerable<TableParameter>
ToString() : string
```
