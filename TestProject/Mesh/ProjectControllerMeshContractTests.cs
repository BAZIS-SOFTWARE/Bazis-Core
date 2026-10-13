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

namespace TestProject_test.Mesh
{
    [TestClass]
    public class ProjectControllerMeshContractTests
    {
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

    }
}
