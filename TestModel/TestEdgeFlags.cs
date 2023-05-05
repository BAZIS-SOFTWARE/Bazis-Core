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
    }
}
