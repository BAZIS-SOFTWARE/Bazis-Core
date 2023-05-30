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
        public void TestOnComplicatedModel()
        {
            var model = new ModelData();
            var loader = new LoadModelFromCDBTextFile();
            loader.LoadEvent += (ar1, ar2) => { };

            //var loadModel = loader.Load(@"..\..\Examples\3D\Lazer.inp");
            var loadModel = loader.Load(@"..\..\Examples\CDB\longModel.cdb");

            model.Load(loadModel);

            var presenter = new ModelScenePresentator(model);

            foreach (string item in presenter?.GetObjectsTypes())
            {
                presenter.HideInsideObjects(item);

                Tuple<int, int, int,int> tuple = presenter.CreateVBOIndexes(item);
                int[] ptrs = presenter.CreateVBOPointers(item, tuple.Item1);
                float[] glCoords = presenter.CreateVBOVertexes(item, tuple.Item2, "координаты");
                float[] glColors = presenter.CreateVBOVertexes(item, tuple.Item3, "цвет");
                float[] glNormals = presenter.CreateVBOVertexes(item, tuple.Item2, "нормаль");
                bool[] edges = presenter.CreateVBOEdges(item, tuple.Item4);

                Assert.AreEqual(tuple.Item4, edges.Length);
            }
        }
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
            var edges = presentator.CreateVBOEdges("Элементы2D", values.Item4);
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

            var presentator = new ModelScenePresentator(model);
            var values = presentator.CreateVBOIndexes("Элементы2D");
            var edges = presentator.CreateVBOEdges("Элементы2D", values.Item4);
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
            var presentator = new ModelScenePresentator(model);
            var values = presentator.CreateVBOIndexes("Поверхность");
            var edges = presentator.CreateVBOEdges("Поверхность", values.Item4);
            
            Assert.AreEqual(12, edges.Length);
            var expectedFlags = new bool[] { true, true, false, false, true, false,
                                             false, true, false, false, true, true};
            for (var i = 0; i < expectedFlags.Length; ++i)
                Assert.AreEqual(expectedFlags[i], edges[i]);
        }
    }
}
