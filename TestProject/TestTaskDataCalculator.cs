using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Project;
using Project.IO;
using Project.Tasks;
using Project.Tasks.Functions;
using Project.Tasks.FrameCreators;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace TestProject
{
    [TestClass]
    public class TestTaskDataCalculator
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        private ProjectData StartUpProjForTests(string path)
        {
            var proj = new ProjectData();

            proj.Loader.LoadEvent += (ar1, ar2) => { };
            proj.Load(path);

            return proj;
        }
        [TestMethod]
        public void CalcStopTime()
        {
            //assign

            var proj = new ProjectData();

            proj.Loader.LoadEvent += (ar1, ar2) => { };
            //act
            proj.Load(@"..\..\..\Examples\proj_traj.bpf");

            var heat = proj.TaskData.Find<HeatData>().First();
            
            var motionTime = (heat.FrameFunction.LocalFrame as MovedFrame).CalcMotionTime();
            var stopTime = heat.StartTime + motionTime;

            //assert
            Assert.AreEqual(50, stopTime);

        }

        [TestMethod]
        public void CheckTrajNodes()
        {
            //assign

            var proj = new ProjectData();

            proj.Loader.LoadEvent += (ar1, ar2) => { };
            //act
            proj.Load(@"..\..\..\Examples\proj_traj.bpf");

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

            var proj = new ProjectData();

            proj.Loader.LoadEvent += (ar1, ar2) => { };
            //act
            proj.Load(@"..\..\..\Examples\proj_traj_baseLine_not_sorted.bpf");

            var baseLineGr = proj.ModelData.GroupData.Find("baseLine");
            var refLineGr = proj.ModelData.GroupData.Find("refLine");
            var stNodesGr = proj.ModelData.GroupData.Find("startNodes");

            var mff = new MovedFrameBuilder().Build(stNodesGr, baseLineGr, refLineGr, 5);

            //assert
            Assert.AreEqual(mff.BaseLine[0].Number, mff.StartPoint.Number);
        }

        public static MovedFrame SetUpGroupData
            (string baseLineGroupName, string refLineGroupName, string startGroupName, string endGroupName)
        {
            var proj = new ProjectData();

            proj.Loader.LoadEvent += (ar1, ar2) => { };
            proj.Load(@"..\..\..\Examples\TrajTest.bpf");

            var baseLine = proj.ModelData.GroupData.Find(baseLineGroupName);
            var refLine = proj.ModelData.GroupData.Find(refLineGroupName);
            var startPoints = proj.ModelData.GroupData.Find(startGroupName);
            

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
