using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using ModelController.ModelScenePresentator;
using System;
using System.Drawing;
using System.Linq;

namespace TestModel
{
    [TestClass]
    public class TestModelScenePresentor
    {
        [TestMethod]
        public void Select1DObjects()
        {
            //assign
            var nodes = new Node[]
            {
                new Node(0, new Geometry.Point3D(0,0,0)),
                new Node(1,new Geometry.Point3D(10,10,0)),
                new Node(2,new Geometry.Point3D(20,10,0))
            };

            var edgeElems = new Element1D[]
            {
                new Element1D(3,new Node[]{nodes[0],nodes[1]}, Model.Interfaces.ObjKind.Beam),
                new Element1D(4,new Node[]{nodes[1],nodes[2]}, Model.Interfaces.ObjKind.Beam)
            };

            var model = new ModelData();
            model.ObjectData.AddRange(edgeElems);

            var presenter = new ModelScenePresentator(model);

            //act
            presenter.ColorObject(1, Color.Green, "Элементы1D");

            //assert
            Assert.AreEqual(model.ObjectData[0].MasterColor, Color.FromArgb(255, 192, 203));
            Assert.AreEqual(model.ObjectData[1].MasterColor, Color.Green);            
        }

        [TestMethod]
        public void HideInside2DObjects()
        {
            //assign
            var nodes = new Node[]
            {
                new Node(0, new Geometry.Point3D(0,0,0)),
                new Node(1,new Geometry.Point3D(10,10,0)),
                new Node(2,new Geometry.Point3D(20,10,0))
            };

            var surface = new Surface(3, nodes, ObjKind.ElementSurface);

            var model = new ModelData();
            model.ObjectData.AddRange(nodes);
            model.ObjectData.Add(surface);

            var presenter = new ModelScenePresentator(model);

            //act
            presenter.HideInsideObjects("Поверхность");

            //assert
            var surfObj = (ISurfaceObject)model.ObjectData[3];
            Assert.AreEqual(surfObj.GetSurface().All(x => x.ViewState == true), true);
        }

        [TestMethod]
        public void Hide1DObjects()
        {
            //assign
            var nodes = new Node[]
            {
                new Node(0, new Geometry.Point3D(0,0,0)),
                new Node(1,new Geometry.Point3D(10,10,0)),
                new Node(2,new Geometry.Point3D(20,10,0))
            };

            var edgeElems = new Element1D[]
            {
                new Element1D(3,new Node[]{nodes[0],nodes[1]}, Model.Interfaces.ObjKind.Beam),
                new Element1D(4,new Node[]{nodes[1],nodes[2]}, Model.Interfaces.ObjKind.Beam)
            };

            var model = new ModelData();
            model.ObjectData.AddRange(nodes);
            model.ObjectData.AddRange(edgeElems);

            var presenter = new ModelScenePresentator(model);

            //act
            presenter.ColorObject(1, Color.Green, "Элементы1D");

            presenter.HideObjects(Color.Green, "Элементы1D");

            //assert
            Assert.AreEqual(model.ObjectData[3].ViewState, true);
            Assert.AreEqual(model.ObjectData[4].ViewState, false);
        }

        [TestMethod]
        public void Hide3DObjects_byType()
        {
            //assign
            var nodes = new Node[]
            {
                new Node(0, new Geometry.Point3D(0,0,0)),
                new Node(1,new Geometry.Point3D(10,0,0)),
                new Node(2,new Geometry.Point3D(0,10,0)),
                new Node(3,new Geometry.Point3D(0,0,10))
            };

            var tetra = new Element3D(4, nodes, Model.Interfaces.ObjKind.Tetra);

            var model = new ModelData();
            model.ObjectData.AddRange(nodes);
            model.ObjectData.Add(tetra);

            var presenter = new ModelScenePresentator(model);

            //act

            presenter.HideObjects("Элементы3D");

            //assert
            Assert.AreEqual(model.ObjectData[4].ViewState, false);

            var surfObj = (ISurfaceObject)model.ObjectData[4];
            Assert.AreEqual(surfObj.GetSurface().All(x => x.ViewState == false), true);
        }

        [TestMethod]
        public void ShowAll3DObjects_byType()
        {
            //assign
            var nodes = new Node[]
            {
                new Node(0, new Geometry.Point3D(0,0,0)),
                new Node(1,new Geometry.Point3D(10,0,0)),
                new Node(2,new Geometry.Point3D(0,10,0)),
                new Node(3,new Geometry.Point3D(0,0,10))
            };

            var tetra = new Element3D(4, nodes, Model.Interfaces.ObjKind.Tetra);

            var model = new ModelData();
            model.ObjectData.AddRange(nodes);
            model.ObjectData.Add(tetra);

            var presenter = new ModelScenePresentator(model);

            //act
            presenter.HideObjects("Узлы");
            presenter.HideObjects("Элементы3D");
            presenter.ShowAllObjects();

            //assert
            Assert.AreEqual(model.ObjectData[0].ViewState, true);
            Assert.AreEqual(model.ObjectData[1].ViewState, true);
            Assert.AreEqual(model.ObjectData[2].ViewState, true);
            Assert.AreEqual(model.ObjectData[3].ViewState, true);
            Assert.AreEqual(model.ObjectData[4].ViewState, true);

            var surfObj = (ISurfaceObject)model.ObjectData[4];
            Assert.AreEqual(surfObj.GetSurface().All(x => x.ViewState == true), true);
        }

        [TestMethod]
        public void Hide3DObjects_byColor()
        {
            //assign
            var nodes = new Node[]
            {
                new Node(0, new Geometry.Point3D(0,0,0)),
                new Node(1,new Geometry.Point3D(10,0,0)),
                new Node(2,new Geometry.Point3D(0,10,0)),
                new Node(3,new Geometry.Point3D(0,0,10))
            };

            var tetra = new Element3D(4, nodes, Model.Interfaces.ObjKind.Tetra);
            tetra.MasterColor = Color.Green;

            var model = new ModelData();
            model.ObjectData.AddRange(nodes);
            model.ObjectData.Add(tetra);

            var presenter = new ModelScenePresentator(model);

            //act

            presenter.HideObjects(Color.Green,"Элементы3D");

            //assert
            Assert.AreEqual(model.ObjectData[4].ViewState, false);

            var surfObj = (ISurfaceObject)model.ObjectData[4];
            Assert.AreEqual(surfObj.GetSurface().All(x => x.ViewState == false), true);
        }

        [TestMethod]
        public void Show3DObjects()
        {
            //assign
            var nodes = new Node[]
            {
                new Node(0, new Geometry.Point3D(0,0,0)),
                new Node(1,new Geometry.Point3D(10,0,0)),
                new Node(2,new Geometry.Point3D(0,10,0)),
                new Node(3,new Geometry.Point3D(0,0,10))
            };

            var tetra = new Element3D(4, nodes, Model.Interfaces.ObjKind.Tetra);

            var model = new ModelData();
            model.ObjectData.AddRange(nodes);
            model.ObjectData.Add(tetra);

            var presenter = new ModelScenePresentator(model);

            //act

            presenter.HideObjects("Элементы3D");
            presenter.ShowObjects("Элементы3D");

            //assert
            Assert.AreEqual(model.ObjectData[4].ViewState, true);

            var surfObj = (ISurfaceObject)model.ObjectData[4];
            Assert.AreEqual(surfObj.GetSurface().All(x => x.ViewState == true), true);
        }
    }
}
