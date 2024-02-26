using GmshApi.GmshController;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.MeshObjects;
using Model.IO;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using Model.GeometryObjects;
using ModelInterfaces.ObjectsFinders;
using Geometry;
using System.Linq;

namespace TestModel
{
    [TestClass]
    public class TestModelData
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
            model.ObjectData.AddRange(nodes);
            model.ObjectData.AddRange(objs);
            //act
            var elements = model.ObjectData.FindMany(objsNumbr);
            //assert
            Assert.AreEqual(objsCount, elements.Count());
        }

        [DataTestMethod]
        [DataRow(ObjType.Элемент3D)]
        [DataRow(ObjType.Узел)]
        public void SortAllObjs(ObjType type)
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)),
                new Node(2,new Point3D(10,0,0)),
                new Node(1,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
            };
            var objs = new Model.ModelObject[]
            {
                new Tetra( 10,nodes),
                new Tetra( 4,nodes),
                new Tetra(5,nodes),
                new Tetra( 6,nodes),
                new Tetra(3,nodes),
                new Tetra(8,nodes)
            };

            //act

            var model = new ModelData();
            model.ObjectData.AddRange(nodes);
            model.ObjectData.AddRange(objs);

            model.ObjectData.Sort();

            //assert
            var numb = model.ObjectData[type].First().Number;
            foreach (var item in model.ObjectData[type].Skip(1))
            {
                if (item.Number < numb)
                    throw new Exception("Уменьшение номера");
                numb = item.Number;
            }
   
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
            model.ObjectData.AddRange(nodes);
            model.ObjectData.AddRange(objs);

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

            model.ObjectData.Add(node0);
            //assert
            model.ObjectData.Add(node0);
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

            model.ObjectData.Add(node0);
            //assert
            model.ObjectData.AddRange(new List<Node>() { node0 ,node1});
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

            model.ObjectData.Add(node0);
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
            var node0 = new GeometryPoint(0, new Geometry.Point3D());
            var node1 = new GeometryPoint(1, new Geometry.Point3D());
            var node2 = new GeometryPoint(3, new Geometry.Point3D());

            var line = new Line(2, new GeometryPoint[] { node0, node1 });

            model.ObjectData.Add(node0);
            model.ObjectData.Add(node1);
            model.ObjectData.Add(line);
            model.ObjectData.Add(node2);

            model.ObjectData.Remove(objType);
            //assert
            Assert.AreEqual(model.ObjectData.ContainsKey(objType), false);
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
