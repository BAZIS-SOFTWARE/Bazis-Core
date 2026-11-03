using MaterialDB.MaterialData.MetallurgicalData;
using MaterialDB.Utilities;
using System;
using System.Data;
using System.Linq;

namespace MaterialDB.MaterialData
{
    public class PropertyCalc
    {
        /// <summary>
        /// Вычисляет свойство по температуре и долям фаз.
        /// </summary>
        public float CalcProp(DataTable phaseProp, PhaseData phaseData, float temp)
        {
            if (phaseProp.Columns.Count - 1 != phaseData.Count)
                throw new ArgumentException($"Phase count must be {phaseProp.Columns.Count - 1}.", nameof(phaseData));

            var xAr = phaseProp.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[0])).ToArray();
            var result = 0.0f;
            for (var column = 1; column < phaseProp.Columns.Count; column++)
            {
                var yAr = phaseProp.Rows.Cast<DataRow>().Select(row => Convert.ToSingle(row[column])).ToArray();
                var value = yAr.Length == 1 ? yAr[0] : InterpolationSearch.InterpolatedValue(xAr, yAr, temp);
                result += value * phaseData[column - 1].Value;
            }

            return result;
        }
    }
}
