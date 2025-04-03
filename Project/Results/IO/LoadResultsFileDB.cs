using Project.Interfaces.IO;
using Project.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Project.Results;
using System.Security.Cryptography;

namespace Project.Results.IO
{
    /// <summary>
    /// LoadResultsFileDB
    /// </summary>
    public class LoadResultsFileDB : IResultsLoader
    {
        /// <inheritdoc/>
        public event Action<object, ILoadEventArgs> LoadEvent;

/// <inheritdoc/>
        public Result GetResult(string fullPath, List<string> tables, float time)
        {
            var dbName = Path.GetFileNameWithoutExtension(fullPath).Split('_')[0];

            TaskKind taskKind;

            if (dbName == "термическая")
                taskKind = TaskKind.термическая;
            else
                taskKind = TaskKind.механическая;

            var db = new ResultDataBase(fullPath);
            var newResult = new Result(time, taskKind);
            db.OpenConnection();
            foreach (var table in tables)
            {
                LoadEvent?.Invoke(this, new LoadEventArgs($"Загрузка результатов из таблицы {table}")) ;
                var dataTable = db.GetResults($"{table}", "*", $" WHERE Time = {time}");

                if(dataTable == null)
                    throw new System.Exception($"Время {time} отсутсвует в таблице {table}");

                newResult.AddDataTable(CreateResultTables(dataTable));
            }

            db.CloseConnection();

            return newResult;
        }
/// <inheritdoc/>

        public IEnumerable<Result> GetResults(string fullPath, List<string> tables)
        {
            var dbName = Path.GetFileNameWithoutExtension(fullPath).Split('_')[0];

            TaskKind taskKind;

            if (dbName == "термическая")
                taskKind = TaskKind.термическая;
            else
                taskKind = TaskKind.механическая;

            var times = GetValues(fullPath, tables[0], "Time");

            var db = new ResultDataBase(fullPath);
            db.OpenConnection();

            foreach (var time in times)
            {
                var newResult = new Result(time, taskKind);
                foreach (var table in tables)
                {
                    LoadEvent?.Invoke(this, new LoadEventArgs($"Загрузка результатов из таблицы {table} время {time}"));
                    var dataTable = db.GetResults($"{table}", "*", $" WHERE Time = {time}");

                    if (dataTable == null)
                        throw new System.Exception($"Время {time} отсутсвует в таблице {table}");

                    newResult.AddDataTable(CreateResultTables(dataTable));
                }

                yield return newResult;
            }
            
            db.CloseConnection();
        }
        /// <inheritdoc/>

        public IEnumerable<float> GetValues(string fullPath, string table,string column)
        {
            var db = new ResultDataBase(fullPath);
            db.OpenConnection();

            var timeDataTable = db.GetResults(table, $"DISTINCT {column}","");

            db.CloseConnection();

            var times = timeDataTable.Rows.Cast<DataRow>().Select(x => Convert.ToSingle(x[0]));
            if (times.Count() == 0)
                throw new Exception("База данных не содержит результатов!");

            return times;
        }
/// <inheritdoc/>

        public IEnumerable<string> GetTablesNames(string fullPath)
        {
            var db = new ResultDataBase(fullPath);
            db.OpenConnection();
                var namesTable = db.GetResults("sqlite_master", "name", "WHERE type='table'");
            db.CloseConnection();
            var tables = namesTable.Rows.Cast<DataRow>().Select(x => Convert.ToString(x[0])).
                    Where(x => x != "sqlite_sequence" & x != "name");
                return tables;
  

        }



        private DataTable CreateResultTables(DataTable dbDataTable)
        {
            var rows = dbDataTable.Rows;
            var columns = dbDataTable.Columns;

            var newTable = new DataTable(dbDataTable.TableName);

            var columnsCount = dbDataTable.Columns.Count;

            newTable.Columns.Add(dbDataTable.Columns[2].ColumnName, typeof(int));
            for (int i = 3; i < columnsCount; i++)
            {
                var column = new DataColumn(dbDataTable.Columns[i].ColumnName, typeof(float)) { DefaultValue = 0 };
                newTable.Columns.Add(column);
            }


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

            return newTable;
        }
/// <inheritdoc/>

        public IEnumerable<KeyValuePair<string, List<string>>> GetTablesSchemes(string fullPath)
        {
            var db = new ResultDataBase(fullPath);
            db.OpenConnection();
            var namesTable = db.GetResults("sqlite_master", "name", "WHERE type = 'table'");

            var tables = namesTable.Rows.Cast<DataRow>().Select(x => Convert.ToString(x[0])).
                    Where(x => x != "sqlite_sequence" & x != "name");

            foreach (var table in tables)
            {
                var columns = db.GetTableShema(table).Skip(2).ToList();
                yield return new KeyValuePair<string, List<string>>(table, columns);
            }

            db.CloseConnection();
        }
    }
}
