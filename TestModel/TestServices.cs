using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController;
using ModelController.IO;
using ModelController.MeshObjsUtility;

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

            var presenter = new ModelScenePresentator(model);

            //var creator = presenter.CreateSurfaceObjsCreator("Элементы3D", true);

            //var indexes = creator.CreateIndexes();

            //var ptrs = creator.CreatePointers(indexes.Item1);
            //var coords = creator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = creator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = creator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());
            //assert
        }

        [TestMethod]
        public void FindNodesInDirection()
        {
            //assign
            var loader = new LoadModelFromProjectTextFile();
            loader.LoadEvent += (ar1, ar2) => { };

            //act
            var model = loader.Load(@"C:\Projs\TestProjTerm\tjoint\proj_traj.bpf");

            //baseLine | refLine
            var strGroup = "baseLine";
            var group = model.GroupData.Find(strGroup);

            var selector = new SelectionHelper(model.ObjectData);

            var firstN = model.ObjectData.Find(group[0]);
            var secondN = model.ObjectData.Find(group[1]);

            var selNodes = selector.SelectInDirecion<Element3D>(10, firstN.Number, secondN.Number, Color.Green);

            //assert
            var actNodes = group.ObjsNumbers.ToList();
            for (int i = 0; i < selNodes.Count; i++)
                Assert.AreEqual(selNodes[i], actNodes[i]);

        }

        [TestMethod]
        public void FindElementsInPlane()
        {
            //assign
            var loader = new LoadModelFromGMSHTextFile();
            loader.LoadEvent += (ar1, ar2) => { };

            //act
            var model = loader.Load(@"C:\Projs\ExampleMesh\Tube.inp");

            var selector = new SelectionHelper(model.ObjectData);

            var el2D = model.ObjectData.FindMany<Element2D>();
            var numb = el2D.Last().Number;
            selector.SelectInPlane<Element2D>(10, numb, Color.Green);

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

            merge.Merge<Node>(coincidentNodes);

            //assert
            //freeNodes.ToArray()[0] = 14;
            //freeNodes.ToArray()[1] = 15;
        }

        [TestMethod]
        public void FindCoincidentNodes()
        {
            var model = new ModelData();
            var loader = new LoadModelFromProjectTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            //act
            //var modelBPF = loader.Load(@"C:\Projs\TestProjTerm\Cub\cub_unsorted_freeNodesCoincident.bpf");
            var modelBPF = loader.Load(@"C:\Projs\TestProjTerm\CoincidentMesh\Tube_3d_only.bpf");
        var finder = new FindCoincidentObjects(modelBPF.ObjectData, 0.01f);
            finder.ProgressEvent += (ar1, ar2) => { };

            var coincidentNodes = finder.Find<Node>();

            var numberOfObjs = coincidentNodes.Count();

            Assert.AreEqual(numberOfObjs, 15121);
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
            normalCalculator.CalcNodeNormals();
            
            //var edge = Services.GetBoundedEdges(elements);
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
