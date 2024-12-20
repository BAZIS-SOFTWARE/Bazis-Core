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
using Project.Interfaces.Tasks;

namespace TestProject
{
    [TestClass]
    public class TestMFF
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
 

        [TestMethod]
        [DataRow(@"..\..\Examples\LWTest.bpf", DisplayName = "LW")]
        [DataRow(@"..\..\Examples\ARCTest.bpf", DisplayName = "ARC")]
        [DataRow(@"..\..\Examples\WeldingHeatSource\FSW\proj.bpf", DisplayName = "FSW")]
        public void MFF_EnergyCalculated(string path)
        {
            //assign
            var proj = new ProjectData();
            proj.ModelData = new ModelData();
            proj.TaskData = new TaskData();

            //act
            proj.Load(path);

            var lazerData = proj.TaskData.Find("Нагрев").Select(x => (HeatData)x).ToArray();
            for (var i = 0; i < lazerData.Count(); i++)
            {
                var heatData = lazerData[i];
                var gr = heatData.Group;

                if (heatData.FrameFunction is FSWPin fswPin)
                    fswPin.GetParameters().ElementAt(0).Value = 200;
                if (heatData.FrameFunction is FSWShoulder fswShoulder)
                    fswShoulder.GetParameters().ElementAt(0).Value = 0.001f;

                var powerFunc = (IPowerFunction)heatData.FrameFunction;
                //assert
                var energy = new float[gr.Count];
                var frame = heatData.MovedFrame.CalcFrame(1);
                var counter = 0;
                foreach (var obj in gr)
                {
                    energy[counter] = powerFunc.CalcPower(frame, obj.CalcCentr());
                    counter++;
                }
                var max = energy.Max();
                if (max == 0)
                    throw new Exception("Энергия не вводится!");
            }
        }

        //ARC//
        [DataRow(new[] { -10.0000001f, 0f, 0f })]
        [DataRow(new[] { 0f, -10.0000001f, 0f })]
        [DataRow(new[] { 0f, 0f, -10.0000001f })]
        [DataRow(new[] { 10.0000001f, 0f, 0f })]
        [DataRow(new[] { 0f, 10.0000001f, 0f })]
        [DataRow(new[] { 0f, 0f, 10.0000001f })]
        [DataRow(new[] { 9.99f, 0f, 0f })]
        [DataRow(new[] { 0f, 9.99f, 0f })]
        [DataRow(new[] { 0f, 0f, 9.99f })]
        [DataRow(new[] { 10f, 0f, 0f })]
        [DataRow(new[] { 0f, 10f, 0f })]
        [DataRow(new[] { 0f, 0f, 10f })]
        [TestMethod]
        public void SphereFuncIsPointInside_ShouldPass_WhenPointInsideSphere(float[] checkPoint)
        {
            var arc = new ArcBuilder();
            arc.SetWidth("20");

            var frame = new Frame(new Point3D(5f, 0f, 0f), new Point3D(0f, 5f, 0f), new Point3D(0f, 0f, 5f), new Point3D(0f, 0f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsTrue(((Arc)arc).IsPointInside(frame, point, out _, out _));
        }


        [TestMethod]
        [DataRow(new[] { -1f, -1f, -1f })]
        [DataRow(new[] { -5f, -5f, -5f })]
        [DataRow(new[] { 0f, 0f, 0f })]
        [DataRow(new[] { -9f, 2f, 0f })]
        public void SphereFuncIsPointInside_ShouldPass_WhenHasEdgeCollision(float[] checkPoint)
        {
            var arc = new ArcBuilder();
            arc.SetWidth("20");

            var frame = new Frame(new Point3D(5f, 0f, 0f), new Point3D(0f, 5f, 0f), new Point3D(0f, 0f, 5f), new Point3D(0f, 0f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsTrue(((Arc)arc).IsPointInside(frame, point, out _, out _));
        }


        [DataTestMethod]
        [DataRow(new[] { 10.1f, 0f, 0f })]
        [DataRow(new[] { 0f, 10.1f, 0f })]
        [DataRow(new[] { 0f, 0f, 10.1f })]
        [DataRow(new[] { 12f, 6f, 5f })]
        [DataRow(new[] { -10.1f, 0f, 0f })]
        [DataRow(new[] { 0f, -10.1f, 0f })]
        [DataRow(new[] { 0f, 0f, -10.1f })]
        public void SphereFuncIsPointInside_ShouldFail_WhenNoCollision(float[] checkPoint)
        {
            var arc = new ArcBuilder();
            arc.SetWidth("20");

            var frame = new Frame(new Point3D(5f, 0f, 0f), new Point3D(0f, 5f, 0f), new Point3D(0f, 0f, 5f), new Point3D(0f, 0f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsFalse(((Arc)arc).IsPointInside(frame, point, out _, out _));
        }

        //Lazer//

        [TestMethod]
        [DataRow(new[] { 2f, 5f, 0f })]
        [DataRow(new[] { 0f, 5f, -3f })]
        [DataRow(new[] { 0f, 5f, -6f })]
        public void LazerFuncIsPointInside_ShouldPass_WhenPointInsideCillinder(float[] checkPoint)
        {
            var lazer = new LazerBuilder();
            lazer.SetLength("10");
            lazer.SetBottomDiam("10");
            lazer.SetUpperDiam("20");

            var frame = new Frame(new Point3D(5f, 0f, 0f), new Point3D(0f, 5f, 0f), new Point3D(0f, 0f, 5f), new Point3D(0f, 0f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsTrue(((Lazer)lazer).IsPointInside(frame, point, out _, out _, out _, out _));
        }


        [TestMethod]
        [DataRow(new[] { 4.99f, 0f, 0f })]
        [DataRow(new[] { 9.99f, 0f, 0f })]
        [DataRow(new[] { 0f, 0f, -4.99f })]
        [DataRow(new[] { 0f, 0f, -9.99f })]
        [DataRow(new[] { 4.99f, 5f, 0f })]
        [DataRow(new[] { 0f, 5f, -4.99f })]
        [DataRow(new[] { 0f, 5f, -9.99f })]
        public void LazerFuncIsPointInside_ShouldPass_WhenHasEdgeCollision(float[] checkPoint)
        {
            var lazer = new LazerBuilder();
            lazer.SetLength("10");
            lazer.SetBottomDiam("10");
            lazer.SetUpperDiam("20");

            var frame = new Frame(new Point3D(5f, 0f, 0f), new Point3D(0f, 5f, 0f), new Point3D(0f, 0f, 5f), new Point3D(0f, 0f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsTrue(((Lazer)lazer).IsPointInside(frame, point, out _, out _, out _, out _));
        }


        [TestMethod]
        [DataRow(new[] { 4.99f, 10f, 0f })]
        [DataRow(new[] { 9.99f, 10f, 0f })]
        [DataRow(new[] { 0f, 10f, -4.99f })]
        [DataRow(new[] { 0f, 10f, -9.99f })]
        [DataRow(new[] { 9.99f, 5f, 0f })]
        [DataRow(new[] { 4.99f, -10f, 0f })]
        [DataRow(new[] { 9.99f, -10f, 0f })]
        public void LazerFuncIsPointInside_ShouldFail_WhenNoCollision(float[] checkPoint)
        {
            var lazer = new LazerBuilder();
            lazer.SetLength("10");
            lazer.SetBottomDiam("10");
            lazer.SetUpperDiam("20");

            var frame = new Frame(new Point3D(10f, 0f, 0f), new Point3D(0f, 10f, 0f), new Point3D(0f, 0f, 10f), new Point3D(0f, 0f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsFalse(((Lazer)lazer).IsPointInside(frame, point, out _, out _, out _, out _));
        }

        //FSWShoulder//
        [TestMethod]
        [DataRow(new[] { 2f, 5f, 0f })]
        [DataRow(new[] { 0f, 5f, -3f })]
        [DataRow(new[] { 0f, 5f, -6f })]
        [DataRow(new[] { 0f, 10f, -9.99f })]
        [DataRow(new[] { 0f, 10f, -4.99f })]
        public void FSWShoulderFuncIsPointInside_ShouldPass_WhenPointInsideCillinder(float[] checkPoint)
        {
            var fswShoulder = new FSWShoulderBuilder();
            fswShoulder.SetLength("10");
            fswShoulder.SetBottomDiam("20");
            fswShoulder.SetUpperDiam("20");

            var frame = new Frame(new Point3D(5f, 1f, 0f), new Point3D(0f, 6f, 0f), new Point3D(0f, 1f, 5f), new Point3D(0f, 1f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsTrue(((FSWShoulder)fswShoulder).IsPointInside(frame, point, out _, out _, out _, out _));
        }


        [TestMethod]
        [DataRow(new[] { 4.99f, 0f, 0f })]
        [DataRow(new[] { 0f, 0f, -4.99f })]
        [DataRow(new[] { 0f, 0f, -9.99f })]
        [DataRow(new[] { 4.99f, 5f, 0f })]
        [DataRow(new[] { 0f, 5f, -4.99f })]
        [DataRow(new[] { 0f, 5f, -9.99f })]
        public void FSWShoulderFuncIsPointInside_ShouldPass_WhenHasEdgeCollision(float[] checkPoint)
        {
            var fswShoulder = new FSWShoulderBuilder();
            fswShoulder.SetLength("10");
            fswShoulder.SetBottomDiam("20");
            fswShoulder.SetUpperDiam("20");

            var frame = new Frame(new Point3D(5f, 1f, 0f), new Point3D(0f, 6f, 0f), new Point3D(0f, 1f, 5f), new Point3D(0f, 1f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsTrue(((FSWShoulder)fswShoulder).IsPointInside(frame, point, out _, out _, out _, out _));
        }


        [TestMethod]
        [DataRow(new[] { 4.99f, 10f, 0f })]
        [DataRow(new[] { 9.99f, 10f, 0f })]
        [DataRow(new[] { 9.99f, 0f, 0f })]
        [DataRow(new[] { 9.99f, 5f, 0f })]
        [DataRow(new[] { 4.99f, -10f, 0f })]
        [DataRow(new[] { 9.99f, -10f, 0f })]
        public void FSWShoulderFuncIsPointInside_ShouldFail_WhenNoCollision(float[] checkPoint)
        {
            var fswShoulder = new FSWShoulderBuilder();
            fswShoulder.SetLength("10");
            fswShoulder.SetBottomDiam("20");
            fswShoulder.SetUpperDiam("20");

            var frame = new Frame(new Point3D(5f, 1f, 0f), new Point3D(0f, 6f, 0f), new Point3D(0f, 1f, 5f), new Point3D(0f, 1f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsFalse(((FSWShoulder)fswShoulder).IsPointInside(frame, point, out _, out _, out _, out _));
        }

        //FSWPin//

        [TestMethod]
        [DataRow(new[] { 2f, 5f, 0f })]
        [DataRow(new[] { 0f, 5f, -3f })]
        [DataRow(new[] { 0f, 5f, -6f })]
        public void FSWPinFuncIsPointInside_ShouldPass_WhenPointInsideCillinder(float[] checkPoint)
        {
            var fswPin = new FSWPinBuilder();
            fswPin.SetLength("10");
            fswPin.SetBottomDiam("10");
            fswPin.SetUpperDiam("20");

            var frame = new Frame(new Point3D(5f, 0f, 0f), new Point3D(0f, 5f, 0f), new Point3D(0f, 0f, 5f), new Point3D(0f, 0f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsTrue(((FSWPin)fswPin).IsPointInside(frame, point, out _, out _, out _, out _));
        }

        [TestMethod]
        [DataRow(new[] { 4.99f, 0f, 0f })]
        [DataRow(new[] { 9.99f, 0f, 0f })]
        [DataRow(new[] { 0f, 0f, -4.99f })]
        [DataRow(new[] { 0f, 0f, -9.99f })]
        [DataRow(new[] { 4.99f, 5f, 0f })]
        [DataRow(new[] { 0f, 5f, -4.99f })]
        [DataRow(new[] { 0f, 5f, -9.99f })]
        public void FSWPinFuncIsPointInside_ShouldPass_WhenHasEdgeCollision(float[] checkPoint)
        {
            var fswPin = new FSWPinBuilder();
            fswPin.SetLength("10");
            fswPin.SetBottomDiam("10");
            fswPin.SetUpperDiam("20");

            var frame = new Frame(new Point3D(5f, 0f, 0f), new Point3D(0f, 5f, 0f), new Point3D(0f, 0f, 5f), new Point3D(0f, 0f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsTrue(((FSWPin)fswPin).IsPointInside(frame, point, out _, out _, out _, out _));
        }


        [TestMethod]
        [DataRow(new[] { 4.99f, 10f, 0f })]
        [DataRow(new[] { 9.99f, 10f, 0f })]
        [DataRow(new[] { 0f, 10f, -4.99f })]
        [DataRow(new[] { 0f, 10f, -9.99f })]
        [DataRow(new[] { 9.99f, 5f, 0f })]
        [DataRow(new[] { 4.99f, -10f, 0f })]
        [DataRow(new[] { 9.99f, -10f, 0f })]
        public void FSWPinFuncIsPointInside_ShouldFail_WhenNoCollision(float[] checkPoint)
        {
            var fswPin = new FSWPinBuilder();
            fswPin.SetLength("10");
            fswPin.SetBottomDiam("10");
            fswPin.SetUpperDiam("20");

            var frame = new Frame(new Point3D(5f, 0f, 0f), new Point3D(0f, 5f, 0f), new Point3D(0f, 0f, 5f), new Point3D(0f, 0f, 0f));
            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            Assert.IsFalse(((FSWPin)fswPin).IsPointInside(frame, point, out _, out _, out _, out _));
        }

        [DataRow(
            new float[]
            {
                0f, 0f, 0f,
                -1f, -1f, 0f,
                -2f, 2f, 0f,
                -2f, 2f, 1f,

                0f, 0f, 1f,
                -2f, -2f, 0f,
                2f, 3f, 0f,
                2f, 3f, 1f,

                0f, 6f, 0f,
                0f, 6f, 1f,
                4f, 4f, 0f,
                4f, 6f, 0f
            },
            new float[]
            {
                0f, 0f, 0f,
                -1f, -1f, 0f,
                -2f, 2f, 0f,
                -2f, 2f, 1f,

                0f, 0f, 1f,
                -2f, -2f, 0f,
                2f, 3f, 0f,
                2f, 3f, 1f,
            })]
        [DataRow(
            new float[]
            {
                0f, 0f, 0f,
                0f, 0f, 1f,
                2f, 0f, 0f,
                2f, 1f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                2f, 1f, 0f,
                1f, 2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                1f, 2f, 0f,
                0f, 2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                0f, 2f, 0f,
                -1f, 2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                -1f, 2f, 0f,
                -2f, 1f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                -2f, 1f, 0f,
                -2f, 0f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                -2f, -1f, 0f,
                -1f, -2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                -1f, -2f, 0f,
                0f, -2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                0f, -2f, 0f,
                1f, -2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                1f, -2f, 0f,
                2f, -1f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                2f, -1f, 0f,
                2f, 0f, 0f
            },
            new float[]
            {
                0f, 0f, 0f,
                0f, 0f, 1f,
                2f, 0f, 0f,
                2f, 1f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                2f, 1f, 0f,
                1f, 2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                1f, 2f, 0f,
                0f, 2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                0f, 2f, 0f,
                -1f, 2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                -1f, 2f, 0f,
                -2f, 1f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                -2f, 1f, 0f,
                -2f, 0f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                -2f, -1f, 0f,
                -1f, -2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                -1f, -2f, 0f,
                0f, -2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                0f, -2f, 0f,
                1f, -2f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                1f, -2f, 0f,
                2f, -1f, 0f,

                0f, 0f, 0f,
                0f, 0f, 1f,
                2f, -1f, 0f,
                2f, 0f, 0f
            })]
        [TestMethod]
        public void SphereIsPointInside_ShouldPass_WhenSelectNecessaryNodes(float[] input, float[] expected)
        {
            var setup = SetUpTest(input, expected);
            var expectedElements = setup.Item1;
            var resultList = setup.Item2;

            Assert.IsTrue(expectedElements.Count == resultList.Count);
            for(var i = 0; i < expectedElements.Count; i++)
            {
                Assert.AreEqual(expectedElements.ToString(), resultList.ToString());
                for (var j = 0; j < 4; j++)
                    Assert.AreEqual(expectedElements[i][j].ToString(), resultList[i][j].ToString());
            }
        }

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
            var heatData = new HeatData(new Group("",ObjType.Объект), "ARC;10;100;25 Load 0 49 baseLine|refLine;5;startNodes;startNodes");

            var baseLine = new Group("base", ObjType.Узел);
            var refLine = new Group("ref", ObjType.Узел);

            var start = new Group("start", ObjType.Узел);
            start.AddRange(new[] { new Node(10000, new Point3D(0f, 0f, 0f)), new Node(10001, new Point3D(0f, 0f, 1f)) });

            for (var i = 0; i < heatData.StopTime; i++)
            {
                baseLine.Add(new Node(i, new Point3D(0f, 0f, i)));
                refLine.Add(new Node(i + 5000, new Point3D(0f, -1f, i)));
            }

            var mf = heatData.MovedFrame;
            mf.BaseLine = baseLine;
            mf.RefLine = refLine;
            mf.StartPoints = start;
            mf.StopPoints = start;

            var expectedElements = new List<IElement>();
            var inputElements = new List<IElement>();

            if (expected.Count() >= 12)
                expectedElements = GetTetraElements(expected);
            if (input.Count() >= 12)
                inputElements = GetTetraElements(input);
            var resultList = new List<IElement>();

            var end = mf.CalcMotionTime();

            for (var time = 0f; time < end; time += mf.Velosity)
            {
                var frame = mf.CalcFrame(time);
                resultList.AddRange(heatData.FrameFunction.GetIntersectedObjects(frame, inputElements));
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
            Arc mff = new ArcBuilder().SetWidth(width.ToString());
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
            Lazer mff = new LazerBuilder().SetBottomDiam(bottom.ToString()).SetUpperDiam(upper.ToString());
            Assert.IsTrue(mff.IsOverlappingSelf(velocity));
        }


    }
}
