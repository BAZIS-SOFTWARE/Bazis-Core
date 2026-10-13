using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project.IO;
using Project.Tasks;
using Project.Tasks.LocalFrames;
using System.Globalization;
using Project.Interfaces;
using Project.Tasks.Functions;
using Model.IO;

namespace TestProject_test.Project
{
    [TestClass]
    public class TestTaskDataCalculator
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private IProjectData StartUpProjForTests(string path)
        {
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(path);

            return proj;
        }
        [TestMethod]
        [DataRow(@"../../../SharedData/ProjectFiles/proj_traj.bpf",50)]
        //[DataRow(@"c:\projs\Roman\LazerWelding\Proj.bpf2", 5)]
        
        public void CalcStopTime(string path, float stopTimeRes)
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(path);

            //act
            var heat = proj.TaskData.Find<HeatData>().First();

            var motionTime = (heat.LocalFrame as MovedFrame).CalcMotionTime();
            var stopTime = heat.StartTime + motionTime;

            //assert
            Assert.AreEqual(stopTimeRes, stopTime);

        }

        [TestMethod]
        public void CheckTrajNodes()
        {
            //assign
            var loader = new LoadProjectFromTextFormat();
            var proj = loader.Load(@"../../../SharedData/ProjectFiles/proj_traj.bpf");

            //act
            var baseLineGr = proj.ModelData.GroupData.Find("baseLine");
            var refLineGr = proj.ModelData.GroupData.Find("refLine");
            var stNodesGr = proj.ModelData.GroupData.Find("startNodes");

            var mff = new MovedFrame(baseLineGr, refLineGr,1);

            //assert
            Assert.AreNotEqual(mff.BaseLine.Count(), 0);
        }

        [TestMethod]
        public void SortTrajNodes()
        {
            //assign
            var loader = new LoadModelFromBPFTextFile();
            var model = loader.Load(@"../../../SharedData/ProjectFiles/proj_traj_baseLine_not_sorted.bpf");

            //act
            var baseLineGr = model.GroupData.Find("baseLine");
            var refLineGr = model.GroupData.Find("refLine");
            var stNodesGr = model.GroupData.Find("startNodes");

            var mff = new MovedFrameBuilder().Build(stNodesGr, baseLineGr, refLineGr, 5);

            //assert
            Assert.AreEqual(674, mff.BaseLine[0].Number);
            Assert.AreEqual(39, mff.RefLine[0].Number);
        }

        public static MovedFrame SetUpGroupData
            (string baseLineGroupName, string refLineGroupName, string startGroupName, string endGroupName)
        {
            var loader = new LoadModelFromBPFTextFile();
            var model = loader.Load(@"../../../SharedData/ProjectFiles/TrajTest.bpf");

            var baseLine = model.GroupData.Find(baseLineGroupName);
            var refLine = model.GroupData.Find(refLineGroupName);
            var startPoints = model.GroupData.Find(startGroupName);
            

            return new MovedFrameBuilder().Build(startPoints, baseLine, refLine, 5);
        }

        [DataTestMethod]
        [DataRow("parallel_base", "parallel_ref", "parallel_start", "parallel_end",
            DisplayName = "WhenBaseAndRefParallel")]
        [DataRow("empty_nodes", "empty_nodes", "empty_start", "empty_end",
            DisplayName = "WhenCheckWithMultiplePointsInOnePoint")]
        [DataRow("gt3_intersect_base", "gt3_intersect_refl", "gt3_intersect_start", "gt3_intersect_end",
            DisplayName = "WhenMoreThenTwoPointsInLineIntersect")]
        [DataRow("sort_base", "sort_ref", "sort_start", "sort_end",
            DisplayName = "WhenTrajsIntersectInBegin")]
        [DataRow("parallel_base", "parallel_ref", "manyPoint_start", "parallel_end",
            DisplayName = "WhenManyPointStart")]
        [DataRow("parallel_base", "parallel_ref", "onePOint_start", "parallel_end",
            DisplayName = "WhenOnePointStart")]
        [DataRow("onePointGroup", "onePointGroup", "onePointGroup", "onePointGroup",
            DisplayName = "WhenOnePointForAllGroups")]
        [DataRow("empty_nodes", "empty_nodes", "invalid_start", "empty_end",
            DisplayName = "WhenInvalidStart")]
        [DataRow("empty_nodes", "empty_nodes", "another_invalid_start", "empty_end",
            DisplayName = "WhenAnotherInvalidStart")]
        [DataRow("parallel_base", "parallel_ref", "parallel_start", "onePOint_end",
            DisplayName = "WhenOnePointEnd")]
        [DataRow("onePointGroup", "onePointGroup", "onePointGroup", "onePointGroup",
            DisplayName = "WhenOnePointForAllGroups")]
        [DataRow("twoPointIntersection_base", "EndIntersection_ref", "twoPointIntersection_start", "twoPointIntersection_end",
            DisplayName = "WhenIntersectsEnd")]
        [DataRow("twoPointIntersection_base", "twoPointIntersection_ref", "twoPointIntersection_start", "twoPointIntersection_end",
             DisplayName = "TwoIntersectionsSharp")]
        [DataRow("empty_nodes", "empty_nodes_without_start_end", "empty_start", "empty_end",
            DisplayName = "WhenBaseOrRefLineDoesNotContainStartAndEndDuringSort")]
        [DataRow("startIsPartOfTheEnd_base", "startIsPartOfTheEnd_ref", "startIsPartOfTheEnd_start", "startIsPartOfTheEnd_end",
            DisplayName = "WhenStartIsPartOfTheEnd")]
        [DataRow("twoPointIntersection_base", "StartIntersection_ref", "twoPointIntersection_start", "twoPointIntersection_end",
            DisplayName = "WhenIntersectsStart")]
        [DataRow("twoPointIntersection_base", "StartEndIntersection_ref", "twoPointIntersection_start", "twoPointIntersection_end",
            DisplayName = "WhenIntersectsStartEnd")]
        public void MovedFrameFunction_ShouldFall
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<Exception>(() => SetUpGroupData(baseLine, refLine, start, end));
        }
    }
}
