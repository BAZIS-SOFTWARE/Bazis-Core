using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using GmshApi.GmshController;
using System;
using Model;
using ModelInterfaces;
using ModelControllerInterfaces.GmshController;
using ModelInterfaces.ObjectsFinders;
using System.Collections.Generic;
using System.IO;
using GmshApi;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerMeshTests
    {
        private IGmshController controller = new GmshController();
        private static string modelFolder = @"..\..\..\Examples\GMSHAPI\";

        [TestInitialize]
        public void Init()
        {
            controller.Load(@"..\..\..\..\GmshApi\gmsh.dll");
        }

        [TestCleanup]
        public void Cleanup()
        {
            var ierr = 0;
            controller.Clear(ref ierr);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetNodes(string fileName)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(-1, -1, false);
            Assert.AreEqual(187, nodes.Count);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestRefineMesh(string fileName)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var nodesUntil = controller.GetNodes(-1, -1);
            controller.ModelMeshRefine(ref ierr);
            var nodesAfter = controller.GetNodes(-1, -1);
            Assert.IsTrue(nodesAfter.Count() > nodesUntil.Count());
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", 223, 46, 370, 707)]
        [DataRow("Korpus_Nasosa.step", 2152, 1438, 3802, 6992)]
        public void GetMeshObjects(string fileName, int expNodes, int expE1d, int expE2d, int expE3d)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(3, ref ierr);
            var result = controller.GetMeshObjects();
            Assert.AreEqual(expNodes, result.Item1.Count());
            Assert.AreEqual(expE1d, result.Item2.Count());
            Assert.AreEqual(expE2d, result.Item3.Count());
            Assert.AreEqual(expE3d, result.Item4.Count());
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestDensity(string fileName)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(2, -1, true);
            var untilElements = controller.GetMeshEntities(2, -1, nodes);
            controller.OptionSetNumber("Mesh.MeshSizeFactor", 0.5, ref ierr);
            controller.ModelMeshGenerate(1, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
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
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(dim, ref ierr);
            var lengthUntil = controller.GetNodes(dim).Count;
            controller.OptionSetNumber("Mesh.MeshSizeFactor", 0.5, ref ierr);
            controller.ModelMeshGenerate(1, ref ierr);
            controller.ModelMeshGenerate(dim, ref ierr);
            var lengthAfter = controller.GetNodes(dim).Count;
            Assert.AreEqual(true, lengthAfter > lengthUntil);
            controller.OptionSetNumber("Mesh.MeshSizeFactor", 1.0, ref ierr);
            controller.ModelMeshGenerate(1, ref ierr);
            controller.ModelMeshGenerate(dim, ref ierr);
            var lengthLast = controller.GetNodes(dim).Count;
            Assert.AreEqual(true, lengthLast < lengthAfter);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 1, 2, 3 }, new int[] { 112, 112, 74 })]
        public void GetSurfaceByIdTest(string fileName, int[] tags, int[] expLength)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
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
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var nodesCountUntil = controller.GetNodes(2, -1).Count;
            controller.OptionSetNumber("Mesh.Algorithm", 2, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var nodesCountAfter = controller.GetNodes(2, -1).Count;
            Assert.IsTrue(nodesCountAfter != nodesCountUntil);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TestSetPointsOnCurve(string fileName)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var nodesCountUntil = controller.GetNodes(2, -1).Count;
            controller.ModelMeshSetTransfiniteCurve(4, 10, "Progression", 1.5, ref ierr);
            controller.ModelMeshGenerate(1, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var nodesCountAfter = controller.GetNodes(2, -1).Count;
            Assert.IsTrue(nodesCountAfter != nodesCountUntil);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", 2)]
        public void DeleteSurfaceTest(string fileName, int surfaceId)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(2, surfaceId);
            var lengthBefore = controller.GetMeshEntities(2, surfaceId, nodes).ToArray().Length;
            controller.ModelMeshClear(new int[] { 2, surfaceId }, (IntPtr)2, ref ierr);
            nodes = controller.GetNodes(2, surfaceId);
            var lengthAfter = controller.GetMeshEntities(2, surfaceId, nodes).ToArray().Length;
            Assert.AreEqual(true, lengthBefore > lengthAfter);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void DeleteAllSurfaces(string fileName)
        {
            var ierr = 0; ;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var dimTags = new int[] { 2, 1, 2, 2, 2, 3, 2, 4 };
            controller.ModelMeshClear(dimTags, (IntPtr)dimTags.Length, ref ierr);
            var nodes = controller.GetNodes(2, -1);
            var length = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            Assert.IsTrue(length == 0);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new long[] { 206, 261, 251, 265, 210, 213 })]
        public void DeleteMeshElementsTest(string fileName, long[] elTags)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
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
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(1, ref ierr);
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
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
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
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
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
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(1, ref ierr);
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
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
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
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
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
            controller.Open(modelFolder + fileName, ref ierr);
            var view = controller.ModelMeshFieldAdd("BoundaryLayer", 0, ref ierr);
            int[] tags;
            controller.ModelMeshFieldList(out tags);
            CollectionAssert.AreEqual(tags, new int[] { 0 });
            controller.ModelMeshFieldRemove(0, ref ierr);
            controller.ModelMeshFieldList(out tags);
            CollectionAssert.AreEqual(tags, new int[] { });
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void AddBoundaryLayerTest(string fileName)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(2);
            var el2dCount = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            var view = controller.ModelMeshFieldAdd("BoundaryLayer", 0, ref ierr);
            controller.ModelMeshFieldSetNumbers(0, "CurvesList", new double[] { 4.0, 5.0 }, (IntPtr)2, ref ierr);
            controller.ModelMeshFieldSetNumbers(0, "ExcludedSurfacesList",
                                                    new double[] { 1.0, 2.0 }, (IntPtr)2, ref ierr);
            controller.ModelMeshFieldSetNumbers(0, "PointsList", new double[] { 2.0, 4.0 }, (IntPtr)2, ref ierr);
            controller.ModelMeshFieldSetNumber(0, "Thickness", 2.0, ref ierr);
            controller.ModelMeshFieldSetAsBoundaryLayer(0, ref ierr);
            controller.ModelMeshGenerate(1, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            nodes = controller.GetNodes(2);
            var newEl2dCount = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            Assert.AreNotEqual(el2dCount, newEl2dCount);
            controller.ModelMeshFieldRemove(0, ref ierr);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void TransfiniteSurfaceTest(string fileName)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var nodes = controller.GetNodes(2);
            var beforeCount = controller.GetMeshEntities(2, -1, nodes).Count;
            controller.ModelMeshGenerate(1, ref ierr);
            controller.ModelMeshSetTransfiniteSurface(1, "Left", new int[] { 4 }, ref ierr);
            Assert.IsTrue(ierr == 0);
            controller.ModelMeshGenerate(2, ref ierr);
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
            controller.Open(path, ref ierr);
            controller.ModelMeshGenerate(3, ref ierr);
            var results = controller.GetMeshObjects();
            modelData.ObjectData.NodeCollection.AddRange(results.Item1);
            modelData.ObjectData.E2DCollection.AddRange(results.Item3);
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
            controller.Open(modelFolder + fileName, ref ierr);
            var coords = new double[3];
            controller.ModelOccGetCenterOfMass(2, surfTag, ref coords[0], ref coords[1], ref coords[2], ref ierr);
            var status = controller.ModelIsInside(2, surfTag, coords, (IntPtr)3, 0, ref ierr);
            Assert.AreEqual(1,status);
            coords[1] -= 100;
            status = controller.ModelIsInside(2, surfTag, coords, (IntPtr)3, 0, ref ierr);
            Assert.AreEqual(0, status);
        }
        //Тест гарантирующий сброс трансфиниции кривой в состояние по умолчанию :)
        [DataTestMethod]
        [DataRow("Cilindr.step", 2)]
        public void ResetTransfinitionTest(string fileName, int curveTag)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshGenerate(1, ref ierr);
            var defaultNodeCount = controller.GetNodes(1, curveTag).Count;
            controller.ModelMeshClear(new int[0], (IntPtr)0, ref ierr);
            controller.ModelMeshSetTransfiniteCurve(curveTag, 20, "Progression", 1.0, ref ierr);
            controller.ModelMeshGenerate(1, ref ierr);
            var afterTransfNodeCount = controller.GetNodes(1, curveTag).Count;
            Assert.AreEqual(20, afterTransfNodeCount);
            controller.ModelMeshClear(new int[0], (IntPtr)0, ref ierr);
            controller.ModelMeshRemoveConstraints(new int[] { 1 , curveTag }, (IntPtr)2, ref ierr);
            controller.ModelMeshGenerate(1, ref ierr);
            var afterResetNodeCount = controller.GetNodes(1, curveTag).Count;
            Assert.AreEqual(defaultNodeCount, afterResetNodeCount);
        }

        //Тест фильтра Extend, проверяется последний аргумент(то что получили в gmsh) с актуальным значением
        //Берется ближайший 3д элемент у границы модели и ближайший 3д элемент в центре, предполагается,
        //что объем элементов около центральной точки будет больше объемов граничных элементов 
        [DataTestMethod]
        [DataRow("Cilindr.step", 15, 3, 139457)]
        public void ExtendFilterTest(string fileName, double distFromBound, double sizeMax, int expElements)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            var field = controller.ModelMeshFieldAdd("Extend", -1, ref ierr);
            int[] pointDimTags, curveDimTags, surfDimTags, volDimTags;
            controller.ModelGetGeometryEntities(out pointDimTags, 0);
            controller.ModelMeshSetSize(pointDimTags, (IntPtr)pointDimTags.Length, 0.4, ref ierr);
            controller.ModelGetGeometryEntities(out curveDimTags, 1);
            controller.ModelGetGeometryEntities(out surfDimTags, 2);
            var curveTags = curveDimTags.Where((v, i) => (i & 1) != 0)
                                        .Select(v => (double)v).ToArray();
            var surfTags = surfDimTags.Where((v, i) => (i & 1) != 0)
                                      .Select(v => (double)v).ToArray();
            controller.ModelMeshFieldSetNumbers(field, "CurvesList", curveTags, (IntPtr)curveTags.Length, ref ierr);
            controller.ModelMeshFieldSetNumbers(field, "SurfacesList", surfTags, (IntPtr)surfTags.Length, ref ierr);
            controller.ModelMeshFieldSetNumber(field, "DistMax", distFromBound, ref ierr);
            controller.ModelMeshFieldSetNumber(field, "SizeMax", sizeMax, ref ierr);
            controller.ModelMeshFieldSetAsBackgroundMesh(field, ref ierr);
            //Устанавливаем флаг, чтобы gmsh учитывал фильтр Extend и применял для сетки
            controller.OptionSetNumber("Mesh.MeshSizeExtendFromBoundary", -2, ref ierr);
            controller.ModelMeshGenerate(3, ref ierr);
            var list = controller.GetMeshObjects();
            controller.ModelMeshFieldRemove(field, ref ierr);
            Assert.AreEqual(expElements, list.Item4.Count);
            controller.ModelGetGeometryEntities(out volDimTags, 3);
            double xC = 0 , yC = 0, zC = 0;//Точка центра объема
            double xB = 0, yB = 0, zB = 0;//Точка на границе объема
            long[] centerElemTags, boundElemTags;
            controller.ModelOccGetCenterOfMass(3, volDimTags[1], ref xC, ref yC, ref zC, ref ierr);
            controller.ModelMeshGetElementsByCoordinates(xC, yC, zC, 3, false, out centerElemTags);
            controller.ModelOccGetCenterOfMass(0, pointDimTags[1], ref xB, ref yB, ref zB, ref ierr);
            controller.ModelMeshGetElementsByCoordinates(xB, yB, zB, 3, false, out boundElemTags);
            Assert.AreNotEqual(null, centerElemTags);
            Assert.AreNotEqual(null, boundElemTags);
            var centrElement =  ByNumberFinder.Find(list.Item4,(int)centerElemTags[0]);
            var boundElement = ByNumberFinder.Find(list.Item4,(int)boundElemTags[0]);
            Assert.AreNotEqual(null, centrElement);
            Assert.AreNotEqual(null, boundElement);
            var cVolume = centrElement.CalcVolume();
            var bVolume = boundElement.CalcVolume();
            Assert.IsTrue(cVolume > bVolume);
            /*
            controller.OptionSetNumber("Mesh.SaveGroupsOfNodes", 1, ref ierr);//Для визуального теста
            controller.Write(@"C:\Mesh\Cilindr.inp", ref ierr);*/
        }

        /// Проверка автоматического создания геометрической оболочки по готовой stl сетке
        [DataTestMethod]
        [DataRow("sphere.stl", Math.PI / 4, Math.PI)]
        [DataRow("box.stl", Math.PI / 3, Math.PI / 3)]
        public void BuildGeometryFromMeshTest(string fileName, double classifyAngleSurf, double classifyAngleCurve)
        {
            var ierr = 0;
            controller.Open(@"..\..\..\Examples\STL\" + fileName, ref ierr);
            controller.CreateGeometry(classifyAngleSurf, classifyAngleCurve);
            int[] dimTags;

            controller.ModelGetGeometryEntities(out dimTags, 2);
            dimTags = dimTags.Where((v, i) => (i & 1) == 1).ToArray();
            var tag = controller.ModelGeoAddSurfaceLoop(dimTags, (IntPtr)dimTags.Length, 1, ref ierr);
            
            // если нужно замешивать объем
            controller.ModelGeoAddVolume(new int[] { tag }, (IntPtr)1, -1, ref ierr);

            controller.ModelGeoSynchronize(ref ierr);//Обязательный вызов Synchronize
            controller.ModelGetGeometryEntities(out dimTags, 3);
            Assert.AreNotEqual(0, dimTags.Length);
            controller.ModelMeshGenerate(2, ref ierr);
            var list = controller.GetMeshObjects();

            var before = list.Item3.Count;
            Assert.AreNotEqual(0, before);
            controller.ModelGetGeometryEntities(out dimTags, 1);
            controller.ModelMeshSetTransfiniteCurve(dimTags[1], 50, "Progression", 1.0, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            list = controller.GetMeshObjects();
            var after = list.Item3.Count;
            Assert.AreNotEqual(after, before);
            //Проверка создания 3д элементов
            controller.ModelMeshGenerate(3, ref ierr);
            list = controller.GetMeshObjects();
            Assert.AreNotEqual(0, list.Item4.Count);
        }

        ///Проверяем построение геометрической 2D оболочки ручным созданием дискретных сущностей,
        ///проверяем построение сетки по построенной оболочке и перегенерацию сетки с увеличенной плотностью
        [DataTestMethod]
        public void BuildManualDiscrete2DGeometryTest()
        {
            var ierr = 0;
            controller.ModelAdd("Discrete2D", ref ierr);
            var localPoints = new double[] { -0.5, -0.5, 0,
                                              0.5, -0.5, 0,
                                              0.5,  0.5, 0,
                                             -0.5,  0.5, 0,
                                            };
            var nodeTags = new IntPtr[] { (IntPtr)1, (IntPtr)2, (IntPtr)3, (IntPtr)4 };
            var linesTags = new IntPtr[] { (IntPtr)1, (IntPtr)2, (IntPtr)3, (IntPtr)4, (IntPtr)1 };
            var trisTags = new IntPtr[] { (IntPtr)1, (IntPtr)2, (IntPtr)4, (IntPtr)2, (IntPtr)3, (IntPtr)4 };
            //Геометрические точки
            for (int i = 0; i < 4; ++i)
            {
                var index = i * 3;
                controller.ModelAddDiscreteEntity(0, i + 1, new int[0], ref ierr);
                controller.ModelSetCoordinates(i + 1, localPoints[index], localPoints[index + 1], 
                                               localPoints[index + 2], ref ierr);
            }
            int[] dimTags;
            controller.ModelGetGeometryEntities(out dimTags, 0);
            Assert.AreEqual(8, dimTags.Length);
            controller.ModelMeshSetSize(dimTags, (IntPtr)dimTags.Length, 1, ref ierr);
            //Кривые
            for (int i = 1; i < 5; ++i)
            {
                var last = i == 4 ? 1 : i + 1;
                controller.ModelAddDiscreteEntity(1, i, new int[] { i, last }, ref ierr);
            }
            controller.ModelGetGeometryEntities(out dimTags, 1);
            Assert.AreEqual(8, dimTags.Length);
            //Поверхности
            controller.ModelAddDiscreteEntity(2, 1, new int[] { 1, 2, 3, 4 }, ref ierr);
            controller.ModelGetGeometryEntities(out dimTags, 2);
            Assert.AreEqual(2, dimTags.Length);
            controller.ModelMeshAddNodes(2, 1, nodeTags, localPoints, new double[0], ref ierr);
            for(var i = 0; i < 4; ++i)
            {
                controller.ModelMeshAddElementsByType(i + 1, 15, new IntPtr[0], new IntPtr[] { nodeTags[i] }, ref ierr);
                controller.ModelMeshAddElementsByType(i + 1, 1, new IntPtr[0], new IntPtr[] { linesTags[i], linesTags[i + 1] },
                                                      ref ierr);
            }
            controller.ModelMeshAddElementsByType(1, 2, new IntPtr[0], trisTags, ref ierr);
            controller.ModelGetGeometryEntities(out dimTags, -1);
            controller.ModelMeshReclassifyNodes(ref ierr);
            controller.ModelMeshCreateGeometry(dimTags, (IntPtr)dimTags.Length, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            var list = controller.GetMeshObjects();
            var before = list.Item3.Count;
            Assert.AreNotEqual(0, before);
            controller.ModelGetGeometryEntities(out dimTags, 0);
            controller.ModelMeshSetSize(dimTags, (IntPtr)dimTags.Length, 0.4, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            list = controller.GetMeshObjects();
            var after = list.Item3.Count;
            Assert.IsTrue(after > before);
        }

        [DataTestMethod]
        //[DataRow("T_joint.geo")]
        [DataRow("Build1.step")]
        public void AutomaticSurfaceTransfinitionTest(string fileName)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            int[] surfaces;
            controller.ModelGetGeometryEntities(out surfaces, 2);
            controller.ModelMeshSetTransfiniteAutomatic(surfaces, Math.PI, true, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            string actual;
            controller.LoggerGetLastError(out actual);
            Assert.AreEqual("", actual);
            /*//Визуальный тест
            var name = Path.GetFileNameWithoutExtension(fileName);
            controller.Write($@"D:\{name}.inp", ref ierr);*/
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GeoWriterTest(string fileName)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            var geoWriter = new GmshGeoWriter(controller);

            controller.ModelMeshSetSize(new int[] { 0, 1, 0, 2 }, (IntPtr)4, 0.3, ref ierr);
            controller.ModelMeshSetSize(new int[] { 0, 3 }, (IntPtr)2, 0.7, ref ierr);
            controller.ModelMeshSetSize(new int[] { 0, 4 }, (IntPtr)2, 0.95, ref ierr);

            controller.ModelSetAttribute("transfinite 2", new string[] {"7","Bump","1.23"}, (IntPtr)3, ref ierr);
            controller.ModelMeshSetTransfiniteCurve(2, 7, "Bump", 1.23, ref ierr);

            controller.ModelSetAttribute("transfinite 6", new string[] { "7", "Bump", "1.23" }, (IntPtr)3, ref ierr);
            controller.ModelMeshSetTransfiniteCurve(6, 7, "Bump", 1.23, ref ierr);

            controller.ModelSetAttribute("transfinite 4", new string[] { "16", "Beta", "1.7" }, (IntPtr)3, ref ierr);
            controller.ModelMeshSetTransfiniteCurve(4, 16, "Beta", 1.7, ref ierr);

            controller.ModelSetAttribute("transfinite 5", new string[] { "5", "Progression", "0.7" }, (IntPtr)3, ref ierr);
            controller.ModelMeshSetTransfiniteCurve(5, 5, "Progression", 0.7, ref ierr);

            controller.ModelSetAttribute("transfinite 3", new string[] { "", "Progression", "0.7" }, (IntPtr)3, ref ierr);

            geoWriter.WriteSettingsToGeo();

            controller.Clear(ref ierr);
            controller.Open(geoWriter.ResultFileName, ref ierr);

            Assert.AreEqual(0, ierr);
        }
    }
}