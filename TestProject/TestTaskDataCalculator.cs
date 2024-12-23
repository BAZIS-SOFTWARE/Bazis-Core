using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Project;
using Project.IO;
using Project.Tasks;
using Project.Tasks.Functions;
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
            proj.ModelData = new ModelData();

            proj.Loader.LoadEvent += (ar1, ar2) => { };
            proj.Load(path);

            return proj;
        }
        [TestMethod]
        public void CalcStopTime()
        {
            //assign

            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();
            proj.Loader.LoadEvent += (ar1, ar2) => { };
            //act
            proj.Load(@"..\..\..\Examples\proj_traj.bpf");

            var heat = proj.TaskData.Find<HeatData>().First();
            
            var motionTime = heat.MovedFrame.CalcMotionTime();
            var stopTime = heat.StartTime + motionTime;

            //assert
            Assert.AreEqual(50, stopTime);

        }

        [TestMethod]
        public void CheckTrajNodes()
        {
            //assign

            var proj = new ProjectData();
            proj.ModelData = new ModelData();

            proj.Loader.LoadEvent += (ar1, ar2) => { };
            //act
            proj.Load(@"..\..\..\Examples\proj_traj.bpf");

            var baseLineGr = proj.ModelData.GroupData.Find("baseLine");
            var refLineGr = proj.ModelData.GroupData.Find("refLine");
            var stNodesGr = proj.ModelData.GroupData.Find("startNodes");

            var mff = new MovedFrame(1);

            mff.BaseLine = baseLineGr;
            mff.RefLine = refLineGr;
            mff.StartPoints = stNodesGr;
            mff.StopPoints = stNodesGr;

            //assert
            mff.CheckTrajNodes();
        }

        [TestMethod]
        public void SortTrajNodes()
        {
            //assign

            var proj = new ProjectData();
            proj.ModelData = new ModelData();

            proj.Loader.LoadEvent += (ar1, ar2) => { };
            //act
            proj.Load(@"..\..\..\Examples\proj_traj_baseLine_not_sorted.bpf");

            var baseLineGr = proj.ModelData.GroupData.Find("baseLine");
            var refLineGr = proj.ModelData.GroupData.Find("refLine");
            var stNodesGr = proj.ModelData.GroupData.Find("startNodes");

            var mff = new MovedFrame(1);

            mff.BaseLine = baseLineGr;
            mff.RefLine = refLineGr;
            mff.StartPoints = stNodesGr;
            mff.StopPoints = stNodesGr;

            mff.SortTrajNodes();

            //assert
            Assert.AreEqual(mff.BaseLine[0].Number, mff.StartPoints[1].Number);
            Assert.AreEqual(mff.RefLine[0].Number, mff.StartPoints[0].Number);

        }

        public static MovedFrame SetUpGroupData
            (string baseLineGroupName, string refLineGroupName, string startGroupName, string endGroupName)
        {
            var proj = new ProjectData();
            proj.ModelData = new ModelData();

            proj.Loader.LoadEvent += (ar1, ar2) => { };
            proj.Load(@"..\..\..\Examples\TrajTest.bpf");

            return new MovedFrame(1)
            {
                BaseLine = proj.ModelData.GroupData.Find(baseLineGroupName),
                RefLine = proj.ModelData.GroupData.Find(refLineGroupName),
                StartPoints = proj.ModelData.GroupData.Find(startGroupName),
                StopPoints = proj.ModelData.GroupData.Find(endGroupName),
            };
        }

        [TestMethod]
        [DataRow("empty_nodes", "empty_nodes", "empty_start", "empty_end")]
        public void MovedFrameFunction_ShouldFall_WhenCheckWithMultiplePointsInOnePoint
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<Exception>(() => SetUpGroupData(baseLine, refLine, start, end).CheckTrajNodes());
        }

        [TestMethod]
        [DataRow("gt3_intersect_base", "gt3_intersect_refl", "gt3_intersect_start", "gt3_intersect_end")]
        public void MovedFrameFunction_ShouldFall_WhenMoreThenTwoPointsInLineIntersect
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<Exception>(() => SetUpGroupData(baseLine, refLine, start, end).CheckTrajNodes());
        }

        [TestMethod]
        [DataRow("sort_base", "sort_ref", "sort_start", "sort_end")]
        public void MovedFrameFunction_ShouldPass_WhenSortCorrect
            (string baseLine, string refLine, string start, string end)
        {
            var mff = SetUpGroupData(baseLine, refLine, start, end);
            mff.SortTrajNodes();

            Assert.AreEqual("sort_base Узел 8 | 2 3 0 1 8 10 12 14", mff.BaseLine.ToString());
            Assert.AreEqual("sort_ref Узел 8 | 2 3 11 1 0 7 13 9", mff.RefLine.ToString());
        }

        [TestMethod]
        [DataRow("empty_nodes", "empty_nodes_without_start_end", "empty_start", "empty_end")]
        [DataRow("empty_nodes_without_start_end", "empty_node", "empty_start", "empty_end")]
        public void MovedFrameFunction_ShouldFail_WhenBaseOrRefLineDoesNotContainStartAndEndDuringSort
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<InvalidOperationException>(() => SetUpGroupData(baseLine, refLine, start, end).SortTrajNodes());
        }

        [TestMethod]
        [DataRow("empty_nodes", "empty_nodes", "invalid_start", "empty_end")]
        [DataRow("empty_nodes", "empty_nodes", "another_invalid_start", "empty_end")]
        public void MovedFrameFunction_ShouldFail_WhenInvalidStart
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<InvalidOperationException>(() => SetUpGroupData(baseLine, refLine, start, end).SortTrajNodes());
        }

        [TestMethod]
        [DataRow("parallel_base", "parallel_ref", "parallel_start", "parallel_end")]
        public void MovedFrameFunction_ShouldFail_WhenBaseAndRefParallel
            (string baseLine, string refLine, string start, string end)
        {
            var arc = SetUpGroupData(baseLine, refLine, start, end);
            arc.CheckTrajNodes();
            arc.SortTrajNodes();
        }

        [TestMethod]
        [DataRow("parallel_base", "parallel_ref", "parallel_start", "onePOint_end")]
        public void MovedFrameFunction_ShouldPass_WhenOnePointEnd
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<Exception>(() => SetUpGroupData(baseLine, refLine, start, end).CheckTrajNodes());
        }

        [TestMethod]
        [DataRow("parallel_base", "parallel_ref", "parallel_start", "manyPoint_end")]
        public void MovedFrameFunction_ShouldPass_WhenManyPointEnd
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<Exception>(() => SetUpGroupData(baseLine, refLine, start, end).CheckTrajNodes());
        }

        [TestMethod]
        [DataRow("parallel_base", "parallel_ref", "onePOint_start", "parallel_end")]
        public void MovedFrameFunction_ShouldFail_WhenOnePointStart
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<Exception>(() => SetUpGroupData(baseLine, refLine, start, end).CheckTrajNodes());
        }

        [TestMethod]
        [DataRow("parallel_base", "parallel_ref", "manyPoint_start", "parallel_end")]
        public void MovedFrameFunction_ShouldFail_WhenManyPointStart
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<Exception>(() => SetUpGroupData(baseLine, refLine, start, end).CheckTrajNodes());
        }

        //should fail, but pass
        [TestMethod]
        [DataRow("onePointGroup", "onePointGroup", "onePointGroup", "onePointGroup")]
        public void MovedFrameFunction_ShouldFail_WhenOnePointForAllGroups
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<Exception>(() => SetUpGroupData(baseLine, refLine, start, end).CheckTrajNodes());
        }

        [TestMethod]
        [DataRow("twoPointIntersection_base", "twoPointIntersection_ref", "twoPointIntersection_start", "twoPointIntersection_end")]
        public void MovedFrameFunction_ShouldFail_TwoIntersectionsSharp
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var arc = SetUpGroupData(baseLine, refLine, start, end);
                arc.CheckTrajNodes();
                arc.SortTrajNodes();
            });
        }

        [TestMethod]
        [DataRow("twoPointIntersection_base", "StartIntersection_ref", "twoPointIntersection_start", "twoPointIntersection_end")]
        public void MovedFrameFunction_ShouldPass_WhenIntersectsStart
            (string baseLine, string refLine, string start, string end)
        {
            var arc = SetUpGroupData(baseLine, refLine, start, end);
            arc.CheckTrajNodes();
            arc.SortTrajNodes();
        }

        [TestMethod]
        [DataRow("twoPointIntersection_base", "EndIntersection_ref", "twoPointIntersection_start", "twoPointIntersection_end")]
        public void MovedFrameFunction_ShouldFail_WhenIntersectsEnd
            (string baseLine, string refLine, string start, string end)
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var arc = SetUpGroupData(baseLine, refLine, start, end);
                arc.CheckTrajNodes();
                arc.SortTrajNodes();
            });
        }

        [TestMethod]
        [DataRow("twoPointIntersection_base", "StartEndIntersection_ref", "twoPointIntersection_start", "twoPointIntersection_end")]
        public void MovedFrameFunction_ShouldPass_WhenIntersectsStartEnd
            (string baseLine, string refLine, string start, string end)
        {
            var arc = SetUpGroupData(baseLine, refLine, start, end);
            arc.CheckTrajNodes();
            arc.SortTrajNodes();
        }

        [TestMethod]
        [DataRow("startIsPartOfTheEnd_base", "startIsPartOfTheEnd_ref", "startIsPartOfTheEnd_start", "startIsPartOfTheEnd_end")]
        public void MovedFrameFunction_ShouldPass_WhenStartIsPartOfTheEnd
            (string baseLine, string refLine, string start, string end)
        {
            var arc = SetUpGroupData(baseLine, refLine, start, end);
            arc.CheckTrajNodes();
            arc.SortTrajNodes();
        }
    }
}
