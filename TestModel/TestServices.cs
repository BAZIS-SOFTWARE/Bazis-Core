using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelInterfaces;
using ModelController.MeshObjsUtility;
using Model.IO;
using Model.MeshObjects;
using ModelInterfaces.MeshObjects;

namespace TestModel
{
    [TestClass]
    public class TestServices
    {
        [TestMethod]
        public void FindFreeNodes()
        {
            //assing
            var model = new ModelData();

            //act
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

        public void SelectionHelper_OutSideModel_WhenAngleGiven(float angle, int first, string path)
        {
            //assing
            var model = new ModelData();

            //act
            model.Load(path);

            var selector = new SelectionHelper();
            var selectedElementNumbers = selector.SelectE2DInPlane(model.ObjectData, angle, first, Color.Green);

            Assert.AreEqual(selectedElementNumbers.Count, model.ObjectData.E2DCollection.Count);

        }

        [TestMethod]
        public void FindBoundaryEdges_ShouldWork_OnModelWithDifferentElements()
        {
            var model = new ModelData();
            model.Loader = new LoadModelFromProjectTextFile();
            model.Load(@"..\..\..\Examples\3D\cub_unsorted.bpf");

            var modelController = new ModelController.ModelController(model);
            var linesNodes = modelController.BoundaryEdgesFinder.Find(model.ObjectData.GetAllElements().Select(x => (ISurfaceElement)x));
            var bounds = modelController.BoundaryEdgesFinder.CreateBoundaryEdges(linesNodes,model);
            
            Assert.AreEqual(19, bounds.Count());

            var presentor = modelController.PresentersCreator.CreateLineObjectsPresenter(bounds);
            Assert.AreEqual(19, presentor.Count());
            
        }

        [TestMethod]
        public void GetElementsNormals()
        {
            var model = new ModelData();
            model.Loader = new LoadModelFromProjectTextFile();
            model.Load(@"..\..\..\Examples\3D\cub_unsorted.bpf");

            var modelController = new ModelController.ModelController(model);
            var normals = modelController.NormalCalculator.CalcElemsNormals(model.ObjectData.GetAllElements().Select(x => (ISurfaceElement)x)); 

            Assert.AreNotEqual(0, normals.Count());

            var presentor = modelController.PresentersCreator.CreateLineObjectsPresenter(normals);
            Assert.AreNotEqual(0, presentor.Count());

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
            model.Load(@"..\..\..\Examples\3D\cub_unsorted_freeNodesCoincident.bpf");

            var finder = new FindCoincidentObjects();

            finder.ProgressEvent += (ar1, ar2) => { };

            var coincidentNodes = finder.Find(model.ObjectData.GetObjects(ObjType.Узел).ToList(), 0.01f);

            var merger = new MergeObjects();

            var mergedNodes = merger.Merge<INode>(coincidentNodes, model.ObjectData.NodeCollection.ToList());

            //assert
            model.ObjectData.NodeCollection.Clear();
            model.ObjectData.NodeCollection.AddRange(mergedNodes);
            var numb = mergedNodes.First().Number;

            foreach (var item in mergedNodes.Skip(1))
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
            var finder = new FindCoincidentObjects();
            finder.ProgressEvent += (ar1, ar2) => { };
            var coincidentNodes = finder.Find(model.ObjectData.GetObjects(ObjType.Узел).ToList(), 1.1f);
            var numberOfObjs = coincidentNodes.Count();
            Assert.AreEqual(4, numberOfObjs);
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
            var surfaceNodes = finder.Find(elements);
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
                new Tetra(15,new Node[] { A, B, C,D })
            };
            var nodes = new List<Node> { A, B, C, D, E, F, G, H };
            var objectsData = new ObjectsData();
            objectsData.NodeCollection.AddRange(nodes);
            objectsData.E2DCollection.AddRange(elements2D);
            objectsData.E3DCollection.AddRange(elements3D);
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
                new Tetra(15,new Node[] { A, B, C,D })
            };
            var nodes = new List<Node> { A, B, C, D, E, F, G, H };
            var objectsData = new ObjectsData();
            objectsData.NodeCollection.AddRange(nodes);
            objectsData.E2DCollection.AddRange(elements2D);
            objectsData.E3DCollection.AddRange(elements3D);
            return objectsData;
        }
    }

}
