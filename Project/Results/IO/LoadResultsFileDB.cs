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
            return GetResultsFromDb(path);
        }
/// <inheritdoc/>



        public Result GetResultFromDb(string fullPath, string table)
        {
            var dbName = Path.GetFileNameWithoutExtension(fullPath).Split('_')[0];

            TaskKind taskKind;

            if (dbName == "термическая")
                taskKind = TaskKind.термическая;
            else
                taskKind = TaskKind.механическая;

            var db = new ResultDataBase(fullPath);

            db.OpenConnection();
            var dataTable = db.GetResults($"{table}", "*");
            db.CloseConnection();

            var newResult = new Result(dataTable,taskKind);    

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

        //public IEnumerable<Result> GetResultsFromDb_v2(string fullPath)
        //{
        //    var dbName = Path.GetFileNameWithoutExtension(fullPath).Split('_')[0];

        //    TaskKind taskKind;


        //    if (dbName == "термическая")
        //        taskKind = TaskKind.термическая;
        //    else
        //        taskKind = TaskKind.механическая;

        //    var db = new ResultDataBase(fullPath);


        //    //"select name from sqlite_master where type='table'"

        //    db.OpenConnection();

        //    var tablesNames = db.GetResults("sqlite_master WHERE type='table'", "name");

        //    var timeDataTable = db.GetResults("elements", "DISTINCT Time");

        //    var times = timeDataTable.Rows.Cast<DataRow>().Select(x => Convert.ToSingle(x[0]));

        //    if (times.Count() == 0)
        //        throw new Exception("База данных не содержит результатов!");

        //    var nodesDataTable = db.GetResults("nodes", "*");
        //    var elemsDataTable = db.GetResults("elements", "*");
        //    db.CloseConnection();

        //    foreach (var time in times)
        //    {
        //        var newDataSet = new DataSet();

        //        var newResult = new Result(newDataSet, time, taskKind);

        //        newResult.AddTable(nodesDataTable);
        //        newResult.AddTable(elemsDataTable);

        //        LoadEvent?.Invoke(this, new LoadEventArgs("Загружены результаты для времени " + time.ToString()));

        //        yield return newResult;
        //    }
        //}


        /// <summary>
        /// GetResultsFromDb
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<Result> GetResultsFromDb(string fullPath)
        {
            var dbAr = Path.GetFileNameWithoutExtension(fullPath).Split('_');

            TaskKind taskKind;

            if (dbAr[0] == "термическая")
                taskKind = TaskKind.термическая;
            else
                taskKind = TaskKind.механическая;

            var db = new ResultDataBase(fullPath);

            db.OpenConnection();

            var namesTable = db.GetResults("sqlite_master WHERE type='table'", "name");

            var names = namesTable.Rows.Cast<DataRow>().Select(x => Convert.ToString(x[0])).
                Where(x => x != "sqlite_master" & x != "name");

            foreach (var item in names)
            {
                var timeTable = db.GetResults(item, "DISTINCT Time");

                var times = timeTable.Rows.Cast<DataRow>().Select(x => Convert.ToSingle(x[0]));

                if (times.Count() == 0)
                    throw new Exception("База данных не содержит результатов по времени!");

                var dataTable = db.GetResults(item, "*");
                foreach (var time in times)
                {
                    var newResult = new Result(time, taskKind);
                    newResult.AddData(dataTable);
                    LoadEvent?.Invoke(this, new LoadEventArgs("Загружены результаты для времени " + time.ToString()));
                    yield return newResult;
                }

            }
            db.CloseConnection();
        }
    }
}
