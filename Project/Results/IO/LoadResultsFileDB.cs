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

        public IEnumerable<IResult> Load(string path)
        {
            LoadEvent?.Invoke(this, new LoadEventArgs("Загрузка результатов *.db"));
            return GetResultsFromDb(path);
        }
        /// <summary>
        /// GetResultsFromDb
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<IResult> GetResultsFromDb(string fullPath)
        {
            var dbName = Path.GetFileNameWithoutExtension(fullPath).Split('_')[0];

            TaskKind taskKind;


            if (dbName == "термическая")
                taskKind = TaskKind.термическая;
            else
                taskKind = TaskKind.механическая;

            var db = new ResultDataBase(fullPath);
            db.OpenConnection();

            var timeDataTable = db.GetResults("nodes", "DISTINCT Time");

            var times = timeDataTable.Rows.Cast<DataRow>().Select(x => Convert.ToSingle(x[0]));

            if (times.Count() == 0)
                throw new Exception("База данных не содержит результатов!");

            var nodesDataTable = db.GetResults("nodes", "*");
            var elemsDataTable = db.GetResults("elements", "*");
            db.CloseConnection();

            foreach (var time in times)
            {
                var newDataSet = new DataSet();

                var newResult = new Result(newDataSet, time, taskKind);

                FillResultTables(newResult, nodesDataTable);
                FillResultTables(newResult, elemsDataTable);

                LoadEvent?.Invoke(this, new LoadEventArgs("Загружены результаты для времени " + time.ToString()));

                yield return newResult;
            }
        }

        private void FillResultTables(Result result, DataTable dbDataTable)
        {
            var time = result.Time;
            var rows = dbDataTable.Select("Time = " + time.ToString(CultureInfo.InvariantCulture));
            var columns = dbDataTable.Columns;

            var newTable = new DataTable(dbDataTable.TableName);

            var columnsCount = dbDataTable.Columns.Count;

            newTable.Columns.Add("Индекс", typeof(int));
            for (int i = 3; i < columnsCount; i++)
            {
                var column = new DataColumn(dbDataTable.Columns[i].ColumnName, typeof(float)) { DefaultValue = 0 };
                newTable.Columns.Add(column);
            }


            for (int i = 0; i < rows.Length; i++)
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

            result.Data.Tables.Add(newTable);
        }
    }
}
