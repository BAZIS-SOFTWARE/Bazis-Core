using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.MeshObjects;
using ModelController.MeshObjsUtility;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System;

namespace TestModel
{
    /// <summary>
    /// Сводное описание для TestObjectsRenumber
    /// </summary>
    [TestClass]
    public class TestObjectsRenumber
    {
        [TestMethod]
        public void BaseCheck()
        {
            var expectedTypes = new[] { ObjType.Элемент1D, ObjType.Элемент2D, ObjType.Элемент2D, ObjType.Элемент3D};
            var beamNodes = new Node[] { new Node(0, new Point3D()), new Node(1, new Point3D(1, 1, 1)) };
            var quadNodes = new Node[] { new Node(0, new Point3D()), new Node(1, new Point3D(1, 0, 0)),
                new Node(2, new Point3D(0, 1, 0)), new Node(3, new Point3D(1, 1, 0)) };
            var triangleNodes = new Node[] { new Node(0, new Point3D()), new Node(1, new Point3D(1, 1, 1)),
                new Node(2, new Point3D(1, 0, 1)) };
            var tetraNodes = new Node[] { new Node(0, new Point3D()), new Node(1, new Point3D(1, 1, 1)), new Node(2, new Point3D(2, 1, 1)),
                new Node(3, new Point3D(1, 2, 1)) };

            var modelData = new ModelData();
            modelData.ObjectData.E1DCollection.Add(new Beam(4, beamNodes));
            modelData.ObjectData.E2DCollection.Add(new Quad(1, quadNodes));
            modelData.ObjectData.E2DCollection.Add(new Triangle(2, triangleNodes));
            modelData.ObjectData.E3DCollection.Add(new Tetra(6, tetraNodes));

            var renumber = new ObjectsRenumber();
            renumber.Renumber(modelData.ObjectData, ObjType.Элемент);

            var i = 0;
            foreach (var data in modelData.ObjectData.GetObjects(ObjType.Элемент))
            {
                Assert.AreEqual(i, data.Number);
                Assert.AreEqual(expectedTypes[i], data.ObjType);
                i++;
            }
        }


        [TestMethod]
        [DataRow(ObjType.Точка)]
        [DataRow(ObjType.Фигура)]
        [DataRow(ObjType.Фигура3D)]
        [DataRow(ObjType.Фигура2D)]
        public void ObjectRenumber_ShouldFail_WhenGeometry(ObjType ot)
        {
            Assert.ThrowsException<Exception>( () => new ObjectsRenumber().Renumber(new ModelData().ObjectData, ot));
        }

        [TestMethod]
        [DataRow(ObjType.Линия)]
        [DataRow(ObjType.Поверхность)]
        public void ObjectRenumber_ShouldFail_WhenNotImplementObjType(ObjType ot)
        {
            Assert.ThrowsException<NotImplementedException>(() => new ObjectsRenumber().Renumber(new ModelData().ObjectData, ot));
        }
    }
}
