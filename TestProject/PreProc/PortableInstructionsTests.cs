using Microsoft.VisualStudio.TestTools.UnitTesting;
using PreProc;
using PreProc.Interfaces;
using Project.Interfaces.Tasks;
using Project.IO;
using Project.TaskParameters;
using Project.Tasks;
using System.Globalization;

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
        [DataRow("ru-RU", 0.1f, 0.25f, "термическая_3_0.100000001_0.25_выполнить")]
        [DataRow("en-US", 0.1f, 0.25f, "термическая_3_0.100000001_0.25_выполнить")]
        [DataRow("ru-RU", 0f, 0.25f, "термическая_3_0_0.25_выполнить")]
        [DataRow("en-US", 0f, 0.25f, "термическая_3_0_0.25_выполнить")]
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
            var taskKinds = TaskKind.термическая | TaskKind.механическая;
            var parameters = preprocessor.CalcCompDataV2(project.TaskData, taskKinds);
            Assert.IsTrue(parameters.Count > 2);
            Assert.IsFalse(Directory.Exists(Path.Combine(root, "InputData")));
            Assert.IsFalse(Directory.Exists(Path.Combine(root, "ComputationData")));

            for (var i = 0; i < parameters.Count; i++)
            {
                var item = parameters[i];
                Assert.AreEqual(i / 2 + 1, item.Index);
                Assert.AreEqual(i % 2 == 0 ? TaskKind.термическая : TaskKind.механическая, item.TaskKind);
            }

            var resultNames = parameters.Select(item => $"{item.FileStem}.db").ToHashSet(StringComparer.Ordinal);
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

            var linkedCount = parameters.Count;
            var thermalOnly = preprocessor.CalcCompDataV2(project.TaskData, TaskKind.термическая);
            Assert.AreEqual(linkedCount / 2, thermalOnly.Count);
            Assert.IsTrue(thermalOnly.All(item => item.TaskKind == TaskKind.термическая));
            Assert.IsFalse(Directory.Exists(Path.Combine(root, "InputData")));
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
