using ModelController.MeshObjsUtility;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelInterfaces;
using Model.IO;
using System.Collections.Generic;
using System.Linq;
using Model.MeshObjects;

namespace TestModel
{
    [TestClass]
    public class TestsCutSection
    {
        private Element3D tetra;
        private Element3D hex;

        [TestInitialize]
        public void InitializateTetra()
        {
            var tetraLoader = new LoadModelFromCDBTextFile();
            tetraLoader.LoadEvent += (arg1, arg2) => { };
            var modelTetraData = tetraLoader.Load(@"..\..\..\Examples\CDB\tetra.cdb");
            var modelTetraNodes = modelTetraData.ObjectData.FindMany<Node>().ToArray();
            tetra = new Tetra(1, modelTetraNodes);

            var hexLoader = new LoadModelFromCDBTextFile();
            hexLoader.LoadEvent += (arg1, arg2) => { };
            var modelHexData = hexLoader.Load(@"..\..\..\Examples\CDB\hex.cdb");
            var modelHexNodes = modelHexData.ObjectData.FindMany<Node>().ToArray();
            hex = new Tetra(1, modelHexNodes);
        }

        public bool CheckSetEquality<T>(IEnumerable<T> set1, IEnumerable<T> set2)
        {
            return set1.ToHashSet().Except(set2).Count() == 0 &&
                set2.ToHashSet().Except(set1).Count() == 0;
        }

            [DataTestMethod]
        [DataRow(new float[]
        {
            0.6f, 0, 0.4f, 0, 0.5f, 0.5f,0, 0, 0.5f
        })]
        [DataRow(new float[]
        {
            1, 0, 0, 0, 0, 0,0, 1, 0
        })]
        public void TestTetraCrossSection(float[] array)
        {
            var points = new List<Point3D>()
            {
                new Point3D(array[0],array[1],array[2]),
                new Point3D(array[3],array[4],array[5]),
                new Point3D(array[6],array[7],array[8])
            };
            var plane = new Plane(points[0], points[1], points[2]);
            var crossSectionCreator = new CrossSection();
            var crossNodes = crossSectionCreator.GetSectionSurfaces(new List<Element3D> { tetra }, plane);
            var result = crossNodes[1];
            Assert.AreEqual(3, result.GetPoints().Count());
            Assert.IsTrue(CheckSetEquality(points, result.GetPoints()));
        }

        [TestMethod]
        public void TestHexaCrossSection()
        {
            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 1),
                new Point3D(0, 1, 1),
                new Point3D(1, 0, 0.5f),
                new Point3D(1, 1, 0.5f),
            };
            var plane = new Plane(points[0], points[1], points[2]);

            var crossSectionCreator = new CrossSection();
            var crossNodes = crossSectionCreator.GetSectionSurfaces(new List<Element3D> { hex }, plane);
          
            var result = crossNodes[1];
            Assert.AreEqual(4, result.GetPoints().Count());
            Assert.IsTrue(CheckSetEquality(result.GetPoints(), points));
        }

        [TestMethod]
        public void TestLongModelCrossSection()
        {
            var loader = new LoadModelFromCDBTextFile();
            loader.LoadEvent += (arg1, arg2) => { };
            var modelData = loader.Load(@"..\..\..\Examples\CDB\longModel.cdb");

            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 0.1f),
                new Point3D(1, 0, 0.1f),
                new Point3D(0, 1, 0.1f)
            };
            var plane = new Plane(points[0], points[1], points[2]);
            var elements = modelData.ObjectData.FindMany<Element3D>().ToList();
            var crossSection = new CrossSection();
            var crossNodes = crossSection.GetSectionSurfaces(elements, plane);
        }

        [TestMethod]
        public void TestCutOnObjectSurface()
        {
            var plane = new Plane(
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 0),
                new Point3D(0, 1, 0));
            var nodes = new Node[] {
                new Node(1, new Point3D(-23, 195, 0)),
                new Node(2, new Point3D(-23, 203, 0)),
                new Node(3, new Point3D(-20, 203, -11)),
                new Node(4, new Point3D(-41, 198, 0)),
            };
            var model = new Tetra(1, nodes);
            var crossSection = new CrossSection();
            var section = crossSection.GetSectionSurfaces(new List<Element3D> { model }, plane);
            var triangleNodes = section.Values.First().GetPoints();
            Assert.IsTrue(triangleNodes.Contains(new Point3D(-23, 195, 0)));
            Assert.IsTrue(triangleNodes.Contains(new Point3D(-23, 203, 0)));
            Assert.IsTrue(triangleNodes.Contains(new Point3D(-41, 198, 0)));
        }
    }
}
