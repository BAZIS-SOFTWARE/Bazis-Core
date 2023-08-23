using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using System;
using ModelController.ModelScenePresentator.GlObjsArraysCreators;

namespace TestModel
{
    [TestClass]
    public class TestAlphaColor
    {
        [TestMethod]
        public void TestPointObjsPresenter()
        {
            var nodes = new Node[] { new Node(0, new Point3D(0.5f, 1.0f,-1.0f)) };
            var presenter = new PointObjsPresenter(nodes);
            var indices = presenter.CreateIndexes();
            var pointers = presenter.CreatePointers(indices.Item1);
            var colors = presenter.CreateVertexes(indices.Item3, "цвет");
            //var expectedIndices = new Tuple<int,int,int,int>(1,3,4,0);
            var expectedPointers = new int[] { 0 };
            var expectedColors = new float[] { 0.0f, 0.0f, 1.0f, 1.0f };
            //Assert.AreEqual(expectedIndices, indices);
            CollectionAssert.AreEqual(expectedPointers, pointers);
            CollectionAssert.AreEqual(expectedColors, colors);
        }
        [TestMethod]
        public void TestLineObjsPresenter()
        {
            var nodes = new Node[] { new Node(0, new Point3D(0.5f, 1.0f, 1.0f)),
                                     new Node(1, new Point3D(1.0f, 1.0f, -1.0f))
                                   };
            var line = new Curve[] { new Curve(0, nodes) };
            var presenter = new LineObjsPresenter(line);
            var indices = presenter.CreateIndexes();
            var pointers = presenter.CreatePointers(indices.Item1);
            var colors = presenter.CreateVertexes(indices.Item3, "цвет");
            //var expectedIndices = new Tuple<int, int, int,int>(2, 6, 8,1);
            var expectedPointers = new int[] { 0, 1 };
            var expectedColors = new float[] { 0.0f, 0.0f, 0.0f, 1.0f,
                                               0.0f, 0.0f, 0.0f, 1.0f };
            //Assert.AreEqual(expectedIndices, indices);
            CollectionAssert.AreEqual(expectedPointers, pointers);
            CollectionAssert.AreEqual(expectedColors, colors);
        }

        [TestMethod]
        public void TestSurfaceObjsPresenter()
        {
            var nodes = new Node[] { new Node(0, new Point3D(-0.5f, 0.0f, 0.0f)),
                                     new Node(1, new Point3D(0.5f, 0.0f, 0.0f)),
                                     new Node(2, new Point3D(0.0f, 0.5f, 0.0f))
                                   };
            var triangle = new Element2D[] { new Element2D(0, nodes, ObjKind.Triangle) };
            var presenter = new SurfaceObjsPresenter(triangle);
            var indices = presenter.CreateIndexes();
            var pointers = presenter.CreatePointers(indices.Item1);
            var colors = presenter.CreateVertexes(indices.Item3, "цвет");
            //var expectedIndices = new Tuple<int, int, int,int>(3, 9, 12,3);
            var expectedPointers = new int[] { 0, 1, 2 };
            var expectedColors = new float[] { 0.0f, 0.0f, 1.0f, 1.0f,
                                               0.0f, 0.0f, 1.0f, 1.0f,
                                               0.0f, 0.0f, 1.0f, 1.0f};
            //Assert.AreEqual(expectedIndices, indices);
            CollectionAssert.AreEqual(expectedPointers, pointers);
            CollectionAssert.AreEqual(expectedColors, colors);
        }

    }
}
