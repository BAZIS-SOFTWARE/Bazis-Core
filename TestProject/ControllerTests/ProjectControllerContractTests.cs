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

namespace TestProject.ControllerTests
{
    /// <summary>
    /// Проверяет замену проекта, типизированные события и прогресс чтения.
    /// </summary>
    [TestClass]
    public class ProjectControllerContractTests
    {
        /// <summary>
        /// Проверяет изменение имени проекта через команду и отсутствие повторного уведомления.
        /// </summary>
        [TestMethod]
        public void ChangeProjectName_PublishesOnlyWhenChanged()
        {
            var controller = new ProjectController();
            controller.Create("old.bpf2");
            var changes = new List<ProjectChange>();
            controller.Message += (_, args) =>
            {
                if (args.Change is ProjectChange change)
                    changes.Add(change);
            };

            controller.ChangeProjectName("new.bpf2");
            controller.ChangeProjectName("new.bpf2");

            Assert.AreEqual("new.bpf2", controller.Name);
            Assert.IsTrue(controller.IsModified);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(ProjectProperty.Name, changes[0].Property);
        }

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
                controller.Message += (_, args) => changes.Add(args.Change);

                controller.Open(path, progress);
                CollectionAssert.AreEqual(new[] { 0, 50, 100 }, progress.Values.Distinct().ToArray());

                Assert.AreEqual(path, controller.FilePath);
                Assert.AreEqual(Color.Green, controller.SelectionColor);
                Assert.AreEqual((byte)128, controller.Transparency);
                Assert.IsTrue(controller.HideInsideSurfaces);
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(ProjectChangeKind.Loaded, ((ProjectChange)changes[0]).ChangeKind);
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
            controller.Message += (_, args) => changes.Add(args.Change);

            Assert.ThrowsException<FileNotFoundException>(() => controller.Open(path));

            Assert.AreEqual("current.bpf2", controller.FilePath);
            Assert.IsTrue(controller.HasProject);
            Assert.AreEqual(0, changes.Count);
        }

        /// <summary>
        /// Проверяет адрес обновлённого набора после добавления узла.
        /// </summary>
        [TestMethod]
        public void CreateNode_PublishesMeshChange()
        {
            var controller = new ProjectController();
            controller.Create("mesh.bpf2");
            MeshChange change = null;
            controller.Message += (_, args) => change = args.Change as MeshChange;

            controller.CreateNode(7, 0, 0, 0);

            Assert.IsNotNull(change);
            Assert.AreEqual(MeshDataKind.Set, change.DataKind);
            Assert.AreEqual(ChangeAction.Updated, change.Action);
            Assert.AreEqual(ObjType.Узел, change.ObjectType);
            Assert.AreEqual(controller.GetModelSetInfo(ObjType.Узел, 7).Number, change.EntityId);
        }

        /// <summary>
        /// Проверяет адреса созданной и удалённой группы.
        /// </summary>
        [TestMethod]
        public void GroupLifecycle_PublishesCreatedAndDeleted()
        {
            var controller = new ProjectController();
            controller.Create("groups.bpf2");
            controller.CreateNode(7, 0, 0, 0);
            var modelObject = controller.GetModelObject(ObjType.Узел, 7);
            var changes = new List<MeshChange>();
            controller.Message += (_, args) =>
            {
                if (args.Change is MeshChange change && change.DataKind == MeshDataKind.Group)
                    changes.Add(change);
            };

            controller.CreateGroup("test", new[] { modelObject });
            var groupId = controller.GetModelGroup("test").Number;
            controller.DeleteModelGroup("test");

            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(ChangeAction.Created, changes[0].Action);
            Assert.AreEqual(ChangeAction.Deleted, changes[1].Action);
            Assert.IsTrue(changes.All(change => change.EntityId == groupId && change.ObjectType == ObjType.Узел));
        }

        /// <summary>
        /// Проверяет создание, изменение и удаление условия по индексу.
        /// </summary>
        [TestMethod]
        public void ConditionLifecycle_PublishesActionsAndIndices()
        {
            var controller = new ProjectController();
            controller.Create("conditions.bpf2");
            controller.CreateNode(7, 0, 0, 0);
            var modelObject = controller.GetModelObject(ObjType.Узел, 7);
            controller.CreateGroup("test", new[] { modelObject });
            var group = controller.GetModelGroup("test");
            var changes = new List<CondChange>();
            controller.Message += (_, args) =>
            {
                if (args.Change is CondChange change)
                    changes.Add(change);
            };

            var load = new LoadData(group, 0, 1);
            controller.AddTaskData(load);
            var heat = new HeatData(group, 0, 1);
            controller.UpdateCondition(0, heat);
            controller.DeleteCond(0);

            Assert.AreEqual(3, changes.Count);
            CollectionAssert.AreEqual(new[] { ChangeAction.Created, ChangeAction.Updated, ChangeAction.Deleted }, changes.Select(change => change.Action).ToArray());
            CollectionAssert.AreEqual(new[] { DataKind.Нагрузка, DataKind.Нагрев, DataKind.Нагрев }, changes.Select(change => change.DataKind).ToArray());
            Assert.IsTrue(changes.All(change => change.Id == 0));
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
        /// Проверяет сохранение вида и уведомление при присоединении модели.
        /// </summary>
        [TestMethod]
        public void Append_PreservesViewAndPublishesMeshChange()
        {
            var path = Path.Combine(Path.GetTempPath(), $"project-controller-{Guid.NewGuid():N}.bpf2");
            File.WriteAllLines(path, new[] { "Узел", "7 0 0 0", "#Узел" });
            try
            {
                var controller = new ProjectController();
                controller.Create("append.bpf2");
                controller.CreateNode(1, 0, 0, 0);
                controller.Select(ObjType.Узел, new[] { 1 });
                var changes = new List<MeshChange>();
                controller.Message += (_, args) =>
                {
                    if (args.Change is MeshChange change)
                        changes.Add(change);
                };

                controller.Append(path);

                Assert.IsNotNull(controller.GetModelObject(ObjType.Узел, 7));
                Assert.IsTrue(controller.IsSelected(ObjType.Узел, 1));
                Assert.IsTrue(changes.Any(change => change.ObjectType == ObjType.Узел && change.Action == ChangeAction.Updated));
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

        /// <summary>
        /// Проверяет объединение изменений вида в одно уведомление.
        /// </summary>
        [TestMethod]
        public void BeginViewUpdate_PublishesOneViewChange()
        {
            var controller = new ProjectController();
            controller.Create("view.bpf2");
            controller.CreateNode(7, 0, 0, 0);
            var changes = new List<ViewChange>();
            controller.Message += (_, args) =>
            {
                if (args.Change is ViewChange change)
                    changes.Add(change);
            };

            using (controller.BeginViewUpdate())
            {
                controller.Select(ObjType.Узел, new[] { 7 });
                controller.SetVisible(ObjType.Узел, new[] { 7 }, false);
            }

            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(1, changes[0].AffectedSets.Count());
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
