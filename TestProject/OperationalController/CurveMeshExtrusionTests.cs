using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using OperationalController;

namespace TestProject_test.OperationalController
{
    [TestClass]
    [DoNotParallelize]
    public class CurveMeshExtrusionTests
    {
        [DataTestMethod]
        [DataRow("surface", false)]
        [DataRow("surface", true)]
        [DataRow("set", false)]
        [DataRow("set", true)]
        [DataRow("adjusted", false)]
        [DataRow("adjusted", true)]
        [DataRow("beam", false)]
        public void ExtrudeCurveMesh_PublishesCompletedOperation(string operation, bool transfinite)
        {
            var dllPath = GmshLibrary.FilePath;
            var controller = new ProjectController();
            controller.Create("curve-mesh.bpf2");
            controller.InitializeGeometry(dllPath);
            try
            {
                var first = controller.CreatePoint(0, 0, 0, 0.5);
                var second = controller.CreatePoint(1, 0, 0, 0.5);
                var third = controller.CreatePoint(1, 1, 0, 0.5);
                var fourth = controller.CreatePoint(0, 1, 0, 0.5);
                var top = controller.CreatePoint(0, 0, 1, 0.5);
                var edges = new[]
                {
                    controller.CreateLine(first, second),
                    controller.CreateLine(second, third),
                    controller.CreateLine(third, fourth),
                    controller.CreateLine(fourth, first)
                };
                var surface = controller.CreateSurface(edges);
                var curve = controller.CreateLine(first, top);
                controller.GenerateMesh(2);
                var source = controller.GetModelSetsInfo(ObjType.Элемент2D).First().Name;
                var startNode = controller.GetAllModelNodes().First(node => node.Position._x == 0 && node.Position._y == 0 && node.Position._z == 0).Number;
                if (operation == "adjusted")
                {
                    controller.CreateNode(1000000, 0.0005f, 0, 1);
                    controller.CreateNode(1000001, 2, 0, 1);
                    controller.CreateElement("attached", 1, [1000000, 1000001]);
                }
                var nodesBefore = controller.GetAllModelNodes().Count();
                var targetType = operation == "beam" ? ObjType.Элемент1D : ObjType.Элемент3D;
                var elementsBefore = controller.GetModelObjects(targetType).Count();
                var batches = new List<ProjectMessageEventArgs>();
                controller.Message += (_, args) =>
                {
                    Assert.IsTrue(controller.GetModelObjects(targetType).Count() > elementsBefore);
                    if (operation == "adjusted")
                    {
                        var coincident = controller.FindCoincidentObjects(ObjType.Узел, 0.0001f);
                        Assert.AreEqual(0, coincident.Count());
                        Assert.IsFalse(controller.GetAllModelNodes().Any(node => node.Number == 1000000));
                        var attached = controller.GetModelObjects(ObjType.Элемент1D);
                        Assert.IsTrue(attached.Any());
                    }
                    batches.Add(args);
                };

                // Ошибка до добавления данных должна освободить область, чтобы следующий вызов опубликовал пакет.
                if (operation == "surface")
                    Assert.ThrowsException<IndexOutOfRangeException>(() => controller.ExtrudeElement3DAlongCurveMesh(surface, [], first, transfinite));
                else if (operation == "set")
                    Assert.ThrowsException<KeyNotFoundException>(() => controller.ExtrudeElement3DAlongCurveMesh("missing", [curve], first, transfinite));
                else if (operation == "adjusted")
                    Assert.ThrowsException<ArgumentOutOfRangeException>(() => controller.ExtrudeElement3DAlongCurveMeshAdjusted(source, [curve], first, transfinite, 0));
                else
                    Assert.ThrowsException<KeyNotFoundException>(() => controller.ExtrudeElement1DAlongCurveMesh([curve], first, int.MaxValue));
                Assert.AreEqual(0, batches.Count);

                var result = operation switch
                {
                    "surface" => controller.ExtrudeElement3DAlongCurveMesh(surface, [curve], first, transfinite),
                    "set" => controller.ExtrudeElement3DAlongCurveMesh(source, [curve], first, transfinite),
                    "adjusted" => controller.ExtrudeElement3DAlongCurveMeshAdjusted(source, [curve], first, transfinite, 0.001),
                    _ => controller.ExtrudeElement1DAlongCurveMesh([curve], first, startNode)
                };

                Assert.AreEqual(1, batches.Count);
                Assert.IsTrue(controller.GetModelObjects(targetType).Count() > elementsBefore);
                Assert.IsTrue(controller.GetAllModelNodes().Count() > nodesBefore);
                var resultSet = controller.GetModelSetInfo(targetType, result);
                var meshChanges = batches[0].Changes.OfType<MeshChange>().ToArray();
                Assert.IsTrue(meshChanges.Any(change => change.ObjectType == ObjType.Узел));
                Assert.IsTrue(meshChanges.Any(change => change.ObjectType == targetType && change.Action == ChangeAction.Created && change.EntityId == resultSet.Number));
            }
            finally
            {
                controller.UnloadGeometry();
            }
        }
    }
}
