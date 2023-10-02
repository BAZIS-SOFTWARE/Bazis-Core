using Microsoft.VisualStudio.TestTools.UnitTesting;
using GmshApi;
using System.Linq;
using GmshApi.GmshController;
using Model.Interfaces;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace TestModel.GmshTests
{
    [TestClass]
    public class ControllerGeometryTests
    {
        private GmshController controller = new GmshController("C:\\Windows\\System32\\gmsh.dll");
        private static string modelFolder = @"..\..\..\Examples\GMSHConnector\";
        [TestInitialize]
        public void Init()
        {
            /*var argv = string.Empty;
            var ierr = 0;
            controller.gmshInitialize(0, ref argv, 0, 0, ref ierr);*/
        }
        [TestCleanup]
        public void Cleanup()
        {
            var ierr = 0;
            //controller.gmshFinalize(ref ierr);
            controller.gmshClear(ref ierr);
        }
        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetCurvesTest(string fileName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            var status = false;
            var curves = controller.GetGeometryEntities(ObjKind.Curve, ref status);
            Assert.IsTrue(status);
            Assert.AreEqual(6, curves.Count());
        }
        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetElements1DTest(string fileName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            var status = false;
            var curves = controller.GetGeometryEntities(ObjKind.Beam, ref status);
            Assert.IsTrue(status);
            Assert.AreEqual(6, curves.Count());
            for (var i = 0; i < curves.Count; ++i)
                Assert.AreEqual(ObjKind.Beam, curves[i].ObjKind);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step")]
        public void GetControlPointsTest(string fileName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            var status = false;
            var controlPoints = controller.GetGeometryEntities(ObjKind.Point, ref status);
            Assert.IsTrue(status);
            Assert.AreEqual(4, controlPoints.Count());
            for (var i = 0; i < controlPoints.Count; ++i)
                Assert.AreEqual(ObjKind.Node, controlPoints[i].ObjKind);
        }

        [DataTestMethod]
        [DataRow("test.geo")]
        public void TestCurveSteps(string fileName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            var status = false;
            var curveUntil = controller.GetGeometryEntities(ObjKind.Beam, ref status).First();
            controller.gmshOptionSetNumber("Geometry.NumSubEdges", 15, ref ierr);
            var curveAfter = controller.GetGeometryEntities(ObjKind.Beam, ref status).First();
            Assert.IsTrue(curveAfter.NumberOfPoints < curveUntil.NumberOfPoints);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 1 })]
        public void RemoveEntityTest(string fileName, int[] dimArray)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            for (var i = 0; i < dimArray.Length; ++i)
            {
                int[] dimTags;
                controller.ModelGetEntities(out dimTags, -1);
                var lengthBefore = dimTags.Length;
                controller.RemoveEntities(new int[] { dimArray[i], dimTags[1] });
                controller.ModelGetEntities(out dimTags, -1);
                var lengthAfter = dimTags.Length;
                Assert.IsTrue(lengthBefore > lengthAfter);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { 4, 6, 4, 1 })]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 376, 684, 291, 1 })]
        [DataRow("model.igs", "3", new int[] { 136, 138, 28 })]
        public void GetEntitiesGetAllDimensionTest(string fileName, string modelID, int[] expTags)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            var dim = controller.gmshModelGetDimension(ref ierr);
            Assert.AreNotEqual(0, dim);
            var tagDims = new int[expTags.Length][];
            for (var i = 0; i < tagDims.GetLength(0); ++i)
                tagDims[i] = Enumerable.Range(1, expTags[i]).ToArray();
            var expectedTags = tagDims.SelectMany(v => v).ToArray();
            int[] dimTags;
            controller.ModelGetEntities(out dimTags, -1);
            var actualTags = dimTags.Where((v, i) => (i & 1) == 1).ToArray();
            CollectionAssert.AreEqual(expectedTags, actualTags);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new int[] { 4, 6, 4, 1 },
                  new string[] { "Point", "Curve", "Surface", "Volume" })]
        [DataRow("Korpus_Nasosa.step", "2", new int[] { 376, 684, 291, 1 },
                  new string[] { "Point", "Curve", "Surface", "Volume" })]
        [DataRow("model.igs", "3", new int[] { 136, 138, 28 }, new string[] { "Point", "Curve", "Surface" })]
        public void SetGetRemoveEntityNameTest(string fileName, string modelID, int[] entCount, string[] entName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            int[] dimTags;
            controller.ModelGetEntities(out dimTags,-1);
            for (var i = 1; i < dimTags.Length; i += 2)
            {
                var dim = dimTags[i - 1];
                var tag = dimTags[i];
                var expectedName = entName[dim] + tag.ToString();
                controller.gmshModelSetEntityName(dim, tag, expectedName, ref ierr);
                string actual;
                controller.ModelGetEntityName(dim, tag, out actual);
                Assert.AreEqual(expectedName, actual);
            }
            //Remove Test: 1) Remove exists name 2) Remove not exists name
            RemoveNameTest(ref entName[0], 0, entCount[0] - 2, entCount[0]);
            RemoveNameTest(ref entName[0], 0, entCount[0] + 2, entCount[0]);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1", new double[] { -14.2188, -2.7094, -13.2231 },
                                      new double[] { 10.6366, 19.599, 10.7593 })]
        [DataRow("Korpus_Nasosa.step", "2", new double[] { -88.2608, -0.679586, -61.2934 },
                                            new double[] { 88.2608, 273.279, 61.2934 })]
        [DataRow("model.igs", "3", new double[] { -1.72724e-05, -179.902, -0.0020751 },
                                   new double[] { 2591.2, 2540, 508 })]
        [DataRow("sphere.geo", "4", new double[] { -1, -1, -1 },
                                    new double[] { 1, 1, 1 })]
        public void ModelBoundingBoxTest(string fileName, string modelID, double[] minExp, double[] maxExp)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            double xMin = 0.0, yMin = 0.0, zMin = 0.0;
            double xMax = 0.0, yMax = 0.0, zMax = 0.0;
            controller.gmshModelGetBoundingBox(-1, -1, ref xMin, ref yMin, ref zMin, ref xMax, 
                                                ref yMax, ref zMax, ref ierr);
            var min = new double[3] { xMin, yMin, zMin };
            var max = new double[3] { xMax, yMax, zMax };
            for (var i = 0; i < 3; ++i)
                Assert.AreEqual(minExp[i], min[i], 1e-02);
            for (var i = 0; i < 3; ++i)
                Assert.AreEqual(maxExp[i], max[i], 1e-02);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1", 1, new int[] { }, new int[] { 1, 2, 6 }, 0)]
        [DataRow("Cilindr.step", "1", 2, new int[] { }, new int[] { 1, 4, 5 }, 0)]
        [DataRow("Cilindr.step", "1", 3, new int[] { }, new int[] { 2, 3, 6 }, 0)]
        [DataRow("Cilindr.step", "1", 4, new int[] { }, new int[] { 3, 4, 5 }, 0)]
        [DataRow("sphere.geo", "2", 1, new int[] { }, new int[] { }, 0)]
        [DataRow("sphere.geo", "2", 2, new int[] { }, new int[] { 1, 4, 9, 12 }, 0)]
        [DataRow("sphere.geo", "2", 3, new int[] { }, new int[] { 1, 2, 5, 8 }, 0)]
        [DataRow("sphere.geo", "2", 4, new int[] { }, new int[] { 2, 3, 10, 11 }, 0)]
        [DataRow("sphere.geo", "2", 5, new int[] { }, new int[] { 3, 4, 6, 7 }, 0)]
        [DataRow("sphere.geo", "2", 6, new int[] { }, new int[] { 5, 6, 11, 12 }, 0)]
        [DataRow("sphere.geo", "2", 7, new int[] { }, new int[] { 7, 8, 9, 10 }, 0)]
        [DataRow("sphere.geo", "1", 1, new int[] { 2, 3 }, new int[] { 18, 22 }, 1)]
        [DataRow("sphere.geo", "1", 2, new int[] { 3, 4 }, new int[] { 14, 20 }, 1)]
        [DataRow("sphere.geo", "1", 3, new int[] { 4, 5 }, new int[] { 16, 24 }, 1)]
        [DataRow("sphere.geo", "1", 4, new int[] { 5, 2 }, new int[] { 26, 28 }, 1)]
        [DataRow("sphere.geo", "1", 5, new int[] { 3, 6 }, new int[] { 20, 22 }, 1)]
        [DataRow("sphere.geo", "1", 6, new int[] { 6, 5 }, new int[] { 24, 28 }, 1)]
        [DataRow("sphere.geo", "1", 7, new int[] { 5, 7 }, new int[] { 16, 26 }, 1)]
        [DataRow("sphere.geo", "1", 8, new int[] { 7, 3 }, new int[] { 14, 18 }, 1)]
        [DataRow("sphere.geo", "1", 9, new int[] { 2, 7 }, new int[] { 18, 26 }, 1)]
        [DataRow("sphere.geo", "1", 10, new int[] { 7, 4 }, new int[] { 14, 16 }, 1)]
        [DataRow("sphere.geo", "1", 11, new int[] { 4, 6 }, new int[] { 20, 24 }, 1)]
        [DataRow("sphere.geo", "1", 12, new int[] { 6, 2 }, new int[] { 22, 28 }, 1)]
        [DataRow("Cilindr.step", "1", 1, new int[] { 1, 2, 3, 4 }, new int[] { 1 }, 2)]
        [DataRow("Cilindr.step", "1", 2, new int[] { 1, 5, 3, 6 }, new int[] { 1 }, 2)]
        [DataRow("Cilindr.step", "1", 3, new int[] { 4, 5 }, new int[] { 1 }, 2)]
        [DataRow("Cilindr.step", "1", 4, new int[] { 6, 2 }, new int[] { 1 }, 2)]
        [DataRow("Cilindr.step", "1", 1, new int[] { 1, 2, 3, 4 }, new int[] { }, 3)]
        [DataRow("sphere.geo", "2", 30, new int[] { 28, 26, 16, 14, 20, 24, 22, 18 }, new int[] { }, 3)]
        [DataRow("sphere.geo", "2", 1, null, null, 3)]//Not Found Test
        public void GetAdjacenciesTest(string fileName, string modelId, int tag, int[] downExp, int[] upExp, int dim)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            int[] upward, downward;
            controller.ModelGetAdjacencies(dim, tag, out upward, out downward);
            CollectionAssert.AreEqual(downExp, downward);
            CollectionAssert.AreEqual(upExp, upward);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1", 1, new double[] { }, new double[] { 7.04956865241, 19.1445358209, -2.46386184958 })]
        [DataRow("Cilindr.step", "1", 2, new double[] { }, new double[] { 10.6317844476, 2.25496193279, 0 })]
        [DataRow("Cilindr.step", "1", 3, new double[] { }, new double[] { -14.2140002427, 14.6346119553, -2.46386184958 })]
        [DataRow("Cilindr.step", "1", 4, new double[] { }, new double[] { -10.6317844476, -2.25496193279, 2.33468147911e-16 })]
        [DataRow("Cilindr.step", "1", 5, new double[] { }, new double[] { })]//Not found test

        public void GetLocalCoordsByParametricTest(string fileName, string modelID, int tag,
                                                   double[] parametric, double[] expected)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            double[] coord;
            controller.ModelGetValue(0, tag, parametric, out coord);
            for (var i = 0; i < expected.Length; ++i)
                Assert.AreEqual(expected[i], coord[i], 1e-08);
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 1, 2, 3, 4, 5, 6 })]
        [DataRow("sphere.geo", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 })]
        public void ParametrizationCurvesTest(string fileName, int[] tags)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            double[] min, max;
            for (var i = 0; i < tags.Length; ++i)
            {
                controller.ModelGetParametrizationBounds(1, tags[i], out min, out max);
                var numInsideCount = controller.gmshModelIsInside(1, tags[i], min, (IntPtr) min.Length, 1, ref ierr);
                Assert.AreNotEqual(0, numInsideCount);
                numInsideCount = controller.gmshModelIsInside(1, tags[i], max, (IntPtr)max.Length, 1, ref ierr);
                Assert.AreNotEqual(0, numInsideCount);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", new int[] { 1 })]
        public void ParametrizationSurfacesTest(string fileName, int[] tags)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            double[] min, max;
            for (var i = 0; i < tags.Length; ++i)
            {
                controller.ModelGetParametrizationBounds(2, tags[i], out min, out max);
                var numInsideCount = controller.gmshModelIsInside(1, tags[i], min, (IntPtr)min.Length, 1, ref ierr);
                Assert.AreNotEqual(0, numInsideCount);
                numInsideCount = controller.gmshModelIsInside(2, tags[i], max, (IntPtr)max.Length, 1, ref ierr);
                Assert.AreNotEqual(0, numInsideCount);
            }
        }

        [DataTestMethod]
        [DataRow("Cilindr.step", "1")]
        public void AttributeTest(string fileName, string modelName)
        {
            var ierr = 0;
            controller.gmshOpen(modelFolder + fileName, ref ierr);
            controller.gmshModelMeshSetTransfiniteCurve(4, 20, "Progression", 1.0, ref ierr);
            var expected = new string[] { "4", "5" };
            controller.gmshModelSetAttribute("Progression", expected, (IntPtr)expected.Length, ref ierr);
            string[] attributes;
            controller.ModelGetAttribute("Progression", out attributes);
            CollectionAssert.AreEqual(expected, attributes);
            var transValues = new string[] { "1", "2" };
            controller.gmshModelSetAttribute("Transfinite", transValues, (IntPtr)2, ref ierr);
            string[] names;
            controller.ModelGetAttributeNames(out names);
            CollectionAssert.AreEqual(new string[] { "Progression", "Transfinite" }, names);
            controller.gmshModelRemoveAttribute("Progression", ref ierr);
            controller.ModelGetAttributeNames(out names);
            CollectionAssert.AreEqual(new string[] { "Transfinite" }, names);
        }

        private void RemoveNameTest(ref string entName, int dim, int delId, int maxId)
        {
            var ierr = 0;
            var status = maxId >= delId;
            var delName = entName + delId.ToString();
            controller.gmshModelRemoveEntityName(delName, ref ierr);
            /*if (status)
                Assert.AreEqual("", ControllerGeometry.GetEntityName(dim, delId));*/
        }
    }
}
