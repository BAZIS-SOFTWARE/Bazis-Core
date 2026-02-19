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
using Model.IO;
using MaterialDB.MaterialData;
using MaterialDB.FunctionData;
using System.Security.Cryptography;
using System.Xml.Linq;
using Project.IO;

namespace TestProject
{
    [TestClass]
    public class TestTaskData
    {
        string testDir = @"..\..\..\Examples";
        MaterialDBData materials;
        FunctionDBData functions;

        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var settingsSerializer = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            materials = JsonConvert.DeserializeObject<MaterialDBData>
                (File.ReadAllText($@"{testDir}\materials.jsf"), settingsSerializer);
            functions = JsonConvert.DeserializeObject<FunctionDBData>
    (File.ReadAllText($@"{testDir}\functions.jsf"), settingsSerializer);
        }
        [TestMethod]
        public void RemoveTaskDataByInfoString()
        {
            //assign
            var loader = new LoadProjectFromTextFormat();

            //act
            var proj = loader.Load($@"{testDir}\proj_traj.bpf");

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
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load($@"{testDir}\proj_traj.bpf");
            //act

            proj.ModelData.GroupData.RemoveAt(5);
            proj.TaskData.ClearNotExisted(proj.ModelData.GroupData);

            //assert
            Assert.AreEqual(proj.TaskData.Count, 5);

        }

        [TestMethod]
        public void FindTaskDataByType()
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

            //act
            var res = proj.TaskData.Find<ICondData>();
            //assert

            Assert.AreEqual(res.Count(), 6);

        }

        [TestMethod]
        [DataRow(@"..\..\..\Examples\proj_traj.bpf", DisplayName = "proj_traj")]
        //[DataRow(@"..\..\..\Examples\proj_traj_baseLine_not_sorted.bpf", DisplayName = "proj_traj_baseLine_not_sorted")]

        //[DataRow(@"c:\projs\Roman\Proba\777.bpf2")]
        //[DataRow(@"c:\projs\kamaz\4.bpf")]
        public void TestFrameFunctionInTime(string relativeProjPath)
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(relativeProjPath);
            var ext = Path.GetExtension(relativeProjPath);

            //act
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
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

            //act
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
        [DataRow(10.1f, 10.2f, 10.3f,1,0.5f,0)]
        [DataRow(0, 0, 0 ,0 ,0, 0)]
        [DataRow(-1f, -1f, -1f, -1f, -1f, -1f)]
        [DataRow(1f, 1f, 1f, 1f, 1f, 1f)]
        [DataRow(0.001f, 0.001f, 0.001f, 0.001f, 0.001f, 0.001f)]
        public void TestFrameFunctionTransformation(float rotX, float rotY, float rotZ, float x,float y,float z)
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

            //act
            var heatData = proj.TaskData.Find(DataKind.Нагрев).Select(h => (HeatData)h).First();
            heatData.FrameFunction.LocalFrame.Rotation_X = rotX;
            heatData.FrameFunction.LocalFrame.Rotation_Y = rotY;
            heatData.FrameFunction.LocalFrame.Rotation_Z = rotZ;
            heatData.FrameFunction.LocalFrame.Shifting = new Point3D(x,y,z);
            //assert
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Rotation_X, rotX);
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Rotation_Y, rotY);
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Rotation_Z, rotZ);
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Shifting, new Point3D(x,y,z));
        }

        [DataTestMethod]
        [DataRow(10.1f, 10.2f, 10.3f, 1, 0.5f, 0)]
        [DataRow(0, 0, 0, 0, 0, 0)]
        [DataRow(-1f, -1f, -1f, -1f, -1f, -1f)]
        [DataRow(1f, 1f, 1f, 1f, 1f, 1f)]
        [DataRow(0.001f, 0.001f, 0.001f, 0.001f, 0.001f, 0.001f)]
        [DataRow(-0.001f, 0.001f, -0.001f, 0.001f, -0.001f, 0.001f)]
        [DataRow(10000000, 10000000, 10000000, 10000000, 10000000, 10000000)]
        public void TestFrameFunctionStringTransformation(float rotX, float rotY, float rotZ, float x, float y, float z)
        {
            var str = $"{x}|{y}|{z}|{rotX}|{rotY}|{rotZ}";
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

            //act
            var heatData = proj.TaskData.Find(DataKind.Нагрев).Select(h => (HeatData)h).First();
            heatData.FrameFunction.LocalFrame.SetShiftingAndRotationV2(str);

            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Rotation_X, rotX);
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Rotation_Y, rotY);
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Rotation_Z, rotZ);
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Shifting, new Point3D(x, y, z));
        }

        [DataTestMethod]
        [DataRow("--1|2|3|4|5|6")]
        [DataRow("@|#|%|=|+|_")]
        [DataRow("11f|22f|33f|44f|55f|66f")]
        [DataRow("a|b|c|d|e|f")]
        public void TestFrameFunctionOnInvalidString(string fail)
        {
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

            //act
            var heatData = proj.TaskData.Find(DataKind.Нагрев).Select(h => (HeatData)h).First();
            Assert.ThrowsException<Exception>(() => heatData.FrameFunction.LocalFrame.SetShiftingAndRotationV2(fail));
        }

        [DataTestMethod]
        [DataRow(10.3f, 1, 0.5f, 0.1f)]
        [DataRow(1, 1, 1, 1)]
        [DataRow(-1f, -1f, -1f, -1f)]
        public void TestFrameFunctionOnOldStringFormat(float x, float y, float z, float angle)
        {
            var str = $"{x}|{y}|{z}|{angle}";
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

            //act
            var heatData = proj.TaskData.Find(DataKind.Нагрев).Select(h => (HeatData)h).First();
            heatData.FrameFunction.LocalFrame.SetShiftingAndRotationV2(str);

            Assert.AreNotEqual(heatData.FrameFunction.LocalFrame.Rotation_X, angle);
            Assert.AreNotEqual(heatData.FrameFunction.LocalFrame.Shifting, new Point3D(x, y, z));
        }

        [DataTestMethod]
        public void TestFrameFunctionOnOldFileStringFormat()
        {
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\proj_traj_old_LocalFrame.bpf");

            //act
            var heatData = proj.TaskData.Find(DataKind.Нагрев).Select(h => (HeatData)h).First();
            var str = heatData.FrameFunction.LocalFrame.ToString().Split(";").Last();
            var parts = str.Split('|');

            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Shifting, new Point3D(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2])));
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Rotation_X, float.Parse(parts[3]));
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Rotation_Y, float.Parse(parts[4]));
            Assert.AreEqual(heatData.FrameFunction.LocalFrame.Rotation_Z, float.Parse(parts[5]));
        }

        [DataTestMethod]
        [DataRow("newGroupЭлемент1D стержни 10 Сталь_W_S420 0 1 *")]
        [DataRow("newGroupЭлемент1D пластины 0.5 Сталь_Р18 0 1 *")]
        [DataRow("newGroupЭлемент1D пластины 15 Сталь_W_S420 0 1 *")]
        [DataRow("newGroupЭлемент3D Сталь_Р18 0 1 *")]
        [DataRow("newGroupЭлемент3D Сталь_20ХМ 3 5 *")]

        //[DataRow("newGroupЭлемент3D Сталь_20ХМ 5 1 *")]
        //[DataRow("newGroupЭлемент3D Сталь_20ХМ  1500 *")]
        //[DataRow("newGroupЭлемент3D Сталь_10 * * *")]
        //[DataRow("newGroupЭлемент3D Сталь_20ХМ 3 5 ISORAPID.277.HM")]

        public void TestMaterialDataCreator(string info)
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"..\..\..\Examples\MixedMesh\createMat.bpf");
            //act
            var dataKind = DataKind.Материал;
            var material = proj.TaskData.Create(dataKind, info, proj.ModelData.GroupData);
            //assert
            Assert.AreEqual($"{dataKind} : {info}", material.ToString());
        }

        [DataTestMethod]
        [DataRow(DataKind.Нагрев)]
        [DataRow(DataKind.Среда)]
        [DataRow(DataKind.Закрепление)]
        [DataRow(DataKind.Материал)]
        public void TestFindTaskData(DataKind datakind)
        {
            //assign
            var loader = new LoadProjectFromTextFormat();

            //act
            var project = loader.Load(@"..\..\..\Examples\proj_traj.bpf");

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
        [DataRow("1 Heating * Load 0 49.99998", DataKind.Нагрев)]
        [DataRow("0.001 * * Load 0 49.99998", DataKind.Нагрев)]
        [DataRow("1 Heating * Load 0.0001 49", DataKind.Нагрев)]
        [DataRow("Load Жесткое X 0.255 * 0 1500 *", DataKind.Закрепление)]
        [DataRow("Load Жесткое None 0.255 * 0 1500 *", DataKind.Закрепление)]
        [DataRow("Load Жесткое X 1 Heating 0 1500 *", DataKind.Закрепление)]
        [DataRow("Load Жесткое Y 0 * 0 1500 *", DataKind.Закрепление)]
        [DataRow("Load Жесткое Z 0 * 0 1500 *", DataKind.Закрепление)]
        [DataRow("Load Сила None 0 * 0 1500 *", DataKind.Нагрузка)]
        [DataRow("Load Сила Z 0 * 0 1500 *", DataKind.Нагрузка)]
        [DataRow("Load Сила X 0.0001 Heating 0 1500 *", DataKind.Нагрузка)]
        [DataRow("Load Коэф.теплоотдачи.воздух 20.005 0 1500 *", DataKind.Среда)]
        [DataRow("Load * Термоцикл 0 1500 *", DataKind.Среда)]
        [DataRow("Load Коэф.теплоотдачи.воздух 20 0 1500 *", DataKind.Среда)]
        [DataRow("Load 0.002 Heating 0 1500 *", DataKind.Среда)]
        [DataRow("Load Коэф.теплоотдачи.воздух 20 0 1500.001 *", DataKind.Среда)]
        [DataRow("Load 1E-05 19.9999 0 1500.001 *", DataKind.Среда)]
        [DataRow("Load * 19.9999 0 1500.001 *", DataKind.Среда)]


        public void CheckTaskData(string dataString, DataKind dataKind)
        {
            //assign
            var taskData = new TaskData();
            taskData.Materials = materials;
            taskData.Functions = functions;

            var grData = new GroupData();

            grData.Add(new Group("Load", ObjType.Узел));
            var condData = taskData.Create(dataKind, dataString, grData);

            //assert
            Assert.AreEqual($"{dataKind} : {dataString}", condData.ToString());

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
