using Project.Interfaces;
using Project.Interfaces.Tasks;
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
    [Serializable]
    public class Result
    {
        /// <inheritdoc/>
        public TaskKind TaskKind { get;}
/// <inheritdoc/>

        public float Time { get; }
/// <inheritdoc/>

        public DataSet Data { get; private set; }

        public Result(DataSet dataSet, float time, TaskKind taskKind):
            this(time,taskKind)
        {
            Data = dataSet;
        }

        public Result(float time, TaskKind taskKind)
        {
            Data = new DataSet();
            Time = time;
            TaskKind = taskKind;
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{TaskKind} {Time}";
        }
/// <inheritdoc/>

        public float GetValue(string tableName,int objNumber, string resDescr)
        {
            var table = Data.Tables[tableName];
            return table.GetTableData(objNumber, resDescr);
        }

        /// <inheritdoc/>

        public float GetValue(int tableInd, int objNumber, string resDescr)
        {
            var table = Data.Tables[tableInd];
            return table.GetTableData(objNumber, resDescr);
        }
/// <inheritdoc/>

        public void AddDataTable(DataTable dbDataTable)
        {
            Data.Tables.Add(dbDataTable);
        }
    }

}
