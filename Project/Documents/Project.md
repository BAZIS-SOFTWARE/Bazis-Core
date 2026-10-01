# Project

[Project.mdpuml](Project.mdpuml) показывает `ProjectData` как агрегат общих сведений,
`ModelData` из сборки `Model` и `TaskData`. Интерфейсы на диаграмме соответствуют публичным
свойствам этих объектов. Дочерние области отмечены именами файлов на основной схеме:

- [Project-1.mdpuml](Project-1.mdpuml) — типы условий задачи и связь с группами модели.
- [Project-2.mdpuml](Project-2.mdpuml) — параметры расчёта и наборы физических полей.
- [Project-3.mdpuml](Project-3.mdpuml) — функции и чтение/сохранение проекта.

`TaskData` хранит последовательность `ICondData`; конкретные условия наследуются от
`CondData`. `GeneralParameters.InputSets` задаёт внешние наборы полей, `InitialSet` —
подтверждённое начальное состояние. `PhysicalSet` содержит источник: значения по группам
или файл результатов; отдельная `PhysicalField` хранит компоненты величины. Правила обмена
физическими полями приведены в корневом [AGENTS.md](../../AGENTS.md).

Старый [Classes.mdpuml](Classes.mdpuml) сохранён для истории; актуальная схема — `Project.mdpuml`.

Исходники: [ProjectData.cs](../ProjectData.cs), [TaskData.cs](../Tasks/TaskData.cs),
[GeneralParameters.cs](../TaskParameters/GeneralParameters.cs).
