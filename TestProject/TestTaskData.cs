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
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();
            //act
            proj.Load($@"{testDir}\proj_traj.bpf");

            var dic = proj.TaskData.Find("Закрепление").ToArray();

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
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();
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
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();
            //act
            proj.Load(@"..\..\..\Examples\proj_traj.bpf");

            var res = proj.TaskData.Find<IValuableData>();
            //assert

            Assert.AreEqual(res.Count(), 6);

        }

        [TestMethod]
        [DataRow(@"..\..\..\Examples\proj_traj.bpf", DisplayName = "proj_traj")]
        [DataRow(@"..\..\..\Examples\proj_traj_save.bpf", DisplayName = "proj_traj_save")]
        [DataRow(@"c:\projs\ElenaLu\Bulk\1.bpf", DisplayName = "1")]
        //[DataRow(@"c:\projs\testProj\TubeToTubeTJoint\proj_v1.bpf")]
        //[DataRow(@"c:\projs\kamaz\4.bpf")]
        public void TestFrameFunctionInTime(string relativeProjPath)
        {
            //assign
            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();

            //act
            proj.Load(relativeProjPath);

            var selectedData = proj.TaskData.Find("Нагрев").Select(x => (IValuableData)x).ToArray();
            foreach (var preFrame in selectedData)
            {
                var stepTime = (preFrame.StopTime - preFrame.StartTime) / 10;
                for (float i = preFrame.StartTime; i < preFrame.StopTime; i+= stepTime)
                {
                    var frame = preFrame.MovedFrame.CalcFrame(i);

                    if (frame.Centre._x is float.NaN || frame.Centre._y is float.NaN || frame.Centre._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат центра не определена. frame: {preFrame}");
                    if (frame.X._x is float.NaN || frame.X._y is float.NaN || frame.X._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат оси X не определена. frame: {preFrame}");
                    if (frame.Y._x is float.NaN || frame.Y._y is float.NaN || frame.Y._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат оси Y не определена. frame: {preFrame}");
                    if (frame.Z._x is float.NaN || frame.Z._y is float.NaN || frame.Z._z is float.NaN)
                        throw new Exception($"СК определена неверно!: одна из координат оси Z не определена. frame: {preFrame}");
                }

            }
            //var frame = selectedData[0].MovedFrameFunction.CalcFrame(0);
            //assert
        }

        [TestMethod]
        public void TestFrameFunctionSetInfo()
        {
            //assign
            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();

            //act
            proj.Load(@"..\..\..\Examples\proj_traj.bpf");

            var selectedData = proj.TaskData.Find("Нагрев").Select(x => (IValuableData)x).ToArray();


            //assert
            selectedData[0].SetInfo("ARC;4;100;25 Load 0 49.99998 baseLine|refLine;2;startNodes;startNodes");
            //
        }

        [TestMethod]
        public void TestFrameFunctionTrajectory()
        {
            //assign
            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();

            //act
            proj.Load(@"..\..\..\Examples\proj_traj.bpf");

            var selectedData = proj.TaskData.Find("Нагрев").Select(x => (IValuableData)x).ToArray();

            var baseLine = selectedData[0].MovedFrame.BaseLine;
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
        [DataRow("ARC;4;100;25 Load 0 10.3 baseLine|refLine;2;startNodes;startNodes;1|0.5|0|10.1",10.1f,1,0.5f,0)]
        [DataRow("ARC;4;100;25 Load 0 10.3 baseLine|refLine;2;startNodes;startNodes;*",0,0,0,0)]
        [DataRow("ARC;4;100;25 Load 0 10.3 baseLine|refLine;2;startNodes;startNodes", 0, 0, 0, 0)]
        public void TestFrameFunctionShifting(string input, float rotation,float p0,float p1,float p2)
        {
            //assign
            //act
            var heatData = new HeatData(new Group("", ObjType.Элемент3D), input);

            //assert
            Assert.AreEqual(heatData.MovedFrame.Rotation, rotation);
            Assert.AreEqual(heatData.MovedFrame.Shifting, new Geometry.Point3D(p0,p1,p2));
        }

        [TestMethod]
        public void TestLazerHeatSource()
        {
            //assign
            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();

            //act
            proj.Load(@"..\..\..\Examples\test_t_lazer_0_10.bpf");

            var heatData = proj.TaskData.Find("Нагрев").Select(x => (HeatData)x).ToArray()[0];

            var gr = heatData.Group;

            var lazer = (Lazer)heatData.FrameFunction;
            //assert
            var energy = new float[gr.Count];

            var counter = 0;
            var frame = heatData.MovedFrame.CalcFrame(1);
            foreach (var obj in gr)
            {
                energy[counter] = lazer.CalcPower(frame, obj.CalcCentr());
                counter++;
            }
            var max = energy.Max();
            if (max == 0)
                throw new Exception("Энергия не вводится!");
        }

        [TestMethod]
        public void TestLazerHeatSource_Rotation180_And_Shifting4_HeatSource()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            //assign
            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();
            //act
            proj.Load(@"..\..\..\Examples\test_t_lazer_180_0_10.bpf");

            var heatData = proj.TaskData.Find("Нагрев").Select(x => (HeatData)x).ToArray()[0];

            var gr = heatData.Group;

            //assert
            var energy = new Dictionary<int,float>();
            var frame = heatData.MovedFrame.CalcFrame(1);
            foreach (var obj in gr)
            {

                var lazer = (Lazer)heatData.FrameFunction;

                var energyVal = lazer.CalcPower(frame, obj.CalcCentr());
                energy.Add(obj.Number, energyVal);
            }
            var max = energy.Max(x => x.Value);

            var nanOrInfRes = energy.Where(x => float.IsNaN(x.Value) | float.IsInfinity(x.Value));

            // centre 21446 21450 21486 21642 21694
            var e1 = energy[21446];
            var e2 = energy[21450];
            var e3 = energy[21486];
            var e4 = energy[21642];
            var e5 = energy[21694];

            // side 21486 21833 21834 21835 21836 21837 21838
            var e11 = energy[21833];
            var e22 = energy[21834];
            var e33 = energy[21835];
            var e44 = energy[21836];
            var e55 = energy[21837];

            if (max == 0)
                throw new Exception("Энергия не вводится!");
            if (nanOrInfRes.Count() > 0)
                throw new Exception("Решение содержит NaN!");
        }

        [DataTestMethod]
        [DataRow("Нагрев")]
        [DataRow("Среда")]
        [DataRow("Закрепление")]
        [DataRow("Материал")]
        public void TestFindTaskData(string dataType)
        {
            //assign
            var project = new ProjectData();
            project.ModelData = new ModelData();
            project.TaskData = new TaskData();
            //act
            project.Load(@"..\..\..\Examples\proj_traj.bpf");
 
            //assert

            var nodeInfo = project.ModelData.GroupData.Find("Узлы");

            var dataInfo = project.TaskData.Find(dataType);
            if (dataInfo.Count() == 0)
                throw new Exception("Ошибка выгрузки данных");
        }

        // TO DO Добавить тесты на исключения 
        private void SetUpTestLoadBoundTaskDataException(DataKind dataKind, string dataString)
        {
            //assign
            var project = new ProjectData("", "");
            var dataAr = dataString.Split(' ');
            Data data;
            switch (dataKind)
            {
                case DataKind.Mat:
                    data = new MatData(new Group("", ObjType.Элемент3D), dataString)
                    { Group = new Group(dataAr[0], ObjType.Элемент3D) };
                    break;
                case DataKind.Med:
                    data = new MediaData(new Group("", ObjType.Элемент3D), dataString)
                    { Group = new Group(dataAr[1], ObjType.Элемент3D) };
                    break;
                case DataKind.Heat:
                    var heatData = new HeatData(new Group("", ObjType.Элемент3D), dataString)
                    { Group = new Group(dataAr[1], ObjType.Элемент3D) };
                    var baseLine = new Group("baseLine", ObjType.Узел);
                    var refLine = new Group("refLine", ObjType.Узел);
                    var startNodes = new Group("startNodes", ObjType.Узел);
                    heatData.MovedFrame.BaseLine = baseLine;
                    heatData.MovedFrame.RefLine = refLine;
                    heatData.MovedFrame.StartPoints = startNodes;
                    heatData.MovedFrame.StopPoints = startNodes;
                    data = heatData;
                    break;
                case DataKind.Clamp:
                    data = new ClampData(new Group("", ObjType.Узел), dataString)
                    { Group = new Group(dataAr[0], ObjType.Узел) };
                    break;
                default:
                    data = new LoadData(new Group("", ObjType.Узел), dataString)
                    { Group = new Group(dataAr[0], ObjType.Узел) };
                    break;
            }
        }

        [DataTestMethod]
        [DataRow(DataKind.Med, null)]
        [DataRow(DataKind.Mat, null)]
        [DataRow(DataKind.Heat, null)]
        [DataRow(DataKind.Clamp, null)]
        [DataRow(DataKind.Load, null)]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestTaskDataNullReferenceException(DataKind dataKind, string dataString) =>
            SetUpTestLoadBoundTaskDataException(dataKind, dataString);

        [DataTestMethod]
        [DataRow(DataKind.Med, "")]
        [DataRow(DataKind.Med, "Элементы3D_3")]

        [DataRow(DataKind.Mat, "")]
        [DataRow(DataKind.Mat, "Элементы3D_3")]
        [DataRow(DataKind.Mat, "Элементы3D_3 Сталь_10 ")]

        [DataRow(DataKind.Heat, "")]
        [DataRow(DataKind.Heat, "ARC;4;100;25 Load 0 49.99998")]
        [DataRow(DataKind.Heat, "Load 0 49.99998 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]
        [DataRow(DataKind.Heat, "ARC;4;100;25 0 49.99998 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]
        [DataRow(DataKind.Heat, "ARC;4;100;25 Load 49.99998 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]
        [DataRow(DataKind.Heat, "ARC;4;100;25 Load 0 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]

        [DataRow(DataKind.Clamp, "")]

        [DataRow(DataKind.Load, "")]
        [DataRow(DataKind.Load, "Сила Z 0 * 0 1500 *")]
        [DataRow(DataKind.Load, "Узлы_7 Z 0 * 0 1500 *")]
        [DataRow(DataKind.Load, "Узлы_7 Сила 0 * 0 1500 *")]
        [DataRow(DataKind.Load, "Узлы_7 Сила Z * 0 1500 *")]
        [ExpectedException(typeof(Exception))]
        public void TestTaskDataException(DataKind dataKind, string dataString) =>
            SetUpTestLoadBoundTaskDataException(dataKind, dataString);

        [DataTestMethod]
        [DataRow(DataKind.Mat, "Сталь_20ХМ 0 1500 *")]
        [DataRow(DataKind.Mat, "Элементы3D_3 Сталь_20ХМ  1500 *")]
        [DataRow(DataKind.Mat, "Элементы3D_3 Сталь_20ХМ 0  *")]
        [DataRow(DataKind.Mat, "Элементы3D_3 Сталь_10 * * *")]

        [DataRow(DataKind.Med, "Элементы3D_2  20 0 1")]
        [DataRow(DataKind.Med, "Коэф.теплоотдачи.воздух 20 0 1 *")]
        [DataRow(DataKind.Med, "Элементы3D_2 20 0 1 *")]
        [DataRow(DataKind.Med, "Элементы3D_2 Коэф.теплоотдачи.воздух 0 1 *")]
        [DataRow(DataKind.Med, "Элементы3D_2 Коэф.теплоотдачи.воздух 20 1 *")]
        [DataRow(DataKind.Med, "Элементы3D_2 Коэф.теплоотдачи.воздух 20 0 *")]

        [DataRow(DataKind.Clamp, "Жесткое X * 0 1500 *")]
        [DataRow(DataKind.Clamp, "Узлы_7 X * 0 1500 *")]
        [DataRow(DataKind.Clamp, "Узлы_7 Жесткое * 0 1500 *")]
        [DataRow(DataKind.Clamp, "Узлы_7 Жесткое X 0.0.0 1500 *")]
        [DataRow(DataKind.Clamp, "Узлы_7 Жесткое X r5r 1500 *")]
        [DataRow(DataKind.Clamp, "Узлы_7 Жесткое X !!! 0 *")]

        [DataRow(DataKind.Load, "Узлы_7 Сила Z 0 0 1500 *")]
        [DataRow(DataKind.Load, "Узлы_7 Сила Z 0 0,0! 1500 *")]
        [DataRow(DataKind.Load, "Узлы_7 Сила Z 0 000!& 0 *")]
        [ExpectedException(typeof(Exception))]
        public void TestTaskDataFormatException(DataKind dataKind, string dataString) =>
            SetUpTestLoadBoundTaskDataException(dataKind, dataString);

        [DataTestMethod]
        [DataRow(DataKind.Heat, "PowerCycle Load 0 49.99998 *")]
        [DataRow(DataKind.Heat, "0.001 Load 0 49.99998 *")]
        [DataRow(DataKind.Heat, "ARC;4;100;25 Load 0 49.99998 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]
        [DataRow(DataKind.Med, "air Коэф.теплоотдачи.воздух 20 0 1500 *")]
        [DataRow(DataKind.Med, "air * 25 0 1500 *")]
        [DataRow(DataKind.Med, "air * Термоцикл 0 1500 *")]
        [DataRow(DataKind.Med, "air 0.002 Термоцикл 0 1500 *")]
        [DataRow(DataKind.Med, "air Коэф.теплоотдачи.воздух 20.01 0 1500 *")]
        [DataRow(DataKind.Med, "Элементы3D_2 Коэф.теплоотдачи.воздух 20 0 1 *")]
        [DataRow(DataKind.Clamp, "Узлы_7 Жесткое X * 0 1500 *")]
        [DataRow(DataKind.Clamp, "Узлы_7 Жесткое None * 0 1500 *")]
        [DataRow(DataKind.Clamp, "Узлы_7 Жесткое X Функция 0 1500 *")]
        [DataRow(DataKind.Clamp, "Узлы_7 Жесткое Y * 0 1500 *")]
        [DataRow(DataKind.Clamp, "Узлы_7 Жесткое Z * 0 1500 *")]
        [DataRow(DataKind.Load, "Узлы_7 Сила None 0 * 0 1500 *")]
        [DataRow(DataKind.Load, "Узлы_7 Сила Z 0 * 0 1500 *")]
        [DataRow(DataKind.Load, "Узлы_7 Сила X 0.0001 loadCycle 0 1500 *")]
        [DataRow(DataKind.Mat, "Элементы3D_3 Сталь_20ХМ 0 1500 *")]
        [DataRow(DataKind.Heat, "FSWPin;100;12;12;8;120 load 0 1 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]
        [DataRow(DataKind.Heat, "FSWShoulder;100;100;30;12;12;120 load 0 1 baseLine|refLine;2;startNodes;startNodes;0|0|0|0")]
        public void TestCreateTaskData(DataKind dataKind,  string dataString)
        {
            //assign
            var project = new ProjectData("","");
            var dataAr = dataString.Split(' ');
            Data data;
            switch (dataKind)
            {
                case DataKind.Mat:
                    data = new MatData(new Group("", ObjType.Элемент3D), dataString) 
                    { Group = new Group(dataAr[0], ObjType.Элемент3D) };
                    break;
                case DataKind.Med:
                    data = new MediaData(new Group("", ObjType.Элемент2D), dataString)
                    { Group = new Group(dataAr[0], ObjType.Элемент2D) };
                    break;
                case DataKind.Heat:
                    var heatData = new HeatData(new Group("", ObjType.Элемент3D), dataString)
                    { Group = new Group(dataAr[1], ObjType.Элемент3D) };
                    var baseLine = new Group("baseLine", ObjType.Узел);
                    var refLine = new Group("refLine", ObjType.Узел);
                    var startNodes = new Group("startNodes", ObjType.Узел);

                    if (heatData.MovedFrame != null)
                    {
                        heatData.MovedFrame.BaseLine = baseLine;
                        heatData.MovedFrame.RefLine = refLine;
                        heatData.MovedFrame.StartPoints = startNodes;
                        heatData.MovedFrame.StopPoints = startNodes;
                    }

                    data = heatData;
                    break;
                case DataKind.Clamp:
                    data = new ClampData(new Group("", ObjType.Узел), dataString)
                    { Group = new Group(dataAr[0], ObjType.Узел) };
                    break;
                default:
                    data = new LoadData(new Group("", ObjType.Узел), dataString)
                    { Group = new Group(dataAr[0], ObjType.Узел) };
                    break;
            }
            //assert

            Assert.AreEqual(data.GetInfo, dataString);

        }

        [DataTestMethod]
        [DataRow("air Коэф.теплоотдачи.воздух 20.005 0 1500 *", MediaType.HeatFlux, "20.005")]
        [DataRow("air * Термоцикл 0 1500 *", MediaType.ConstantTemp, "Термоцикл")]
        [DataRow("air Коэф.теплоотдачи.воздух 20 0 1500 *", MediaType.HeatFlux, "20")]
        [DataRow("air * Термоцикл 0 1500 *", MediaType.ConstantTemp, "Термоцикл")]
        [DataRow("air 0.002 Термоцикл 0 1500 *", MediaType.HeatFlux, "Термоцикл")]
        [DataRow("air Коэф.теплоотдачи.воздух 20 0 1500.001 *", MediaType.HeatFlux, "20")]
        [DataRow("air 0.00001 19.9999 0 1500.001 *", MediaType.HeatFlux, "19.9999")]

        public void CheckMediaData(string dataString, MediaType mediaType, string tempVal)
        {
            //assign

            var data = new MediaData(new Group("", ObjType.Элемент2D), dataString);

            //assert

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
