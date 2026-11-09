# MaterialDB-10 — Совместимость и проверка данных

[Диаграмма](MaterialDB-10.mdpuml). [Обзор проекта](MaterialDB.md).

## Совместимость: `IArgumentKindResolver`, `ArgumentKindResolver`

Пространство имён: `MaterialDB.Compatibility`. Исходники: [IArgumentKindResolver.cs](../../MaterialDB/Compatibility/IArgumentKindResolver.cs), [ArgumentKindResolver.cs](../../MaterialDB/Compatibility/ArgumentKindResolver.cs).

```text
Resolve(MaterialDBItem material) : void
Resolve(FunctionDBData functions) : void
```

Выводит `ArgumentKind` только у записей со значением `Undefined` (старые файлы), явное значение не меняет:

| Где | Вид |
| --- | --- |
| «Общие сведения», таблица 1×1 | `Constant` |
| «Общие сведения», остальные | `Descriptor` |
| «Металлургия» | `Process` |
| Тепловые, механические, Fluid, Gas | `TemperaturePhase`, если есть «Структура», иначе `Argument` |
| Функция, одна строка | `Constant` |
| Функция, остальные | `Argument` |

Запускают `MaterialDBItemConverter` ([MaterialDB-8](MaterialDB-8.mdpuml)) и `FunctionDBData.OnDeserialized` ([MaterialDB-2](MaterialDB-2.mdpuml)). После загрузки `Undefined` не встречается.

## Проверка: `IMaterialValidator`, `MaterialValidator`, `ValidationIssue`

Пространство имён: `MaterialDB.Validation`. Исходники: [IMaterialValidator.cs](../../MaterialDB/Validation/IMaterialValidator.cs), [MaterialValidator.cs](../../MaterialDB/Validation/MaterialValidator.cs), [ValidationIssue.cs](../../MaterialDB/Validation/ValidationIssue.cs).

```text
IMaterialValidator:
  Validate(MaterialDBItem material) : IEnumerable<ValidationIssue>
  Validate(FunctionDBData functions) : IEnumerable<ValidationIssue>
MaterialValidator(PressureColumnConvention pressureColumns)
ValidationIssue(IssueSeverity severity, string source, string category, string item, string message)
  Category, Item, Message, Severity, Source : [get]
  ToString() : string
IssueSeverity: Error, Warning
```

Проверка не бросает исключений и возвращает замечания лениво (`yield return`). Правила материала:

| № | Правило | Уровень |
| --- | --- | --- |
| V1 | `ArgumentKind` не `Undefined` | Error |
| V2 | Колонки и строки соответствуют виду (`Argument`: имена колонок свойства «Температура», «Значение» — Warning) | Error |
| V3 | Аргумент и давления в заголовках `P_…` возрастают строго | Error |
| V4 | Числовые ячейки не пустые и конечные | Error |
| V5 | Колонки `TemperaturePhase` совпадают с фазами «Структуры» по имени и порядку | Error |
| V6 | `TemperaturePhase` и `Process` без «Структуры»; `TemperaturePressure` у твёрдого | Error |
| V7 | Нет обязательного свойства среды; у газа при `IdealGas` нет строки «M» в «Модели плотности», при `Sutherland` — строк μ0, T0, S в «Модели вязкости» | Error |
| V8 | Идеальный газ или Сазерленд не у газа; «M», опорное давление, μ0, T0, S ≤ 0 | Error |
| V9 | «Структура» у жидкости или газа | Error |
| V10 | Сумма долей «Структуры» ≠ 1 | Warning |
| V11 | Таблица плотности при `IdealGas` (остаётся после `UseIdealGasDensity`, при расчёте игнорируется) | Warning |
| V12 | Таблица вязкости при `Sutherland` (остаётся после `UseSutherlandViscosity`, при расчёте игнорируется) | Warning |
| V13 | «Модель материала» = 2: нет троек X/Y/Z; ортотропные E и ТКЛР у жидкости или газа | Error |
| V14 | «Модель твердости» = 1: нет «Структуры», нет колонки индекса, индекс вне «Структуры» или повторён | Error |
| V15 | Непустая категория Fluid у газа или твёрдого, Gas у жидкости или твёрдого | Error |

Функции проверяются по V1–V4. Проверки V9 и V15 остаются проверками данных: API классов материалов лишних категорий создать не позволяет, но файл их содержать может.