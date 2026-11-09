using System.Data;
using System.Globalization;

namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Соглашение об именах колонок давления в таблицах TemperaturePressure: «P_&lt;давление в МПа&gt;».
    /// </summary>
    public sealed class PressureColumnConvention
    {
        public string Prefix { get; } = "P";

        /// <summary>
        /// Имя колонки для давления: 0.101325 → «P_0.101325».
        /// </summary>
        public string Format(double pressure)
        {
            return $"{Prefix}_{pressure.ToString(CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// Разбирает имя колонки давления.
        /// </summary>
        public bool TryParse(string columnName, out double pressure)
        {
            pressure = 0;
            var start = Prefix + "_";
            if (!columnName.StartsWith(start, StringComparison.Ordinal))
                return false;

            return double.TryParse(columnName.Substring(start.Length), NumberStyles.Float, CultureInfo.InvariantCulture, out pressure);
        }

        /// <summary>
        /// Давления из заголовков колонок таблицы (первая колонка — температура).
        /// </summary>
        public IEnumerable<double> GetPressures(DataTable table)
        {
            for (var column = 1; column < table.Columns.Count; column++)
            {
                if (TryParse(table.Columns[column].ColumnName, out var pressure))
                    yield return pressure;
            }
        }
    }
}
