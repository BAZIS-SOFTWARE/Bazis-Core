# Project-10 — Параметры и физические поля

[Диаграмма](Project-10.mdpuml). [Обзор проекта](Project.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `LinearSolverKind`

Пространство имён: `Project.TaskParameters`.

Исходники: [SolverSettings.cs](../../Project/TaskParameters/SolverSettings.cs).

```text
BiCGStab
ConjugateGradient
SymmetricUtdu
```

## `MatrixStorageKind`

Пространство имён: `Project.TaskParameters`.

Исходники: [SolverSettings.cs](../../Project/TaskParameters/SolverSettings.cs).

```text
GeneralCsr
SymmetricCsr
```

## `MechanicalConvergence`

Пространство имён: `Project.TaskParameters`.

Исходники: [MechanicalConvergence.cs](../../Project/TaskParameters/MechanicalConvergence.cs).

```text
DUm : float [get, set]
Is_Physically_NonLinear : bool [get, set]
Is_Switched_Um : bool [get, set]
MaterialPlasticityCoeff : float [get, set]
PlasticityCriterion : float [get, set]
SiStm : float [get, set]
Um : float [get, set]
```

## `MechanicalParameters`

Пространство имён: `Project.TaskParameters`.

Исходники: [MechanicalParameters.cs](../../Project/TaskParameters/MechanicalParameters.cs).

Базовые типы: `GeneralParameters`.

```text
MechanicalConvergence : MechanicalConvergence [get, set]
MechanicalParameters()
```

## `PhysicalField`

Пространство имён: `Project.TaskParameters`.

Исходники: [PhysicalSet.cs](../../Project/TaskParameters/PhysicalSet.cs).

```text
Components : double[] [get, set]
Name : PhysicalFieldName [get, set]
```

## `PhysicalFieldName`

Пространство имён: `Project.TaskParameters`.

Исходники: [PhysicalSet.cs](../../Project/TaskParameters/PhysicalSet.cs).

```text
Concentration
Displacement
PhaseComposition
Pressure
Strain
Stress
Temperature
Velocity
```
