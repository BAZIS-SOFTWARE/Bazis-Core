using ResultDB.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Transactions;

namespace ResultDB.IO
{
    enum TaskKind { термическая , механическая }
    /// <summary>
    /// LoadResultsFileDB
    /// </summary>
    public class LoadResultsFileDB : IResultsLoader
    {
        /// <inheritdoc/>
        public event Action<object, string> LoadEvent;

/// <inheritdoc/>
        public Result GetResult(string fullPath, List<string> tables, float time)
        {
            var dbName = Path.GetFileNameWithoutExtension(fullPath).Split('_')[0];

            //TaskKind taskKind;

            //if (dbName == "термическая")
            //    taskKind = TaskKind.термическая;
            //else
            //    taskKind = TaskKind.механическая;

            var db = new ResultDataBase(fullPath);
            var newResult = new Result(time, dbName);
            db.OpenConnection();
            foreach (var table in tables)
            {
                LoadEvent?.Invoke(this, $"Загрузка результатов из таблицы {table}") ;
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

            //TaskKind taskKind;

            //if (dbName == "термическая")
            //    taskKind = TaskKind.термическая;
            //else
            //    taskKind = TaskKind.механическая;

            var times = GetValues(fullPath, tables[0], "Time");

            var db = new ResultDataBase(fullPath);
            db.OpenConnection();

            foreach (var time in times)
            {
                var newResult = new Result(time, dbName);
                foreach (var table in tables)
                {
                    LoadEvent?.Invoke(this, $"Загрузка результатов из таблицы {table} время {time}");
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

        /// <summary>
        /// Объединяет две базы данных результатов в одну новую базу.
        /// </summary>
        /// <param name="dbPath1">Файловый путь к первой (первичной) базе данных.</param>
        /// <param name="dbPath2">Файловый путь ко второй (вторичной) базе данных.</param>
        /// <exception cref="InvalidOperationException">
        /// Выбрасывается, если типы БД различаются (например, термическая и механическая),
        /// либо если временные шаги пересекаются некорректным образом.
        /// </exception>
        /// <exception cref="Exception">Выбрасывается при ошибке определения типа БД или при ошибках работы с SQLite.</exception>
        public void Merge(string dbPath1, string dbPath2)
        {
            var type1 = GetDbType(dbPath1);
            var type2 = GetDbType(dbPath2);

            if (type1 != type2)
                throw new InvalidOperationException("Нельзя объединять БД разных типов");

            var tables = GetTablesNames(dbPath1).ToList();
            var times1 = GetValues(dbPath1, tables.First(), "Time").ToList();
            var times2 = GetValues(dbPath2, GetTablesNames(dbPath2).First(), "Time").ToList();

            ValidateTimeIntersection(times1, times2);

            var directory = Path.GetDirectoryName(dbPath1);
            float minTime = Math.Min(times1.Min(), times2.Min());
            float maxTime = Math.Max(times1.Max(), times2.Max());

            var resultDBPath = Path.Combine(directory, $"{type1}_0_{minTime}_{maxTime}.db");

            using (var connection = new SQLiteConnection("Data Source=" + resultDBPath + ";Version=3;Synchronous=OFF;Journal Mode=DELETE;", true))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    var db1Path = dbPath1;
                    var db2Path = dbPath2;

                    if (times1.Last() > times2.Last())
                        (db1Path, db2Path) = (db2Path, db1Path);

                    command.CommandText =
                        $"ATTACH DATABASE '{db1Path}' AS db1; " +
                        $"ATTACH DATABASE '{db2Path}' AS db2;";
                    command.ExecuteNonQuery();

                    try
                    {
                        foreach (var table in tables)
                        {
                            command.CommandText =
                                $"CREATE TABLE {table} AS SELECT * FROM db1.{table} WHERE 0;";
                            command.ExecuteNonQuery();
                            command.CommandText =
                                $"INSERT INTO main.{table} SELECT * FROM db1.{table};";
                            command.ExecuteNonQuery();
                            command.CommandText = 
                                $"INSERT INTO main.{table} SELECT * FROM db2.{table} " +
                                $"WHERE time > (SELECT MAX(time) FROM db1.{table});";
                            command.ExecuteNonQuery();
                        }
                    }
                    finally
                    {
                        command.CommandText =
                            "DETACH DATABASE db1;" +
                            "DETACH DATABASE db2;";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private TaskKind GetDbType(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path).Split('_')[0];

            switch (name)
            {
                case "термическая":
                    return TaskKind.термическая;
                 case "механическая":
                    return TaskKind.механическая;
                 default:
                    throw new Exception("Неизвестный тип БД");
            }
        }

        private void ValidateTimeIntersection(List<float> t1, List<float> t2)
        {
            float min1 = t1.Min();
            float max1 = t1.Max();

            float min2 = t2.Min();
            float max2 = t2.Max();

            bool valid =
                max1 == min2 ||
                max2 == min1 ||
                max1 < min2 ||
                max2 < min1;

            if (!valid)
                throw new InvalidOperationException("Некорректное пересечение временных шагов");
        }
    }
}
