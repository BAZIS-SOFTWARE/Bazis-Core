using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using Model.IO;
using OperationalController;
using Project.Interfaces.Tasks;
using Project.Tasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace TestProject_test.IO
{
    [TestClass]
    public class ProjectControllerIOContractTests
    {
        /// <summary>
        /// Проверяет открытие проекта, перенос настроек вида и процент чтения.
        /// </summary>
        [DataTestMethod]
        [DataRow("bpf")]
        [DataRow("bpf2")]
        [DataRow("BPF2")]
        public void OpenProject_ReportsProgressAndTransfersViewSettings(string extension)
        {
            var path = Path.Combine(Path.GetTempPath(), $"project-controller-{Guid.NewGuid():N}.{extension}");
            File.WriteAllText(path, string.Empty);
            try
            {
                var controller = new ProjectController();
                controller.Create("current.bpf2");
                controller.SetSelectionColor(Color.Green);
                controller.SetTransparency(128);
                controller.SetHideInsideSurfaces(true);
                var progress = new RecordedProgress();
                var changes = new List<ChangeMessage>();
                controller.Message += (_, args) => changes.AddRange(args.Changes);

                controller.Open(path, progress);
                CollectionAssert.AreEqual(new[] { 0, 50, 100 }, progress.Values.Distinct().ToArray());

                Assert.AreEqual(path, controller.FilePath);
                Assert.AreEqual(Color.Green, controller.SelectionColor);
                Assert.AreEqual((byte)128, controller.Transparency);
                Assert.IsTrue(controller.HideInsideSurfaces);
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(ProjectChange.ProjectChangeKind.Loaded, ((ProjectChange)changes[0]).ChangeKind);
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Проверяет сохранение текущего проекта при ошибке чтения файла.
        /// </summary>
        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Open_MissingFilePreservesCurrentProject(bool meshFile)
        {
            var extension = meshFile ? "stl" : "bpf2";
            var path = Path.Combine(Path.GetTempPath(), $"missing-project-controller-{Guid.NewGuid():N}.{extension}");
            var controller = new ProjectController();
            controller.Create("current.bpf2");
            var changes = new List<ChangeMessage>();
            controller.Message += (_, args) => changes.AddRange(args.Changes);

            Assert.ThrowsException<FileNotFoundException>(() => controller.Open(path));

            Assert.AreEqual("current.bpf2", controller.FilePath);
            Assert.IsTrue(controller.HasProject);
            Assert.AreEqual(0, changes.Count);
        }

        /// <summary>
        /// Проверяет процент прочитанных строк текстового файла.
        /// </summary>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(4)]
        public void ProgressTextReader_ReportsReadLines(int lineCount)
        {
            var path = Path.GetTempFileName();
            File.WriteAllLines(path, Enumerable.Range(1, lineCount).Select(number => number.ToString()));
            try
            {
                var progress = new RecordedProgress();
                using (var reader = new ProgressTextReader(path, progress))
                    while (reader.ReadLine() != null) { }

                Assert.AreEqual(0, progress.Values.First());
                Assert.AreEqual(100, progress.Values.Last());
                Assert.IsTrue(progress.Values.SequenceEqual(progress.Values.OrderBy(value => value)));
                if (lineCount == 4)
                    CollectionAssert.AreEqual(new[] { 0, 25, 50, 75, 100 }, progress.Values);
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Проверяет процент прочитанных байт бинарного STL.
        /// </summary>
        [TestMethod]
        public void Open_ReportsBinaryBytes()
        {
            var path = Path.Combine(Path.GetTempPath(), $"project-controller-{Guid.NewGuid():N}.stl");
            File.WriteAllBytes(path, new byte[84]);
            try
            {
                var progress = new RecordedProgress();
                var controller = new ProjectController();
                controller.Open(path, progress);

                Assert.IsTrue(controller.HasProject);
                Assert.AreEqual(0, progress.Values.First());
                Assert.AreEqual(100, progress.Values.Last());
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Проверяет замену модели с переносом только общих настроек вида.
        /// </summary>
        [TestMethod]
        public void OpenMeshFile_TransfersCommonViewSettingsAndClearsSelection()
        {
            var path = Path.Combine(Path.GetTempPath(), $"project-controller-{Guid.NewGuid():N}.stl");
            File.WriteAllBytes(path, new byte[84]);
            try
            {
                var controller = new ProjectController();
                controller.Create("old.bpf2");
                controller.CreateNode(1, 0, 0, 0);
                controller.Select(ObjType.Узел, new[] { 1 });
                controller.SetSelectionColor(Color.Green);
                controller.SetTransparency(128);
                controller.SetHideInsideSurfaces(true);

                controller.Open(path);

                Assert.IsNull(controller.GetModelObject(ObjType.Узел, 1));
                Assert.AreEqual(Path.Combine(Path.GetDirectoryName(path), "новый_проект.bpf2"), controller.FilePath);
                Assert.AreEqual(0, controller.SelectedCount);
                Assert.AreEqual(Color.Green, controller.SelectionColor);
                Assert.AreEqual((byte)128, controller.Transparency);
                Assert.IsTrue(controller.HideInsideSurfaces);
            }
            finally
            {
                File.Delete(path);
            }
        }

        private sealed class RecordedProgress : IProgress<int>
        {
            public List<int> Values { get; } = new List<int>();

            /// <summary>
            /// Сохраняет полученный процент для проверки порядка сообщений.
            /// </summary>
            public void Report(int value)
            {
                Values.Add(value);
            }
        }
    }
}
