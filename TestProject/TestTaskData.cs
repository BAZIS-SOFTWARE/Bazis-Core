using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Newtonsoft.Json;
using System.IO;
using Project;
using Project.Interfaces.Tasks;
using Model.GroupsData;
using Model.Interfaces;
using Geometry;
using System.Runtime.InteropServices;
using Project.Tasks;
using Project.Tasks.Functions;
using Project.TaskParameters;
using Project.Tasks.FrameCreators;
using System.Runtime.InteropServices.ComTypes;

namespace TestProject
{
    [TestClass]
    public class TestTaskData
    {
        string testDir = @"..\..\..\Examples";
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; 
        }
        [TestMethod]
        public void RemoveTaskDataByInfoString()
        {
            //assign

            var proj = new ProjectData();

            //act
            proj.Load($@"{testDir}\proj_traj.bpf");

            var dic = proj.TaskData.Find(DataKind.Закрепление).ToArray();

            foreach (var item in dic)
                proj.TaskData.Remove(item);
            //assert
            Assert.AreEqual(proj.TaskData.Count, 3);

        }

        [TestMethod]
        public void RemoveTaskDataByExistState()
        {
            //assign

            var proj = new ProjectData();
            //act
            proj.Load($@"{testDir}\proj_traj.bpf");

            proj.ModelData.GroupData.RemoveAt(5);

            proj.TaskData.ClearNotExisted(proj.ModelData.GroupData);

            //assert
            Assert.AreEqual(proj.TaskData.Count, 5);

        }

        [TestMethod]
        public void FindTaskDataByType()
        {
            //assign

            var proj = new ProjectData();

            //act
            proj.Load(@"..\..\..\Examples\proj_traj.bpf");

            var res = proj.TaskData.Find<ICondData>();
            //assert

            Assert.AreEqual(res.Count(), 6);

        }

        [TestMethod]
        [DataRow(@"..\..\..\Examples\proj_traj.bpf", DisplayName = "proj_traj")]
        //[DataRow(@"..\..\..\Examples\proj_traj_baseLine_not_sorted.bpf", DisplayName = "proj_traj_baseLine_not_sorted")]

        //[DataRow(@"c:\projs\testProj\TubeToTubeTJoint\proj_v1.bpf")]
        //[DataRow(@"c:\projs\kamaz\4.bpf")]
        public void TestFrameFunctionInTime(string relativeProjPath)
        {
            //assign
            var proj = new ProjectData();

            //act
            proj.Load(relativeProjPath);

            var selectedData = proj.TaskData.Find(DataKind.Нагрев).Select(x => (ICondData)x).ToArray();
            foreach (var preFrame in selectedData)
            {
                var stepTime = (preFrame.StopTime - preFrame.StartTime) / 10;
                for (float i = preFrame.StartTime; i < preFrame.StopTime; i+= stepTime)
                {
                    var mf = preFrame.FrameFunction.LocalFrame as MovedFrame;
                    mf.Time = i;
                    var frame = mf.Frame;

                    if (frame.Centre._x is float.NaN || frame.Centre._y is float.NaN || frame.Centre._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат центра не определена. frame: {preFrame}");
                    if (frame.Dir_X._x is float.NaN || frame.Dir_X._y is float.NaN || frame.Dir_X._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат оси X не определена. frame: {preFrame}");
                    if (frame.Dir_Y._x is float.NaN || frame.Dir_Y._y is float.NaN || frame.Dir_Y._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат оси Y не определена. frame: {preFrame}");
                    if (frame.Dir_Z._x is float.NaN || frame.Dir_Z._y is float.NaN || frame.Dir_Z._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат оси Z не определена. frame: {preFrame}");
                }

            }
            //var frame = selectedData[0].MovedFrameFunction.CalcFrame(0);
            //assert
        }

        [TestMethod]
        public void TestFrameFunctionSortingTrajectory()
        {
            //assign
            var proj = new ProjectData();

            //act
            proj.Load(@"..\..\..\Examples\proj_traj.bpf");

            var selectedData = proj.TaskData.Find(DataKind.Нагрев).Select(x => (ICondData)x).ToArray();

            

            var baseLine = (selectedData[0].FrameFunction.LocalFrame as MovedFrame).BaseLine;
            //assert
            var first = baseLine[0];
            var secd = baseLine[1];
            var startLength = new Segment3D(first.CalcCentr(), secd.CalcCentr());
            foreach (var node in baseLine.Skip(1))
            {
                var line = new Segment3D(first.CalcCentr(), node.CalcCentr());
                if(startLength.GetLength() > line.GetLength())
                    throw new Exception("траектория вероятно не отсортирована!");
            }
            //
        }

        [DataTestMethod]
        [DataRow(10.1f,1,0.5f,0)]
        [DataRow(0,0,0,0)]
        [DataRow(0, 0, 0, 0)]
        public void TestFrameFunctionTransformation(float rotation,float x,float y,float z)
        {
            //assign
            var proj = new ProjectData();

            //act
            proj.Load(@"..\..\..\Examples\proj_traj.bpf");

            var heatData = proj.TaskData.Find(DataKind.Нагрев).Select(h => (HeatData)h).First();
            heatData.FrameFunction.LocalFrame.Rotation = rotation;
            heatData.FrameFunction.LocalFrame.Shifting = new Point3D(x,y,z);
            //assert
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Rotation, rotation);
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Shifting, new Point3D(x,y,z));
        }

        [DataTestMethod]
        [DataRow(DataKind.Нагрев)]
        [DataRow(DataKind.Среда)]
        [DataRow(DataKind.Закрепление)]
        [DataRow(DataKind.Материал)]
        public void TestFindTaskData(DataKind datakind)
        {
            //assign
            var project = new ProjectData();

            //act
            project.Load(@"..\..\..\Examples\proj_traj.bpf");
 
            //assert

            var nodeInfo = project.ModelData.GroupData.Find("Узлы");

            var dataInfo = project.TaskData.Find(datakind);
            if (dataInfo.Count() == 0)
                throw new Exception("Ошибка выгрузки данных");
        }

        // TO DO Добавить тесты на исключения 
        //private void SetUpTestLoadBoundTaskDataException(DataKind dataKind, string dataString)
        //{
        //    //assign
        //    var project = new ProjectData("", "");
        //    var dataAr = dataString.Split(' ');
        //    PhysicalData data;
        //    switch (dataKind)
        //    {
        //        case DataKind.Материал:
        //            data = new MatData(new Group("", ObjType.Элемент3D), dataString)
        //            { Group = new Group(dataAr[0], ObjType.Элемент3D) };
        //            break;
        //        case DataKind.Среда:
        //            data = new MediaData(new Group("", ObjType.Элемент3D), dataString)
        //            { Group = new Group(dataAr[1], ObjType.Элемент3D) };
        //            break;
        //        case DataKind.Нагрев:
        //            var heatData = new HeatData(new Group("", ObjType.Элемент3D), dataString)
        //            { Group = new Group(dataAr[1], ObjType.Элемент3D) };
        //            var baseLine = new Group("baseLine", ObjType.Узел);
        //            var refLine = new Group("refLine", ObjType.Узел);

        //            heatData.FrameFunction.LocalFrame = new MovedFrame(baseLine, refLine, 5);

        //            data = heatData;
        //            break;
        //        case DataKind.Закрепление:
        //            data = new ClampData(new Group("", ObjType.Узел), dataString)
        //            { Group = new Group(dataAr[0], ObjType.Узел) };
        //            break;
        //        default:
        //            data = new LoadData(new Group("", ObjType.Узел), dataString)
        //            { Group = new Group(dataAr[0], ObjType.Узел) };
        //            break;
        //    }
        //}

        //[DataTestMethod]
        //[DataRow(DataKind.Среда, "")]
        //[DataRow(DataKind.Среда, "Элементы3D_3")]

        //[DataRow(DataKind.Материал, "")]
        //[DataRow(DataKind.Материал, "Элементы3D_3")]
        //[DataRow(DataKind.Материал, "Элементы3D_3 Сталь_10 ")]

        //[DataRow(DataKind.Нагрев, "")]
        //[DataRow(DataKind.Нагрев, "ARC;4;100;25 Load 0 49.99998")]
        //[DataRow(DataKind.Нагрев, "Load 0 49.99998 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]
        //[DataRow(DataKind.Нагрев, "ARC;4;100;25 0 49.99998 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]
        //[DataRow(DataKind.Нагрев, "ARC;4;100;25 Load 49.99998 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]
        //[DataRow(DataKind.Нагрев, "ARC;4;100;25 Load 0 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]

        //[DataRow(DataKind.Закрепление, "")]

        //[DataRow(DataKind.Нагрузка, "")]
        //[DataRow(DataKind.Нагрузка, "Сила Z 0 * 0 1500 *")]
        //[DataRow(DataKind.Нагрузка, "Узлы_7 Z 0 * 0 1500 *")]
        //[DataRow(DataKind.Нагрузка, "Узлы_7 Сила 0 * 0 1500 *")]
        //[DataRow(DataKind.Нагрузка, "Узлы_7 Сила Z * 0 1500 *")]
        //[ExpectedException(typeof(Exception))]
        //public void TestTaskDataException(DataKind dataKind, string dataString) =>
        //    SetUpTestLoadBoundTaskDataException(dataKind, dataString);

        //[DataTestMethod]
        //[DataRow(DataKind.Материал, "Сталь_20ХМ 0 1500 *")]
        //[DataRow(DataKind.Материал, "Элементы3D_3 Сталь_20ХМ  1500 *")]
        //[DataRow(DataKind.Материал, "Элементы3D_3 Сталь_20ХМ 0  *")]
        //[DataRow(DataKind.Материал, "Элементы3D_3 Сталь_10 * * *")]

        //[DataRow(DataKind.Среда, "Элементы3D_2  20 0 1")]
        //[DataRow(DataKind.Среда, "Коэф.теплоотдачи.воздух 20 0 1 *")]
        //[DataRow(DataKind.Среда, "Элементы3D_2 20 0 1 *")]
        //[DataRow(DataKind.Среда, "Элементы3D_2 Коэф.теплоотдачи.воздух 0 1 *")]
        //[DataRow(DataKind.Среда, "Элементы3D_2 Коэф.теплоотдачи.воздух 20 1 *")]
        //[DataRow(DataKind.Среда, "Элементы3D_2 Коэф.теплоотдачи.воздух 20 0 *")]

        //[DataRow(DataKind.Закрепление, "Жесткое X * 0 1500 *")]
        //[DataRow(DataKind.Закрепление, "Узлы_7 X * 0 1500 *")]
        //[DataRow(DataKind.Закрепление, "Узлы_7 Жесткое * 0 1500 *")]
        //[DataRow(DataKind.Закрепление, "Узлы_7 Жесткое X 0.0.0 1500 *")]
        //[DataRow(DataKind.Закрепление, "Узлы_7 Жесткое X r5r 1500 *")]
        //[DataRow(DataKind.Закрепление, "Узлы_7 Жесткое X !!! 0 *")]

        //[DataRow(DataKind.Нагрузка, "Узлы_7 Сила Z 0 0 1500 *")]
        //[DataRow(DataKind.Нагрузка, "Узлы_7 Сила Z 0 0,0! 1500 *")]
        //[DataRow(DataKind.Нагрузка, "Узлы_7 Сила Z 0 000!& 0 *")]
        //[ExpectedException(typeof(Exception))]
        //public void TestTaskDataFormatException(DataKind dataKind, string dataString) =>
        //    SetUpTestLoadBoundTaskDataException(dataKind, dataString);

        [DataTestMethod]
        [DataRow("1 PowerCycle * Load 0 49.99998")]
        [DataRow("0.001 * * Load 0 49.99998")]
        [DataRow("1 timeFunc * Load 0.0001 49")]
        

        public void CheckHeatData(string dataString)
        {
            //assign
            var strAr = dataString.Split(' ');
            var startTime = float.Parse(strAr[4]);
            var stopTime = float.Parse(strAr[5]);

            var heatData = new HeatData(new Group(strAr[3], ObjType.Узел), startTime, stopTime);
            
            heatData.TrySetFrameFunction(strAr[2]);
            heatData.TrySetValue(strAr[0]);
            heatData.TrySetTimeFunction(strAr[1]);

            //assert
            Assert.AreEqual($"{DataKind.Нагрев} : {dataString}", heatData.ToString());

        }

        [DataRow("Узлы_7 Жесткое X 0.255 * 0 1500 *")]
        [DataRow("Узлы_7 Жесткое None 0.255 * 0 1500 *")]
        [DataRow("Узлы_7 Жесткое X 1 Функция 0 1500 *")]
        [DataRow("Узлы_7 Жесткое Y 0 * 0 1500 *")]
        [DataRow("Узлы_7 Жесткое Z 0 * 0 1500 *")]

        public void CheckClampData(string dataString)
        {
            //assign
            var strAr = dataString.Split(' ');
            var startTime = float.Parse(strAr[5]);
            var stopTime = float.Parse(strAr[6]);

            var clampData = new ClampData(new Group(strAr[0], ObjType.Узел), startTime, stopTime);
            clampData.TrySetKind(strAr[1]);
            clampData.TrySetDirection(strAr[2]);
            clampData.TrySetValue(strAr[3]);
            clampData.TrySetTimeFunction(strAr[4]);

            //assert
            Assert.AreEqual($"{DataKind.Закрепление} : {dataString}", clampData.ToString());

        }

        [DataRow("Узлы_7 Сила None 0 * 0 1500 *")]
        [DataRow("Узлы_7 Сила Z 0 * 0 1500 *")]
        [DataRow("Узлы_7 Сила X 0.0001 loadCycle 0 1500 *")]
        public void CheckLoadData(string dataString)
        {
            //assign
            var strAr = dataString.Split(' ');
            var startTime = float.Parse(strAr[5]);
            var stopTime = float.Parse(strAr[6]);

            var loadData = new LoadData(new Group(strAr[0], ObjType.Узел), startTime,stopTime);
            loadData.TrySetKind(strAr[1]);
            loadData.TrySetDirection(strAr[2]);
            loadData.TrySetValue(strAr[3]);
            loadData.TrySetTimeFunction(strAr[4]);

            //assert
            Assert.AreEqual($"{DataKind.Нагрузка} : {dataString}", loadData.ToString());

        }

        [DataTestMethod]
        [DataRow("air Коэф.теплоотдачи.воздух 20.005 0 1500 *", MediaType.HeatFlux, "20.005")]
        [DataRow("air * Термоцикл 0 1500 *", MediaType.ConstantTemp, "Термоцикл")]
        [DataRow("air Коэф.теплоотдачи.воздух 20 0 1500 *", MediaType.HeatFlux, "20")]
        [DataRow("air 0.002 Термоцикл 0 1500 *", MediaType.HeatFlux, "Термоцикл")]
        [DataRow("air Коэф.теплоотдачи.воздух 20 0 1500.001 *", MediaType.HeatFlux, "20")]
        [DataRow("air 1E-05 19.9999 0 1500.001 *", MediaType.HeatFlux, "19.9999")]
        [DataRow("air * 19.9999 0 1500.001 *", MediaType.ConstantTemp, "19.9999")]

        public void CheckMediaData(string dataString, MediaType mediaType, string tempVal)
        {
            //assign
            var strAr = dataString.Split(' ');
            var startTime = float.Parse(strAr[3]);
            var stopTime = float.Parse(strAr[4]);

            var data = new MediaData(new Group("air", ObjType.Элемент2D), startTime,stopTime);

            data.TrySetTemp(strAr[2]);
            data.TrySetHeatExchange(strAr[1]);
            
            //assert
            Assert.AreEqual($"{DataKind.Среда} : {dataString}", data.ToString());
            Assert.AreEqual(data.MediaType, mediaType);
            Assert.AreEqual(data.Direction, Direction.None);
        }


        [DataTestMethod]
        [DataRow(new string[] { "тепловая" })]
        [DataRow(new string[] { "механическая" })]
        [DataRow(new string[] { "химическая" })]
        public void GenerateParameterFiles(string[] ar)
        {
            GeneralParameters parameters;
            if (ar[0] == "тепловая")
                parameters = new TermalParameters();
            else if (ar[0] == "механическая")
                parameters = new MechanicalParameters();
            else
                parameters = new ChemicalParameters();

            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            parameters.MetallurgicalProcesses = new List<string>() {"Охлаждение", "Нагрев"}; 

            var serParams = string.Empty;
            if (ar[0] == "тепловая")
                serParams = JsonConvert.SerializeObject((TermalParameters)parameters, settingsSerializer);
            else if (ar[0] == "механическая")
                serParams = JsonConvert.SerializeObject((MechanicalParameters)parameters, settingsSerializer);
            else
                serParams = JsonConvert.SerializeObject((ChemicalParameters)parameters, settingsSerializer);

            File.WriteAllText($@"{testDir}\TestTaskParameters\{ar[0]}_testParam.jsp", serParams);

            if (ar[0] == "тепловая")
            {
                var desParamsTerm = JsonConvert.DeserializeObject<TermalParameters>
(File.ReadAllText($@"{testDir}\TestTaskParameters\{ar[0]}_testParam.jsp"), settingsSerializer);
            }
            else if (ar[0] == "механическая")
            {
                var desParamsMech = JsonConvert.DeserializeObject<MechanicalParameters>
(File.ReadAllText($@"{testDir}\TestTaskParameters\{ar[0]}_testParam.jsp"), settingsSerializer);
            }
            else
            {
                var desParams = JsonConvert.DeserializeObject<ChemicalParameters>
(File.ReadAllText($@"{testDir}\TestTaskParameters\{ar[0]}_testParam.jsp"), settingsSerializer);
            }
        }
    }
}
