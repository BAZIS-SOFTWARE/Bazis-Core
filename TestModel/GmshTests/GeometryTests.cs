using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using GmshApi.GmshController;
using System;
using System.Collections.Generic;
using Geometry;
using Model.Interfaces;
using Model;
using Model.MeshObjects;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerGeometryTests
    {
        private GmshController controller = new GmshController();
        private static string modelFolder = @"..\..\..\Examples";
        [TestInitialize]
        public void Init()
        {
            controller.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            /*var argv = string.Empty;
            var ierr = 0;
            controller.gmshInitialize(0, ref argv, 0, 0, ref ierr);*/
        }
        [TestCleanup]
        public void Cleanup()
        {
            var ierr = 0;
            //controller.gmshFinalize(ref ierr);
            controller.Clear(ref ierr);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", 6, ObjType.Кривая)]
        [DataRow(@"\CAD\Cilindr.step", 4, ObjType.Точка)]
        public void GetGeometryObjectsTest(string fileName, int numbOfObjs, ObjType objType)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);

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
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);

            var curveUntil = controller.CreateLines().First();
            controller.OptionSetNumber("Geometry.NumSubEdges", 15, ref ierr);
            var curveAfter = controller.CreateLines().First();
            Assert.IsTrue(curveAfter.NumberOfPoints < curveUntil.NumberOfPoints);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\STL\ACII.stl", ObjType.Узел,0,0)]
        [DataRow(@"..\..\..\Examples\STL\ACII.stl", ObjType.Элемент2D,4,0)]
        public void RemoveAllData(string stlPath, ObjType objType, int nNumb, int eNumb)
        {
            var ierr = 0;
            //var status = true;
            var modelData = new ModelData();

            controller.Merge(stlPath, ref ierr);
            var nodes = controller.GetNodes();
            //Assert.AreEqual(true, status);
            Assert.AreEqual(4, nodes.Count);
            var elements = controller.GetMeshEntities(2, -1, nodes);
            //Assert.AreEqual(true, status);
            Assert.AreEqual(2, elements.Count);
            nodes.ForEach(x =>  modelData.ObjectData.NodesSet.Add(x.Number, x));
            modelData.ObjectData.E2DCollection.Add("e2d");
            elements.ForEach(x => modelData.ObjectData.E2DCollection.Add("e2d", (Element2D)x));
            modelData.ObjectData.Clear(objType);
            //assert
            Assert.AreEqual(modelData.ObjectData.NodesSet.Count, nNumb);
            Assert.AreEqual(modelData.ObjectData.E2DCollection.Count, eNumb);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 1 })]
        public void RemoveEntityTest(string fileName, int[] dimArray)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            for (var i = 0; i < dimArray.Length; ++i)
            {
                int[] dimTags;
                controller.ModelGetGeometryEntities(out dimTags, -1);
                var lengthBefore = dimTags.Length;
                controller.RemoveGeometryObjects(new int[] { dimArray[i], dimTags[1] });
                controller.ModelGetGeometryEntities(out dimTags, -1);
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
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            var dim = controller.GetGeometryObjectDimension(ref ierr);
            Assert.AreNotEqual(0, dim);
            var tagDims = new int[expTags.Length][];
            for (var i = 0; i < tagDims.GetLength(0); ++i)
                tagDims[i] = Enumerable.Range(1, expTags[i]).ToArray();
            var expectedTags = tagDims.SelectMany(v => v).ToArray();
            int[] dimTags;
            controller.ModelGetGeometryEntities(out dimTags, -1);
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
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            int[] dimTags;
            controller.ModelGetGeometryEntities(out dimTags,-1);
            for (var i = 1; i < dimTags.Length; i += 2)
            {
                var dim = dimTags[i - 1];
                var tag = dimTags[i];
                var expectedName = entName[dim] + tag.ToString();
                controller.ModelSetEntityName(dim, tag, expectedName, ref ierr);
                string actual;
                controller.ModelGetGeometryEntityName(dim, tag, out actual);
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
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            double xMin = 0.0, yMin = 0.0, zMin = 0.0;
            double xMax = 0.0, yMax = 0.0, zMax = 0.0;
            controller.ModelGetBoundingBox(-1, -1, ref xMin, ref yMin, ref zMin, ref xMax, 
                                                ref yMax, ref zMax, ref ierr);
            var min = new double[3] { xMin, yMin, zMin };
            var max = new double[3] { xMax, yMax, zMax };
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
        [DataRow(@"\GMSHAPI\sphere.geo", "2", 1, null, null, 3)]//Not Found Test
        public void GetAdjacenciesTest(string fileName, string modelId, int tag, int[] downExp, int[] upExp, int dim)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            int[] upward, downward;
            controller.ModelGetAdjacencies(dim, tag, out upward, out downward);
            CollectionAssert.AreEqual(downExp, downward);
            CollectionAssert.AreEqual(upExp, upward);
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
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            double[] coord;
            controller.ModelGetValue(0, tag, parametric, out coord);
            for (var i = 0; i < expected.Length; ++i)
                Assert.AreEqual(expected[i], coord[i], 1e-08);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 1, 2, 3, 4, 5, 6 })]
        [DataRow(@"\GMSHAPI\sphere.geo", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 })]
        public void ParametrizationCurvesTest(string fileName, int[] tags)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            double[] min, max;
            for (var i = 0; i < tags.Length; ++i)
            {
                controller.ModelGetParametrizationBounds(1, tags[i], out min, out max);
                var numInsideCount = controller.ModelIsInside(1, tags[i], min, (IntPtr) min.Length, 1, ref ierr);
                Assert.AreNotEqual(0, numInsideCount);
                numInsideCount = controller.ModelIsInside(1, tags[i], max, (IntPtr)max.Length, 1, ref ierr);
                Assert.AreNotEqual(0, numInsideCount);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 1 })]
        public void ParametrizationSurfacesTest(string fileName, int[] tags)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            double[] min, max;
            for (var i = 0; i < tags.Length; ++i)
            {
                controller.ModelGetParametrizationBounds(2, tags[i], out min, out max);
                var numInsideCount = controller.ModelIsInside(1, tags[i], min, (IntPtr)min.Length, 1, ref ierr);
                Assert.AreNotEqual(0, numInsideCount);
                numInsideCount = controller.ModelIsInside(2, tags[i], max, (IntPtr)max.Length, 1, ref ierr);
                Assert.AreNotEqual(0, numInsideCount);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        public void AttributeTest(string fileName, string modelName)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            controller.ModelMeshSetTransfiniteCurve(4, 20, "Progression", 1.0, ref ierr);
            var expected = new string[] { "4", "5" };
            controller.ModelSetAttribute("Progression", expected, (IntPtr)expected.Length, ref ierr);
            string[] attributes;
            controller.ModelGetAttribute("Progression", out attributes);
            CollectionAssert.AreEqual(expected, attributes);
            var transValues = new string[] { "1", "2" };
            controller.ModelSetAttribute("Transfinite", transValues, (IntPtr)2, ref ierr);
            string[] names;
            controller.ModelGetAttributeNames(out names);
            CollectionAssert.AreEqual(new string[] { "Progression", "Transfinite" }, names);
            controller.ModelRemoveAttribute("Progression", ref ierr);
            controller.ModelGetAttributeNames(out names);
            CollectionAssert.AreEqual(new string[] { "Transfinite" }, names);
        }
        ///Тест, гарантирующий загрузку 3д элементов для STL
        [DataTestMethod]
        [DataRow("sphere.stl")]
        public void AddElements3DForStlTest(string fileName)
        {
            var ierr = 0;
            controller.Open(@"..\..\..\Examples\STL\" + fileName, ref ierr);
            controller.ModelMeshGenerate(3, ref ierr);
            var nodes = controller.GetNodes(3, -1);
            var elements = controller.GetMeshEntities(3, -1, nodes);
            Assert.AreEqual(0, elements.Count);
            int[] dimTags;
            controller.ModelGetGeometryEntities(out dimTags, 2);
            dimTags = dimTags.Where((v,i) => (i & 1) == 1).ToArray();
            var shell = controller.ModelGeoAddSurfaceLoop(dimTags, (IntPtr)dimTags.Length, -1, ref ierr);
            var shellTags = new int[] { shell };
            var volume = controller.ModelGeoAddVolume(shellTags, (IntPtr)shellTags.Length, -1, ref ierr);
            controller.ModelGeoSynchronize(ref ierr);//Обязательный вызов Synchronize, после добавления объемов
            controller.ModelMeshGenerate(3, ref ierr);
            Assert.AreNotEqual(1, ierr);
            nodes = controller.GetNodes(3, -1);
            elements = controller.GetMeshEntities(3, -1, nodes);
            Assert.AreNotEqual(0, elements.Count);
        }
        //Тест проверки 4 координаты контрольной точки в файле
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\GMSHAPI\simple.geo", new double[] { 0.1, 0.1, 0.1, 0.01 })]
        public void CheckMeshSizesInFileTest(string path, double[] expected)
        {
            var ierr = 0;
            int[] dimTags;
            controller.Open(path, ref ierr);
            controller.ModelGetGeometryEntities(out dimTags, 0);
            var coords = controller.GetMeshDensity(dimTags);
            CollectionAssert.AreEqual(expected, coords);
        }
        //Тест - устанавливаем плотность в точке, затем проверям(берем разные файлы моделей!)
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\CAD\Cilindr.step", new double[] { 0.35, 0.8, 0.25, 1.0 })]
        [DataRow(@"..\..\..\Examples\GMSHAPI\simple.geo", new double[] { 0.5, 0.9, 0.01, 0.7 })]
        public void SetAndGetMeshSizesTest(string path, double[] expected)
        {
            var ierr = 0;
            int[] dimTags;
            controller.Open(path, ref ierr);
            controller.ModelGetGeometryEntities(out dimTags, 0);
            for (int i = 1, j = 0; i < dimTags.Length; i += 2, ++j)
            {
                var dim = dimTags[i - 1];
                var tag = dimTags[i];
                controller.ModelMeshSetSize(new int[] { dim, tag }, (IntPtr)2 , expected[j], ref ierr);
            }
            var coords = controller.GetMeshDensity(dimTags);
            CollectionAssert.AreEqual(expected, coords);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\GMSHAPI\simple.geo", new string[] { "18", "bump","1.8"})]
        public void SetAndGetCurveTransfinite(string path, string[] expected)
        {
            var ierr = 0;
            controller.Open(path, ref ierr);
            controller.ModelMeshSetTransfiniteCurve(1, 15, "Bump", 1.8, ref ierr);
            controller.ModelSetAttribute("transfinite", expected, (IntPtr)3, ref ierr);
            string[] attributes;
            controller.ModelGetAttribute("transfinite", out attributes);
            CollectionAssert.AreEqual(expected, attributes);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 2, 4, 5, 6 })]
        public void GetCurveLengthTest(string fileName, int[] curvesId)
        {
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            for (var i = 0; i < curvesId.Length; ++i)
            {
                int[] down, up;
                var coords = new double[2][];
                controller.ModelGetAdjacencies(1, curvesId[i], out up, out down);
                Assert.AreEqual(2, down.Length);
                if (down.Length == 2)
                {
                    for (var j = 0; j < down.Length; ++j)
                        controller.ModelGetValue(0, down[j], new double[0], out coords[j]);
                    var point0 = new Point3D((float)coords[0][0], (float)coords[0][1], (float)coords[0][2]);
                    var point1 = new Point3D((float)coords[1][0], (float)coords[1][1], (float)coords[1][2]);
                    var radius = Vector.GetVectorLenght(point0.Sub(point1)) / 2;
                    var expectedLength = Math.PI * radius;
                    var actualLength = controller.GetCurveLength(curvesId[i]);
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
            var ierr = 0;
            controller.Open(modelFolder + fileName, ref ierr);
            var actual = controller.GetPointsCountByElementSize(curveId, elemSize);
            //int[] pointDimTags;
            //controller.ModelGetGeometryEntities(out pointDimTags, 0);
            //controller.ModelMeshSetSize(pointDimTags, (IntPtr)pointDimTags.Length, elemSize, ref ierr);
            controller.ModelMeshSetTransfiniteCurve(curveId, actual, "Progression", 1.0, ref ierr);
            controller.ModelMeshGenerate(2, ref ierr);
            long[] tags;
            double[] coords, parametric;
            controller.ModelMeshGetNodes(1, curveId, true, false, out tags, out coords, out parametric);
            var expected = coords.Length / 3;
            Assert.AreEqual(expected, actual);
        }

        private void RemoveNameTest(ref string entName, int dim, int delId, int maxId)
        {
            var ierr = 0;
            var status = maxId >= delId;
            var delName = entName + delId.ToString();
            controller.ModelRemoveEntityName(delName, ref ierr);
            /*if (status)
                Assert.AreEqual("", ControllerGeometry.GetEntityName(dim, delId));*/
        }
    }
}
