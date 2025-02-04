using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.MeshObjects;
using Model.IO;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using Model.GeometryObjects;
using Geometry;
using System.Linq;
using Model.GroupsData;
using GmshApi.GmshController;

namespace TestModel
{
    [TestClass]
    public class TestObjectData
    {
        [DataTestMethod]
        [DataRow(ObjType.Узел, 3, DisplayName = "Узел")]
        [DataRow(ObjType.Элемент, 8, DisplayName = "Элемент")]
        [DataRow(ObjType.Элемент3D, 8, DisplayName = "Элемент3D")]
        [DataRow(ObjType.Объект, 8, DisplayName = "Объект")]
        [DataRow(ObjType.Элемент2D, 0, DisplayName = "Элемент2D")]
        [DataRow(ObjType.Элемент1D, 0, DisplayName = "Элемент1D")]
        public void GetLastObjsNumber(ObjType objType, int objsNumbr)
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
            };
            var e3D = new Element3D[]
            {
                new Tetra( 0,nodes),
                new Tetra( 4,nodes),
                new Tetra(5,nodes),
                new Tetra( 6,nodes),
                new Tetra(7,nodes),
                new Tetra(8,nodes)
            };

            var model = new ModelData();
            model.ObjectData.NodeCollection.AddRange(ObjType.Узел.ToString(), nodes);
            model.ObjectData.E3DCollection.AddRange(ObjType.Элемент3D.ToString(), e3D);
            //act
            var findNumber = model.ObjectData.GetMaxNumber(objType);
            //assert
            Assert.AreEqual(objsNumbr, findNumber);
        }

        [DataTestMethod]
        [DataRow(0, 2)]
        [DataRow(8, 1)]
        [DataRow(3, 1)]
        public void FindManyObjsByNumber(int objsNumbr, int objsCount)
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
            };
            var objs = new Element3D[]
            {
                new Tetra( 0,nodes),
                new Tetra( 4,nodes),
                new Tetra(5,nodes),
                new Tetra( 6,nodes),
                new Tetra(7,nodes),
                new Tetra(8,nodes)
            };

            var model = new ModelData();
            model.ObjectData.NodeCollection.AddRange(ObjType.Узел.ToString(), nodes);
            model.ObjectData.E3DCollection.AddRange(ObjType.Элемент3D.ToString(), objs);
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
            var objs = new Element3D[]
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
            model.ObjectData.NodeCollection.AddRange(ObjType.Узел.ToString(), nodes);
            model.ObjectData.E3DCollection.AddRange(ObjType.Элемент3D.ToString(), objs);

            //assert            
            if (reqType == "Тетраэдры")
                Assert.AreEqual(objsCount, model.ObjectData.E3DCollection.Count);
            else if (reqType == "Узлы")
                Assert.AreEqual(objsCount, model.ObjectData.NodeCollection.Count);
            else
                Assert.AreEqual(objsCount, model.ObjectData.Count(ObjType.Объект));
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddObjectWithSameNumbers()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());

            model.ObjectData.NodeCollection.Add(ObjType.Узел.ToString(), node0);
            //assert
            model.ObjectData.NodeCollection.Add(ObjType.Узел.ToString(), node0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddSomeObjectsWithSameNumbers()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());
            var node1 = new Node(1, new Geometry.Point3D());

            model.ObjectData.NodeCollection.Add(ObjType.Узел.ToString(), node0);
            //assert
            model.ObjectData.NodeCollection.AddRange(ObjType.Узел.ToString(), new List<Node>() { node0, node1 });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddCertainObjectsWithSameNumbers()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());
            var node1 = new Node(1, new Geometry.Point3D());

            var modelObjects = new List<Node>() { node0, node1 };

            model.ObjectData.NodeCollection.Add(ObjType.Узел.ToString(), node0);
            //assert
            model.ObjectData.NodeCollection.AddRange(ObjType.Узел.ToString(), modelObjects);
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

            model.ObjectData.PointCollection.Add(ObjType.Точка.ToString(), point0);
            model.ObjectData.PointCollection.Add(ObjType.Точка.ToString(), point1);
            model.ObjectData.LineCollection.Add(ObjType.Линия.ToString(), line);
            model.ObjectData.PointCollection.Add(ObjType.Точка.ToString(), point2);

            model.ObjectData.Clear(objType);
            //assert
            Assert.AreEqual(model.ObjectData.GetObjects(objType).Count(), 0);
        }

        [DataTestMethod]
        [DataRow(ObjType.Узел, 0, 0)]
        [DataRow(ObjType.Элемент3D, 4, 0)]
        [DataRow(ObjType.Объект, 0, 0)]
        public void RemoveObjectsSyncWithGroups(ObjType objType, int nNumbs, int grps)
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
            };
            var objects = new Element3D[]
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

            model.ObjectData.NodeCollection.AddRange(ObjType.Узел.ToString(), nodes);
            model.ObjectData.E3DCollection.AddRange(ObjType.Элемент3D.ToString(), objects);

            model.GroupData.Add(group);

            model.ObjectData.Clear(objType);
            model.GroupData.ClearNotExisted();
            //assert            
            Assert.AreEqual(model.ObjectData.NodeCollection.Count, nNumbs, 0);
            Assert.AreEqual(model.GroupData.FindMany(objType).Count(), grps, 0);

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
            model.ObjectData.Remove(ObjType.Узел, objNumber);
            model.ObjectData.ClearEmpty();

            //assert
            var res = model.ObjectData.Find(ObjType.Узел, objNumber);
            if (res != null)
                throw new Exception();
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\STL\ACII.stl", ObjType.Узел)]
        [DataRow(@"..\..\..\Examples\STL\sphere.stl", ObjType.Узел)]
        public void MoveObjects(string filePath, ObjType objType)
        {
            //assign
            var modelData = new ModelData();
            modelData.Loader = new LoadModelFromSTLFile();
            modelData.Load(filePath);

            var oldCoords = new List<Point3D>();

            foreach (var item in modelData.ObjectData.GetObjects(objType))
                oldCoords.Add(item.CalcCentr());

            //act
            modelData.ObjectData.Move(objType, new Point3D(1, 1, 1));

            //assert
            var newCoords = modelData.ObjectData.GetObjects(objType).
                Select(x => x.CalcCentr()).ToList();

            for (int i = 0; i < newCoords.Count(); i++)
            {
                Assert.AreEqual(oldCoords[i]._x, newCoords[i]._x - 1, 0.001f);
                Assert.AreEqual(oldCoords[i]._y, newCoords[i]._y - 1, 0.001f);
                Assert.AreEqual(oldCoords[i]._z, newCoords[i]._z - 1, 0.001f);
            }

        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\STL\sphere.stl", ObjType.Узел)]
        public void RotateObjects(string filePath, ObjType objType)
        {
            //assign
            var modelData = new ModelData();
            modelData.Loader = new LoadModelFromSTLFile();
            modelData.Load(filePath);

            var oldCoords = new List<Point3D>();

            foreach (var item in modelData.ObjectData.GetObjects(objType))
                oldCoords.Add(item.CalcCentr());

            //act
            modelData.ObjectData.Rotate(objType, new Point3D(1, 1, 1),25.0f);

            //assert
            var newCoords = modelData.ObjectData.GetObjects(objType).
                Select(x => x.CalcCentr()).ToList();

            for (int i = 0; i < newCoords.Count(); i++)
            {
                Assert.AreNotEqual(oldCoords[i], newCoords[i]);
            }
        }
    }
}
