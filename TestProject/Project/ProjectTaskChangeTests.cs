using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using OperationalController;
using Project.Interfaces.Tasks;
using Project.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestProject_test.Project
{
    [TestClass]
    public class ProjectTaskChangeTests
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
            var batches = 0;
            controller.Message += (_, args) =>
            {
                batches++;
                foreach (var message in args.Changes)
                    if (message is CondChange condition)
                    {
                        Assert.AreEqual(ChangeAction.Deleted, condition.Action);
                        indices.Add(condition.Id);
                    }
            };
            switch (operation)
            {
                case "create": controller.CreateTask(); break;
                case "kind": controller.ChangeTaskKind(TaskKind.термическая); break;
                case "type": controller.ChangeTaskType(TaskType.Volume); break;
            }
            Assert.AreEqual(1, batches);
            CollectionAssert.AreEqual(new[] { 1, 0 }, indices);
            Assert.AreSame(group, controller.GetModelGroup("task"));
        }
    }
}
