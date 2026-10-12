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

namespace TestProject_test.OperationalController
{
    /// <summary>
    /// Проверяет замену проекта, типизированные события и прогресс чтения.
    /// </summary>
    [TestClass]
    public class ProjectControllerContractTests
    {
        /// <summary>
        /// Проверяет независимость опубликованного пакета от исходного изменяемого списка.
        /// </summary>
        [TestMethod]
        public void ProjectMessageEventArgs_CopiesSourceChanges()
        {
            var source = new List<ChangeMessage>
            {
                new ProjectChange(ProjectChange.ProjectChangeKind.Created)
            };
            var args = new ProjectMessageEventArgs(source);

            source.Clear();

            Assert.AreEqual(1, args.Changes.Count);
            Assert.IsInstanceOfType<ProjectChange>(args.Changes[0]);
        }

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
                foreach (var message in args.Changes)
                    if (message is ProjectChange change)
                        changes.Add(change);
            };

            controller.ChangeProjectName("new.bpf2");
            controller.ChangeProjectName("new.bpf2");

            Assert.AreEqual("new.bpf2", controller.Name);
            Assert.IsTrue(controller.IsModified);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(ProjectChange.ProjectProperty.Name, changes[0].Property);
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
        /// Проверяет адрес обновлённого набора после добавления узла.
        /// </summary>
        [TestMethod]
        public void CreateNode_PublishesMeshChange()
        {
            var controller = new ProjectController();
            controller.Create("mesh.bpf2");
            MeshChange change = null;
            controller.Message += (_, args) =>
            {
                foreach (var message in args.Changes)
                    if (message is MeshChange meshChange)
                        change = meshChange;
            };

            controller.CreateNode(7, 0, 0, 0);

            Assert.IsNotNull(change);
            Assert.AreEqual(MeshChange.MeshDataKind.Set, change.DataKind);
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
                foreach (var message in args.Changes)
                    if (message is MeshChange change && change.DataKind == MeshChange.MeshDataKind.Group)
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
        /// Проверяет отдельное сообщение с прежним и новым именем набора.
        /// </summary>
        [TestMethod]
        public void ChangeMeshSetName_PublishesSetRename()
        {
            var controller = new ProjectController();
            controller.Create("rename-set.bpf2");
            controller.CreateNode(0, 0, 0, 0);
            controller.CreateNode(1, 1, 0, 0);
            controller.CreateElement("old-set", 1, new[] { 0, 1 });
            var setInfo = controller.GetModelSetInfo(ObjType.Элемент1D, "old-set");
            var changes = new List<ChangeMessage>();
            var renames = new List<SetRename>();
            controller.Message += (_, args) =>
            {
                changes.AddRange(args.Changes);
                foreach (var message in args.Changes)
                    if (message is SetRename rename)
                        renames.Add(rename);
            };

            controller.ChangeMeshSetName(1, "old-set", "new-set");

            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(1, renames.Count);
            var rename = renames[0];
            Assert.AreEqual(ObjType.Элемент1D, rename.ObjectType);
            Assert.AreEqual(setInfo.Number, rename.EntityId);
            Assert.AreEqual("old-set", rename.OldName);
            Assert.AreEqual("new-set", rename.NewName);
            Assert.AreSame(setInfo, controller.GetModelSetInfo(ObjType.Элемент1D, "new-set"));
        }

        /// <summary>
        /// Проверяет отдельное сообщение с прежним и новым именем группы.
        /// </summary>
        [TestMethod]
        public void ChangeGroupName_PublishesGroupRename()
        {
            var controller = new ProjectController();
            controller.Create("rename-group.bpf2");
            controller.CreateNode(7, 0, 0, 0);
            var modelObject = controller.GetModelObject(ObjType.Узел, 7);
            controller.CreateGroup("old-group", new[] { modelObject });
            var group = controller.GetModelGroup("old-group");
            var changes = new List<ChangeMessage>();
            var renames = new List<GroupRename>();
            controller.Message += (_, args) =>
            {
                changes.AddRange(args.Changes);
                foreach (var message in args.Changes)
                    if (message is GroupRename rename)
                        renames.Add(rename);
            };

            controller.ChangeGroupName("old-group", "new-group");

            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(1, renames.Count);
            var rename = renames[0];
            Assert.AreEqual(ObjType.Узел, rename.ObjectType);
            Assert.AreEqual(group.Number, rename.EntityId);
            Assert.AreEqual("old-group", rename.OldName);
            Assert.AreEqual("new-group", rename.NewName);
            Assert.AreSame(group, controller.GetModelGroup("new-group"));
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
                foreach (var message in args.Changes)
                    if (message is CondChange change)
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
        /// Проверяет сохранение вида и одно уведомление после добавления всех узлов модели.
        /// </summary>
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(128)]
        public void Append_PreservesViewAndPublishesMeshChange(int objectCount)
        {
            var path = Path.Combine(Path.GetTempPath(), $"project-controller-{Guid.NewGuid():N}.bpf2");
            var lines = new List<string> { "Узел" };
            for (var number = 7; number < 7 + objectCount; number++)
                lines.Add($"{number} 0 0 0");
            lines.Add("#Узел");
            File.WriteAllLines(path, lines);
            try
            {
                var controller = new ProjectController();
                controller.Create("append.bpf2");
                controller.CreateNode(1, 0, 0, 0);
                controller.Select(ObjType.Узел, new[] { 1 });
                var changes = new List<MeshChange>();
                controller.Message += (_, args) =>
                {
                    foreach (var message in args.Changes)
                        if (message is MeshChange change)
                        {
                            Assert.AreEqual(objectCount + 1, controller.GetAllModelNodes().Count());
                            changes.Add(change);
                        }
                };

                controller.Append(path);

                if (objectCount != 0)
                    Assert.IsNotNull(controller.GetModelObject(ObjType.Узел, 7));
                Assert.IsTrue(controller.IsSelected(ObjType.Узел, 1));
                Assert.AreEqual(objectCount == 0 ? 0 : 1, changes.Count);
                Assert.IsTrue(changes.All(change => change.ObjectType == ObjType.Узел && change.Action == ChangeAction.Updated));
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>Проверяет одно сообщение для каждого полностью добавленного набора, включая пустой и переименованный.</summary>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(128)]
        public void Append_CollectionsPublishOncePerCompletedSet(int objectCount)
        {
            var path = Path.Combine(Path.GetTempPath(), $"project-controller-{Guid.NewGuid():N}.bpf2");
            var lines = new List<string> { "Узел", "10 0 0 0", "11 1 0 0", "12 0 1 0", "#Узел", "Элемент1D shared" };
            for (var number = 0; number < objectCount; number++)
                lines.Add($"{number} 0 10 11");
            lines.Add("#Элемент1D");
            lines.Add("Элемент1D empty");
            lines.Add("#Элемент1D");
            lines.Add("Элемент2D shared");
            for (var number = 0; number < objectCount; number++)
                lines.Add($"{number} 0 10 11 12");
            lines.Add("#Элемент2D");
            File.WriteAllLines(path, lines);
            try
            {
                var controller = new ProjectController();
                controller.Create("collections.bpf2");
                controller.CreateNode(0, 0, 0, 0);
                controller.CreateNode(1, 1, 0, 0);
                controller.CreateElement("shared", 1, new[] { 0, 1 });
                var changes = new List<MeshChange>();
                controller.Message += (_, args) =>
                {
                    foreach (var message in args.Changes)
                        if (message is MeshChange change)
                        {
                            Assert.AreEqual(objectCount, controller.GetModelSetInfo(ObjType.Элемент1D, "shared_1").NumberOfObjects);
                            Assert.AreEqual(objectCount, controller.GetModelSetInfo(ObjType.Элемент2D, "shared").NumberOfObjects);
                            Assert.AreEqual(0, controller.GetModelSetInfo(ObjType.Элемент1D, "empty").NumberOfObjects);
                            changes.Add(change);
                        }
                };

                controller.Append(path);

                Assert.AreEqual(4, changes.Count);
                Assert.AreEqual(1, changes.Count(change => change.Action == ChangeAction.Updated));
                var createdChanges = changes.Where(change => change.Action == ChangeAction.Created).ToList();
                Assert.AreEqual(3, createdChanges.Count);
                Assert.AreEqual(3, createdChanges.Select(change => $"{change.ObjectType}:{change.EntityId}").Distinct().Count());
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
            var batches = 0;
            controller.Message += (_, args) =>
            {
                batches++;
                Assert.AreEqual(1, args.Changes.Count);
                foreach (var message in args.Changes)
                    if (message is ViewChange change)
                        changes.Add(change);
            };

            using (controller.BeginViewUpdate())
            {
                controller.Select(ObjType.Узел, new[] { 7 });
                controller.SetVisible(ObjType.Узел, new[] { 7 }, false);
            }

            Assert.AreEqual(1, batches);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(1, changes[0].AffectedSets.Count());
        }

        /// <summary>
        /// Проверяет включение очистки представления в пакет операции изменения данных.
        /// </summary>
        [TestMethod]
        public void DeleteSelectedObject_IncludesViewChangeInDataBatch()
        {
            var controller = new ProjectController();
            controller.Create("view-prune.bpf2");
            controller.CreateNode(7, 0, 0, 0);
            controller.Select(ObjType.Узел, new[] { 7 });
            var batches = new List<IReadOnlyList<ChangeMessage>>();
            controller.Message += (_, args) => batches.Add(args.Changes);

            controller.DeleteModelObjects(ObjType.Узел, new[] { 7 });

            Assert.AreEqual(1, batches.Count);
            Assert.IsTrue(batches[0].Any(change => change is MeshChange));
            Assert.IsTrue(batches[0].Any(change => change is ViewChange));
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
