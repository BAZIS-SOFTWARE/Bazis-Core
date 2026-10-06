using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using OperationalController;
using Project.Interfaces.Tasks;
using Project.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestProject_test.OperationalController
{
    /// <summary>Проверяет точечные уведомления и состояние внутри обработчиков каскадных изменений.</summary>
    [TestClass]
    public class PointDataChangeTests
    {
        private ProjectController controller = null!;

        /// <summary>Создаёт независимую модель с тремя узлами и одним набором элементов.</summary>
        [TestInitialize]
        public void Initialize()
        {
            controller = new ProjectController();
            controller.Create("events.bpf2");
            controller.CreateNode(0, 0, 0, 0);
            controller.CreateNode(1, 1, 0, 0);
            controller.CreateNode(2, 2, 0, 0);
            controller.CreateElement("beams", 1, new[] { 0, 1 });
        }

        /// <summary>Проверяет наблюдаемое промежуточное состояние каскада удаления набора.</summary>
        [TestMethod]
        public void DeleteSet_PublishesBeforeDependentGroupsAndConditionsAreRemoved()
        {
            var element = controller.GetModelObject(ObjType.Элемент1D, 0);
            controller.CreateGroup("dependent", new[] { element });
            var group = controller.GetModelGroup("dependent");
            var first = new LoadData(group, 0, 1);
            var second = new HeatData(group, 0, 1);
            controller.AddTaskData(first);
            controller.AddTaskData(second);
            var setId = controller.GetModelSetInfo(ObjType.Элемент1D, "beams").Number;
            var trace = new List<string>();
            controller.Message += (_, args) =>
            {
                if (args.Change is SetDelete deletedSet)
                {
                    Assert.AreEqual(ObjType.Элемент1D, deletedSet.ObjectType);
                    Assert.AreEqual(setId, deletedSet.EntityId);
                    Assert.AreEqual("beams", deletedSet.Name);
                    Assert.IsNull(controller.GetModelSetInfo(ObjType.Элемент1D, "beams"));
                    Assert.AreSame(group, controller.GetModelGroup("dependent"));
                    Assert.AreEqual(2, controller.GetAllCondData().Count());
                    trace.Add("set");
                }
                if (args.Change is MeshChange deletedGroup && deletedGroup.DataKind == MeshChange.MeshDataKind.Group)
                {
                    Assert.IsNull(controller.GetModelGroup("dependent"));
                    Assert.AreEqual(2, controller.GetAllCondData().Count());
                    trace.Add("group");
                }
                if (args.Change is CondChange condition)
                {
                    Assert.AreEqual(ChangeAction.Deleted, condition.Action);
                    Assert.AreEqual(condition.Id, controller.GetAllCondData().Count());
                    trace.Add($"condition{condition.Id}");
                }
            };
            controller.DeleteModelSet(ObjType.Элемент1D, "beams");
            controller.DeleteModelSet(ObjType.Элемент1D, "beams");
            CollectionAssert.AreEqual(new[] { "set", "group", "condition1", "condition0" }, trace);
        }

        /// <summary>Проверяет сохранение объекта частично очищенной группы и ссылки условия.</summary>
        [TestMethod]
        public void DeleteNode_PreservesSurvivingGroupIdentityAndCondition()
        {
            var nodes = controller.GetAllModelNodes().ToArray();
            controller.CreateGroup("survivor", nodes);
            var group = controller.GetModelGroup("survivor");
            var load = new LoadData(group, 0, 1);
            controller.AddTaskData(load);
            var updates = 0;
            controller.Message += (_, args) =>
            {
                if (args.Change is MeshChange mesh && mesh.DataKind == MeshChange.MeshDataKind.Group)
                {
                    Assert.AreEqual(ChangeAction.Updated, mesh.Action);
                    Assert.AreSame(group, controller.GetModelGroup("survivor"));
                    Assert.AreSame(group, load.Group);
                    Assert.AreEqual(2, group.Count);
                    updates++;
                }
                Assert.IsFalse(args.Change is CondChange);
            };
            controller.DeleteModelObjects(ObjType.Узел, new[] { 2 });
            controller.ClearNotExistedGroupData();
            Assert.AreEqual(1, updates);
            Assert.AreSame(load, controller.GetAllCondData().Single());
        }

        /// <summary>Проверяет разные действия очистки постоянного и удаляемого наборов.</summary>
        [TestMethod]
        public void ClearNodes_UpdatesFixedSetAndDeletesElementSet_SecondClearIsSilent()
        {
            var updates = new List<MeshChange>();
            var deletions = new List<SetDelete>();
            controller.Message += (_, args) =>
            {
                if (args.Change is MeshChange mesh)
                {
                    Assert.AreEqual(MeshChange.MeshDataKind.Set, mesh.DataKind);
                    Assert.AreEqual(ObjType.Узел, mesh.ObjectType);
                    Assert.AreEqual(ChangeAction.Updated, mesh.Action);
                    Assert.AreEqual(0, controller.GetAllModelNodes().Count());
                    updates.Add(mesh);
                }
                if (args.Change is SetDelete deletion)
                {
                    Assert.AreEqual(ObjType.Элемент1D, deletion.ObjectType);
                    Assert.AreEqual("beams", deletion.Name);
                    Assert.IsNull(controller.GetModelSetInfo(ObjType.Элемент1D, "beams"));
                    deletions.Add(deletion);
                }
            };
            controller.ClearModelCollection(ObjType.Узел);
            controller.ClearModelCollection(ObjType.Узел);
            Assert.AreEqual(1, updates.Count);
            Assert.AreEqual(1, deletions.Count);
        }

        /// <summary>Проверяет отсутствие уведомлений для повторных присвоений и пустых преобразований.</summary>
        [DataTestMethod]
        [DataRow("rename")]
        [DataRow("order")]
        [DataRow("move")]
        [DataRow("rotate")]
        [DataRow("renumber")]
        public void UnchangedOperations_DoNotPublish(string operation)
        {
            controller.ChangeMeshSetOrder(1, "beams", 1);
            controller.RenumberElements();
            var messages = new List<ChangeMessage>();
            controller.Message += (_, args) => messages.Add(args.Change);
            switch (operation)
            {
                case "rename": controller.ChangeMeshSetName(1, "beams", "beams"); break;
                case "order": controller.ChangeMeshSetOrder(1, "beams", 1); break;
                case "move":
                    var zero = new Point3D(0, 0, 0);
                    controller.MoveMesh(ObjType.Узел, zero);
                    break;
                case "rotate":
                    var axis = new Point3D(0, 0, 1);
                    controller.RotateMesh(ObjType.Узел, axis, 0);
                    break;
                case "renumber": controller.RenumberElements(); break;
            }
            Assert.AreEqual(0, messages.Count);
        }

        /// <summary>Проверяет одно уведомление об обновлении после добавления всех элементов набора.</summary>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(128)]
        public void MergeElements_PublishesOneUpdateForCompletedAppend(int objectCount)
        {
            for (var index = 0; index < objectCount; index++)
                controller.CreateElement("slave", 1, new[] { 0, 1 });
            var master = controller.GetModelSetInfo(ObjType.Элемент1D, "beams");
            var changes = new List<MeshChange>();
            var deletions = new List<SetDelete>();
            controller.Message += (_, args) =>
            {
                if (args.Change is MeshChange change)
                {
                    if (change.Action == ChangeAction.Updated && change.EntityId == master.Number)
                        Assert.AreEqual(objectCount + 1, master.NumberOfObjects);
                    changes.Add(change);
                }
                if (args.Change is SetDelete deletion)
                    deletions.Add(deletion);
            };

            controller.MergeElements(ObjType.Элемент1D, "beams", "slave");

            Assert.AreEqual(1, changes.Count(change => change.Action == ChangeAction.Updated));
            Assert.AreEqual(1, deletions.Count);
            Assert.AreEqual("slave", deletions[0].Name);
            Assert.IsNull(controller.GetModelSetInfo(ObjType.Элемент1D, "slave"));
        }

        /// <summary>Проверяет передачу ошибки нумерации без уведомлений из незавершённого добавления.</summary>
        [TestMethod]
        public void MergeElements_InvalidNumbersThrowWithoutAppendNotifications()
        {
            controller.CreateElement("slave", 1, new[] { 0, 1 });
            controller.CreateElement("slave", 1, new[] { 1, 2 });
            var first = controller.GetModelObject(ObjType.Элемент1D, 1);
            var second = controller.GetModelObject(ObjType.Элемент1D, 2);
            second.SetNumber(first.Number);
            var messages = new List<ChangeMessage>();
            controller.Message += (_, args) => messages.Add(args.Change);

            Assert.ThrowsException<ArgumentException>(() => controller.MergeElements(ObjType.Элемент1D, "beams", "slave"));

            Assert.AreEqual(0, messages.Count);
            Assert.IsNotNull(controller.GetModelSetInfo(ObjType.Элемент1D, "slave"));
        }

        /// <summary>Проверяет немедленное прекращение каскада при исключении обработчика.</summary>
        [TestMethod]
        public void ThrowingSubscriber_LeavesAlreadyPublishedMutationAndStopsCascade()
        {
            var element = controller.GetModelObject(ObjType.Элемент1D, 0);
            controller.CreateGroup("dependent", new[] { element });
            var group = controller.GetModelGroup("dependent");
            var load = new LoadData(group, 0, 1);
            controller.AddTaskData(load);
            var error = new InvalidOperationException("Subscriber failure.");
            controller.Message += (_, args) => throw error;
            var thrown = Assert.ThrowsException<InvalidOperationException>(() => controller.DeleteModelGroup("dependent"));
            Assert.AreSame(error, thrown);
            Assert.IsNull(controller.GetModelGroup("dependent"));
            Assert.AreSame(load, controller.GetAllCondData().Single());
            Assert.IsTrue(controller.IsModified);
        }

        /// <summary>Проверяет обратные индексы удаления условий при замене или изменении задачи.</summary>
        [DataTestMethod]
        [DataRow("create")]
        [DataRow("kind")]
        [DataRow("type")]
        public void TaskChanges_PublishConditionRemovalsAtCurrentIndices(string operation)
        {
            var node = controller.GetModelObject(ObjType.Узел, 0);
            controller.CreateGroup("task", new[] { node });
            var group = controller.GetModelGroup("task");
            var load = new LoadData(group, 0, 1);
            var heat = new HeatData(group, 0, 1);
            controller.AddTaskData(load);
            controller.AddTaskData(heat);
            var indices = new List<int>();
            controller.Message += (_, args) =>
            {
                if (args.Change is CondChange condition)
                {
                    Assert.AreEqual(ChangeAction.Deleted, condition.Action);
                    Assert.AreEqual(condition.Id, controller.GetAllCondData().Count());
                    indices.Add(condition.Id);
                }
            };
            switch (operation)
            {
                case "create": controller.CreateTask(); break;
                case "kind": controller.ChangeTaskKind(TaskKind.термическая); break;
                case "type": controller.ChangeTaskType(TaskType.Volume); break;
            }
            CollectionAssert.AreEqual(new[] { 1, 0 }, indices);
            Assert.AreSame(group, controller.GetModelGroup("task"));
        }
    }
}
