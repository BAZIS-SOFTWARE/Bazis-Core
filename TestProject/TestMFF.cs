using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Globalization;
using System.Threading;
using Geometry;
using Model.MeshObjects;
using Model.GroupsData;
using Model.Interfaces.MeshObjects;
using Project.Tasks;
using Model.Interfaces;
using Project.Tasks.Functions;
using Project.Tasks.FrameCreators;
using Project.Interfaces.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Project.Tasks.Functions.Welding;
using Project.IO;

namespace TestProject
{
    [TestClass]
    public class TestMFF
    {
        string testDir = @"..\..\..\Examples";
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }


        [TestMethod]
        [DataRow(@"\WeldingHeatSource\LWTest_v.bpf", DisplayName = "LWTest_v.bpf")]
        [DataRow(@"\WeldingHeatSource\test_t_lazer_0_10.bpf", DisplayName = "test_t_lazer_0_10.bpf")]
        [DataRow(@"\WeldingHeatSource\test_t_lazer_180_0_10.bpf", DisplayName = "test_t_lazer_180_0_10.bpf")]
        [DataRow(@"\WeldingHeatSource\ARCTest_v.bpf", DisplayName = "ARC")]
        [DataRow(@"\WeldingHeatSource\FSW\proj.bpf", DisplayName = "FSW")]

        public void MFF_EnergyCalculated(string path)
        {
            //assing
            var loader = new LoadProjectFromTextFormat();
            loader.LoadEvent += (ar1, ar2) => { };
            var proj = loader.Load($@"{testDir}\{path}");

            //act
            var lazerData = proj.TaskData.Find(DataKind.Нагрев).Select(x => (HeatData)x).ToArray();
            for (var i = 0; i < lazerData.Count(); i++)
            {
                var heatData = lazerData[i];
                var gr = heatData.Group;

                //if (heatData.FrameFunction is CillindricalFunction fswPin)
                //    fswPin.GetParameters().ElementAt(0).Value = 200;
                //if (heatData.FrameFunction is FSWShoulder fswShoulder)
                //    fswShoulder.GetParameters().ElementAt(0).Value = 0.001f;

                var powerFunc = heatData.FrameFunction;
                //assert
                var energy = new float[gr.Count];
                var mf = heatData.FrameFunction.LocalFrame as MovedFrame;
                mf.Time = 1;
                mf.CalcPosition();
                var counter = 0;
                foreach (var obj in gr)
                {
                    energy[counter] = powerFunc.CalcValue(obj.CalcCentr());
                    counter++;
                }
                var max = energy.Max();
                if (max == 0)
                    throw new Exception("Энергия не вводится!");
            }
        }

        [DataRow("arc", new[] { -10.0000001f, 0f, 0f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 0f, -10.0000001f, 0f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 0f, 0f, -10.0000001f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 10.0000001f, 0f, 0f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 0f, 10.0000001f, 0f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 0f, 0f, 10.0000001f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 9.99f, 0f, 0f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 0f, 9.99f, 0f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 0f, 0f, 9.99f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 10f, 0f, 0f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 0f, 10f, 0f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 0f, 0f, 10f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { -1f, -1f, -1f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { -5f, -5f, -5f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 0f, 0f, 0f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { -9f, 2f, 0f }, 1, DisplayName = "arc_WhenPointInsideSphere")]
        [DataRow("arc", new[] { 10.1f, 0f, 0f }, 0, DisplayName = "arc_NoCollision")]
        [DataRow("arc", new[] { 0f, 10.1f, 0f }, 0, DisplayName = "arc_NoCollision")]
        [DataRow("arc", new[] { 0f, 0f, 10.1f }, 0, DisplayName = "arc_NoCollision")]
        [DataRow("arc", new[] { 12f, 6f, 5f }, 0, DisplayName = "arc_NoCollision")]
        [DataRow("arc", new[] { -10.1f, 0f, 0f }, 0, DisplayName = "arc_NoCollision")]
        [DataRow("arc", new[] { 0f, -10.1f, 0f }, 0, DisplayName = "arc_NoCollision")]
        [DataRow("arc", new[] { 0f, 0f, -10.1f }, 0, DisplayName = "arc_NoCollision")]
        [DataRow("lz", new[] { 2f, 5f, 0f }, 1, DisplayName = "lz_WhenPointInsideSphere")]
        [DataRow("lz", new[] { 0f, 5f, -3f }, 1, DisplayName = "lz_WhenPointInsideSphere")]
        [DataRow("lz", new[] { 0f, 5f, -6f }, 1, DisplayName = "lz_WhenPointInsideSphere")]
        [DataRow("lz", new[] { 4.99f, 0f, 0f }, 1, DisplayName = "lz_WhenPointTouches")]
        [DataRow("lz", new[] { 9.99f, 0f, 0f }, 1, DisplayName = "lz_WhenPointTouches")]
        [DataRow("lz", new[] { 0f, 0f, -4.99f }, 1, DisplayName = "lz_WhenPointTouches")]
        [DataRow("lz", new[] { 0f, 0f, -9.99f }, 1, DisplayName = "lz_WhenPointTouches")]
        [DataRow("lz", new[] { 4.99f, 5f, 0f }, 1, DisplayName = "lz_WhenPointTouches")]
        [DataRow("lz", new[] { 0f, 5f, -4.99f }, 1, DisplayName = "lz_WhenPointTouches")]
        [DataRow("lz", new[] { 0f, 5f, -9.99f }, 1, DisplayName = "lz_WhenPointTouches")]
        [DataRow("lz", new[] { 4.99f, 10f, 0f }, 0, DisplayName = "lz_NoCollision")]
        [DataRow("lz", new[] { 9.99f, 10f, 0f }, 0, DisplayName = "lz_NoCollision")]
        [DataRow("lz", new[] { 0f, 10f, -4.99f }, 0, DisplayName = "lz_NoCollision")]
        [DataRow("lz", new[] { 0f, 10f, -9.99f }, 0, DisplayName = "lz_NoCollision")]
        [DataRow("lz", new[] { 9.99f, 5f, 0f }, 0, DisplayName = "lz_NoCollision")]
        [DataRow("lz", new[] { 4.99f, -10f, 0f }, 0, DisplayName = "lz_NoCollision")]
        [DataRow("lz", new[] { 9.99f, -10f, 0f }, 0, DisplayName = "lz_NoCollision")]
        [DataRow("FSWShoulder", new[] { 2f, 5f, 0f }, 1, DisplayName = "FSWShoulder_WhenPointTouches")]
        [DataRow("FSWShoulder", new[] { 0f, 5f, -3f }, 1, DisplayName = "FSWShoulder_WhenPointTouches")]
        [DataRow("FSWShoulder", new[] { 0f, 5f, -6f }, 1, DisplayName = "FSWShoulder_WhenPointTouches")]
        [DataRow("FSWShoulder", new[] { 0f, 10f, -9.99f }, 1, DisplayName = "FSWShoulder_WhenPointTouches")]
        [DataRow("FSWShoulder", new[] { 0f, 10f, -4.99f }, 1, DisplayName = "FSWShoulder_WhenPointTouches")]
        [DataRow("FSWShoulder", new[] { 4.99f, 0f, 0f }, 1, DisplayName = "FSWShoulder_WhenHasEdgeCollision")]
        [DataRow("FSWShoulder", new[] { 0f, 0f, -4.99f }, 1, DisplayName = "FSWShoulder_WhenHasEdgeCollision")]
        [DataRow("FSWShoulder", new[] { 0f, 0f, -9.99f }, 1, DisplayName = "FSWShoulder_WhenHasEdgeCollision")]
        [DataRow("FSWShoulder", new[] { 4.99f, 5f, 0f }, 1, DisplayName = "FSWShoulder_WhenHasEdgeCollision")]
        [DataRow("FSWShoulder", new[] { 0f, 5f, -4.99f }, 1, DisplayName = "FSWShoulder_WhenHasEdgeCollision")]
        [DataRow("FSWShoulder", new[] { 0f, 5f, -9.99f }, 1, DisplayName = "FSWShoulder_WhenHasEdgeCollision")]
        [DataRow("FSWShoulder", new[] { 4.99f, 10f, 0f }, 0, DisplayName = "FSWShoulder_NoCollision")]
        [DataRow("FSWShoulder", new[] { 9.99f, 10f, 0f }, 0, DisplayName = "FSWShoulder_NoCollision")]
        [DataRow("FSWShoulder", new[] { 9.99f, 0f, 0f }, 1, DisplayName = "FSWShoulder_NoCollision")]
        [DataRow("FSWShoulder", new[] { 9.99f, 5f, 0f }, 0, DisplayName = "FSWShoulder_NoCollision")]
        [DataRow("FSWShoulder", new[] { 4.99f, -10f, 0f }, 0, DisplayName = "FSWShoulder_NoCollision")]
        [DataRow("FSWShoulder", new[] { 9.99f, -10f, 0f }, 0, DisplayName = "FSWShoulder_NoCollision")]
        [DataRow("FSWPin", new[] { 2f, 5f, 0f }, 1, DisplayName = "FSWPin_WhenPointInsideCillinder")]
        [DataRow("FSWPin", new[] { 0f, 5f, -3f }, 1, DisplayName = "FSWPin_WhenPointInsideCillinder")]
        [DataRow("FSWPin", new[] { 0f, 5f, -6f }, 1, DisplayName = "FSWPin_WhenPointInsideCillinder")]
        [DataRow("FSWPin", new[] { 4.99f, 0f, 0f }, 1, DisplayName = "FSWPin_WhenHasEdgeCollision")]
        [DataRow("FSWPin", new[] { 9.99f, 0f, 0f }, 1, DisplayName = "FSWPin_WhenHasEdgeCollision")]
        [DataRow("FSWPin", new[] { 0f, 0f, -4.99f }, 1, DisplayName = "FSWPin_WhenHasEdgeCollision")]
        [DataRow("FSWPin", new[] { 0f, 0f, -9.99f }, 1, DisplayName = "FSWPin_WhenHasEdgeCollision")]
        [DataRow("FSWPin", new[] { 4.99f, 5f, 0f }, 1, DisplayName = "FSWPin_WhenHasEdgeCollision")]
        [DataRow("FSWPin", new[] { 0f, 5f, -4.99f }, 1, DisplayName = "FSWPin_WhenHasEdgeCollision")]
        [DataRow("FSWPin", new[] { 0f, 5f, -9.99f }, 1, DisplayName = "FSWPin_WhenHasEdgeCollision")]
        [DataRow("FSWPin", new[] { 4.99f, 10f, 0f }, 0, DisplayName = "FSWPin_NoCollision")]
        [DataRow("FSWPin", new[] { 9.99f, 10f, 0f }, 0, DisplayName = "FSWPin_NoCollision")]
        [DataRow("FSWPin", new[] { 0f, 10f, -4.99f }, 0, DisplayName = "FSWPin_NoCollision")]
        [DataRow("FSWPin", new[] { 0f, 10f, -9.99f }, 0, DisplayName = "FSWPin_NoCollision")]
        [DataRow("FSWPin", new[] { 9.99f, 5f, 0f }, 0, DisplayName = "FSWPin_NoCollision")]
        [DataRow("FSWPin", new[] { 4.99f, -10f, 0f }, 0, DisplayName = "FSWPin_NoCollision")]
        [DataRow("FSWPin", new[] { 9.99f, -10f, 0f }, 0, DisplayName = "FSWPin_NoCollision")]

        [TestMethod]
        public void FrameFunction_GetIntersectedObjects(string fName, float[] checkPoint, int res)
        {
            var frame = new Frame();
            FrameFunction pFunction;
            if (fName == "arc")
            {
                var arc = new SphereBuilder();
                arc.SetWidth("20");

                arc.SetFrame(frame);
                pFunction = (SphereFunction)arc;
            }
            else
            {
                var lazer = new CillindricalBuilder();
                lazer.SetLength("10");
                lazer.SetBottomDiam("10");
                lazer.SetUpperDiam("20");
                lazer.SetFrame(frame);
                pFunction = (CillindricalFunction)lazer;
            }
            //else if (fName == "FSWShoulder")
            //{
            //    var fswShoulder = new FSWShoulderBuilder();
            //    fswShoulder.SetLength("10");
            //    fswShoulder.SetBottomDiam("20");
            //    fswShoulder.SetUpperDiam("20");
            //    fswShoulder.SetFrame(frame);
            //    pFunction = (FSWShoulder)fswShoulder;
            //}
            //else
            //{
            //    var fswPin = new FSWPinBuilder();
            //    fswPin.SetLength("10");
            //    fswPin.SetBottomDiam("10");
            //    fswPin.SetUpperDiam("20");
            //    fswPin.SetFrame(frame);
            //    pFunction = (FSWPin)fswPin;
            //}

            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);
            //pFunction.
            var list = new List<Node>() { new Node(0, point) };
            Assert.AreEqual(pFunction.GetIntersectedObjects(list).Count, res);
        }

        //[DataRow(
        //    new float[]
        //    {
        //        0f, 0f, 0f,
        //        -1f, -1f, 0f,
        //        -2f, 2f, 0f,
        //        -2f, 2f, 1f,

        //        0f, 0f, 1f,
        //        -2f, -2f, 0f,
        //        2f, 3f, 0f,
        //        2f, 3f, 1f,

        //        0f, 6f, 0f,
        //        0f, 6f, 1f,
        //        4f, 4f, 0f,
        //        4f, 6f, 0f
        //    },
        //    new float[]
        //    {
        //        0f, 0f, 0f,
        //        -1f, -1f, 0f,
        //        -2f, 2f, 0f,
        //        -2f, 2f, 1f,

        //        0f, 0f, 1f,
        //        -2f, -2f, 0f,
        //        2f, 3f, 0f,
        //        2f, 3f, 1f,
        //    })]
        //[DataRow(
        //    new float[]
        //    {
        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        2f, 0f, 0f,
        //        2f, 1f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        2f, 1f, 0f,
        //        1f, 2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        1f, 2f, 0f,
        //        0f, 2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        0f, 2f, 0f,
        //        -1f, 2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        -1f, 2f, 0f,
        //        -2f, 1f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        -2f, 1f, 0f,
        //        -2f, 0f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        -2f, -1f, 0f,
        //        -1f, -2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        -1f, -2f, 0f,
        //        0f, -2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        0f, -2f, 0f,
        //        1f, -2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        1f, -2f, 0f,
        //        2f, -1f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        2f, -1f, 0f,
        //        2f, 0f, 0f
        //    },
        //    new float[]
        //    {
        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        2f, 0f, 0f,
        //        2f, 1f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        2f, 1f, 0f,
        //        1f, 2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        1f, 2f, 0f,
        //        0f, 2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        0f, 2f, 0f,
        //        -1f, 2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        -1f, 2f, 0f,
        //        -2f, 1f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        -2f, 1f, 0f,
        //        -2f, 0f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        -2f, -1f, 0f,
        //        -1f, -2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        -1f, -2f, 0f,
        //        0f, -2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        0f, -2f, 0f,
        //        1f, -2f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        1f, -2f, 0f,
        //        2f, -1f, 0f,

        //        0f, 0f, 0f,
        //        0f, 0f, 1f,
        //        2f, -1f, 0f,
        //        2f, 0f, 0f
        //    })]
        //[TestMethod]
        //public void SphereIsPointInside_ShouldPass_WhenSelectNecessaryNodes(float[] input, float[] expected)
        //{
        //    var setup = SetUpTest(input, expected);
        //    var expectedElements = setup.Item1;
        //    var resultList = setup.Item2;

        //    Assert.IsTrue(expectedElements.Count == resultList.Count);
        //    for (var i = 0; i < expectedElements.Count; i++)
        //    {
        //        Assert.AreEqual(expectedElements.ToString(), resultList.ToString());
        //        for (var j = 0; j < 4; j++)
        //            Assert.AreEqual(expectedElements[i][j].ToString(), resultList[i][j].ToString());
        //    }
        //}

        [DataRow(
            new float[]
            {
                0f, 0f, -10f,
                0f, 0f, -11f,
                2f, 0f, -10f,
                2f, 1f, -10f
            })]
        [DataRow(
            new float[]
            {
                4f, 0f, 10f,
                7f, 1f, 11f,
                7f, -1f, 10f,
                7f, 1f, 10f
            })]
        [DataRow(
            new float[]
            {
                4f, 0f, 100f,
                7f, 1f, 101f,
                7f, -1f, 100f,
                7f, 1f, 100f
            })]
        [TestMethod]
        public void SphereIsPointInside_ShouldPass_WhenNoElementCenterInsideSphere(float[] input)
        {
            var setup = SetUpTest(input, input);
            var resultList = setup.Item2;

            Assert.IsTrue(resultList.Count == 0);
        }

        private (List<IElement>, List<IElement>) SetUpTest(float[] input, float[] expected)
        {
            var heatData = new HeatData(new Group("", ObjType.Элемент3D), 0, 49);

            var baseLine = new Group("base", ObjType.Узел);
            var refLine = new Group("ref", ObjType.Узел);

            var start = new Group("start", ObjType.Узел);
            start.AddRange(new[] { new Node(10000, new Point3D(0f, 0f, 0f)), new Node(10001, new Point3D(0f, 0f, 1f)) });


            //Добавить ультилиту проверки траекторий

            for (var i = 0; i < heatData.StopTime; i++)
            {
                baseLine.Add(new Node(i, new Point3D(0f, 0f, i)));
                refLine.Add(new Node(i + 5000, new Point3D(0f, -1f, i)));
            }

            heatData.TrySetFrameFunction("SPH;10");
            heatData.FrameFunction.LocalFrame = new MovedFrame(baseLine, refLine, 5);
            heatData.FrameFunction.LocalFrame.CalcPosition();
            var expectedElements = new List<IElement>();
            var inputElements = new List<IElement>();

            if (expected.Count() >= 12)
                expectedElements = GetTetraElements(expected);
            if (input.Count() >= 12)
                inputElements = GetTetraElements(input);
            var resultList = new List<IElement>();
            var mf = heatData.FrameFunction.LocalFrame as MovedFrame;
            var end = mf.CalcMotionTime();

            for (var time = 0.0f; time < end; time += mf.Velocity)
            {
                mf.Time = time;
                resultList.AddRange(heatData.FrameFunction.GetIntersectedObjects(inputElements));
            }

            return (expectedElements, resultList);
        }

        private List<IElement> GetTetraElements(float[] input)
        {
            if (input.Length % 12 != 0)
                throw new Exception("Кол-во элементов для тестового массива должно быть кратно 3");

            var tetras = new List<IElement>();

            for (int i = 0; i < input.Length; i += 12)
            {
                var nodes = new Node[]
                {
                    new Node(i, new Point3D(input[i], input[i + 1], input[i + 2])),
                    new Node(i + 1, new Point3D(input[i + 3], input[i + 4], input[i + 5])),
                    new Node(i + 2, new Point3D(input[i + 6], input[i + 7], input[i + 8])),
                    new Node(i + 3, new Point3D(input[i + 9], input[i + 10], input[i + 11]))
                };

                tetras.Add(new Tetra(i, nodes));
            }
            return tetras;
        }

        [DataRow(2, 20)]
        [DataRow(5, 10)]
        [DataRow(1, 2)]
        [DataRow(0.2f, 2)]
        [TestMethod]
        public void IsOverlappingSelfSphere_ShouldPass_WhenOverlap(float velocity, float width)
        {
            SphereFunction mff = new SphereBuilder().SetWidth(width.ToString());
            Assert.IsTrue(mff.IsOverlappingSelf(velocity));
        }

        [DataRow(2, 20, 10)]
        [DataRow(1, 4, 2)]
        [DataRow(0.2f, 2, 0.4f)]
        [DataRow(1, 2, 2)]
        [DataRow(0.2f, 0.4f, 0.4f)]
        [TestMethod]
        public void IsOverlappingSelfCillinder_ShouldPass_WhenOverlap(float velocity, float upper, float bottom)
        {
            CillindricalFunction mff = new CillindricalBuilder().SetBottomDiam(bottom.ToString()).SetUpperDiam(upper.ToString());
            Assert.IsTrue(mff.IsOverlappingSelf(velocity));
        }

        [TestMethod]
        [DataRow(new[] { 15f, 15f, 15f }, "inside", DisplayName = "central point")]
        [DataRow(new[] { 14.7f, 13.7f, 15f }, "inside", DisplayName = "point in head ellipse, inside")]
        [DataRow(new[] { 12f, 19f, 14f }, "inside", DisplayName = "point in head ellipse, inside")]
        [DataRow(new[] { 2.8f, 15f, 14.6f }, "outside", DisplayName = "point in head ellipse, outside")]
        [DataRow(new[] { 10f, 19f, 13f }, "outside", DisplayName = "point in head ellipse, outside")]
        [DataRow(new[] { 15.3f, 14.4f, 15f }, "inside", DisplayName = "point in back ellipse, inside")]
        [DataRow(new[] { 12f, 3f, 14f }, "inside", DisplayName = "point in back ellipse, inside")]
        [DataRow(new[] { 15f, 3.8f, 10f }, "outside", DisplayName = "point in back ellipse, outside")]
        [DataRow(new[] { 10f, 4f, 5f }, "outside", DisplayName = "point in back ellipse, outside")]
        public void DoubleEllipsIsPointInside_FrameParallelGlobalAxes(float[] checkPoint, string location)
        {
            var doubleEllips = (DoubleEllips)new EllipsBuilder().
                SetWidth(14).
                SetDepth(5).
                SetFrontPart(5).
                SetBackPart(15).
    SetFrame(new Frame(new Point3D(15, 15, 15),
    new Point3D(16, 15, 15),
    new Point3D(15, 16, 15)));

            doubleEllips.MaxAndMinSemiaxis();

            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            if (location == "inside")
                Assert.IsTrue(doubleEllips.IsPointInside(point));
            else
                Assert.IsFalse(doubleEllips.IsPointInside(point));

        }

        [TestMethod]
        [DataRow(new[] { 25f, 22.2f, 19.5f }, "inside", DisplayName = "25f, 22.2f, 19.5f")]
        [DataRow(new[] { 25f, 24.3f, 20f }, "inside", DisplayName = "25f, 24.3f, 20f")]
        [DataRow(new[] { 24.7f, 23.7f, 20f }, "inside", DisplayName = "24.7f, 23.7f, 20f")]
        [DataRow(new[] { 25f, 24f, 19.5f }, "inside", DisplayName = "25f, 24.0f, 19.5f")]
        [DataRow(new[] { 13f, 25f, 20f }, "outside", DisplayName = "13f, 25f, 20f")]
        [DataRow(new[] { 13f, 20f, 20f }, "outside", DisplayName = "13f, 20f, 20f")]
        public void DoubleEllipsIsPointInside_FrameXYRotated30DegreesGlobalAxes(float[] checkPoint, string location)
        {
            var doubleEllips = (DoubleEllips)new EllipsBuilder().SetWidth(14).SetDepth(5).
                SetFrontPart(5).
                SetBackPart(15).
                SetFrame(new Frame(new Point3D(20f, 20f, 20f),
                new Point3D(20.866f, 20.5f, 20f),
                new Point3D(19.5f, 20.866f, 20f)));

            doubleEllips.MaxAndMinSemiaxis();

            //doubleEllips.DetermineInverseMatrix();

            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            if (location == "inside")
                Assert.IsTrue(doubleEllips.IsPointInside(point));
            else
                Assert.IsFalse(doubleEllips.IsPointInside(point));
        }


        [TestMethod]
        [DataRow(2, 5, 10)]
        [DataRow(7.5f, 5, 10)]
        public void IsOverlappingSelfEllips_ShouldPass_WhenOverlap(float velocity, float frontPart, float backPart)
        {
            var doubleEllips = (DoubleEllips)new EllipsBuilder().
    SetFrontPart(frontPart).
    SetBackPart(backPart);

            Assert.IsTrue(doubleEllips.IsOverlappingSelf(velocity));
        }

        [TestMethod]
        [DataRow(
    new float[]
    { 1f, 1f, 0f,
       0f, 0f, 1f
    },
    new float[]
    { 0.707107f, 0.707107f,  0f,
       0f,   0f,    1f,
       0.707107f, -0.707107f, 0f
    })]
        public void DetermineInverseMatrixTest(float[] pointsArr, float[] resultArr)
        {
            var doubleEllips = (DoubleEllips)new EllipsBuilder().
                SetFrame(new Frame(
                new Point3D(0f, 0f, 0f),
                 new Point3D(pointsArr[0], pointsArr[1], pointsArr[2]),
                 new Point3D(pointsArr[3], pointsArr[4], pointsArr[5])
                ));


            //doubleEllips.DetermineInverseMatrix();
            var matrix = doubleEllips.LocalFrame.Frame.InverseTransitionMatrix;
            var result = Matrix<float>.Build.DenseOfArray(new float[3, 3] {
                                               {resultArr[0], resultArr[1], resultArr[2]},
                                               {resultArr[3], resultArr[4], resultArr[5]},
                                               {resultArr[6], resultArr[7], resultArr[8]}});

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Assert.AreEqual(result[i, j], matrix[i, j], 1e-6);
                }

            }
        }

        [TestMethod]
        [DataRow(1, 0, 0, 0.75f)]
        [DataRow(4, 4, 4, 0)]
        [DataRow(1, 4, 0.4f, 0)]
        [DataRow(3, 1, 0.8f, 0.75f)]
        public void LazerCalcPowerTest(float x, float y, float z, float power)
        {
            //assing            
            var function = new CustomFunction();
            function.CreateEngine(@"..\..\..\Examples\PythonScript\LaserSourcePython.py");

            //act
            var result = function.CalcValue(new Point3D(x, y, z));

            //assert
            Assert.AreEqual(power, result);
        }

    }
}
