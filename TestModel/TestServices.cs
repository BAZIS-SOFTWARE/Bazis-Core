using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController;
using ModelController.IO;
using ModelController.MeshObjsUtility;
using ModelInterface;

namespace TestModel
{
    [TestClass]
    public class TestServices
    {
        [TestMethod]
        public void TestHideAllElementArrays()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();
            model.SetLoader(loader);
            model.LoadEvent += Model_LoadEvent;
            model.Load(@"C:\Bazis_newGUI\MESH\rhombDATA1.ASC");

            //act

            var elements = model.ObjectData.FindMany<Element3D>().ToArray();

            model.ObjectData.HideModelObjs(elements);

            var presenter = new ModelScenePresentator(model.ObjectData.ToArray());

            //var creator = presenter.CreateSurfaceObjsCreator("Элементы3D", true);

            //var indexes = creator.CreateIndexes();

            //var ptrs = creator.CreatePointers(indexes.Item1);
            //var coords = creator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = creator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = creator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());
            //assert
        }

        [TestMethod]
        public void TestBoundEdge()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();
            model.SetLoader(loader);
            model.LoadEvent += Model_LoadEvent;
            model.Load(@"C:\Bazis_newGUI\MESH\rhombDATA1.ASC");

            //act
            var elements = model.ObjectData.FindMany<Element3D>().ToArray();
            var nodes = model.ObjectData.FindMany<Node>();

            var interNodesFinder = new FindInterfacedNodes(elements);
            var edgeFinder = new FindBoundaryEdges(model);

            var interNodes = interNodesFinder.Find();

            var edges = edgeFinder.Find(interNodes);
            //assert
        }

        [TestMethod]
        public void FindFreeNodes()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromGMSH_InpFile();
            model.SetLoader(loader);

            model.LoadEvent += Model_LoadEvent;
            //act
            model.Load(@"C:\Projs\TestProjTerm\box\box1.inp");

            var nodes = model.ObjectData.FindMany<Node>().ToArray();
            var freeNodesFinder = new FreeNodesFinder(nodes);

            var els3D = model.ObjectData.FindMany<Element3D>().ToArray();
            var interNodesFinder = new FindInterfacedNodes(els3D);

            var freeNodes = freeNodesFinder.Find(interNodesFinder.Find());

            //assert
            Assert.AreEqual(freeNodes.Count, 0);
        }

        [TestMethod]
        public void FindElementNodes()
        {
            //assing
            var model = new ModelData();
            var loaderGMSH = new LoadModelFromGMSH_InpFile();
            model.SetLoader(loaderGMSH);

            model.LoadEvent += Model_LoadEvent;
            //act
            model.Load(@"C:\Projs\TestProjTerm\box\box1.inp");

            var saver = new SaveModelToProjectTextFile();
            model.SetSaver(saver);
            model.SaveEvent += Model_SaveEvent;

            model.Save(@"C:\Projs\TestProjTerm\box\task_1.bpf");

            var loaderBPF = new LoadModelFromProjectTextFile();
            model.SetLoader(loaderBPF);

            model.Load(@"C:\Projs\TestProjTerm\box\task_1.bpf");

            var els3D = model.ObjectData.FindMany<Element3D>();
            var els2D = model.ObjectData.FindMany<Element2D>();

            var interNodesEls3DFinder = new FindInterfacedNodes(els3D.ToArray());
            var interNodesEls2DFinder = new FindInterfacedNodes(els2D.ToArray());

            var interfacedNodesEls3D = interNodesEls3DFinder.Find(new List<int>() { 3, 48, 71 });

            var elsForNode = new List<int>() { 1573,2056,2058,2059 };
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
            model.SetLoader(loaderGMSH);

            model.LoadEvent += Model_LoadEvent;
            //act
            model.Load(@"C:\Projs\ENERGOMASH\CALC\proj_new.bpf");

            model.ObjectData.RemoveRange("Элементы2D");

            var els2D = model.ObjectData.FindMany<Element2D>();

            if (els2D.Count() != 0)
                throw new Exception();
        }

        [TestMethod]
        public void TestCalcNodeNormals()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();
            model.SetLoader(loader);
            model.LoadEvent += Model_LoadEvent;
            model.Load(@"C:\Bazis_newGUI\MESH\rhombDATA1.ASC");


            //act
            var elements = model.ObjectData.FindMany<Element3D>().ToArray();

            var normalCalculator = new NormalsCalculator(elements);
            var nodeNormals = normalCalculator.CalcNodeNormals();
            
            //var edge = Services.GetBoundedEdges(elements);
            //assert
            var openSurfNodes = new List<int>();
            foreach (var normal in nodeNormals)
                if (Vector.GetVectorLenght(normal.Value) > 1e-4)
                    openSurfNodes.Add(normal.Key);

            if (openSurfNodes.Contains(0))
                throw new Exception("Последовательность содержит неверный узел!");
        }
    }

}
