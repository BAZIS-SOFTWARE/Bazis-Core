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
        [DataRow(@"MNOGOPROHOD.bpf")]
        [DataRow(@"FastCheck.bpf")]
        public void PreProc_PrepareParameters_ShouldPass(string file)
        {
            var loader = new LoadProjectFromTextFormat();
            var project = loader.Load(projPath + file);

            var resData = new List<ICondData>();

            foreach (var data in project.TaskData)
                resData.Add(data as CondData);

            var preProc = new global::PreProc.PreProc();
            var parameters = preProc.CalcCompDataV2(resData, TaskKind.термическая | TaskKind.механическая);
            Assert.IsTrue(parameters.Count > 0);
        }

        [TestMethod]
        public void PreProc_PreparesActiveConditionReferences()
        {
            var loader = new LoadProjectFromTextFormat();
            var project = loader.Load(projPath + "FastCheck.bpf");
            var preProc = new global::PreProc.PreProc();
            var parameters = preProc.CalcCompDataV2(project.TaskData, TaskKind.термическая);
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
            var parameters = preProc.CalcCompDataV2(project.TaskData, TaskKind.термическая | TaskKind.механическая);
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
            Assert.ThrowsException<Exception>(() => preProc.CalcCompDataV2(resData, TaskKind.термическая | TaskKind.механическая));
        }
    }
}
