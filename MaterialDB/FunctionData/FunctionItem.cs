using MaterialDB.Utilities;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;

namespace MaterialDB.FunctionData
{
    /// <summary>
    /// Табличная функция.
    /// </summary>
    [Serializable]
    [Obsolete("Тип будет удалён в MaterialDB 2.0.0; используйте DataItem.")]
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
        /// Создаёт функцию с табличными данными и видом зависимости.
        /// </summary>
        [JsonConstructor]
        public FunctionItem(string name, string x_unit, string y_unit, string units, DataTable dataTable, ArgumentKind argumentKind)
            : base(name, x_unit, y_unit, units, dataTable, argumentKind)
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
        [Obsolete("Табличная функция вычисляется в Project (LinearTable); метод будет удалён в MaterialDB 2.0.0.")]
        public float CalcProp(float variable)
        {
            var xAr = DataTable.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[0])).ToArray();
            var yAr = DataTable.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[1])).ToArray();
            return InterpolationSearch.InterpolatedValue(xAr, yAr, variable);
        }
    }
}
