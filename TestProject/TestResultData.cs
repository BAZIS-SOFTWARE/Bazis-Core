using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project;
using Model;
using System.Globalization;
using System.Threading;
using ModelController.MeshObjsUtility;
using Project.Interfaces;
using Results.ResultsData;
using Project.Interfaces.Tasks;
using Model.MeshObjects;
using ModelController.ModelScenePresentator;
using Model.Interfaces.MeshObjects;
using Model.Interfaces;
using System.Data;
using System.IO;
using Project.Results.IO;
using PostProc.ScenePresenter;
using PostProc;
using Project.Tasks;
using Project.Results;
using Results.ScenePresenter;
using PostProc.IO.GridExport;
using Project.Results.IO.ResultExport;


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
        [DataRow(@"..\..\Examples\ProjAndResults", "термическая_0_0_0.5.db", 0.5f)]
        [DataRow(@"..\..\Examples\ProjAndResults", "термическая_0_0_1500.db", 200.8569f)]
        public void TestLoadDBSQLiteFindByTime(string path, string resName, float time)
        {
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1,ar2) => { };
            var project = new ProjectData("", path);

            var results = loaderDB.Load(project.GeneralData.Path + $@"\{resName}");


            project.ResultData = new ResultData();

            project.ResultData.AddRange(results);

            var search = project.ResultData.FindByTime("термическая", time);

            Assert.AreNotEqual(search, null);
            Assert.AreNotEqual(search.Data.Tables.Count, 0);
            Assert.AreNotEqual(search.Data.Tables[0].Rows.Count, 0);
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
            project.ModelData = new ModelData();
            project.Load();
            var results = loaderDB.Load(project.GeneralData.Path + $@"\{resName}");
            //var search = results.Where(x => x.TaskKind.ToString() == "термическая" & x.Time == time);
            //var resList = search.ToList();
            //var saverDB = new SaveResultsFileDb();
            //saverDB.Save(resList, dir + "\\temp.db", false);

            var els3D = project.ModelData.ObjectData.E3DCollection;
            var controller = new ResultsController();

            var intervalVals = new ItemRange[]
{
                new ItemRange() { Min = 20, Max = 316 },
                new ItemRange() { Min = 316, Max = 612 },
                new ItemRange() { Min = 612, Max = 908 },
                new ItemRange() { Min = 908, Max = 1204 },
                new ItemRange() { Min = 1204, Max = 1500 }
};
            //act find subsurf for one interval
            var сolorRange = new Color[5];

            сolorRange[0] = Color.FromArgb(0, 0, 255);
            сolorRange[1] = Color.FromArgb(0, 128, 255);
            сolorRange[2] = Color.FromArgb(0, 128, 255);
            сolorRange[3] = Color.FromArgb(0, 128, 255);
            сolorRange[4] = Color.FromArgb(0, 128, 255);

            controller.ResultsFieldsCreator.SetScaleItems(сolorRange, intervalVals);
            controller.ResultsFieldsCreator.ScaleFactor = 1;
            var elsResults = controller.ResultsFieldsCreator.CreateSurfaceObjects(results.First(), ObjType.Узел, "T", els3D);

            Assert.AreNotEqual(elsResults, null);
            Assert.AreNotEqual(elsResults.Count(), 0);
        }

        [TestMethod]
        public void TestLoadDBSQLiteFindByTimeInterval()
        {
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };

            var project = new ProjectData("", @"..\..\Examples\ProjAndResults");
            var results = loaderDB.Load(project.GeneralData.Path + @"\термическая_0_0_0.5.db");

            project.ResultData = new ResultData();

            project.ResultData.AddRange(results);

            var search = project.ResultData.FindByTimeInterval("термическая", 0,0.25f);
        }

        [TestMethod]
        public void TestMergeResults()
        {
            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();

            proj.Load(@"..\..\Examples\ProjAndResults\two_tetra_term_cycle_multiStep.bpf");

            proj.ResultData = new ResultData();

            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var results = loaderDB.Load(@"..\..\Examples\ProjAndResults\термическая_0_0_0.5.db");

            proj.ResultData.AddRange(results);

            IElement[] elements;
            if (proj.TaskType == TaskType.Volume)
            {
                elements = proj.ModelData.ObjectData.E3DCollection.ToArray();
            }
            else elements = proj.ModelData.ObjectData.E2DCollection.ToArray();

            var interfaceNodesFinder = new InterfacedNodesFinder();
            var resultsFind = proj.ResultData.FindByTimeInterval("термическая", 0, 0.5f);

            var mergeResults = new MergeResults();

            mergeResults.Merge(interfaceNodesFinder.Find(elements), "Q", resultsFind.ToList());
        }

        [TestMethod]
        public void TestDistinctResults()
        {
            //assign
            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();

            //act
            proj.Load(@"..\..\Examples\ProjAndResults\two_tetra_term_cycle_multiStep.bpf");

            proj.ResultData = new ResultData();

            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };

            var results1 = loaderDB.Load(@"..\..\Examples\ProjAndResults\термическая_0_0_0.5.db");
            proj.ResultData.AddRange(results1, new ResultsComparer());

            var results0 = loaderDB.Load(@"..\..\Examples\ProjAndResults\термическая_0_0_0.5.db");
            proj.ResultData.AddRange(results0,new ResultsComparer());

            //Assert.AreEqual(project.ResultData.Count(), results.Count);
        }

        [TestMethod]
        public void CreateDBSQLite_ResultPage_Result_Presenter()
        {
            var project = new ProjectData("", @"..\..\Examples\ProjAndResults");
            project.ResultData = new ResultData();
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var results = loaderDB.Load(project.GeneralData.Path + @"\термическая_0_0_0.5.db");
            project.ResultData.AddRange(results);

            //var resPagePresenter = new ResultPagePresenter(project.ResultData);

 
            var kindRes = project.ResultData.GetResultKinds();
            var timeRes = project.ResultData.FindByTime("термическая",0);

            if (kindRes.Count() == 0)
                throw new Exception();
            foreach (var res in kindRes)
                Assert.AreEqual(res, TaskKind.термическая);

            Assert.AreEqual(timeRes.Time, 0);
        }

        [DataTestMethod]
        [DataRow("csv_table_1.csv")]
        public void CreateAndSaveDBSQLite_Example(string file)
        {
            var csvPath = @"..\..\ResultTableCSV";

            var convert = new CSVToDataTableConverter();

            var dataTable = convert.ConvertCSVtoDataTable($@"{csvPath}\{file}");
            dataTable.TableName = "nodes";
            var dataSet = new System.Data.DataSet();
            dataSet.Tables.Add(dataTable);
            var result = new Result(dataSet,0.87051f,TaskKind.термическая);

            var db = new ResultDataBase($@"{csvPath}\results.db");

            db.OpenConnection();

            db.DeleteTable("nodes");
            db.DeleteTable("elements");

            db.CreateTables("elements", GetTableSchema(result.Data.Tables["nodes"]));
            db.CreateTables("nodes", GetTableSchema(result.Data.Tables["nodes"]));

            db.AddData(result);
            db.CloseConnection();

            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var resultTOLoad = loaderDB.Load($@"{csvPath}\results.db");

            Assert.AreEqual(resultTOLoad.First().TaskKind, TaskKind.механическая);
        }

        public List<string> GetTableSchema(DataTable dataTable)
        {
            var schema = new List<string>();

            for (int i = 1; i < dataTable.Columns.Count; i++)
            {
                schema.Add(dataTable.Columns[i].ColumnName);
            }

            return schema;
        }

        [DataTestMethod]
        [DataRow(
            "two_tetra_term_cycle_multiStep.bpf","термическая_0_0_0.5.db","термическая", 0f,0.0002f,20,"T",
            @"..\..\Examples\ProjAndResults")]
        public void Test_Scene_Result_Presenter(string projName,string resName,string resKind,float min,float max,float curVal,string resDesc,string resPath )
        {
            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();
            proj.Loader.LoadEvent += (ar1, ar2) => { };

            proj.Load($@"{resPath}\{projName}");
            proj.ResultData = new ResultData();
            var dbExtension = System.IO.Path.GetExtension($@"{resPath}\{resName}");
            //var dbExtension = System.IO.Path.GetExtension(@"C:\Bazis_newGUI\PROJ\Test_3D\Term\Termal_0_0,5.db");

            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var results = loaderDB.Load($@"{resPath}\{resName}");

            proj.ResultData.AddRange(results);
            var result = proj.ResultData.FindByTaskKind(resKind);

            var elements = proj.ModelData.ObjectData.E3DCollection;

            var itemRanges = new ItemRange[]
{
                new ItemRange() { Min = min, Max = max }
};
            //act find subsurf for one interval
            var сolorRanges = new Color[1];
            сolorRanges[0] = Color.FromArgb(0, 0, 255);

            var resultsCreator = new ResultsFieldsCreator();
            resultsCreator.ScaleFactor = 1;
            resultsCreator.SetScaleItems(сolorRanges, itemRanges);

            var resultObjects = resultsCreator.CreateSurfaceObjects(result.First(), ObjType.Узел,resDesc, elements);
            var points = new List<ResultPoint>();
            foreach (var elems in resultObjects)
                points.AddRange(elems.GetVertexes());

            var gridPath = $@"{resPath}\testGridSafe.bpf";
            var saver = new SaveGridSurfacesToBPF();
            saver.SaveGrid(resultObjects, gridPath);

            var TXTSaver = new SaveObjectsResultsToTXT();
            var resultNodesTXTPath = $@"{resPath}\testResultNodes.txt";
            var resultElementsTXTPath = $@"{resPath}\testResultElemets.txt";

            TXTSaver.SaveResults(proj.ModelData.ObjectData.NodeCollection, result.First(), resDesc, resultNodesTXTPath);
            TXTSaver.SaveResults(proj.ModelData.ObjectData.GetAllElements(), result.First(), resDesc, resultElementsTXTPath);

            var CSVSaver = new SaveObjectsResultsToCSV();
            var resultNodesCSVPath = $@"{resPath}\testResultNodes.csv";
            var resultElementsCSVPath = $@"{resPath}\testResultElements.csv";

            CSVSaver.SaveResults(proj.ModelData.ObjectData.NodeCollection, result.First(), resDesc, resultNodesCSVPath);
            CSVSaver.SaveResults(proj.ModelData.ObjectData.GetAllElements(), result.First(), resDesc, resultElementsCSVPath);

            var modelPresenter = new PresentersCreator();
            var presenter = modelPresenter.CreateSurfaceObjectsPresenter(resultObjects, false);

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
        public void TestSurfaceGradientsCreator_TryIntervalsIntersecCloseResults(float v1,float v2,bool res)
        {
            //arrange

            var nodesCoords = new Point3D[]
            {
                new Point3D(1, 0, 0),
                new Point3D(0, 1, 0)
            };

            var surfGrad = new SurfaceGradientsCreator();

            var nodesVal = new float[] { v1, v2};

            var interval = new ItemRange() { Min = 1, Max = 10 };


            //act find subsurf for one interval
            List<Point3D> egCoords;
            var resTry = surfGrad.TryIntervalsIntersec(interval, nodesVal, nodesCoords, out egCoords);

            //assert  
            Assert.AreEqual(resTry, res);
        }


        [TestMethod]
        public void TestSurfaceGradientsCreator_OutOfIntervalsUp()
        {
            //arrange

            var nodes = new Node[]
            {
                new Node(0,new Point3D(1, 0, 0)),
                new Node(1,new Point3D(0, 1, 0)),
                new Node(2,new Point3D(0, 0, 0))
            };

            var surfGrad = new SurfaceGradientsCreator();

            var nodesVal = new List<float>() { 7, 8, 6 };

            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 1, Max = 2 },
                new ItemRange() { Min = 2, Max = 3 },
                new ItemRange() { Min = 3, Max = 4 },
                new ItemRange() { Min = 4, Max = 5 }
        };

            var сolorRange = new Color[4];

            сolorRange[0] = Color.FromArgb(0, 0, 255);
            сolorRange[1] = Color.FromArgb(0, 128, 255);
            сolorRange[2] = Color.FromArgb(0, 255, 255);
            сolorRange[3] = Color.FromArgb(0, 255, 80);


            //act find subsurf for one interval

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, сolorRange, nodes.Select(x => x.Position).ToList(), nodesVal).ToList();

            //assert  
            Assert.AreEqual(resSurfaces.Count, 1);
            Assert.AreEqual(resSurfaces[0].MasterColor, сolorRange[3]);
        }

        [TestMethod]
        public void TestSurfaceGradientsCreator_OutOfIntervalsDown()
        {
            //arrange

            var nodes = new Node[]
            {
                new Node(0,new Point3D(1, 0, 0)),
                new Node(1,new Point3D(0, 1, 0)),
                new Node(2,new Point3D(0, 0, 0))
            };

            var surfGrad = new SurfaceGradientsCreator();

            var nodesVal = new List<float>() { -1, 0, 0.5f };

            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 1, Max = 2 },
                new ItemRange() { Min = 2, Max = 3 },
                new ItemRange() { Min = 3, Max = 4 },
                new ItemRange() { Min = 4, Max = 5 }
        };

            var сolorRange = new Color[4];

            сolorRange[0] = Color.FromArgb(0, 0, 255);
            сolorRange[1] = Color.FromArgb(0, 128, 255);
            сolorRange[2] = Color.FromArgb(0, 255, 255);
            сolorRange[3] = Color.FromArgb(0, 255, 80);


            //act find subsurf for one interval

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, сolorRange, nodes.Select(x => x.Position).ToList(), nodesVal).ToList();

            //assert  
            Assert.AreEqual(resSurfaces.Count, 1);
            Assert.AreEqual(resSurfaces[0].MasterColor, сolorRange[0]);
        }

        [TestMethod]
        [DataRow(19.99998f, 19.99998f, 20.00001f, 20.00001f)]
        [DataRow(20.00001f, 19.99998f, 20.00001f, 19.99998f)]
        public void TestSurfaceGradientsCreator_OneTetraTwoIntervalsShouldCreateResults(float v1,float v2,float v3,float v4)
        {
            //arrange
            var nodes = new Node[]
            {
                new Node(0,new Point3D(1, 0, 0)),
                new Node(1,new Point3D(0, 1, 0)),
                new Node(2,new Point3D(0, 0, 0)),
                new Node(3,new Point3D(0, 0, 1))
            };


            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 20, Max = 30 },
                new ItemRange() { Min = 30, Max = 40 }
        };
            //act find subsurf for one interval
            var сolorRange = new Color[2];

            сolorRange[0] = Color.FromArgb(0, 0, 255);
            сolorRange[1] = Color.FromArgb(0, 128, 255);

            var nodesVal = new List<float>() { v1, v2, v3,v4 };
            var surfGrad = new SurfaceGradientsCreator();

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, сolorRange, nodes.Select(x => x.Position).ToList(), nodesVal).ToList();

            //assert           
            Assert.AreNotEqual(resSurfaces.Count, 0);
        }



        [TestMethod]
        public void TestSurfaceGradientsCreator_OneSurfaceTwoIntervals()
        {
            //arrange
            var nodes = new Node[]
            {
                new Node(0,new Point3D(1, 0, 0)),
                new Node(1,new Point3D(0, 1, 0)),
                new Node(2,new Point3D(0, 0, 0))
            };

 
            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 2, Max = 3 },
                new ItemRange() { Min = 3, Max = 4 }
        };
            //act find subsurf for one interval
            var сolorRange = new Color[2];

            сolorRange[0] = Color.FromArgb(0, 0, 255);
            сolorRange[1] = Color.FromArgb(0, 128, 255);

            var nodesVal = new List<float>() { 1, 4, 5 };
            var surfGrad = new SurfaceGradientsCreator();

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, сolorRange, nodes.Select(x => x.Position).ToList(), nodesVal).ToList();

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
        public void TestSurfaceGradientsCreator_OneSurfaceSeveralIntervals()
        {
            //arrange

            var nodes = new Node[]
            {
                new Node(0,new Point3D(1, 0, 0)),
                new Node(1,new Point3D(0, 1, 0)),
                new Node(2,new Point3D(0, 0, 0))
            };

            var surfGrad = new SurfaceGradientsCreator();

            var nodesVal = new List<float>() { 1, 4, 5 };

            var intervalVals = new ItemRange[]
            {
                new ItemRange() { Min = 1, Max = 2 },
                new ItemRange() { Min = 2, Max = 3 },
                new ItemRange() { Min = 3, Max = 4 },
                new ItemRange() { Min = 4, Max = 5 }
        };

            var сolorRange = new Color[4];

            сolorRange[0] = Color.FromArgb(0, 0, 255);
            сolorRange[1] = Color.FromArgb(0, 128, 255);
            сolorRange[2] = Color.FromArgb(0, 255, 255);
            сolorRange[3] = Color.FromArgb(0, 255, 80);


            //act find subsurf for one interval

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, сolorRange, nodes.Select(x => x.Position).ToList(), nodesVal).ToList();

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
                    new Point3D(0.25f, 0, 0),
                    new Point3D(0, 0, 0)
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

            var nodes = new Node[]
            {
                new Node(0,new Point3D(59003.5f, 12318.13f, 52106.64f)),
                new Node(1,new Point3D(59005.25f, 12318.13f, 52106.64f)),
                new Node(2,new Point3D(59003.5f, 12315.9f, 52104.03f))
            };

            var surfGrad = new SurfaceGradientsCreator();

            var nodesVal = new List<float>() { 1, 4, 5 };

            var intervalVals = new ItemRange[]
 {
                new ItemRange() { Min = 20, Max = 316 },
                new ItemRange() { Min = 316, Max = 612 },
                new ItemRange() { Min = 612, Max = 908 },
                new ItemRange() { Min = 908, Max = 1204 },
                new ItemRange() { Min = 1204, Max = 1500 }
 };

            var сolorRange = new Color[5];

            сolorRange[0] = Color.FromArgb(0, 0, 255);
            сolorRange[1] = Color.FromArgb(0, 128, 255);
            сolorRange[2] = Color.FromArgb(0, 255, 255);
            сolorRange[3] = Color.FromArgb(0, 255, 80);
            сolorRange[4] = Color.FromArgb(0, 255, 80);


            //act find subsurf for one interval

            var resSurfaces = surfGrad.CreateGradientObjects(intervalVals, сolorRange, nodes.Select(x => x.Position).ToList(), nodesVal).ToList();

            //assert  
            
        }

    }
}
