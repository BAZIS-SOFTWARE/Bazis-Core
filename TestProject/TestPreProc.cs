using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using PreProc;
using Model.GroupsData;
using Project.Interfaces.Tasks;
using Project.Tasks.IO;
using Project.Tasks;
using Model.Interfaces;
using PreProc.Interfaces;
using Model.MeshObjects;
using Project.IO;

namespace TestProject
{
    [TestClass]
    public class TestPreProc
    {
        
        private static string projPath =  Path.GetFullPath(@".\..\..\..\..\TestProject\CalcCompDataTest\");

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



        //[TestMethod]
        //[DataRow(new string[]
        //{
        //    "mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 0 3 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 103 106 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 206 209 *",
        //    "clamp/Узел_8 Жесткое X 0 * 0 1500 *"
        //})]
        //[DataRow(new string[]
        //{
        //    "mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 0 3 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 103 106 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 206 209 *",
        //    "clamp/Узел_8 Жесткое X 0 * 0 1500 *",
        //    "clamp/Узел_8 Жесткое X 0 * 0 3 *",
        //    "clamp/Узел_8 Жесткое X 0 * 103 106 *",
        //    "clamp/Узел_8 Жесткое X 0 * 206 209 *",
        //})]
        //[DataRow(new string[]
        //{
        //    "mat/Элементы3D_3 Сталь_20ХМ 0 100 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 0 100 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 0 5 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 15 20 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 25 30 *",
        //    "clamp/Узел_8 Жесткое X 0 * 0 100 *"
        //})]
        //[DataRow(new string[]
        //{
        //    "mat/Элементы3D_3 Сталь_20ХМ 0 100 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 0 100 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 0 5 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 15 20 *",
        //    "media/air Коэф.теплоотдачи.воздух 20 25 30 *",
        //    "clamp/Узел_8 Жесткое X 0 * 0 100 *",
        //    "clamp/Узел_8 Жесткое X 0 * 0 5 *",
        //    "clamp/Узел_8 Жесткое X 0 * 15 20 *",
        //    "clamp/Узел_8 Жесткое X 0 * 25 30 *",
        //})]
        //public void CalcCompData_ShouldPass_WhenMainBoundDataBoundsMinors(string[] parameters)
        //{
        //    var preProc = new PreProc.PreProc();
        //    preProc.CalcCompDataV1(CreatePhysicalData(parameters).ToList(), ProcessType.Welding, projPath + "TsfFolder\\");
        //    preProc.CalcCompDataV2(CreatePhysicalData(parameters).ToList(), new ProcessProperty { TaskKind = TaskKind.термическая_механическая, CommonTaskType = ProcessType.Welding }, projPath);
        //}

        //[TestMethod]
        //[DataRow(0, 120, 1, 1500, 0)]
        //[DataRow(0, 50.0001f, 50.001f, 100, 0)]
        //public void CalcCompData_Fails_WhenHeatDataStartEarlierThenMedDataStop
        //    (float heatStart, float heatStop, float medStart, float medStop, ProcessType pt)
        //{
        //    var heatfileName = $"термическая термическая{1}{heatStart}_{heatStop}.tsf выполнить";
        //    var medfileName = $"термическая термическая{2}{medStart}_{medStop}.tsf выполнить";

        //    var valuableData = CreatePhysicalData(new[]
        //    {
        //        $"media/air Коэф.теплоотдачи.воздух 20 {medStart} {medStop} *",
        //        $"heat/1 * * Load {heatStart} {heatStop} baseLine|refLine;2;0|0|0|0",
        //        "mat/Элементы3D_3 Сталь_20ХМ 0 1500 *"
        //    }).ToList();
            
        //    var preProc = new PreProc.PreProc();
        //    Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV1(valuableData, pt, projPath + "TsfFolder\\"));
        //    Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV2(valuableData, new ProcessProperty { TaskKind = TaskKind.термическая_механическая, CommonTaskType = pt }, projPath));
        //}

        //[TestMethod]
        //[DataRow(new[]
        //{
        //    "heat/1 * * Load 0 50 baseLine|refLine;2;0|0|0|0",
        //    "media/air Коэф.теплоотдачи.воздух 20 0 1500 *",
        //    "mat/Элементы3D_3 Сталь_20ХМ 0 1500 *",
        //    "clamp/Узел_8 Жесткое X 0 * 0 1500 *"
        //})]
        //public void CalcCompData_ReturnsTrue_WhenFileTSFSuccesfullyCreated(string[] parameters)
        //{
        //    var heatFileName = $"термическая термическая10_50.tsf выполнить";
        //    var medFileName = $"термическая термическая20_1500.tsf выполнить";
        //    var preProc = new PreProc.PreProc();
        //    preProc.CalcCompDataV1(CreatePhysicalData(parameters).ToList(), ProcessType.Welding, projPath + "TsfFolder\\");
        //    preProc.CalcCompDataV2(CreatePhysicalData(parameters).ToList(), new ProcessProperty { TaskKind = TaskKind.термическая_механическая, CommonTaskType = ProcessType.Welding }, projPath);

        //    var projDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //    var heatLocation = Path.GetFullPath(heatFileName);
        //    var medLocation = Path.GetFullPath(medFileName);

        //    Assert.AreEqual($"{projDir}\\{heatFileName}", heatLocation);
        //    Assert.AreEqual($"{projDir}\\{medFileName}", medLocation);
        //}



        [TestMethod]
        [DataRow(@"MNOGOPROHOD.bpf", ProcessType.Welding)]
        [DataRow(@"FastCheck.bpf", ProcessType.Welding)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Heating)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Quenching)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Tempering)]
        //[DataRow(@".\..\..\CalcCompDataTest\1.bpf", ProcessType.Welding)]
        //[DataRow(@".\..\..\CalcCompDataTest\6.bpf", ProcessType.Welding)]
        public void PreProc_GenerateTSF_ShouldPass(string file, ProcessType pt)
        {
            var loader = new LoadProjectFromTextFormat();
            loader.LoadEvent += (ar1, ar2) => { };
            var project = loader.Load(projPath + file);

            var resData = new List<ICondData>();

            foreach (var data in project.TaskData)
                resData.Add(data as CondData);

            var preProc = new PreProc.PreProc();
            //preProc.CalcCompDataV1(resData, pt, projPath + "TsfShouldPass\\");
            preProc.CalcCompDataV2(resData, new ProcessProperty { TaskKind = TaskKind.термическая | TaskKind.механическая, CommonTaskType = pt }, projPath + "TsfShouldPass\\");
        }

        [TestMethod]
        [DataRow(@"MatDataShorterThenBoundData.bpf", DisplayName = "MatData_Shorter_Then_BoundData")]
        [DataRow(@"NoMaterialData.bpf", DisplayName = "No_MaterialData")]
        [DataRow(@"NoBoundData.bpf", DisplayName = "No_BoundData")]
        [DataRow(@"ClampDataStartsAfterProcessStart.bpf", DisplayName = "ClampData_Starts_After_Process_Start")]
        [DataRow(@"MedDataStartsAfterProcessStart.bpf", DisplayName = "MedData_Starts_After_Process_Start")]
        [DataRow(@"MaterialDataStartsAfterProcessStart.bpf", DisplayName = "MaterialDataStartsAfterProcessStart")]
        [DataRow(@"FreeSpacesBetweenClampData.bpf",DisplayName = "FreeSpacesBetweenClampData")]
        [DataRow(@"FreeSpacesBetweenMediaData.bpf")]
        [DataRow(@"FreeSpacesBetweenMaterialData.bpf")]
        [DataRow(@"HeatDataContiniousWithoutMediaData.bpf")]
        public void PreProc_GenerateTSF_ShouldFail(string file)
        {
            var resData = new List<ICondData>();
            var group = new Group("test", ObjType.Узел);
            group.Insert(0, new Node(0, new Geometry.Point3D()));
            group.Insert(0, new Node(1, new Geometry.Point3D(1,0,0)));
            group.Insert(0, new Node(2, new Geometry.Point3D(1,1,0)));

            var groupData = new GroupData();
            groupData.Add(group);
            var textData = new LoadTaskDataTextFormat().Load(projPath + file, groupData);
            foreach (var data in textData)
            {
                resData.Add(data as CondData);
            }
            var preProc = new PreProc.PreProc();
            //Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV1(resData, ProcessType.Welding, projPath));
            Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV2(resData, new ProcessProperty { TaskKind = TaskKind.термическая | TaskKind.механическая, CommonTaskType = ProcessType.Welding }, projPath));
        }
    }
}
