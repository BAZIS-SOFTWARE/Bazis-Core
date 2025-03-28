using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using Model;
using Model.Interfaces;
using Model.Interfaces.ObjectsFinders;
using GmshApi;
using System.IO;
using ModelController.GmshController;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerMeshTests
    {
        private IGmshController controller = new GmshController();
        private static string modelFolder = @"..\..\..\Examples";

        [TestInitialize]
        public void Init()
        {
            controller.Load(@"..\..\..\..\GmshApi\gmsh.dll");
        }

        [TestCleanup]
        public void Cleanup()
        {
            controller.Gmsh.Clear();
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step")]
        public void GetNodes(string fileName)
        {

            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodes = controller.GetNodes(-1, -1, false);
            Assert.AreEqual(187, nodes.Count);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step")]
        public void TestRefineMesh(string fileName)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodesUntil = controller.GetNodes(-1, -1);
            controller.Gmsh.Model.Mesh.Refine();
            var nodesAfter = controller.GetNodes(-1, -1);
            Assert.IsTrue(nodesAfter.Count() > nodesUntil.Count());
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", 223, 46, 370, 707)]
        [DataRow(@"\CAD\nasos.step", 2152, 1438, 3802, 6992)]
        public void GetMeshObjects(string fileName, int expNodes, int expE1d, int expE2d, int expE3d)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(3);
            var result = controller.GetMeshObjects();
            Assert.AreEqual(expNodes, result.Item1.Count());
            Assert.AreEqual(expE1d, result.Item2.Count());
            Assert.AreEqual(expE2d, result.Item3.Count());
            Assert.AreEqual(expE3d, result.Item4.Count());
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step")]
        public void TestDensity(string fileName)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodes = controller.GetNodes(2, -1, true);
            var untilElements = controller.GetMeshEntities(2, -1, nodes);
            controller.Gmsh.Option.SetNumber("Mesh.MeshSizeFactor", 0.5);
            controller.Gmsh.Model.Mesh.Generate(1);
            controller.Gmsh.Model.Mesh.Generate(2);
            nodes = controller.GetNodes(2, -1, true);
            var afterElements = controller.GetMeshEntities(2, -1, nodes);
            Assert.IsTrue(untilElements.Count() < afterElements.Count());
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", 1)]
        [DataRow(@"\CAD\Cilindr.step", 2)]
        [DataRow(@"\CAD\Cilindr.step", 3)]
        public void GetDensityForElementNodes(string fileName, int dim)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(dim);
            var lengthUntil = controller.GetNodes(dim).Count;
            controller.Gmsh.Option.SetNumber("Mesh.MeshSizeFactor", 0.5);
            controller.Gmsh.Model.Mesh.Generate(1);
            controller.Gmsh.Model.Mesh.Generate(dim);
            var lengthAfter = controller.GetNodes(dim).Count;
            Assert.AreEqual(true, lengthAfter > lengthUntil);
            controller.Gmsh.Option.SetNumber("Mesh.MeshSizeFactor", 1.0);
            controller.Gmsh.Model.Mesh.Generate(1);
            controller.Gmsh.Model.Mesh.Generate(dim);
            var lengthLast = controller.GetNodes(dim).Count;
            Assert.AreEqual(true, lengthLast < lengthAfter);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 1, 2, 3 }, new int[] { 112, 112, 74 })]
        public void GetSurfaceByIdTest(string fileName, int[] tags, int[] expLength)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            for (var i = 0; i < tags.Length; ++i)
            {
                var nodes = controller.GetNodes(2, tags[i], true);
                var actual = controller.GetMeshEntities(2, tags[i], nodes).ToArray();
                Assert.AreEqual(expLength[i], actual.Length);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step")]
        public void TestChangeTriangulationAlgorithm2D(string fileName)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodesCountUntil = controller.GetNodes(2, -1).Count;
            controller.Gmsh.Option.SetNumber("Mesh.Algorithm", 2);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodesCountAfter = controller.GetNodes(2, -1).Count;
            Assert.IsTrue(nodesCountAfter != nodesCountUntil);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step")]
        public void TestSetPointsOnCurve(string fileName)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodesCountUntil = controller.GetNodes(2, -1).Count;
            controller.Gmsh.Model.Mesh.SetTransfiniteCurve(4, 10, MeshType.Progression, 1.5);
            controller.Gmsh.Model.Mesh.Generate(1);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodesCountAfter = controller.GetNodes(2, -1).Count;
            Assert.IsTrue(nodesCountAfter != nodesCountUntil);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", 2)]
        public void DeleteSurfaceTest(string fileName, int surfaceId)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodes = controller.GetNodes(2, surfaceId);
            var lengthBefore = controller.GetMeshEntities(2, surfaceId, nodes).ToArray().Length;
            controller.Gmsh.Model.Mesh.Clear(new int[] { 2, surfaceId });
            nodes = controller.GetNodes(2, surfaceId);
            var lengthAfter = controller.GetMeshEntities(2, surfaceId, nodes).ToArray().Length;
            Assert.AreEqual(true, lengthBefore > lengthAfter);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step")]
        public void DeleteAllSurfaces(string fileName)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var dimTags = new int[] { 2, 1, 2, 2, 2, 3, 2, 4 };
            controller.Gmsh.Model.Mesh.Clear(dimTags);
            var nodes = controller.GetNodes(2, -1);
            var length = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            Assert.IsTrue(length == 0);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new long[] { 206, 261, 251, 265, 210, 213 })]
        public void DeleteMeshElementsTest(string fileName, long[] elTags)
        {
            var tags = elTags.Select(v => (IntPtr)v).ToArray();
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodes = controller.GetNodes(2, -1);
            var lengthBefore = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            controller.DeleteMeshElements(tags);
            nodes = controller.GetNodes(2, -1);
            var lengthAfter = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            Assert.IsTrue(lengthBefore > lengthAfter);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 7, 8, 1, 3, 6 }, new int[] { 0, 0, 5, 5, 9 })]
        [DataRow(@"\CAD\nasos.step", new int[] { 700, 720, 670, 667, 663 }, new int[] { 0, 0, 5, 2, 9 })]
        [DataRow(@"\CAD\block.igs", new int[] { 124, 126, 140, 136, 137 }, new int[] { 1, 1, 0, 5, 6 })]
        [DataRow(@"\GMSHAPI\sphere.geo", new int[] { 13, 14, 1, 2, 3 }, new int[] { 0, 0, 3, 3, 3 })]

        public void GenerateMeshGet1DElementsByIdTest(string fileName, int[] lineTags, int[] expectedLength)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(1);
            for (var i = 0; i < lineTags.Length; ++i)
            {
                var nodes = controller.GetNodes(1, lineTags[i], true);
                var lines = controller.GetMeshEntities(1, lineTags[i], nodes).ToArray();
                Assert.AreEqual(expectedLength[i], lines.Length);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 1, 2, 4, 5, 6 }, new int[] { 112, 112, 72, 0, 0 })]
        [DataRow(@"\CAD\nasos.step", new int[] { 1, 2, 7, 292, 293 }, new int[] { 6, 10, 6, 0, 0 })]
        [DataRow(@"\CAD\block.igs", new int[] { 3, 4, 5, 29, 30 }, new int[] { 2, 28, 2, 0, 0 })]
        [DataRow(@"\GMSHAPI\sphere.geo", new int[] { 1, 2, 3, 14, 20 }, new int[] { 0, 0, 0, 13, 15 })]
        public void GenerateMeshGet2DElementsByIdTest(string fileName, int[] surfTags, int[] expectedLength)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            for (var i = 0; i < surfTags.Length; ++i)
            {
                var nodes = controller.GetNodes(2, surfTags[i], true);
                var surfs = controller.GetMeshEntities(2, surfTags[i], nodes).ToArray();
                Assert.AreEqual(expectedLength[i], surfs.Length);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow(@"\CAD\nasos.step", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow(@"\CAD\block.igs", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow(@"\GMSHAPI\sphere.geo", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        public void GenerateMeshGetAllNodesTest(string fileName, int[] nodesDim, int[] paramsCount)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            for (var i = 0; i < nodesDim.Length; ++i)
            {
                var data = controller.Gmsh.Model.Mesh.GetNodes(nodesDim[i], -1, true, true);
                if (nodesDim[i] == -1)
                    Assert.AreEqual(0, data.Item3.Length);
                else
                {
                    Assert.AreEqual(0, data.Item2.Length % 3);
                    var totalPoints = data.Item2.Length / 3;
                    Assert.AreNotEqual(0, data.Item3.Length);
                    Assert.AreEqual(data.Item3.Length, totalPoints * paramsCount[i]);
                }
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 1, 2, 3 }, new int[] { 6, 10, 6 })]
        [DataRow(@"\CAD\nasos.step", new int[] { 1, 2, 3 }, new int[] { 3, 3, 3 })]
        [DataRow(@"\CAD\block.igs", new int[] { 1, 2, 3 }, new int[] { 8, 2, 8 })]
        [DataRow(@"\GMSHAPI\sphere.geo", new int[] { 1, 2, 3 }, new int[] { 4, 4, 4 })]
        public void GenerateMeshGetLineNodesByTag(string fileName, int[] tags, int[] expectedLength)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(1);
            for (var i = 0; i < tags.Length; ++i)
            {
                var data = controller.Gmsh.Model.Mesh.GetNodes(1, tags[i], true, true);
                Assert.AreEqual(expectedLength[i], data.Item1.Length);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\nasos.step", new int[] { 170, 168, 166 }, new int[] { 4, 8, 8 })]
        [DataRow(@"\CAD\block.igs", new int[] { 5, 3, 12 }, new int[] { 4, 4, 9 })]
        public void GenerateMeshGetSurfacesNodesByTag(string fileName, int[] tags, int[] expectedLength)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            for (var i = 0; i < tags.Length; ++i)
            {
                var data = controller.Gmsh.Model.Mesh.GetNodes(2, tags[i], true, true);
                Assert.AreEqual(expectedLength[i], data.Item1.Length);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", 75)]
        public void GenerateMeshGetElementsByCoordinates(string fileName, int expected)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var point = new double[] { -8.74248599355, 1.39424515076, 6.51327932777 };
            var elements = controller.Gmsh.Model.Mesh.GetElementsByCoordinates(point[0], point[1], point[2], 2, false);
            Assert.AreEqual(expected, (int)elements[0]);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step")]
        public void AddRemoveFieldTest(string fileName)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            var view = controller.Gmsh.Model.Mesh.Field.Add(FieldType.BoundaryLayer);
            var tags = controller.Gmsh.Model.Mesh.Field.List();
            CollectionAssert.AreEqual(tags, new int[] { view });
            controller.Gmsh.Model.Mesh.Field.Remove(view);
            tags = controller.Gmsh.Model.Mesh.Field.List();
            CollectionAssert.AreEqual(tags, new int[] { });
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\CAD\cilindr.step", DisplayName = "AddBoundaryLayer for \"cilindr.step\"")]
        public void AddBoundaryLayerTest(string fullFileName)
        {
            controller.Gmsh.Open(fullFileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodes = controller.GetNodes(2);
            var el2dCount = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            var view = controller.Gmsh.Model.Mesh.Field.Add(FieldType.BoundaryLayer);
            controller.Gmsh.Model.Mesh.Field.SetNumbers(view, BoundaryLayerOptions.CurvesList.ToString(), new double[] { 4.0, 5.0 });
            controller.Gmsh.Model.Mesh.Field.SetNumbers(view, BoundaryLayerOptions.ExcludedSurfacesList.ToString(),
                                                    new double[] { 1.0, 2.0 });
            controller.Gmsh.Model.Mesh.Field.SetNumbers(view, BoundaryLayerOptions.PointsList.ToString(), new double[] { 2.0, 4.0 });
            controller.Gmsh.Model.Mesh.Field.SetNumber(view, BoundaryLayerOptions.Thickness.ToString(), 2.0);
            controller.Gmsh.Model.Mesh.Field.SetAsBoundaryLayer(view);
            controller.Gmsh.Model.Mesh.Generate(1);
            controller.Gmsh.Model.Mesh.Generate(2);
            nodes = controller.GetNodes(2);
            var newEl2dCount = controller.GetMeshEntities(2, -1, nodes).ToArray().Length;
            Assert.AreNotEqual(el2dCount, newEl2dCount);
            controller.Gmsh.Model.Mesh.Field.Remove(view);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step")]
        public void TransfiniteSurfaceTest(string fileName)
        {
            var ierr = 0;
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(2);
            var nodes = controller.GetNodes(2);
            var beforeCount = controller.GetMeshEntities(2, -1, nodes).Count;
            controller.Gmsh.Model.Mesh.Generate(1);
            controller.Gmsh.Model.Mesh.SetTransfiniteSurface(1);
            var error = controller.Gmsh.Logger.GetLastError();
            Assert.IsTrue(string.IsNullOrEmpty(error));
            controller.Gmsh.Model.Mesh.Generate(2);
            nodes = controller.GetNodes(2);
            var afterCount = controller.GetMeshEntities(2, -1, nodes).Count;
            Assert.IsTrue(afterCount != beforeCount);
        }
        //Тест гарантирующий удаление элементов в случае удаления общего узла (проверка единой области памяти!)
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\GMSHAPI\simple.geo", 347, 673)]
        public void DeleteNodeTest(string path, int delNodeNumber, int expElementsAfterDel)
        {
            var modelData = new ModelData();
            controller.Gmsh.Open(path);
            controller.Gmsh.Model.Mesh.Generate(3);
            var results = controller.GetMeshObjects();

            results.Item1.ForEach(x => modelData.ObjectData.NodesSet.Add(x.Number, x));
            modelData.ObjectData.E2DCollection.AddRange(ObjType.Элемент2D.ToString(), results.Item3);
            
            //act
            modelData.ObjectData.Remove(ObjType.Узел, delNodeNumber);   
            modelData.ObjectData.E2DCollection.ClearNotExisted();

            //assert
            Assert.AreEqual(expElementsAfterDel, modelData.ObjectData.E2DCollection.Count);
        }
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\CAD\cilindr.step", 1)]
        [DataRow(@"..\..\..\Examples\CAD\cilindr.step", 2)]
        [DataRow(@"..\..\..\Examples\CAD\cilindr.step", 3)]
        [DataRow(@"..\..\..\Examples\CAD\cilindr.step", 4)]
        public void CenterOfSurfaceTest(string fullFileName, int surfTag)
        {
            controller.Gmsh.Open(fullFileName);
            var data = controller.Gmsh.Model.Occ.GetCenterOfMass(2, surfTag);
            var coords = new double[] { data.Item1, data.Item2, data.Item3 };
            var status = controller.Gmsh.Model.IsInside(2, surfTag, coords);
            Assert.AreEqual(1, status);
            coords[1] -= 100;
            status = controller.Gmsh.Model.IsInside(2, surfTag, coords);
            Assert.AreEqual(0, status);
        }
        //Тест гарантирующий сброс трансфиниции кривой в состояние по умолчанию :)
        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", 2)]
        public void ResetTransfinitionTest(string fileName, int curveTag)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(1);
            var defaultNodeCount = controller.GetNodes(1, curveTag).Count;
            controller.Gmsh.Model.Mesh.Clear();
            controller.Gmsh.Model.Mesh.SetTransfiniteCurve(curveTag, 20);
            controller.Gmsh.Model.Mesh.Generate(1);
            var afterTransfNodeCount = controller.GetNodes(1, curveTag).Count;
            Assert.AreEqual(20, afterTransfNodeCount);
            controller.Gmsh.Model.Mesh.Clear();
            controller.Gmsh.Model.Mesh.RemoveConstraints(new int[] { 1 , curveTag });
            controller.Gmsh.Model.Mesh.Generate(1);
            var afterResetNodeCount = controller.GetNodes(1, curveTag).Count;
            Assert.AreEqual(defaultNodeCount, afterResetNodeCount);
        }

        //Тест фильтра Extend, проверяется последний аргумент(то что получили в gmsh) с актуальным значением
        //Берется ближайший 3д элемент у границы модели и ближайший 3д элемент в центре, предполагается,
        //что объем элементов около центральной точки будет больше объемов граничных элементов 
        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", 15, 3, 139457)]
        public void ExtendFilterTest(string fileName, double distFromBound, double sizeMax, int expElements)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            var field = controller.Gmsh.Model.Mesh.Field.Add(FieldType.Extend);
            var pointDimTags = controller.Gmsh.Model.GetEntities(0);
            controller.Gmsh.Model.Mesh.SetSize(pointDimTags, 0.4);
            var curveDimTags = controller.Gmsh.Model.GetEntities(1);
            var surfDimTags = controller.Gmsh.Model.GetEntities(2);
            var curveTags = curveDimTags.Where((v, i) => (i & 1) != 0)
                                        .Select(v => (double)v).ToArray();
            var surfTags = surfDimTags.Where((v, i) => (i & 1) != 0)
                                      .Select(v => (double)v).ToArray();
            controller.Gmsh.Model.Mesh.Field.SetNumbers(field, ExtendOptions.CurvesList.ToString(), curveTags);
            controller.Gmsh.Model.Mesh.Field.SetNumbers(field, ExtendOptions.SurfacesList.ToString(), surfTags);
            controller.Gmsh.Model.Mesh.Field.SetNumber(field, ExtendOptions.DistMax.ToString(), distFromBound);
            controller.Gmsh.Model.Mesh.Field.SetNumber(field, ExtendOptions.SizeMax.ToString(), sizeMax);
            controller.Gmsh.Model.Mesh.Field.SetAsBackgroundMesh(field);
            //Устанавливаем флаг, чтобы gmsh учитывал фильтр Extend и применял для сетки
            controller.Gmsh.Option.SetNumber("Mesh.MeshSizeExtendFromBoundary", -2);
            controller.Gmsh.Model.Mesh.Generate(3);
            var list = controller.GetMeshObjects();
            controller.Gmsh.Model.Mesh.Field.Remove(field);
            Assert.AreEqual(expElements, list.Item4.Count);
            var volDimTags = controller.Gmsh.Model.GetEntities(3);

            var volCenter = controller.Gmsh.Model.Occ.GetCenterOfMass(3, volDimTags[1]);
            var centerElemTags = controller.Gmsh.Model.Mesh.GetElementsByCoordinates(volCenter.Item1, volCenter.Item2, volCenter.Item3, 3, false);
            var boundCoords = controller.Gmsh.Model.Occ.GetCenterOfMass(0, pointDimTags[1]);
            var boundElemTags = controller.Gmsh.Model.Mesh.GetElementsByCoordinates(boundCoords.Item1, boundCoords.Item2, boundCoords.Item3, 3, false);
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
            controller.Gmsh.Open(@"..\..\..\Examples\STL\" + fileName);
            controller.CreateGeometry(classifyAngleSurf, classifyAngleCurve);

            var dimTags = controller.Gmsh.Model.GetEntities(2);
            dimTags = dimTags.Where((v, i) => (i & 1) == 1).ToArray();
            var tag = controller.Gmsh.Model.Geo.AddSurfaceLoop(dimTags);
            
            // если нужно замешивать объем
            controller.Gmsh.Model.Geo.AddVolume(new int[] { tag });

            controller.Gmsh.Model.Geo.Synchronize();//Обязательный вызов Synchronize
            dimTags = controller.Gmsh.Model.GetEntities(3);
            Assert.AreNotEqual(0, dimTags.Length);
            controller.Gmsh.Model.Mesh.Generate(2);
            var list = controller.GetMeshObjects();

            var before = list.Item3.Count;
            Assert.AreNotEqual(0, before);
            dimTags = controller.Gmsh.Model.GetEntities(1);
            controller.Gmsh.Model.Mesh.SetTransfiniteCurve(dimTags[1], 50);
            controller.Gmsh.Model.Mesh.Generate(2);
            list = controller.GetMeshObjects();
            var after = list.Item3.Count;
            Assert.AreNotEqual(after, before);
            //Проверка создания 3д элементов
            controller.Gmsh.Model.Mesh.Generate(3);
            list = controller.GetMeshObjects();
            Assert.AreNotEqual(0, list.Item4.Count);
        }

        ///Проверяем построение геометрической 2D оболочки ручным созданием дискретных сущностей,
        ///проверяем построение сетки по построенной оболочке и перегенерацию сетки с увеличенной плотностью
        [DataTestMethod]
        public void BuildManualDiscrete2DGeometryTest()
        {
            controller.Gmsh.Model.Add("Discrete2D");
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
                controller.Gmsh.Model.AddDiscreteEntity(0, i + 1);
                controller.Gmsh.Model.SetCoordinates(i + 1, localPoints[index], localPoints[index + 1], localPoints[index + 2]);
            }
            var dimTags = controller.Gmsh.Model.GetEntities(0);
            Assert.AreEqual(8, dimTags.Length);
            controller.Gmsh.Model.Mesh.SetSize(dimTags, 1);
            //Кривые
            for (int i = 1; i < 5; ++i)
            {
                var last = i == 4 ? 1 : i + 1;
                controller.Gmsh.Model.AddDiscreteEntity(1, i, new int[] { i, last });
            }
            dimTags = controller.Gmsh.Model.GetEntities(1);
            Assert.AreEqual(8, dimTags.Length);
            //Поверхности
            controller.Gmsh.Model.AddDiscreteEntity(2, 1, new int[] { 1, 2, 3, 4 });
            dimTags = controller.Gmsh.Model.GetEntities(2);
            Assert.AreEqual(2, dimTags.Length);
            controller.Gmsh.Model.Mesh.AddNodes(2, 1, nodeTags, localPoints);
            for(var i = 0; i < 4; ++i)
            {
                controller.Gmsh.Model.Mesh.AddElementsByType(i + 1, 15, new IntPtr[0], new IntPtr[] { nodeTags[i] });
                controller.Gmsh.Model.Mesh.AddElementsByType(i + 1, 1, new IntPtr[0], new IntPtr[] { linesTags[i], linesTags[i + 1] });
            }
            controller.Gmsh.Model.Mesh.AddElementsByType(1, 2, new IntPtr[0], trisTags);
            //dimTags = controller.Gmsh.Model.GetEntities();
            controller.Gmsh.Model.Mesh.ReclassifyNodes();
            controller.Gmsh.Model.Mesh.CreateGeometry();
            controller.Gmsh.Model.Mesh.Generate(2);
            var list = controller.GetMeshObjects();
            var before = list.Item3.Count;
            Assert.AreNotEqual(0, before);
            dimTags = controller.Gmsh.Model.GetEntities(0);
            controller.Gmsh.Model.Mesh.SetSize(dimTags, 0.4);
            controller.Gmsh.Model.Mesh.Generate(2);
            list = controller.GetMeshObjects();
            var after = list.Item3.Count;
            Assert.IsTrue(after > before);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\screw.step")]
        [DataRow(@"\CAD\assembly_1.step")]
        [DataRow(@"\CAD\assembly_2.stp")]
        [DataRow(@"\CAD\nasos.step")]
        [DataRow(@"\CAD\block.igs")]
        [DataRow(@"\GMSHAPI\cubes.geo")]//Для geo ядра презентация STL не работает (как и в gmsh)
        public void StlRepresentationTest(string fileName)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.ImportStl();

            var ext = Path.GetExtension(fileName);
            var volumes = controller.GetVolumeFigures();
            if (ext.Contains("step") || ext.Contains("stp"))
            {
                Assert.AreNotEqual(0, volumes.Count);
            }
            else if (ext.Contains("igs"))
            {
                var surfaces = controller.GetSurfaceFigures();
                Assert.AreNotEqual(0, surfaces.Count);
            }
            else
                Assert.AreEqual(0, volumes.Count);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\weld_seam_1.step", 38, new int[] { 18, 19, 23, 24 }, new int[] { 2 }, new double[] { 0.25, 0.05, 0.5 })]
        [DataRow(@"\CAD\weld_seam_2.step", 38, new int[] { 18, 19, 23, 24 }, new int[] { 2 }, new double[] { 0.25, 0.05, 0.5 })]
        [DataRow(@"\CAD\Tavr.step", 6, new int[] { 25, 27, 29, 31, 32, 33, 34, 36, 38, 40, 41, 42 }, new int[] { 3, 4 }, new double[] { 20, 10, 3.5, 3.5 })]
        public void VolumeSizesPointSizesCase(string fileName, int seamPointCurves, int[] seamCurves, int[] seamVolumes, double[] volumeSizes)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            var volumes = controller.Gmsh.Model.GetEntities(3);
            //Вариант 1 - использование плотностей в точках, сначало генерируем плотности в точках, затем проводим трансфиницию (порядок важен!)
            for(int i = 0, j = 0; i < volumes.Length; i += 2, ++j)
            {
                var dimTags = new int[] { volumes[i], volumes[i + 1] };
                var points = controller.Gmsh.Model.GetBoundary(dimTags, true, true, true);
                controller.Gmsh.Model.Mesh.SetSize(points, volumeSizes[j]);
            }
            MakeTranfinite(seamPointCurves, seamCurves, seamVolumes);
            controller.Gmsh.Model.Mesh.Generate(3);
            var error = controller.Gmsh.Logger.GetLastError();
            Assert.IsTrue(string.IsNullOrEmpty(error));
            //var file = Path.GetFileNameWithoutExtension(fileName);
            //controller.Gmsh.Write($@"D:\{file}.inp");//Для визуального теста
        }

        [DataTestMethod]
        [DataRow(@"\CAD\weld_seam_1.step", 38, new int[] { 18, 19, 23, 24 }, new int[] { 2 }, new double[] { 0.25, 0.05, 0.5 })]
        [DataRow(@"\CAD\weld_seam_2.step", 38, new int[] { 18, 19, 23, 24 }, new int[] { 2 }, new double[] { 0.25, 0.05, 0.5 })]
        [DataRow(@"\CAD\Tavr.step", 6, new int[] { 25, 27, 29, 31, 32, 33, 34, 36, 38, 40, 41, 42 }, new int[] { 3, 4 }, new double[] { 20, 10, 3.5, 3.5 })]
        public void VolumeSizesMeshFieldsCase(string fileName, int seamPointCurves, int[] seamCurves, int[] seamVolumes, double[] volumeSizes)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            var volumes = controller.Gmsh.Model.GetEntities(3);
            //Вариант 2 - использование Mesh Fields, сначало генерируем Mesh Fields, затем проводим трансфиницию (порядок важен!)
            for (int i = 0, j = 0; i < volumes.Length; i += 2, ++j)
            {
                var dimTags = new int[] { volumes[i], volumes[i + 1] };
                var field = controller.Gmsh.Model.Mesh.Field.Add(FieldType.Constant);
                controller.Gmsh.Model.Mesh.Field.SetNumbers(field, "VolumesList", new double[] { field });
                controller.Gmsh.Model.Mesh.Field.SetNumber(field, "VIn", volumeSizes[j]);
            }
            var list = controller.Gmsh.Model.Mesh.Field.List().Select(v => (double)v).ToArray();
            var baseField = controller.Gmsh.Model.Mesh.Field.Add(FieldType.Min);
            controller.Gmsh.Model.Mesh.Field.SetNumbers(baseField, "FieldsList", list);
            controller.Gmsh.Model.Mesh.Field.SetAsBackgroundMesh(baseField);
            MakeTranfinite(seamPointCurves, seamCurves, seamVolumes);
            controller.Gmsh.Model.Mesh.Generate(3);
            var error = controller.Gmsh.Logger.GetLastError();
            Assert.IsTrue(string.IsNullOrEmpty(error));
            //file = Path.GetFileNameWithoutExtension(fileName);
            //controller.Gmsh.Write($@"D:\{file}_field.inp");//Для визуального теста
        }

        [DataTestMethod]
        [DataRow(@"\STL\sphere2.stl", @"\STL\cube2.stl")]
        public void MergeTwoStlTest(string first, string second)
        {
            var meshFiles = new string[] { modelFolder + first, modelFolder + second };
            controller.MergeTwoMeshes(meshFiles);
            controller.Gmsh.Model.Mesh.Generate(2);
            var error = controller.Gmsh.Logger.GetLastError();
            Assert.IsTrue(string.IsNullOrEmpty(error));
            controller.Gmsh.Write(@"\..\..\..\..\Mesh\mergeResult.stl");
        }

        private void MakeTranfinite(int seamPointCurves, int[] seamCurves, int[] seamVolumes)
        {
            //Трансфиниция кривых
            for (var i = 0; i < seamCurves.Length; ++i)
                controller.Gmsh.Model.Mesh.SetTransfiniteCurve(seamCurves[i], seamPointCurves);
            //Трансфиниция поверхностей
            for (var i = 0; i < seamVolumes.Length; ++i)
            {
                var seamData = controller.Gmsh.Model.GetAdjacencies(3, seamVolumes[i]);
                var seamSurfs = seamData.Item2;
                for (var j = 0; j < seamSurfs.Length; ++j)
                    controller.Gmsh.Model.Mesh.SetTransfiniteAutomatic(new int[] { 2, seamSurfs[j] }, 2.35, false);
            }
        }
    }
}