using Project.Interfaces.Tasks;
using Project.Tasks.Utilities;
using System;
using System.Data;
using System.Linq;

namespace Project.Tasks.Functions
{
    public class TableFunction : ITableFunction
    {
        /// <inheritdoc/>
        public string Name { get; set; }
        /// <inheritdoc/>
        public string X_unit { get; set; }
        /// <inheritdoc/>
        public string Y_unit { get; set; }
        /// <inheritdoc/>
        public string Units { get; set; }
        /// <inheritdoc/>
        public DataTable DataTable { get; set; }
        /// <inheritdoc/>
        public float CalcProp(float param)
        {
            var xAr = DataTable.Rows.Cast<DataRow>().Select(r => Convert.ToSingle(r[0])).ToArray();
            var yAr = DataTable.Rows.Cast<DataRow>().Select(r => Convert.ToSingle(r[1])).ToArray();
            return Interpolation.InterpolatedValue(xAr, yAr, param);
        }
    }
}
