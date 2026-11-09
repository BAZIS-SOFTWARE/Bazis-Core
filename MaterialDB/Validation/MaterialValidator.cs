using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using System.Data;

namespace MaterialDB.Validation
{
    /// <summary>
    /// Проверка материалов (V1–V15) и функций (V1–V4). Каждое сообщение начинается с номера правила.
    /// </summary>
    public sealed class MaterialValidator : IMaterialValidator
    {
        private readonly PressureColumnConvention pressureColumns;

        private readonly string[] hardnessIndexNames =
        {
            nameof(HardnessPhaseIndices.FerriteIndex),
            nameof(HardnessPhaseIndices.PerliteIndex),
            nameof(HardnessPhaseIndices.BeiniteIndex),
            nameof(HardnessPhaseIndices.MartensiteIndex),
            nameof(HardnessPhaseIndices.AusteniteIndex)
        };

        /// <summary>
        /// Создаёт проверку с соглашением об именах колонок давления.
        /// </summary>
        public MaterialValidator(PressureColumnConvention pressureColumns)
        {
            this.pressureColumns = pressureColumns;
        }

        /// <inheritdoc/>
        public IEnumerable<ValidationIssue> Validate(MaterialDBItem material)
        {
            var phases = material.GetPhases().ToList();
            var context = new MaterialContext(material, phases);

            foreach (var category in material.CategoryData)
            {
                foreach (var property in category.Value.PropertyData)
                {
                    foreach (var issue in ValidateItem(context, category.Key, property.Value))
                        yield return issue;
                }
            }

            foreach (var issue in ValidateMedium(context))
                yield return issue;

            foreach (var issue in ValidateCompleteness(context))
                yield return issue;

            foreach (var issue in ValidateStructure(context))
                yield return issue;

            foreach (var issue in ValidateHardness(context))
                yield return issue;
        }

        /// <inheritdoc/>
        public IEnumerable<ValidationIssue> Validate(FunctionDBData functions)
        {
            foreach (var function in functions)
            {
                var issues = ValidateTable(functions.Name ?? "", "", function.Key, function.Value, false);
                foreach (var issue in issues)
                    yield return issue;
            }
        }

        private IEnumerable<ValidationIssue> ValidateItem(MaterialContext context, string category, DataItem item)
        {
            var source = context.Material.Name;
            var name = item.Name;

            foreach (var issue in ValidateTable(source, category, name, item, true))
                yield return issue;

            if (item.ArgumentKind == ArgumentKind.TemperaturePhase && !context.Material.HasMetallurgy)
                yield return Error(source, category, name, "V6: таблица TemperaturePhase у материала без «Структуры».");

            if (item.ArgumentKind == ArgumentKind.Process && !context.Material.HasMetallurgy)
                yield return Error(source, category, name, "V6: процесс металлургии у материала без «Структуры».");

            if (item.ArgumentKind == ArgumentKind.TemperaturePressure && context.Material.Medium == MediumKind.Solid)
                yield return Error(source, category, name, "V6: таблица TemperaturePressure у твёрдого материала.");

            if (item.ArgumentKind == ArgumentKind.TemperaturePhase && item.DataTable != null)
            {
                foreach (var issue in ValidatePhaseColumns(context, category, item))
                    yield return issue;
            }
        }

        private IEnumerable<ValidationIssue> ValidateTable(string source, string category, string name, DataItem item, bool isMaterialProperty)
        {
            if (item.ArgumentKind == ArgumentKind.Undefined)
            {
                yield return Error(source, category, name, "V1: вид зависимости ArgumentKind не задан.");
                yield break;
            }

            var table = item.DataTable;
            if (table == null)
            {
                yield return Error(source, category, name, "V2: таблица отсутствует.");
                yield break;
            }

            if (item.ArgumentKind == ArgumentKind.Descriptor)
                yield break;

            foreach (var issue in ValidateShape(source, category, name, item, isMaterialProperty))
                yield return issue;

            if (!HasOnlyFiniteNumbers(table))
                yield return Error(source, category, name, "V4: в таблице есть пустые, нечисловые или бесконечные значения.");
            else if (item.ArgumentKind != ArgumentKind.Constant && !IsStrictlyIncreasing(table, 0))
                yield return Error(source, category, name, "V3: значения первой колонки не возрастают строго.");
        }

        private IEnumerable<ValidationIssue> ValidateShape(string source, string category, string name, DataItem item, bool isMaterialProperty)
        {
            var table = item.DataTable;
            var rows = table.Rows.Count;
            var columns = table.Columns.Count;

            switch (item.ArgumentKind)
            {
                case ArgumentKind.Constant:
                    if (rows != 1)
                        yield return Error(source, category, name, $"V2: таблица Constant должна содержать одну строку, а содержит {rows}.");
                    break;

                case ArgumentKind.Argument:
                    if (columns != 2 || rows < 1)
                        yield return Error(source, category, name, $"V2: таблица Argument должна содержать 2 колонки и хотя бы одну строку ({columns}×{rows}).");
                    else if (isMaterialProperty && (table.Columns[0].ColumnName != "Температура" || table.Columns[1].ColumnName != "Значение"))
                        yield return Warning(source, category, name, "V2: ожидаются колонки «Температура», «Значение».");
                    break;

                case ArgumentKind.TemperaturePhase:
                    if (columns < 2 || rows < 1)
                        yield return Error(source, category, name, "V2: таблица TemperaturePhase должна содержать «Температура», колонки фаз и хотя бы одну строку.");
                    break;

                case ArgumentKind.TemperaturePressure:
                    foreach (var issue in ValidatePressureColumns(source, category, name, table))
                        yield return issue;
                    break;

                case ArgumentKind.Process:
                    if (columns < 2 || rows < 2)
                        yield return Error(source, category, name, "V2: процесс должен содержать не менее 2 колонок и 2 строк.");
                    break;
            }
        }

        private IEnumerable<ValidationIssue> ValidatePressureColumns(string source, string category, string name, DataTable table)
        {
            if (table.Columns.Count < 2 || table.Rows.Count < 1)
            {
                yield return Error(source, category, name, "V2: таблица TemperaturePressure должна содержать «Температура», колонки давлений и хотя бы одну строку.");
                yield break;
            }

            var previous = double.NegativeInfinity;
            for (var column = 1; column < table.Columns.Count; column++)
            {
                var columnName = table.Columns[column].ColumnName;
                if (!pressureColumns.TryParse(columnName, out var pressure))
                {
                    yield return Error(source, category, name, $"V3: колонка «{columnName}» не соответствует соглашению «{pressureColumns.Prefix}_<МПа>».");
                    yield break;
                }

                if (pressure <= previous)
                {
                    yield return Error(source, category, name, "V3: давления в заголовках колонок не возрастают строго.");
                    yield break;
                }

                previous = pressure;
            }
        }

        private IEnumerable<ValidationIssue> ValidatePhaseColumns(MaterialContext context, string category, DataItem item)
        {
            var table = item.DataTable;
            var phases = context.Phases;
            var source = context.Material.Name;

            if (table.Columns.Count != phases.Count + 1)
            {
                yield return Error(source, category, item.Name, $"V5: число колонок фаз {table.Columns.Count - 1} не совпадает с числом фаз «Структуры» {phases.Count}.");
                yield break;
            }

            for (var index = 0; index < phases.Count; index++)
            {
                var columnName = table.Columns[index + 1].ColumnName;
                if (columnName != phases[index])
                {
                    yield return Error(source, category, item.Name, $"V5: колонка «{columnName}» не совпадает с фазой «{phases[index]}» «Структуры».");
                    yield break;
                }
            }
        }

        private IEnumerable<ValidationIssue> ValidateMedium(MaterialContext context)
        {
            var source = context.Material.Name;
            var general = context.Material.CategoryDic[CategoryEnum.General];
            var material = context.Material;
            var isGas = material.Medium == MediumKind.Gas;

            foreach (var categoryEnum in new[] { CategoryEnum.Fluid, CategoryEnum.Gas })
            {
                var categoryName = context.Material.CategoryDic[categoryEnum];
                if (categoryEnum != material.FluidCategory && context.Material.CategoryData.ContainsKey(categoryName) && context.Material[categoryName].PropertyData.Count > 0)
                    yield return Error(source, categoryName, "", $"V15: категория «{categoryName}» не соответствует среде материала ({material.Medium}).");
            }

            if (material.Medium != MediumKind.Solid && material.HasMetallurgy)
                yield return Error(source, general, "Структура", "V9: металлургия запрещена для жидкостей и газов.");

            if (material.DensityModel == DensityModel.IdealGas && !isGas)
                yield return Error(source, general, "Модель плотности", "V8: модель идеального газа допустима только для газа.");

            if (material.ViscosityModel == ViscosityModel.Sutherland && !isGas)
                yield return Error(source, general, "Модель вязкости", "V8: модель Сазерленда допустима только для газа.");

            if (context.MolarMass.HasValue && context.MolarMass.Value <= 0)
                yield return Error(source, general, "Модель плотности", "V8: молярная масса «M» должна быть больше нуля.");

            if (context.ReferencePressure.HasValue && context.ReferencePressure.Value <= 0)
                yield return Error(source, general, "Опорное давление", "V8: опорное давление должно быть больше нуля.");

            if (material.ViscosityModel == ViscosityModel.Sutherland && context.TryGetItem(CategoryEnum.General, "Модель вязкости", out var viscosityModel) && !HasPositiveValues(viscosityModel.DataTable, 1))
                yield return Error(source, general, "Модель вязкости", "V8: коэффициенты Сазерленда μ0, T0, S должны быть больше нуля.");

            if (material.DensityModel == DensityModel.IdealGas && context.TryGetItem(CategoryEnum.Thermal, "Плотность", out _))
                yield return Warning(source, context.Material.CategoryDic[CategoryEnum.Thermal], "Плотность", "V11: таблица плотности игнорируется при модели идеального газа.");

            if (material.ViscosityModel == ViscosityModel.Sutherland && context.TryGetItem(material.FluidCategory, "Динамическая вязкость", out _))
                yield return Warning(source, context.Material.CategoryDic[material.FluidCategory], "Динамическая вязкость", "V12: таблица вязкости игнорируется при модели Сазерленда.");

            if (material.Medium == MediumKind.Solid)
                yield break;

            foreach (var propertyName in GetDirectionalNames("Модуль Юнга").Concat(GetDirectionalNames("ТКЛР")))
            {
                if (context.TryGetItem(CategoryEnum.Mechanical, propertyName, out _))
                    yield return Error(source, context.Material.CategoryDic[CategoryEnum.Mechanical], propertyName, "V13: ортотропные механические свойства допустимы только для твёрдого материала.");
            }
        }

        private IEnumerable<ValidationIssue> ValidateCompleteness(MaterialContext context)
        {
            var material = context.Material;
            var source = context.Material.Name;
            var orthotropic = context.IsOrthotropic;

            foreach (var propertyName in RequiredThermal(material, orthotropic))
            {
                if (!context.TryGetItem(CategoryEnum.Thermal, propertyName, out _))
                    yield return Missing(context, CategoryEnum.Thermal, propertyName, IsDirectional(propertyName));
            }

            if (material.Medium == MediumKind.Solid)
            {
                foreach (var propertyName in RequiredMechanical(orthotropic))
                {
                    if (!context.TryGetItem(CategoryEnum.Mechanical, propertyName, out _))
                        yield return Missing(context, CategoryEnum.Mechanical, propertyName, IsDirectional(propertyName));
                }

                yield break;
            }

            if (material.ViscosityModel == ViscosityModel.Table && !context.TryGetItem(material.FluidCategory, "Динамическая вязкость", out _))
                yield return Missing(context, material.FluidCategory, "Динамическая вязкость", false);

            if (material.ViscosityModel == ViscosityModel.Sutherland)
            {
                foreach (var coefficient in new[] { "μ0", "T0", "S" })
                {
                    if (!material.TryGetModelParameter("Модель вязкости", coefficient, out _))
                        yield return Error(source, context.Material.CategoryDic[CategoryEnum.General], "Модель вязкости", $"V7: в «Модели вязкости» нет коэффициента Сазерленда «{coefficient}».");
                }
            }

            if (material.DensityModel == DensityModel.IdealGas && !context.MolarMass.HasValue)
                yield return Error(source, context.Material.CategoryDic[CategoryEnum.General], "Модель плотности", "V7: в «Модели плотности» нет молярной массы «M».");

            var needsPressure = material.DensityModel == DensityModel.IdealGas || context.HasPressureTables;
            if (needsPressure && !context.ReferencePressure.HasValue)
                yield return Missing(context, CategoryEnum.General, "Опорное давление", false);
        }

        private IEnumerable<ValidationIssue> ValidateStructure(MaterialContext context)
        {
            if (!context.TryGetItem(CategoryEnum.General, "Структура", out var structure) || structure.DataTable == null)
                yield break;

            var table = structure.DataTable;
            if (table.Columns.Count < 2)
                yield break;

            var sum = 0.0;
            foreach (DataRow row in table.Rows)
            {
                if (!TryGetNumber(row[1], out var fraction))
                    yield break;
                sum += fraction;
            }

            if (Math.Abs(sum - 1.0) > 1e-6)
                yield return Warning(context.Material.Name, context.Material.CategoryDic[CategoryEnum.General], "Структура", $"V10: сумма долей фаз равна {sum}, а не 1.");
        }

        private IEnumerable<ValidationIssue> ValidateHardness(MaterialContext context)
        {
            var material = context.Material;
            var source = material.Name;
            var general = material.CategoryDic[CategoryEnum.General];
            const string modelName = "Модель твердости";

            if (!material.TryGetGeneralValue(modelName, out var model) || (int)model != 1)
                yield break;

            if (!context.Material.HasMetallurgy)
                yield return Error(source, general, modelName, "V14: модель твердости требует «Структуры».");

            if (!context.TryGetItem(CategoryEnum.General, modelName, out var item) || item.DataTable == null || item.DataTable.Rows.Count == 0)
                yield break;

            var table = item.DataTable;
            var used = new HashSet<int>();
            foreach (var key in hardnessIndexNames)
            {
                if (!table.Columns.Contains(key))
                {
                    yield return Error(source, general, modelName, $"V14: при «Модель твердости» = 1 в записи нет колонки {key}.");
                    continue;
                }

                if (!TryGetNumber(table.Rows[0][key], out var value))
                {
                    yield return Error(source, general, modelName, $"V14: индекс {key} не задан.");
                    continue;
                }

                var index = (int)value;
                if (index == -1)
                    continue;

                if (index < 0 || index >= context.Phases.Count)
                    yield return Error(source, general, modelName, $"V14: индекс {key} = {index} вне «Структуры» (фаз {context.Phases.Count}).");
                else if (!used.Add(index))
                    yield return Error(source, general, modelName, $"V14: индекс {index} указан для нескольких фаз.");
            }
        }
        private IEnumerable<string> RequiredThermal(MaterialDBItem material, bool orthotropic)
        {
            foreach (var propertyName in GetDirectionalNames("Теплопроводность", orthotropic))
                yield return propertyName;

            yield return "Теплоемкость";

            if (material.DensityModel == DensityModel.Table)
                yield return "Плотность";
        }

        private IEnumerable<string> RequiredMechanical(bool orthotropic)
        {
            yield return "Предел текучести";
            yield return "Предел прочности";
            yield return "Коэффициент упрочнения";

            foreach (var propertyName in GetDirectionalNames("Модуль Юнга", orthotropic))
                yield return propertyName;

            foreach (var propertyName in GetDirectionalNames("ТКЛР", orthotropic))
                yield return propertyName;
        }

        private IEnumerable<string> GetDirectionalNames(string propertyName, bool orthotropic = true)
        {
            if (!orthotropic)
            {
                yield return propertyName;
                yield break;
            }

            yield return propertyName + " X";
            yield return propertyName + " Y";
            yield return propertyName + " Z";
        }

        private bool IsDirectional(string propertyName)
        {
            return propertyName.EndsWith(" X") || propertyName.EndsWith(" Y") || propertyName.EndsWith(" Z");
        }

        private ValidationIssue Missing(MaterialContext context, CategoryEnum category, string propertyName, bool directional)
        {
            var rule = directional ? "V13" : "V7";
            return Error(context.Material.Name, context.Material.CategoryDic[category], propertyName, $"{rule}: отсутствует обязательное свойство «{propertyName}».");
        }

        private bool HasOnlyFiniteNumbers(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (var cell in row.ItemArray)
                {
                    if (!TryGetNumber(cell, out _))
                        return false;
                }
            }

            return true;
        }

        private bool HasPositiveValues(DataTable table, int column)
        {
            if (table == null || table.Columns.Count <= column)
                return false;

            foreach (DataRow row in table.Rows)
            {
                if (!TryGetNumber(row[column], out var value) || value <= 0)
                    return false;
            }

            return true;
        }

        private bool IsStrictlyIncreasing(DataTable table, int column)
        {
            var previous = double.NegativeInfinity;
            foreach (DataRow row in table.Rows)
            {
                TryGetNumber(row[column], out var value);
                if (value <= previous)
                    return false;
                previous = value;
            }

            return true;
        }

        private bool TryGetNumber(object? cell, out double value)
        {
            value = 0;
            if (cell == null || cell is DBNull || cell is string || cell is not IConvertible)
                return false;

            value = Convert.ToDouble(cell);
            return double.IsFinite(value);
        }

        private ValidationIssue Error(string source, string category, string item, string message)
        {
            return new ValidationIssue(IssueSeverity.Error, source, category, item, message);
        }

        private ValidationIssue Warning(string source, string category, string item, string message)
        {
            return new ValidationIssue(IssueSeverity.Warning, source, category, item, message);
        }

        /// <summary>
        /// Данные материала, которые нужны нескольким правилам.
        /// </summary>
        private sealed class MaterialContext
        {
            public MaterialContext(MaterialDBItem material, List<string> phases)
            {
                Material = material;
                Phases = phases;
                MolarMass = material.TryGetModelParameter("Модель плотности", "M", out var molarMass) ? molarMass : null;
                ReferencePressure = material.TryGetGeneralValue("Опорное давление", out var pressure) ? pressure : null;
                IsOrthotropic = material.TryGetGeneralValue("Модель материала", out var model) && (int)model == 2;
                HasPressureTables = material.CategoryData.Values
                    .SelectMany(category => category.PropertyData.Values)
                    .Any(item => item.ArgumentKind == ArgumentKind.TemperaturePressure);
            }

            public MaterialDBItem Material { get; }
            public double? MolarMass { get; }
            public double? ReferencePressure { get; }
            public List<string> Phases { get; }
            public bool IsOrthotropic { get; }
            public bool HasPressureTables { get; }

            public bool TryGetItem(CategoryEnum category, string propertyName, out DataItem item)
            {
                item = null!;
                var categoryName = Material.CategoryDic[category];
                if (!Material.CategoryData.ContainsKey(categoryName))
                    return false;

                var properties = Material[categoryName].PropertyData;
                if (!properties.ContainsKey(propertyName))
                    return false;

                item = properties[propertyName];
                return true;
            }
        }
    }
}
