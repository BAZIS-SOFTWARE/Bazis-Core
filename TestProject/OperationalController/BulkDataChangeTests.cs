using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using Model.MeshObjects;
using Model.ObjectsCollections;
using OperationalController;
using OperationalController.MeshObjsUtility;

namespace TestProject_test.OperationalController
{
    /// <summary>Проверяет уведомления после полного обхода больших коллекций.</summary>
    [TestClass]
    public class BulkDataChangeTests
    {
        /// <summary>Проверяет одно сообщение о готовом наборе независимо от количества объектов.</summary>
        [DataTestMethod]
        [DataRow("move", 1)]
        [DataRow("move", 128)]
        [DataRow("move", 4096)]
        [DataRow("rotate", 1)]
        [DataRow("rotate", 128)]
        [DataRow("rotate", 4096)]
        [DataRow("order", 1)]
        [DataRow("order", 128)]
        [DataRow("renumber", 1)]
        [DataRow("renumber", 128)]
        [DataRow("delete", 1)]
        [DataRow("delete", 128)]
        [DataRow("delete", 4096)]
        public void BulkOperation_PublishesOnceAfterAllObjectsChange(string operation, int count)
        {
            var controller = new ProjectController();
            controller.Create("bulk.bpf2");
            for (var index = 0; index < count; index++)
                controller.CreateNode(index, index + 1, 0, 0);
            if (operation == "order" || operation == "renumber")
            {
                controller.CreateNode(count, count + 1, 0, 0);
                for (var index = 0; index < count; index++)
                    controller.CreateElement("beams", 1, new[] { index, index + 1 });
                if (operation == "renumber")
                    foreach (var element in controller.GetAllModelElements())
                        element.SetNumber(element.Number + count);
            }
            var changes = new List<MeshChange>();
            controller.Message += (_, args) =>
            {
                if (args.Change is MeshChange change)
                {
                    Assert.AreEqual(MeshDataKind.Set, change.DataKind);
                    Assert.AreEqual(ChangeAction.Updated, change.Action);
                    switch (operation)
                    {
                        case "move":
                            foreach (var node in controller.GetAllModelNodes())
                                Assert.AreEqual(node.Number + 2, node.Position._x, 1e-5);
                            break;
                        case "rotate":
                            foreach (var node in controller.GetAllModelNodes())
                            {
                                Assert.AreEqual(0, node.Position._x, 1e-5);
                                Assert.AreEqual(node.Number + 1, node.Position._y, 1e-5);
                            }
                            break;
                        case "order":
                            Assert.IsTrue(controller.GetAllModelElements().All(element => element.Level == 2));
                            break;
                        case "renumber":
                            var numbers = controller.GetAllModelElements().Select(element => element.Number).ToArray();
                            var expected = Enumerable.Range(0, count).ToArray();
                            CollectionAssert.AreEqual(expected, numbers);
                            break;
                        case "delete":
                            Assert.AreEqual(0, controller.GetAllModelNodes().Count());
                            break;
                    }
                    changes.Add(change);
                }
            };
            switch (operation)
            {
                case "move":
                    var vector = new Point3D(1, 0, 0);
                    controller.MoveMesh(ObjType.Узел, vector);
                    break;
                case "rotate":
                    var axis = new Point3D(0, 0, 1);
                    controller.RotateMesh(ObjType.Узел, axis, 90);
                    break;
                case "order": controller.ChangeMeshSetOrder(1, "beams", 2); break;
                case "renumber": controller.RenumberElements(); break;
                case "delete":
                    var deletedNumbers = Enumerable.Range(0, count);
                    controller.DeleteModelObjects(ObjType.Узел, deletedNumbers);
                    break;
            }
            Assert.AreEqual(1, changes.Count);
        }

        /// <summary>Проверяет одно создание полностью заполненного набора границы.</summary>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(128)]
        public void CreateBoundary_PublishesCompletedSetOnce(int count)
        {
            var controller = new ProjectController();
            controller.Create("boundary.bpf2");
            for (var index = 0; index < count; index++)
            {
                var first = index * 3;
                controller.CreateNode(first, index * 2, 0, 0);
                controller.CreateNode(first + 1, index * 2 + 1, 0, 0);
                controller.CreateNode(first + 2, index * 2, 1, 0);
                controller.CreateElement("triangles", 2, new[] { first, first + 1, first + 2 });
            }
            var messages = 0;
            controller.Message += (_, args) =>
            {
                if (args.Change is MeshChange change)
                {
                    Assert.AreEqual(ChangeAction.Created, change.Action);
                    Assert.AreEqual(ObjType.Элемент1D, change.ObjectType);
                    var set = controller.GetModelSetInfo(ObjType.Элемент1D, "boundary1DSet_0");
                    Assert.AreEqual(count * 3, set.NumberOfObjects);
                    messages++;
                }
            };
            Assert.IsTrue(controller.Create1DFrom2D());
            Assert.AreEqual(1, messages);
        }

        /// <summary>Проверяет уникальность результатов слияния при нескольких заменах узлов каждого элемента.</summary>
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(128)]
        public void Merge_ReturnsChangedElementsOnceAndRepeatedMergeIsEmpty(int count)
        {
            var nodes = new ObjectsSet<Node>("nodes");
            var elements = new List<Beam>();
            var master = new Node(0, new Point3D(0, 0, 0));
            nodes.Add(0, master);
            var numbers = new List<int> { 0 };
            for (var index = 0; index < count; index++)
            {
                var firstNumber = index * 2 + 1;
                var first = new Node(firstNumber, new Point3D(0, 0, 0));
                var second = new Node(firstNumber + 1, new Point3D(0, 0, 0));
                nodes.Add(first.Number, first);
                nodes.Add(second.Number, second);
                numbers.Add(first.Number);
                numbers.Add(second.Number);
                var beam = new Beam(index, new[] { first, second });
                elements.Add(beam);
            }
            var groups = new List<List<int>> { numbers };
            var merger = new MergeObjects();
            merger.Merge(groups, nodes, out var changed, out var removed);
            Assert.IsTrue(removed);
            Assert.AreEqual(count, changed.Count);
            Assert.AreEqual(1, nodes.Count);
            foreach (var beam in elements)
            {
                Assert.AreSame(master, beam[0]);
                Assert.AreSame(master, beam[1]);
            }
            merger.Merge(groups, nodes, out var unchanged, out var removedAgain);
            Assert.IsFalse(removedAgain);
            Assert.AreEqual(0, unchanged.Count);
        }

        /// <summary>Проверяет уведомления о двух наборах со связанными узлами после массового слияния или удаления.</summary>
        [DataTestMethod]
        [DataRow("merge", 1)]
        [DataRow("merge", 128)]
        [DataRow("delete", 1)]
        [DataRow("delete", 128)]
        public void ConnectedSets_PublishOncePerSetAfterAllNodesAreProcessed(string operation, int count)
        {
            var path = Path.Combine(Path.GetTempPath(), $"bulk-connected-{Guid.NewGuid():N}.bpf2");
            var lines = new List<string> { "Узел", "0 0 0 0" };
            for (var number = 1; number <= count * 2; number++)
                lines.Add($"{number} 0 0 0");
            lines.Add("#Узел");
            foreach (var name in new[] { "first", "second" })
            {
                lines.Add($"Элемент1D {name}");
                for (var index = 0; index < count; index++)
                {
                    var elementNumber = name == "first" ? index : index + count;
                    lines.Add($"{elementNumber} 0 {index * 2 + 1} {index * 2 + 2}");
                }
                lines.Add("#Элемент1D");
            }
            File.WriteAllLines(path, lines);
            try
            {
                var controller = new ProjectController();
                controller.Open(path);
                var changes = new List<MeshChange>();
                controller.Message += (_, args) =>
                {
                    if (args.Change is MeshChange change)
                    {
                        Assert.AreEqual(1, controller.GetAllModelNodes().Count());
                        Assert.AreEqual(MeshDataKind.Set, change.DataKind);
                        if (operation == "merge")
                            foreach (var element in controller.GetAllModelElements())
                            {
                                Assert.AreEqual(0, element[0].Number);
                                Assert.AreEqual(0, element[1].Number);
                            }
                        else
                            Assert.AreEqual(0, controller.GetAllModelElements().Count());
                        changes.Add(change);
                    }
                };
                var numbers = Enumerable.Range(0, count * 2 + 1).ToList();
                var groups = new List<List<int>> { numbers };
                var deletedNumbers = numbers.Skip(1).ToArray();
                if (operation == "merge")
                    controller.MergeNodes(groups);
                else
                    controller.DeleteModelObjects(ObjType.Узел, deletedNumbers);
                Assert.AreEqual(3, changes.Count);
                var identities = changes.Select(change => $"{change.ObjectType}:{change.EntityId}").Distinct();
                Assert.AreEqual(3, identities.Count());
                Assert.AreEqual(operation == "merge" ? 3 : 1, changes.Count(change => change.Action == ChangeAction.Updated));
                if (operation == "merge")
                    controller.MergeNodes(groups);
                else
                    controller.DeleteModelObjects(ObjType.Узел, deletedNumbers);
                Assert.AreEqual(3, changes.Count);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
