# Project-2 — Параметры и физические поля

[Диаграмма](Project-2.mdpuml). [Обзор проекта](Project.md).

Это состав параметров и физических полей. Общий контекст проекта приведён в [архитектуре](../../Documents/Architecture.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `CalculationMode`

Пространство имён: `Project.TaskParameters`.

Исходники: [GeneralParameters.cs](../../Project/TaskParameters/GeneralParameters.cs).

```text
Stationary
Transient
```

## `TaskStatus`

Пространство имён: `Project.TaskParameters`.

Исходник: [GeneralParameters.cs](../../Project/TaskParameters/GeneralParameters.cs).

```text
выполнить
пропустить
```

## `ChemicalConvergence`

Пространство имён: `Project.TaskParameters`.

Исходники: [ChemicalConvergence.cs](../../Project/TaskParameters/ChemicalConvergence.cs).

```text
Cm : float [get, set]
Is_Switched_Cm : bool [get, set]
```

## `ChemicalParameters`

Пространство имён: `Project.TaskParameters`.

Исходники: [ChemicalParameters.cs](../../Project/TaskParameters/ChemicalParameters.cs).

Базовые типы: `GeneralParameters`.

```text
ChemicalConvergence : ChemicalConvergence [get, set]
ChemicalParameters()
```

## `ConditionReference`

Пространство имён: `Project.TaskParameters`.

Исходники: [ConditionReference.cs](../../Project/TaskParameters/ConditionReference.cs).

```text
GroupName : string [get]
Index : int [get]
Kind : DataKind [get]
ConditionReference(int index, DataKind kind, string groupName)
Parse(string value) : ConditionReference
ToString() : string
```

## `ConditionReferenceJsonConverter`

Пространство имён: `Project.TaskParameters`.

Исходники: [ConditionReference.cs](../../Project/TaskParameters/ConditionReference.cs).

Базовые типы: `JsonConverter<ConditionReference>`.

```text
ReadJson(JsonReader reader, Type objectType, ConditionReference existingValue, bool hasExistingValue, JsonSerializer serializer) : ConditionReference
WriteJson(JsonWriter writer, ConditionReference value, JsonSerializer serializer) : void
```

## `GeneralParameters`

Пространство имён: `Project.TaskParameters`.

Исходники: [GeneralParameters.cs](../../Project/TaskParameters/GeneralParameters.cs).

```text
ActiveConditions : List<ConditionReference> [get, set]
CalculationMode : CalculationMode [get, set]
InitialSet : PhysicalSet? [get, set]
Index : int [get, set]
InputSets : List<PhysicalSet> [get, set]
Iterations : int [get, set]
SaveRate : int [get, set]
SolverSettings : SolverSettings [get, set]
TaskKind : TaskKind [get, set]
Status : TaskStatus [get, set]
FileStem : string [get]
TimeSettings : TimeSettings [get, set]
UpdMatrixIteration : int [get, set]
GetInputField(PhysicalSetName fieldSet) : PhysicalSet
ToString() : string
```

`Index` задаётся PreProc для каждого расчётного интервала. `FileStem` возвращает основу имени TSF и DB в виде `вид_индекс_начало_конец`; время форматируется через G9 и InvariantCulture. `ToString()` добавляет к этой основе статус инструкции для отображения в очереди. `Index` и `Status` сериализуются вместе с параметрами; при чтении TCF статус из команды задаёт статус объекта.
