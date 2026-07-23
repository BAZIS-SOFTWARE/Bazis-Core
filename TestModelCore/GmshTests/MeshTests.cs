using Geometry;
using GmshApi;
using IronPython.Runtime;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Spatial.Euclidean;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsFinders;
using Model.IO;
using Model.MeshObjects;
using Model.Utilities;
using OperationalController;
using OperationalController.GmshController;
using OperationalController.MeshObjsCreator;
using OperationalController.MeshObjsCreator.Extrude;
using OperationalController.MeshObjsUtility;
using Project.Tasks;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Cryptography;
using static Community.CsharpSqlite.Sqlite3;
using Point3D = Geometry.Point3D;

namespace GmshTests
{
    [TestClass]
    public class MeshTests
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
        [DataRow(@"\CAD\assembly_2.stp", 14)]
        [DataRow(@"\CAD\assembly_1.step", 29)]
        [DataRow(@"\CAD\weld_seam_1.step", 3)]
        public void GetMeshSets(string fileName, int exp3DSets)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.Generate(3);
            var nds = controller.GetNodes();
            var e3Ds = controller.GetMeshSets<Element3D>(nds);
            var e2Ds = controller.GetMeshSets<Element2D>(nds);
            var quads = controller.GetMeshSets<Quad>(nds);
            Assert.AreEqual(exp3DSets, e3Ds.Count());
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
            modelData.ObjectData.E2DCollection.RemoveNotExistedObjects();

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
            controller.Gmsh.Model.Mesh.RemoveConstraints(new int[] { 1, curveTag });
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
            var centrElement = ByNumberFinder.Find(list.Item4, (int)centerElemTags[0]);
            var boundElement = ByNumberFinder.Find(list.Item4, (int)boundElemTags[0]);
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
            for (var i = 0; i < 4; ++i)
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
        [DataRow(@"\CAD\model_with_free_surfaces.stp")]
        [DataRow(@"\CAD\block.igs")]

        //[DataRow(@"\GMSHAPI\cubes.geo")]
        //Для geo ядра презентация STL не работает (как и в gmsh)
        public void StlRepresentationTest(string fileName)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.ImportStl();

            var ext = Path.GetExtension(fileName);
            var surfSets = controller.CreateSurfaceData();
            if (ext.Contains("step") || ext.Contains("stp"))
            {
                Assert.AreNotEqual(0, surfSets.Item1.Count);
            }
            else if (ext.Contains("igs"))
            {
                Assert.AreNotEqual(0, surfSets.Item1.Count);
            }
            else
                Assert.AreEqual(0, surfSets.Item1.Count);
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
            for (int i = 0, j = 0; i < volumes.Length; i += 2, ++j)
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
        //[DataRow("Prism", "Automatic")]//Для призм автоматическая трансфиниция не работает
        [DataRow("Prism", "Manual")]
        [DataRow("Hexa", "Automatic")]
        [DataRow("Hexa", "Manual")]
        public void GenHexaPentaElementsForPrismAndBoxTest(string volumeType, string genMethod)
        {
            var volume = volumeType == "Prism" ? controller.Gmsh.Model.Occ.AddWedge(0, 0, 0, 1, 1, 1)
                                               : controller.Gmsh.Model.Occ.AddBox(0, 0, 0, 1, 1, 1);
            controller.Gmsh.Model.Occ.Synchronize();

            var surfaces = controller.Gmsh.Model.GetEntities(2);
            if (genMethod == "Manual")
                for (var i = 0; i < surfaces.Length; i += 2)
                {
                    controller.Gmsh.Model.Mesh.SetTransfiniteSurface(surfaces[i + 1]);
                    controller.Gmsh.Model.Mesh.SetRecombine(surfaces[i], surfaces[i + 1]);
                }
            else
                controller.Gmsh.Model.Mesh.SetTransfiniteAutomatic(surfaces);

            controller.Gmsh.Model.Mesh.SetTransfiniteVolume(volume);

            controller.Gmsh.Model.Mesh.Generate(3);

            var error = controller.Gmsh.Logger.GetLastError();
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var elements = controller.GetMeshObjects();
            foreach (var element in elements.Item4)
                Assert.IsTrue(element.ElementKind == ElementKind.Пента || element.ElementKind == ElementKind.Гекса);

            //controller.Gmsh.Write($@"D:\{volumeType}{genMethod}.inp");//Для визуального теста
        }

        [DataTestMethod]
        [DataRow("geo")]
        [DataRow("occ")]
        public void GetStringSettingsTest(string expected)
        {
            controller.Gmsh.Option.SetString("General.ScriptingLanguages", expected);

            var actual = controller.Gmsh.Option.GetString("General.ScriptingLanguages");
            Assert.IsTrue(expected == actual);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Tavr.step", 0.58, 3.5, MeshAlgorithm3D.Delaunay, 1)]
        [DataRow(@"\CAD\cilindr.step", 0.33, 1.8, MeshAlgorithm3D.Frontal, 1.2)]
        [DataRow(@"\CAD\screw.step", 0.7, 1.7, MeshAlgorithm3D.HXT, 0.85)]
        [DataRow(@"\CAD\nasos.step", 10, 16, MeshAlgorithm3D.RTree, 1.8)]
        public void ChechMesh3DParameters(string fileName, double expMinSize, double expMaxSize, MeshAlgorithm3D expAlgo3d, double sizeFactor)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Option.SetNumber("Mesh.MeshSizeMin", expMinSize);
            controller.Gmsh.Option.SetNumber("Mesh.MeshSizeMax", expMaxSize);
            controller.Gmsh.Option.SetNumber("Mesh.Algorithm3D", (int)expAlgo3d);
            controller.Gmsh.Option.SetNumber("Mesh.MeshSizeFactor", sizeFactor);

            controller.Gmsh.Model.Mesh.Generate(3);

            var error = controller.Gmsh.Logger.GetLastError();
            var actMinSize = controller.Gmsh.Option.GetNumber("Mesh.MeshSizeMin");
            var actMaxSize = controller.Gmsh.Option.GetNumber("Mesh.MeshSizeMax");
            var actAlgo3d = controller.Gmsh.Option.GetNumber("Mesh.Algorithm3D");
            var actSizeFactor = controller.Gmsh.Option.GetNumber("Mesh.MeshSizeFactor");

            Assert.IsTrue(string.IsNullOrEmpty(error));
            Assert.IsTrue(expMinSize == actMinSize);
            Assert.IsTrue(expMaxSize == actMaxSize);
            Assert.IsTrue((int)expAlgo3d == actAlgo3d);
            Assert.IsTrue(sizeFactor == actSizeFactor);
            //var file = Path.GetFileNameWithoutExtension(modelFolder + fileName);
            //controller.Gmsh.Write($@"C:\Mesh\INP\{file + "MinSize"}.inp");//Для визуального теста
        }



        [DataTestMethod]
        [DataRow(@"\CAD\Tavr.step", MeshAlgorithm2D.FrontalDelaunay)]
        [DataRow(@"\CAD\cilindr.step", MeshAlgorithm2D.Delaunay)]
        [DataRow(@"\CAD\screw.step", MeshAlgorithm2D.BAMG)]
        [DataRow(@"\CAD\nasos.step", MeshAlgorithm2D.Delaunay)]
        public void MeshAlgorithm2DTest(string fileName, MeshAlgorithm2D expAlgo2d)
        {
            controller.Gmsh.Open(modelFolder + fileName);

            controller.Gmsh.Option.SetNumber("Mesh.Algorithm", (int)expAlgo2d);

            controller.Gmsh.Model.Mesh.Generate(2);

            var error = controller.Gmsh.Logger.GetLastError();
            var actAlgo2d = controller.Gmsh.Option.GetNumber("Mesh.Algorithm");

            Assert.IsTrue(string.IsNullOrEmpty(error));
            Assert.IsTrue((int)expAlgo2d == actAlgo2d);

            //var file = Path.GetFileNameWithoutExtension(modelFolder + fileName);
            //controller.Gmsh.Write($@"C:\Mesh\INP\{file + "MeshAlgorithm" + expAlgo2d.ToString()}.inp");//Для визуального теста
        }

        [DataTestMethod]
        public void EmbedTest()
        {
            controller.Gmsh.Model.Occ.AddRectangle(0, 0, 0, 2, 2, -1, 0);
            controller.Gmsh.Model.Occ.AddRectangle(0.5, 0.5, 0, 1, 1, -1, 0);
            controller.Gmsh.Model.Occ.Synchronize();

            var expected = new int[] { 5, 6, 7, 8 };
            controller.Gmsh.Model.Mesh.Embed(1, expected, 2, 1);
            var actual = controller.Gmsh.Model.Mesh.GetEmbedded(2, 1).Where((v, i) => (i & 1) == 1).ToArray();
            CollectionAssert.AreEquivalent(expected, actual);

            //Если удалили, то должен показать пустой массив
            controller.Gmsh.Model.Mesh.RemoveEmbedded([2, 1]);
            var arr = controller.Gmsh.Model.Mesh.GetEmbedded(2, 1);
            Assert.AreEqual(0, arr.Length);

            controller.Gmsh.Model.Mesh.Embed(1, expected, 2, 1);
            controller.Gmsh.Model.Mesh.Generate(2);
            //controller.Gmsh.Write(@"D:\mixed.msh");//Для визуализации
        }

        [DataTestMethod]
        [DataRow(@"\CAD\cilindr.step")]
        public void Hide3DTest(string path)
        {
            var proj = new ProjectController();
            proj.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            proj.ImportCAD(modelFolder + @"\" + path);

            proj.GenerateMesh(3);

            //Скрытие\показ 3д элементов сопряженных с объемом и проверка элементов 
            for (var i = 0; i < 2; ++i)
            {
                var expState = Convert.ToBoolean(i);
                proj.GetModelVolumes().First().ViewState = expState;
                proj.HideMesh(3);

                foreach (var elems in proj.GetModelObjects(ObjType.Элемент3D))
                    Assert.AreEqual(expState, elems.ViewState);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\cilindr.step", 3, 3, 1)]
        [DataRow(@"\CAD\cilindr.step", 2, 2, 1)]
        [DataRow(@"\CAD\cilindr.step", 0, 2, 1)]
        [DataRow(@"\CAD\cilindr.step", 0, 3, 1)]
        public void CreateGroupByGeoId(string path,int meshDim,int geoDim,int tag)
        {
            var proj = new ProjectController();
            proj.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            proj.ImportCAD(modelFolder + @"\" + path);

            proj.GenerateMesh(3);

            var gr = proj.CreateGroupByGeoObjs(meshDim, geoDim, tag);

            //Скрытие\показ 3д элементов сопряженных с объемом и проверка элементов 
            Assert.AreNotEqual(gr.Count(), 0);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\PART_3.stp")]
        public void CreateOpenSurfaceNodesGroup(string path)
        {
            var proj = new ProjectController();
            proj.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            proj.ImportCAD(modelFolder + @"\" + path);
            proj.GmshController.Gmsh.Option.SetNumber("Mesh.MeshSizeMax", 10);

            proj.GenerateMesh(3);
            //e3D_1
            var grSurfNodes = proj.CreateOpenSurfaceNodesGroup("e3D_2");

            //Скрытие\показ 3д элементов сопряженных с объемом и проверка элементов 
            Assert.AreNotEqual(proj.GetModelGroup(grSurfNodes).Count(), 0);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\cilindr.step")]
        public void Hide2DTest(string path)
        {
            var proj = new ProjectController();
            proj.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            proj.ImportCAD(modelFolder + @"\" + path);

            proj.GenerateMesh(3);

            //Скрытие\показ 2д элементов сопряженных с поверхностью и проверка элементов
            var elemTags = proj.GmshController.Gmsh.Model.Mesh.GetElements(2, 1).Item2
                                                              .SelectMany(v => v)
                                                              .Select(v => (int)v)
                                                              .ToHashSet();
            for (var i = 0; i < 2; ++i)
            {
                var expState = Convert.ToBoolean(i);
                proj.GetModelSurface(1).ViewState = expState;
                proj.HideMesh(2);

                foreach (var elems in proj.GetModelObjects(ObjType.Элемент2D))
                {
                    if (elemTags.Contains(elems.Number))
                        Assert.AreEqual(expState, elems.ViewState);
                    else
                        Assert.AreEqual(true, elems.ViewState);//Элементы не сопряженные с поверхность 1 должны быть показаны
                }
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\cilindr.step")]
        public void Hide1DTest(string path)
        {
            var proj = new ProjectController();
            proj.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            proj.ImportCAD(modelFolder + @"\" + path);

            proj.GenerateMesh(3);

            //Скрытие\показ 1д элементов сопряженных с кривой и проверка элементов
            var elemTags = proj.GmshController.Gmsh.Model.Mesh.GetElements(1, 1).Item2
                                                              .SelectMany(v => v)
                                                              .Select(v => (int)v)
                                                              .ToHashSet();
            for (var i = 0; i < 2; ++i)
            {
                var expState = Convert.ToBoolean(i);
                proj.GetModelCurve(1).ViewState = expState;
                proj.HideMesh(1);

                foreach (var elems in proj.GetModelObjects(ObjType.Элемент1D))
                {
                    if (elemTags.Contains(elems.Number))
                        Assert.AreEqual(expState, elems.ViewState);
                    else
                        Assert.AreEqual(true, elems.ViewState);//Элементы не сопряженные с кривой 1 должны быть показаны
                }
            }
        }
        [DataTestMethod]
        [DataRow(4, 0.25f, false, false)]
        [DataRow(2, 0.15f, true, false)]
        [DataRow(7, 0.55f, false, true)]
        [DataRow(2, 0.38f, true, true)]
        public void ExtrudeTranslateTest(int layersCount, float size, bool recombine, bool transfinite)
        {
            var proj = new ProjectController();
            proj.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");

            var ctr = proj.GmshController;

            CreateGearSurface(ctr, 5, 6, 8);

            ctr.Gmsh.Model.Mesh.Generate(2);
            if (recombine)
                ctr.Gmsh.Model.Mesh.Recombine();
            var objs = ctr.GetMeshObjects();

            var dir = new Point3D(0f, 0f, 1f);
            var extruder = new TranslateExtruder();

            var e3d = transfinite ? extruder.ExtrudeTransfinite(objs.Item3, dir, layersCount, size)
                                  : extruder.Extrude(objs.Item3, dir, layersCount, size);

            var md = new ModelData();
            md.ObjectData.E3DCollection.AddRange("Extruded", e3d.SelectMany(v => v));

            foreach (var node in extruder.Nodes.Values)
                md.ObjectData.NodesSet.Add(node.Number, node);

            Assert.AreEqual(0, extruder.MissedElements);

            var saver = new SaveModelToBPF2TextFile();
            saver.Save(md, $@"D:\gearRecombine{recombine.ToString()}Transfinite{transfinite.ToString()}.bpf2");
        }

        [DataTestMethod]
        [DataRow(8, "jug", MathF.PI / 2, false, false)]
        [DataRow(11, "jug", MathF.PI / 8, true, false)]
        [DataRow(32, "jug", 2 * MathF.PI, false, true)]
        [DataRow(13, "jug", 5 * MathF.PI / 3, true, true)]
        public void ExtrudeRotationTest(int segments, string fileName, float angle, bool recombine, bool transfinite)
        {
            var proj = new ProjectController();
            proj.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");

            var ctr = proj.GmshController;
            ctr.Gmsh.Open(modelFolder + $@"\CAD\{fileName}.step");

            ctr.Gmsh.Model.Mesh.Generate(2);
            if (recombine)
                ctr.Gmsh.Model.Mesh.Recombine();
            var objs = ctr.GetMeshObjects();

            var extruder = new RotateExtruder();
            var e3d = transfinite ? extruder.ExtrudeTransfinite(objs.Item3, angle, Vector3.Zero, Vector3.UnitY, segments)
                                  : extruder.Extrude(objs.Item3, angle, Vector3.Zero, Vector3.UnitY, segments);

            var md = new ModelData();
            md.ObjectData.E3DCollection.AddRange("Extruded", e3d.SelectMany(v => v));

            foreach (var node in extruder.Nodes.Values)
                md.ObjectData.NodesSet.Add(node.Number, node);

            Assert.AreEqual(0, extruder.MissedElements);

            var saver = new SaveModelToBPF2TextFile();
            saver.Save(md, $@"D:\{fileName}Recombine{recombine.ToString()}Transfinite{transfinite.ToString()}.bpf2");
        }

        //Тест на вращение вокруг произвольной точки
        [DataTestMethod]
        [DataRow(8, MathF.PI, false, false, new float[] { 0f, -75f, 5f, 5f, -85f, 0f, 0f, -75f, 2.0212862E-13f })]
        public void TriangleRotationTest(int segments, float angle, bool recombine, bool transfinite, float[] coords)
        {
            var proj = new ProjectController();
            var ctr = proj.GmshController;

            var triangle = new List<Triangle_c>() { 
                                                        new Triangle_c(1, new Node[] { new Node(1, new Point3D(coords[0], coords[1], coords[2])),
                                                                                       new Node(2, new Point3D(coords[3], coords[4], coords[5])),
                                                                                       new Node(3, new Point3D(coords[6], coords[7], coords[8])) }
                                                                                     )
                                                  };

            var extruder = new RotateExtruder();
            var e3d = transfinite ? extruder.ExtrudeTransfinite(triangle, angle, new Vector3(0, -75, 0), Vector3.UnitZ, segments)
                                  : extruder.Extrude(triangle, angle, new Vector3(0, -75, 0), Vector3.UnitZ, segments);

            var md = new ModelData();
            md.ObjectData.E3DCollection.AddRange("Extruded", e3d.SelectMany(v => v));

            foreach (var node in extruder.Nodes.Values)
                md.ObjectData.NodesSet.Add(node.Number, node);

            Assert.AreEqual(0, extruder.MissedElements);

            var saver = new SaveModelToBPF2TextFile();
            saver.Save(md, $@"D:\RotatedTriangle.bpf2");
        }

        [DataTestMethod]
        [DataRow(@"\CAD\multiCurves.step", new int[] { 1, 2, 3 }, 1, new double[] { 0.1, 0.2, 0.25 }, new float[] { -1, 0, 0, -1.1f, -0.1f, 0, -1, 0, 0.1f }, true)]
        [DataRow(@"\CAD\multiCurves2.step", new int[] { 2, 4, 3, 1 }, 2, new double[] { 0.05, 0.13, 0.3, 0.35 }, new float[] { 0, -1, 0, 0f, -1.25f, 0, 0, -1, 0.25f }, true)]
        [DataRow(@"\CAD\closeWire.step", new int[] { 1, 2, 3, 4, 5, 6 }, 1, new double[] { 0.1, 0.3, 0.22, 0.05, 0.2, 0.4 }, new float[] { -1, 0, 0, -1f, -0.1f, 0f, -1, 0, 0.1f }, true)]
        public void ExtrudeMultiCurvesTest(string fileName, int[] curves, int point, double[] steps, float[] coords, bool transfinite)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.MergeGeometry();

            var elements = GenTriangle(coords);

            var md = new ModelData();

            var extruder = new CurveExtruder(controller, curves, steps, point);
            var e3d = transfinite ? extruder.ExtrudeTransfinite(elements)
                                  : extruder.Extrude(elements);

            Assert.IsTrue(extruder.MissedElements == 0);

            foreach (var node in extruder.Nodes.Values)
                md.ObjectData.NodesSet.Add(node.Number, node);

            md.ObjectData.E3DCollection.AddRange($"Mulpiple Seams", e3d.SelectMany(v => v));

            var fName = Path.GetFileNameWithoutExtension(fileName);

            var saver = new SaveModelToBPF2TextFile();
            saver.Save(md, $@"D:\{fName}.bpf2");
        }

        [DataTestMethod]
        [DataRow(@"\CAD\PART_3.stp", new int[] { 47, 44, 41, 40, 35, 32, 29, 26 }, 33, new double[] { 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 }
                                   ,new float[] { 77, -197.08f, -2.0f, 77, -197.08f, -7.0f, 82, -197.08f, -2.0f }, true,
                                    new int[] { 1, 7, 8, 11, 14, 19, 20 }, 2, new double[] { 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1 }
                                   ,new float[] { 77, -197.08f, 8.0f, 77, -197.08f, 13.0f, 82, -197.08f, 8.0f }, true)]
        public void ExtrudeMultiCurvesTest2(string fileName, int[] curves, int point, double[] steps, float[] coords, bool transfinite,
                                            int[] curves2, int point2, double[] steps2, float[] coords2, bool transfinite2)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.MergeGeometry();

            var elements = GenSurfaceTriangle(controller, coords);
            var elements2 = GenSurfaceTriangle(controller, coords2);

            var md = new ModelData();

            var extruder = new CurveExtruder(controller, curves, steps, point);
            var e3d = transfinite ? extruder.ExtrudeTransfinite(elements)
                                  : extruder.Extrude(elements);

            Assert.IsTrue(extruder.MissedElements == 0);

            foreach (var node in extruder.Nodes.Values)
                md.ObjectData.NodesSet.Add(node.Number, node);

            md.ObjectData.E3DCollection.AddRange($"Mulpiple Seams", e3d.SelectMany(v => v));
            
            var md2 = new ModelData();

            var extruder2 = new CurveExtruder(controller, curves2, steps2, point2);
            var e3d2 = transfinite ? extruder2.ExtrudeTransfinite(elements2)
                                  : extruder2.Extrude(elements2);

            Assert.IsTrue(extruder2.MissedElements == 0);

            foreach (var node in extruder2.Nodes.Values)
                md2.ObjectData.NodesSet.Add(node.Number, node);

            md2.ObjectData.E3DCollection.AddRange($"Mulpiple Seams 2", e3d2.SelectMany(v => v));

            md.Append(md2);

            var fName = Path.GetFileNameWithoutExtension(fileName);

            var saver = new SaveModelToBPF2TextFile();
            saver.Save(md, $@"D:\{fName}.bpf2");
        }

        [DataTestMethod]
        [DataRow(@"\CAD\PART_3.stp", new int[] { 20, 19, 14, 11, 8, 7, 1, 50, 47, 44, 41, 40, 35, 32, 29, 26, 23 }, 15, 3.0,
                                                 new float[] { 77, -24.08f, 8.0f, 77, -24.08f, 13.0f, 82, -24.08f, 8.0f }, true)]
        [DataRow(@"\CAD\PART_3.stp", new int[] { 20, 14, 19, 8, 11, 7, 1, 50, 47, 44, 40, 41, 35, 23, 29, 26, 32 }, 15, 3.0,
                                                 new float[] { 77, -24.08f, 8.0f, 77, -24.08f, 13.0f, 82, -24.08f, 8.0f }, true)]//Тест на сортировку петли
        public void ExtrudeMultiCurvesTest3(string fileName, int[] curves, int point, double step, float[] coords, bool transfinite)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.MergeGeometry();

            var elements = GenSurfaceTriangle(controller, coords);
            var lenList = new List<double>();

            var md = new ModelData();
            var extruder = new CurveExtruder(controller, curves, step, point);
            var e3d = transfinite ? extruder.ExtrudeTransfinite(elements)
                                  : extruder.Extrude(elements);

            Assert.IsTrue(extruder.MissedElements == 0);

            foreach (var node in extruder.Nodes.Values)
                md.ObjectData.NodesSet.Add(node.Number, node);

            md.ObjectData.E3DCollection.AddRange($"Mulpiple Seams", e3d.SelectMany(v => v));

            var fName = Path.GetFileNameWithoutExtension(fileName);

            var saver = new SaveModelToBPF2TextFile();
            saver.Save(md, $@"D:\{fName}.bpf2");
        }


        /// <summary>
        /// Экструзия по кривой
        /// /// <summary>
        /// Путь до файла, [кривая_1, базовая точка кривой_1...], [параметры шага эструзии (мин,макс)], [координаты точек сечения треугольника в глобальном пространстве]
        /// </summary>
        /// </summary>
        /// <param name="fileName">Путь до файла</param>
        /// <param name="curveData">Массив данных кривой [номер кривой_1, точка кривой_1, номер кривой_2, точка кривой_2...]</param>
        /// <param name="extrusionData">Данные экструзии [минимальный шаг_1, минимальный шаг_2...]</param>
        /// <param name="transfinite">true - для трансифинитного построения 3д элементов</param>
        /// <param name="recombine">Используется при генерации окружности если true</param>
        /// <param name="coords">Координаты сечения треугольника, null - специальный случай используем окружность</param>
        [DataTestMethod]
        [DataRow(@"\CAD\cilindr.step", new int[] { 5, 2, 4, 2 }, new double[] { 0.15, 0.25 }, true, false, new float[] {10.63178f, 2.25962f, 0.0f,
                                                                                                                       10.42f, 3.22338f, -0.14127485f,
                                                                                                                       11.6700233f, 2.46244f, -9.28e-11f})]//Тест на двух кривых
        [DataRow(@"\CAD\cilindr.step", new int[] { 1, 2 }, new double[] { 0.25 }, false, false, new float[] {10.63178f, 2.25962f, 0.0f,
                                                                                                             11.67f, 3.22338f, 0.5f,
                                                                                                             11.67f, 3.22338f, -0.5f})]//Тест по прямой
        [DataRow(@"\CAD\curve.step", new int[] { 1, 2 }, new double[] { 0.1 }, true, true, null)]//Тест по кривой с разной кривизной
        [DataRow(@"\CAD\curve2.step", new int[] { 1, 1 }, new double[] { 0.025 }, true, false, null)]//Тест по кривой с разной кривизной
        public void ExtrudeAlongCurveTest(string fileName, int[] curveData, double[] extrusionData, bool transfinite, bool recombine, float[] coords)
        {
            controller.Gmsh.Open(modelFolder + fileName);

            var mdArray = new List<ModelData>();

            var crossSection = coords != null ? GenTriangle(coords) : GenCircle(controller, curveData[0], curveData[1], recombine);
            for (var i = 0; i < curveData.Length; i += 2)
            {
                var md = new ModelData();

                var extruder = new CurveExtruder(controller, curveData[i], extrusionData[i >> 1], curveData[i + 1]);
                var e3d = transfinite ? extruder.ExtrudeTransfinite(crossSection)
                                      : extruder.Extrude(crossSection);

                Assert.IsTrue(extruder.MissedElements == 0);

                foreach (var node in extruder.Nodes.Values)
                    md.ObjectData.NodesSet.Add(node.Number, node);

                md.ObjectData.E3DCollection.AddRange($"Seam {i >> 1}", e3d.SelectMany(v => v));

                mdArray.Add(md);
            }

            for (var i = 1; i < mdArray.Count; ++i)
                mdArray[0].Append(mdArray[i]);

            var fName = Path.GetFileNameWithoutExtension(fileName);

            var curves = curveData.Where((v, i) => (i & 1) == 1).Select(v => v.ToString()).ToArray();
            var stringKey = string.Join(" ", curves);
            var stringTransfinite = transfinite ? "Transfinite" : string.Empty;
            var stringRecombine = recombine ? "Recombine" : string.Empty;

            var saver = new SaveModelToBPF2TextFile();
            saver.Save(mdArray[0], $@"D:\{fName}Curves{stringKey}{stringTransfinite}{stringRecombine}.bpf2");
        }

        public static IEnumerable<object[]> GetVectorLists()
        {
            yield return new object[]
            {
                new List<List<Vector2>>() { new List<Vector2>() { new Vector2(-1.2f, -1.0f), new Vector2(-0.6f, -1.0f) , new Vector2(-0.6f, -0.6f), new Vector2( -0.8f, -0.6f) },
                                            new List<Vector2>() { new Vector2(-0.6f, -1.0f), new Vector2( 0.0f, -1.0f) , new Vector2( 0.0f, -0.6f), new Vector2(-0.3f, -0.6f),
                                                                  new Vector2(-0.6f, -0.6f) },
                                            new List<Vector2>() { new Vector2(0.0f, -1.0f), new Vector2(0.6f, -1.0f), new Vector2(0.6f, -0.6f), new Vector2( 0.3f, -0.6f),
                                                                  new Vector2(0.0f, -0.6f) },
                                            new List<Vector2>() { new Vector2(0.6f, -1.0f), new Vector2(1.2f, -1.0f), new Vector2(0.8f, -0.6f), new Vector2(0.6f, -0.6f) },

                                            new List<Vector2>() { new Vector2(-0.8f, -0.6f), new Vector2(-0.6f, -0.6f), new Vector2(-0.3f, -0.6f), new Vector2(-0.3f, 0.0f),
                                                                  new Vector2(-0.7f, 0.0f) },
                                            new List<Vector2>() { new Vector2(-0.3f, -0.6f), new Vector2(0.0f, -0.6f), new Vector2(0.3f, -0.6f), new Vector2(0.3f, 0.0f),
                                                                  new Vector2(0.0f, 0.0f), new Vector2(-0.3f, 0.0f) },
                                            new List<Vector2>() { new Vector2(0.3f, -0.6f), new Vector2(0.6f, -0.6f), new Vector2(0.8f, -0.6f), new Vector2(0.7f, 0.0f),
                                                                  new Vector2(0.3f, 0.0f)},

                                            new List<Vector2>() { new Vector2(-0.7f, 0.0f), new Vector2(-0.3f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.6f),
                                                                  new Vector2(-0.4f, 0.6f) },
                                            new List<Vector2>() { new Vector2(0.0f, 0.0f), new Vector2(0.3f, 0.0f), new Vector2(0.7f, 0.0f), new Vector2(0.4f, 0.6f),
                                                                  new Vector2(0.0f, 0.6f) },

                                            new List<Vector2>() { new Vector2(-0.4f, 0.6f), new Vector2(0.0f, 0.6f), new Vector2(0.4f, 0.6f), new Vector2( 0.0f, 1.2f) }
                                          },
                "Triangle_0",
                Vector3.Zero,
                Vector3.UnitX,
                Vector3.UnitY
            };

            yield return new object[]
            {
                new List<List<Vector2>>() { new List<Vector2>() { new Vector2(-0.8f, -0.6f), new Vector2( 0.8f, -0.6f), new Vector2( 0.0f, 0.0f) },
                                            new List<Vector2>() { new Vector2(-1.2f, -1.0f), new Vector2( -0.8f, -1.0f), new Vector2( -0.8f, -0.6f) },
                                            new List<Vector2>() { new Vector2(-0.8f, -1.0f), new Vector2( 0.8f, -1.0f), new Vector2( 0.8f, -0.6f), new Vector2( -0.8f, -0.6f) },
                                            new List<Vector2>() { new Vector2(0.8f, -1.0f), new Vector2(1.2f, -1.0f), new Vector2(0.8f, -0.6f) },
                                          },
                "Triangle_1",
                Vector3.Zero,
                Vector3.UnitX,
                Vector3.UnitY
            };
            //Тест поворотов осей и смещения
            yield return new object[]
           {
                new List<List<Vector2>>() { new List<Vector2>() { new Vector2(-0.8f, -0.6f), new Vector2( 0.8f, -0.6f), new Vector2( 0.0f, 0.0f) },
                                            new List<Vector2>() { new Vector2(-1.2f, -1.0f), new Vector2( -0.8f, -1.0f), new Vector2( -0.8f, -0.6f) },
                                            new List<Vector2>() { new Vector2(-0.8f, -1.0f), new Vector2( 0.8f, -1.0f), new Vector2( 0.8f, -0.6f), new Vector2( -0.8f, -0.6f) },
                                            new List<Vector2>() { new Vector2(0.8f, -1.0f), new Vector2(1.2f, -1.0f), new Vector2(0.8f, -0.6f) },
                                          },
                "Triangle_2",
                Vector3.UnitX,
                new Vector3(0.707f, 0.707f, 0),
                new Vector3(-0.707f, 0.707f, 0)
           };
        }

        [TestMethod]
        [DynamicData(nameof(GetVectorLists), DynamicDataSourceType.Method)]
        public void MultiPassSectionTest(List<List<Vector2>> list, string modelName, Vector3 origin, Vector3 axisX, Vector3 axisY)
        {
            var section = new MultiPassSection(controller);

            section.Origin = origin;
            section.AxisX = axisX;
            section.AxisY = axisY;

            var elements = section.Create(list);

            var modelData = new ModelData();

            foreach(var node in section.Nodes)
                modelData.ObjectData.NodesSet.Add(node.Number, node);

            foreach (var element in elements)
                modelData.ObjectData.E2DCollection.Add(element);
            var saver = new SaveModelToBPF2TextFile();
            saver.Save(modelData, $@"D:\{modelName}.bpf2");

        }

        private List<Element2D> GenTriangle(float[] coords)
        {
            var nodes = new List<Node>() { new Node(1, new Point3D(coords[0], coords[1], coords[2])),
                                           new Node(2, new Point3D(coords[3], coords[4], coords[5])),
                                           new Node(3, new Point3D(coords[6], coords[7], coords[8]))};

            var crossSection = new List<Element2D> { new Triangle_c(1, nodes.ToArray()) };
            return crossSection;
        }

        private List<Element2D> GenSurfaceTriangle(IGmshController controller, float[] coords, double meshSize = 10)
        {
            controller.Gmsh.Model.Add("Section");

            var pList = new List<int>();
            var lList = new List<int>();
            for (var i = 0; i < 3; ++i)
            {
                var point = controller.Gmsh.Model.Occ.AddPoint(coords[i * 3], coords[i * 3 + 1], coords[i * 3 + 2]);
                if (point == 0)
                    throw new Exception(controller.Gmsh.Logger.GetLastError());
                pList.Add(point);
            }

            for(var i = 0; i < 3; ++i)
            {
                var line = controller.Gmsh.Model.Occ.AddLine(pList[i], i == 2 ? pList[0] : pList[i + 1]);
                if (line == 0)
                    throw new Exception(controller.Gmsh.Logger.GetLastError());
                lList.Add(line);
            }

            var wire = controller.Gmsh.Model.Occ.AddWire([lList[0], lList[1], lList[2]]);
            var surface = controller.Gmsh.Model.Occ.AddPlaneSurface([wire]);

            controller.Gmsh.Model.Occ.Synchronize();

            controller.Gmsh.Option.SetNumber("Mesh.MeshSizeMax", meshSize);
            controller.Gmsh.Model.Mesh.Generate(2);
            var data = controller.GetMeshObjects();

            controller.Gmsh.Model.Remove();

            return data.Item3;
        }

        private List<Element2D> GenCircle(IGmshController controller, int curveTag, int curvePoint, bool recombine, double radius = 0.05)
        {
            var bounds = controller.Gmsh.Model.GetParametrizationBounds(1, curveTag);
            var points = controller.Gmsh.Model.GetAdjacencies(1, curveTag).Item2;
            var mul = 1;
            if(curvePoint != points[0])
            {
                var temp = bounds.Item1[0];
                bounds.Item1[0] = bounds.Item2[0];
                bounds.Item2[0] = temp;
                mul = -1;
            }
            var center = controller.Gmsh.Model.GetValue(1, curveTag, [bounds.Item1[0]]);
            var tangent = controller.Gmsh.Model.GetDerivative(1, curveTag, [bounds.Item1[0]])
                                               .Select(v => v * mul)
                                               .ToArray();

            var circle = controller.Gmsh.Model.Occ.AddCircle(center[0], center[1], center[2], radius, -1,  0, 2 * Math.PI, tangent);
            var wire = controller.Gmsh.Model.Occ.AddWire([circle]);
            var surface = controller.Gmsh.Model.Occ.AddPlaneSurface([wire]);

            controller.Gmsh.Model.Occ.Synchronize();
            controller.Gmsh.Model.Mesh.Generate(2);
            if (recombine)
                controller.Gmsh.Model.Mesh.Recombine();

            var data = controller.GetMeshObjects();
            return data.Item3;
        }

        /*
        [DataTestMethod]
        [DataRow(90, false, false)]
        [DataRow(77, true, false)]
        [DataRow(51, false, true)]
        [DataRow(86, true, true)]
        public void ExtrudeSpiralTest(int layers, bool recombine, bool transfinite)
        {
            var proj = new ProjectController();
            proj.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");

            var ctr = proj.GmshController;
            var curve = CreateSpiral(ctr, 1, 0.5, 5);

            var circleCurve = ctr.Gmsh.Model.Occ.AddCircle(0, 0, 0, 0.15);
            var circleWire = ctr.Gmsh.Model.Occ.AddWire([circleCurve]);

            ctr.Gmsh.Model.Occ.AddPlaneSurface([circleWire]);
            ctr.Gmsh.Model.Occ.Synchronize();

            ctr.Gmsh.Model.Mesh.Generate(2);
            if (recombine)
                ctr.Gmsh.Model.Mesh.Recombine();

            var objs = ctr.GetMeshObjects();

            var extruder = new CurveExtruder(ctr, curve);
            var e3d = transfinite ? extruder.ExtrudeTransfinite(objs.Item3, layers)
                                  : extruder.Extrude(objs.Item3, layers);

            var md = new ModelData();
            md.ObjectData.E3DCollection.AddRange("Extruded", e3d.SelectMany(v => v));

            foreach (var node in extruder.Nodes.Values)
                md.ObjectData.NodesSet.Add(node.Number, node);

            Assert.AreEqual(0, extruder.MissedElements);

            var saver = new SaveModelToBPF2TextFile();
            saver.Save(md, $@"D:\spiralExtrudeRecombine{recombine.ToString()}Transfinite{transfinite.ToString()}.bpf2");
        }

        private int CreateSpiral(IGmshController controller, double radius, double height, int turns)
        {
            if(turns < 1)
                throw new ArgumentException("Число витков меньше 1");

            var first = new double[] { radius, 0, 0 };
            var cosCache = new double[] { 0, -radius, 0 };
            var sinCache = new double[] { -radius, 0, radius };

            var pointData = new List<int>();
            var p0 = controller.Gmsh.Model.Occ.AddPoint(first[0], first[1], first[2]);
            pointData.Add(p0);

            for (var i = 0; i < turns; ++i)
            {
                var p1 = controller.Gmsh.Model.Occ.AddPoint(cosCache[0], first[1] + 0.25 * height, sinCache[0]);
                var p2 = controller.Gmsh.Model.Occ.AddPoint(cosCache[1], first[1] + 0.5 * height, sinCache[1]);
                var p3 = controller.Gmsh.Model.Occ.AddPoint(cosCache[2], first[1] + 0.75 * height, sinCache[2]);
                var p4 = controller.Gmsh.Model.Occ.AddPoint(first[0], first[1] + height, first[2]);

                pointData.AddRange([p1, p2, p3, p4]);
                first[1] += height;
            }

            var curve = controller.Gmsh.Model.Occ.AddSpline(pointData.ToArray());
            controller.Gmsh.Model.Occ.Synchronize();
            return curve;
        }*/

        private void CreateGearSurface(IGmshController controller, double radius, double teethRadius, int teeths, double teethAngle = 15)
        {
            if (teeths < 3)
                throw new ArgumentException("Число зубьев шестерни меньше 3");
            if (teethRadius <= radius)
                throw new ArgumentException("Радиус зубьев не должен быть меньше или равен радиусу основания");

            var contour = controller.Gmsh.Model.Occ.AddCircle(0, 0, 0, radius);
            var circleHole = controller.Gmsh.Model.Occ.AddCircle(0, 0, 0, radius * 0.7);

            var teethRadAngle = Math.PI / 180 * teethAngle;
            var offset = 2 * Math.PI / teeths;
            var teethLength = (offset - 2 * teethRadAngle) / 2;

            var curAngle = 0.0;

            var rectList = new List<int>();
            for (var i = 0; i < teeths; ++i)
            {
                var aPoint = new Vector3D(radius * Math.Cos(curAngle), radius * Math.Sin(curAngle), 0);
                var bPoint = new Vector3D(teethRadius * Math.Cos(curAngle + teethRadAngle), teethRadius * Math.Sin(curAngle + teethRadAngle), 0);
                var cPoint = new Vector3D(teethRadius * Math.Cos(curAngle + teethRadAngle + teethLength),
                                          teethRadius * Math.Sin(curAngle + teethRadAngle + teethLength), 0);
                var dPoint = new Vector3D(radius * Math.Cos(curAngle + 2 * teethRadAngle + teethLength),
                                          radius * Math.Sin(curAngle + 2 * teethRadAngle + teethLength), 0);

                var a = controller.Gmsh.Model.Occ.AddPoint(aPoint.X, aPoint.Y, aPoint.Z);
                var b = controller.Gmsh.Model.Occ.AddPoint(bPoint.X, bPoint.Y, bPoint.Z);
                var c = controller.Gmsh.Model.Occ.AddPoint(cPoint.X, cPoint.Y, cPoint.Z);
                var d = controller.Gmsh.Model.Occ.AddPoint(dPoint.X, dPoint.Y, dPoint.Z);

                var aLine = controller.Gmsh.Model.Occ.AddLine(a, b);
                var bLine = controller.Gmsh.Model.Occ.AddLine(b, c);
                var cLine = controller.Gmsh.Model.Occ.AddLine(c, d);
                var dLine = controller.Gmsh.Model.Occ.AddLine(d, a);

                var wire = controller.Gmsh.Model.Occ.AddWire([aLine, bLine, cLine, dLine]);
                var surf = controller.Gmsh.Model.Occ.AddPlaneSurface([wire]);

                rectList.Add(2);
                rectList.Add(surf);
                curAngle += offset;
            }

            var circleHoleWire = controller.Gmsh.Model.Occ.AddWire([circleHole]);
            var circelWire = controller.Gmsh.Model.Occ.AddWire([contour]);
            var surface = controller.Gmsh.Model.Occ.AddPlaneSurface([circelWire, circleHoleWire]);

            controller.Gmsh.Model.Occ.Fuse([2, surface], rectList.ToArray());
            controller.Gmsh.Model.Occ.Synchronize();
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