using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project;
using System.Globalization;
using System.Threading;
using Project.Interfaces.Tasks;
using Model.Interfaces.MeshObjects;
using System.Data;
using System.IO;
using Project.Results.IO;
using PostProc.ScenePresenter;
using PostProc;
using Project.Results;
using Results.ScenePresenter;
using PostProc.IO.GridExport;
using PostProc.IO.ResultExport;
using Model.IO;
using OperationalController.MeshObjsUtility;
using OperationalController.ModelScenePresentator;


namespace TestProject
{
    [TestClass]
    public class TestResultData
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
            var project = new ProjectData("", path);

            var results = loaderDB.GetResult(project.GeneralData.Path + $@"\{resName}",
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
            var project = new ProjectData("", path);

            var shemes = loaderDB.GetTablesSchemes(project.GeneralData.Path + $@"\{resName}").ToList();

            Assert.AreNotEqual(shemes, null);
            //Assert.AreNotEqual(results.Data.Tables["nodes"].Rows.Count, 0);
        }

        [TestMethod]
        [DataRow(@"..\..\..\Examples\ProjAndResults", "термическая_0_0_0.5.db")]
        public void TestLoadDBSQLiteFindByTimeInterval(string path, string dbName)
        {
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };

            var project = new ProjectData("", path);

            var times = loaderDB.GetValues(project.GeneralData.Path + $@"\{dbName}", "nodes", "Time");

            var results = new List<Result>();

            foreach (var item in times)
            {
                results.Add(loaderDB.GetResult(project.GeneralData.Path + $@"\{dbName}",
                    new List<string> { "nodes", "elements" }, item));
            }

            var kind = dbName.Split('_')[0];

            var search = results.Where(x => x.TaskKind.ToString() == kind &&

x.Time.CompareTo(0) >= 0
& x.Time.CompareTo(0.25f) <= 0);

            Assert.AreNotEqual(search, null);
        }

        [DataTestMethod]
        [DataRow(@"c:\projs\ElenaLu\Bulk\1.bpf", "термическая_0.db", 12.71871f)]
        public void TestCreateSurfResFromFile(string path, string resName, float time)
        {
var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };

            var dir = Path.GetDirectoryName(path);
            var file = Path.GetFileName(path);

            var project = new ProjectData(file, dir);
            project.Load();

            var times = loaderDB.GetValues(project.GeneralData.Path + $@"\{resName}", "nodes", "Time");

            var results = loaderDB.GetResults(project.GeneralData.Path + $@"\{resName}",
                    new List<string> { "nodes", "elements" });

            var els3D = project.ModelData.ObjectData.E3DCollection;
            var controller = new PostProcController();

            var intervalVals = new ItemRange[]
{
                new ItemRange() { Min = 20, Max = 316,Color = Color.FromArgb(0, 0, 255) },
                new ItemRange() { Min = 316, Max = 612,Color = Color.FromArgb(0, 128, 255) },
                new ItemRange() { Min = 612, Max = 908,Color = Color.FromArgb(0, 128, 255) },
                new ItemRange() { Min = 908, Max = 1204,Color = Color.FromArgb(0, 128, 255) },
                new ItemRange() { Min = 1204, Max = 1500,Color = Color.FromArgb(0, 128, 255) }
};
            //act find subsurf for one interval

            controller.ResultsFieldsCreator.SetScaleItems(intervalVals);
            controller.ResultsFieldsCreator.ScaleFactor = 1;

            var search = results.FirstOrDefault(x => x.Time.Equals(time));
       
            var elsResults = controller.ResultsFieldsCreator.CreateSurfaceObjects(search, "nodes", "T", els3D.GetObjects());

            Assert.AreNotEqual(elsResults, null);
            Assert.AreNotEqual(elsResults.Count(), 0);
        }



        [TestMethod]
        [DataRow(@"..\..\..\Examples\ProjAndResults\термическая_0_0_0.5.db")]
        public void TestMergeResults(string dbName)
        {
            var proj = new ProjectData();

            proj.Load(@"..\..\..\Examples\ProjAndResults\two_tetra_term_cycle_multiStep.bpf");

            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            
            
            var res = loaderDB.GetResults(dbName, new List<string>() { "nodes", "elements" }).ToArray();

            IEnumerable<IElement> elements;
            if (proj.GeneralData.TaskType == TaskType.Volume)
            {
                elements = proj.ModelData.ObjectData.E3DCollection.GetObjects();
            }
            else elements = proj.ModelData.ObjectData.E2DCollection.GetObjects();

            var interfaceNodesFinder = new InterfacedNodesFinder();

            var mergeResults = new MergeResults();
            var intNodes = interfaceNodesFinder.Find(elements);
            for (int i = 0; i < res.Length; i++)
                mergeResults.Merge(intNodes, "Q", res[i]);

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
            var result = new Result(dataSet, 0.87051f,TaskKind.термическая);

            var dbPath = $@"{csvPath}\results.db";

            var saverDB = new SaveResultsFileDb();
            saverDB.Save(new List<Result>() { result }, dbPath, false);
   
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var times = loaderDB.GetValues(dbPath, "nodes", "Time");

            var resultTOLoad = loaderDB.GetResult(dbPath, new List<string>() { "nodes"}, times.First());

            Assert.AreEqual(resultTOLoad.TaskKind, TaskKind.механическая);
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

        [DataTestMethod]
        [DataRow(
            "two_tetra_term_cycle_multiStep.bpf","термическая_0_0_0.5.db","термическая", 0f,0.0002f,"T",
            @"..\..\..\Examples\ProjAndResults")]
        [DataRow(
            "one_quad_term_cycle.bpf2", "механическая_1_0_3.db", "механическая", 0f, 0.0037f,"XY",
            @"..\..\..\Examples\ProjAndResults")]
        public void Test_Scene_Result_Presenter(string projName,string resName,string resKind,float min,float max,string resDesc,string resPath )
        {
            var proj = new ProjectData();

            var ext = Path.GetExtension(projName);
            if (ext == ".bpf2")
                proj.ModelData.Loader = new LoadModelFromBPF2TextFile();

            proj.Loader.LoadEvent += (ar1, ar2) => { };

            proj.Load($@"{resPath}\{projName}");

            var dbExtension = System.IO.Path.GetExtension($@"{resPath}\{resName}");
            //var dbExtension = System.IO.Path.GetExtension(@"C:\Bazis_newGUI\PROJ\Test_3D\Term\Termal_0_0,5.db");

            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var results = loaderDB.GetResults($@"{resPath}\{resName}",new List<string>() { "nodes","elements"});

            IEnumerable<ISurfaceElement> elements;
            if(proj.GeneralData.TaskType == TaskType.Volume)
                elements = proj.ModelData.ObjectData.E3DCollection.GetObjects();
            else
                elements = proj.ModelData.ObjectData.E2DCollection.GetObjects();
            
            var itemRanges = new ItemRange[]
{
                new ItemRange() { Min = min, Max = max, Color = Color.FromArgb(0, 0, 255) }
};

            var resultsCreator = new ResultsFieldsCreator();
            resultsCreator.ScaleFactor = 1;
            resultsCreator.SetScaleItems(itemRanges);

            var search = results.FirstOrDefault(x => x.Time.Equals(0));

            var resultObjects = resultsCreator.CreateSurfaceObjects(search, "nodes", resDesc, elements);
            var points = new List<ResultPoint>();
            foreach (var elems in resultObjects)
                points.AddRange(elems.GetVertexes());

            var gridPath = $@"{resPath}\testGridSafe.bpf";
            var saver = new SaveGridSurfacesToBPF();
            saver.SaveGrid(resultObjects, gridPath);

            var TXTSaver = new SaveObjectsResultsToTXT();
            var resultNodesTXTPath = $@"{resPath}\testResultNodes.txt";
            var resultElementsTXTPath = $@"{resPath}\testResultElemets.txt";

            TXTSaver.SaveResults(proj.ModelData.ObjectData.NodesSet.Values, results.First(), resDesc, resultNodesTXTPath);
            TXTSaver.SaveResults(proj.ModelData.ObjectData.GetAllElements(), results.First(), resDesc, resultElementsTXTPath);

            var CSVSaver = new SaveObjectsResultsToCSV();
            var resultNodesCSVPath = $@"{resPath}\testResultNodes.csv";
            var resultElementsCSVPath = $@"{resPath}\testResultElements.csv";

            CSVSaver.SaveResults(proj.ModelData.ObjectData.NodesSet.Values, results.First(), resDesc, resultNodesCSVPath);
            CSVSaver.SaveResults(proj.ModelData.ObjectData.GetAllElements(), results.First(), resDesc, resultElementsCSVPath);

            var modelPresenter = new PresentersCreator();
            var presenter = modelPresenter.CreateSurfaceObjectsPresenter(resultObjects);

            if (resultObjects.Count() == 0)
                throw new Exception();
            if (presenter.Count() == 0)
                throw new Exception();

            var inds = presenter.CreateIndexes();
            var ptrs = presenter.CreatePointers(inds.Item1);
            var crds = presenter.CreateVertexes(inds.Item2, "координаты");
            var colors = presenter.CreateVertexes(inds.Item3, "цвет");
            var normals = presenter.CreateVertexes(inds.Item2, "нормаль");
            var edges = presenter.CreateEdgeFlags(inds.Item4);
        }

        [TestMethod]
        [DataRow(7.0001f, 7.00015f, true)]
        [DataRow(7.0001f, 17.00015f, true)]
        [DataRow(-7.0001f, 7.00015f, true)]
        [DataRow(-7.0001f, 0.00015f, false)]
        [DataRow(11.0001f, 12.00015f, false)]
        public void TryIntervalsIntersecCloseResults(float v1,float v2,bool res)
        {
            //arrange

            var ResultItems = new ResultItem[]
            {
                new ResultItem(new Point3D(1, 0, 0),v1),
                new ResultItem(new Point3D(0, 1, 0),v2)
            };

            var surfGrad = new SurfaceGradientsCreator();

            var interval = new ItemRange() { Min = 1, Max = 10 };


            //act find subsurf for one interval
            List<Point3D> egCoords;
            var resTry = surfGrad.TryIntervalsIntersec(interval, ResultItems[0], ResultItems[1], out egCoords);

            //assert  
            Assert.AreEqual(resTry, res);
        }


        [TestMethod]
        public void OneTriangleSurface_OutOfIntervalsUp()
        {
            //arrange

            var resItems = new List<ResultItem>()
            {
                new ResultItem(new Point3D(1, 0, 0),7),
                new ResultItem(new Point3D(0, 1, 0),8),
                new ResultItem(new Point3D(0, 0, 0),6)
            };

            var surfGrad = new SurfaceGradientsCreator();

            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 1, Max = 2,Color = Color.FromArgb(0, 0, 255) },
                new ItemRange() { Min = 2, Max = 3,Color = Color.FromArgb(0, 128, 255) },
                new ItemRange() { Min = 3, Max = 4,Color = Color.FromArgb(0, 255, 255) },
                new ItemRange() { Min = 4, Max = 5,Color = Color.FromArgb(0, 255, 80) }
        };

            //act find subsurf for one interval

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, resItems).ToList();

            //assert  
            Assert.AreEqual(resSurfaces.Count, 1);
            Assert.AreEqual(resSurfaces[0].Color, intervalVals[3].Color);
        }

        [TestMethod]
        public void OneTriangleSurface_OutOfIntervalsDown()
        {
            //arrange
            var resItems = new List<ResultItem>()
            {
                new ResultItem(new Point3D(1, 0, 0),-1),
                new ResultItem(new Point3D(0, 1, 0),0),
                new ResultItem(new Point3D(0, 0, 0),0.5f)
            };


            var surfGrad = new SurfaceGradientsCreator();

            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 1, Max = 2,Color = Color.FromArgb(0, 0, 255) },
                new ItemRange() { Min = 2, Max = 3,Color = Color.FromArgb(0, 128, 255)},
                new ItemRange() { Min = 3, Max = 4,Color = Color.FromArgb(0, 255, 255) },
                new ItemRange() { Min = 4, Max = 5,Color = Color.FromArgb(0, 255, 80) }
        };
            //act find subsurf for one interval

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, resItems).ToList();

            //assert  
            Assert.AreEqual(resSurfaces.Count, 1);
            Assert.AreEqual(resSurfaces[0].Color, intervalVals[0].Color);
        }

        [TestMethod]
        [DataRow(19.99998f, 19.99998f, 20.00001f, 20.00001f)]
        [DataRow(20.00001f, 19.99998f, 20.00001f, 19.99998f)]
        public void OneTetraTwoIntervalsShouldCreateResults(float v1,float v2,float v3,float v4)
        {
            //assign
            var resItems = new List<ResultItem>()
            {
                new ResultItem(new Point3D(1, 0, 0),v1),
                new ResultItem(new Point3D(0, 1, 0),v2),
                new ResultItem(new Point3D(0, 0, 0),v3),
                new ResultItem(new Point3D(0, 0, 1),v4)
            };


            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 20, Max = 30, Color = Color.FromArgb(0, 0, 255) },
                new ItemRange() { Min = 30, Max = 40,Color = Color.FromArgb(0, 128, 255) }
        };
            //act find subsurf for one interval
            var surfGrad = new SurfaceGradientsCreator();

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, resItems).ToList();

            //assert           
            Assert.AreNotEqual(resSurfaces.Count, 0);
        }

        [TestMethod]
        public void OneQuadSurface_TwoIntervals()
        {
            //arrange
            var resItems = new List<ResultItem>()
            {
                new ResultItem(new Point3D(1, 0, 0),1),
                new ResultItem(new Point3D(1, 1, 0),5),
                new ResultItem(new Point3D(0, 1, 0),4),
                new ResultItem(new Point3D(0, 0, 0),1)
            };


            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 2, Max = 3,Color = Color.FromArgb(0, 0, 255) },
                new ItemRange() { Min = 3, Max = 4,Color = Color.FromArgb(0, 128, 255) }
        };
            //act find subsurf for one interval
            var surfGrad = new SurfaceGradientsCreator();

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, resItems).ToList();

            //assert
            Assert.AreEqual(resSurfaces.Count, 2);
            Assert.AreEqual(resSurfaces[0].NumberOfPoints, 4);
            Assert.AreEqual(resSurfaces[1].NumberOfPoints, 4);
        }



        [TestMethod]
        public void OneTriangleSurface_TwoIntervals()
        {
            //arrange
            var resItems = new List<ResultItem>()
            {
                new ResultItem(new Point3D(1, 0, 0),1),
                new ResultItem(new Point3D(0, 1, 0),4),
                new ResultItem(new Point3D(0, 0, 0),5)
            };


            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 2, Max = 3,Color = Color.FromArgb(0, 0, 255) },
                new ItemRange() { Min = 3, Max = 4,Color = Color.FromArgb(0, 128, 255) }
        };
            //act find subsurf for one interval
            var surfGrad = new SurfaceGradientsCreator();

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, resItems).ToList();

            //assert           
            var comSurfaces = new List<ResultFigure>()
            {
                new ResultFigure(
                new List<Point3D>
                {
                    new Point3D(1,0,0),
                    new Point3D(0.3333f,0.6666f,0),
                    new Point3D(0.5f, 0, 0)                   
                },Color.Red),
                new ResultFigure(
                new List<Point3D>
                {
                    new Point3D(0.3333f,0.6666f,0),
                    new Point3D(0, 1, 0),
                    new Point3D(0, 0, 0),
                    new Point3D(0.5f,0,0),
                },Color.Red)
            };

            if (resSurfaces.Count() != comSurfaces.Count())
                throw new Exception();


            for (int i = 0; i < resSurfaces.Count(); i++)
            {
                var pointsCount = resSurfaces[i].NumberOfPoints;
                var resPoints = resSurfaces[i].GetCoordinates().ToList();
                for (int j = 0; j < pointsCount; j++)
                {
                    if (resPoints[j] != comSurfaces[i][j].Position)
                        throw new Exception("Узлы не совпадают!");
                }
            }
        }

        [TestMethod]
        public void OneTriangleSurface_SeveralIntervals()
        {
            //arrange

            var resItems = new List<ResultItem>()
            {
                new ResultItem(new Point3D(1, 0, 0),1),
                new ResultItem(new Point3D(0, 1, 0),4),
                new ResultItem(new Point3D(0, 0, 0),5)
            };

            var surfGrad = new SurfaceGradientsCreator();

            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 1, Max = 2, Color = Color.FromArgb(0, 0, 255) },
                new ItemRange() { Min = 2, Max = 3, Color = Color.FromArgb(0, 128, 255) },
                new ItemRange() { Min = 3, Max = 4,Color = Color.FromArgb(0, 255, 255) },
                new ItemRange() { Min = 4, Max = 5,Color = Color.FromArgb(0, 255, 80) }
        };

            //act find subsurf for one interval

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, resItems).ToList();

            //assert  
            var comSurfaces = new List<ResultFigure>()
            {
                new ResultFigure(
                new List<Point3D>()
                {
                    new Point3D(1,0,0),
                    new Point3D(0.6666f,0.3333f,0),
                    new Point3D(0.75f, 0, 0)
                },
                Color.Red),
                new ResultFigure(
                new List<Point3D>()
                {
                    new Point3D(0.6666f,0.3333f,0),
                    new Point3D(0.3333f,0.6666f,0),
                    new Point3D(0.5f, 0, 0),
                    new Point3D(0.75f, 0, 0),
                },
                Color.Red),
                new ResultFigure(
                new List<Point3D>
                {
                    new Point3D(0.3333f,0.6666f,0),
                    new Point3D(0,1,0),
                    new Point3D(0.25f, 0, 0),
                    new Point3D(0.5f, 0, 0)
                },
                Color.Red),
                new ResultFigure(
                new List<Point3D>
                {
                    new Point3D(0,1,0),
                    new Point3D(0, 0, 0),
                    new Point3D(0.25f, 0, 0)
                },
                Color.Red)
            };
                
            if (resSurfaces.Count() != comSurfaces.Count())
                throw new Exception();

            for (int i = 0; i < resSurfaces.Count(); i++)
            {
                var pointsCount = resSurfaces[i].NumberOfPoints;
                var resPoints = resSurfaces[i].GetCoordinates().ToList();
                for (int j = 0; j < pointsCount; j++)
                {
                    if (resPoints[j] != comSurfaces[i][j].Position)
                        throw new Exception("Узлы не совпадают!");
                }
            }
        }

        [TestMethod]
        public void OneSurfaceFiveIntervals()
        {
            //arrange
            var resItems = new List<ResultItem>()
            {
                new ResultItem(new Point3D(59003.5f, 12318.13f, 52106.64f),1),
                new ResultItem(new Point3D(59005.25f, 12318.13f, 52106.64f),4),
                new ResultItem(new Point3D(59003.5f, 12315.9f, 52104.03f),5)
            };

            var surfGrad = new SurfaceGradientsCreator();

            var intervalVals = new ItemRange[]
 {
                new ItemRange() { Min = 20, Max = 316,Color = Color.FromArgb(0, 0, 255) },
                new ItemRange() { Min = 316, Max = 612,Color = Color.FromArgb(0, 128, 255) },
                new ItemRange() { Min = 612, Max = 908,Color = Color.FromArgb(0, 255, 255) },
                new ItemRange() { Min = 908, Max = 1204,Color = Color.FromArgb(0, 255, 80) },
                new ItemRange() { Min = 1204, Max = 1500,Color = Color.FromArgb(0, 255, 80) }
 };

            //act find subsurf for one interval

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, resItems).ToList();

            //assert  
            
        }

    }
}
