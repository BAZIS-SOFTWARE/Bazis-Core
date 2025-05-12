using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using PreProc;
using Model.GroupsData;
using Project;
using Model;
using Project.Interfaces.Tasks;
using Project.Tasks.IO;
using Project.Tasks;
using Model.Interfaces;
using PreProc.Interfaces;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace TestProject
{
    [TestClass]
    public class TestPreProc
    {
        
        private static string projPath =  Path.GetFullPath(@".\..\..\..\..\TestProject\CalcCompDataTest\");

        /// <summary>
        /// Создать ValuableDat, для параметризации тестов
        /// </summary>
        /// <param name="typeAndArgs">Массив строк, которые состоят из типа задачи и аргументов, разделенных подстрокой "/"</param>
        /// <returns>Ленивая коллекция из IValuableData, представляющая данные по задачам</returns>
        private static IEnumerable<IPhysicalData> CreateValuableData(string[] typeAndArgs)
        {
            foreach (var str in typeAndArgs.Select(x => x.Split('/')))
            {
                Func<Group, string,IPhysicalData > func;
                switch (str[0].ToLower())
                {
                    case "media": func = (g, s) => new MediaData(g, s); break;
                    case "heat": func = (g, s) => new HeatData(g, s); break;
                    case "clamp": func = (g, s) => new ClampData(g, s); break;
                    case "load": func = (g, s) => new LoadData(g, s); break;
                    default: func = (g, s) => new MatData(g, s); break;
                }
                yield return func(new Group("", ObjType.Узел), str[1]);
            }
        }

        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [TestMethod]
    [DataRow(
        new string[]
        {
            "путь\\механическая_1_0_3.tsf",
            "путь\\механическая_2_3_1500.tsf",
            "путь\\термическая_1_0_3.tsf",
            "путь\\термическая_2_3_1500.tsf"
        },
        new string[]
        {
            "термическая путь\\термическая_1_0_3.tsf выполнить",
            "механическая путь\\механическая_1_0_3.tsf выполнить",
            "термическая путь\\термическая_2_3_1500.tsf выполнить",
            "механическая путь\\механическая_2_3_1500.tsf выполнить"
        }
        )]
    [DataRow(
            new string[]
        {
            "путь\\термическая_1_0_7.05826.tsf",
            "путь\\термическая_2_7.05826_10.tsf",
            "путь\\термическая_3_10_15.57324.tsf",
            "путь\\механическая_1_0_7.05826.tsf",
            "путь\\механическая_2_7.05826_10.tsf",
            "путь\\механическая_3_10_15.57324.tsf"
        },
                        new string[]
        {
            "термическая путь\\термическая_1_0_7.05826.tsf выполнить",
            "механическая путь\\механическая_1_0_7.05826.tsf выполнить",
            "термическая путь\\термическая_2_7.05826_10.tsf выполнить",
            "механическая путь\\механическая_2_7.05826_10.tsf выполнить",
            "термическая путь\\термическая_3_10_15.57324.tsf выполнить",
            "механическая путь\\механическая_3_10_15.57324.tsf выполнить"
        }
        )]
        [DataRow(
            new string[]
        {
            "c:путь_test\\механическая_1_0_20.12732.tsf",
            "c:путь_test\\механическая_2_0_20.12732.tsf",
            "c:путь_test\\механическая_3_0_20.12732.tsf",
            "c:путь_test\\термическая_1_0_20.12732.tsf",
            "c:путь_test\\термическая_2_0_20.12732.tsf",
            "c:путь_test\\термическая_3_0_20.12732.tsf"
        },
                        new string[]
        {
            "термическая c:путь_test\\термическая_1_0_20.12732.tsf выполнить",
            "механическая c:путь_test\\механическая_1_0_20.12732.tsf выполнить",
            "термическая c:путь_test\\термическая_2_0_20.12732.tsf выполнить",
            "механическая c:путь_test\\механическая_2_0_20.12732.tsf выполнить",
            "термическая c:путь_test\\термическая_3_0_20.12732.tsf выполнить",
            "механическая c:путь_test\\механическая_3_0_20.12732.tsf выполнить"
        }
        )]
        [DataRow(
            new string[] 
        {
            "путь\\механическая_1_0_20.12732.tsf",
            "путь\\механическая_2_0_20.12732.tsf",
            "путь\\механическая_3_0_20.12732.tsf",
            "путь\\термическая_1_0_20.12732.tsf",
            "путь\\термическая_2_0_20.12732.tsf",
            "путь\\термическая_3_0_20.12732.tsf"
        },
                        new string[]
        {
            "термическая путь\\термическая_1_0_20.12732.tsf выполнить",
            "механическая путь\\механическая_1_0_20.12732.tsf выполнить",
            "термическая путь\\термическая_2_0_20.12732.tsf выполнить",
            "механическая путь\\механическая_2_0_20.12732.tsf выполнить",
            "термическая путь\\термическая_3_0_20.12732.tsf выполнить",
            "механическая путь\\механическая_3_0_20.12732.tsf выполнить"
        }
        )]
        [DataRow(
            new string[]
        {
            "путь\\механическая_1_0.001_0.12732.tsf",
            "путь\\механическая_2_0.12732_20.12732.tsf",
            "путь\\механическая_3_20.12732_21.12732.tsf",
            "путь\\термическая_1_0.001_0.12732.tsf",
            "путь\\термическая_2_0.12732_20.12732.tsf",
            "путь\\термическая_3_20.12732_21.12732.tsf"
        },
                        new string[]
        {
            "термическая путь\\термическая_1_0.001_0.12732.tsf выполнить",
            "механическая путь\\механическая_1_0.001_0.12732.tsf выполнить",
            "термическая путь\\термическая_2_0.12732_20.12732.tsf выполнить",
            "механическая путь\\механическая_2_0.12732_20.12732.tsf выполнить",
            "термическая путь\\термическая_3_20.12732_21.12732.tsf выполнить",
            "механическая путь\\механическая_3_20.12732_21.12732.tsf выполнить"
        }
        )]
        [DataRow(
            new string[]
        {
            "путь\\механическая_10_0.001_0.12732.tsf",
            "путь\\механическая_20_0.12732_20.12732.tsf",
            "путь\\механическая_30_20.12732_21.12732.tsf",
            "путь\\термическая_10_0.001_0.12732.tsf",
            "путь\\термическая_20_0.12732_20.12732.tsf",
            "путь\\термическая_30_20.12732_21.12732.tsf"
        },
                        new string[]
        {
            "термическая путь\\термическая_10_0.001_0.12732.tsf выполнить",
            "механическая путь\\механическая_10_0.001_0.12732.tsf выполнить",
            "термическая путь\\термическая_20_0.12732_20.12732.tsf выполнить",
            "механическая путь\\механическая_20_0.12732_20.12732.tsf выполнить",
            "термическая путь\\термическая_30_20.12732_21.12732.tsf выполнить",
            "механическая путь\\механическая_30_20.12732_21.12732.tsf выполнить"
        }
        )]
        public void SortingByTimeAndTypeCheck(string[] strsIn, string[]strsOut)
        {
            var preProc = new PreProc.PreProc();

            var results = preProc.SortCompDataByTimeAndType(strsIn);

            for (int i = 0; i < results.Count; i++)
                Assert.AreEqual(results[i], strsOut[i]);
        }

        [TestMethod]
        [DataRow(new[] { "путь\\термическая_^3_20.12732_21.12732.tsf" })]
        [DataRow(new[] { "путь\\%термическая_2_20.12732_21.12732.tsf" })]
        [DataRow(new[] { "путь\\!_2_20.12732_21.12732.tsf" })]
        [DataRow(new[] { "путь\\термическая_7_20.12732_#.12732.tsf" })]
        [DataRow(new[] { "путь\\термическая_6_20.12?32_21.12732.tsf" })]
        [DataRow(new[] { "путь\\термическая_5_20.1232_21.127()2.tsf" })]
        [DataRow(new[] { "путь\\термическая_4_20.1232_21.12732.ts-f" })]
        [DataRow(new[] { "путь\\термическая_3_20.1232_21=.12732.tsf" })]
        [DataRow(new[] { "путь\\@_3_20.12732_21.12732.tsf" })]
        [DataRow(new[] { "путь\\механическая_3_{}}.12732_21.12732.tsf" })]
        [DataRow(new[] { "путь\\механическая_3_20.12732_21.127[].tsf" })]
        [DataRow(new[] { "путь\\механическая_3_20.12732_21.12;;.tsf" })]
        [DataRow(new[] { "путь\\type_int_float_float.tsf" })]
        public void SortingByTimeAndTypeCheck_ShouldFail_WhenContainsIlligalSymbol(string[] strsIn)
        {
            var preProc = new PreProc.PreProc();
            Assert.ThrowsException<Exception>( () => preProc.SortCompDataByTimeAndType(strsIn));
        }

        [TestMethod]
        [DataRow(new string[]
        {
            "heat/1 * * Load 0 49 baseLine|refLine;2;0|0|0|0",
            "media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
            "clamp/Узел_8 Жесткое X 0 * 0 1500 *"
        })]
        public void BaseCheck(string[] parameters)
        {
            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(CreateValuableData(parameters).ToList(), ProcessType.Welding, projPath + "TsfFolder\\");
            preProc.CalcCompDataV2(CreateValuableData(parameters).ToList(), new ProcessProperty { GeneralTaskType = ComplexTaskType.термическая_механическая, CommonTaskType = ProcessType.Welding }, projPath);
        }

        [TestMethod]
        [DataRow(new string[]
        {
            "mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
            "media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "media/air Коэф.теплоотдачи.воздух 20 0 3 *",
            "media/air Коэф.теплоотдачи.воздух 20 103 106 *",
            "media/air Коэф.теплоотдачи.воздух 20 206 209 *",
            "clamp/Узел_8 Жесткое X 0 * 0 1500 *"
        })]
        [DataRow(new string[]
        {
            "mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
            "media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "media/air Коэф.теплоотдачи.воздух 20 0 3 *",
            "media/air Коэф.теплоотдачи.воздух 20 103 106 *",
            "media/air Коэф.теплоотдачи.воздух 20 206 209 *",
            "clamp/Узел_8 Жесткое X 0 * 0 1500 *",
            "clamp/Узел_8 Жесткое X 0 * 0 3 *",
            "clamp/Узел_8 Жесткое X 0 * 103 106 *",
            "clamp/Узел_8 Жесткое X 0 * 206 209 *",
        })]
        [DataRow(new string[]
        {
            "mat/Элементы3D_3 Сталь_20ХМ 0 100 *",
            "media/air Коэф.теплоотдачи.воздух 20 0 100 *",
            "media/air Коэф.теплоотдачи.воздух 20 0 5 *",
            "media/air Коэф.теплоотдачи.воздух 20 15 20 *",
            "media/air Коэф.теплоотдачи.воздух 20 25 30 *",
            "clamp/Узел_8 Жесткое X 0 * 0 100 *"
        })]
        [DataRow(new string[]
        {
            "mat/Элементы3D_3 Сталь_20ХМ 0 100 *",
            "media/air Коэф.теплоотдачи.воздух 20 0 100 *",
            "media/air Коэф.теплоотдачи.воздух 20 0 5 *",
            "media/air Коэф.теплоотдачи.воздух 20 15 20 *",
            "media/air Коэф.теплоотдачи.воздух 20 25 30 *",
            "clamp/Узел_8 Жесткое X 0 * 0 100 *",
            "clamp/Узел_8 Жесткое X 0 * 0 5 *",
            "clamp/Узел_8 Жесткое X 0 * 15 20 *",
            "clamp/Узел_8 Жесткое X 0 * 25 30 *",
        })]
        public void CalcCompData_ShouldPass_WhenMainBoundDataBoundsMinors(string[] parameters)
        {
            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(CreateValuableData(parameters).ToList(), ProcessType.Welding, projPath + "TsfFolder\\");
            preProc.CalcCompDataV2(CreateValuableData(parameters).ToList(), new ProcessProperty { GeneralTaskType = ComplexTaskType.термическая_механическая, CommonTaskType = ProcessType.Welding }, projPath);
        }

        [TestMethod]
        [DataRow(0, 120, 1, 1500, 0)]
        [DataRow(0, 50.0001f, 50.001f, 100, 0)]
        public void CalcCompData_Fails_WhenHeatDataStartEarlierThenMedDataStop
            (float heatStart, float heatStop, float medStart, float medStop, ProcessType pt)
        {
            var heatfileName = $"термическая термическая{1}{heatStart}_{heatStop}.tsf выполнить";
            var medfileName = $"термическая термическая{2}{medStart}_{medStop}.tsf выполнить";

            var valuableData = CreateValuableData(new[]
            {
                $"media/air Коэф.теплоотдачи.воздух 20 {medStart} {medStop} *",
                $"heat/1 * * Load {heatStart} {heatStop} baseLine|refLine;2;0|0|0|0",
                "mat/Элементы3D_3 Сталь_20ХМ 0 1500 *"
            }).ToList();
            
            var preProc = new PreProc.PreProc();
            Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV1(valuableData, pt, projPath + "TsfFolder\\"));
            Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV2(valuableData, new ProcessProperty { GeneralTaskType = ComplexTaskType.термическая_механическая, CommonTaskType = pt }, projPath));
        }

        [TestMethod]
        [DataRow(new[]
        {
            "heat/1 * * Load 0 50 baseLine|refLine;2;0|0|0|0",
            "media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
            "clamp/Узел_8 Жесткое X 0 * 0 1500 *"
        })]
        public void CalcCompData_ReturnsTrue_WhenFileTSFSuccesfullyCreated(string[] parameters)
        {
            var heatFileName = $"термическая термическая10_50.tsf выполнить";
            var medFileName = $"термическая термическая20_1500.tsf выполнить";
            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(CreateValuableData(parameters).ToList(), ProcessType.Welding, projPath + "TsfFolder\\");
            preProc.CalcCompDataV2(CreateValuableData(parameters).ToList(), new ProcessProperty { GeneralTaskType = ComplexTaskType.термическая_механическая, CommonTaskType = ProcessType.Welding }, projPath);

            var projDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var heatLocation = Path.GetFullPath(heatFileName);
            var medLocation = Path.GetFullPath(medFileName);

            Assert.AreEqual($"{projDir}\\{heatFileName}", heatLocation);
            Assert.AreEqual($"{projDir}\\{medFileName}", medLocation);
        }

        [TestMethod]
        [DataRow(new[]
        {
            "mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
            "media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "heat/1 * * Load 0 49.998 baseLine|refLine;2;0|0|0|0",
            "clamp/Узел_8 Жесткое X 0 * 0 1500 *"
        })]
        public void CalcCompData_ReturnsTrue_WhenMaterialContinousWholeProcess(string[] parameters)
        {
            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(CreateValuableData(parameters).ToList(), ProcessType.Welding, projPath + "TsfFolder\\");
            preProc.CalcCompDataV2(CreateValuableData(parameters).ToList(), new ProcessProperty { GeneralTaskType = ComplexTaskType.термическая_механическая, CommonTaskType = ProcessType.Welding }, projPath);
        }

        [TestMethod]
        [DataRow(@"MNOGOPROHOD.bpf", ProcessType.Welding)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Heating)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Quenching)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Tempering)]
        //[DataRow(@".\..\..\CalcCompDataTest\1.bpf", ProcessType.Welding)]
        //[DataRow(@".\..\..\CalcCompDataTest\6.bpf", ProcessType.Welding)]
        public void PreProc_GenerateTSF_ShouldPass(string file, ProcessType pt)
        {
            var project = new ProjectData();

            project.ModelData = new ModelData();
            project.TaskData = new TaskData();

            project.Load(projPath + file);

            var resData = new List<IPhysicalData>();

            foreach (var data in project.TaskData)
                resData.Add(data as PhysicalData);

            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(resData, pt, projPath + "TsfFolder\\");
            preProc.CalcCompDataV2(resData, new ProcessProperty { GeneralTaskType = ComplexTaskType.термическая_механическая, CommonTaskType = pt }, projPath + "TsfFolder\\");
        }

        [TestMethod]
        [DataRow(@"MatDataShorterThenBoundData.bpf", DisplayName = "MatData_Shorter_Then_BoundData")]
        [DataRow(@"NoMaterialData.bpf", DisplayName = "No_MaterialData")]
        [DataRow(@"NoBoundData.bpf", DisplayName = "No_BoundData")]
        [DataRow(@"ClampDataStartsAfterProcessStart.bpf", DisplayName = "ClampData_Starts_After_Process_Start")]
        [DataRow(@"MedDataStartsAfterProcessStart.bpf", DisplayName = "MedData_Starts_After_Process_Start")]
        [DataRow(@"MaterialDataStartsAfterProcessStart.bpf")]
        [DataRow(@"FreeSpacesBetweenClampData.bpf")]
        [DataRow(@"FreeSpacesBetweenMediaData.bpf")]
        [DataRow(@"FreeSpacesBetweenMaterialData.bpf")]
        [DataRow(@"HeatDataContiniousWithoutMediaData.bpf")]
        public void PreProc_GenerateTSF_ShouldFail(string file)
        {
            var resData = new List<IPhysicalData>();
            var group = new Group("test",ObjType.Узел);
            var groupData = new GroupData();
            groupData.Add(group);
            var textData = new LoadTaskDataTextFormat().Load(projPath + file, groupData);
            foreach (var data in textData)
            {
                resData.Add(data as PhysicalData);
            }
            var preProc = new PreProc.PreProc();
            Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV1(resData, ProcessType.Welding, projPath));
            Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV2(resData, new ProcessProperty { GeneralTaskType = ComplexTaskType.термическая_механическая, CommonTaskType = ProcessType.Welding }, projPath));
        }

        [TestMethod]
        [DataRow(new[]
        {
            "media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "heat/1 * * Элемент3D_2 0 95.08694 Узел_5|Узел_6;2;0|0|0|0",
            "clamp/Узел_8 Жесткое X 0 * 0 1500 *",
            "mat/mat Сталь_20ХМ 0 1500 *"
        })]
        public void FastCheck(string[] parameters)
        {
            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(CreateValuableData(parameters).ToList(), ProcessType.Welding, projPath);
            preProc.CalcCompDataV2(CreateValuableData(parameters).ToList(), new ProcessProperty { GeneralTaskType = ComplexTaskType.термическая_механическая, CommonTaskType = ProcessType.Welding }, projPath);
        }

        [DataRow(ComplexTaskType.термическая, ProcessType.Welding, new[]
        {
            "Media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "Heat/1 * * Элемент3D_2 0 95.08694 Узел_5|Узел_6;2;0|0|0|0",
            "Mat/mat Сталь_20ХМ 0 1500 *"
        })]
        [DataRow(ComplexTaskType.термическая, ProcessType.Tempering, new[]
        {
            "Media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "Mat/mat Сталь_20ХМ 0 1500 *"
        })]
        [DataRow(ComplexTaskType.механическая, ProcessType.Welding, new[]
        {
            "Media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "Mat/mat Сталь_20ХМ 0 1500 *",
            "Clamp/Узел_8 Жесткое X 0 * 0 1500 *"
        })]
        [TestMethod]
        public void CalcCompDataV2_ShouldPass_OnSingleProcessTaskCreation(ComplexTaskType generalTaskType, ProcessType commonTaskType, string[] valuableDataStrings)
        {
            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV2(CreateValuableData(valuableDataStrings).ToList(), new ProcessProperty { GeneralTaskType = generalTaskType, CommonTaskType = commonTaskType }, projPath);
        }


        [DataRow(ComplexTaskType.термическая_механическая, ProcessType.Welding, new[]
        {
            "Media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "Heat/1 * * Элемент3D_2 0 95.08694 Узел_5|Узел_6;2;0|0|0|0",
            "Mat/mat Сталь_20ХМ 0 1500 *",
            "Clamp/Узел_8 Жесткое X 0 * 0 1500 *"
        })]
        [TestMethod]
        public void CalcCompDataV2_ShouldPass_OnComplexProcessTaskCreation(ComplexTaskType generalTaskType, ProcessType commonTaskType, string[] valuableDataStrings)
        {
            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV2(CreateValuableData(valuableDataStrings).ToList(), new ProcessProperty { GeneralTaskType = generalTaskType, CommonTaskType = commonTaskType }, projPath);
        }

        [TestMethod]
        [DataRow(ComplexTaskType.термическая_механическая, ProcessType.Welding, new[]
        {
            "Mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
            "Media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
            "Clamp/Узлы_7 Жесткое X 0 * 0 1500 *",
            "Clamp/Узлы_7 Жесткое Y 0 * 0 1500 *",
            "Clamp/Узлы_7 Жесткое Z 0 * 0 1500 *",
            "Heat/1 * * Load 0 50 baseLine|refLine;2;0|0|0|0"
        })]
        [DataRow(ComplexTaskType.термическая, ProcessType.Welding, new[]
        {
            "Mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
            "Media/air Коэф.теплоотдачи.воздух 20 0 1500 *"
        })]
        [DataRow(ComplexTaskType.механическая, ProcessType.Welding, new[]
        {
            "Mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
            "Clamp/Узлы_7 Жесткое X 0 * 0 1500 *",
        })]
        public void CalcCompDataV2_ShouldPass_WithoutWeldingTSF(ComplexTaskType generalTaskType, ProcessType commonTaskType, string[] valuableDataStrings)
        {
            var preProc = new PreProc.PreProc();
            var path = Path.GetFullPath(projPath) + "\\TsfFolder\\\\V2TestCase";
            preProc.CalcCompDataV2(CreateValuableData(valuableDataStrings).ToList(), new ProcessProperty { GeneralTaskType = generalTaskType, CommonTaskType = commonTaskType }, path);
            preProc.SortCompDataByTimeAndType(Directory.GetFiles(path));
        }
    }
}
