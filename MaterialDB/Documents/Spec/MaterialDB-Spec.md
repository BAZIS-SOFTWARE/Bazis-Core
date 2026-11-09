# MaterialDB — спецификация целевого состояния (среды, аргументы таблиц, единый DataItem, вынос расчёта)

Статус: проект, ревизия 2 (@core-architect). В эту ревизию вошли решения пользователя: объединение `PropertyItem`/`FunctionItem` в `DataItem`, переименование `ArgumentKind.Temperature` → `Argument`, модель вязкости. Код и `.csproj` не изменены.

Диаграммы целевого состояния (MaterialDB 2.0.0): [MaterialDB-Target](MaterialDB-Target.mdpuml) и поддиаграммы `MaterialDB-Target-1…6`. Промежуточную версию 1.2.0 показывает [MaterialDB-Target-6](MaterialDB-Target-6.mdpuml). Диаграммы текущего кода — [../MaterialDB.md](../MaterialDB.md); их обновят после реализации.

## 1. Цель

1. Вынести расчёт свойств из MaterialDB. `PropertyItem.CalcProp` и `FunctionItem.CalcProp` вызываются в горячем цикле BazisSolver, перебирают `DataTable` через LINQ и создают массивы при каждом вызове.
2. Описывать материалы без металлургии (медь без фаз), жидкости и газы, свойства которых зависят от T и p.
3. Явно хранить, от чего зависит таблица (`ArgumentKind`).
4. Оставить одну сущность хранения таблицы — `DataItem`. Смысл записи (свойство материала или функция) задаёт контейнер, а не тип.
5. Читать существующие JSON-базы без ручной правки.

## 2. Границы ответственности

```text
SolverCore ──► PropertiesCalculator ──► MaterialDB ◄── Project (BazisCore) ◄── PreProc, OperationalController
               (Binding: MaterialCompiler → CompiledMaterial)     (TableParameter: своя 1D-интерполяция)
```

| Компонент | Отвечает за | Не отвечает за |
| --- | --- | --- |
| **MaterialDB** (BazisCore) | Модель данных; загрузку и сохранение (Newtonsoft); **единственное место** выведения `ArgumentKind` для старых файлов; проверку структуры и полноты; соглашения об именах колонок | Интерполяцию, уравнения состояния, смешение по фазам, модель Сазерленда, перевод единиц |
| **PropertiesCalculator** (BazisSolver) | Однократную компиляцию `MaterialDBItem` в `CompiledMaterial` (`double[]`, бинарный поиск, интерполяция по T, (T, p), смешение по фазам, идеальный газ, Сазерленд) | Формат хранения, выведение `ArgumentKind` |
| **Project** (BazisCore) | `TableParameter`: однократное преобразование `DataItem` функции в массивы и одномерную интерполяцию | Ссылку на PropertiesCalculator (запрещена: другое решение) |
| **SolverCore** | Вызов `CompiledMaterial` в горячем цикле | Доступ к `DataTable` |

### 2.1. Контракт выведения ArgumentKind

* `ArgumentKind` для старых файлов выводит **только** MaterialDB (`ArgumentKindResolver`) во время десериализации (раздел 6.2).
* После загрузки ни у одного `DataItem` нет `ArgumentKind.Undefined`. Если `Undefined` всё же встретился (объект создан в коде без указания вида), это ошибка данных V1. Её сообщает `MaterialValidator`.
* PropertiesCalculator и Project **доверяют** значению `ArgumentKind` и не выводят его повторно по числу колонок или по наличию «Структуры». При `Error` от валидатора `MaterialCompiler` отказывается компилировать материал.

## 3. Две независимые оси классификации материала

| | Без металлургии | С металлургией (есть «Структура») |
| --- | --- | --- |
| **Solid** | медь, алюминий: таблицы `Argument` (X = T) | стали: `TemperaturePhase`, процессы в «Металлургия» |
| **Liquid** | вода, масло: `Argument` / `TemperaturePressure` | не поддерживается в 1.x/2.0 (правило V9) |
| **Gas** | воздух, аргон: `TemperaturePressure`, идеальный газ, вязкость по Сазерленду | не поддерживается |

* `MediumKind` — класс материала для выбора физической задачи, а не мгновенное агрегатное состояние.
* Металлургия задаётся не флагом, а наличием свойства «Структура» с хотя бы одной строкой в «Общих сведениях».
* Плавление металла: материал остаётся `Solid` с металлургией, жидкость — одна из фаз «Структуры», интервал TS/TL задаёт существующее свойство «Кристаллизация».

## 4. Типы целевого состояния (MaterialDB 2.0.0)

Пространства имён: `MaterialDB` (`DataItem`), `MaterialDB.MaterialData`, `MaterialDB.FunctionData`, новые `MaterialDB.Compatibility` и `MaterialDB.Validation`.

### 4.1. Перечисления (новые)

```csharp
public enum MediumKind     { Solid = 0, Liquid = 1, Gas = 2 }
public enum DensityModel   { Table = 0, IdealGas = 1 }
public enum ViscosityModel { Table = 0, Sutherland = 1 }

public enum ArgumentKind
{
    Undefined = 0,           // маркер «поля нет в JSON»; после загрузки не встречается и не сохраняется
    Constant = 1,            // одна строка, одна колонка «Значение»
    Argument = 2,            // одна переменная X: для свойств материала X = температура, для функций — любая (время в «Термоцикле»)
    TemperaturePhase = 3,    // «Температура» + колонка на каждую фазу «Структуры»
    TemperaturePressure = 4, // «Температура» + колонки «P_<давление>»
    Descriptor = 5,          // описательная нечисловая таблица: «Структура», химсостав, параметры зерна, модели плотности и вязкости газа
    Process = 6              // процесс металлургии: «Температура» × «Масс.Доли_<φ>» (или «Температура», доля)
}
```

* Значения перечислений заданы явно. `ArgumentKind` хранится в JSON строкой, поэтому переименование `Temperature` → `Argument` сделано до первой публикации 1.2.0 и не затрагивает ни один файл.
* `Descriptor` и `Process` утверждены пользователем (2026-11-06). `Descriptor` — многострочные описательные таблицы «Общих сведений» («Структура», «Химический состав», «Модель роста аустенитного зерна», «Модель плотности» и «Модель вязкости» газа); они не интерполируются. `Process` — числовые таблицы процессов «Металлургии» («Нагрев A-M» и т.п.), которые PropertiesCalculator компилирует в `CompiledProcess`.

### 4.2. Изменённые перечисления

```csharp
public enum CategoryEnum { General, Thermal, Mechanical, Metallurgical, Fluid /* new: «Гидродинамические свойства» — жидкость */, Gas /* new: «Газодинамические свойства» — газ */ }

public enum PropertyEnum
{
    // 12 существующих значений без изменений …
    MediumKind,             // «Агрегатное состояние»
    DensityModel,           // «Модель плотности»
    [Obsolete] MolarMass,   // «Молярная масса» — отдельной записи нет, строка «M» в «Модели плотности»
    ReferencePressure,      // «Опорное давление»
    ViscosityModel,         // «Модель вязкости»
    [Obsolete] SutherlandCoefficients, // «Коэффициенты Сазерленда» — отдельной записи нет, строки μ0, T0, S в «Модели вязкости»
    DynamicViscosity,       // «Динамическая вязкость»
    Compressibility,        // «Сжимаемость»
    VolumeExpansion,        // «Коэффициент объёмного расширения»
    ThermalConductivityX,   // «Теплопроводность X»  — ортотропный материал («Модель материала» = 2)
    ThermalConductivityY,   // «Теплопроводность Y»
    ThermalConductivityZ,   // «Теплопроводность Z»
    YoungModuleX,           // «Модуль Юнга X»
    YoungModuleY,           // «Модуль Юнга Y»
    YoungModuleZ,           // «Модуль Юнга Z»
    ThermalExpansionX,      // «ТКЛР X»
    ThermalExpansionY,      // «ТКЛР Y»
    ThermalExpansionZ,      // «ТКЛР Z»
    HardnessModel           // «Модель твердости»: 0 — выключена, 1 — включена (модель Куркина); индексы фаз — в той же записи
}
```

`MaterialDBItem.CategoryDic` получает пары `{ Fluid, "Гидродинамические свойства" }` и `{ Gas, "Газодинамические свойства" }`. Газ — отдельное состояние вещества (решение пользователя): вязкость, сжимаемость и объёмное расширение газа лежат в `Gas`, жидкости — в `Fluid`; категорию текучей среды выбирает `MaterialDBItem.FluidCategory` (Gas при `MediumKind.Gas`, иначе Fluid). Категория, не соответствующая среде, — ошибка V15. Имена свойств в `Fluid` и `Gas` одинаковы, поэтому `Category.PropertyDic` от категории не зависит. В `Category.PropertyDic` добавлено девятнадцать новых пар (девять для сред, девять ортотропных и «Модель твердости»). Новые значения добавляются в конец перечислений.

**Ортотропия (решение пользователя).** SolverCore уже читает «Теплопроводность X/Y/Z», «Модуль Юнга X/Y/Z», «ТКЛР X/Y/Z» при «Модель материала» = 2, а MaterialDB их не знал. Формат их узаконивает: при «Модель материала» = 2 вместо скалярных «Теплопроводность», «Модуль Юнга», «ТКЛР» обязательны их тройки X/Y/Z (той же раскладки `TemperaturePhase`/`Argument`); при 1 или отсутствии поля — скалярные. «Модель материала»: 1 — изотропный (по умолчанию), 2 — ортотропный; хранится как `int`. Ортотропия допустима для всех сред только для λ; E и ТКЛР — только Solid.

### 4.3. Иерархия материалов (решение пользователя: абстрактный `MaterialDBItem`)

`MaterialDBItem` перегружен: твёрдому телу, жидкости и газу нужны разные группы свойств. Он становится абстрактным, наследники делятся **по среде**, группы свойств остаются категориями (`Category`) внутри него. По группам свойств наследовать нельзя: у твёрдого тела сразу три группы, а у класса C# один родитель.

```text
abstract MaterialDBItem   Name, CategoryData, индексаторы, Copy, Medium (abstract), HasMetallurgy,
 │                        «Общие сведения», тепловые свойства (виртуальные AddProps, CheckProps),
 │                        «Модель материала» (изотропия/ортотропия), GetPhases
 ├─ SolidMaterialItem     + механические свойства, металлургия, «Структура», «Кристаллизация»,
 │                          «Модель упрочнения», «Модель твердости» (+ индексы фаз Куркина)
 ├─ LiquidMaterialItem    + «Гидродинамические свойства» (вязкость), «Опорное давление»
 └─ GasMaterialItem       + «Газодинамические свойства» (вязкость), модели плотности и вязкости,
                            (параметры моделей — строки в их записях), «Опорное давление»
```

| Состав | Solid | Liquid | Gas |
| --- | --- | --- | --- |
| Общие сведения, тепловые свойства | да | да | да |
| Механические свойства, металлургия | да | нет | нет |
| Гидродинамические свойства (`Fluid`) | нет | да | нет |
| Газодинамические свойства (`Gas`) | нет | нет | да |
| Химические свойства (`ChemicalProperties`) | позже: когда химическая задача определит набор данных | — | — |

* Среду определяет **класс**, а не запись: `MaterialDBItem.Medium` — абстрактное свойство с `[JsonIgnore]`. Запись «Агрегатное состояние» остаётся в файле жидкости и газа как метка для загрузчика; у твёрдого тела её нет (формат старых баз не меняется). Метод `AddMediumKind` удалён: `AddGeneralProps()` жидкости и газа записывает метку сам.
* **Полиморфизм свойств (решение пользователя).** `MaterialDBItem` объявляет `public virtual bool CheckProps()` и `protected virtual void AddProps()`; базовая реализация работает только с тепловыми свойствами. Каждый наследник переопределяет оба метода: вызывает базовую реализацию и добавляет или проверяет свойства своей среды — механические у твёрдого тела, «Гидродинамические свойства» у жидкости, «Газодинамические свойства» у газа. Клиент проверяет любой материал одним вызовом `CheckProps()` через базовый тип, без приведения.
* **Состав свойств задаётся при создании (решение пользователя).** Конструктор принимает объект настроек (`MaterialDB.MaterialData.Settings`): `SolidMaterialSettings` (модель материала, начальный фазовый состав — по умолчанию одна фаза «phase», интервал кристаллизации TS/TL — по умолчанию 1400/1500, «Модель упрочнения» — по умолчанию 2, «Модель твердости» — по умолчанию выключена, индексы фаз твердости), `FluidMaterialSettings` (модель материала, «Опорное давление», по умолчанию 1 МПа), `GasMaterialSettings` (дополнительно модели плотности и вязкости, молярная масса, `SutherlandCoefficients`). Защищённый конструктор `MaterialDBItem(name, settings)` пишет «Общие сведения» и «Модель материала». Конструктор наследника записывает настройки среды и вызывает `AddProps()`; базовый конструктор `AddProps()` не вызывает. Газ с `IdealGas` без молярной массы или с `Sutherland` без коэффициентов — `ArgumentException`; твёрдое тело без фаз, с TS ≥ TL или с «Моделью твердости» = 1 без индексов — тоже. Конструктор твёрдого тела создаёт пустую категорию «Металлургия»; фазовые переходы задаются отдельно (решение пользователя). Конструктор только с именем использует настройки по умолчанию. После создания состав таблиц согласован с моделями, поэтому методы записи закрыты: `AddProps`, `AddGeneralProps`, `AddMaterialModel`, `SetGeneralValue`, `RemoveProperty` — `protected`; `AddPhaseData`, `AddCrystallization`, `AddHardeningModel`, `AddHardnessModel`, `AddReferencePressure`, `SetDensityModel`, `SetViscosityModel` — `private` (`AddDensityModel`, `AddMolarMass`, `AddSutherlandCoefficients` удалены). `AddMetallurgicalProps` удалён.
* **Изменение состава после создания (решение пользователя).** Публичные методы меняют состав согласованно:
  * `SolidMaterialItem.AddPhase(name, massFraction)` добавляет фазу в конец «Структуры» и колонку фазы, заполненную нулями, во все таблицы `TemperaturePhase`. `RemovePhase(name)` удаляет фазу со всем связанным: строку «Структуры», колонки, процессы «Металлургии», где фаза исходная или конечная; индекс фазы в «Модели твердости» становится −1, следующие сдвигаются на 1. Последнюю фазу удалить нельзя. Поэтому у твёрдого тела всегда есть хотя бы одна фаза, и его таблицы всегда `TemperaturePhase`; материал «без металлургии» (медь) — это материал с одной фазой.
  * `GasMaterialItem.UseTableDensity()` / `UseIdealGasDensity(molarMass)` и `UseTableViscosity()` / `UseSutherlandViscosity(coefficients)` меняют запись модели на месте (решение пользователя): перезаписывают строку «Модель», удаляют строки параметров и при нетабличной модели добавляют их заново («M» или μ0, T0, S). Таблицы «Плотность» и «Динамическая вязкость» при переходе на IdealGas или Сазерленда не удаляются: калькулятор по строке «Модель» понимает, что брать параметры из строк ниже. При переходе на таблицу пустая таблица создаётся, только если её нет.
* Публичный `CheckPhaseData` есть только у `SolidMaterialItem`.
* **Копирование.** `CreateEmpty(name)` создаёт пустой материал через внутренний конструктор без параметров, и `Copy()` переносит категории оригинала без дублей.
* **Загрузка и сохранение.** Абстрактный класс Newtonsoft не создаст, поэтому на `MaterialDBItem` стоит `[JsonConverter(typeof(MaterialDBItemConverter))]`. Конвертер читает «Агрегатное состояние» из «Общих сведений» (нет записи — `SolidMaterialItem`; неизвестное значение — `JsonSerializationException`), создаёт нужный класс, заполняет `Name` и `CategoryData` и вызывает `ArgumentKindResolver`. Пишет он те же два поля (`Name`, `CategoryData`) без `$type`, поэтому формат файла не меняется и `$type` не появляется при `TypeNameHandling.Auto`.
* **Что типы гарантируют, а что нет.** Через API нельзя создать металлургию у жидкости или механику у газа: нужных методов у класса нет. Но файл может содержать лишние данные (например, «Структуру» в газе), поэтому проверки V9 и V15 остаются как проверки данных.
* **Ломающее изменение API.** `new MaterialDBItem(name)` больше не компилируется; `CheckPhaseData` недоступен через базовый тип; `AddThermalProps`, `AddMechProps`, `AddFluidProps`, `AddGasProps`, `CheckThermalProps`, `CheckMechanicalProps`, `CheckFluidProps`, `CheckGasProps` заменены на `AddProps`/`CheckProps`; методы записи настроек закрыты. По semver это мажорное изменение; по решению пользователя оно входит в текущую версию 1.2.0 (см. раздел 8).
### 4.4. `DataItem` — единая запись таблицы (2.0.0)

```csharp
namespace MaterialDB
{
    [Serializable]
    public sealed class DataItem
    {
        [JsonConstructor]
        public DataItem(string name, string x_unit, string y_unit, string units, DataTable dataTable, ArgumentKind argumentKind);

        public string Name { get; set; }
        public string X_unit { get; set; }
        public string Y_unit { get; set; }
        public string Units { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ArgumentKind ArgumentKind { get; set; }
        public DataTable DataTable { get; set; }

        public double this[int row, int col] { get; set; }   // сохраняется (используется GUI)
        public DataItem Copy(string copyName);
        public void AddRow();                                  // сохраняется
        public override string ToString();                     // сохраняется
    }
}
```

* Удаляются `PropertyItem`, `FunctionItem`, `IProperty`, `CalcProp` ×2 и `MaterialDB.Utilities.InterpolationSearch`.
* `sealed` и единственный публичный конструктор с `[JsonConstructor]`. Если поля `ArgumentKind` нет в JSON, в конструктор приходит `default` = `Undefined`, а затем срабатывает резолвер.
* Индексатор `this[row, col]`, `AddRow()` и `ToString()` уже есть в текущем `DataItem` и сохраняются. Пользователь перечислил минимальный состав; эти члены не являются новыми атомарными методами; сохранение утверждено пользователем.

### 4.5. Контейнеры, типизированные по `DataItem` (2.0.0)

| Тип | Было | Станет |
| --- | --- | --- |
| `PropertyData` | `IDictionary<string, PropertyItem>` | `IDictionary<string, DataItem>` |
| `FunctionDBData` | `IDictionary<string, FunctionItem>` | `IDictionary<string, DataItem>`, + `[OnDeserialized]` (раздел 6.2) |
| `ICategory.this[string]` | `PropertyItem` | `DataItem` |
| `Category.this[PropertyEnum]` | `PropertyItem` | `DataItem` |
| `Process<T>.Reaction` | `PropertyItem` | `DataItem` |
| `ProcessData(reactions, reacTypes)` | `IEnumerable<PropertyItem>` | `IEnumerable<DataItem>` |

Смысл записи задаёт контейнер:

* запись в `PropertyData` категорий Thermal, Mechanical, Fluid или Gas — свойство материала, X = температура;
* запись в «Общих сведениях» или «Металлургии» — описатель;
* запись в `FunctionDBData` — функция с произвольным X.

`MaterialValidator` проверяет `MaterialDBItem` и знает категорию каждой записи. Для функций применяется отдельный набор правил (V1–V4).

### 4.6. Классы материалов (изменены)

```csharp
// MaterialDBItem (abstract)
public abstract MediumKind Medium { get; }                                   // new, [JsonIgnore]
public bool HasMetallurgy { get; }                                           // new, [JsonIgnore]
protected MaterialDBItem(string name, MaterialSettings settings);           // «Общие сведения» + «Модель материала»
public virtual bool CheckProps();                                            // тепловые свойства
protected virtual void AddProps();                                           // тепловые свойства
protected void AddGeneralProps();                                            // жидкость и газ записывают «Агрегатное состояние»
protected void AddMaterialModel(MaterialModel model);
protected void SetGeneralValue(string propertyName, Type valueType, object value);   // заменяет запись «Общих сведений»
protected void RemoveProperty(CategoryEnum category, string propertyName);           // нет категории или свойства — ничего
internal bool TryGetModelParameter(string recordName, string parameter, out double value);   // строка-параметр записи «Параметр»/«Значение»
// SolidMaterialItem
public SolidMaterialItem(string name);  public SolidMaterialItem(string name, SolidMaterialSettings settings);
public override bool CheckProps();      protected override void AddProps();  // + механические свойства
public bool CheckPhaseData();
public void AddPhase(string phaseName, double massFraction);   // + колонка фазы (нули) во всех таблицах TemperaturePhase
public void RemovePhase(string phaseName);                     // − колонки, процессы фазы; сдвиг индексов твердости
// LiquidMaterialItem
public LiquidMaterialItem(string name);  public LiquidMaterialItem(string name, FluidMaterialSettings settings);
public override bool CheckProps();       protected override void AddProps();  // + «Гидродинамические свойства»
// GasMaterialItem
public GasMaterialItem(string name);     public GasMaterialItem(string name, GasMaterialSettings settings);
public override bool CheckProps();       protected override void AddProps();  // + «Газодинамические свойства»
public void UseTableDensity();           public void UseIdealGasDensity(double molarMass);
public void UseTableViscosity();         public void UseSutherlandViscosity(SutherlandCoefficients coefficients);
// MaterialDB.MaterialData.Settings
public class MaterialSettings      { MaterialModel MaterialModel = Isotropic; }
public class SolidMaterialSettings : MaterialSettings      { PhaseData Phases = { phase: 1 }; double SolidusTemperature = 1400; double LiquidusTemperature = 1500;
                                                             int HardeningModel = 2; int HardnessModel = 0; HardnessPhaseIndices? HardnessIndices; }
public class FluidMaterialSettings : MaterialSettings      { double ReferencePressure = 1; }
public class GasMaterialSettings   : FluidMaterialSettings { DensityModel; ViscosityModel; double? MolarMass; SutherlandCoefficients? Sutherland; }  // параметры сохраняются только при нетабличной модели
public sealed class SutherlandCoefficients { SutherlandCoefficients(double mu0, double t0, double s); Mu0; T0; S; }
// MaterialDB.MaterialData
public enum MaterialModel { Isotropic = 1, Orthotropic = 2 }
```
Изменения поведения:

| Член | Было | Станет |
| --- | --- | --- |
| `GetPhases()` | `KeyNotFoundException` без «Структуры» | пустая последовательность без металлургии |
| Создание материала | конструктор с именем, затем цепочка `AddGeneralProps`, `Add*Model`, `AddThermalProps`, `AddMechProps`/`AddFluidProps`/`AddGasProps` | конструктор с объектом настроек строит согласованный материал целиком; «Модель материала» записывается всегда; твёрдое тело получает «Структуру» (минимум одна фаза), «Кристаллизацию», «Модель упрочнения», «Модель твердости» и пустую «Металлургию» |
| Фазовый состав твёрдого тела | `AddPhaseData` один раз, таблицы не перестраиваются | `AddPhase`/`RemovePhase` меняют «Структуру» и колонки фаз во всех таблицах `TemperaturePhase` |
| Модели газа после создания | `AddDensityModel`/`AddViscosityModel` (повтор — исключение) | `Use*`-методы меняют запись модели на месте (строка «Модель» и строки параметров); таблицы «Плотность» и «Динамическая вязкость» сохраняются |
| `AddProps()` (бывшие `AddThermalProps()`, `AddMechProps()`, `AddFluidProps()`, `AddGasProps()`) | «Температура» + фазы | раскладка по `HasMetallurgy` и записям моделей: при металлургии `TemperaturePhase`, иначе `Argument`; ArgumentKind задаётся явно; при «Модель материала» = 2 создаются тройки X/Y/Z вместо скалярных λ, E, ТКЛР |
| `CheckProps()` (бывшие `CheckThermalProps()`, `CheckMechanicalProps()`, `CheckFluidProps()`, `CheckGasProps()`), ортотропия | скалярные λ, E, ТКЛР | при «Модель материала» = 2 требуют тройки X/Y/Z |
| `Category.PropertyDic` | `PropertyEnum.Capacity` → «Общие сведения» (ошибка) | `Capacity` → «Теплоемкость» (исправление в 1.2.0) |
| `CategoryData[PropertyEnum]` | ищет по `key.ToString()` (английское имя) и всегда бросает `KeyNotFoundException` | ищет по русскому имени категории через `CategoryDic`/`PropertyDic` (исправление в 1.2.0) |
| `Add*` в «Общих сведениях» при повторном вызове | исключение (`Dictionary.Add`) | без изменений: исключение остаётся (решение пользователя); замена записи — `SetGeneralValue` |
| `AddPhaseData`, `AddCrystallization`, `AddMaterialModel`, `AddHardeningModel` | `new PropertyItem(…)` | `new DataItem(…, ArgumentKind.Descriptor/Constant)` |
| `CheckProps()`, тепловая часть | требует λ, c, ρ | ρ не требуется при `DensityModel.IdealGas` |
| `CheckProps()` жидкости и газа | — | жидкость: «Динамическая вязкость» в Fluid; газ: строка «M» в «Модели плотности» при `IdealGas`; «Динамическая вязкость» в Gas при `ViscosityModel.Table`, строки μ0, T0, S в «Модели вязкости» при `Sutherland` |
| `TryGetGeneralValue` | первая колонка первой строки | колонка «Значение», если она есть, иначе первая колонка (читает и новый, и старый формат моделей газа) |
| `Copy()` | пустой материал через публичный конструктор | через внутренний конструктор без категорий |
| `CreateTable()` (private) | без параметров | `CreateTable(ArgumentKind kind)`; для `TemperaturePressure` создаёт только «Температура», колонки давлений добавляет клиент через `PressureColumnConvention` |

`AddProps()` жидкости после тепловых свойств создаёт «Гидродинамические свойства» (Fluid) и «Динамическую вязкость» (`Argument`); `AddProps()` газа создаёт «Газодинамические свойства» (Gas) и при `ViscosityModel.Table` — «Динамическую вязкость» (`TemperaturePressure`). «Сжимаемость» и «Коэффициент объёмного расширения» необязательны и методом не создаются.

### 4.7. Совместимость, проверка, соглашение по колонкам (новые)

```csharp
namespace MaterialDB.Compatibility
{
    public interface IArgumentKindResolver
    {
        void Resolve(MaterialDBItem material);   // все Undefined-записи материала
        void Resolve(FunctionDBData functions);  // все Undefined-записи базы функций
    }
    public sealed class ArgumentKindResolver : IArgumentKindResolver { }
}

namespace MaterialDB.Validation
{
    public enum IssueSeverity { Error, Warning }
    public sealed class ValidationIssue
    {
        public ValidationIssue(IssueSeverity severity, string source, string category, string item, string message);
        public IssueSeverity Severity { get; }
        public string Source { get; }    // имя материала или базы функций
        public string Category { get; }  // категория материала; для функций — пусто
        public string Item { get; }      // имя DataItem
        public string Message { get; }
    }
    public interface IMaterialValidator
    {
        IEnumerable<ValidationIssue> Validate(MaterialDBItem material);   // yield return
        IEnumerable<ValidationIssue> Validate(FunctionDBData functions);  // yield return, правила V1–V4
    }
    public sealed class MaterialValidator : IMaterialValidator
    {
        public MaterialValidator(PressureColumnConvention convention);
    }
}

namespace MaterialDB.MaterialData
{
    public sealed class PressureColumnConvention       // экземплярный, не static
    {
        public string Prefix { get; }                   // "P"
        public string Format(double pressure);          // 0.101325 → "P_0.101325" (МПа, InvariantCulture)
        public bool TryParse(string columnName, out double pressure);
        public IEnumerable<double> GetPressures(DataTable table);   // yield return
    }
}
```

`MaterialDBItem.Check*` остаются быстрыми проверками с исключением, которыми уже пользуется решатель. `MaterialValidator` выполняет полную проверку без исключений.

## 5. Правила ArgumentKind и колонок

### 5.1. Соглашение по колонкам

| ArgumentKind | Колонки `DataTable` | Строки | Смысл |
| --- | --- | --- | --- |
| `Constant` | `Значение` | ровно 1 | y = const |
| `Argument` | свойство материала: `Температура`, `Значение`; функция: `X`, `Y` (как в `functions.jsf`) | ≥ 1 | y(X) |
| `TemperaturePhase` | `Температура`, `<фаза₁>` … `<фазаₙ>` — имена и порядок как в «Структуре» | ≥ 1 | y(T, φ) = Σ φᵢ·yᵢ(T) |
| `TemperaturePressure` | `Температура`, `P_<p₁>` … `P_<pₘ>` | ≥ 1 | y(T, p), сетка T × p |
| `Descriptor` | задаются свойством | любые | не интерполируется |
| `Process` | `Температура`, `Масс.Доли_<φ₁>` … `Масс.Доли_<φₖ>` (времена достижения долей) либо `Температура`, `<доля>` (2 колонки) | ≥ 2 | кинетика превращения (Аврами / табличная) |

* Значения в первой колонке (аргумент) и давления в заголовках строго возрастают. Числовые ячейки не `DBNull` и конечны.
* `Argument` берёт колонки по позиции: 0 — X, 1 — Y. Имена колонок не нормативны: так совместимы и старые функции `X`/`Y`, и свойства «Температура»/«Значение». Для свойства материала с другими именами выдаётся Warning.
* Колонки давлений следуют соглашению «имя_значение», как `Масс.Доли_0.5` в процессах металлургии. Разделитель — первый `_`, число записывается в InvariantCulture. Давление в МПа (как напряжения в базе): например `P_0.101325`, `P_0.5`.
* Одна строка в `Argument`, `TemperaturePhase` или `TemperaturePressure` означает константу по аргументу. Это сохраняет текущее поведение `CalcProp` при `yAr.Length == 1`.

### 5.2. Новые записи «Общих сведений»

| Запись (`PropertyEnum`) | Колонки | Строки | ArgumentKind | Значения |
| --- | --- | --- | --- | --- |
| «Агрегатное состояние» (`MediumKind`) | `Агрегатное состояние` (int) | 1 | Constant* | 0 Solid, 1 Liquid, 2 Gas |
| «Модель плотности» (`DensityModel`) | `Параметр` (string), `Значение` (double) | Table: 1 (`Модель`); IdealGas: 2 (`Модель`, `M`) | Descriptor | `Модель`: 0 Table, 1 IdealGas; `M` — молярная масса, кг/моль |
| «Модель вязкости» (`ViscosityModel`) | `Параметр` (string), `Значение` (double) | Table: 1 (`Модель`); Sutherland: 4 (`Модель`, `μ0`, `T0`, `S`) | Descriptor | `Модель`: 0 Table, 1 Sutherland; μ0 — МПа·с (Н·с/мм²); T0, S — К |
| «Опорное давление» (`ReferencePressure`) | `Опорное давление` (double) | 1 | Constant* | МПа |
| «Модель твердости» (`HardnessModel`) | без индексов: колонка `Модель твердости` (int), 1 строка. С индексами: `Модель твердости` (int), `FerriteIndex`, `PerliteIndex`, `BeiniteIndex`, `MartensiteIndex`, `AusteniteIndex` (int), 1 строка | 1 | Constant* без индексов, Descriptor с индексами | 0 — выключена, 1 — включена (Куркин); индексы — номер строки фазы в «Структуре», с 0, −1 — фазы нет |

\* Колонка одна и названа по свойству, как в существующих «Модель упрочнения» и «Размер аустенитного зерна». Поэтому для записей «Общих сведений» правило 5.1 «колонка `Значение`» не проверяется: они сопоставляются по имени свойства.

**Модель твердости (решение пользователя).** Индексы фаз модели Куркина хранятся прямо в файле материала, **в той же записи «Модель твердости»** (отдельной записи нет), и при компиляции материала попадают в модель (PropertiesCalculator). Они лежат в колонках единственной строки с именами свойств модели (`FerriteIndex`, `PerliteIndex`, `BeiniteIndex`, `MartensiteIndex`, `AusteniteIndex`); режим 0/1 — в первой колонке, поэтому читающий только режим код (`TryGetGeneralValue`) работает с обеими раскладками. Порядок фаз в «Структуре» произвольный: пользователь сам указывает, какая строка какой фазе соответствует, и отвечает за правильность. Запись обязательна при «Модель твердости» = 1. Запись создаёт конструктор твёрдого тела из `SolidMaterialSettings.HardnessModel` (по умолчанию 0) и `HardnessIndices`; при `HardnessModel` = 1 индексы обязательны. `HardnessPhaseIndices` — DTO MaterialDB с пятью свойствами `int` тех же имён; используется только при записи, читают индексы валидатор и PropertiesCalculator по колонкам. `RemovePhase` пересчитывает индексы в записи.

**Модели газа (решение пользователя).** Параметры модели хранятся в записи самой модели: первая строка `Модель` — номер модели, строки ниже — её параметры. Отдельных записей «Молярная масса» и «Коэффициенты Сазерленда» нет (`PropertyEnum.MolarMass` и `SutherlandCoefficients` помечены `[Obsolete]`). Калькулятор читает строку `Модель`: при табличной модели берёт таблицу «Плотность» или «Динамическая вязкость», иначе — параметры из строк ниже. Строковые ключи `M`, `μ0`, `T0`, `S` нужно согласовать с PropertiesCalculator: раньше он читал отдельные записи. Тип модели в старом формате (одна числовая колонка) тоже читается; при смене модели такая запись пересоздаётся в новом формате. Формула Сазерленда для расчёта: μ(T) = μ0·(T/T0)^{3/2}·(T0 + S)/(T + S), T в кельвинах. Её применяет компилятор, MaterialDB только хранит коэффициенты.

### 5.3. Обязательные данные по среде

| Данные | Solid | Liquid | Gas |
| --- | --- | --- | --- |
| «Агрегатное состояние» | необязательно (по умолчанию Solid) | обязательно | обязательно |
| «Модель плотности» | только `Table` | `Table` | `Table` / `IdealGas` |
| Строка «M» в «Модели плотности» | — | — | обязательно при `IdealGas` (> 0) |
| «Опорное давление» | — | при таблице `TemperaturePressure` | при `IdealGas` или таблице `TemperaturePressure` (> 0) |
| «Модель вязкости» | — | `Table` | `Table` / `Sutherland` |
| Строки μ0, T0, S в «Модели вязкости» | — | — | обязательно при `Sutherland` (> 0) |
| «Динамическая вязкость» (Fluid — жидкость, Gas — газ) | — | обязательно (`Table`), в Fluid | обязательно при `Table`, в Gas |
| Thermal: «Теплопроводность», «Теплоемкость» | обязательно | обязательно | обязательно |
| Thermal: «Плотность» | обязательно | обязательно | обязательно при `Table` |
| «Механические свойства» (состав `SolidMaterialItem.CheckProps`) | обязательно | — | — |
| «Структура» | необязательно | запрещено | запрещено |
| `TemperaturePhase` | только при металлургии | — | — |
| `TemperaturePressure` | запрещено | разрешено | разрешено |

### 5.4. Плотность и вязкость: один источник

* ρ для всех сред хранится только в «Тепловых свойствах» (решение пользователя): тепловая задача нужна каждой среде, а дублирование не допускается. При `IdealGas` PropertiesCalculator считает ρ = p·M/(R·T_K) с переводом в кг/мм³.
* Вязкость задаётся либо таблицей «Динамическая вязкость» в Fluid (жидкость) или Gas (газ), либо коэффициентами Сазерленда в строках «Модели вязкости». Какой способ действует, определяет строка `Модель`. Таблица может остаться и при Сазерленде (после `UseSutherlandViscosity`); при расчёте она игнорируется, валидатор предупреждает V12. Так же для плотности при `IdealGas` (V11).

## 6. Обратная совместимость с существующими JSON-базами

### 6.1. Формат файлов и `$type` (проверено)

* В `materials_v1.jsf`, `Materials_v7.jsf` и `functions.jsf` нет `"$type"`. Newtonsoft восстанавливает объекты по **объявленному** типу свойства или словаря (`PropertyItem`, `FunctionItem`). Поэтому замена объявленного типа на `DataItem` при тех же именах полей (`Name`, `X_unit`, `Y_unit`, `Units`, `DataTable`) читает старые базы без изменений.
* **Предупреждение.** Загрузчики `MaterialDBData` и `FunctionDBData` используют `TypeNameHandling.Auto`. Этот режим пишет `$type`, когда фактический тип объекта отличается от объявленного. Чтобы `$type` не появился:
  - `DataItem` объявлен `sealed`, наследники не вводятся;
  - словари и свойства не объявляются как `object`, интерфейс или абстрактный тип;
  - в 1.2.0 (переходный период) `PropertyData` объявлена по `PropertyItem` и хранит только `PropertyItem`, а `FunctionDBData` — только `FunctionItem`;
  - тест цикла «сохранение → чтение» проверяет, что в выходном JSON нет подстроки `"$type"` (раздел 9).
* Если файл с `$type` всё же появится, 2.0.0 его не прочитает: тип `PropertyItem` удалён. Это критерий для теста, а не сценарий поддержки.

### 6.2. Где выполняется выведение (SRP)

* Правило выведения находится только в `ArgumentKindResolver` (`MaterialDB.Compatibility`). Модель данных содержит лишь точки запуска.
* `MaterialDBItem` — `MaterialDBItemConverter.ReadJson` → `Resolve(material)` сразу после создания материала (абстрактный класс создаёт конвертер, поэтому `[OnDeserialized]` не используется). Это минимальная единица с контекстом («Структура» видна только на уровне материала). Точка срабатывает при любом способе загрузки: `new MaterialDBData(name, folder)`, прямой `JsonConvert.DeserializeObject<MaterialDBData>` (так делают тесты BazisCore и BazisSolver), загрузка одного материала.
* `FunctionDBData` — `[OnDeserialized]` → `Resolve(this)`. В 2.0.0 `DataItem` не знает своего контейнера, поэтому точка запуска находится в контейнере. В 1.2.0 используется та же точка.
* Не выбраны: выведение в PropertiesCalculator (нарушает контракт 2.1, GUI видел бы `Undefined`) и выведение в `DataItem` (нет контекста).
* Риск для реализации: проверить тестом, что Newtonsoft вызывает `[OnDeserialized]` у типа, реализующего `IDictionary<,>` (`FunctionDBData`). Если не вызывает, точкой запуска становится конструктор `FunctionDBData(dbName, dbFolder)`.

### 6.3. Правило выведения (только для `Undefined`, явное значение не меняется)

1. «Общие сведения»: таблица 1×1 → `Constant`, остальные → `Descriptor`. «Металлургия»: все таблицы → `Process`.
2. Thermal, Mechanical, Fluid, Gas: если есть «Структура» → `TemperaturePhase`, иначе `Argument`.
3. `FunctionDBData`: одна строка → `Constant`, иначе `Argument`.
4. Выведение не проверяет и не перекраивает таблицы. Несоответствия сообщает `MaterialValidator`.

Факт: в `Materials_v7.jsf` 43 материала, у всех есть «Структура». После выведения все тепловые и механические таблицы получают `TemperaturePhase`.

### 6.4. Сохранение и чтение в обе стороны

* Новые версии всегда пишут `ArgumentKind` явно, `Undefined` не пишется. Пересохранение старой базы и есть её миграция.
* Клиенты ≤ 1.1.1 игнорируют неизвестное поле и новую категорию. Таблицы `Argument` без фаз и `TemperaturePressure` они не посчитают, это допустимо.

## 7. Валидация (`MaterialValidator`)

| № | Правило | Уровень |
| --- | --- | --- |
| V1 | `ArgumentKind != Undefined` (материалы и функции) | Error |
| V2 | Колонки и строки соответствуют 5.1 (имена колонок `Argument` не нормативны — Warning) | Error |
| V3 | Аргумент строго возрастает; заголовки `P_` разбираются и строго возрастают | Error |
| V4 | Числовые ячейки не `DBNull` и конечны | Error |
| V5 | `TemperaturePhase`: набор и порядок фаз совпадают со «Структурой» (сейчас `CalcProp` сопоставляет по позиции) | Error |
| V6 | `TemperaturePhase` или `Process` без металлургии; `TemperaturePressure` у Solid | Error |
| V7 | Полнота по 5.3, включая строку «M» в «Модели плотности» при `IdealGas` и строки μ0, T0, S в «Модели вязкости» при `Sutherland` | Error |
| V8 | `IdealGas` или `Sutherland` у не-Gas; «M», опорное давление, μ0, T0, S ≤ 0 | Error |
| V9 | Металлургия у Liquid/Gas (запрещено решением пользователя) | Error |
| V10 | Сумма долей «Структуры» ≠ 1 (±1e-6) | Warning |
| V11 | «Плотность» задана при `IdealGas` (будет проигнорирована; остаётся после `UseIdealGasDensity` — решение пользователя оставить предупреждение) | Warning |
| V12 | «Динамическая вязкость» задана при `Sutherland` (будет проигнорирована; остаётся после `UseSutherlandViscosity` — решение пользователя оставить предупреждение) | Warning |
| V13 | «Модель материала» = 2: нет любой из троек X/Y/Z (λ — все среды; E, ТКЛР — Solid); E или ТКЛР X/Y/Z у Liquid/Gas | Error |
| V14 | «Модель твердости» = 1: нет металлургии, в записи нет колонки индекса, индекс не −1 и вне «Структуры», индексы ≠ −1 повторяются | Error |
| V15 | Категория текучей среды не соответствует MediumKind: Gas у жидкости или твёрдого, Fluid у газа или твёрдого (непустая) | Error |

Функции проверяются по V1–V4.

## 8. План версий и миграции

Правило AGENTS.md: `<Version>` в `.csproj` и та же версия в `PublishVersion`; публикация — push в `Master_dev`. В рамках этой спецификации файлы не меняются. Пакет Project ссылается на MaterialDB (`ProjectReference`) и транзитивно тянет его, поэтому его версия поднимается **синхронно**. PreProc и OperationalController ссылаются на Project и MaterialDB; при 2.0.0 их нужно пересобрать и проверить. Нужно ли поднимать их версии, решается при реализации.

| Шаг | MaterialDB | Project | Содержание |
| --- | --- | --- | --- |
| 1 | **1.2.0** | **6.3.0** (сейчас 6.2.3) | Содержит одно ломающее изменение — абстрактный `MaterialDBItem` (раздел 4.3, решение пользователя «в текущей версии»); по semver ему соответствует 2.0.0. Добавляются `ArgumentKind` (в `DataItem` + конструкторы `[JsonConstructor]` у `PropertyItem`/`FunctionItem`), `MediumKind`, `DensityModel`, `ViscosityModel`, `HardnessPhaseIndices`, свойства `MaterialDBItem.Medium`/`HasMetallurgy`, категории Fluid и Gas, новые записи «Общих сведений», резолвер, валидатор, `PressureColumnConvention`. Исправляются ошибки 10.4–10.5 (`PropertyEnum.Capacity`, индексатор `CategoryData[PropertyEnum]`) — исправление ошибок, а не смена контракта: индексатор сейчас неработоспособен. Добавляется ортотропия (тройки X/Y/Z). `[Obsolete]` получают `PropertyItem.CalcProp`, `FunctionItem.CalcProp`, `InterpolationSearch`, а также типы `PropertyItem`, `FunctionItem`, `IProperty` с текстом «будет удалён в MaterialDB 2.0.0, используйте DataItem» (сделано в коде). Репозиторий задаёт `CS0618 = error` (`.editorconfig`), поэтому места использования помечены точечными `#pragma warning disable CS0618`, внутри MaterialDB — `NoWarn`. В Project 6.3.0 `TableParameter.GetValue` переводится на собственную интерполяцию (шаг 5 ниже), чтобы не вызывать устаревший `CalcProp`; публичный API Project не меняется |
| 2 | — | — | PropertiesCalculator переходит на 1.2.0. Он реализует `MaterialCompiler`/`CompiledMaterial`, заменяет `InterpolationSearch` собственной `PiecewiseLinearTable` (`Kostinen`, `TableModel`, `Avrami`; см. PropertiesCalculator-Spec.md), `InterpolationSearch` не переносится, а SolverCore (`HeatTask`, `SetPhysicalProperties`) переводит на `CompiledMaterial`. Критерий готовности — нет предупреждений CS0618 |
| 3 | — | — | Миграция данных: `Materials_v7.jsf` → `Materials_v8.jsf`, `functions.jsf` пересохраняются через 1.2.0 с явным `ArgumentKind`. Проверка — в файлах нет `"$type"` |
| 4 | **2.0.0** | **7.0.0** | Ломающие изменения. `DataItem` становится `sealed` конкретным классом; удаляются `PropertyItem`, `FunctionItem`, `IProperty`, `CalcProp` ×2, `InterpolationSearch`; `PropertyData`, `FunctionDBData`, `ICategory`, `Category`, `Process<T>`, `ProcessData` типизируются по `DataItem`. В Project `TableParameter.Table` меняет тип `FunctionItem` → `DataItem`: публичный API, поэтому мажорная версия. |
| 5 | (часть шагов 1 и 4) | | **Project.TableParameter.** Одномерная интерполяция остаётся в BazisCore внутри Project, не в MaterialDB: Project не может ссылаться на PropertiesCalculator из BazisSolver. При первом вызове `GetValue` (или в конструкторе) `TableParameter` один раз переводит `DataItem` (`Argument`/`Constant`) в `double[] x`, `double[] y`, затем на каждом вызове выполняет бинарный поиск и линейную интерполяцию с насыщением на краях, как в текущем `InterpolationSearch`. Предлагаемый экземплярный класс — `Project.Tasks.Functions.LinearTable` (конструктор `LinearTable(DataItem table)`, метод `Interpolate(double x) : double`). `Constant` → значение единственной строки. `TableParameter` доверяет `ArgumentKind` (контракт 2.1). `LinearTable` строится один раз в конструкторе `TableParameter` и является снимком таблицы: правки функции в GUI учитываются при повторном разборе задачи, когда `TaskData` пересоздаёт параметры (решение пользователя, вариант А); подписки на события `DataTable` нет. Поведение сохраняется: за пределами таблицы — крайнее значение, одна строка — константа; `float` → `double`, перебор интервала → двоичный поиск, без выделений памяти в `GetValue()` |

Затрагиваемые файлы BazisCore (для @core-developer):

| Файл | Шаг 1 (1.2.0) | Шаг 4 (2.0.0) |
| --- | --- | --- |
| `Project/Tasks/Functions/TableParameter.cs` (`Table.CalcProp` в `GetValue`, строка 37) | `GetValue` → `LinearTable` | `FunctionItem Table` → `DataItem Table`, конструктор `TableParameter(DataItem table, …)` |
| `TestProject/MaterialDB/TestMaterialDB.cs` (`property.CalcProp`, `function.CalcProp` — строки 41, 61, 113) | тесты остаются, CS0618 подавляется точечно | тесты расчёта переносятся в тесты `LinearTable` (Project) и PropertiesCalculator; здесь остаются загрузка, резолвер, валидатор |
| `TestProject/PreProc/PhysicalTaskPreparationTests.cs` (`new PropertyItem(…)` — строки 50, 53) | без изменений (CS0618) | `new DataItem(…, ArgumentKind.Descriptor)` |
| `TestProject/Project/ProjectPathTests.cs` (`new FunctionItem(…)` — строка 62) | без изменений (CS0618) | `new DataItem(…, ArgumentKind.Argument)` |
| `MaterialDB/MaterialData/MetallurgicalData/Process.cs`, `ProcessData.cs` | — | `PropertyItem` → `DataItem` |

`ArgumentKindResolver` остаётся и в 2.0.0, чтобы читать архивные базы v1/v7.

## 9. Тестирование (для @core-tester)

* Резолвер на `Materials_v7.jsf` и `functions.jsf`: тепловые и механические таблицы → `TemperaturePhase`; «Структура» → `Descriptor`; процессы «Металлургии» → `Process`; «Модель упрочнения» → `Constant`; функции → `Argument`/`Constant`. Явное значение не перезаписывается. `[OnDeserialized]` у `FunctionDBData` действительно вызывается.
* Цикл «загрузка → сохранение → загрузка» сохраняет `ArgumentKind`, «Агрегатное состояние», «Модель плотности» (со строкой «M»), «Модель вязкости» (со строками μ0, T0, S); в JSON **нет `"$type"`**.
* 2.0.0: `materials_v1.jsf`, `Materials_v7.jsf`, `functions.jsf` загружаются в `DataItem` без ошибок.
* Класс материала выбирается по записи «Агрегатное состояние» (нет записи — `SolidMaterialItem`); `Copy` сохраняет класс; неизвестное значение отклоняется; в сохранённом файле нет `$type`.
* Материал без «Структуры» (жидкость): `GetPhases()` пустая, конструктор создаёт таблицы `Температура`, `Значение` с `Argument`.
* Твёрдое тело по умолчанию: фаза «phase» с долей 1, таблицы `TemperaturePhase`, «Кристаллизация» 1400/1500, «Модель упрочнения» 2, «Модель твердости» 0, пустая «Металлургия»; без фаз, с TS ≥ TL или с твердостью 1 без индексов — `ArgumentException`.
* `AddPhase` добавляет колонку из нулей во все таблицы `TemperaturePhase`, повтор — `ArgumentException`. `RemovePhase` удаляет колонки и процессы фазы, сдвигает индексы твердости; неизвестная фаза — `ArgumentException`, последняя — `InvalidOperationException`.
* Газ: `Use*`-методы меняют записи моделей на месте (тот же объект таблицы), добавляют и удаляют строки «M» и μ0, T0, S; таблицы «Плотность» и «Динамическая вязкость» сохраняются; повторный вызов безопасен.
* Валидатор: нет строки «M» или μ0/T0/S при нетабличной модели — V7; отрицательные значения — V8.
* Конструкторы с настройками: газ с `IdealGas`/`Sutherland` не получает «Плотность» и таблицу вязкости; без молярной массы или коэффициентов — `ArgumentException`; «Опорное давление» по умолчанию 1 МПа; `Copy()` сохраняет состав.
* `CheckProps()` наследника падает и при отсутствии тепловых свойств, и при отсутствии свойств своей среды.
* `PressureColumnConvention`: `Format(0.101325) == "P_0.101325"`, разбор при культуре ru-RU, отказ на `"P_abc"`.
* `MaterialValidator`: по одному негативному тесту на каждое правило V1–V15; Solid, Liquid, Gas (`Table`/`IdealGas`, `Table`/`Sutherland`).
* `LinearTable` (Project): совпадение с текущим `InterpolationSearch` на `functions.jsf`, края, одна строка.

## 10. Расхождения с текущим кодом (обнаружены при анализе)

1. Четыре утверждённых `ArgumentKind` не описывают многострочные нечисловые таблицы «Общих сведений» и процессы «Металлургии» → добавлены `Descriptor` и `Process` (решено).
2. Единицы базы: мм, кг, МПа, °C (плотность в кг/мм³). Давление принято в МПа (решено). Для идеального газа и Сазерленда нужен перевод °C → K.
3. `PropertyItem.CalcProp` сопоставляет колонки фаз по позиции, а не по имени (правило V5).
4. `Category.PropertyDic`: `PropertyEnum.Capacity` → «Общие сведения» вместо «Теплоемкость» — исправляется в 1.2.0.
5. `CategoryData[PropertyEnum]` ищет по `key.ToString()` (английское имя) при русских ключах и всегда бросает `KeyNotFoundException` — исправляется в 1.2.0.
6. SolverCore (`HeatTask.cs:470–476`) читает «Теплопроводность X/Y/Z» (а `SetPhysicalProperties` — «Модуль Юнга X/Y/Z», «ТКЛР X/Y/Z»), которых MaterialDB не знает — узакониваются в 1.2.0 (раздел 4.2).
7. `Kostinen`, `TableModel`, `Avrami` (PropertiesCalculator) используют `MaterialDB.Utilities.InterpolationSearch` (шаг 2).
8. `PropertiesCalculator.csproj`: `PackageReference MaterialDB 1.1.1` указан дважды, второй раз без условия.
9. В PropertiesCalculator лежат исключённые из компиляции копии `MaterialData/`, `Interfaces/`, `Utilities/`.
10. `Project/Tasks/Functions/TableParameter.cs` вызывает `FunctionItem.CalcProp`. Устраняется шагом 5 плана.
11. `TableParameter.cs` содержит неиспользуемый `using static IronPython.Modules._ast;`. Его стоит убрать при правке файла.

## 11. Открытые вопросы

Открытых вопросов нет.

Закрыты решениями пользователя (2026-11-06):

* **В1** — добавлены `ArgumentKind.Descriptor` (описательные таблицы «Общих сведений») и `ArgumentKind.Process` (процессы «Металлургии»).
* **В2** — давление в МПа (`P_…`, «Опорное давление»); μ0 — МПа·с.
* **В3** — `ArgumentKind.Temperature` переименован в `Argument`.
* **В4** — плотность всех сред только в «Тепловых свойствах».
* **В5** — металлургия у Liquid/Gas запрещена (V9).
* **В7** — `MediumKind`, `DensityModel`, `ViscosityModel` хранятся как `int`.
* **В11** — ключи строк `μ0`, `T0`, `S` (согласовано с PropertiesCalculator).
* **В6** — повторный `Add*` в «Общих сведениях» бросает исключение, как сейчас.
* **В8** — ортотропия нужна: тройки X/Y/Z для λ, E, ТКЛР при «Модель материала» = 2 (раздел 4.2).
* **В9** — ошибки `PropertyEnum.Capacity` и индексатора `CategoryData[PropertyEnum]` исправляются в 1.2.0.
* **В10** — `this[row, col]`, `AddRow()`, `ToString()` в `DataItem` сохраняются.
* **Индексы фаз модели твердости** — хранятся в файле материала в той же записи «Модель твердости» (раздел 5.2; правило V14); общей таблицы ролей фаз нет, порядок фаз в «Структуре» произвольный.
