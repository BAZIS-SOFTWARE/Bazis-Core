using System;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Model.MeshObjects;
using OperationalController.MeshObjsUtility;

namespace ControllerTests
{
    [TestClass]
    public class TestsCrossSection
    {
        private bool IsPointNearOfAnyPoint(Point3D p, List<Point3D> points)
        {
            var epsilon = 1e-2;
            foreach (var point in points)
            {
                if (Math.Abs(point._x - p._x) < epsilon
                    && Math.Abs(point._y - p._y) < epsilon
                    && Math.Abs(point._z - p._z) < epsilon)
                {
                    return true;
                }
            }
            return false;
        }

        [TestMethod]
        public void GetSectionSurfaces_ShouldBeCorrect_OnSingleTetra()
        {
            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 0),
                new Point3D(0.5f, 0.866f, 0),
                new Point3D(0.5f, 0.289f, 0.816f),
            };
            var nodes = points.Select((p, i) => new Node(i, p)).ToArray();
            var tetra = new Tetra(1, nodes);
            var plane = new Plane(new Point3D(0.29f, 0.5f, 0), new Point3D(0.5f, 0.44f, 0.6f), new Point3D(0.7f, 0, 0));
            var instance = new CrossSection();
            var expectedPoints = new List<Point3D>
            {
                new Point3D(0.29f, 0.5f, 0),
                new Point3D(0.5f, 0.44f, 0.6f),
                new Point3D(0.7f, 0, 0),
                new Point3D(0.69f, 0.18f, 0.5f),
            };

            var surface = instance.GetSectionSurfaces(new List<Element3D> { tetra }, plane);

            var actualPoints = surface.GetCoordinates().ToList();
            Assert.IsTrue(expectedPoints.All(p => IsPointNearOfAnyPoint(p, actualPoints)));
        }

        [TestMethod]
        public void GetSectionSurfaces_ShouldBeCorrect_OnSingleHexa()
        {
            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 0),
                new Point3D(1, 1, 0),
                new Point3D(0, 1, 0),
                new Point3D(0, 0, 1),
                new Point3D(1, 0, 1),
                new Point3D(1, 1, 1),
                new Point3D(0, 1, 1),
            };
            var nodes = points.Select((p, i) => new Node(i, p)).ToArray();
            var hexa = new Hexa(1, nodes);
            var plane = new Plane(new Point3D(0, 0, 0.5f), new Point3D(0.3f, 0, 0), new Point3D(0.8f, 1, 0));
            var instance = new CrossSection();
            var expectedPoints = new List<Point3D>
            {
                new Point3D(0, 0, 0.5f),
                new Point3D(0.3f, 0, 0),
                new Point3D(0.8f, 1, 0),
                new Point3D(0, 0.6f, 1),
                new Point3D(0.2f, 1, 1),
            };

            var surface = instance.GetSectionSurfaces(new List<Element3D> { hexa }, plane);

            var actualPoints = surface.GetCoordinates().ToList();
            Assert.IsTrue(expectedPoints.All(p => IsPointNearOfAnyPoint(p, actualPoints)));
        }

        //[TestMethod]
        //public void GetSectionSurfaces_ShouldWork_OnLargeModel()
        //{
        //    var loader = new LoadModelFromProjectTextFile();
        //    loader.LoadEvent += (arg1, arg2) => { };
        //    var modelData = loader.Load(@"..\..\Examples\3D\1.bpf");

        //    var A = modelData.ObjectData.GetObjects(ObjType.Узел).First(node => node.Number == 26180);
        //    var B = modelData.ObjectData.GetObjects(ObjType.Узел).First(node => node.Number == 27192);
        //    var C = modelData.ObjectData.GetObjects(ObjType.Узел).First(node => node.Number == 29809);

        //    var plane = new Plane(A.CalcCentr(), B.CalcCentr(), C.CalcCentr());
        //    var elements = modelData.ObjectData.E3DCollection;

        //    var instance = new CrossSection();
        //    var section = instance.GetSectionSurfaces(elements, plane);
        //}
    }
}