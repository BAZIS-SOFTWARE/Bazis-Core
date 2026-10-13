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

namespace TestProject_test.Project
{
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

    }
}
