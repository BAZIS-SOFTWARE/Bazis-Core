using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using GMSH_Importer;
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

            loader.LoadEvent += Model_LoadEvent;
            var modeLoaded = loader.Load(@"C:\Bazis_newGUI\MESH\rhombDATA1.ASC");
            model.Load(modeLoaded);
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

            loader.LoadEvent += Model_LoadEvent;
            var modelLoaded = loader.Load(@"C:\Bazis_newGUI\MESH\rhombDATA1.ASC");
            model.Load(modelLoaded);
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
            var loader = new Import_Mesh();

            loader.LoadEvent += Model_LoadEvent;
            //act
            var modelLoaded = loader.Load(@"C:\Projs\TestProjTerm\box\box1.inp");
            model.Load(modelLoaded);
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
            var loaderGMSH = new Import_Mesh();


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
            loaderGMSH.LoadEvent += Model_LoadEvent;
            //act
            var modelGMSH = loaderGMSH.Load(@"C:\Projs\ENERGOMASH\CALC\proj_new.bpf");
            model.Load(modelGMSH);
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

            loader.LoadEvent += Model_LoadEvent;
            var modelASCII = loader.Load(@"C:\Bazis_newGUI\MESH\rhombDATA1.ASC");
            model.Load(modelASCII);

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
