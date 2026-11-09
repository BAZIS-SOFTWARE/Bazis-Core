# MaterialDB-4 — Контракты и данные материалов

[Диаграмма](MaterialDB-4.mdpuml). [Обзор проекта](MaterialDB.md).

Состав API материала. Метки: `[protected]` и `[internal]` показывают видимость, остальные члены публичные. Члены, зависящие от среды, находятся в наследниках — [MaterialDB-8](MaterialDB-8.mdpuml). [Расхождения](../../Documents/ArchitectureDifferences.md).

## `MaterialDBItem` (абстрактный)

Пространство имён: `MaterialDB.MaterialData`.

Исходники: [MaterialDBItem.cs](../../MaterialDB/MaterialData/MaterialDBItem.cs).

Атрибуты: `[JsonConverter(typeof(MaterialDBItemConverter))]`. Наследники: `SolidMaterialItem`, `LiquidMaterialItem`, `GasMaterialItem` ([MaterialDB-8](MaterialDB-8.mdpuml)).

```text
CategoryData : CategoryData [get, set]
CategoryDic : Dictionary<CategoryEnum, string> [field, JsonIgnore]
HasMetallurgy : bool [get, JsonIgnore]
Medium : MediumKind [get, abstract, JsonIgnore]
Name : string [get, set]
this[CategoryEnum key] : Category [get, set]
this[string key] : Category [get, set]
CheckProps() : bool [virtual]
Copy(string copyName) : MaterialDBItem
GetHashCode() : int
GetPhases() : IEnumerable<string>
MaterialDBItem(string name, MaterialSettings settings) [protected]
MaterialDBItem() [internal]
AddProps() : void [protected, virtual]
AddGeneralProps() : void [protected]
AddMaterialModel(MaterialModel model) : void [protected]
CreateEmpty(string name) : MaterialDBItem [protected, abstract]
AddCategory(CategoryEnum category) : void [protected]
AddTableProperty(CategoryEnum category, string propertyName, ArgumentKind kind) : void [protected]
AddGeneralValue(string propertyName, Type valueType, object value) : void [protected]
AddGeneralTable(string propertyName, DataTable table, ArgumentKind kind) : void [protected]
CreateCoefficientTable(Type valueType) : DataTable [protected]
GetDirectionalNames(string propertyName) : IEnumerable<string> [protected]
TryGetGeneralValue(string propertyName, out double value) : bool [internal]
FluidCategory : CategoryEnum [get, internal]
DensityModel : DensityModel [get, internal]
ViscosityModel : ViscosityModel [get, internal]
```

Правила:

* `Medium` определяет класс. Если в файле нет записи «Агрегатное состояние», загрузчик создаёт `SolidMaterialItem`.
* Состав свойств задаётся при создании. Защищённый конструктор вызывает `AddGeneralProps()` и записывает «Модель материала» из [`MaterialSettings`](MaterialDB-11.md). Конструктор наследника записывает настройки своей среды и вызывает `AddProps()`. Базовый конструктор `AddProps()` не вызывает: виртуальный вызов из него выполнялся бы до инициализации наследника.
* `AddProps()` базового класса добавляет тепловые свойства, `CheckProps()` проверяет их. Наследники переопределяют оба метода: сначала вызывают базовую реализацию, затем добавляют или проверяют свойства своей среды ([MaterialDB-8](MaterialDB-8.mdpuml)).
* `AddGeneralProps()` у жидкости и газа записывает «Агрегатное состояние» — по ней загрузчик выбирает класс; у твёрдого тела записи нет.
* Внутренний конструктор без параметров создаёт пустой материал без категорий. Его используют загрузчик и `CreateEmpty` (а через него `Copy()`), которые заполняют категории сами.
* «Модель материала» ([`MaterialModel`](MaterialDB-9.md)): 1 — изотропный, 2 — ортотропный. Новый материал всегда получает эту запись; в старых файлах её может не быть, тогда материал считается изотропным. При 2 вместо скалярных «Теплопроводность», «Модуль Юнга», «ТКЛР» создаются и требуются свойства с суффиксами X/Y/Z.
* Повторный вызов `Add*` для записи «Общих сведений» бросает исключение.