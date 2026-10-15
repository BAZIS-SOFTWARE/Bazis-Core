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
using Project.TaskParameters;

namespace TestProject_test.PreProc
{
    [TestClass]
    public class TestPreProc
    {
        
        private static string projPath =  Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "TestProject", "PreProc", "Data", "CalcCompDataTest")) + Path.DirectorySeparatorChar;

        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [TestMethod]
    [DataRow(
        new string[]
        {
            "путь/механическая_1_0_3.tsf",
            "путь/механическая_2_3_1500.tsf",
            "путь/термическая_1_0_3.tsf",
            "путь/термическая_2_3_1500.tsf"
        },
        new string[]
        {
            "термическая путь/термическая_1_0_3.tsf выполнить",
            "механическая путь/механическая_1_0_3.tsf выполнить",
            "термическая путь/термическая_2_3_1500.tsf выполнить",
            "механическая путь/механическая_2_3_1500.tsf выполнить"
        }
        )]
    [DataRow(
            new string[]
        {
            "путь/термическая_1_0_7.05826.tsf",
            "путь/термическая_2_7.05826_10.tsf",
            "путь/термическая_3_10_15.57324.tsf",
            "путь/механическая_1_0_7.05826.tsf",
            "путь/механическая_2_7.05826_10.tsf",
            "путь/механическая_3_10_15.57324.tsf"
        },
                        new string[]
        {
            "термическая путь/термическая_1_0_7.05826.tsf выполнить",
            "механическая путь/механическая_1_0_7.05826.tsf выполнить",
            "термическая путь/термическая_2_7.05826_10.tsf выполнить",
            "механическая путь/механическая_2_7.05826_10.tsf выполнить",
            "термическая путь/термическая_3_10_15.57324.tsf выполнить",
            "механическая путь/механическая_3_10_15.57324.tsf выполнить"
        }
        )]
        [DataRow(
            new string[]
        {
            "путь_test/механическая_1_0_20.12732.tsf",
            "путь_test/механическая_2_0_20.12732.tsf",
            "путь_test/механическая_3_0_20.12732.tsf",
            "путь_test/термическая_1_0_20.12732.tsf",
            "путь_test/термическая_2_0_20.12732.tsf",
            "путь_test/термическая_3_0_20.12732.tsf"
        },
                        new string[]
        {
            "термическая путь_test/термическая_1_0_20.12732.tsf выполнить",
            "механическая путь_test/механическая_1_0_20.12732.tsf выполнить",
            "термическая путь_test/термическая_2_0_20.12732.tsf выполнить",
            "механическая путь_test/механическая_2_0_20.12732.tsf выполнить",
            "термическая путь_test/термическая_3_0_20.12732.tsf выполнить",
            "механическая путь_test/механическая_3_0_20.12732.tsf выполнить"
        }
        )]
        [DataRow(
            new string[] 
        {
            "путь/механическая_1_0_20.12732.tsf",
            "путь/механическая_2_0_20.12732.tsf",
            "путь/механическая_3_0_20.12732.tsf",
            "путь/термическая_1_0_20.12732.tsf",
            "путь/термическая_2_0_20.12732.tsf",
            "путь/термическая_3_0_20.12732.tsf"
        },
                        new string[]
        {
            "термическая путь/термическая_1_0_20.12732.tsf выполнить",
            "механическая путь/механическая_1_0_20.12732.tsf выполнить",
            "термическая путь/термическая_2_0_20.12732.tsf выполнить",
            "механическая путь/механическая_2_0_20.12732.tsf выполнить",
            "термическая путь/термическая_3_0_20.12732.tsf выполнить",
            "механическая путь/механическая_3_0_20.12732.tsf выполнить"
        }
        )]
        [DataRow(
            new string[]
        {
            "путь/механическая_1_0.001_0.12732.tsf",
            "путь/механическая_2_0.12732_20.12732.tsf",
            "путь/механическая_3_20.12732_21.12732.tsf",
            "путь/термическая_1_0.001_0.12732.tsf",
            "путь/термическая_2_0.12732_20.12732.tsf",
            "путь/термическая_3_20.12732_21.12732.tsf"
        },
                        new string[]
        {
            "термическая путь/термическая_1_0.001_0.12732.tsf выполнить",
            "механическая путь/механическая_1_0.001_0.12732.tsf выполнить",
            "термическая путь/термическая_2_0.12732_20.12732.tsf выполнить",
            "механическая путь/механическая_2_0.12732_20.12732.tsf выполнить",
            "термическая путь/термическая_3_20.12732_21.12732.tsf выполнить",
            "механическая путь/механическая_3_20.12732_21.12732.tsf выполнить"
        }
        )]
        [DataRow(
            new string[]
        {
            "путь/механическая_10_0.001_0.12732.tsf",
            "путь/механическая_20_0.12732_20.12732.tsf",
            "путь/механическая_30_20.12732_21.12732.tsf",
            "путь/термическая_10_0.001_0.12732.tsf",
            "путь/термическая_20_0.12732_20.12732.tsf",
            "путь/термическая_30_20.12732_21.12732.tsf"
        },
                        new string[]
        {
            "термическая путь/термическая_10_0.001_0.12732.tsf выполнить",
            "механическая путь/механическая_10_0.001_0.12732.tsf выполнить",
            "термическая путь/термическая_20_0.12732_20.12732.tsf выполнить",
            "механическая путь/механическая_20_0.12732_20.12732.tsf выполнить",
            "термическая путь/термическая_30_20.12732_21.12732.tsf выполнить",
            "механическая путь/механическая_30_20.12732_21.12732.tsf выполнить"
        }
        )]
        public void SortingByTimeAndTypeCheck(string[] strsIn, string[]strsOut)
        {
            var preProc = new global::PreProc.PreProc();

            var results = preProc.SortCompDataByTimeAndType(strsIn);

            for (int i = 0; i < results.Count; i++)
                Assert.AreEqual(results[i], strsOut[i]);
        }

        [TestMethod]
        [DataRow(new[] { "путь/термическая_^3_20.12732_21.12732.tsf" })]
        [DataRow(new[] { "путь/%термическая_2_20.12732_21.12732.tsf" })]
        [DataRow(new[] { "путь/!_2_20.12732_21.12732.tsf" })]
        [DataRow(new[] { "путь/термическая_7_20.12732_#.12732.tsf" })]
        [DataRow(new[] { "путь/термическая_6_20.12?32_21.12732.tsf" })]
        [DataRow(new[] { "путь/термическая_5_20.1232_21.127()2.tsf" })]
        [DataRow(new[] { "путь/термическая_4_20.1232_21.12732.ts-f" })]
        [DataRow(new[] { "путь/термическая_3_20.1232_21=.12732.tsf" })]
        [DataRow(new[] { "путь/@_3_20.12732_21.12732.tsf" })]
        [DataRow(new[] { "путь/механическая_3_{}}.12732_21.12732.tsf" })]
        [DataRow(new[] { "путь/механическая_3_20.12732_21.127[].tsf" })]
        [DataRow(new[] { "путь/механическая_3_20.12732_21.12;;.tsf" })]
        [DataRow(new[] { "путь/type_int_float_float.tsf" })]
        public void SortingByTimeAndTypeCheck_ShouldFail_WhenContainsIlligalSymbol(string[] strsIn)
        {
            var preProc = new global::PreProc.PreProc();
            Assert.ThrowsException<DraftException>( () => preProc.SortCompDataByTimeAndType(strsIn));
        }



        [TestMethod]
        [DataRow(@"MNOGOPROHOD.bpf", ProcessType.Welding)]
        [DataRow(@"FastCheck.bpf", ProcessType.Welding)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Heating)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Quenching)]
        //[DataRow(@"c:\projs\testProj\Tempering\proj.bpf", ProcessType.Tempering)]
        //[DataRow(@".\..\..\PreProc\Data\CalcCompDataTest\1.bpf", ProcessType.Welding)]
        //[DataRow(@".\..\..\PreProc\Data\CalcCompDataTest\6.bpf", ProcessType.Welding)]
        public void PreProc_PrepareParameters_ShouldPass(string file, ProcessType pt)
        {
            var loader = new LoadProjectFromTextFormat();
            var project = loader.Load(projPath + file);

            var resData = new List<ICondData>();

            foreach (var data in project.TaskData)
                resData.Add(data as CondData);

            var preProc = new global::PreProc.PreProc();
            var property = new ProcessProperty { TaskKind = TaskKind.термическая | TaskKind.механическая, CommonTaskType = pt };
            var parameters = preProc.CalcCompDataV2(resData, property);
            Assert.IsTrue(parameters.Count > 0);
        }

        [TestMethod]
        public void PreProc_PreparesActiveConditionReferences()
        {
            var loader = new LoadProjectFromTextFormat();
            var project = loader.Load(projPath + "FastCheck.bpf");
            var preProc = new global::PreProc.PreProc();
            var property = new ProcessProperty { TaskKind = TaskKind.термическая, CommonTaskType = ProcessType.Welding };
            var parameters = preProc.CalcCompDataV2(project.TaskData, property);
            Assert.IsTrue(parameters.Count > 0);
            foreach (var item in parameters)
            {
                Assert.IsNotNull(item.ActiveConditions);
                foreach (var reference in item.ActiveConditions)
                {
                    var condition = project.TaskData[reference.Index];
                    Assert.AreEqual(condition.Kind, reference.Kind);
                    Assert.AreEqual(condition.Group.Name, reference.GroupName);
                    Assert.IsTrue(condition.StartTime <= item.TimeSettings.StartTime);
                    Assert.IsTrue(condition.StopTime >= item.TimeSettings.StopTime);
                }
            }
        }

        [TestMethod]
        public void PreProc_PreparesTemperatureInputForMechanicalTask()
        {
            var loader = new LoadProjectFromTextFormat();
            var project = loader.Load(projPath + "FastCheck.bpf");
            var preProc = new global::PreProc.PreProc();
            var property = new ProcessProperty { TaskKind = TaskKind.термическая | TaskKind.механическая, CommonTaskType = ProcessType.Welding };
            var parameters = preProc.CalcCompDataV2(project.TaskData, property);
            var mechanicalTasks = parameters.Where(item => item.TaskKind == TaskKind.механическая).ToList();
            Assert.IsTrue(mechanicalTasks.Count > 0);
            foreach (var item in mechanicalTasks)
            {
                var inputField = item.InputSets.Single(input => input.Name == PhysicalSetName.Thermal);
                Assert.AreEqual(PhysicalSetSource.ResultFile, inputField.Source);
                Assert.AreEqual(0, inputField.Values.Count);
                StringAssert.StartsWith(inputField.File, "../ResultsData/термическая_");
                var initialField = item.InitialSet;
                Assert.IsNotNull(initialField);
                Assert.AreEqual(PhysicalSetName.Mechanical, initialField.Name);
                if (initialField.Source == PhysicalSetSource.Values)
                {
                    Assert.IsTrue(initialField.Values.Count > 0);
                    Assert.IsTrue(initialField.Values.Values.All(values => values.Count == 1
                        && values[0].Name == PhysicalFieldName.Temperature
                        && values[0].Components.Length == 1
                        && values[0].Components[0] == 20.0));
                }
                else
                {
                    Assert.AreEqual(PhysicalSetSource.ResultFile, initialField.Source);
                    StringAssert.StartsWith(initialField.File, "../ResultsData/механическая_");
                }
            }
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
        public void PreProc_PrepareParameters_ShouldFail(string file)
        {
            var resData = new List<ICondData>();
            var group = new Group("test", ObjType.Узел);
            group.Insert(0, new Node(0, new global::Geometry.Point3D()));
            group.Insert(0, new Node(1, new global::Geometry.Point3D(1,0,0)));
            group.Insert(0, new Node(2, new global::Geometry.Point3D(1,1,0)));

            var groupData = new GroupData();
            groupData.Add(group);
            var textData = new LoadTaskDataTextFormat().Load(projPath + file, groupData);
            foreach (var data in textData)
            {
                resData.Add(data as CondData);
            }
            var preProc = new global::PreProc.PreProc();
            var property = new ProcessProperty { TaskKind = TaskKind.термическая | TaskKind.механическая, CommonTaskType = ProcessType.Welding };
            Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV2(resData, property));
        }
    }
}
