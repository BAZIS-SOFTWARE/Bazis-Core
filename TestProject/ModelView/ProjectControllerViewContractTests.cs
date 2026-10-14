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

namespace TestProject_test.ModelView
{
    [TestClass]
    public class ProjectControllerViewContractTests
    {
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

    }
}
