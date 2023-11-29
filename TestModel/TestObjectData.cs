using GmshApi.GmshController;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.MeshObjects;
using Model.IO;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using Model.GeometryObjects;

namespace TestModel
{
    [TestClass]
    public class TestObjectData
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestObjectDataAddObjectWithSameNumbers()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());

            model.ObjectData.Add(node0);
            //assert
            model.ObjectData.Add(node0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestObjectDataAddObjectsWithSameNumbers()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());
            var node1 = new Node(1, new Geometry.Point3D());

            model.ObjectData.Add(node0);
            //assert
            model.ObjectData.AddRange(new List<Node>() { node0 ,node1});
        }

        [TestMethod]
        public void TestObjectDataRemoveObjectsWithSameTypes()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());
            var node1 = new Node(1, new Geometry.Point3D());
            var node2 = new Node(3, new Geometry.Point3D());

            var line = new Line(2, new Node[] { node0, node1 });

            model.ObjectData.Add(node0);
            model.ObjectData.Add(node1);
            model.ObjectData.Add(line);
            model.ObjectData.Add(node2);

            model.ObjectData.Remove(ObjType.Линия);
            //assert
            Assert.AreEqual(model.ObjectData.NumberOfNodes, 3);
            Assert.AreEqual(model.ObjectData.Find(ObjType.Линия,0), null);
            Assert.AreEqual(model.ObjectData.Find(ObjType.Линия,3), null);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\STL\ACII.stl")]
        public void TestRemoveAllData(string stlPath)
        {
            var ierr = 0;
            var status = true;
            var modelData = new ModelData();

            var controller = new GmshController(@"..\..\..\..\GmshApi\gmsh.dll");
            controller.gmshMerge(stlPath, ref ierr);
            var nodes = controller.GetNodes(ref status);
            Assert.AreEqual(true, status);
            Assert.AreEqual(4, nodes.Count);
            var surfaces = controller.GetMeshEntities(2, -1, ref status);
            Assert.AreEqual(true, status);
            Assert.AreEqual(2, surfaces.Count);
            modelData.ObjectData.AddRange(nodes);
            modelData.ObjectData.AddRange(surfaces);
            //modelData.ObjectData.Sort();
            modelData.ObjectData.Remove(ObjType.Элемент2D);
            modelData.ObjectData.Remove(ObjType.Узел);//Здесь падает, эмулирую такое же поведение как в Базисе

            //assert
            Assert.AreEqual(modelData.ObjectData.Count, 0);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(0)]
        [DataRow(2)]
        public void TestRemoveSomeObjects(int objNumber)
        {
            //assing
            var model = new ModelData();
            model.Loader = new LoadModelFromASCIITextFile();

            model.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");

            //act
            model.ObjectData.Find(ObjType.Узел,objNumber).ExistState = false;
            model.ObjectData.ClearNotExisted();

            //assert
            var res = model.ObjectData.Find(ObjType.Узел, objNumber);
            if (res != null)
                throw new Exception();
        }
    }
}
