using Microsoft.VisualStudio.TestTools.UnitTesting;
using PreProc;
using PreProc.Interfaces;
using Project.Interfaces.Tasks;
using Project.IO;
using Project.TaskParameters;
using Project.Tasks;
using System.Globalization;
using System.Text;

namespace TestProject_test.PreProc
{
    [TestClass]
    public class PortableInstructionsTests
    {
        private string root = null!;
        private string projectPath = null!;
        private CultureInfo originalCulture = null!;

        /// <summary>
        /// Создаёт изолированный расчётный комплект.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            originalCulture = CultureInfo.CurrentCulture;
            root = Path.Combine(Path.GetTempPath(), $"BazisPortable_Расчёт с пробелами_{Guid.NewGuid():N}");
            Directory.CreateDirectory(root);
            projectPath = Path.Combine(root, "Модель с пробелами.bpf");
            File.WriteAllText(projectPath, "Project fixture placeholder.");
        }

        /// <summary>
        /// Удаляет только собственный временный каталог и восстанавливает культуру.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            CultureInfo.CurrentCulture = originalCulture;
            var temporaryRoot = Path.GetFullPath(Path.GetTempPath());
            var fullRoot = Path.GetFullPath(root);
            var relative = Path.GetRelativePath(temporaryRoot, fullRoot);
            Assert.IsFalse(Path.IsPathRooted(relative));
            Assert.IsTrue(relative.StartsWith("BazisPortable_", StringComparison.Ordinal));
            Assert.IsFalse(relative.Contains(Path.DirectorySeparatorChar));
            if (Directory.Exists(fullRoot))
                Directory.Delete(fullRoot, true);
        }

        /// <summary>
        /// Имя задачи содержит индекс и представление времени, независимое от локали.
        /// </summary>
        [TestMethod]
        [DataRow("ru-RU", 0.1f, 0.25f, "термическая_3_0.100000001_0.25")]
        [DataRow("en-US", 0.1f, 0.25f, "термическая_3_0.100000001_0.25")]
        [DataRow("ru-RU", 0f, 0.25f, "термическая_3_0_0.25")]
        [DataRow("en-US", 0f, 0.25f, "термическая_3_0_0.25")]
        public void GeneralParameters_NameIsPortable(string culture, float start, float stop, string expected)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            var parameters = new GeneralParameters
            {
                TaskKind = TaskKind.термическая,
                Index = 3,
                TimeSettings = new TimeSettings { StartTime = start, StopTime = stop }
            };
            Assert.AreEqual(expected, parameters.ToString());
        }

        /// <summary>
        /// Малые и большие интервалы сохраняют точное значение Single в имени.
        /// </summary>
        [TestMethod]
        [DataRow(1e-20f, 2e-20f)]
        [DataRow(1e20f, 2e20f)]
        public void GeneralParameters_NameSupportsExponent(float start, float stop)
        {
            var parameters = new GeneralParameters
            {
                TaskKind = TaskKind.механическая,
                Index = 10,
                TimeSettings = new TimeSettings { StartTime = start, StopTime = stop }
            };
            var name = parameters.ToString();
            StringAssert.Contains(name, "E");
            var parts = name.Split('_');
            Assert.AreEqual(start, float.Parse(parts[2], CultureInfo.InvariantCulture));
            Assert.AreEqual(stop, float.Parse(parts[3], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Новый метод возвращает существующие параметры в порядке индексов и не пишет файлы.
        /// </summary>
        [TestMethod]
        [DataRow("ru-RU")]
        [DataRow("en-US")]
        public void PrepareParameters_ReturnsOrderedLinkedTasksWithoutFiles(string culture)
        {
            var project = LoadFixtureProject();
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            var preprocessor = new global::PreProc.PreProc();
            var property = new ProcessProperty { TaskKind = TaskKind.термическая | TaskKind.механическая, CommonTaskType = ProcessType.Welding };
            var parameters = preprocessor.CalcCompDataV2(project.TaskData, property);
            Assert.IsTrue(parameters.Count > 2);
            Assert.IsFalse(Directory.Exists(Path.Combine(root, "InputData")));
            Assert.IsFalse(Directory.Exists(Path.Combine(root, "ComputationData")));

            for (var i = 0; i < parameters.Count; i++)
            {
                var item = parameters[i];
                Assert.AreEqual(i / 2 + 1, item.Index);
                Assert.AreEqual(i % 2 == 0 ? TaskKind.термическая : TaskKind.механическая, item.TaskKind);
            }

            var resultNames = parameters.Select(item => $"{item}.db").ToHashSet(StringComparer.Ordinal);
            var references = 0;
            foreach (var item in parameters)
            {
                var fields = new List<PhysicalSet>(item.InputSets);
                if (item.InitialSet != null)
                    fields.Add(item.InitialSet);
                foreach (var field in fields)
                {
                    if (field.Source != PhysicalSetSource.ResultFile)
                        continue;
                    references++;
                    StringAssert.StartsWith(field.File, "../ResultsData/");
                    Assert.IsFalse(Path.IsPathRooted(field.File));
                    Assert.IsFalse(field.File.Contains('\\'));
                    Assert.IsTrue(resultNames.Contains(Path.GetFileName(field.File)), field.File);
                }
            }
            Assert.IsTrue(references >= 3);

            property.TaskKind = TaskKind.термическая;
            var thermalOnly = preprocessor.CalcCompDataV2(project.TaskData, property);
            Assert.AreEqual(parameters.Count / 2, thermalOnly.Count);
            Assert.IsTrue(thermalOnly.All(item => item.TaskKind == TaskKind.термическая));
            Assert.IsFalse(Directory.Exists(Path.Combine(root, "InputData")));
        }

        /// <summary>
        /// Старая сортировка GUI также использует числовой индекс и порядок физик.
        /// </summary>
        [TestMethod]
        public void SortExistingTaskFiles_UsesIndexBeforeTime()
        {
            var paths = new[]
            {
                "механическая_10_0_1.tsf", "механическая_2_90_100.tsf",
                "термическая_1_500_900.tsf", "термическая_10_0_1.tsf",
                "термическая_2_90_100.tsf", "механическая_1_500_900.tsf"
            };
            var preprocessor = new global::PreProc.PreProc();
            var result = preprocessor.SortCompDataByTimeAndType(paths);
            CollectionAssert.AreEqual(new[]
            {
                "термическая термическая_1_500_900.tsf выполнить",
                "механическая механическая_1_500_900.tsf выполнить",
                "термическая термическая_2_90_100.tsf выполнить",
                "механическая механическая_2_90_100.tsf выполнить",
                "термическая термическая_10_0_1.tsf выполнить",
                "механическая механическая_10_0_1.tsf выполнить"
            }, result);
        }

        /// <summary>
        /// Сортировка отвергает неверный вид, индекс, интервал и расширение файла.
        /// </summary>
        [TestMethod]
        [DataRow("термическая_0_0_1.tsf")]
        [DataRow("термическая_-1_0_1.tsf")]
        [DataRow("термическая_1_NaN_1.tsf")]
        [DataRow("термическая_1_0_Infinity.tsf")]
        [DataRow("термическая_1_2_1.tsf")]
        [DataRow("термическая_1_-1_1.tsf")]
        [DataRow("4_1_0_1.tsf")]
        [DataRow("термическая_1_0,1_1.tsf")]
        [DataRow("термическая_1_0_1.txt")]
        public void SortExistingTaskFiles_RejectsMalformedName(string file)
        {
            var preprocessor = new global::PreProc.PreProc();
            Assert.ThrowsException<global::PreProc.DraftException>(() => preprocessor.SortCompDataByTimeAndType(new[] { file }));
        }

        /// <summary>
        /// Два файла одного вида и индекса не образуют допустимую очередь.
        /// </summary>
        [TestMethod]
        public void SortExistingTaskFiles_RejectsDuplicateKindAndIndex()
        {
            var preprocessor = new global::PreProc.PreProc();
            var files = new[] { "термическая_1_0_1.tsf", "термическая_1_1_2.tsf" };
            Assert.ThrowsException<global::PreProc.DraftException>(() => preprocessor.SortCompDataByTimeAndType(files));
        }

        /// <summary>
        /// TCF сохраняет текущий порядок строк GUI, статусы и переносимые пути.
        /// </summary>
        [TestMethod]
        [DataRow("ru-RU")]
        [DataRow("en-US")]
        public void WriteCommandFile_PreservesQueueAndTaskFiles(string culture)
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            var input = Path.Combine(root, "InputData", "Очередь с пробелами");
            Directory.CreateDirectory(input);
            var first = Path.Combine(input, "термическая_1_0_1.tsf");
            var second = Path.Combine(input, "термическая_2_1_2.tsf");
            var original = Encoding.UTF8.GetBytes("TSF bytes must remain unchanged.");
            File.WriteAllBytes(first, original);
            File.WriteAllBytes(second, original);
            var queue = new[]
            {
                "термическая InputData/Очередь с пробелами/термическая_2_1_2.tsf пропустить",
                "термическая InputData/Очередь с пробелами/термическая_1_0_1.tsf выполнить"
            };
            var preprocessor = new global::PreProc.PreProc();
            var tcf = preprocessor.WriteCommandFile(projectPath, queue);
            CollectionAssert.AreEqual(new[]
            {
                "загрузить проект \"../Модель с пробелами.bpf\"",
                "расчет термическая \"../InputData/Очередь с пробелами/термическая_2_1_2.tsf\" пропустить",
                "расчет термическая \"../InputData/Очередь с пробелами/термическая_1_0_1.tsf\" выполнить"
            }, File.ReadAllLines(tcf));
            var bytes = File.ReadAllBytes(tcf);
            Assert.IsFalse(bytes.Length >= 3 && bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf);
            CollectionAssert.AreEqual(original, File.ReadAllBytes(first));
            CollectionAssert.AreEqual(original, File.ReadAllBytes(second));

            var moved = root + "_moved";
            Directory.Move(root, moved);
            root = moved;
            var movedTcf = Path.Combine(moved, "ComputationData", "computation.tcf");
            var directory = Path.GetDirectoryName(movedTcf)!;
            foreach (var line in File.ReadAllLines(movedTcf))
            {
                var reference = line.Split('"')[1];
                Assert.IsFalse(Path.IsPathRooted(reference));
                Assert.IsFalse(reference.Contains('\\'));
                var resolved = Path.GetFullPath(reference, directory);
                Assert.IsTrue(File.Exists(resolved), reference);
            }
        }

        /// <summary>
        /// Некорректная очередь не заменяет готовый TCF.
        /// </summary>
        [TestMethod]
        [DataRow("outside")]
        [DataRow("missing")]
        [DataRow("duplicate")]
        [DataRow("status")]
        [DataRow("empty")]
        [DataRow("null")]
        [DataRow("invalid-character")]
        [DataRow("wrong-kind")]
        [DataRow("wrong-time")]
        public void WriteCommandFile_InvalidQueuePreservesExistingFile(string failure)
        {
            var input = Path.Combine(root, "InputData");
            Directory.CreateDirectory(input);
            var task = Path.Combine(input, "термическая_1_0_1.tsf");
            File.WriteAllText(task, "{}");
            var preprocessor = new global::PreProc.PreProc();
            var valid = "термическая InputData/термическая_1_0_1.tsf выполнить";
            var tcf = preprocessor.WriteCommandFile(projectPath, new[] { valid });
            var original = File.ReadAllBytes(tcf);
            var queue = new List<string> { valid };
            switch (failure)
            {
                case "outside": queue.Add("термическая ../термическая_2_1_2.tsf выполнить"); break;
                case "missing": queue.Add("термическая InputData/термическая_2_1_2.tsf выполнить"); break;
                case "duplicate": queue.Add("термическая InputData/термическая_1_1_2.tsf выполнить"); break;
                case "status": queue[0] = "термическая InputData/термическая_1_0_1.tsf unknown"; break;
                case "empty": queue.Clear(); break;
                case "null": queue.Add(null!); break;
                case "invalid-character": queue.Add("термическая Input*Data/термическая_2_1_2.tsf выполнить"); break;
                case "wrong-kind": queue.Add("механическая InputData/термическая_2_1_2.tsf выполнить"); break;
                case "wrong-time": queue.Add("термическая InputData/термическая_2_NaN_2.tsf выполнить"); break;
            }
            var rejected = false;
            try
            {
                preprocessor.WriteCommandFile(projectPath, queue);
            }
            catch (ArgumentException)
            {
                rejected = true;
            }
            catch (FileNotFoundException)
            {
                rejected = true;
            }
            Assert.IsTrue(rejected, failure);
            CollectionAssert.AreEqual(original, File.ReadAllBytes(tcf));
            Assert.AreEqual(0, Directory.GetFiles(Path.GetDirectoryName(tcf)!, "*.tmp").Length);
        }

        /// <summary>
        /// Путь TSF на границе CORE обязан быть относительным и переносимым.
        /// </summary>
        [TestMethod]
        [DataRow("C:/InputData/термическая_1_0_1.tsf")]
        [DataRow("/InputData/термическая_1_0_1.tsf")]
        [DataRow("InputData\\термическая_1_0_1.tsf")]
        [DataRow("Input\"Data/термическая_1_0_1.tsf")]
        [DataRow("Input\nData/термическая_1_0_1.tsf")]
        public void WriteCommandFile_RejectsNonportableTaskPath(string taskFile)
        {
            var preprocessor = new global::PreProc.PreProc();
            var queue = new[] { $"термическая {taskFile} выполнить" };
            Assert.ThrowsException<ArgumentException>(() => preprocessor.WriteCommandFile(projectPath, queue));
            Assert.IsFalse(Directory.Exists(Path.Combine(root, "ComputationData")));
        }

        /// <summary>
        /// Регистр пути проверяется одинаково на обеих ОС.
        /// </summary>
        [TestMethod]
        [DataRow("filename")]
        [DataRow("directory")]
        [DataRow("project")]
        [DataRow("output-directory")]
        public void WriteCommandFile_RejectsMismatchedPathCase(string mismatch)
        {
            var actualDirectory = mismatch == "directory" ? "inputdata" : "InputData";
            var input = Path.Combine(root, actualDirectory);
            Directory.CreateDirectory(input);
            var actualFile = mismatch == "filename" ? "Термическая_1_0_1.tsf" : "термическая_1_0_1.tsf";
            File.WriteAllText(Path.Combine(input, actualFile), "{}");
            var selectedProject = mismatch == "project" ? Path.Combine(root, "модель с пробелами.bpf") : projectPath;
            if (mismatch == "output-directory")
                Directory.CreateDirectory(Path.Combine(root, "computationdata"));
            var preprocessor = new global::PreProc.PreProc();
            var queue = new[] { "термическая InputData/термическая_1_0_1.tsf выполнить" };
            Assert.ThrowsException<ArgumentException>(() => preprocessor.WriteCommandFile(selectedProject, queue));
            Assert.AreEqual(0, Directory.GetFiles(root, "*.tcf", SearchOption.AllDirectories).Length);
        }

        /// <summary>
        /// Загружает проект с реальными исходными условиями для PreProc.
        /// </summary>
        private global::Project.Interfaces.IProjectData LoadFixtureProject()
        {
            var fixture = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "TestProject", "PreProc", "Data", "CalcCompDataTest", "FastCheck.bpf"));
            File.Copy(fixture, projectPath, true);
            var fixtureDirectory = Path.GetDirectoryName(fixture)!;
            foreach (var database in new[] { "materials.jsf", "functions.jsf" })
            {
                var source = Path.Combine(fixtureDirectory, database);
                var target = Path.Combine(root, database);
                File.Copy(source, target);
            }
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var loader = new LoadProjectFromTextFormat();
            return loader.Load(projectPath);
        }
    }
}
