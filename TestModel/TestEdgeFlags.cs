using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelInterfaces;
using ModelController.ModelScenePresentator;
using Model.IO;
using System;

namespace TestModel
{
    [TestClass]
    public class TestEdgeFlags
    {
        [TestMethod]
        public void TestOnSimpleTriangle() 
        { 
            var model = new ModelData();
            var nodes = new Node[]
            {
                new Node(0, new Point3D(0.0f,0.0f,0.0f)),
                new Node(1, new Point3D(1.0f,0.0f,0.0f)),
                new Node(2, new Point3D(0.5f,1.0f,0.0f)),
            };
            var el2D = new Element2D(0, nodes, ObjKind.Triangle);
            model.ObjectData.Add(el2D);
            var presentator = new ModelScenePresentator(model.ObjectData);
            var values = presentator["Элементы2D"].CreateIndexes();
            var edges = presentator["Элементы2D"].CreateEdgeFlags(values.Item4);
            Assert.AreEqual(3, edges.Length);
            for (var i = 0; i < edges.Length; ++i)
                Assert.AreEqual(true, edges[i]);
        }
        [TestMethod]
        public void TestOnSimpleRectangle() 
        {
            var model = new ModelData();
            var nodes = new Node[]
            {
                new Node(0, new Point3D(0.0f,0.0f,0.0f)),
                new Node(1, new Point3D(1.0f,0.0f,0.0f)),
                new Node(2, new Point3D(1.0f,1.0f,0.0f)),
                new Node(3, new Point3D(0.0f,1.0f,0.0f))
            };
            var el2D = new Element2D(0, nodes, ObjKind.Quad);

            model.ObjectData.Add(el2D);

            var presentator = new ModelScenePresentator(model.ObjectData);
            var values = presentator["Элементы2D"].CreateIndexes();
            var edges = presentator["Элементы2D"].CreateEdgeFlags(values.Item4);
            Assert.AreEqual(6, edges.Length);
            var expectedFlags = new bool[] { true, true, false, false, true, true };
            for (var i = 0; i < expectedFlags.Length; ++i)
                Assert.AreEqual(expectedFlags[i], edges[i]);
        }

        [TestMethod]
        public void TestOnPolygon()
        {
            var model = new ModelData();
            var nodes = new Node[]
            {
                new Node(0, new Point3D(0.0f,0.0f,0.0f)),
                new Node(1, new Point3D(1.0f,0.0f,0.0f)),
                new Node(2, new Point3D(1.0f,1.0f,0.0f)),
                new Node(3, new Point3D(0.0f,1.0f,0.0f)),
                new Node(4, new Point3D(-1.0f,1.0f,0.0f)),
                new Node(5, new Point3D(-1.0f,0.0f,0.0f))
            };
            var poligon = new Surface(0, nodes, ObjKind.ElementSurface); 
            model.ObjectData.Add(poligon);
            var presentator = new ModelScenePresentator(model.ObjectData);
            var values = presentator["Поверхности"].CreateIndexes();
            var edges = presentator["Поверхности"].CreateEdgeFlags(values.Item4);
            
            Assert.AreEqual(12, edges.Length);
            var expectedFlags = new bool[] { true, true, false, false, true, false,
                                             false, true, false, false, true, true};
            for (var i = 0; i < expectedFlags.Length; ++i)
                Assert.AreEqual(expectedFlags[i], edges[i]);
        }
    }
}
