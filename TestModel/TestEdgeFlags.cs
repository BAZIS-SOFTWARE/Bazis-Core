using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelInterfaces;
using ModelController.ModelScenePresentator;
using Model.MeshObjects;
using Model.GeometryObjects;
using ModelController.ModelScenePresentator.GlObjsPresenters;
using ModelInterfaces.MeshObjects;
using Model.ObjectsCollections;
using System.Linq;

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
            var el2D = new Triangle(0, nodes);
            model.ObjectData.E2DCollection.Add(el2D);

            var creator = new PresentersCreator();

            var presentator = creator.CreateSurfaceElementsPresenter(model.ObjectData.E2DCollection, false);

            var values = presentator.CreateIndexes();
            var edges = presentator.CreateEdgeFlags(values.Item4);
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
            var el2D = new Quad(0, nodes);

            model.ObjectData.E2DCollection.Add(el2D);

            var creator = new PresentersCreator();
            
            var presentator = creator.CreateSurfaceElementsPresenter(model.ObjectData.E2DCollection, false);
            var values = presentator.CreateIndexes();
            var edges = presentator.CreateEdgeFlags(values.Item4);
            Assert.AreEqual(6, edges.Length);
            var expectedFlags = new bool[] { true, true, false, false, true, true };
            for (var i = 0; i < expectedFlags.Length; ++i)
                Assert.AreEqual(expectedFlags[i], edges[i]);
        }

        [TestMethod]
        public void TestOnPolygon()
        {
            var model = new ModelData();
            var points = new GeometryPoint[]
            {
                new GeometryPoint(0, new Point3D(0.0f,0.0f,0.0f)),
                new GeometryPoint(1, new Point3D(1.0f,0.0f,0.0f)),
                new GeometryPoint(2, new Point3D(1.0f,1.0f,0.0f)),
                new GeometryPoint(3, new Point3D(0.0f,1.0f,0.0f)),
                new GeometryPoint(4, new Point3D(-1.0f,1.0f,0.0f)),
                new GeometryPoint(5, new Point3D(-1.0f,0.0f,0.0f))
            };
            var poligon = new Figure2D(0, points); 

            model.ObjectData.Fig2DCollection.Add(poligon);
            var creator = new PresentersCreator();

            var presentator = creator.CreateSurfaceGeometryPresenter(model.ObjectData.Fig2DCollection, false);

            var values = presentator.CreateIndexes();
            var edges = presentator.CreateEdgeFlags(values.Item4);
            
            Assert.AreEqual(12, edges.Length);
            var expectedFlags = new bool[] { true, true, false, false, true, false,
                                             false, true, false, false, true, true};
            for (var i = 0; i < expectedFlags.Length; ++i)
                Assert.AreEqual(expectedFlags[i], edges[i]);
        }
    }
}
