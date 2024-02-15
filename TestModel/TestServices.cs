using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Events;
using ModelInterfaces;
using ModelController.MeshObjsUtility;
using Model.IO;
using Model.MeshObjects;
using GmshApi.GmshController;
using ModelInterfaces.MeshObjects;

namespace TestModel
{
    [TestClass]
    public class TestServices
    {
        private ObjectsData objectsData;

        private void LoadModel()
        {
            var A = new Node(1, new Point3D(0, 0, 0));
            var B = new Node(2, new Point3D(0, 0, 1));
            var C = new Node(3, new Point3D(0, 1, 0));
            var D = new Node(4, new Point3D(0, 1, 1));
            var E = new Node(5, new Point3D(0, 1.5f, 0));
            var F = new Node(6, new Point3D(0, 2, 0));
            var G = new Node(7, new Point3D(0, 2, 1));
            var H = new Node(8, new Point3D(-0.5f, 3.5f, 0));
            var elements = new List<Element2D>
            {
                new Triangle(9, new Node[] { A, B, C }),
                new Triangle(10, new Node[] { B, C, D }),
                new Triangle(11, new Node[] { D, C, E }),
                new Triangle(12, new Node[] { E, D, G }),
                new Triangle(13, new Node[] { E, F, G }),
                new Triangle(14, new Node[] { F, G, H }),
            };
            var nodes = new List<Node> { A, B, C, D, E, F, G, H };
            objectsData = new ObjectsData();
            objectsData.NodeCollection.AddRange(nodes);
            objectsData.E2DCollection.AddRange(elements);
        }

        [TestMethod]
        public void FindFreeNodes()
        {
            //assing
            var model = new ModelData();

            //act
            model.Load(@"C:\Projs\TestProjTerm\Cub\cub_unsorted_freeNodes.bpf");

            var freeNodesFinder = new FreeNodesFinder(model.ObjectData);
            var freeNodes = freeNodesFinder.Find();

            //assert
            freeNodes.ToArray()[0] = 14;
            freeNodes.ToArray()[1] = 15;
        }

        [TestMethod]
        [DataRow(19, 8, 6, new int[] { 8, 6, 5, 3, 1})]
        [DataRow(18, 8, 6, new int[] { 8, 6 })]
        [DataRow(18, 1, 3, new int[] { 1, 3, 5, 6 })]
        [DataRow(19, 1, 3, new int[] { 1, 3, 5, 6, 8 })]
        public void FindNodesInDirection(float angle, int first, int second, int[] expected)
        {
            LoadModel();
            var selector = new SelectionHelper(objectsData);
            var actual = selector.SelectNodeInDirection(angle, first, second, Color.Green);
            Assert.IsTrue(actual.SequenceEqual(expected)); 
        }

        [DataTestMethod]
        [DataRow(5, 272570, @"..\..\..\Examples\3D\comp_sm.bpf")]
        public void SelectCorrectElementsUseModel(float angle, int first, string path)
        {
            var model = new ModelData();
            model.Load(path);

            var selector = new SelectionHelper(model.ObjectData);
            var selectedElementNumbers = selector.SelectE2DInPlane(angle, first, Color.Green);
            Assert.IsTrue(selectedElementNumbers.Count > 0);
        }

        [DataTestMethod]
        [DataRow(19, 9, new int[] { 9, 10, 11, 12, 13, 14 })]
        [DataRow(18, 9, new int[] { 9, 10, 11, 12, 13 })]
        public void SelectCorrectElements(float angle, int first, int[] expected)
        {
            LoadModel();
            var selector = new SelectionHelper(objectsData);
            var selectedElementNumbers = selector.SelectE2DInPlane(angle, first, Color.Green);
            Assert.IsTrue(selectedElementNumbers.SequenceEqual(expected));
        }

        [TestMethod]
        public void FindBoundaryEdges_ShouldWork_OnModelWithDifferentElements()
        {
            var model = new ModelData();
            model.Loader = new LoadModelFromProjectTextFile();
            model.Load(@"..\..\..\Examples\3D\cub_unsorted.bpf");
            var instance = new FindBoundaryEdges(model);
            var bounds = instance.Find();
            Assert.AreEqual(19, bounds.Count);
        }     

        [TestMethod]
        public void FindElementNodes()
        {
            //assing
            var model = new ModelData();

            //act
            model.Load(@"..\..\..\Examples\3D\task_1.bpf");

            var els3D = model.ObjectData.E3DCollection;

            var interNodesEls3DFinder = new InterfacedNodesFinder();

            var interfacedNodesEls3D = interNodesEls3DFinder.Find(new List<int>() { 3, 48, 71 }, els3D);

            var elsForNode = new List<int>() { 1565,2048,2050, 2051 };
            //assert
            for (int i = 0; i < interfacedNodesEls3D[48].Count; i++)
            {
                if (!elsForNode.Contains(interfacedNodesEls3D[48][i]))
                    throw new Exception("номера не совпадают");
            }
        }   

        [TestMethod]
        public void MergeNodes()
        {
            //assing
            var model = new ModelData();

            //act
            //var modelBPF = loader.Load(@"C:\Projs\ExampleMesh\Tube.inp");
            model.Load(@"C:\Projs\ExampleMesh\Tube_3d_only.bpf");

            var finder = new FindCoincidentObjects(model.ObjectData, 0.01f);

            finder.ProgressEvent += (ar1, ar2) => { };

            var coincidentNodes = finder.Find(ObjType.Узел);

            var merger = new MergeObjects(model.ObjectData);

            merger.Merge(coincidentNodes, ObjType.Узел);

            //assert
            var nodes = model.ObjectData.GetObjects(ObjType.Узел).ToList();
            var numb = nodes.First().Number;

            foreach (var item in nodes.Skip(1))
            {
                if (item.Number <= numb)
                    throw new Exception($"{item.Number}");
                numb = item.Number;
            }
            
            //freeNodes.ToArray()[0] = 14;
            //freeNodes.ToArray()[1] = 15;
        }

        [TestMethod]
        public void FindCoincidentNodes()
        {
            var loader = new LoadModelFromCDBTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            var model = loader.Load(@"..\..\..\Examples\CDB\hex.cdb");
            var finder = new FindCoincidentObjects(model.ObjectData, 1.1f);
            finder.ProgressEvent += (ar1, ar2) => { };
            var coincidentNodes = finder.Find(ObjType.Узел);
            var numberOfObjs = coincidentNodes.Count();
            Assert.AreEqual(4, numberOfObjs);
        }

        [TestMethod, Timeout(5000)]
        public void FindManyCoincidentObjects1() 
        {
            var loader = new LoadModelFromCDBTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            var model = loader.Load(@"..\..\..\Examples\CDB\10e+5Nodes.cdb");
            var finder = new FindCoincidentObjects(model.ObjectData, 1.0f);
            var coincidentNodes = finder.Find(ObjType.Узел);
        }

        [TestMethod]
        public void TestCalcFindOpenSurfaceNodes()
        {
            //assing
            var model = new ModelData();
            model.Loader = new LoadModelFromASCIITextFile();

            model.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");

            //act
            var elements = model.ObjectData.E3DCollection;

            //assert
            var openSurfNodes = new List<int>();

            var finder = new FindOpenSurfaceNodes();
            var surfaceNodes = finder.Find(elements);
            var distinctNumbers = surfaceNodes.Distinct();
            if (distinctNumbers.Contains(0))
                throw new Exception("Последовательность содержит неверный узел!");
        }
    }

}
