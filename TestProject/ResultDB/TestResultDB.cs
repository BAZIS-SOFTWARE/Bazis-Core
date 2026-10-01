using Geometry;
using MathNet.Numerics.Distributions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces.MeshObjects;
using Model.IO;
using OperationalController.MeshObjsUtility;
using OperationalController.ModelScenePresentator;
using PostProc;
using PostProc.IO.GridExport;
using PostProc.IO.ResultExport;
using PostProc.ScenePresenter;
using Project;
using Project.Interfaces.Tasks;
using Project.IO;
using ResultDB;
using ResultDB.IO;
using Results.ScenePresenter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using static Community.CsharpSqlite.Sqlite3;


namespace TestProject_test.ResultDB
{
    [TestClass]
    public class TestResultDB
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\ProjAndResults", "термическая_0_0_0.5.db", 0.5f)]
        [DataRow(@"..\..\..\Examples\ProjAndResults", "термическая_0_0_1500.db", 200.8569f)]
        public void TestLoadDBSQLiteFindByTime(string path, string resName, float time)
        {
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1,ar2) => { };
            var project = new ProjectData($"{path}\\newProj.bpf2");

            var results = loaderDB.GetResult(path + $@"\{resName}",
                new List<string>() { "nodes" }, time) ; 
            
            Assert.AreNotEqual(results, null);
            Assert.AreNotEqual(results.Data.Tables["nodes"].Rows.Count, 0);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\ProjAndResults", "термическая_0_0_0.5.db")]
        [DataRow(@"..\..\..\Examples\ProjAndResults", "термическая_0_0_1500.db")]
        public void TestLoadDBSQLiteTablesScheme(string path, string resName)
        {
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            //var project = new ProjectData($"{path}\\newProj.bpf2");

            var shemes = loaderDB.GetTablesSchemes(path + $@"\{resName}").ToList();

            Assert.AreNotEqual(shemes, null);
            //Assert.AreNotEqual(results.Data.Tables["nodes"].Rows.Count, 0);
        }

        [TestMethod]
        [DataRow(@"..\..\..\Examples\ProjAndResults", "термическая_0_0_0.5.db")]
        public void TestLoadDBSQLiteFindByTimeInterval(string path, string dbName)
        {
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };

            //var project = new ProjectData($"{path}\\newProj.bpf2");

            var times = loaderDB.GetValues(path + $@"\{dbName}", "nodes", "Time");

            var results = new List<Result>();

            foreach (var item in times)
            {
                results.Add(loaderDB.GetResult(path + $@"\{dbName}",
                    new List<string> { "nodes", "elements" }, item));
            }

            var kind = dbName.Split('_')[0];

            var search = results.Where(x => x.Name.ToString() == kind &&

x.Time.CompareTo(0) >= 0
& x.Time.CompareTo(0.25f) <= 0);

            Assert.AreNotEqual(search, null);
        }

        [DataTestMethod]
        [DataRow("csv_table_1.csv")]
        public void CSVToDataTableConverter_Example(string file)
        {
            var csvPath = @"..\..\..\ResultTableCSV";

            var convert = new CSVToDataTableConverter();

            var dataTable = convert.ConvertCSVtoDataTable($@"{csvPath}\{file}");
            dataTable.TableName = "nodes";

            var dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);
            var result = new Result(dataSet, 0.87051f, "термическая");

            var dbPath = $@"{csvPath}\results.db";

            var saverDB = new SaveResultsFileDb();
            saverDB.Save(new List<Result>() { result }, dbPath, false);
   
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var times = loaderDB.GetValues(dbPath, "nodes", "Time");

            var resultTOLoad = loaderDB.GetResult(dbPath, new List<string>() { "nodes"}, times.First());

            Assert.AreEqual(resultTOLoad.Name, "results");
        }   

        [DataTestMethod]
        [DataRow(new string[]
       {
            "Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL",
            "[Time] real DEFAULT(0) NOT NULL",
            "[Index] INTEGER DEFAULT(0) NOT NULL",
            "[Value] real DEFAULT(0) NOT NULL"
       },
            
       new object[]
       {
            0.01,
            2,
            1.666666
       },
            DisplayName = "3 with [] and 1 without")]
        [DataRow(new string[]
       {
            "[Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL",
            "[Time] real DEFAULT(0) NOT NULL",
            "[Index] INTEGER DEFAULT(0) NOT NULL",
            "[Value] real DEFAULT(0) NOT NULL"
       },
            new object[]
       {
            0.01,
            2,
            1.666666
       },

            DisplayName = "4 with []")]
        [DataRow(new string[]
       {
            "Number INTEGER PRIMARY KEY NOT NULL",
            "Time real DEFAULT(0) NOT NULL",
            "Value real DEFAULT(0) NOT NULL"
       },
                 new object[]
       {
            2,
            0.01,
            1.666666
       },

            DisplayName = "3 without  []")]
        public void BasicDbOperations(string[] columns, object[] values)
        {
            var path = @"..\..\..\TestTableColumns\test.db";

            var db = new ResultDataBase(path);
            db.OpenConnection();

            db.DeleteTable("testTable");
            db.CreateTables("testTable", columns);

            var valStr = string.Join(",", values);

            var columns_ch = columns.Where(x => !x.Contains("AUTOINCREMENT")).
                Select(x => x.Split(' ')[0]);

            var clmStr = string.Join(",", columns_ch);

            var cmd = db.CreateAddCommand("testTable", clmStr, valStr);
            db.AddData(cmd);
            var table = db.GetResults("testTable", "*","");

            var schema = db.GetTableShema("testTable").ToList();

            db.CloseConnection();
            Assert.AreEqual(table.Columns.Count, schema.Count());
            Assert.AreEqual(table.Columns.Count, columns.Length);
        }

        [TestMethod]
        [DataRow(@"..\..\..\Examples\ProjAndResults\merge\термическая_2_1_2.db",
                 @"..\..\..\Examples\ProjAndResults\merge\термическая_3_2_3.db",
                 @"..\..\..\Examples\ProjAndResults\merge\термическая_0_1_3.db")]
        [DataRow(@"..\..\..\Examples\ProjAndResults\merge\термическая_3_2_3.db",
                 @"..\..\..\Examples\ProjAndResults\merge\термическая_2_1_2.db",
                 @"..\..\..\Examples\ProjAndResults\merge\термическая_0_1_3.db")]
        [DataRow(@"..\..\..\Examples\ProjAndResults\merge\термическая_2_1_2.db",
                 @"..\..\..\Examples\ProjAndResults\merge\термическая_2_1_2.db",
                 @"..\..\..\Examples\ProjAndResults\merge\термическая_0_1_2.db")]
        public void MergeDataBase(string path1, string path2, string resultPath)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var db1Path = path1;
                var db2Path = path2;

                var loader = new LoadResultsFileDB();
                loader.Merge(db1Path, db2Path);

                Assert.IsTrue(File.Exists(resultPath), "Merged database was not created.");

                var tableName = loader.GetTablesNames(path1).First();
                var expectedRows = GetExpectedRows(path1, path2, tableName);
                var actualRows = GetTotalRows(resultPath, tableName);

                Assert.AreEqual(expectedRows, actualRows, "Merged row count is incorrect.");
                
                // Локальная вспомогательная функция: возвращает общее количество строк в таблице указанной БД.
                int GetTotalRows(string dbPath, string tableName)
                {
                    var db = new ResultDataBase(dbPath);
                    db.OpenConnection();
                    try{ return db.GetResults(tableName, "*", "").Rows.Count;}
                    finally{ db.CloseConnection();}
                }

                // Локальная вспомогательная функция: вычисляет ожидаемое количество строк после слияния двух баз.
                // Логика:
                // - Получаем набор времен (Time) из обеих БД.
                // - Определяем, какая БД содержит меньший максимальный Time — это будет "db1" (ранняя).
                // - Количество ожидаемых строк = все строки из db1 + строки из db2, у которых time > maxTimeDb1.
                int GetExpectedRows(string path1, string path2, string tableName)
                {
                    var t1 = loader.GetValues(path1, loader.GetTablesNames(path1).First(), "Time").ToList();
                    var t2 = loader.GetValues(path2, loader.GetTablesNames(path2).First(), "Time").ToList();
                    var db1Path = path1;
                    var db2Path = path2;

                    var maxTimeDb1 = t1.Last();
                    // Обмениваем пути, чтобы db1 всегда была с меньшим последним временем
                    if (t1.Last() > t2.Last()) 
                    {
                        (db1Path, db2Path) = (db2Path, db1Path);
                        maxTimeDb1 = t2.Last();
                    }
                        

                    var db1 = new ResultDataBase(db1Path);
                    var db2 = new ResultDataBase(db2Path);
                    db1.OpenConnection();
                    db2.OpenConnection();

                    try
                    {
                        var table1 = db1.GetResults(tableName, "*", "");
                        var table2 = db2.GetResults(tableName, "*", "");

                        if (table1 == null || table2 == null)
                            return 0;
                        var rowsDb1 = table1.Rows.Count;

                        // Считаем строки во второй таблице с time > maxTimeDb1
                        var rowsDb2Filtered = table2.AsEnumerable()
                            .Count(r => r["time"] != DBNull.Value &&
                                        Convert.ToInt64(r["time"]) > maxTimeDb1);

                        return rowsDb1 + rowsDb2Filtered;
                    }
                    finally
                    {
                        db1.CloseConnection();
                        db2.CloseConnection();
                    }
                }
            }
            catch (Exception ex) {Assert.Fail($"An exception occurred during merging: {ex.Message}");}
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                File.Delete(resultPath);
            }
        }
    }
}
