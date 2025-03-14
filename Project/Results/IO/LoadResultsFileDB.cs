using Project.Interfaces.IO;
using Project.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Project.Results;

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

        public IEnumerable<Result> Load(string path)
        {
            LoadEvent?.Invoke(this, new LoadEventArgs("Загрузка результатов *.db"));
            return GetResultsFromDb(path, "sqlite_master");
        }
/// <inheritdoc/>



        public Result GetResultFromDb(string fullPath, string table, float time)
        {
            var dbName = Path.GetFileNameWithoutExtension(fullPath).Split('_')[0];

            TaskKind taskKind;

            if (dbName == "термическая")
                taskKind = TaskKind.термическая;
            else
                taskKind = TaskKind.механическая;

            var db = new ResultDataBase(fullPath);

            db.OpenConnection();
            var dataTable = db.GetResults($"{table} WHERE Time = {time}", "*");
            db.CloseConnection();

            var newTable = CreateResultTables(dataTable);

            var newResult = new Result(newTable, time,taskKind);    

            return newResult;
        }
/// <inheritdoc/>

        public IEnumerable<float> GetValuesFromDb(string fullPath, string table,string column)
        {
            var db = new ResultDataBase(fullPath);
            db.OpenConnection();

            var timeDataTable = db.GetResults(table, $"DISTINCT {column}");

            db.CloseConnection();

            var times = timeDataTable.Rows.Cast<DataRow>().Select(x => Convert.ToSingle(x[0]));
            if (times.Count() == 0)
                throw new Exception("База данных не содержит результатов!");

            return times;
        }

        /// <summary>
        /// GetResultsFromDb
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<Result> GetResultsFromDb(string fullPath, string table)
        {
            var dbAr = Path.GetFileNameWithoutExtension(fullPath).Split('_');

            TaskKind taskKind;

            if (dbAr[0] == "термическая")
                taskKind = TaskKind.термическая;
            else
                taskKind = TaskKind.механическая;

            var db = new ResultDataBase(fullPath);

            db.OpenConnection();

            IEnumerable<string> tables;
            if (table == "sqlite_master")
            {
                var namesTable = db.GetResults("sqlite_master WHERE type='table'", "name");
                tables = namesTable.Rows.Cast<DataRow>().Select(x => Convert.ToString(x[0])).
                    Where(x => x != "sqlite_sequence" & x != "name");
            }
            else
                tables = new List<string> { table };

            foreach (var item in tables)
            {
                var timeTable = db.GetResults(item, "DISTINCT Time");

                var times = timeTable.Rows.Cast<DataRow>().Select(x => Convert.ToSingle(x[0]));

                foreach (var time in times)
                {
                    var dataTable = db.GetResults($"{item} WHERE Time = {time}", "*");
                    var newTable = CreateResultTables(dataTable);
                    var newResult = new Result(newTable,time, taskKind);

                    LoadEvent?.Invoke(this, new LoadEventArgs("Загружены результаты для времени " + time.ToString()));
                    yield return newResult;
                }

            }
            db.CloseConnection();
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
    }
}
