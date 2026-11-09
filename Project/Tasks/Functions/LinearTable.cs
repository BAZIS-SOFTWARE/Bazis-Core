using MaterialDB;
using System;

namespace Project.Tasks.Functions
{
    /// <summary>
    /// Табличная функция одной переменной, один раз переведённая из DataItem в массивы.
    /// Снимок таблицы: последующие правки DataItem не учитываются.
    /// </summary>
    public sealed class LinearTable
    {
        private readonly double[] arguments;
        private readonly double[] values;

        /// <summary>
        /// Переводит таблицу функции в массивы. Constant — значение единственной строки (последняя колонка),
        /// иначе колонка 0 — аргумент, колонка 1 — значение.
        /// </summary>
        public LinearTable(DataItem table)
        {
            var rows = table.DataTable.Rows;
            var valueColumn = table.ArgumentKind == ArgumentKind.Constant ? table.DataTable.Columns.Count - 1 : 1;

            arguments = new double[rows.Count];
            values = new double[rows.Count];

            for (var row = 0; row < rows.Count; row++)
            {
                arguments[row] = Convert.ToDouble(rows[row][0]);
                values[row] = Convert.ToDouble(rows[row][valueColumn]);
            }
        }

        /// <summary>
        /// Линейная интерполяция; за пределами таблицы — крайнее значение.
        /// </summary>
        public double Interpolate(double argument)
        {
            var last = arguments.Length - 1;
            if (last <= 0 || argument <= arguments[0])
                return values[0];

            if (argument >= arguments[last])
                return values[last];

            var index = Array.BinarySearch(arguments, argument);
            if (index >= 0)
                return values[index];

            var upper = ~index;
            var lower = upper - 1;
            var ratio = (argument - arguments[lower]) / (arguments[upper] - arguments[lower]);

            return values[lower] + ratio * (values[upper] - values[lower]);
        }
    }
}
