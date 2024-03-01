using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using GmshApi.GmshController;
using System;
using Functions.Sort;
using Model;
using ModelInterfaces;
using System.Collections.Generic;
using Model.MeshObjects;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerMeshTests
    {
        private GmshController controller = new GmshController(@"..\..\..\..\GmshApi\gmsh.dll");
        private static string modelFolder = @"..\..\..\Examples\GMSHAPI\";

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
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(-1, -1, false);
            Assert.AreEqual(187, nodes.Count);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestRefineMesh(string fileName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodesUntil = controller.GetNodes(-1, -1);
            controller.gmshModelMeshRefine(ref ierr);
            var nodesAfter = controller.GetNodes(-1, -1);
            Assert.IsTrue(nodesAfter.Count() > nodesUntil.Count());
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", 223, 46, 370, 707)]
        [DataRow("Korpus_Nasosa.step", 2152, 1438, 3802, 6992)]
        public void GetMeshObjects(string fileName, int expNodes, int expE1d, int expE2d, int expE3d)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(3, ref ierr);
            controller.GetMeshObjects();
            Assert.AreEqual(expNodes, controller.Nodes.Count());
            Assert.AreEqual(expE1d, controller.E1D.Count());
            Assert.AreEqual(expE2d, controller.E2D.Count());
            Assert.AreEqual(expE3d, controller.E3D.Count());
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestDensity(string fileName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(2, -1, true);
            var untilElements = controller.GetMeshEntities(2, -1, nodes);
            controller.gmshOptionSetNumber("Mesh.MeshSizeFactor", 0.5, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            nodes = controller.GetNodes(2, -1, true);
            var afterElements = controller.GetMeshEntities(2, -1, nodes);
            Assert.IsTrue(untilElements.Count() < afterElements.Count());
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", 1)]
        [DataRow("Cilindr.step", 2)]
        [DataRow("Cilindr.step", 3)]
        public void GetDensityForElementNodes(string fileName, int dim)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(dim, ref ierr);
            var lengthUntil = controller.GetNodes(dim).Count;
            controller.gmshOptionSetNumber("Mesh.MeshSizeFactor", 0.5, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(dim, ref ierr);
            var lengthAfter = controller.GetNodes(dim).Count;
            Assert.AreEqual(true, lengthAfter > lengthUntil);
            controller.gmshOptionSetNumber("Mesh.MeshSizeFactor", 1.0, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(dim, ref ierr);
            var lengthLast = controller.GetNodes(dim).Count;
            Assert.AreEqual(true, lengthLast < lengthAfter);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 1, 2, 3 }, new int[] { 112, 112, 74 })]
        public void GetSurfaceByIdTest(string fileName, int[] tags, int[] expLength)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            for (var i = 0; i < tags.Length; ++i)
            {
                var nodes = controller.GetNodes(2, tags[i], true);
                var actual = controller.GetMeshEntities(2, tags[i], nodes).ToArray();
                Assert.AreEqual(expLength[i], actual.Length);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestChangeTriangulationAlgorithm2D(string fileName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodesCountUntil = controller.GetNodes(2, -1).Count;
            controller.gmshOptionSetNumber("Mesh.Algorithm", 2, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodesCountAfter = controller.GetNodes(2, -1).Count;
            Assert.IsTrue(nodesCountAfter != nodesCountUntil);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestSetPointsOnCurve(string fileName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodesCountUntil = controller.GetNodes(2, -1).Count;
            controller.gmshModelMeshSetTransfiniteCurve(4, 10, "Progression", 1.5, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodesCountAfter = controller.GetNodes(2, -1).Count;
            Assert.IsTrue(nodesCountAfter != nodesCountUntil);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", 2)]
        public void DeleteSurfaceTest(string fileName, int surfaceId)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(2, surfaceId);
            var lengthBefore = controller.GetMeshEntities(2, surfaceId, nodes).ToArray().Length;
            controller.gmshModelMeshClear(new int[] { 2, surfaceId }, (IntPtr)2, ref ierr);
            nodes = controller.GetNodes(2, surfaceId);
            var lengthAfter = controller.GetMeshEntities(2, surfaceId, nodes).ToArray().Length;
            Assert.AreEqual(true, lengthBefore > lengthAfter);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void DeleteAllSurfaces(string fileName)
        {
            var ierr = 0; ;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var dimTags = new int[] { 2, 1, 2, 2, 2, 3, 2, 4 };
            controller.gmshModelMeshClear(dimTags, (IntPtr)dimTags.Length, ref ierr);
            var nodes = controller.GetNodes(2, -1);
            var length = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            Assert.IsTrue(length == 0);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new long[] { 206, 261, 251, 265, 210, 213 })]
        public void DeleteMeshElementsTest(string fileName, long[] elTags)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(2, -1);
            var lengthBefore = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            controller.DeleteMeshElements(elTags);
            nodes = controller.GetNodes(2, -1);
            var lengthAfter = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
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
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            for (var i = 0; i < lineTags.Length; ++i)
            {
                var nodes = controller.GetNodes(1, lineTags[i], true);
                var lines = controller.GetMeshEntities(1, lineTags[i], nodes).ToArray();
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
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            for (var i = 0; i < surfTags.Length; ++i)
            {
                var nodes = controller.GetNodes(2, surfTags[i], true);
                var surfs = controller.GetMeshEntities(2, surfTags[i], nodes).ToArray();
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
        [DataRow("model.igs", new int[] { 1, 2, 3 }, new int[] { 8, 2, 8 })]
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
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(2);
            var el2dCount = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            var view = controller.gmshModelMeshFieldAdd("BoundaryLayer", 0, ref ierr);
            controller.gmshModelMeshFieldSetNumbers(0, "CurvesList", new double[] { 4.0, 5.0 }, (IntPtr)2, ref ierr);
            controller.gmshModelMeshFieldSetNumbers(0, "ExcludedSurfacesList",
                                                    new double[] { 1.0, 2.0 }, (IntPtr)2, ref ierr);
            controller.gmshModelMeshFieldSetNumbers(0, "PointsList", new double[] { 2.0, 4.0 }, (IntPtr)2, ref ierr);
            controller.gmshModelMeshFieldSetNumber(0, "Thickness", 2.0, ref ierr);
            controller.gmshModelMeshFieldSetAsBoundaryLayer(0, ref ierr);
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            nodes = controller.GetNodes(2);
            var newEl2dCount = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            Assert.AreNotEqual(el2dCount, newEl2dCount);
            controller.gmshModelMeshFieldRemove(0, ref ierr);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TransfiniteSurfaceTest(string fileName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(2);
            var beforeCount = controller.GetMeshEntities(2, -1, nodes).Count;
            controller.gmshModelMeshGenerate(1, ref ierr);
            controller.gmshModelMeshSetTransfiniteSurface(1, "Left", new int[] { 4 }, (IntPtr)1, ref ierr);
            Assert.IsTrue(ierr == 0);
            controller.gmshModelMeshGenerate(2, ref ierr);
            nodes = controller.GetNodes(2);
            var afterCount = controller.GetMeshEntities(2, -1, nodes).Count;
            Assert.IsTrue(afterCount != beforeCount);
        }
        //Тест гарантирующий удаление элементов в случае удаления общего узла (проверка единой области памяти!)
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\GMSHAPI\simple.geo", 347, 673)]
        public void DeleteNodeTest(string path, int delNodeNumber, int expElementsAfterDel)
        {
            var ierr = 0;
            var modelData = new ModelData();
            controller.gmshOpen(path, ref ierr);
            controller.gmshModelMeshGenerate(3, ref ierr);
            controller.GetMeshObjects();
            modelData.ObjectData.NodeCollection.AddRange(controller.Nodes);
            modelData.ObjectData.E2DCollection.AddRange(controller.E2D);
            var delNode = modelData.ObjectData.Find(ObjType.Узел, delNodeNumber);
            delNode.ExistState = false;
            modelData.ObjectData.E2DCollection.ClearNotExisted();
            Assert.AreEqual(expElementsAfterDel, modelData.ObjectData.E2DCollection.Count);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step", 1)]
        [DataRow("Cilindr.step", 2)]
        [DataRow("Cilindr.step", 3)]
        [DataRow("Cilindr.step", 4)]
        public void CenterOfSurfaceTest(string fileName, int surfTag)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            var coords = new double[3];
            controller.gmshModelOccGetCenterOfMass(2, surfTag, ref coords[0], ref coords[1], ref coords[2], ref ierr);
            var status = controller.gmshModelIsInside(2, surfTag, coords, (IntPtr)3, 0, ref ierr);
            Assert.AreEqual(1,status);
            coords[1] -= 100;
            status = controller.gmshModelIsInside(2, surfTag, coords, (IntPtr)3, 0, ref ierr);
            Assert.AreEqual(0, status);
        }
    }
}