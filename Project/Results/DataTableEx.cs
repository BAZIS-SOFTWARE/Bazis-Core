using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Project.Results
{
    public static class DataTableEx
    {
        public static List<string> GetTableSchema(this DataTable dataTable)
        {
            var schema = new List<string>();

            for (int i = 1; i < dataTable.Columns.Count; i++)
            {
                schema.Add(dataTable.Columns[i].ColumnName);
            }

            return schema;
        }
    }
}
