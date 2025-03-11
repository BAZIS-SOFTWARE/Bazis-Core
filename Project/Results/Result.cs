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

        public Result(DataTable table, float time, TaskKind taskKind)
        {
            Data = table;
            Time = time;
            TaskKind = taskKind;
        }

        public Result(float time, TaskKind taskKind)
        {
            Time = time;
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
            Data = dbDataTable;
        }
    }

}
