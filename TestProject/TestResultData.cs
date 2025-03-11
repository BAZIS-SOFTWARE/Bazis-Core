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
using Model.IO;
using Model.Interfaces.ObjectsCollections;
using Model.ObjectsCollections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;


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

            var results = loaderDB.GetResultFromDb(project.GeneralData.Path + $@"\{resName}",
                $"nodes", time) ; 

            Assert.AreNotEqual(results, null);
            Assert.AreNotEqual(results.Data.Rows.Count, 0);
        }

        [TestMethod]
        [DataRow(@"..\..\..\Examples\ProjAndResults", "термическая_0_0_0.5.db")]
        public void TestLoadDBSQLiteFindByTimeInterval(string path, string dbName)
        {
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };

            var project = new ProjectData("", path);
            var results = loaderDB.Load(project.GeneralData.Path + $@"\{dbName}");

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
        [DataRow(@"..\..\..\Examples\ProjAndResults\термическая_0_0_0.5.db")]
        public void TestMergeResults(string dbName)
        {
            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();

            proj.Load(@"..\..\..\Examples\ProjAndResults\two_tetra_term_cycle_multiStep.bpf");

            proj.ResultDB = dbName;

            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var results = loaderDB.Load(proj.ResultDB);

            IEnumerable<IElement> elements;
            if (proj.GeneralData.TaskType == TaskType.Volume)
            {
                elements = proj.ModelData.ObjectData.E3DCollection.GetObjects();
            }
            else elements = proj.ModelData.ObjectData.E2DCollection.GetObjects();

            var interfaceNodesFinder = new InterfacedNodesFinder();

            var dbFile = Path.GetFileNameWithoutExtension(dbName);
            var ar = dbFile.Split('_');
            var kind = ar[0];
            var fTime = Convert.ToSingle(ar[2],CultureInfo.InvariantCulture);
            var sTime = Convert.ToSingle(ar[3], CultureInfo.InvariantCulture);
            var search = results.Where(x => x.TaskKind.ToString() == kind &&

x.Time.CompareTo(fTime) >= 0
& x.Time.CompareTo(sTime) <= 0);

            var mergeResults = new MergeResults();

            mergeResults.Merge(interfaceNodesFinder.Find(elements), "Q", search);
        }

        [DataTestMethod]
        [DataRow("csv_table_1.csv")]
        public void CreateAndSaveDBSQLite_Example(string file)
        {
            var csvPath = @"..\..\..\ResultTableCSV";

            var convert = new CSVToDataTableConverter();

            var dataTable = convert.ConvertCSVtoDataTable($@"{csvPath}\{file}");
            dataTable.TableName = "nodes";

            var result = new Result(dataTable, 0.87051f,TaskKind.термическая);

            var dbPath = $@"{csvPath}\results.db";

            var saverDB = new SaveResultsFileDb();
            saverDB.Save(new List<IResult>() { result }, dbPath, false);
   
            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var times = loaderDB.GetValuesFromDb(dbPath, "nodes", "Time");

            var resultTOLoad = loaderDB.GetResultFromDb(dbPath, $"nodes", times.First());

            Assert.AreEqual(resultTOLoad.TaskKind, TaskKind.механическая);
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
            proj.ModelData = new ModelData();

            var ext = Path.GetExtension(projName);
            if (ext == ".bpf2")
                proj.ModelData.Loader = new LoadModelFromBPF2TextFile();

            proj.TaskData = new TaskData();
            proj.Loader.LoadEvent += (ar1, ar2) => { };

            proj.Load($@"{resPath}\{projName}");

            var dbExtension = System.IO.Path.GetExtension($@"{resPath}\{resName}");
            //var dbExtension = System.IO.Path.GetExtension(@"C:\Bazis_newGUI\PROJ\Test_3D\Term\Termal_0_0,5.db");

            var loaderDB = new LoadResultsFileDB();
            loaderDB.LoadEvent += (ar1, ar2) => { };
            var results = loaderDB.Load($@"{resPath}\{resName}");

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

            var resultObjects = resultsCreator.CreateSurfaceObjects(results.First(), ObjType.Узел,resDesc, elements);
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
