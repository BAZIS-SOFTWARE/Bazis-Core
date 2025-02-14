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
        [DataTestMethod]
        [DataRow(ObjType.Элемент1D)]
        [DataRow(ObjType.Элемент2D)]
        [DataRow(ObjType.Элемент2D)]
        [DataRow(ObjType.Элемент3D)]
        public void BaseCheck(ObjType expType)
        {
            //var expectedTypes = new[] { ObjType.Элемент1D, ObjType.Элемент2D, ObjType.Элемент2D, ObjType.Элемент3D};
            var beamNodes = new Node[] { new Node(0, new Point3D()), new Node(1, new Point3D(1, 1, 1)) };
            var quadNodes = new Node[] { new Node(0, new Point3D()), new Node(1, new Point3D(1, 0, 0)),
                new Node(2, new Point3D(0, 1, 0)), new Node(3, new Point3D(1, 1, 0)) };
            var triangleNodes = new Node[] { new Node(0, new Point3D()), new Node(1, new Point3D(1, 1, 1)),
                new Node(2, new Point3D(1, 0, 1)) };
            var tetraNodes = new Node[] { new Node(0, new Point3D()), new Node(1, new Point3D(1, 1, 1)), new Node(2, new Point3D(2, 1, 1)),
                new Node(3, new Point3D(1, 2, 1)) };

            var modelData = new ModelData();
            modelData.ObjectData.E1DCollection.Add(ObjType.Элемент1D.ToString(), new Beam(4, beamNodes));
            modelData.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(),new Quad(1, quadNodes));
            modelData.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(),new Triangle(2, triangleNodes));
            modelData.ObjectData.E3DCollection.Add(ObjType.Элемент3D.ToString(),new Tetra(6, tetraNodes));

            var renumber = new ObjectsRenumber();
            renumber.Renumber(modelData.ObjectData, expType);

            var i = 0;
            foreach (var data in modelData.ObjectData.GetObjects(expType))
            {
                Assert.AreEqual(i, data.Number);
                Assert.AreEqual(expType, data.ObjType);
                i++;
            }
        }


        [TestMethod]
        [DataRow(ObjType.Точка)]
        [DataRow(ObjType.Объем)]
        [DataRow(ObjType.Поверхность)]
        public void ObjectRenumber_ShouldFail_WhenGeometry(ObjType ot)
        {
            Assert.ThrowsException<Exception>( () => new ObjectsRenumber().Renumber(new ModelData().ObjectData, ot));
        }

        [TestMethod]
        [DataRow(ObjType.Кривая)]
        public void ObjectRenumber_ShouldFail_WhenNotImplementObjType(ObjType ot)
        {
            Assert.ThrowsException<NotImplementedException>(() => new ObjectsRenumber().Renumber(new ModelData().ObjectData, ot));
        }
    }
}
