using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.ModelScenePresentator;
using System;
using System.Drawing;

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
    }
}
