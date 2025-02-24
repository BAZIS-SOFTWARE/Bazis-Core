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
using PostProc.IO.ResultExport;


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
            var elsResults = controller.ResultsFieldsCreator.CreateSurfaceObjects(results.First(), ObjType.Узел, "T", els3D.GetObjects());

            Assert.AreNotEqual(elsResults, null);
            Assert.AreNotEqual(elsResults.Count(), 0);
        }

        [TestMethod]
        public void TestLoadDBSQLiteFindByTimeInterval()
        {
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };

            var project = new ProjectData("", @"..\..\..\Examples\ProjAndResults");
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

            proj.Load(@"..\..\..\Examples\ProjAndResults\two_tetra_term_cycle_multiStep.bpf");

            proj.ResultData = new ResultData();

            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var results = loaderDB.Load(@"..\..\..\Examples\ProjAndResults\термическая_0_0_0.5.db");

            proj.ResultData.AddRange(results);

            IEnumerable<IElement> elements;
            if (proj.TaskType == TaskType.Volume)
            {
                elements = proj.ModelData.ObjectData.E3DCollection.GetObjects();
            }
            else elements = proj.ModelData.ObjectData.E2DCollection.GetObjects();

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
            proj.Load(@"..\..\..\Examples\ProjAndResults\two_tetra_term_cycle_multiStep.bpf");

            proj.ResultData = new ResultData();

            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };

            var results1 = loaderDB.Load(@"..\..\..\Examples\ProjAndResults\термическая_0_0_0.5.db");
            proj.ResultData.AddRange(results1, new ResultsComparer());

            var results0 = loaderDB.Load(@"..\..\..\Examples\ProjAndResults\термическая_0_0_0.5.db");
            proj.ResultData.AddRange(results0,new ResultsComparer());

            //Assert.AreEqual(project.ResultData.Count(), results.Count);
        }

        [TestMethod]
        public void CreateDBSQLite_ResultPage_Result_Presenter()
        {
            var project = new ProjectData("", @"..\..\..\Examples\ProjAndResults");
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
            var csvPath = @"..\..\..\ResultTableCSV";

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
            @"..\..\..\Examples\ProjAndResults")]
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
                new ItemRange() { Min = min, Max = max, Color = Color.FromArgb(0, 0, 255) }
};

            var resultsCreator = new ResultsFieldsCreator();
            resultsCreator.ScaleFactor = 1;
            resultsCreator.SetScaleItems(itemRanges);

            var resultObjects = resultsCreator.CreateSurfaceObjects(result.First(), ObjType.Узел,resDesc, elements.GetObjects());
            var points = new List<ResultPoint>();
            foreach (var elems in resultObjects)
                points.AddRange(elems.GetVertexes());

            var gridPath = $@"{resPath}\testGridSafe.bpf";
            var saver = new SaveGridSurfacesToBPF();
            saver.SaveGrid(resultObjects, gridPath);

            var TXTSaver = new SaveObjectsResultsToTXT();
            var resultNodesTXTPath = $@"{resPath}\testResultNodes.txt";
            var resultElementsTXTPath = $@"{resPath}\testResultElemets.txt";

            TXTSaver.SaveResults(proj.ModelData.ObjectData.NodesSet.Values, result.First(), resDesc, resultNodesTXTPath);
            TXTSaver.SaveResults(proj.ModelData.ObjectData.GetAllElements(), result.First(), resDesc, resultElementsTXTPath);

            var CSVSaver = new SaveObjectsResultsToCSV();
            var resultNodesCSVPath = $@"{resPath}\testResultNodes.csv";
            var resultElementsCSVPath = $@"{resPath}\testResultElements.csv";

            CSVSaver.SaveResults(proj.ModelData.ObjectData.NodesSet.Values, result.First(), resDesc, resultNodesCSVPath);
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
