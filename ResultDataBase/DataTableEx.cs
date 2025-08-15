using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ResultDB
{
    public static class DataTableEx
    {
        /// <summary>
        /// GetTableSchema
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string[] GetTableSchema(this DataTable table)
        {
            var schema = new string[table.Columns.Count];

            for (int i = 0; i < table.Columns.Count; i++)
            {
                schema[i] = table.Columns[i].ColumnName;
            }

            return schema;
        }
        /// <summary>
        /// GetTableData
        /// </summary>
        /// <param name="table"></param>
        /// <param name="objNumber"></param>
        /// <param name="resDescr"></param>
        /// <returns></returns>
        public static float GetTableData(this DataTable table, int objNumber, string resDescr)
        {
            var resu = table.Rows.Find(objNumber);
            if (resu != null && table.Columns.Contains(resDescr))
            {
                return (float)resu[resDescr];
            }
            else return 0.0f;
        }
    }
}
