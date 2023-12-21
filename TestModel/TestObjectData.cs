using GmshApi.GmshController;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.MeshObjects;
using Model.IO;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using Model.GeometryObjects;
using Geometry;
using System.Linq;
using Model.GroupsData;

namespace TestModel
{
    [TestClass]
    public class TestObjectData
    {
        [DataTestMethod]
        [DataRow(0,2)]
        [DataRow(8,1)]
        [DataRow(3,1)]
        public void FindManyObjsByNumber(int objsNumbr,int objsCount)
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
            };
            var objs = new Model.ModelObject[]
            {
                new Tetra( 0,nodes),
                new Tetra( 4,nodes),
                new Tetra(5,nodes),
                new Tetra( 6,nodes),
                new Tetra(7,nodes),
                new Tetra(8,nodes)
            };

            var model = new ModelData();
            model.ObjectData.Add(ObjType.Узел,nodes);
            model.ObjectData.Add(ObjType.Элемент3D, objs);
            //act
            var elements = model.ObjectData.FindMany(objsNumbr);
            //assert
            Assert.AreEqual(objsCount, elements.Count());
        }

        [DataTestMethod]
        [DataRow("Все объекты", 10)]
        [DataRow("Тетраэдры", 6)]
        [DataRow("Узлы", 4)]
        public void FindManyObjsByType(string reqType, int objsCount)
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
            };
            var objs = new Model.ModelObject[]
            {
                new Tetra( 0,nodes),
                new Tetra( 4,nodes),
                new Tetra(5,nodes),
                new Tetra( 6,nodes),
                new Tetra(7,nodes),
                new Tetra(8,nodes)
            };

            //act

            var model = new ModelData();
            model.ObjectData.Add(ObjType.Узел, nodes);
            model.ObjectData.Add(ObjType.Элемент3D, objs);

            //assert            
            if (reqType == "Тетраэдры")
                Assert.AreEqual(objsCount, model.ObjectData.FindMany<Tetra>().Count());
            else if (reqType == "Узлы")
                Assert.AreEqual(objsCount, model.ObjectData.FindMany<Node>().Count());
            else
                Assert.AreEqual(objsCount, model.ObjectData.FindMany<IModelObject>().Count());
        }
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AddObjectWithSameNumbers()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());

            model.ObjectData.Add(ObjType.Узел, node0);
            //assert
            model.ObjectData.Add(ObjType.Узел, node0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AddSomeObjectsWithSameNumbers()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());
            var node1 = new Node(1, new Geometry.Point3D());

            model.ObjectData.Add(ObjType.Узел, node0);
            //assert
            model.ObjectData.Add(ObjType.Узел, new List<Node>() { node0 ,node1});
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AddCertainObjectsWithSameNumbers()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());
            var node1 = new Node(1, new Geometry.Point3D());

            var modelObjects = new List<IModelObject>() { node0, node1 };

            model.ObjectData.Add(ObjType.Узел, node0);
            //assert
            model.ObjectData.Add(ObjType.Узел, modelObjects);
        }

        [DataTestMethod]
        [DataRow(ObjType.Линия)]
        [DataRow(ObjType.Точка)]
        public void RemoveObjectsWithSameTypes(ObjType objType)
        {
            //assign
            var model = new ModelData();

            //act
            var point0 = new GeometryPoint(0, new Geometry.Point3D());
            var point1 = new GeometryPoint(1, new Geometry.Point3D());
            var point2 = new GeometryPoint(3, new Geometry.Point3D());

            var line = new Line(2, new GeometryPoint[] { point0, point1 });

            model.ObjectData.Add(ObjType.Точка, point0);
            model.ObjectData.Add(ObjType.Точка, point1);
            model.ObjectData.Add(ObjType.Линия, line);
            model.ObjectData.Add(ObjType.Точка, point2);

            model.Remove(objType);

            //assert
            Assert.AreEqual(model.ObjectData[objType].Count(), 0);
        }

        [DataTestMethod]
        [DataRow(ObjType.Узел, 0, 0)]
        [DataRow(ObjType.Элемент3D, 4, 0)]
        [DataRow(ObjType.Объект, 0, 0)]
        public void RemoveObjectsSyncWithGroups(ObjType objType, int nNumbs,int grps)
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
            };
            var objects = new Model.ModelObject[]
            {
                new Tetra( 0,nodes),
                new Tetra( 4,nodes),
                new Tetra(5,nodes),
                new Tetra( 6,nodes),
                new Tetra(7,nodes),
                new Tetra(8,nodes)
            };

            var group = new Group("test", objType);
            group.AddRange(objects);
            //act

            var model = new ModelData();

            model.ObjectData.Add(ObjType.Узел, nodes);
            model.ObjectData.Add(ObjType.Элемент3D, objects);

            model.GroupData.Add(group);

            model.Remove(objType);
            //assert            
            Assert.AreEqual(model.ObjectData.NumberOfNodes,nNumbs, 0);
            Assert.AreEqual(model.GroupData.FindMany(objType).Count(), grps,0);

        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\STL\ACII.stl", ObjType.Узел, 0)]
        [DataRow(@"..\..\..\Examples\STL\ACII.stl", ObjType.Элемент2D, 4)]
        public void TestRemoveAllData(string stlPath, ObjType objType, int objNumb)
        {
            var ierr = 0;
            var status = true;
            var modelData = new ModelData();

            var controller = new GmshController(@"..\..\..\..\GmshApi\gmsh.dll");
            controller.gmshMerge(stlPath, ref ierr);
            var nodes = controller.GetNodes(ref status);
            Assert.AreEqual(true, status);
            Assert.AreEqual(4, nodes.Count);
            var elements = controller.GetMeshEntities(2, -1, ref status);
            Assert.AreEqual(true, status);
            Assert.AreEqual(2, elements.Count);
            modelData.ObjectData.Add(ObjType.Узел, nodes);
            modelData.ObjectData.Add(ObjType.Элемент2D, elements);
            //modelData.ObjectData.Sort();
            modelData.Remove(objType);
            //assert
            Assert.AreEqual(modelData.ObjectData[ObjType.Узел].Count, objNumb);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(0)]
        [DataRow(2)]
        public void RemoveSomeObjectsByExistState(int objNumber)
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
