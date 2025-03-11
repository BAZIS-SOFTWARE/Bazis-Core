using Project.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Project.Results
{
    /// <summary>
    /// Result
    /// </summary>
    public class Result : IResult
    {
        /// <inheritdoc/>
        public TaskKind TaskKind { get;}
/// <inheritdoc/>

        public float Time { get; }
/// <inheritdoc/>

        public DataTable Data { get; private set; }
/// <inheritdoc/>

        public string Name { get { return Data.TableName; } }

        /// <inheritdoc/>

        public List<string> GetDataSchema()
        {
            var table = Data.Clone();

            var schema = new List<string>(); 
            foreach (DataColumn column in table.Columns)
                schema.Add(column.ColumnName);
            return schema;
        }

        public Result(DataTable data, TaskKind taskKind)
        {
            AddData(data);
            TaskKind = taskKind;
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return TaskKind + " " + Time.ToString();
        }
/// <inheritdoc/>

        public float GetValue(int objNumber, string resDescr)
        {
            var resu = Data.Rows.Find(objNumber);
            if (resu != null && Data.Columns.Contains(resDescr))
            {
                return (float)resu[resDescr];
            }
            else return 0.0f;
        }

/// <inheritdoc/>

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public void AddData(DataTable dbDataTable)
        {

            // + Time.ToString(CultureInfo.InvariantCulture));

            if (!dbDataTable.Columns.Contains("Time"))
                throw new Exception($"Таблица не содержит данных о времени {Time}");

            if (dbDataTable.Rows.Count == 0)
                throw new Exception($"Таблица не содержит строк");

            var time = Convert.ToSingle(dbDataTable.Rows[0]["Time"]);
            
            if(dbDataTable.Select($"Time = {time}").Length != dbDataTable.Rows.Count)
                throw new Exception($"Значения времени в строках не совпадают");

            var newTable = new DataTable(dbDataTable.TableName);

            var columnsCount = dbDataTable.Columns.Count;

            newTable.Columns.Add("Индекс", typeof(int));
            for (int i = 3; i < columnsCount; i++)
            {
                var column = new DataColumn(dbDataTable.Columns[i].ColumnName, typeof(float)) { DefaultValue = 0 };
                newTable.Columns.Add(column);
            }

            var rows = dbDataTable.Rows;
            var columns = dbDataTable.Columns;

            for (int i = 0; i < rows.Count; i++)
            {
                var row = newTable.NewRow();

                for (int j = 2; j < columns.Count; j++)
                {
                    row[j - 2] = rows[i][j];
                }
                newTable.Rows.Add(row);
            }

            var key = new DataColumn[1];
            key[0] = newTable.Columns[0];
            newTable.PrimaryKey = key;

            Data = newTable;
        }
    }

}
