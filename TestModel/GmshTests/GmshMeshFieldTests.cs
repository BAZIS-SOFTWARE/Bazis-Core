using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;
using System.Linq;
using GmshApi.Api;
using GmshApi.Wrapper;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GmshMeshFieldTests
    {
        private static string modelFolder = @"..\..\Examples\GMSHConnector\";

        [TestInitialize]
        public void Init()
        {
            GmshWrapperConnection.Initialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            GmshWrapperConnection.Finalize();
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void AddRemoveFieldTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var field = new MeshField(MeshFieldType.BoundaryLayer);
                geometry.AddField(field);
                var list = GmshWrapperMeshField.List();
                CollectionAssert.AreEqual(list, new int[] { field.Tag });
                geometry.RemoveField(field);
                list = GmshWrapperMeshField.List();
                CollectionAssert.AreEqual(list, new int[] { });
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void AddBoundaryLayerTest(string fileName)
        {
            using (var geometry = new GeometryObject(modelFolder + fileName))
            {
                var field = new MeshField(MeshFieldType.BoundaryLayer);
                var mesh = geometry.Generate(2);
                mesh.ChangeTriangulationAlgorithm2D(Triangulation2DAlgorithm.FrontalDelaunay);
                var el2dCount = mesh.GetElement2D().ToList().Count;
                field.SetOptionValues(BoundaryLayerOption.CurvesList.ToString(), new double[] { 4.0, 5.0 });
                field.SetOptionValues(BoundaryLayerOption.ExcludedSurfacesList.ToString(), new double[] { 1.0, 2.0 });
                field.SetOptionValues(BoundaryLayerOption.PointsList.ToString(), new double[] { 2.0, 4.0 });
                field.SetOptionValue(BoundaryLayerOption.Thickness.ToString(), 2.0);
                field.SetDisplayFieldType(DisplayFieldType.BoundaryLayer);
                geometry.AddField(field);
                mesh = geometry.Remesh(2);
                var newEl2dCount = mesh.GetElement2D().ToList().Count;
                Assert.AreNotEqual(el2dCount, newEl2dCount);
                geometry.RemoveField(field);
            }
        }

    }
}
