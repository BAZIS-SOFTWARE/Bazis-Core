using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.IO;
using Model.MeshObjects;
using Model.Interfaces.MeshObjects;
using System.Data;
using OperationalController.MeshObjsUtility;
using OperationalController.ModelScenePresentator;
using OperationalController;
using System.Globalization;
using System.Threading;

namespace TestModel
{
    [TestClass]
    public class TestServices
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
        [TestMethod]
        public void FindFreeNodes()
        {
            //assing
            var model = new ModelData();

            //act
            model.Loader = new LoadModelFromBPFTextFile();
            model.Load(@"..\..\..\Examples\3D\cub_unsorted_freeNodes.bpf");

            var freeNodesFinder = new FreeNodesFinder();
            var freeNodes = freeNodesFinder.Find(model.ObjectData);

            //assert
            freeNodes.ToArray()[0] = 14;
            freeNodes.ToArray()[1] = 15;
        }

        [TestMethod]
        [DataRow(19, 8, 6, new int[] { 8, 6, 5, 3, 1})]
        [DataRow(18, 8, 6, new int[] { 8, 6 })]
        [DataRow(18, 1, 3, new int[] { 1, 3, 5, 6 })]
        [DataRow(19, 1, 3, new int[] { 1, 3, 5, 6, 8 })]
        public void SelectionHelper_ShouldSelectCorrectNodes_WhenAngleGiven(float angle, int first, int second, int[] expected)
        {
            var objectsData = InitializeSampleModelWithAscendingNumbers();
            var selector = new SelectionHelper();
            var actual = selector.SelectNodeInDirection(objectsData,angle, first, second, Color.Green);
            Assert.IsTrue(actual.SequenceEqual(expected)); 
        }

        [DataTestMethod]
        [DataRow(19, 0, new int[] { 0, 6, 10, 11, 100, 121 })]
        [DataRow(18, 0, new int[] { 0, 6, 10, 11, 100 })]
        public void SelectionHelper_ShouldSelectCorrect2DElements_MixingNumbers_WhenAngleGiven(float angle, int first, int[] expected)
        {
            var objectsData = InitializeSampleModelWithMixingNumbers();

            //objectsData.E2DCollection.

            var selector = new SelectionHelper();
            var selectedElementNumbers = selector.SelectE2DInPlane(objectsData, angle, first, Color.Green);
            Assert.IsTrue(selectedElementNumbers.SequenceEqual(expected));
        }

        [DataTestMethod]
        [DataRow(19, 9, new int[] { 9, 10, 11, 12, 13, 14 })]
        [DataRow(18, 9, new int[] { 9, 10, 11, 12, 13 })]
        public void SelectionHelper_ShouldSelectCorrect2DElements_WhenAngleGiven(float angle, int first, int[] expected)
        {
            var objectsData = InitializeSampleModelWithAscendingNumbers();
            var selector = new SelectionHelper();
            var selectedElementNumbers = selector.SelectE2DInPlane(objectsData, angle, first, Color.Green);
            Assert.IsTrue(selectedElementNumbers.SequenceEqual(expected));
        }

        [DataTestMethod]
        [DataRow(9, 62528, @"..\..\..\Examples\2D\curvedStrip.bpf")]
        [DataRow(0, 62528, @"..\..\..\Examples\2D\curvedStrip.bpf")]
        [DataRow(15, 62528, @"..\..\..\Examples\2D\curvedStrip.bpf")]
        [DataRow(5, 62528, @"..\..\..\Examples\2D\curvedStrip.bpf")]
        [DataRow(5, 62528, @"..\..\..\Examples\2D\curvedStrip.bpf")]
        public void SelectionHelper_OutSideModel_WhenAngleGiven(float angle, int first, string path)
        {
            //assing
            var model = new ModelData();

            //act
            model.Loader = new LoadModelFromBPFTextFile();
            model.Load(path);

            var selector = new SelectionHelper();
            var selectedElementNumbers = selector.SelectE2DInPlane(model.ObjectData, angle, first, Color.Green);

            Assert.AreEqual(selectedElementNumbers.Count, model.ObjectData.E2DCollection.Count);

        }

        [DataTestMethod]
        [DataRow(5, 169, @"c:\projs\Roman\Project\Project\proj_e.bpf2")]

        public void SelectionHelper_GenerateOnOutSideModel(float angle, int first, string path)
        {
            //assing

            var controller = new OperationalController.ProjectController();

            //act
            controller.Load(path);

            controller.Create2DForm3D();

            var objs = controller.SelectE2DInPlane(angle, first, Color.Green);

            

            Assert.AreNotEqual(objs.Count, 1);

        }

        [TestMethod]
        public void FindBoundaryEdges_ShouldWork_OnModelWithDifferentElements()
        {
            var controller = new OperationalController.ProjectController();
            //controller.ModelData.Loader = new LoadModelFromBPFTextFile();
            //controller.CreateProject(@"..\..\..\Examples\3D\cub_unsorted.bpf");
            controller.Load(@"..\..\..\Examples\3D\cub_unsorted.bpf");
            var linesNodes = controller.FindBoundaryEdges();
            var bounds = controller.CreateBoundaryEdges(linesNodes);
            
            Assert.AreEqual(19, bounds.Count());

            var presentersCreator  = new PresentersCreator();
            var presentor = presentersCreator.CreateLineObjectsPresenter(bounds);
            Assert.AreEqual(19, presentor.Count());
            
        }

        [TestMethod]
        public void GetElementsNormals()
        {
            var controller = new OperationalController.ProjectController();
            //controller.ModelData.Loader = new LoadModelFromBPFTextFile();
            
            //controller.CreateProject(@"..\..\..\Examples\3D\cub_unsorted.bpf");
            controller.Load(@"..\..\..\Examples\3D\cub_unsorted.bpf");
            var normals = controller.CalcElemsNormals(3); 

            Assert.AreNotEqual(0, normals.Count());

            var presentersCreator = new PresentersCreator();
            var presentor = presentersCreator.CreateLineObjectsPresenter(normals);
            Assert.AreNotEqual(0, presentor.Count());

        }

        [TestMethod]
        public void FindElementNodes()
        {
            //assing
            var controller = new OperationalController.ProjectController();

            //act
            controller.Load(@"..\..\..\Examples\3D\task_1.bpf");

            var els3D = controller.GetModelObjects(ObjType.Элемент3D);

            var interNodesEls3DFinder = new InterfacedNodesFinder();

            var interfacedNodesEls3D = interNodesEls3DFinder.Find(new List<int>() { 3, 48, 71 }, els3D.Select(x => (IElement)x));

            var elsForNode = new List<int>() { 1565,2048,2050, 2051 };
            //assert
            for (int i = 0; i < interfacedNodesEls3D[48].Count; i++)
            {
                if (!elsForNode.Contains(interfacedNodesEls3D[48][i]))
                    throw new Exception("номера не совпадают");
            }
        }         

        [TestMethod, Timeout(5000)]
        public void FindManyCoincidentObjects() 
        {
            var loader = new LoadModelFromCDBTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            var model = loader.Load(@"..\..\..\Examples\CDB\10e+5Nodes.cdb");
            var finder = new FindCoincidentObjects();
            var coincidentNodes = finder.Find(model.ObjectData.GetObjects(ObjType.Узел).ToList(), 1.0f);
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
            var surfaceNodes = finder.Find(elements.GetObjects());
            var distinctNumbers = surfaceNodes.Distinct();
            if (distinctNumbers.Contains(0))
                throw new Exception("Последовательность содержит неверный узел!");
        }

        private ObjectsData InitializeSampleModelWithAscendingNumbers()
        {
            var A = new Node(1, new Point3D(0, 0, 0));
            var B = new Node(2, new Point3D(0, 0, 1));
            var C = new Node(3, new Point3D(0, 1, 0));
            var D = new Node(4, new Point3D(0, 1, 1));
            var E = new Node(5, new Point3D(0, 1.5f, 0));
            var F = new Node(6, new Point3D(0, 2, 0));
            var G = new Node(7, new Point3D(0, 2, 1));
            var H = new Node(8, new Point3D(-0.5f, 3.5f, 0));
            var K = new Node(9, new Point3D(1, 0, 0));
            var elements2D = new List<Element2D>
            {
                new Triangle(9, new Node[] { A, B, C }),
                new Triangle(10, new Node[] { B, C, D }),
                new Triangle(11, new Node[] { D, C, E }),
                new Triangle(12, new Node[] { E, D, G }),
                new Triangle(13, new Node[] { E, F, G }),
                new Triangle(14, new Node[] { F, G, H }),
            };
            var elements3D = new List<Element3D>
            {
                new Tetra(15,new Node[] { A, B, C,K })
            };
            var nodes = new List<Node> { A, B, C, D, E, F, G, H };
            var objectsData = new ObjectsData();
            nodes.ForEach(x => objectsData.NodesSet.Add(x.Number, x));
            objectsData.E2DCollection.AddRange(ObjType.Элемент2D.ToString(), elements2D);
            objectsData.E3DCollection.AddRange(ObjType.Элемент3D.ToString(), elements3D);
            return objectsData;
        }

        private ObjectsData InitializeSampleModelWithMixingNumbers()
        {
            var A = new Node(1, new Point3D(0, 0, 0));
            var B = new Node(2, new Point3D(0, 0, 1));
            var C = new Node(3, new Point3D(0, 1, 0));
            var D = new Node(4, new Point3D(0, 1, 1));
            var E = new Node(5, new Point3D(0, 1.5f, 0));
            var F = new Node(6, new Point3D(0, 2, 0));
            var G = new Node(7, new Point3D(0, 2, 1));
            var H = new Node(8, new Point3D(-0.5f, 3.5f, 0));
            var K = new Node(9, new Point3D(1, 0, 0));
            var elements2D = new List<Element2D>
            {
                new Triangle(0, new Node[] { A, B, C }),
                new Triangle(6, new Node[] { B, C, D }),
                new Triangle(10, new Node[] { D, C, E }),
                new Triangle(11, new Node[] { E, D, G }),
                new Triangle(100, new Node[] { E, F, G }),
                new Triangle(121, new Node[] { F, G, H }),
            };
            var elements3D = new List<Element3D>
            {
                new Tetra(15,new Node[] { A, B, C,K })
            };
            var nodes = new List<Node> { A, B, C, D, E, F, G, H };
            var objectsData = new ObjectsData();
            nodes.ForEach(x =>  objectsData.NodesSet.Add(x.Number, x));
            objectsData.E2DCollection.AddRange(ObjType.Элемент2D.ToString(), elements2D);
            objectsData.E3DCollection.AddRange(ObjType.Элемент3D.ToString(), elements3D);
            return objectsData;
        }
    }

}
