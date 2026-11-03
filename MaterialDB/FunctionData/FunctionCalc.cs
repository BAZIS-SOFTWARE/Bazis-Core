using MaterialDB.Utilities;
using System;
using System.Data;
using System.Linq;

namespace MaterialDB.FunctionData
{
    public class FunctionCalc
    {
        /// <summary>
        /// Вычисляет значение табличной функции по её аргументу.
        /// </summary>
        public float CalcProp(DataTable functionData, float variable)
        {
            var xAr = functionData.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[0])).ToArray();
            var yAr = functionData.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[1])).ToArray();
            return InterpolationSearch.InterpolatedValue(xAr, yAr, variable);
        }
    }
}
