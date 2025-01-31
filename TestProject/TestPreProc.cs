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

namespace TestProject
{
    [TestClass]
    public class TestPreProc
    {
        private static string projPath = Path.GetFullPath(@".\..\..\CalcCompDataTest\TsfFolder\");
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [TestMethod]
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
        public void SortingByTimeAndTypeCheck(string[] strsIn, string[]strsOut)
        {
            var preProc = new PreProc.PreProc();

            var results = preProc.SortCompDataByTimeAndType(strsIn);

            for (int i = 0; i < results.Count; i++)
            {
                Assert.AreEqual(results[i], strsOut[i]);
            }
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
        public void BaseCheck()
        {
            var heatInfo = "ARC;4;100;25 Load 0 49 baseLine|refLine;2;startNodes;startNodes";
            var medInfo = "air Коэф.теплоотдачи.воздух 20 0 1500 *";
            var matInfo = "Элементы3D_3 Сталь_20ХМ 0 1500 *";
            var clampInfo = "Узел_8 Жесткое X * 0 1500 *";

            var valuableData = new List<IValuableData>()
            {
                new MediaData(new Group("",ObjType.Объект),medInfo),
                new HeatData(new Group("",ObjType.Объект),heatInfo),
                new MatData(new Group("",ObjType.Объект),matInfo),
                new ClampData(new Group("",ObjType.Объект),clampInfo)
            };

            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(valuableData, ProcessType.Welding, projPath);
        }

        [TestMethod]
        [DataRow(0, 120, 1, 1500, 0)]
        [DataRow(0, 50.0001f, 50.001f, 100, 0)]
        public void CalcCompData_Fails_WhenHeatDataStartEarlierThenMedDataStop
            (float heatStart, float heatStop, float medStart, float medStop, ProcessType pt)
        {
            var heatInfo = $"ARC;4;100;25 Load {heatStart} {heatStop} baseLine|refLine;2;startNodes;startNodes";
            var medInfo = $"air Коэф.теплоотдачи.воздух 20 {medStart} {medStop} *";
            var matInfo = "Элементы3D_3 Сталь_20ХМ 0 1500 *";

            var heatfileName = $"термическая термическая{1}{heatStart}_{heatStop}.tsf выполнить";
            var medfileName = $"термическая термическая{2}{medStart}_{medStop}.tsf выполнить";

            var valuableData = new List<IValuableData>()
            {
                new MediaData(new Group("",ObjType.Объект),medInfo),
                new HeatData(new Group("",ObjType.Объект),heatInfo),
                new MatData(new Group("",ObjType.Объект),matInfo)
            };
            var preProc = new PreProc.PreProc();
            Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV1(valuableData, pt, projPath));
        }

        [TestMethod]
        public void CalcCompData_ReturnsTrue_WhenFileTSFSuccesfullyCreated()
        {
            var heatInfo = "ARC;4;100;25 Load 0 50 baseLine|refLine;2;startNodes;startNodes";
            var medInfo = $"air Коэф.теплоотдачи.воздух 20 0 1500 *";
            var matInfo = "Элементы3D_3 Сталь_20ХМ 0 1500 *";
            var clampInfo = "Узел_8 Жесткое X * 0 1500 *";

            var heatFileName = $"термическая термическая10_50.tsf выполнить";
            var medFileName = $"термическая термическая20_1500.tsf выполнить";

            var valuableData = new List<IValuableData>()
            {
                new MediaData(new Group("",ObjType.Объект),medInfo),
                new HeatData(new Group("",ObjType.Объект),heatInfo),
                new MatData(new Group("",ObjType.Объект),matInfo),
                new ClampData(new Group("",ObjType.Объект),clampInfo)
            };
            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(valuableData, ProcessType.Welding, projPath);

            var projDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var heatLocation = Path.GetFullPath(heatFileName);
            var medLocation = Path.GetFullPath(medFileName);

            Assert.AreEqual($"{projDir}\\{heatFileName}", heatLocation);
            Assert.AreEqual($"{projDir}\\{medFileName}", medLocation);
        }

        [TestMethod]
        public void CalcCompData_ReturnsTrue_WhenMaterialContinousWholeProcess()
        {
            var matInfo = "Элементы3D_3 Сталь_20ХМ 0 1500 *";
            var medInfo = "air Коэф.теплоотдачи.воздух 20 0 1500 *";
            var heatInfo = "ARC;4;100;25 Load 0 49.998 baseLine|refLine;2;startNodes;startNodes;0|0|0|0";
            var clampInfo = "Узел_8 Жесткое X * 0 1500 *";

            var valuableData = new List<IValuableData>()
            {
                new MediaData(new Group("",ObjType.Объект),medInfo),
                new HeatData(new Group("",ObjType.Объект),heatInfo),
                new MatData(new Group("",ObjType.Объект),matInfo),
                new ClampData(new Group("",ObjType.Объект),clampInfo)
            };
            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(valuableData, ProcessType.Welding, projPath);
        }

        [TestMethod]
        [DataRow(@".\..\..\CalcCompDataTest\MNOGOPROHOD.bpf", ProcessType.Welding)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Heating)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Quenching)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Tempering)]
        //[DataRow(@".\..\..\CalcCompDataTest\1.bpf", ProcessType.Welding)]
        //[DataRow(@".\..\..\CalcCompDataTest\6.bpf", ProcessType.Welding)]
        public void PreProc_GenerateTSF_ShouldPass(string path, ProcessType pt)
        {
            var project = new ProjectData();

            project.ModelData = new ModelData();
            project.TaskData = new TaskData();

            project.Load(path);

            var resData = new List<IValuableData>();

            foreach (var data in project.TaskData)
            {
                resData.Add(data as ValuableData);
            }
            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(resData, pt, projPath);
        }

        [TestMethod]
        [DataRow(@".\..\..\CalcCompDataTest\MatDataShorterThenBoundData.bpf", DisplayName = "MatDataShorterThenBoundData")]
        [DataRow(@".\..\..\CalcCompDataTest\NoMaterialData.bpf", DisplayName = "NoMaterialData")]
        [DataRow(@".\..\..\CalcCompDataTest\NoBoundData.bpf", DisplayName = "NoBoundData")]
        [DataRow(@".\..\..\CalcCompDataTest\ClampDataStartsAfterProcessStart.bpf", DisplayName = "ClampDataStartsAfterProcessStart")]
        [DataRow(@".\..\..\CalcCompDataTest\MedDataStartsAfterProcessStart.bpf", DisplayName = "MedDataStartsAfterProcessStart")]
        [DataRow(@".\..\..\CalcCompDataTest\MaterialDataStartsAfterProcessStart.bpf")]
        [DataRow(@".\..\..\CalcCompDataTest\FreeSpacesBetweenClampData.bpf")]
        [DataRow(@".\..\..\CalcCompDataTest\FreeSpacesBetweenMediaData.bpf")]
        [DataRow(@".\..\..\CalcCompDataTest\FreeSpacesBetweenMaterialData.bpf")]
        [DataRow(@".\..\..\CalcCompDataTest\HeatDataContiniousWithoutMediaData.bpf")]
        public void PreProc_GenerateTSF_ShouldFail(string path)
        {
            var resData = new List<IValuableData>();
            var group = new Group("test",ObjType.Объект);
            var groupData = new GroupData();
            groupData.Add(group);
            var textData = new LoadTaskDataTextFormat().Load(path, groupData);
            foreach (var data in textData)
            {
                resData.Add(data as ValuableData);
            }
            var preProc = new PreProc.PreProc();
            Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV1(resData, ProcessType.Welding, projPath));
        }

        [TestMethod]
        public void FastCheck()
        {
            var resData = new List<IValuableData>
            {
                new MediaData(new Group("",ObjType.Объект),"air Коэф.теплоотдачи.воздух 20 0 1500 *"),
                new HeatData(new Group("",ObjType.Объект),"ARC;15;75;15 Элемент3D_2 0 95.08694 Узел_5|Узел_6;2;Узел_7;Узел_7;0|0|0|0"),
                new ClampData(new Group("",ObjType.Объект),"Узел_8 Жесткое X * 0 1500 *"),
                new MatData(new Group("",ObjType.Объект),"mat Сталь_20ХМ 0 1500 *")
            };

            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV1(resData, ProcessType.Welding, projPath);
        }

        [DataRow(ComplexTaskType.термическая, ProcessType.Welding, new[] { "Med/air Коэф.теплоотдачи.воздух 20 0 1500 *", "Heat/ARC;15;75;15 Элемент3D_2 0 95.08694 Узел_5|Узел_6;2;Узел_7;Узел_7;0|0|0|0", "Mat/mat Сталь_20ХМ 0 1500 *" })]
        [TestMethod]
        public void CalcCompDataV2_ShouldPass_OnSingleProcessTaskCreation(ComplexTaskType generalTaskType, ProcessType commonTaskType, params string[] valuableDataStrings)
        {
            var data = new List<IValuableData>();
            foreach(var str in valuableDataStrings.Select(x => x.Split('/')))
            {
                switch(str[0])
                    case "Med":
            }

            var preProc = new PreProc.PreProc();
            preProc.CalcCompDataV2(data, new ProcessProperty { GeneralTaskType = generalTaskType, CommonTaskType = commonTaskType }, projPath);
        }
    }
}
