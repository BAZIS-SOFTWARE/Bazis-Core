using MaterialDB.Utilities;
using System;
using System.Data;
using System.Linq;

namespace MaterialDB.FunctionData
{
    /// <summary>
    /// Табличная функция.
    /// </summary>
    [Serializable]
    public class FunctionItem : DataItem
    {
        /// <summary>
        /// Создаёт функцию с табличными данными.
        /// </summary>
        public FunctionItem(string name, string x_unit, string y_unit, string units, DataTable dataTable)
            : base(name, x_unit, y_unit, units, dataTable)
        {
        }

        /// <summary>
        /// Создаёт независимую копию функции.
        /// </summary>
        private FunctionItem(string copyName, FunctionItem source) : base(copyName, source)
        {
        }

        /// <summary>
        /// Создаёт копию функции с независимой таблицей.
        /// </summary>
        public override FunctionItem Copy(string copyName)
        {
            return new FunctionItem(copyName, this);
        }

        /// <summary>
        /// Вычисляет значение функции по её аргументу.
        /// </summary>
        public float CalcProp(float variable)
        {
            var xAr = DataTable.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[0])).ToArray();
            var yAr = DataTable.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[1])).ToArray();
            return InterpolationSearch.InterpolatedValue(xAr, yAr, variable);
        }
    }
}
