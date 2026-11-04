using MaterialDB.Interfaces;
using MaterialDB.MaterialData.MetallurgicalData;
using MaterialDB.Utilities;
using System;
using System.Data;
using System.Linq;

namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Табличное свойство материала.
    /// </summary>
    [Serializable]
    public class PropertyItem : DataItem, IProperty
    {
        /// <summary>
        /// Создаёт свойство материала с табличными данными.
        /// </summary>
        public PropertyItem(string name, string x_unit, string y_unit, string units, DataTable dataTable)
            : base(name, x_unit, y_unit, units, dataTable)
        {
        }

        /// <summary>
        /// Создаёт независимую копию свойства материала.
        /// </summary>
        private PropertyItem(string copyName, PropertyItem source) : base(copyName, source)
        {
        }

        /// <summary>
        /// Создаёт копию свойства с независимой таблицей.
        /// </summary>
        public override PropertyItem Copy(string copyName)
        {
            return new PropertyItem(copyName, this);
        }

        /// <summary>
        /// Вычисляет свойство по температуре и долям фаз.
        /// </summary>
        public float CalcProp(PhaseData phaseData, float temp)
        {
            if (DataTable.Columns.Count - 1 != phaseData.Count)
                throw new ArgumentException($"Phase count must be {DataTable.Columns.Count - 1}.", nameof(phaseData));

            var xAr = DataTable.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[0])).ToArray();
            var result = 0.0f;
            for (var column = 1; column < DataTable.Columns.Count; column++)
            {
                var yAr = DataTable.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[column])).ToArray();
                var value = yAr.Length == 1 ? yAr[0] : InterpolationSearch.InterpolatedValue(xAr, yAr, temp);
                result += value * phaseData[column - 1].Value;
            }

            return result;
        }
    }
}
