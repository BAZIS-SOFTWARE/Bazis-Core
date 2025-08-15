using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace ResultDB
{
    /// <summary>
    /// Result
    /// </summary>
    [Serializable]
    public class Result
    {
        /// <inheritdoc/>
        public string Name { get;}
/// <inheritdoc/>

        public float Time { get; }
/// <inheritdoc/>

        public DataSet Data { get; private set; }

        public Result(DataSet dataSet, float time, string name):
            this(time, name)
        {
            Data = dataSet;
        }

        public Result(float time, string name)
        {
            Data = new DataSet();
            Time = time;
            Name = name;
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{Name} {Time}";
        }

        public bool ContainsTable(string tableName)
        {
            return Data.Tables[tableName] != null ? true : false;
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
