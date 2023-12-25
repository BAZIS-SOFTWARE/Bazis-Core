using System;
using ModelController.MeshObjsUtility;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelInterfaces;
using Model.IO;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.MeshObjects;

namespace TestModel
{
    [TestClass]
    public class TestsCrossSection
    {
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
                new Point3D(0.69f, 0.18f, 0.51f),
            };

            var surface = instance.GetSectionSurfaces(new List<Element3D> { tetra }, plane).Values.First();

            var actualPoints = surface.GetCoordinates().ToHashSet();
            Assert.IsTrue(actualPoints.SetEquals(expectedPoints));
        }

        [TestMethod]
        public void GetSectionSurfaces_ShouldBeCorrect_OnSingleHexa()
        {
            var points = new List<Point3D>()
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 0),
                new Point3D(0, 1, 0),
                new Point3D(0, 0, 1),
                new Point3D(1, 1, 0),
                new Point3D(1, 0, 1),
                new Point3D(0, 1, 1),
                new Point3D(1, 1, 1)
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

            var surface = instance.GetSectionSurfaces(new List<Element3D> { hexa }, plane).Values.First();
            var actualPoints = surface.GetCoordinates().ToHashSet();
            Assert.IsTrue(actualPoints.SetEquals(expectedPoints));
        }

        [TestMethod]
        public void GetSectionSurfaces_ShouldWork_OnLargeModel()
        {
            var loader = new LoadModelFromCDBTextFile();
            loader.LoadEvent += (arg1, arg2) => { };
            var modelData = loader.Load(@"..\..\..\Examples\CDB\longModel.cdb");

            // scale model
            foreach (var node in modelData.ObjectData.FindMany<Node>())
            {
                node.Position = new Point3D((float)Math.Round(100 * node._x, 2), (float)Math.Round(100 * node._y, 2), (float)Math.Round(100 * node._z, 2));
            }

            var points = new List<Point3D>
            {
                new Point3D(0, 0, 10),
                new Point3D(1, 0, 10),
                new Point3D(0, 1, 10)
            };
            var plane = new Plane(points[0], points[1], points[2]);
            var elements = modelData.ObjectData.FindMany<Element3D>().ToList();
            var crossSection = new CrossSection();
            var crossNodes = crossSection.GetSectionSurfaces(elements, plane);
        }
    }
}
