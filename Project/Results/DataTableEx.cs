using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Project.Results
{
    public static class DataTableEx
    {
        public static string[] GetTableSchema(this DataTable dataTable)
        {
            var schema = new string[dataTable.Columns.Count];

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                schema[i] = dataTable.Columns[i].ColumnName;
            }

            return schema;
        }
    }
}
