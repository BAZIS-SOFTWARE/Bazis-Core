using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using OperationalController;

namespace TestProject_test.Geometry
{
    /// <summary>Проверяет массовые уведомления при работе с настоящим CAD ядром.</summary>
    [TestClass]
    [DoNotParallelize]
    public class GeometryBatchChangeTests
    {
        /// <summary>Проверяет типы геометрии и однократные обновления наборов при создании и удалении.</summary>
        [TestMethod]
        public void CreateGenerateAndDeleteGeometry_PublishTypedChangesOncePerAffectedSet()
        {
            var dllPath = GmshLibrary.FilePath;
            var controller = new ProjectController();
            controller.Create("geometry-batch.bpf2");
            controller.InitializeGeometry(dllPath);
            try
            {
                var phase = "create";
                var changes = new List<MeshChange>();
                var geometryChanges = new List<GeoChange>();
                var batchSizes = new List<int>();
                controller.Message += (_, args) =>
                {
                    batchSizes.Add(args.Changes.Count);
                    foreach (var message in args.Changes)
                    {
                        if (message is MeshChange change)
                        {
                            if (phase == "mesh")
                            {
                                Assert.IsTrue(controller.GetAllModelNodes().Count() > 2);
                                Assert.IsTrue(controller.GetAllModelElements().Count() > 1);
                            }
                            else if (phase == "delete")
                            {
                                Assert.AreEqual(0, controller.GetModelObjects(ObjType.Точка).Count());
                                Assert.AreEqual(0, controller.GetModelObjects(ObjType.Кривая).Count());
                            }
                            changes.Add(change);
                        }
                        if (message is GeoChange geometryChange)
                            geometryChanges.Add(geometryChange);
                    }
                };
                var first = controller.CreatePoint(0, 0, 0, 0.05);
                var second = controller.CreatePoint(1, 0, 0, 0.05);
                var line = controller.CreateLine(first, second);
                CollectionAssert.AreEqual(new[]
                {
                    GeoChange.GeometryEntityKind.Point,
                    GeoChange.GeometryEntityKind.Point,
                    GeoChange.GeometryEntityKind.Curve
                }, geometryChanges.Select(change => change.EntityKind).ToArray());
                CollectionAssert.AreEqual(new[] { 2, 2, 2 }, batchSizes);
                phase = "mesh";
                changes.Clear();
                geometryChanges.Clear();
                batchSizes.Clear();
                controller.GenerateMesh(1);
                CollectionAssert.AreEqual(new[] { 2 }, batchSizes);
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual(1, changes.Count(change => change.Action == ChangeAction.Created));
                Assert.AreEqual(1, changes.Count(change => change.Action == ChangeAction.Updated));
                phase = "delete";
                changes.Clear();
                batchSizes.Clear();
                controller.DeleteGeometryObject(1, line, true);
                CollectionAssert.AreEqual(new[] { 3 }, batchSizes);
                Assert.AreEqual(2, changes.Count);
                Assert.IsTrue(changes.All(change => change.Action == ChangeAction.Updated));
                Assert.AreEqual(2, changes.Select(change => change.ObjectType).Distinct().Count());
                Assert.AreEqual(1, geometryChanges.Count);
                var expectedKind = GeoChange.GeometryEntityKind.Point | GeoChange.GeometryEntityKind.Curve;
                Assert.AreEqual(expectedKind, geometryChanges[0].EntityKind);
            }
            finally
            {
                controller.UnloadGeometry();
            }
        }
    }
}
