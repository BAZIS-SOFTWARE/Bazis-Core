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
using System.Drawing;
using Model.ObjectsCollections;
using Model.Interfaces.ObjectsCollections;
using Model.Interfaces.MeshObjects;
using Model.IO.STL;

namespace TestModel
{
    [TestClass]
    public class TestObjectData
    {
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
        [DataRow(0,0,0,1)]
        [DataRow(0, 10, 0, 10)]
        [DataRow(1, 0, 1, 2)]
        public void AppendSets(int ini_fNumber,int ini_sNumber, int res_fNumber, int res_sNumber)
        {
            //assign
            var model = new ModelData();

            //act
            var node0 = new Node(0, new Geometry.Point3D(0,0,0));
            var node1 = new Node(1, new Geometry.Point3D(1,0,0));
            var node2 = new Node(2, new Geometry.Point3D(0,1,0));

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
        [DataRow(new int[] {0,1,2,3}, new int[] { 0, 1, 2, 3 })]
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

            col1.Append(col2);

            //assert
            Assert.AreEqual(col1.Count, 4);

            Assert.AreEqual(col1.GetMaxNumber(), resNumb.Last());
            Assert.AreEqual(col1.GetMinNumber(), resNumb.First());
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
            model.ObjectData.CurveCollection.Add(ObjType.Кривая.ToString(), line);
            model.ObjectData.PointsSet.Add(point2.Number, point2);

            model.ObjectData.Clear(objType);
            //assert
            Assert.AreEqual(model.ObjectData.GetObjects(objType).Count(), 0);
        }

        [DataTestMethod]
        [DataRow(ObjType.Узел, 0, 0,0)]
        [DataRow(ObjType.Элемент3D, 4, 1,0)]
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
            if(objType == ObjType.Узел)
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
            var model = new ModelData();
            model.Loader = new LoadModelFromASCIITextFile();

            model.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");

            //act
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
            model.ObjectData.E3DCollection.ClearNotExisted();

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
            model.ObjectData.E3DCollection.ClearNotExisted();
            model.ObjectData.E3DCollection.ClearEmpty();

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
            modelData.Loader = new LoadMeshFromSTLFile();
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
            modelData.Loader = new LoadMeshFromSTLFile();
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
