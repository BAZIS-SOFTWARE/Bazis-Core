using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Events;
using Model.Interfaces;
using ModelController.MeshObjsUtility;
using Model.IO;

namespace TestModel
{
    [TestClass]
    public class TestServices
    {
        private ObjectsData objectsData;

        [TestInitialize]
        public void Setup()
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
                new Element2D(9, new Node[] { A, B, C }, ObjKind.Triangle),
                new Element2D(10, new Node[] { B, C, D }, ObjKind.Triangle),
                new Element2D(11, new Node[] { D, C, E }, ObjKind.Triangle),
                new Element2D(12, new Node[] { E, D, G }, ObjKind.Triangle),
                new Element2D(13, new Node[] { E, F, G }, ObjKind.Triangle),
                new Element2D(14, new Node[] { F, G, H }, ObjKind.Triangle),
            };
            var nodes = new List<Node> { A, B, C, D, E, F, G, H };
            objectsData = new ObjectsData();
            objectsData.AddRange(nodes);
            objectsData.AddRange(elements);
        }

        [TestMethod]
        public void FindFreeNodes()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromProjectTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            //act
            var modelINP = loader.Load(@"C:\Projs\TestProjTerm\Cub\cub_unsorted_freeNodes.bpf");
            model.Load(modelINP);

            var freeNodesFinder = new FreeNodesFinder(model.ObjectData);

            var freeNodes = freeNodesFinder.Find<Element>();

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
            var selector = new SelectionHelper(objectsData);
            var actual = selector.SelectInDirection<Element2D>(angle, first, second, Color.Green);
            Assert.IsTrue(actual.SequenceEqual(expected)); 
        }

        [TestMethod]
        public void SelectCorrectElements1()
        {
            var selector = new SelectionHelper(objectsData);
            var selectedElementNumbers = selector.SelectInPlane<Element2D>(19, 9, Color.Green);
            var expected = new List<int>() { 9, 10, 11, 12, 13, 14 };
            Assert.IsTrue(selectedElementNumbers.SequenceEqual(expected));
        }

        [TestMethod]
        public void SelectCorrectElements2()
        {
            var selector = new SelectionHelper(objectsData);
            var selectedElementNumbers = selector.SelectInPlane<Element2D>(18, 9, Color.Green);
            var expected = new List<int>() { 9, 10, 11, 12, 13 };
            Assert.IsTrue(selectedElementNumbers.SequenceEqual(expected));
        }

        [TestMethod]
        public void TestBoundEdge()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += Model_LoadEvent;
            var modelLoaded = loader.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");
            model.Load(modelLoaded);
            //act
            var elements = model.ObjectData.FindMany<Element3D>().ToArray();
            var nodes = model.ObjectData.FindMany<Node>();

            var interNodesFinder = new InterfacedNodesFinder();
            var edgeFinder = new FindBoundaryEdges(model);

            var interNodes = interNodesFinder.Find(elements);

            var edges = edgeFinder.Find();
            //assert
        }     

        [TestMethod]
        public void FindElementNodes()
        {
            //assing
            var model = new ModelData();
            var loaderGMSH = new LoadModelFromGMSHTextFile();


            loaderGMSH.LoadEvent += Model_LoadEvent;
            //act
            var modelGMSH = loaderGMSH.Load(@"C:\Projs\TestProjTerm\box\box1.inp");
            model.Load(modelGMSH);

            var saver = new SaveModelToProjectTextFile();
            saver.SaveEvent += Model_SaveEvent;
            saver.Save(model,@"C:\Projs\TestProjTerm\box\task_1.bpf");

            var loaderBPF = new LoadModelFromProjectTextFile();
            loaderBPF.LoadEvent += Model_LoadEvent;
            var modelBPF = loaderBPF.Load(@"C:\Projs\TestProjTerm\box\task_1.bpf");
            model.Load(modelBPF);

            var els3D = model.ObjectData.FindMany<Element3D>();
            var els2D = model.ObjectData.FindMany<Element2D>();

            var interNodesEls3DFinder = new InterfacedNodesFinder();
            var interNodesEls2DFinder = new InterfacedNodesFinder();

            var interfacedNodesEls3D = interNodesEls3DFinder.Find(new List<int>() { 3, 48, 71 }, els3D);

            var elsForNode = new List<int>() { 2565,3048,3050,3051 };
            //assert
            for (int i = 0; i < interfacedNodesEls3D[48].Count; i++)
            {
                if (!elsForNode.Contains(interfacedNodesEls3D[48][i]))
                    throw new Exception("номера не совпадают");
            }
        }

        private void Model_SaveEvent(object arg1, SaverEventArgs arg2)
        {
            //throw new NotImplementedException();
        }


        private void Model_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void TestRemoveBigData()
        {
            //assing
            var model = new ModelData();
            var loaderGMSH = new LoadModelFromProjectTextFile();
            loaderGMSH.LoadEvent += Model_LoadEvent;
            //act
            var modelGMSH = loaderGMSH.Load(@"C:\Projs\ENERGOMASH\CALC\proj_new.bpf");
            model.Load(modelGMSH);
            model.ObjectData.RemoveRange("Элементы2D");
            model.ObjectData.ClearRemoved();

            var els2D = model.ObjectData.FindMany<Element2D>();

            if (els2D.Count() != 0)
                throw new Exception();
        }

        [TestMethod]
        public void MergeNodes()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromProjectTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            //act
            //var modelBPF = loader.Load(@"C:\Projs\ExampleMesh\Tube.inp");
            var modelBPF = loader.Load(@"C:\Projs\ExampleMesh\Tube_3d_only.bpf");
            model.Load(modelBPF);

            var finder = new FindCoincidentObjects(model.ObjectData, 0.01f);

            finder.ProgressEvent += (ar1, ar2) => { };

            var coincidentNodes = finder.Find<Node>();

            var merge = new MergeObjects(model.ObjectData);


            //assert
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
            var coincidentNodes = finder.Find<Node>();
            var numberOfObjs = coincidentNodes.Count();
            Assert.AreEqual(4, numberOfObjs);
        }

        [TestMethod, Timeout(3000)]
        public void FindManyCoincidentObjects1() 
        {
            var loader = new LoadModelFromCDBTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            var model = loader.Load(@"..\..\..\Examples\CDB\10e+5Nodes.cdb");
            var finder = new FindCoincidentObjects(model.ObjectData, 1.0f);
            var coincidentNodes = finder.Find<Node>().ToList();
        }

        //[TestMethod]
        //public void FindManyCoincidentObjects2()
        //{
        //    var loader = new LoadModelFromGMSHTextFile();
        //    var stopWatch = new Stopwatch();
        //    loader.LoadEvent += (ar1, ar2) => { };
        //    var model = loader.Load(@"..\..\Examples\3D\zub.inp");
        //    var finder = new FindCoincidentObjects(model.ObjectData, 10f);
        //    stopWatch.Start();
        //    var coincidentNodes = finder.Find<Node>().ToList();
        //    stopWatch.Stop();
        //    Assert.IsTrue(stopWatch.ElapsedMilliseconds <= 2000);
        //}

        [TestMethod]
        public void TestCalcFindOpenSurfaceNodes()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += Model_LoadEvent;
            var modelASCII = loader.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");
            model.Load(modelASCII);

            //act
            var elements = model.ObjectData.FindMany<Element3D>();

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
