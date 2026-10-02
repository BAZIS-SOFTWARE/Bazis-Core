using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using OperationalController;

namespace TestProject_test.OperationalController
{
    /// <summary>Проверяет массовые уведомления при работе с настоящим CAD ядром.</summary>
    [TestClass]
    [DoNotParallelize]
    public class GeometryBatchChangeTests
    {
        /// <summary>Проверяет готовую сетку и однократные обновления наборов при рекурсивном удалении линии.</summary>
        [TestMethod]
        public void GenerateAndDeleteGeometry_PublishOncePerAffectedSet()
        {
            var dllPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "GmshApi", "gmsh.dll"));
            var controller = new ProjectController();
            controller.Create("geometry-batch.bpf2");
            controller.InitializeGeometry(dllPath);
            try
            {
                var first = controller.CreatePoint(0, 0, 0, 0.05);
                var second = controller.CreatePoint(1, 0, 0, 0.05);
                var line = controller.CreateLine(first, second);
                var phase = "mesh";
                var changes = new List<MeshChange>();
                var geometryChanges = 0;
                controller.Message += (_, args) =>
                {
                    if (args.Change is MeshChange change)
                    {
                        if (phase == "mesh")
                        {
                            Assert.IsTrue(controller.GetAllModelNodes().Count() > 2);
                            Assert.IsTrue(controller.GetAllModelElements().Count() > 1);
                        }
                        else
                        {
                            Assert.AreEqual(0, controller.GetModelObjects(ObjType.Точка).Count());
                            Assert.AreEqual(0, controller.GetModelObjects(ObjType.Кривая).Count());
                        }
                        changes.Add(change);
                    }
                    if (args.Change is GeoChange)
                        geometryChanges++;
                };
                controller.GenerateMesh(1);
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual(1, changes.Count(change => change.Action == ChangeAction.Created));
                Assert.AreEqual(1, changes.Count(change => change.Action == ChangeAction.Updated));
                phase = "delete";
                changes.Clear();
                controller.DeleteGeometryObject(1, line, true);
                Assert.AreEqual(2, changes.Count);
                Assert.IsTrue(changes.All(change => change.Action == ChangeAction.Updated));
                Assert.AreEqual(2, changes.Select(change => change.ObjectType).Distinct().Count());
                Assert.AreEqual(1, geometryChanges);
            }
            finally
            {
                controller.UnloadGeometry();
            }
        }
    }
}
