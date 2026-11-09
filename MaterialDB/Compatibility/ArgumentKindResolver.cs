using MaterialDB.FunctionData;
using MaterialDB.MaterialData;

namespace MaterialDB.Compatibility
{
    /// <summary>
    /// Правило выведения ArgumentKind для старых файлов:
    /// «Общие сведения» — 1×1 → Constant, иначе Descriptor; «Металлургия» — Process;
    /// остальные категории — TemperaturePhase при наличии «Структуры», иначе Argument;
    /// функции — одна строка → Constant, иначе Argument.
    /// </summary>
    public sealed class ArgumentKindResolver : IArgumentKindResolver
    {
        /// <inheritdoc/>
        public void Resolve(MaterialDBItem material)
        {
            var generalName = material.CategoryDic[CategoryEnum.General];
            var metallurgicalName = material.CategoryDic[CategoryEnum.Metallurgical];
            var hasMetallurgy = material.GetPhases().Any();

            foreach (var category in material.CategoryData)
            {
                foreach (var property in category.Value.PropertyData)
                {
                    var item = property.Value;
                    if (item.ArgumentKind != ArgumentKind.Undefined)
                        continue;

                    if (category.Key == generalName)
                        item.ArgumentKind = IsSingleCell(item) ? ArgumentKind.Constant : ArgumentKind.Descriptor;
                    else if (category.Key == metallurgicalName)
                        item.ArgumentKind = ArgumentKind.Process;
                    else
                        item.ArgumentKind = hasMetallurgy ? ArgumentKind.TemperaturePhase : ArgumentKind.Argument;
                }
            }
        }

        /// <inheritdoc/>
        public void Resolve(FunctionDBData functions)
        {
            foreach (var function in functions.Values)
            {
                if (function.ArgumentKind != ArgumentKind.Undefined)
                    continue;

                var rowCount = function.DataTable?.Rows.Count ?? 0;
                function.ArgumentKind = rowCount == 1 ? ArgumentKind.Constant : ArgumentKind.Argument;
            }
        }

        private bool IsSingleCell(DataItem item)
        {
            return item.DataTable != null && item.DataTable.Rows.Count == 1 && item.DataTable.Columns.Count == 1;
        }
    }
}
