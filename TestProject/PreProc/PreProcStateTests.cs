using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project.TaskParameters;
using Project.Interfaces.Tasks;
using System.Globalization;
using CalculationTaskStatus = Project.TaskParameters.TaskStatus;

namespace TestProject_test.PreProc
{
    [TestClass]
    public class PreProcStateTests
    {
        /// <summary>Проверяет работу с теми же объектами через перечисление и операции очереди.</summary>
        [TestMethod]
        public void Queue_ExposesAndRemovesCurrentParameters()
        {
            var preprocessor = new global::PreProc.PreProc();
            var first = new TermalParameters { Index = 1 };
            var second = new TermalParameters { Index = 2 };
            preprocessor.Load(new GeneralParameters[] { first, second });

            Assert.AreSame(first, preprocessor.First());
            first.Status = global::Project.TaskParameters.TaskStatus.пропустить;
            Assert.AreEqual(global::Project.TaskParameters.TaskStatus.пропустить, preprocessor.First().Status);
            Assert.IsTrue(preprocessor.Remove(first));
            CollectionAssert.AreEqual(new GeneralParameters[] { second }, preprocessor.ToList());
            preprocessor.Clear();
            Assert.IsFalse(preprocessor.Any());
        }

        /// <summary>Проверяет запись и повторное чтение всей очереди со статусами.</summary>
        [TestMethod]
        [DataRow("ru-RU")]
        [DataRow("en-US")]
        public void WriteInstructions_RoundTripsAllParameters(string culture)
        {
            var originalCulture = CultureInfo.CurrentCulture;
            var root = Path.Combine(Path.GetTempPath(), $"BazisPreProcState_Расчёт с пробелами_{Guid.NewGuid():N}");
            Directory.CreateDirectory(root);
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo(culture);
                var project = Path.Combine(root, "model with spaces.bpf");
                File.WriteAllText(project, "Project fixture placeholder.");
                var first = new TermalParameters
                {
                    Index = 0,
                    TimeSettings = new TimeSettings { StartTime = 0, StopTime = 1 },
                    SolverSettings = new SolverSettings(LinearSolverKind.BiCGStab, MatrixStorageKind.GeneralCsr)
                };
                var skipped = new MechanicalParameters
                {
                    Index = 1,
                    TimeSettings = new TimeSettings { StartTime = 1, StopTime = 2 },
                    SolverSettings = new SolverSettings(LinearSolverKind.BiCGStab, MatrixStorageKind.GeneralCsr),
                    Status = CalculationTaskStatus.пропустить
                };
                var preprocessor = new global::PreProc.PreProc();
                preprocessor.Load(new GeneralParameters[] { skipped, first });

                var commandFile = preprocessor.WriteInstructions(project);
                var lines = File.ReadAllLines(commandFile);
                Assert.AreEqual(3, lines.Length);
                Assert.AreEqual("загрузить проект \"../model with spaces.bpf\"", lines[0]);
                Assert.AreEqual("расчет механическая \"../InputData/механическая_1_1_2.tsf\" пропустить", lines[1]);
                Assert.AreEqual("расчет термическая \"../InputData/термическая_0_0_1.tsf\" выполнить", lines[2]);
                Assert.IsFalse(lines.Any(line => line.Contains('\\')));
                var bytes = File.ReadAllBytes(commandFile);
                Assert.IsFalse(bytes.Length >= 3 && bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf);
                Assert.IsTrue(File.Exists(Path.Combine(root, "InputData", "термическая_0_0_1.tsf")));
                Assert.IsTrue(File.Exists(Path.Combine(root, "InputData", "механическая_1_1_2.tsf")));

                var restored = new global::PreProc.PreProc();
                restored.LoadInstructions(project);
                var parameters = restored.ToList();
                Assert.AreEqual(2, parameters.Count);
                Assert.IsInstanceOfType<MechanicalParameters>(parameters[0]);
                Assert.IsInstanceOfType<TermalParameters>(parameters[1]);
                Assert.AreEqual(1, parameters[0].Index);
                Assert.AreEqual(0, parameters[1].Index);
                Assert.AreEqual(CalculationTaskStatus.пропустить, parameters[0].Status);
                Assert.AreEqual(CalculationTaskStatus.выполнить, parameters[1].Status);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
                Directory.Delete(root, true);
            }
        }

        /// <summary>Некорректные объекты очереди не заменяют готовый TCF.</summary>
        [TestMethod]
        [DataRow("duplicate")]
        [DataRow("invalid-status")]
        [DataRow("invalid-time")]
        [DataRow("invalid-index")]
        [DataRow("empty")]
        public void WriteInstructions_InvalidQueuePreservesCommandFile(string failure)
        {
            var root = Path.Combine(Path.GetTempPath(), $"BazisPreProcInvalid_{Guid.NewGuid():N}");
            Directory.CreateDirectory(root);
            try
            {
                var project = Path.Combine(root, "model.bpf");
                File.WriteAllText(project, "Project fixture placeholder.");
                var valid = new TermalParameters
                {
                    Index = 0,
                    TimeSettings = new TimeSettings { StartTime = 0, StopTime = 1 },
                    SolverSettings = new SolverSettings(LinearSolverKind.BiCGStab, MatrixStorageKind.GeneralCsr)
                };
                var preprocessor = new global::PreProc.PreProc();
                preprocessor.Add(valid);
                var commandFile = preprocessor.WriteInstructions(project);
                var original = File.ReadAllBytes(commandFile);
                var second = new TermalParameters
                {
                    Index = failure switch { "duplicate" => 0, "invalid-index" => -1, _ => 1 },
                    TimeSettings = new TimeSettings { StartTime = 1, StopTime = failure == "invalid-time" ? 0 : 2 },
                    SolverSettings = new SolverSettings(LinearSolverKind.BiCGStab, MatrixStorageKind.GeneralCsr),
                    Status = failure == "invalid-status" ? (CalculationTaskStatus)999 : CalculationTaskStatus.выполнить
                };
                if (failure == "empty")
                    preprocessor.Clear();
                else
                    preprocessor.Add(second);

                Assert.ThrowsException<ArgumentException>(() => preprocessor.WriteInstructions(project));
                CollectionAssert.AreEqual(original, File.ReadAllBytes(commandFile));
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }

        /// <summary>Ошибка во второй команде TCF или TSF сохраняет ранее загруженную очередь.</summary>
        [TestMethod]
        [DataRow("missing-status")]
        [DataRow("invalid-tsf")]
        [DataRow("missing-tsf")]
        [DataRow("outside-package")]
        public void LoadInstructions_InvalidSecondTaskPreservesQueue(string failure)
        {
            var root = Path.Combine(Path.GetTempPath(), $"BazisPreProcRead_{Guid.NewGuid():N}");
            Directory.CreateDirectory(root);
            try
            {
                var project = Path.Combine(root, "model.bpf");
                File.WriteAllText(project, "Project fixture placeholder.");
                var parameters = new GeneralParameters[]
                {
                    new TermalParameters { Index = 0, TimeSettings = new TimeSettings { StartTime = 0, StopTime = 1 }, SolverSettings = new SolverSettings(LinearSolverKind.BiCGStab, MatrixStorageKind.GeneralCsr) },
                    new TermalParameters { Index = 1, TimeSettings = new TimeSettings { StartTime = 1, StopTime = 2 }, SolverSettings = new SolverSettings(LinearSolverKind.BiCGStab, MatrixStorageKind.GeneralCsr) }
                };
                var writer = new global::PreProc.PreProc();
                writer.Load(parameters);
                var commandFile = writer.WriteInstructions(project);
                if (failure == "missing-status")
                {
                    var lines = File.ReadAllLines(commandFile);
                    lines[2] = lines[2].Replace(" выполнить", "");
                    File.WriteAllLines(commandFile, lines);
                }
                else if (failure == "invalid-tsf")
                {
                    var taskFile = Path.Combine(root, "InputData", "термическая_1_1_2.tsf");
                    File.WriteAllText(taskFile, "null");
                }
                else if (failure == "missing-tsf")
                {
                    var taskFile = Path.Combine(root, "InputData", "термическая_1_1_2.tsf");
                    File.Delete(taskFile);
                }
                else
                {
                    var lines = File.ReadAllLines(commandFile);
                    lines[2] = "расчет термическая \"../../outside.tsf\" выполнить";
                    File.WriteAllLines(commandFile, lines);
                }

                var restored = new global::PreProc.PreProc();
                var existing = new TermalParameters { Index = 7 };
                restored.Add(existing);
                if (failure == "missing-status")
                    Assert.ThrowsException<FormatException>(() => restored.LoadInstructions(project));
                else if (failure == "missing-tsf")
                    Assert.ThrowsException<FileNotFoundException>(() => restored.LoadInstructions(project));
                else if (failure == "outside-package")
                    Assert.ThrowsException<ArgumentException>(() => restored.LoadInstructions(project));
                else
                    Assert.ThrowsException<InvalidDataException>(() => restored.LoadInstructions(project));
                CollectionAssert.AreEqual(new GeneralParameters[] { existing }, restored.ToList());
            }
            finally
            {
                Directory.Delete(root, true);
            }
        }
    }
}
