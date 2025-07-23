using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using System.Collections.Generic;
using Geometry;
using Model.Interfaces;
using Model;
using Model.MeshObjects;
using OperationalController.GmshController;

namespace TestModel.GmshTests
{
    [TestClass]
    public class GeometryTests
    {
        private GmshController controller = new GmshController();
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
        [DataRow(@"\CAD\Cilindr.step", 6, ObjType.Кривая)]
        [DataRow(@"\CAD\Cilindr.step", 4, ObjType.Точка)]
        public void GetGeometryObjectsTest(string fileName, int numbOfObjs, ObjType objType)
        {
            controller.Gmsh.Open(modelFolder + fileName);

            IEnumerable<IModelObject> objects;
            if (objType == ObjType.Кривая)
                objects = controller.CreateLines();
            else
                objects = controller.CreateControlPoints();

            Assert.AreEqual(numbOfObjs, objects.Count());
            foreach (var obj in objects)
                Assert.AreEqual(objType, obj.ObjType);
        }

        [DataTestMethod]
        [DataRow(@"\GMSHAPI\test.geo")]
        public void TestCurveSteps(string fileName)
        {
            controller.Gmsh.Open(modelFolder + fileName);

            var curveUntil = controller.CreateLines().First();
            controller.Gmsh.Option.SetNumber("Geometry.NumSubEdges", 15);
            var curveAfter = controller.CreateLines().First();
            Assert.IsTrue(curveAfter.NumberOfPoints < curveUntil.NumberOfPoints);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\STL\ACII.stl", ObjType.Узел,0,0)]
        [DataRow(@"..\..\..\Examples\STL\ACII.stl", ObjType.Элемент2D,4,0)]
        public void RemoveAllData(string stlPath, ObjType objType, int nNumb, int eNumb)
        {
            var modelData = new ModelData();
            controller.Gmsh.Merge(stlPath);

            var nodes = controller.GetNodes();
            Assert.AreEqual(4, nodes.Count);

            var elements = controller.GetMeshEntities(2, -1, nodes);
            Assert.AreEqual(2, elements.Count);

            nodes.ForEach(x =>  modelData.ObjectData.NodesSet.Add(x.Number, x));
            modelData.ObjectData.E2DCollection.Add("e2d");
            elements.ForEach(x => modelData.ObjectData.E2DCollection.Add("e2d", (Element2D)x));
            modelData.ObjectData.Clear(objType);

            Assert.AreEqual(modelData.ObjectData.NodesSet.Count, nNumb);
            Assert.AreEqual(modelData.ObjectData.E2DCollection.Count, eNumb);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 1 })]
        public void RemoveEntityTest(string fileName, int[] dimArray)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            for (var i = 0; i < dimArray.Length; ++i)
            {
                var dimTags = controller.Gmsh.Model.GetEntities(-1);
                var lengthBefore = dimTags.Length;
                controller.RemoveGeometryObjects(new int[] { dimArray[i], dimTags[1] });
                dimTags = controller.Gmsh.Model.GetEntities();
                var lengthAfter = dimTags.Length;
                Assert.IsTrue(lengthBefore > lengthAfter);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", "1", new int[] { 4, 6, 4, 1 })]
        [DataRow(@"\CAD\nasos.step", "2", new int[] { 376, 684, 291, 1 })]
        [DataRow(@"\CAD\block.igs", "3", new int[] { 136, 138, 28 })]
        public void GetEntitiesGetAllDimensionTest(string fileName, string modelID, int[] expTags)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            var dim = controller.Gmsh.Model.GetDimension();
            Assert.AreNotEqual(0, dim);
            var tagDims = new int[expTags.Length][];
            for (var i = 0; i < tagDims.GetLength(0); ++i)
                tagDims[i] = Enumerable.Range(1, expTags[i]).ToArray();
            var expectedTags = tagDims.SelectMany(v => v).ToArray();
            var dimTags = controller.Gmsh.Model.GetEntities();
            var actualTags = dimTags.Where((v, i) => (i & 1) == 1).ToArray();
            CollectionAssert.AreEqual(expectedTags, actualTags);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", "1", new int[] { 4, 6, 4, 1 },
                  new string[] { "Point", "Curve", "Surface", "Volume" })]
        [DataRow(@"\CAD\Korpus_Nasosa.step", "2", new int[] { 376, 684, 291, 1 },
                  new string[] { "Point", "Curve", "Surface", "Volume" })]
        [DataRow(@"\CAD\model.igs", "3", new int[] { 136, 138, 28 }, new string[] { "Point", "Curve", "Surface" })]
        public void SetGetRemoveEntityNameTest(string fileName, string modelID, int[] entCount, string[] entName)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            var dimTags = controller.Gmsh.Model.GetEntities();
            for (var i = 1; i < dimTags.Length; i += 2)
            {
                var dim = dimTags[i - 1];
                var tag = dimTags[i];
                var expectedName = entName[dim] + tag.ToString();
                controller.Gmsh.Model.SetEntityName(dim, tag, expectedName);
                var actual = controller.Gmsh.Model.GetEntityName(dim, tag);
                Assert.AreEqual(expectedName, actual);
            }
            //Remove Test: 1) Remove exists name 2) Remove not exists name
            RemoveNameTest(ref entName[0], 0, entCount[0] - 2, entCount[0]);
            RemoveNameTest(ref entName[0], 0, entCount[0] + 2, entCount[0]);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", "1", new double[] { -14.2188, -2.7094, -13.2231 },
                                      new double[] { 10.6366, 19.599, 10.7593 })]
        [DataRow(@"\CAD\nasos.step", "2", new double[] { -88.2608, -0.679586, -61.2934 },
                                            new double[] { 88.2608, 273.279, 61.2934 })]
        [DataRow(@"\CAD\block.igs", "3", new double[] { -1.72724e-05, -179.902, -0.0020751 },
                                   new double[] { 2591.2, 2540, 508 })]
        [DataRow(@"\GMSHAPI\sphere.geo", "4", new double[] { -1, -1, -1 },
                                    new double[] { 1, 1, 1 })]
        public void ModelBoundingBoxTest(string fileName, string modelID, double[] minExp, double[] maxExp)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            var data = controller.Gmsh.Model.GetBoundingBox(-1, -1);
            var min = new double[3] { data.Item1, data.Item2, data.Item3 };
            var max = new double[3] { data.Item4, data.Item5, data.Item6 };
            for (var i = 0; i < 3; ++i)
                Assert.AreEqual(minExp[i], min[i], 1e-02);
            for (var i = 0; i < 3; ++i)
                Assert.AreEqual(maxExp[i], max[i], 1e-02);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", "1", 1, new int[] { }, new int[] { 1, 2, 6 }, 0)]
        [DataRow(@"\CAD\Cilindr.step", "1", 2, new int[] { }, new int[] { 1, 4, 5 }, 0)]
        [DataRow(@"\CAD\Cilindr.step", "1", 3, new int[] { }, new int[] { 2, 3, 6 }, 0)]
        [DataRow(@"\CAD\Cilindr.step", "1", 4, new int[] { }, new int[] { 3, 4, 5 }, 0)]
        [DataRow(@"\GMSHAPI\sphere.geo", "2", 1, new int[] { }, new int[] { }, 0)]
        [DataRow(@"\GMSHAPI\sphere.geo", "2", 2, new int[] { }, new int[] { 1, 4, 9, 12 }, 0)]
        [DataRow(@"\GMSHAPI\sphere.geo", "2", 3, new int[] { }, new int[] { 1, 2, 5, 8 }, 0)]
        [DataRow(@"\GMSHAPI\sphere.geo", "2", 4, new int[] { }, new int[] { 2, 3, 10, 11 }, 0)]
        [DataRow(@"\GMSHAPI\sphere.geo", "2", 5, new int[] { }, new int[] { 3, 4, 6, 7 }, 0)]
        [DataRow(@"\GMSHAPI\sphere.geo", "2", 6, new int[] { }, new int[] { 5, 6, 11, 12 }, 0)]
        [DataRow(@"\GMSHAPI\sphere.geo", "2", 7, new int[] { }, new int[] { 7, 8, 9, 10 }, 0)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 1, new int[] { 2, 3 }, new int[] { 18, 22 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 2, new int[] { 3, 4 }, new int[] { 14, 20 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 3, new int[] { 4, 5 }, new int[] { 16, 24 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 4, new int[] { 5, 2 }, new int[] { 26, 28 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 5, new int[] { 3, 6 }, new int[] { 20, 22 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 6, new int[] { 6, 5 }, new int[] { 24, 28 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 7, new int[] { 5, 7 }, new int[] { 16, 26 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 8, new int[] { 7, 3 }, new int[] { 14, 18 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 9, new int[] { 2, 7 }, new int[] { 18, 26 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 10, new int[] { 7, 4 }, new int[] { 14, 16 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 11, new int[] { 4, 6 }, new int[] { 20, 24 }, 1)]
        [DataRow(@"\GMSHAPI\sphere.geo", "1", 12, new int[] { 6, 2 }, new int[] { 22, 28 }, 1)]
        [DataRow(@"\CAD\Cilindr.step", "1", 1, new int[] { 1, 2, 3, 4 }, new int[] { 1 }, 2)]
        [DataRow(@"\CAD\Cilindr.step", "1", 2, new int[] { 1, 5, 3, 6 }, new int[] { 1 }, 2)]
        [DataRow(@"\CAD\Cilindr.step", "1", 3, new int[] { 4, 5 }, new int[] { 1 }, 2)]
        [DataRow(@"\CAD\Cilindr.step", "1", 4, new int[] { 6, 2 }, new int[] { 1 }, 2)]
        [DataRow(@"\CAD\Cilindr.step", "1", 1, new int[] { 1, 2, 3, 4 }, new int[] { }, 3)]
        [DataRow(@"\GMSHAPI\sphere.geo", "2", 30, new int[] { 28, 26, 16, 14, 20, 24, 22, 18 }, new int[] { }, 3)]
        [DataRow(@"\GMSHAPI\sphere.geo", "2", 1, new int[0], new int[0], 3)]//Not Found Test
        public void GetAdjacenciesTest(string fileName, string modelId, int tag, int[] downExp, int[] upExp, int dim)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            var data = controller.Gmsh.Model.GetAdjacencies(dim, tag);
            CollectionAssert.AreEqual(downExp, data.Item2);
            CollectionAssert.AreEqual(upExp, data.Item1);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", "1", 1, new double[] { }, new double[] { 7.04956865241, 19.1445358209, -2.46386184958 })]
        [DataRow(@"\CAD\Cilindr.step", "1", 2, new double[] { }, new double[] { 10.6317844476, 2.25496193279, 0 })]
        [DataRow(@"\CAD\Cilindr.step", "1", 3, new double[] { }, new double[] { -14.2140002427, 14.6346119553, -2.46386184958 })]
        [DataRow(@"\CAD\Cilindr.step", "1", 4, new double[] { }, new double[] { -10.6317844476, -2.25496193279, 2.33468147911e-16 })]
        [DataRow(@"\CAD\Cilindr.step", "1", 5, new double[] { }, new double[] { })]//Not found test

        public void GetLocalCoordsByParametricTest(string fileName, string modelID, int tag,
                                                   double[] parametric, double[] expected)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            var coord = controller.Gmsh.Model.GetValue(0, tag, parametric);
            for (var i = 0; i < expected.Length; ++i)
                Assert.AreEqual(expected[i], coord[i], 1e-08);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 1, 2, 3, 4, 5, 6 })]
        [DataRow(@"\GMSHAPI\sphere.geo", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 })]
        public void ParametrizationCurvesTest(string fileName, int[] tags)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            for (var i = 0; i < tags.Length; ++i)
            {
                var data = controller.Gmsh.Model.GetParametrizationBounds(1, tags[i]);
                var numInsideCount = controller.Gmsh.Model.IsInside(1, tags[i], data.Item1, true);
                Assert.AreNotEqual(0, numInsideCount);
                numInsideCount = controller.Gmsh.Model.IsInside(1, tags[i], data.Item2, true);
                Assert.AreNotEqual(0, numInsideCount);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 1 })]
        public void ParametrizationSurfacesTest(string fileName, int[] tags)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            for (var i = 0; i < tags.Length; ++i)
            {
                var data = controller.Gmsh.Model.GetParametrizationBounds(2, tags[i]);
                var numInsideCount = controller.Gmsh.Model.IsInside(1, tags[i], data.Item1, true);
                Assert.AreNotEqual(0, numInsideCount);
                numInsideCount = controller.Gmsh.Model.IsInside(2, tags[i], data.Item2, true);
                Assert.AreNotEqual(0, numInsideCount);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        public void AttributeTest(string fileName, string modelName)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            controller.Gmsh.Model.Mesh.SetTransfiniteCurve(4, 20);
            var expected = new string[] { "4", "5" };
            controller.Gmsh.Model.SetAttribute("Progression", expected);
            var attributes = controller.Gmsh.Model.GetAttribute("Progression");
            CollectionAssert.AreEqual(expected, attributes);
            var transValues = new string[] { "1", "2" };
            controller.Gmsh.Model.SetAttribute("Transfinite", transValues);
            var names = controller.Gmsh.Model.GetAttributeNames();
            CollectionAssert.AreEqual(new string[] { "Progression", "Transfinite" }, names);
            controller.Gmsh.Model.RemoveAttribute("Progression");
            names = controller.Gmsh.Model.GetAttributeNames();
            CollectionAssert.AreEqual(new string[] { "Transfinite" }, names);
        }
        ///Тест, гарантирующий загрузку 3д элементов для STL
        [DataTestMethod]
        [DataRow("sphere.stl")]
        public void AddElements3DForStlTest(string fileName)
        {
            controller.Gmsh.Open(@"..\..\..\Examples\STL\" + fileName);
            controller.Gmsh.Model.Mesh.Generate(3);
            var nodes = controller.GetNodes(3, -1);
            var elements = controller.GetMeshEntities(3, -1, nodes);
            Assert.AreEqual(0, elements.Count);
            var dimTags = controller.Gmsh.Model.GetEntities(2);
            dimTags = dimTags.Where((v,i) => (i & 1) == 1).ToArray();
            var shell = controller.Gmsh.Model.Geo.AddSurfaceLoop(dimTags);
            var shellTags = new int[] { shell };
            var volume = controller.Gmsh.Model.Geo.AddVolume(shellTags);
            controller.Gmsh.Model.Geo.Synchronize();//Обязательный вызов Synchronize, после добавления объемов
            controller.Gmsh.Model.Mesh.Generate(3);
            var error = controller.Gmsh.Logger.GetLastError();
            Assert.IsTrue(string.IsNullOrEmpty(error));
            nodes = controller.GetNodes(3, -1);
            elements = controller.GetMeshEntities(3, -1, nodes);
            Assert.AreNotEqual(0, elements.Count);
        }
        //Тест проверки 4 координаты контрольной точки в файле
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\GMSHAPI\simple.geo", new double[] { 0.1, 0.1, 0.1, 0.01 })]
        public void CheckMeshSizesInFileTest(string path, double[] expected)
        {
            controller.Gmsh.Open(path);
            var dimTags = controller.Gmsh.Model.GetEntities(0);
            var coords = controller.Gmsh.Model.Mesh.GetSizes(dimTags);
            CollectionAssert.AreEqual(expected, coords);
        }
        //Тест - устанавливаем плотность в точке, затем проверям(берем разные файлы моделей!)
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\CAD\Cilindr.step", new double[] { 0.35, 0.8, 0.25, 1.0 })]
        [DataRow(@"..\..\..\Examples\GMSHAPI\simple.geo", new double[] { 0.5, 0.9, 0.01, 0.7 })]
        public void SetAndGetMeshSizesTest(string path, double[] expected)
        {
            controller.Gmsh.Open(path);
            var dimTags = controller.Gmsh.Model.GetEntities(0);
            for (int i = 1, j = 0; i < dimTags.Length; i += 2, ++j)
            {
                var dim = dimTags[i - 1];
                var tag = dimTags[i];
                controller.Gmsh.Model.Mesh.SetSize(new int[] { dim, tag }, expected[j]);
            }
            var coords = controller.Gmsh.Model.Mesh.GetSizes(dimTags);
            CollectionAssert.AreEqual(expected, coords);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\GMSHAPI\simple.geo", new string[] { "18", "bump","1.8"})]
        public void SetAndGetCurveTransfinite(string path, string[] expected)
        {
            controller.Gmsh.Open(path);
            controller.Gmsh.Model.Mesh.SetTransfiniteCurve(1, 15, GmshApi.MeshType.Bump, 1.8);
            controller.Gmsh.Model.SetAttribute("transfinite", expected);
            var attributes = controller.Gmsh.Model.GetAttribute("transfinite");
            CollectionAssert.AreEqual(expected, attributes);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 2, 4, 5, 6 })]
        public void GetCurveLengthTest(string fileName, int[] curvesId)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            for (var i = 0; i < curvesId.Length; ++i)
            {
                int[] down, up;
                var coords = new double[2][];
                var data = controller.Gmsh.Model.GetAdjacencies(1, curvesId[i]);
                Assert.AreEqual(2, data.Item2.Length);
                if (data.Item2.Length == 2)
                {
                    for (var j = 0; j < data.Item2.Length; ++j)
                        coords[j] = controller.Gmsh.Model.GetValue(0, data.Item2[j], new double[0]);
                    var point0 = new Point3D((float)coords[0][0], (float)coords[0][1], (float)coords[0][2]);
                    var point1 = new Point3D((float)coords[1][0], (float)coords[1][1], (float)coords[1][2]);
                    var radius = Vector.GetVectorLenght(point0.Sub(point1)) / 2;
                    var expectedLength = Math.PI * radius;
                    var actualLength = controller.CalcCurveLength(curvesId[i]);
                    Assert.AreEqual(expectedLength, actualLength, 1e-4);
                }
            }
        }
        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", 2, 3f)]
        [DataRow(@"\CAD\Cilindr.step", 3, 0.5f)]
        [DataRow(@"\CAD\Cilindr.step", 4, 4.2f)]
        public void GetPointsCountByElementSizeTest(string fileName, int curveId, float elemSize)
        {
            controller.Gmsh.Open(modelFolder + fileName);
            var actual = controller.GetPointsCountByElementSize(curveId, elemSize);

            controller.Gmsh.Model.Mesh.SetTransfiniteCurve(curveId, actual);
            controller.Gmsh.Model.Mesh.Generate(2);
            var data = controller.Gmsh.Model.Mesh.GetNodes(1, curveId, true, false);
            var expected = data.Item2.Length / 3;
            Assert.AreEqual(expected, actual);
        }

        private void RemoveNameTest(ref string entName, int dim, int delId, int maxId)
        {
            var status = maxId >= delId;
            var delName = entName + delId.ToString();
            controller.Gmsh.Model.RemoveEntityName(delName);
        }
    }
}
