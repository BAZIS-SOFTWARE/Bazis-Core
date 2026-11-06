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
    Descriptor = 5,          // описательная нечисловая таблица: «Структура», химсостав, параметры зерна, коэффициенты Сазерленда
    Process = 6              // процесс металлургии: «Температура» × «Масс.Доли_<φ>» (или «Температура», доля)
}
```

* Значения перечислений заданы явно. `ArgumentKind` хранится в JSON строкой, поэтому переименование `Temperature` → `Argument` сделано до первой публикации 1.2.0 и не затрагивает ни один файл.
* `Descriptor` и `Process` утверждены пользователем (2026-11-06). `Descriptor` — многострочные описательные таблицы «Общих сведений» («Структура», «Химический состав», «Модель роста аустенитного зерна», «Коэффициенты Сазерленда»); они не интерполируются. `Process` — числовые таблицы процессов «Металлургии» («Нагрев A-M» и т.п.), которые PropertiesCalculator компилирует в `CompiledProcess`.

### 4.2. Изменённые перечисления

```csharp
public enum CategoryEnum { General, Thermal, Mechanical, Metallurgical, Fluid /* new: «Гидродинамические свойства» */ }

public enum PropertyEnum
{
    // 12 существующих значений без изменений …
    MediumKind,             // «Агрегатное состояние»
    DensityModel,           // «Модель плотности»
    MolarMass,              // «Молярная масса»
    ReferencePressure,      // «Опорное давление»
    ViscosityModel,         // «Модель вязкости»
    SutherlandCoefficients, // «Коэффициенты Сазерленда»
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
    HardnessModel,          // «Модель твердости»: 0 — выключена, 1 — включена (модель Куркина)
    HardnessPhaseIndices    // «Индексы фаз модели твердости»: индексы фаз модели Куркина в «Структуре»
}
```

`MaterialDBItem.CategoryDic` получает пару `{ Fluid, "Гидродинамические свойства" }`, а `Category.PropertyDic` — двадцать новых пар (девять для сред, девять ортотропных и две для модели твердости). Новые значения добавляются в конец перечислений.

**Ортотропия (решение пользователя).** SolverCore уже читает «Теплопроводность X/Y/Z», «Модуль Юнга X/Y/Z», «ТКЛР X/Y/Z» при «Модель материала» = 2, а MaterialDB их не знал. Формат их узаконивает: при «Модель материала» = 2 вместо скалярных «Теплопроводность», «Модуль Юнга», «ТКЛР» обязательны их тройки X/Y/Z (той же раскладки `TemperaturePhase`/`Argument`); при 1 или отсутствии поля — скалярные. «Модель материала»: 1 — изотропный (по умолчанию), 2 — ортотропный; хранится как `int`. Ортотропия допустима для всех сред только для λ; E и ТКЛР — только Solid.

### 4.3. `MaterialDescriptor` (новый DTO, только чтение)

```csharp
public sealed class MaterialDescriptor
{
    public MaterialDescriptor(MediumKind medium, bool hasMetallurgy, DensityModel densityModel, ViscosityModel viscosityModel, double? molarMass, double? referencePressure);
    public MediumKind Medium { get; }
    public bool HasMetallurgy { get; }
    public DensityModel DensityModel { get; }
    public ViscosityModel ViscosityModel { get; }
    public double? MolarMass { get; }          // кг/моль
    public double? ReferencePressure { get; }  // МПа
}
```

Это единая точка, через которую PropertiesCalculator и GUI узнают класс материала. `MaterialDBItem` получает один метод `GetDescriptor()` вместо набора атомарных геттеров. При отсутствии данных действуют значения по умолчанию: `Solid`, `Table`, `Table`, `null`, `null`, поэтому старые базы читаются как раньше. Коэффициенты Сазерленда в дескриптор не входят. Это табличные данные, их читает компилятор (раздел 5.4).

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

* запись в `PropertyData` категорий Thermal, Mechanical или Fluid — свойство материала, X = температура;
* запись в «Общих сведениях» или «Металлургии» — описатель;
* запись в `FunctionDBData` — функция с произвольным X.

`MaterialValidator` проверяет `MaterialDBItem` и знает категорию каждой записи. Для функций применяется отдельный набор правил (V1–V4).

### 4.6. `MaterialDBItem` (изменён)

```csharp
public MaterialDescriptor GetDescriptor();                                  // new
public void AddMediumKind(MediumKind medium);                               // new: «Агрегатное состояние», int
public void AddDensityModel(DensityModel model);                            // new: «Модель плотности», int
public void AddViscosityModel(ViscosityModel model);                        // new: «Модель вязкости», int
public void AddHardnessModel(int mode);                               // new: «Модель твердости», int (0/1)
public void AddHardnessPhaseIndices(HardnessPhaseIndices indices);    // new: «Индексы фаз модели твердости», Descriptor
public void AddSutherlandCoefficients(double mu0, double t0, double s);     // new: «Коэффициенты Сазерленда»
public void AddMolarMass(double molarMass);                                 // new: «Молярная масса», кг/моль
public void AddReferencePressure(double pressure);                          // new: «Опорное давление»
public void AddFluidProps();                                                // new
public bool CheckFluidProps();                                              // new, бросает Exception, как CheckThermalProps
[OnDeserialized] private void OnDeserialized(StreamingContext context);     // new, вызывает IArgumentKindResolver
```

Изменения поведения:

| Член | Было | Станет |
| --- | --- | --- |
| `GetPhases()` | `KeyNotFoundException` без «Структуры» | пустая последовательность без металлургии |
| `AddThermalProps()`, `AddMechProps()` | «Температура» + фазы | раскладка по `GetDescriptor()`: при металлургии `TemperaturePhase`, иначе `Argument`; ArgumentKind задаётся явно; при «Модель материала» = 2 создаются тройки X/Y/Z вместо скалярных λ, E, ТКЛР |
| `CheckThermalProps()`, `CheckMechanicalProps()` (ортотропия) | скалярные λ, E, ТКЛР | при «Модель материала» = 2 требуют тройки X/Y/Z |
| `Category.PropertyDic` | `PropertyEnum.Capacity` → «Общие сведения» (ошибка) | `Capacity` → «Теплоемкость» (исправление в 1.2.0) |
| `CategoryData[PropertyEnum]` | ищет по `key.ToString()` (английское имя) и всегда бросает `KeyNotFoundException` | ищет по русскому имени категории через `CategoryDic`/`PropertyDic` (исправление в 1.2.0) |
| `Add*` в «Общих сведениях» при повторном вызове | исключение (`Dictionary.Add`) | без изменений: исключение остаётся (решение пользователя) |
| `AddPhaseData`, `AddCrystallization`, `AddMaterialModel`, `AddHardeningModel` | `new PropertyItem(…)` | `new DataItem(…, ArgumentKind.Descriptor/Constant)` |
| `CheckThermalProps()` | требует λ, c, ρ | ρ не требуется при `DensityModel.IdealGas` |
| `CheckFluidProps()` | — | для Liquid/Gas: «Динамическая вязкость» при `ViscosityModel.Table`, «Коэффициенты Сазерленда» при `Sutherland` |
| `CreateTable()` (private) | без параметров | `CreateTable(ArgumentKind kind)`; для `TemperaturePressure` создаёт только «Температура», колонки давлений добавляет клиент через `PressureColumnConvention` |

`AddFluidProps()` создаёт категорию «Гидродинамические свойства». При `ViscosityModel.Table` она добавляет «Динамическая вязкость» (`Argument`, для `Gas` — `TemperaturePressure`). «Сжимаемость» и «Коэффициент объёмного расширения» необязательны и методом не создаются.

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
| «Модель плотности» (`DensityModel`) | `Модель плотности` (int) | 1 | Constant* | 0 Table, 1 IdealGas |
| «Модель вязкости» (`ViscosityModel`) | `Модель вязкости` (int) | 1 | Constant* | 0 Table, 1 Sutherland |
| «Коэффициенты Сазерленда» (`SutherlandCoefficients`) | `Коэффициент` (string), `Значение` (double) | 3: `μ0`, `T0`, `S` | Descriptor | μ0 — МПа·с (Н·с/мм²); T0, S — К |
| «Молярная масса» (`MolarMass`) | `Молярная масса` (double) | 1 | Constant* | кг/моль |
| «Опорное давление» (`ReferencePressure`) | `Опорное давление` (double) | 1 | Constant* | МПа |
| «Модель твердости» (`HardnessModel`) | `Модель твердости` (int) | 1 | Constant* | 0 — выключена, 1 — включена (Куркин) |
| «Индексы фаз модели твердости» (`HardnessPhaseIndices`) | `Коэффициент` (string), `Значение` (int) | 5: `FerriteIndex`, `PerliteIndex`, `BeiniteIndex`, `MartensiteIndex`, `AusteniteIndex` | Descriptor | индекс строки фазы в «Структуре», с 0; −1 — фазы нет |

\* Колонка одна и названа по свойству, как в существующих «Модель упрочнения» и «Размер аустенитного зерна». Поэтому для записей «Общих сведений» правило 5.1 «колонка `Значение`» не проверяется: они сопоставляются по имени свойства.

**Модель твердости (решение пользователя).** Индексы фаз модели Куркина хранятся прямо в файле материала и при компиляции материала попадают в модель (PropertiesCalculator). Раскладка повторяет «Модель роста аустенитного зерна»: колонки `Коэффициент`/`Значение`, ключи строк совпадают с именами свойств модели (`FerriteIndex`, `PerliteIndex`, `BeiniteIndex`, `MartensiteIndex`, `AusteniteIndex`). Порядок фаз в «Структуре» произвольный: пользователь сам указывает, какая строка какой фазе соответствует, и отвечает за правильность. Запись обязательна при «Модель твердости» = 1. Записи создаются методами `AddHardnessModel(int mode)` и `AddHardnessPhaseIndices(HardnessPhaseIndices indices)` по образцу `AddHardeningModel` (повторный вызов — исключение); `HardnessPhaseIndices` — DTO MaterialDB с пятью свойствами `int` тех же имён.

«Коэффициенты Сазерленда» повторяют раскладку существующей «Модель роста аустенитного зерна» (`Коэффициент`, `Значение`). Строковые ключи строк `μ0`, `T0`, `S` согласованы с PropertiesCalculator. Формула Сазерленда для расчёта: μ(T) = μ0·(T/T0)^{3/2}·(T0 + S)/(T + S), T в кельвинах. Её применяет компилятор, MaterialDB только хранит коэффициенты.

### 5.3. Обязательные данные по среде

| Данные | Solid | Liquid | Gas |
| --- | --- | --- | --- |
| «Агрегатное состояние» | необязательно (по умолчанию Solid) | обязательно | обязательно |
| «Модель плотности» | только `Table` | `Table` | `Table` / `IdealGas` |
| «Молярная масса» | — | — | обязательно при `IdealGas` (> 0) |
| «Опорное давление» | — | при таблице `TemperaturePressure` | при `IdealGas` или таблице `TemperaturePressure` (> 0) |
| «Модель вязкости» | — | `Table` | `Table` / `Sutherland` |
| «Коэффициенты Сазерленда» | — | — | обязательно при `Sutherland` (μ0, T0, S > 0) |
| Fluid: «Динамическая вязкость» | — | обязательно (`Table`) | обязательно при `Table` |
| Thermal: «Теплопроводность», «Теплоемкость» | обязательно | обязательно | обязательно |
| Thermal: «Плотность» | обязательно | обязательно | обязательно при `Table` |
| «Механические свойства» (состав `CheckMechanicalProps`) | обязательно | — | — |
| «Структура» | необязательно | запрещено | запрещено |
| `TemperaturePhase` | только при металлургии | — | — |
| `TemperaturePressure` | запрещено | разрешено | разрешено |

### 5.4. Плотность и вязкость: один источник

* ρ для всех сред хранится только в «Тепловых свойствах» (решение пользователя): тепловая задача нужна каждой среде, а дублирование не допускается. При `IdealGas` PropertiesCalculator считает ρ = p·M/(R·T_K) с переводом в кг/мм³.
* Вязкость задаётся либо таблицей «Динамическая вязкость» в Fluid, либо коэффициентами Сазерленда в «Общих сведениях», но не обоими способами (V12).

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
* `MaterialDBItem` — `[OnDeserialized]` → `Resolve(this)`. Это минимальная единица с контекстом («Структура» видна только на уровне материала). Точка срабатывает при любом способе загрузки: `new MaterialDBData(name, folder)`, прямой `JsonConvert.DeserializeObject<MaterialDBData>` (так делают тесты BazisCore и BazisSolver), загрузка одного материала.
* `FunctionDBData` — `[OnDeserialized]` → `Resolve(this)`. В 2.0.0 `DataItem` не знает своего контейнера, поэтому точка запуска находится в контейнере. В 1.2.0 используется та же точка.
* Не выбраны: выведение в PropertiesCalculator (нарушает контракт 2.1, GUI видел бы `Undefined`) и выведение в `DataItem` (нет контекста).
* Риск для реализации: проверить тестом, что Newtonsoft вызывает `[OnDeserialized]` у типа, реализующего `IDictionary<,>` (`FunctionDBData`). Если не вызывает, точкой запуска становится конструктор `FunctionDBData(dbName, dbFolder)`.

### 6.3. Правило выведения (только для `Undefined`, явное значение не меняется)

1. «Общие сведения»: таблица 1×1 → `Constant`, остальные → `Descriptor`. «Металлургия»: все таблицы → `Process`.
2. Thermal, Mechanical, Fluid: если есть «Структура» → `TemperaturePhase`, иначе `Argument`.
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
| V7 | Полнота по 5.3 | Error |
| V8 | `IdealGas` или `Sutherland` у не-Gas; молярная масса, опорное давление, μ0, T0, S ≤ 0 | Error |
| V9 | Металлургия у Liquid/Gas (запрещено решением пользователя) | Error |
| V10 | Сумма долей «Структуры» ≠ 1 (±1e-6) | Warning |
| V11 | «Плотность» задана при `IdealGas` (будет проигнорирована) | Warning |
| V12 | «Динамическая вязкость» задана при `Sutherland` (будет проигнорирована) | Warning |
| V13 | «Модель материала» = 2: нет любой из троек X/Y/Z (λ — все среды; E, ТКЛР — Solid); E или ТКЛР X/Y/Z у Liquid/Gas | Error |
| V14 | «Модель твердости» = 1: нет «Индексов фаз модели твердости», нет металлургии, индекс не −1 и вне «Структуры», индексы ≠ −1 повторяются, неизвестный ключ строки | Error |

Функции проверяются по V1–V4.

## 8. План версий и миграции

Правило AGENTS.md: `<Version>` в `.csproj` и та же версия в `PublishVersion`; публикация — push в `Master_dev`. В рамках этой спецификации файлы не меняются. Пакет Project ссылается на MaterialDB (`ProjectReference`) и транзитивно тянет его, поэтому его версия поднимается **синхронно**. PreProc и OperationalController ссылаются на Project и MaterialDB; при 2.0.0 их нужно пересобрать и проверить. Нужно ли поднимать их версии, решается при реализации.

| Шаг | MaterialDB | Project | Содержание |
| --- | --- | --- | --- |
| 1 | **1.2.0** | **6.3.0** (сейчас 6.2.3) | Без ломающих изменений. Добавляются `ArgumentKind` (в `DataItem` + конструкторы `[JsonConstructor]` у `PropertyItem`/`FunctionItem`), `MediumKind`, `DensityModel`, `ViscosityModel`, `MaterialDescriptor`, категория Fluid, новые записи «Общих сведений», резолвер, валидатор, `PressureColumnConvention`. Исправляются ошибки 10.4–10.5 (`PropertyEnum.Capacity`, индексатор `CategoryData[PropertyEnum]`) — исправление ошибок, а не смена контракта: индексатор сейчас неработоспособен. Добавляется ортотропия (тройки X/Y/Z). `[Obsolete]` получают `PropertyItem.CalcProp`, `FunctionItem.CalcProp`, `InterpolationSearch`, а также типы `PropertyItem`, `FunctionItem`, `IProperty` с текстом «будет удалён в MaterialDB 2.0.0, используйте DataItem». В Project 6.3.0 `TableParameter.GetValue` переводится на собственную интерполяцию (шаг 5 ниже), чтобы не вызывать устаревший `CalcProp`; публичный API Project не меняется |
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
* Цикл «загрузка → сохранение → загрузка» сохраняет `ArgumentKind`, «Агрегатное состояние», «Модель плотности», «Модель вязкости», «Коэффициенты Сазерленда»; в JSON **нет `"$type"`**.
* 2.0.0: `materials_v1.jsf`, `Materials_v7.jsf`, `functions.jsf` загружаются в `DataItem` без ошибок.
* Материал без «Структуры»: `GetPhases()` пустая, `AddThermalProps()` даёт `Температура`, `Значение` с `Argument`.
* `PressureColumnConvention`: `Format(0.101325) == "P_0.101325"`, разбор при культуре ru-RU, отказ на `"P_abc"`.
* `MaterialValidator`: по одному негативному тесту на каждое правило V1–V14; Solid, Liquid, Gas (`Table`/`IdealGas`, `Table`/`Sutherland`).
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
* **Индексы фаз модели твердости** — хранятся в файле материала (запись «Индексы фаз модели твердости», раздел 5.2; правило V14); общей таблицы ролей фаз нет, порядок фаз в «Структуре» произвольный.
