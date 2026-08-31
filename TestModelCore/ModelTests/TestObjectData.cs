using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.GeometryObjects;
using Model.GroupsData;
using Model.Interfaces;
using Model.IO;
using Model.IO.STL;
using Model.MeshObjects;
using Model.ObjectsCollections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ModelTests
{
    [TestClass]
    public class TestObjectData
    {

        [ClassInitialize]
        public static void SetUp(TestContext testContext)
        {

        } 
        [DataTestMethod]
        [DataRow(ObjType.Узел, 3, DisplayName = "Узел")]
        //[DataRow(ObjType.Элемент, 8, DisplayName = "Элемент")]
        [DataRow(ObjType.Элемент3D, 8, DisplayName = "Элемент3D")]
        //[DataRow(ObjType.Объект, 8, DisplayName = "Объект")]
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
            foreach (var item in nodes)
                model.ObjectData.NodesSet.Add(item.Number, item);
            model.ObjectData.E3DCollection.AddRange(ObjType.Элемент3D.ToString(), e3D);
            //act

            var findNumber = model.ObjectData.GetMaxNumber(objType);
            //assert
            Assert.AreEqual(objsNumbr, findNumber);
        }

        [DataTestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        [DataRow(ObjType.Элемент2D, 0, DisplayName = "Элемент2D")]
        [DataRow(ObjType.Элемент1D, 0, DisplayName = "Элемент1D")]
        public void GetLastObjsNumber_ShouldFail(ObjType objType, int objsNumbr)
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
            foreach (var item in nodes)
                model.ObjectData.NodesSet.Add(item.Number, item);
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
            foreach (var item in nodes)
                model.ObjectData.NodesSet.Add(item.Number, item);
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
            foreach (var item in nodes)
                model.ObjectData.NodesSet.Add(item.Number, item);
            model.ObjectData.E3DCollection.AddRange(ObjType.Элемент3D.ToString(), objs);

            //assert            
            if (reqType == "Тетраэдры")
                Assert.AreEqual(objsCount, model.ObjectData.E3DCollection.Count);
            else if (reqType == "Узлы")
                Assert.AreEqual(objsCount, model.ObjectData.NodesSet.Count);
            else
                Assert.AreEqual(objsCount, model.ObjectData.GetAllObjects().Count());
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddObjectWithSameNumbers()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());

            model.ObjectData.NodesSet.Add(node0.Number, node0);
            //assert
            model.ObjectData.NodesSet.Add(node0.Number, node0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddSomeNodesWithSameNumbers()
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D());
            var node1 = new Node(1, new Geometry.Point3D());

            model.ObjectData.NodesSet.Add(node0.Number, node0);
            //assert
            new List<Node>() { node0, node1 }.ForEach(x =>
            model.ObjectData.NodesSet.Add(x.Number, x));
        }

        [TestMethod]
        [DataRow(0, 0, 0, 1)]
        [DataRow(0, 10, 0, 10)]
        [DataRow(1, 0, 1, 2)]
        public void AppendSets(int ini_fNumber, int ini_sNumber, int res_fNumber, int res_sNumber)
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D(0, 0, 0));
            var node1 = new Node(1, new Geometry.Point3D(1, 0, 0));
            var node2 = new Node(2, new Geometry.Point3D(0, 1, 0));

            model.ObjectData.NodesSet.Add(node0.Number, node0);
            model.ObjectData.NodesSet.Add(node1.Number, node0);
            model.ObjectData.NodesSet.Add(node2.Number, node0);


            var set1 = new ObjectsSet<Element2D>("e2D");

            set1.Add(ini_fNumber, new Triangle_c(ini_fNumber, new[] { node0, node1, node2 }));
            var set2 = new ObjectsSet<Element2D>("e2D");
            set2.Add(ini_sNumber, new Triangle_c(ini_sNumber, new[] { node0, node1, node2 }));
            set1.Append(set2);
            //assert
            Assert.AreEqual(set1.Count, 2);
            Assert.AreEqual(set1[res_fNumber].Number, res_fNumber);
            Assert.AreEqual(set1[res_sNumber].Number, res_sNumber);

        }

        [TestMethod]
        [DataRow(new int[] { 0, 1, 2, 3 }, new int[] { 0, 1, 2, 3 })]
        [DataRow(new int[] { 0, 1, 0, 1 }, new int[] { 0, 1, 2, 3 })]
        [DataRow(new int[] { 0, 10, 1, 2 }, new int[] { 0, 10, 12, 13 })]
        public void AppendCollections(int[] iniNumb, int[] resNumb)
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D(0, 0, 0));
            var node1 = new Node(1, new Geometry.Point3D(1, 0, 0));
            var node2 = new Node(2, new Geometry.Point3D(0, 1, 0));

            model.ObjectData.NodesSet.Add(node0.Number, node0);
            model.ObjectData.NodesSet.Add(node1.Number, node0);
            model.ObjectData.NodesSet.Add(node2.Number, node0);


            var set1 = new ObjectsSet<Element2D>("e2D_1");
            set1.Add(iniNumb[0], new Triangle_c(iniNumb[0], new[] { node0, node1, node2 }));
            set1.Add(iniNumb[1], new Triangle_c(iniNumb[1], new[] { node0, node1, node2 }));

            var set2 = new ObjectsSet<Element2D>("e2D_2");
            set2.Add(iniNumb[2], new Triangle_c(iniNumb[2], new[] { node0, node1, node2 }));
            set2.Add(iniNumb[3], new Triangle_c(iniNumb[3], new[] { node0, node1, node2 }));

            var col1 = new ObjectsCollection<Element2D>();
            col1.Add(set1);

            var col2 = new ObjectsCollection<Element2D>();
            col2.Add(set2);

            var addedNames = col1.Append(col2);

            //assert
            Assert.AreEqual(col1.Count, 4);

            Assert.AreEqual(col1.GetMaxNumber(), resNumb.Last());
            Assert.AreEqual(col1.GetMinNumber(), resNumb.First());

            CollectionAssert.AreEquivalent(new[] { "e2D_2" }, addedNames.ToArray());
            CollectionAssert.AreEquivalent(new[] { "e2D_1", "e2D_2" }, col1.Names.ToArray());
        }

        private static ObjectsSet<Element2D> CreateElement2DSet(string name, params int[] numbers)
        {
            var nodes = CreateTriangleNodes();
            var set = new ObjectsSet<Element2D>(name);

            foreach (var number in numbers)
                set.Add(number, new Triangle_c(number, nodes));

            return set;
        }

        private static ObjectsCollection<Element2D> CreateElement2DCollection(params ObjectsSet<Element2D>[] sets)
        {
            var collection = new ObjectsCollection<Element2D>();

            foreach (var set in sets)
                collection.Add(set);

            return collection;
        }

        [TestMethod]
        public void AppendKeepsOriginalSetNames()
        {
            //assign
            var target = new ObjectsCollection<Element2D>();
            var source = CreateElement2DCollection(
                CreateElement2DSet("beams", 0, 1),
                CreateElement2DSet("shells", 2, 3));

            //act
            var addedNames = target.Append(source);

            //assert
            CollectionAssert.AreEquivalent(new[] { "beams", "shells" }, addedNames.ToArray());
            CollectionAssert.AreEquivalent(new[] { "beams", "shells" }, target.Names.ToArray());
            Assert.AreEqual(4, target.Count);
        }

        [TestMethod]
        public void AppendAddsSuffixWhenSetNameIsBusy()
        {
            //assign
            var target = CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1));
            var source = CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1));

            //act
            var addedNames = target.Append(source);

            //assert
            Assert.AreEqual(1, addedNames.Count);
            Assert.AreEqual("extrude_1", addedNames[0]);
            CollectionAssert.AreEquivalent(new[] { "extrude", "extrude_1" }, target.Names.ToArray());

            // исходный набор не тронут, добавленные элементы перенумерованы со сдвигом
            Assert.AreEqual(2, target["extrude"].Count);
            Assert.AreEqual(4, target.Count);
        }

        [TestMethod]
        public void AppendIncrementsSuffixOnRepeatedCollisions()
        {
            //assign
            var target = CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1));

            //act
            var firstNames = target.Append(CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1)));
            var secondNames = target.Append(CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1)));

            //assert
            Assert.AreEqual("extrude_1", firstNames.Single());
            Assert.AreEqual("extrude_2", secondNames.Single());
            CollectionAssert.AreEquivalent(new[] { "extrude", "extrude_1", "extrude_2" }, target.Names.ToArray());
            Assert.AreEqual(6, target.Count);
        }

        [TestMethod]
        public void AppendReusesFreedSetName()
        {
            //assign
            var target = CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1));
            target.Append(CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1)));
            target.Remove("extrude");

            //act
            var addedNames = target.Append(CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1)));

            //assert
            Assert.AreEqual("extrude", addedNames.Single());
            CollectionAssert.AreEquivalent(new[] { "extrude", "extrude_1" }, target.Names.ToArray());
        }

        [TestMethod]
        public void AppendedSetNameMatchesKey()
        {
            //assign
            var target = CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1));
            var source = CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1));

            //act
            var addedName = target.Append(source).Single();

            //assert
            Assert.AreEqual(addedName, target[addedName].Name);
        }

        [TestMethod]
        public void AppendCopiesSetColor()
        {
            //assign
            var coloredSet = CreateElement2DSet("colored", 0);
            coloredSet.SetColor(Color.Red);

            var target = new ObjectsCollection<Element2D>();

            //act
            var addedName = target.Append(CreateElement2DCollection(coloredSet)).Single();

            //assert
            Assert.AreEqual(Color.Red.ToArgb(), target[addedName].Color.ToArgb());
        }

        [TestMethod]
        public void AppendEmptyCollectionReturnsNoNames()
        {
            //assign
            var target = CreateElement2DCollection(CreateElement2DSet("extrude", 0, 1));

            //act
            var addedNames = target.Append(new ObjectsCollection<Element2D>());

            //assert
            Assert.AreEqual(0, addedNames.Count);
            Assert.AreEqual(2, target.Count);
            CollectionAssert.AreEquivalent(new[] { "extrude" }, target.Names.ToArray());
        }

        [TestMethod]
        public void AppendEmptySetToFilledSet()
        {
            //assign
            var nodes = CreateTriangleNodes();

            var set1 = new ObjectsSet<Element2D>("e2D_1");
            set1.Add(0, new Triangle_c(0, nodes));
            set1.Add(1, new Triangle_c(1, nodes));

            var set2 = new ObjectsSet<Element2D>("e2D_2");

            //act
            set1.Append(set2);

            //assert
            Assert.AreEqual(2, set1.Count);
            Assert.AreEqual(0, set1[0].Number);
            Assert.AreEqual(1, set1[1].Number);
        }

        [TestMethod]
        public void AppendFilledSetToEmptySet()
        {
            //assign
            var nodes = CreateTriangleNodes();

            var set1 = new ObjectsSet<Element2D>("e2D_1");

            var set2 = new ObjectsSet<Element2D>("e2D_2");
            set2.Add(5, new Triangle_c(5, nodes));
            set2.Add(6, new Triangle_c(6, nodes));

            //act
            set1.Append(set2);

            //assert
            Assert.AreEqual(2, set1.Count);
            Assert.AreEqual(5, set1[5].Number);
            Assert.AreEqual(6, set1[6].Number);
        }

        [TestMethod]
        public void AppendEmptySetToEmptySet()
        {
            //assign
            var set1 = new ObjectsSet<Element2D>("e2D_1");
            var set2 = new ObjectsSet<Element2D>("e2D_2");

            //act
            set1.Append(set2);

            //assert
            Assert.AreEqual(0, set1.Count);
        }

        [TestMethod]
        public void AppendModelWithoutGeometryToModelWithGeometry()
        {
            //assign
            var model = new ModelData();

            var point = new GeometryPoint(0, new Geometry.Point3D(5, 5, 5));
            model.ObjectData.PointsSet.Add(point.Number, point);

            var modelNodes = CreateTriangleNodes();
            foreach (var node in modelNodes)
                model.ObjectData.NodesSet.Add(node.Number, node);

            var modelSet = new ObjectsSet<Element2D>("mesh");
            modelSet.Add(0, new Triangle_c(0, modelNodes));
            model.ObjectData.E2DCollection.Add(modelSet);

            // добавляемая модель содержит только сетку: точек, кривых и поверхностей в ней нет
            var appendedModel = new ModelData();

            var appendedNodes = CreateTriangleNodes();
            foreach (var node in appendedNodes)
                appendedModel.ObjectData.NodesSet.Add(node.Number, node);

            var appendedSet = new ObjectsSet<Element2D>("appended_mesh");
            appendedSet.Add(0, new Triangle_c(0, appendedNodes));
            appendedModel.ObjectData.E2DCollection.Add(appendedSet);

            //act
            model.Append(appendedModel);

            //assert
            Assert.AreEqual(6, model.ObjectData.NodesSet.Count);
            Assert.AreEqual(2, model.ObjectData.E2DCollection.Count);
            Assert.AreEqual(1, model.ObjectData.PointsSet.Count);
            Assert.AreEqual(0, model.ObjectData.CurveSet.Count);
            Assert.AreEqual(point.Number, model.ObjectData.PointsSet.First().Value.Number);
        }

        /// <summary>
        /// Создает узлы треугольного элемента
        /// </summary>
        private static Node[] CreateTriangleNodes() => new[]
        {
            new Node(0, new Geometry.Point3D(0, 0, 0)),
            new Node(1, new Geometry.Point3D(1, 0, 0)),
            new Node(2, new Geometry.Point3D(0, 1, 0))
        };

        [TestMethod]
        [DataRow(new int[] { 0, 10, 1, 2 }, new int[] { 0, 10, 12, 13 })]
        public void ChangeName(int[] iniNumb, int[] resNumb)
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D(0, 0, 0));
            var node1 = new Node(1, new Geometry.Point3D(1, 0, 0));
            var node2 = new Node(2, new Geometry.Point3D(0, 1, 0));

            model.ObjectData.NodesSet.Add(node0.Number, node0);
            model.ObjectData.NodesSet.Add(node1.Number, node0);
            model.ObjectData.NodesSet.Add(node2.Number, node0);


            var set1 = new ObjectsSet<Element2D>("e2D_1");
            set1.Add(iniNumb[0], new Triangle_c(iniNumb[0], new[] { node0, node1, node2 }));
            set1.Add(iniNumb[1], new Triangle_c(iniNumb[1], new[] { node0, node1, node2 }));

            var set2 = new ObjectsSet<Element2D>("e2D_2");
            set2.Add(iniNumb[2], new Triangle_c(iniNumb[2], new[] { node0, node1, node2 }));
            set2.Add(iniNumb[3], new Triangle_c(iniNumb[3], new[] { node0, node1, node2 }));

            var col = new ObjectsCollection<Element2D>();
            col.Add(set1);
            col.Add(set2);

            col.ChangeName("e2D_1", "newNameCol");

            //assert
            Assert.IsTrue(col.Any(x => x.Key == "newNameCol"));
            Assert.AreEqual(col.Where(x => x.Key == "newNameCol").Count(), 1);
        }

        [DataTestMethod]
        [DataRow(ObjType.Кривая)]
        [DataRow(ObjType.Точка)]
        public void RemoveGeometryByTypes(ObjType objType)
        {
            //assign
            var model = new ModelData();

            //act
            var point0 = new GeometryPoint(0, new Geometry.Point3D());
            var point1 = new GeometryPoint(1, new Geometry.Point3D());
            var point2 = new GeometryPoint(3, new Geometry.Point3D());

            var line = new Curve(2, new GeometryPoint[] { point0, point1 });

            model.ObjectData.PointsSet.Add(point0.Number, point0);
            model.ObjectData.PointsSet.Add(point1.Number, point1);
            model.ObjectData.CurveSet.Add(0, line);
            model.ObjectData.PointsSet.Add(point2.Number, point2);

            model.ObjectData.Clear(objType);
            //assert
            Assert.AreEqual(model.ObjectData.GetObjects(objType).Count(), 0);
        }

        [DataTestMethod]
        [DataRow(ObjType.Узел, 0, 0, 0)]
        [DataRow(ObjType.Элемент3D, 4, 1, 0)]
        [DataRow(ObjType.Элемент2D, 4, 0, 6)]
        public void RemoveObjectsSyncWithGroups(ObjType objType, int nNumbs, int e2Numbs, int e3Numbs)
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
            };
            var objects2d = new Element2D[]
{
                new Quad( 0,nodes)
};
            var objects3d = new Element3D[]
            {
                new Tetra( 0,nodes),
                new Tetra( 4,nodes),
                new Tetra(5,nodes),
                new Tetra( 6,nodes),
                new Tetra(7,nodes),
                new Tetra(8,nodes)
            };

            var group = new Group("test", objType);
            if (objType == ObjType.Узел)
                group.AddRange(nodes);
            else if (objType == ObjType.Элемент2D)
                group.AddRange(objects2d);
            else if (objType == ObjType.Элемент3D)
                group.AddRange(objects3d);
            //act

            var model = new ModelData();

            foreach (var item in nodes)
                model.ObjectData.NodesSet.Add(item.Number, item);

            model.ObjectData.E2DCollection.AddRange(ObjType.Элемент2D.ToString(), objects2d);
            model.ObjectData.E3DCollection.AddRange(ObjType.Элемент3D.ToString(), objects3d);

            model.GroupData.Add(group);

            model.ObjectData.Clear(objType);
            model.GroupData.ClearNotExisted();
            //assert            
            Assert.AreEqual(model.ObjectData.NodesSet.Count, nNumbs);
            Assert.AreEqual(model.ObjectData.E2DCollection.Count, e2Numbs);
            Assert.AreEqual(model.ObjectData.E3DCollection.Count, e3Numbs);
            Assert.AreEqual(model.GroupData.FindMany(objType).Count(), 0);

        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(0)]
        [DataRow(2)]
        public void RemoveNodesByNumber(int objNumber)
        {
            //assing
            //var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            var model = loader.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");

            //act
            //model.ObjectData.NodesSet.Remove(objNumber);
            //model.ObjectData.E3DCollection.RemoveNotExistedObjects();
            model.ObjectData.Remove(ObjType.Узел, objNumber);

            //assert
            var res = model.ObjectData.Find(ObjType.Узел, objNumber);
            if (res != null)
                throw new Exception();
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(2)]
        public void RemoveElementsByNumber(int objNumber)
        {
            //assing
            var model = new ModelData();
            model.Loader = new LoadModelFromASCIITextFile();

            model.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");

            //act
            model.ObjectData.E3DCollection.Remove(objNumber);
            model.ObjectData.E3DCollection.RemoveNotExistedObjects();

            //assert
            var res = model.ObjectData.Find(ObjType.Элемент3D, objNumber);
            if (res != null)
                throw new Exception();
        }

        [DataTestMethod]
        [DataRow(ObjType.Узел)]
        [DataRow(ObjType.Элемент3D)]
        public void RemoveMeshByNumber(ObjType objType)
        {
            //assing
            var model = new ModelData();
            model.Loader = new LoadModelFromASCIITextFile();

            model.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");

            //act
            model.ObjectData.Clear(objType);
            model.ObjectData.E3DCollection.RemoveNotExistedObjects();
            model.ObjectData.E3DCollection.ClearEmptySets();

            //assert
            var res = model.ObjectData.Count(objType);
            if (res != 0)
                throw new Exception();
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\STL\ACII.stl", ObjType.Узел)]
        [DataRow(@"..\..\..\Examples\STL\sphere.stl", ObjType.Узел)]
        public void MoveObjects(string filePath, ObjType objType)
        {
            //assign
            var modelData = new ModelData();
            modelData.Loader = new LoadFromSTLFile();
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
            modelData.Loader = new LoadFromSTLFile();
            modelData.Load(filePath);

            var oldCoords = new List<Point3D>();

            foreach (var item in modelData.ObjectData.GetObjects(objType))
                oldCoords.Add(item.CalcCentr());

            //act
            modelData.ObjectData.Rotate(objType, new Point3D(1, 1, 1), 25.0f);

            //assert
            var newCoords = modelData.ObjectData.GetObjects(objType).
                Select(x => x.CalcCentr()).ToList();

            for (int i = 0; i < newCoords.Count(); i++)
            {
                Assert.AreNotEqual(oldCoords[i], newCoords[i]);
            }
        }

        [DataTestMethod]
        [DataRow(new float[] { 0, 0, 1 }, new float[] { 0, 1, 0 }, new float[] { 0, 1, 1 })]
        [DataRow(new float[] { 0, 0, 1 }, new float[] { 1, 0, 0 }, new float[] { 1, 0, 1 })]
        [DataRow(new float[] { 0, 1, 0 }, new float[] { 0, 0, -1 }, new float[] { 0, 1, -1 })]
        [DataRow(new float[] { 0, 1, 0 }, new float[] { -1, 0, 0 }, new float[] { -1, 1, 0 })]
        public void MoveSimpleTest(float[] srcPoint, float[] translate, float[] expected)
        {
            var modelData = new ModelData();

            var point = new Node(1, new Point3D(srcPoint[0], srcPoint[1], srcPoint[2]));
            modelData.ObjectData.NodesSet.Add(1, point);

            var translateVec = new Point3D(translate[0], translate[1], translate[2]);
            modelData.ObjectData.Move(ObjType.Узел, translateVec);

            var actual = modelData.ObjectData.NodesSet.Select(v => v.Value).ToArray();

            Assert.AreEqual(expected[0], actual[0].Position._x, 1e-4);
            Assert.AreEqual(expected[1], actual[0].Position._y, 1e-4);
            Assert.AreEqual(expected[2], actual[0].Position._z, 1e-4);
        }

        [DataTestMethod]
        [DataRow(new float[] { 0, 0, 1 }, new float[] { 0, 1, 0 }, 90.0f, new float[] { 1, 0, 0 })]
        [DataRow(new float[] { 0, 0, 1 }, new float[] { 1, 0, 0 }, -90.0f, new float[] { 0, 1, 0 })]
        [DataRow(new float[] { 0, 0, 1 }, new float[] { 0, 0, 1 }, 90.0f, new float[] { 0, 0, 1 })]
        [DataRow(new float[] { 0, 1, 0 }, new float[] { 0, 0, 1 }, -90.0f, new float[] { 1, 0, 0 })]
        public void RotateSimpleTest(float[] srcPoint, float[] axis, float angle, float[] expected)
        {
            var modelData = new ModelData();

            var point = new Node(1, new Point3D(srcPoint[0], srcPoint[1], srcPoint[2]));
            modelData.ObjectData.NodesSet.Add(1, point);

            var axisVec = new Point3D(axis[0], axis[1], axis[2]);
            modelData.ObjectData.Rotate(ObjType.Узел, axisVec, angle);

            var actual = modelData.ObjectData.NodesSet.Select(v => v.Value).ToArray();

            Assert.AreEqual(expected[0], actual[0].Position._x, 1e-4);
            Assert.AreEqual(expected[1], actual[0].Position._y, 1e-4);
            Assert.AreEqual(expected[2], actual[0].Position._z, 1e-4);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\STL\block.stl", new float[] { -5, 10, 5 })]
        public void MoveModelTest(string filePath, float[] translate)
        {
            //var modelData = new ModelData();
            var loader = new LoadFromSTLFile();
            var modelData = loader.Load(filePath);

            //var keepOld = new ModelData();
            loader = new LoadFromSTLFile();
            var keepOld = loader.Load(filePath);

            var translateVec = new Point3D(translate[0], translate[1], translate[2]);
            modelData.ObjectData.Move(ObjType.Узел, translateVec);

            var oldNodes = keepOld.ObjectData.NodesSet.Select(v => v.Value).ToArray();
            var newNodes = modelData.ObjectData.NodesSet.Select(v => v.Value).ToArray();

            var srcPoint = newNodes[0].Position.Sub(oldNodes[0].Position);
            var srcDelta = Vector.GetVectorLength(srcPoint);

            for (var i = 1; i < newNodes.Length; ++i)
            {
                var curPoint = newNodes[i].Position.Sub(oldNodes[i].Position);
                var curDelta = Vector.GetVectorLength(curPoint);
                Assert.AreEqual(srcDelta, curDelta, 1e-4);
            }
        }

        public static IEnumerable<object[]> AdditionData
        {
            get
            {
                return new[]
                {
            new object[] { ObjType.Элемент3D, Color.DarkGray },
            new object[] { ObjType.Узел, Color.Blue }
        };
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(AdditionData))]
        public void SetBackColor(ObjType reqType, Color color)
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)){ Color = Color.Red},
                new Node(1,new Point3D(10,0,0)){ Color = Color.Red},
                new Node(2,new Point3D(0,10,0)){ Color = Color.Red},
                new Node(3,new Point3D(0,0,10)){ Color = Color.Red}
            };
            var objs = new Element3D[]
            {
                new Tetra( 0,nodes){ Color = Color.Red},
                new Tetra( 4,nodes){ Color = Color.Red},
                new Tetra(5,nodes){ Color = Color.Red},
                new Tetra( 6,nodes){ Color = Color.Red},
                new Tetra(7,nodes){ Color = Color.Red},
                new Tetra(8,nodes){ Color = Color.Red}
            };

            //act

            var model = new ModelData();
            foreach (var item in nodes)
                model.ObjectData.NodesSet.Add(item.Number, item);
            model.ObjectData.E3DCollection.AddRange(ObjType.Элемент3D.ToString(), objs);

            model.ObjectData.SetBackColor(reqType);

            //assert
            foreach (var item in model.ObjectData.GetObjects(reqType))
            {
                Assert.AreEqual(item.Color.R, color.R);
                Assert.AreEqual(item.Color.G, color.G);
                Assert.AreEqual(item.Color.B, color.B);
            }
                
        }
    }
}
