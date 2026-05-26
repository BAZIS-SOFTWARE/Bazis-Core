using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Globalization;
using Geometry;
using Model.MeshObjects;
using Model.GroupsData;
using Model.Interfaces.MeshObjects;
using Project.Tasks;
using Model.Interfaces;
using Project.Tasks.LocalFrames;
using Project.Interfaces.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Project.IO;
using Project.Tasks.Functions;
using Project.Tasks.Functions.FrameFunctions;
using MathNet.Numerics.Statistics;

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
        //[DataRow(@"..\..\..\Examples\WeldingHeatSource\LWTest_v.bpf", DisplayName = "LWTest_v.bpf")]
        [DataRow(@"..\..\..\Examples\WeldingHeatSource\test_t_lazer_0_10.bpf", DisplayName = "test_t_lazer_0_10.bpf")]
        [DataRow(@"..\..\..\Examples\WeldingHeatSource\test_t_lazer_180_0_10.bpf", DisplayName = "test_t_lazer_180_0_10.bpf")]
        [DataRow(@"..\..\..\Examples\WeldingHeatSource\ARCTest_v.bpf", DisplayName = "ARCTest_v")]
        //[DataRow(@"\WeldingHeatSource\FSW\proj.bpf", DisplayName = "FSW")]
        [DataRow("c:\\projs\\testProj\\HeatingByWelding_Tjoint\\proj.bpf2", DisplayName = "c:\\projs\\testProj\\HeatingByWelding_Tjoint\\proj.bpf2")]

        public void MFF_EnergyCalculated(string path)
        {
            //assing
            var loader = new LoadProjectFromTextFormat();
            loader.LoadEvent += (ar1, ar2) => { };
            var proj = loader.Load($@"{path}");

            //act
            var lazerData = proj.TaskData.Find(DataKind.Нагрев).Select(x => (HeatData)x).ToArray();
            for (var i = 0; i < lazerData.Count(); i++)
            {
                var heatData = lazerData[i];
                var gr = heatData.Group;

                //heatData.Function["TIME"].SetValue(1);
                var energy = new double[gr.Count];
                //assert
                (heatData.LocalFrame as MovedFrame).Time = 1;
                heatData.LocalFrame.CalcPosition();
                var counter = 0;

                //var heat = heatData.Value;

                foreach (var obj in gr)
                {
                    var heat = heatData.Value;
                    var centr = obj.CalcCentr();
                    var frameCoords = heatData.LocalFrame.Frame.GetCoordsInFrame(centr);

                    heatData.Function["X"].SetValue(frameCoords._x);
                    heatData.Function["Y"].SetValue(frameCoords._y);
                    heatData.Function["Z"].SetValue(frameCoords._z);
                    
                    heat *= heatData.Function.CalcValue();
                    energy[counter] = heat;
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
        [DataRow("FSWShoulder", new[] { 0f, 10f, -9.99f }, 0, DisplayName = "FSWShoulder_NoCollision")]
        [DataRow("FSWShoulder", new[] { 0f, 10f, -4.99f }, 0, DisplayName = "FSWShoulder_NoCollision")]
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
            FrameFunction pFunction;
            if (fName == "arc")
            {
                var arc = new SphereBuilder();
                arc.SetWidth("20");
                pFunction = arc;
            }
            else
            {
                var lazer = new CillindricalBuilder();
                lazer.SetLength("10");
                lazer.SetBottomDiam("10");
                lazer.SetUpperDiam("20");

                pFunction = lazer;
            }

            //}

            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);
            //pFunction.
            var list = new List<Node>() { new Node(0, point) };
            Assert.AreEqual(pFunction.GetIntersectedObjects(list).Count, res);
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

            var func = (SPH)new SphereBuilder().SetWidth("10");
            var localFrame = new MovedFrame(baseLine, refLine, 5);
            localFrame.CalcPosition();
            var expectedElements = new List<IElement>();
            var inputElements = new List<IElement>();

            if (expected.Count() >= 12)
                expectedElements = GetTetraElements(expected);
            if (input.Count() >= 12)
                inputElements = GetTetraElements(input);
            var resultList = new List<IElement>();
            
            var end = localFrame.CalcMotionTime();

            for (var time = 0.0f; time < end; time += localFrame.Velocity)
            {
                localFrame.Time = time;
                resultList.AddRange(func.GetIntersectedObjects(inputElements));
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
            SPH mff = new SphereBuilder().SetWidth(width.ToString());
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
            CIL mff = new CillindricalBuilder().SetBottomDiam(bottom.ToString()).SetUpperDiam(upper.ToString());
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
            var doubleEllips = (DEL)new EllipsBuilder().
                SetWidth(14).
                SetDepth(5).
                SetFrontPart(5).
                SetBackPart(15);

            doubleEllips.MaxAndMinSemiaxis();

            var frame = new Frame(new Point3D(15, 15, 15),
new Point3D(16, 15, 15),
new Point3D(15, 16, 15));

            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            var pointInFrame = frame.GetCoordsInFrame(point);

            if (location == "inside")
                Assert.IsTrue(doubleEllips.IsPointInside(pointInFrame));
            else
                Assert.IsFalse(doubleEllips.IsPointInside(pointInFrame));

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
            var doubleEllips = (DEL)new EllipsBuilder().
                SetWidth(14).
                SetDepth(5).
                SetFrontPart(5).
                SetBackPart(15);

            doubleEllips.MaxAndMinSemiaxis();

            var frame = new Frame(new Point3D(20f, 20f, 20f),
                new Point3D(20.866f, 20.5f, 20f),
                new Point3D(19.5f, 20.866f, 20f));

            var point = new Point3D(checkPoint[0], checkPoint[1], checkPoint[2]);

            var pointInFrame = frame.GetCoordsInFrame(point);

            if (location == "inside")
                Assert.IsTrue(doubleEllips.IsPointInside(pointInFrame));
            else
                Assert.IsFalse(doubleEllips.IsPointInside(pointInFrame));
        }


        [TestMethod]
        [DataRow(2, 5, 10)]
        [DataRow(7.5f, 5, 10)]
        public void IsOverlappingSelfEllips_ShouldPass_WhenOverlap(float velocity, float frontPart, float backPart)
        {
            var doubleEllips = (DEL)new EllipsBuilder().
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
            var frame = new Frame(
                new Point3D(0f, 0f, 0f),
                 new Point3D(pointsArr[0], pointsArr[1], pointsArr[2]),
                 new Point3D(pointsArr[3], pointsArr[4], pointsArr[5]));

            //doubleEllips.DetermineInverseMatrix();
            var matrix = frame.InverseTransitionMatrix;
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
        [DataRow(1, 0, 0, 75,@"..\..\..\Examples\PythonScript\RectangleSource.py")]
        [DataRow(4, 4, 4, 0,@"..\..\..\Examples\PythonScript\RectangleSource.py")]
        [DataRow(1, 4, 0.4f, 0,@"..\..\..\Examples\PythonScript\RectangleSource.py")]
        [DataRow(3, 1, 0.8f, 75,@"..\..\..\Examples\PythonScript\RectangleSource.py")]
        [DataRow(1, 0, 0, 882.4969,@"..\..\..\Examples\PythonScript\NormalCircularDistribution.py")]
        [DataRow(4, 4, 4, 0,@"..\..\..\Examples\PythonScript\NormalCircularDistribution.py")]
        [DataRow(-1, -1, 0.4f, 778.8,@"..\..\..\Examples\PythonScript\NormalCircularDistribution.py")]
        [DataRow(-1, -1, -1.1f, 0, @"..\..\..\Examples\PythonScript\NormalCircularDistribution.py")]
        [DataRow(0, 0, 0, 1000,@"..\..\..\Examples\PythonScript\NormalCircularDistribution.py")]
        public void PyFunctionsTest(float x, float y, float z, double power, string path)
        {
            //assing            
            var function = new CustomFrameFunction();
            
            function.CreateEngine(path);

            //act
            var result = function.CalcValue(new Point3D(x, y, z));

            //assert
            Assert.AreEqual(power, result,0.1);
        }

        [DataRow(20, 20, 20,
            15, 15, 15,
            -5, -5, -5)]
        [DataRow(0, 0, 0,
            11, 11, 11,
            11, 11, 11)]
        [DataRow(10, 10, 10,
            0, 0, 0,
            -10, -10, -10)]
        [DataRow(-12, -13, -14,
            14, 13, 12,
            26, 26, 26)]
        [DataRow(12, 13, 14,
            -14, -13, -12,
            -26, -26, -26)]
        [TestMethod]
        public void StaticFrame_GetCoordsInFrame_ShouldPass_WhenPositionIncludeShifting(float shiftX, float shiftY, float shiftZ, float x, float y, float z, float assertX, float assertY, float assertZ)
        {
            var staticFrame = new StaticFrame();
            staticFrame.SetShiftingAndRotation($"{shiftX}|{shiftY}|{shiftZ}|0|0|0");
            var res = staticFrame.GetGlobalCoordsInLocalFrame(new Point3D(x, y, z));

            Assert.AreEqual(assertX, res._x, 1e-4f);
            Assert.AreEqual(assertY, res._y, 1e-4f);
            Assert.AreEqual(assertZ, res._z, 1e-4f);
        }

        [DataRow(90, 0, 0,
            1, 2, 3,
            1, 3, -2)]
        [DataRow(0, 90, 0,
            1, 2, 3,
            -3, 2, 1)]
        [DataRow(0, 0, 90,
            1, 2, 3,
            2, -1, 3)]
        [DataRow(180, 0, 0,
            1, 2, 3,
            1, -2, -3)]
        [DataRow(0, 180, 0,
            1, 2, 3,
            -1, 2, -3)]
        [DataRow(0, 0, 180,
            1, 2, 3,
            -1, -2, 3)]
        [TestMethod]
        public void StaticFrame_GetCoordsInFrame_ShouldPass_WhenPositionIncludeRotation(float rotX, float rotY, float rotZ, float x, float y, float z, float assertX, float assertY, float assertZ)
        {
            var staticFrame = new StaticFrame();
            staticFrame.SetShiftingAndRotation($"0|0|0|{rotX}|{rotY}|{rotZ}");
            var res = staticFrame.GetGlobalCoordsInLocalFrame(new Point3D(x, y, z));

            Assert.AreEqual(assertX, res._x, 1e-4f);
            Assert.AreEqual(assertY, res._y, 1e-4f);
            Assert.AreEqual(assertZ, res._z, 1e-4f);
        }

        [DataRow(10, 20, 30, 45, 0, 0,
            1, 0, 0,
            -9, -20, -30)]
        [DataRow(10, 20, 30, 0, 45, 0,
            0, 2, 0,
            -10, -18, -30)]
        [DataRow(10, 20, 30, 0, 0, 45,
            0, 0, 3,
            -10, -20, -27)]
        [TestMethod]
        public void StaticFrame_GetCoordsInFrame_ShouldPass_WhenPositionIncludeShiftingAndRotation(float shiftX, float shiftY, float shiftZ, float rotX, float rotY, float rotZ, float x, float y, float z, float assertX, float assertY, float assertZ)
        {
            var staticFrame = new StaticFrame();
            staticFrame.SetShiftingAndRotation($"{shiftX}|{shiftY}|{shiftZ}|{rotX}|{rotY}|{rotZ}");
            var res = staticFrame.GetGlobalCoordsInLocalFrame(new Point3D(x, y, z));

            Assert.AreEqual(assertX, res._x, 1e-4f);
            Assert.AreEqual(assertY, res._y, 1e-4f);
            Assert.AreEqual(assertZ, res._z, 1e-4f);
        }
    }
}
