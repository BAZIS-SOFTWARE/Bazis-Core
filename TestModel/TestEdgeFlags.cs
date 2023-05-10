using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using ModelController.ModelScenePresentator;
using ModelController.ModelScenePresentator.GlObjsArraysCreators;
using ModelController.ModelScenePresentator.GlObjsVertexCreators;
using ModelParcer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            var presentator = new ModelScenePresentator(model);
            var values = presentator.CreateVBOIndexes("Элементы2D");
            var edges = presentator.CreateVBOEdges("Элементы2D", values.Item3);
            Assert.AreEqual(3, edges.Length);
            for (var i = 0; i < edges.Length; ++i)
                Assert.AreEqual(true, edges[i]);
        }
        [TestMethod]
        public void TestOnSimpleRectangle() 
        {
            var model = new ModelData();
            var first = new Node[]
            {
                new Node(0, new Point3D(0.0f,0.0f,0.0f)),
                new Node(1, new Point3D(1.0f,0.0f,0.0f)),
                new Node(2, new Point3D(1.0f,1.0f,0.0f))
            };
            var second = new Node[]  
            {
                new Node(3, new Point3D(0.0f,0.0f,0.0f)),
                new Node(4, new Point3D(1.0f,1.0f,0.0f)),
                new Node(5, new Point3D(0.0f,1.0f,0.0f))
            };
            var el2D = new Element2D(0, first, ObjKind.Triangle);
            var el2D2 = new Element2D(1, second, ObjKind.Triangle);
            model.ObjectData.Add(el2D);
            model.ObjectData.Add(el2D2);
            var presentator = new ModelScenePresentator(model);
            var values = presentator.CreateVBOIndexes("Элементы2D");
            var edges = presentator.CreateVBOEdges("Элементы2D", values.Item3);
            Assert.AreEqual(6, edges.Length);
            var expectedFlags = new bool[] { true, true, false, false, true, true };
            for (var i = 0; i < expectedFlags.Length; ++i)
                Assert.AreEqual(true, edges[i]);
        }

        [TestMethod]
        public void TestOnPolygon()
        {
            var model = new ModelData();
            var first = new Node[]
            {
                new Node(0, new Point3D(0.0f,0.0f,0.0f)),
                new Node(1, new Point3D(1.0f,0.0f,0.0f)),
                new Node(2, new Point3D(1.0f,1.0f,0.0f))
            };
            var second = new Node[]
            {
                new Node(3, new Point3D(0.0f,0.0f,0.0f)),
                new Node(4, new Point3D(1.0f,1.0f,0.0f)),
                new Node(5, new Point3D(0.0f,1.0f,0.0f))
            };
            var third = new Node[]
            {
                new Node(6, new Point3D(0.0f,1.0f,0.0f)),
                new Node(7, new Point3D(1.0f,1.0f,0.0f)),
                new Node(8, new Point3D(1.0f,2.0f,0.0f))
            };
            var four = new Node[]
            {
                new Node(9, new Point3D(0.0f,1.0f,0.0f)),
                new Node(10, new Point3D(1.0f,2.0f,0.0f)),
                new Node(11, new Point3D(0.0f,2.0f,0.0f))
            };
            var five = new Node[]
            {
                new Node(12, new Point3D(0.0f,2.0f,0.0f)),
                new Node(13, new Point3D(1.0f,2.0f,0.0f)),
                new Node(14, new Point3D(1.0f,3.0f,0.0f))
            };
            var el2D = new Element2D(0, first, ObjKind.Triangle); var el2D2 = new Element2D(1, second, ObjKind.Triangle);
            var el2D3 = new Element2D(2, second, ObjKind.Triangle); var el2D4 = new Element2D(3, second, ObjKind.Triangle);
            var el2D5 = new Element2D(4, second, ObjKind.Triangle);
            model.ObjectData.Add(el2D); 
            model.ObjectData.Add(el2D2); 
            model.ObjectData.Add(el2D3); 
            model.ObjectData.Add(el2D4);
            model.ObjectData.Add(el2D5);
            var presentator = new ModelScenePresentator(model);
            var values = presentator.CreateVBOIndexes("Элементы2D");
            var edges = presentator.CreateVBOEdges("Элементы2D", values.Item3);
            Assert.AreEqual(15, edges.Length);
            var expectedFlags = new bool[] { true, true, false, false, true, true, 
                                             true, true, false, false, true, true,
                                             true, true, true };
            for (var i = 0; i < expectedFlags.Length; ++i)
                Assert.AreEqual(true, edges[i]);
        }
    }
}
