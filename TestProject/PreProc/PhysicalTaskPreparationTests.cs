using MaterialDB.MaterialData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GroupsData;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using Project.TaskParameters;
using Project.Tasks;
using Project.Tasks.Functions;
using Project.Tasks.Functions.FrameFunctions;
using Project.Tasks.LocalFrames;
using System.Data;

namespace TestProject_test.PreProc
{
    [TestClass]
    public class PhysicalTaskPreparationTests
    {
        /// <summary>
        /// Форма источника задает шаг по своему размеру вдоль движения.
        /// </summary>
        [DataTestMethod]
        [DataRow("SPH", 2f)]
        [DataRow("CIL", 3f)]
        [DataRow("DEL", 2f)]
        public void CalcMaxTimeStep_UsesSourceLengthAlongMotion(string shape, float expected)
        {
            FrameFunction source = shape switch
            {
                "SPH" => new SPH { Width = 12 },
                "CIL" => new CIL { UpperDiam = 18, BottomDiam = 2, Length = 40 },
                "DEL" => new DEL { FrontPart = 4, BackPart = 8 },
                _ => throw new ArgumentOutOfRangeException(nameof(shape))
            };

            var actual = source.CalcMaxTimeStep(2f);
            Assert.AreEqual(expected, actual, 0.00001f);
        }

        /// <summary>
        /// Препроцессор использует активный материал и движение источника на каждом интервале.
        /// </summary>
        [DataTestMethod]
        [DataRow(false, 1f)]
        [DataRow(true, 2f)]
        public void CalcCompDataV2_UsesActiveMaterialsAndSourceStep(bool moving, float expectedMaxStep)
        {
            var group = new Group("material-group", ObjType.Узел);
            var firstMaterial = new MaterialDBItem("first");
            firstMaterial.AddMetallurgicalProps();
            firstMaterial[CategoryEnum.Metallurgical].PropertyData.Add("Нагрев", new Property("Нагрев", "", "", "", new DataTable()));
            var secondMaterial = new MaterialDBItem("second");
            secondMaterial.AddMetallurgicalProps();
            secondMaterial[CategoryEnum.Metallurgical].PropertyData.Add("Охлаждение", new Property("Охлаждение", "", "", "", new DataTable()));

            var heat = new HeatData(group, 0f, 4f)
            {
                Function = new SPH { Width = 12 },
                LocalFrame = moving ? new MovedFrame { Velocity = 2f } : new StaticFrame()
            };
            var conditions = new ICondData[]
            {
                new MatData(firstMaterial, group, 0f, 4f),
                new MatData(secondMaterial, group, 2f, 4f),
                new MediaData(group, 0f, 4f),
                heat
            };

            var preprocessor = new global::PreProc.PreProc();
            var parameters = preprocessor.CalcCompDataV2(conditions, TaskKind.термическая);

            Assert.AreEqual(2, parameters.Count);
            var first = (TermalParameters)parameters[0];
            var second = (TermalParameters)parameters[1];
            Assert.AreEqual(expectedMaxStep, first.TimeSettings.MaxTimeStep);
            Assert.AreEqual(expectedMaxStep / 2f, first.TimeSettings.InitTimeStep);
            Assert.AreEqual(expectedMaxStep, second.TimeSettings.MaxTimeStep);
            CollectionAssert.AreEqual(new[] { "Нагрев" }, first.MetallurgicalProcesses);
            CollectionAssert.AreEqual(new[] { "Нагрев", "Охлаждение" }, second.MetallurgicalProcesses);
        }

        /// <summary>
        /// Связанные задачи получают общий шаг только при включенном флаге синхронизации.
        /// </summary>
        [DataTestMethod]
        [DataRow(false, 0d, 2f, 1f)]
        [DataRow(true, 0d, 2f, 2f)]
        [DataRow(true, 3d, 0.5f, 0.5f)]
        public void CalcCompDataV2_SynchronizesLinkedTaskStepsWhenRequested(bool synchronize, double mechanicalWidth, float expectedThermal, float expectedMechanical)
        {
            var group = new Group("material-group", ObjType.Узел);
            var material = new MaterialDBItem("material");
            var heat = new HeatData(group, 0f, 4f)
            {
                Function = new SPH { Width = 12 },
                LocalFrame = new MovedFrame { Velocity = 2f }
            };
            var load = new LoadData(group, 0f, 4f);
            if (mechanicalWidth > 0)
            {
                load.Function = new SPH { Width = mechanicalWidth };
                load.LocalFrame = new MovedFrame { Velocity = 2f };
            }

            var conditions = new ICondData[]
            {
                new MatData(material, group, 0f, 4f),
                new MediaData(group, 0f, 4f),
                new ClampData(group, 0f, 4f),
                heat,
                load
            };

            var preprocessor = new global::PreProc.PreProc();
            var taskKinds = TaskKind.термическая | TaskKind.механическая;
            var parameters = preprocessor.CalcCompDataV2(conditions, taskKinds, synchronize);

            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual(expectedThermal, parameters[0].TimeSettings.MaxTimeStep);
            Assert.AreEqual(expectedThermal / 2f, parameters[0].TimeSettings.InitTimeStep);
            Assert.AreEqual(expectedMechanical, parameters[1].TimeSettings.MaxTimeStep);
            Assert.AreEqual(expectedMechanical / 2f, parameters[1].TimeSettings.InitTimeStep);
        }

        /// <summary>
        /// Общий шаг связанных задач пересчитывается для каждого временного интервала.
        /// </summary>
        [TestMethod]
        public void CalcCompDataV2_SynchronizesEachLinkedIntervalSeparately()
        {
            var group = new Group("material-group", ObjType.Узел);
            var material = new MaterialDBItem("material");
            var movingHeat = new HeatData(group, 0f, 2f)
            {
                Function = new SPH { Width = 12 },
                LocalFrame = new MovedFrame { Velocity = 2f }
            };
            var staticHeat = new HeatData(group, 2f, 4f)
            {
                Function = new SPH { Width = 12 },
                LocalFrame = new StaticFrame()
            };
            var conditions = new ICondData[]
            {
                new MatData(material, group, 0f, 4f),
                new MediaData(group, 0f, 4f),
                new ClampData(group, 0f, 4f),
                new LoadData(group, 0f, 4f),
                movingHeat,
                staticHeat
            };

            var preprocessor = new global::PreProc.PreProc();
            var taskKinds = TaskKind.термическая | TaskKind.механическая;
            var parameters = preprocessor.CalcCompDataV2(conditions, taskKinds, true);

            Assert.AreEqual(4, parameters.Count);
            Assert.AreEqual(2f, parameters[0].TimeSettings.MaxTimeStep);
            Assert.AreEqual(2f, parameters[1].TimeSettings.MaxTimeStep);
            Assert.AreEqual(1f, parameters[0].TimeSettings.InitTimeStep);
            Assert.AreEqual(1f, parameters[1].TimeSettings.InitTimeStep);
            Assert.AreEqual(1f, parameters[2].TimeSettings.MaxTimeStep);
            Assert.AreEqual(1f, parameters[3].TimeSettings.MaxTimeStep);
            Assert.AreEqual(0.5f, parameters[2].TimeSettings.InitTimeStep);
            Assert.AreEqual(0.5f, parameters[3].TimeSettings.InitTimeStep);
        }
    }
}
