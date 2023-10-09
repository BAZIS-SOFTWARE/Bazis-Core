using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using GmshApi.GmshController;
using System;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerMeshTests
    {
        private GmshController controller = new GmshController("C:\\Windows\\System32\\gmsh.dll");
        private static string modelFolder = @"..\..\..\Examples\GMSHConnector\";

        [TestInitialize]
        public void Init()
        {
        }

        [TestCleanup]
        public void Cleanup()
        {
            var ierr = 0;
            controller.gmshClear(ref ierr);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetNodes(string fileName)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(ref status);
            Assert.AreEqual(187, nodes.Count);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestRefineMesh(string fileName)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodesUntil = controller.GetNodes(ref status);
            controller.gmshModelMeshRefine(ref ierr);
            var nodesAfter = controller.GetNodes(ref status);
            Assert.IsTrue(nodesAfter.Count() > nodesUntil.Count());
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", 46)]
        public void TestGetElements1D(string fileName, int expected)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            var e1d = controller.GetMeshEntities(1, -1, ref status);
            Assert.AreEqual(expected, e1d.Count);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", 370)]
        public void TestGetElements2D(string fileName, int expected)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var e2d = controller.GetMeshEntities(2, -1, ref status);
            Assert.AreEqual(expected, e2d.Count);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", 707)]
        public void TestGetElements3D(string fileName, int expected)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(3, ref ierr);
            var e3d = controller.GetMeshEntities(3, -1, ref status);
            Assert.AreEqual(expected, e3d.Count);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestDensity(string fileName)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var untilElements = controller.GetMeshEntities(2, -1, ref status);
            controller.gmshOptionSetNumber("Mesh.MeshSizeFactor", 0.5, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var afterElements = controller.GetMeshEntities(2, -1, ref status);
            Assert.IsTrue(untilElements.Count() < afterElements.Count());
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetDensityForNodesTest(string fileName)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            var lengthUntil = controller.GetNodes(ref status).Count;
            controller.gmshOptionSetNumber("Mesh.MeshSizeFactor", 0.5, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            var lengthAfter = controller.GetNodes(ref status).Count;
            Assert.AreEqual(true, lengthAfter > lengthUntil);
            controller.gmshOptionSetNumber("Mesh.MeshSizeFactor", 1.0, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            var lengthLast = controller.GetNodes(ref status).Count;
            Assert.AreEqual(true, lengthLast < lengthAfter);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetDensityFor2DElementsTest(string fileName)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var lengthUntil = controller.GetMeshEntities(2,-1, ref status).Count;
            controller.gmshOptionSetNumber("Mesh.MeshSizeFactor", 0.5, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var lengthAfter = controller.GetMeshEntities(2, -1, ref status).Count;
            Assert.AreEqual(true, lengthAfter > lengthUntil);
            controller.gmshOptionSetNumber("Mesh.MeshSizeFactor", 1.0, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var lengthLast = controller.GetNodes(ref status).Count;
            Assert.AreEqual(true, lengthLast < lengthAfter);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetDensityFor3DElementsTest(string fileName)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(3, ref ierr);
            var lengthUntil = controller.GetMeshEntities(3, -1, ref status).Count;
            controller.gmshOptionSetNumber("Mesh.MeshSizeFactor", 0.5, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(3, ref ierr);
            var lengthAfter = controller.GetMeshEntities(3, -1, ref status).Count;
            Assert.AreEqual(true, lengthAfter > lengthUntil);
            controller.gmshOptionSetNumber("Mesh.MeshSizeFactor", 1.0, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(3, ref ierr);
            var lengthLast = controller.GetNodes(ref status).Count;
            Assert.AreEqual(true, lengthLast < lengthAfter);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 1, 2, 3 }, new int[] { 112, 112, 74 })]
        public void GetSurfaceByIdTest(string fileName, int[] tags, int[] expLength )
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            for (var i = 0; i < tags.Length; ++i)
            {
                var actual = controller.GetMeshEntities(2, tags[i], ref status).ToArray();
                Assert.AreEqual(expLength[i], actual.Length);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestChangeTriangulationAlgorithm2D(string fileName)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodesCountUntil = controller.GetNodes(ref status).Count;
            controller.gmshOptionSetNumber("Mesh.Algorithm", 2, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodesCountAfter = controller.GetNodes(ref status).Count;
            Assert.IsTrue(nodesCountAfter != nodesCountUntil);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestSetPointsOnCurve(string fileName)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodesCountUntil = controller.GetNodes(ref status).Count;
            controller.gmshModelMeshSetTransfiniteCurve(4, 10, "Progression", 1.5, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodesCountAfter = controller.GetNodes(ref status).Count;
            Assert.IsTrue(nodesCountAfter != nodesCountUntil);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", 2)]
        public void DeleteSurfaceTest(string fileName, int surfaceId)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var lengthBefore = controller.GetMeshEntities(2, -1, ref status).ToArray().Length;
            controller.gmshModelMeshClear(new int[] { 2, surfaceId }, (IntPtr)2, ref ierr);
            var lengthAfter = controller.GetMeshEntities(2, -1, ref status).ToArray().Length;
            Assert.AreEqual(true, lengthBefore > lengthAfter);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void DeleteAllSurfaces(string fileName)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var dimTags = new int[] { 2, 1, 2, 2, 2, 3, 2, 4 };
            controller.gmshModelMeshClear(dimTags, (IntPtr)dimTags.Length, ref ierr);
            var length = controller.GetMeshEntities(2, -1, ref status).ToArray().Length;
            Assert.IsTrue(length == 0);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new long[] { 206, 261, 251, 265, 210, 213 })]
        public void DeleteMeshElementsTest(string fileName, long[] elTags)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var lengthBefore = controller.GetMeshEntities(2, -1, ref status).ToArray().Length;
            controller.DeleteMeshElements(elTags);
            var lengthAfter = controller.GetMeshEntities(2, -1, ref status).ToArray().Length;
            Assert.IsTrue(lengthBefore > lengthAfter);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 7, 8, 1, 3, 6 }, new int[] { 0, 0, 5, 5, 9 })]
        [DataRow("Korpus_Nasosa.step", new int[] { 700, 720, 670, 667, 663 }, new int[] { 0, 0, 5, 2, 9 })]
        [DataRow("model.igs", new int[] { 124, 126, 140, 136, 137 }, new int[] { 1, 1, 0, 5, 6 })]
        [DataRow("sphere.geo", new int[] { 13, 14, 1, 2, 3 }, new int[] { 0, 0, 3, 3, 3 })]

        public void GenerateMeshGet1DElementsByIdTest(string fileName, int[] lineTags, int[] expectedLength)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            for (var i = 0; i < lineTags.Length; ++i)
            {
                var lines = controller.GetMeshEntities(1, lineTags[i], ref status).ToArray();
                Assert.AreEqual(expectedLength[i], lines.Length);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 1, 2, 4, 5, 6 }, new int[] { 112, 112, 72, 0, 0 })]
        [DataRow("Korpus_Nasosa.step", new int[] { 1, 2, 7, 292, 293 }, new int[] { 6, 10, 6, 0, 0 })]
        [DataRow("model.igs", new int[] { 3, 4, 5, 29, 30 }, new int[] { 2, 28, 2, 0, 0 })]
        [DataRow("sphere.geo", new int[] { 1, 2, 3, 14, 20 }, new int[] { 0, 0, 0, 13, 15 })]
        public void GenerateMeshGet2DElementsByIdTest(string fileName, int[] surfTags, int[] expectedLength)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            for (var i = 0; i < surfTags.Length; ++i)
            {
                var surfs = controller.GetMeshEntities(2, surfTags[i], ref status).ToArray();
                Assert.AreEqual(expectedLength[i], surfs.Length);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow("Korpus_Nasosa.step", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow("model.igs", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow("sphere.geo", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        public void GenerateMeshGetAllNodesTest(string fileName, int[] nodesDim, int[] paramsCount)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            long[] nodes;
            double[] local, parametric;
            for (var i = 0; i < nodesDim.Length; ++i)
            {
                controller.ModelMeshGetNodes(nodesDim[i], -1, true, true, out nodes, out local, out parametric);
                if (nodesDim[i] == -1)
                    Assert.AreEqual(0, parametric.Length);
                else
                {
                    Assert.AreEqual(0, local.Length % 3);
                    var totalPoints = local.Length / 3;
                    Assert.AreNotEqual(0, parametric.Length);
                    Assert.AreEqual(parametric.Length, totalPoints * paramsCount[i]);
                }
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 1, 2, 3 }, new int[] { 6, 10, 6 })]
        [DataRow("Korpus_Nasosa.step", new int[] { 1, 2, 3 }, new int[] { 3, 3, 3 })]
        [DataRow("model.igs",  new int[] { 1, 2, 3 }, new int[] { 8, 2, 8 })]
        [DataRow("sphere.geo", new int[] { 1, 2, 3 }, new int[] { 4, 4, 4 })]
        public void GenerateMeshGetLineNodesByTag(string fileName, int[] tags, int[] expectedLength)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            long[] nodes;
            double[] local, parametric;
            for (var i = 0; i < tags.Length; ++i)
            {
                controller.ModelMeshGetNodes(1, tags[i], true, true, out nodes, out local, out parametric);
                Assert.AreEqual(expectedLength[i], nodes.Length);
            }
        }

        [DataTestMethod]
        [DataRow("Korpus_Nasosa.step", new int[] { 170, 168, 166 }, new int[] { 4, 8, 8 })]
        [DataRow("model.igs", new int[] { 5, 3, 12 }, new int[] { 4, 4, 9 })]
        public void GenerateMeshGetSurfacesNodesByTag(string fileName, int[] tags, int[] expectedLength)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            long[] nodes;
            double[] local, parametric;
            for (var i = 0; i < tags.Length; ++i)
            {
                controller.ModelMeshGetNodes(2, tags[i], true, true, out nodes, out local, out parametric);
                Assert.AreEqual(expectedLength[i], nodes.Length);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", 75)]
        public void GenerateMeshGetElementsByCoordinates(string fileName, int expected)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var point = new double[] { -8.74248599355, 1.39424515076, 6.51327932777 };
            long[] elements;
            controller.ModelMeshGetElementsByCoordinates(point[0], point[1], point[2], 2, false, out elements);
            Assert.AreEqual(expected, elements[0]);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void AddRemoveFieldTest(string fileName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            var view = controller.gmshModelMeshFieldAdd("BoundaryLayer", 0, ref ierr);
            int[] tags;
            controller.ModelMeshFieldList(out tags);
            CollectionAssert.AreEqual(tags, new int[] { 0 });
            controller.gmshModelMeshFieldRemove(0, ref ierr);
            controller.ModelMeshFieldList(out tags);
            CollectionAssert.AreEqual(tags, new int[] { });
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void AddBoundaryLayerTest(string fileName)
        {
            var ierr = 0;
            var status = false;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var el2dCount = controller.GetMeshEntities(2, -1, ref status).ToArray().Length;
            var view = controller.gmshModelMeshFieldAdd("BoundaryLayer", 0, ref ierr);
            controller.gmshModelMeshFieldSetNumbers(0, "CurvesList", new double[] { 4.0, 5.0 }, (IntPtr)2, ref ierr);
            controller.gmshModelMeshFieldSetNumbers(0, "ExcludedSurfacesList", 
                                                    new double[] { 1.0, 2.0 }, (IntPtr)2, ref ierr);
            controller.gmshModelMeshFieldSetNumbers(0, "PointsList", new double[] { 2.0, 4.0 }, (IntPtr)2, ref ierr);
            controller.gmshModelMeshFieldSetNumber(0, "Thickness", 2.0 , ref ierr);
            controller.gmshModelMeshFieldSetAsBoundaryLayer(0, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var newEl2dCount = controller.GetMeshEntities(2, -1, ref status).ToArray().Length;
            Assert.AreNotEqual(el2dCount, newEl2dCount);
            controller.gmshModelMeshFieldRemove(0, ref ierr);
        }
    }
}