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

namespace TestProject_test.Mesh
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
            model.Load(@"../../../../TestModelCore/Examples/3D/cub_unsorted_freeNodes.bpf");

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
            var actual = selector.SelectNodeInDirection(objectsData, angle, first, second);
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
            var selectedElementNumbers = selector.SelectE2DInPlane(objectsData, angle, first);
            Assert.IsTrue(selectedElementNumbers.SequenceEqual(expected));
        }

        [DataTestMethod]
        [DataRow(19, 9, new int[] { 9, 10, 11, 12, 13, 14 })]
        [DataRow(18, 9, new int[] { 9, 10, 11, 12, 13 })]
        public void SelectionHelper_ShouldSelectCorrect2DElements_WhenAngleGiven(float angle, int first, int[] expected)
        {
            var objectsData = InitializeSampleModelWithAscendingNumbers();
            var selector = new SelectionHelper();
            var selectedElementNumbers = selector.SelectE2DInPlane(objectsData, angle, first);
            Assert.IsTrue(selectedElementNumbers.SequenceEqual(expected));
        }

        [DataTestMethod]
        [DataRow(9, 62528, @"../../../../TestModelCore/Examples/2D/curvedStrip.bpf")]
        [DataRow(0, 62528, @"../../../../TestModelCore/Examples/2D/curvedStrip.bpf")]
        [DataRow(15, 62528, @"../../../../TestModelCore/Examples/2D/curvedStrip.bpf")]
        [DataRow(5, 62528, @"../../../../TestModelCore/Examples/2D/curvedStrip.bpf")]
        public void SelectionHelper_OutSideModel_WhenAngleGiven(float angle, int first, string path)
        {
            //assing
            var model = new ModelData();

            //act
            model.Loader = new LoadModelFromBPFTextFile();
            model.Load(path);

            var selector = new SelectionHelper();
            var selectedElementNumbers = selector.SelectE2DInPlane(model.ObjectData, angle, first);

            Assert.AreEqual(selectedElementNumbers.Count, model.ObjectData.E2DCollection.Count);

        }









        [TestMethod, Timeout(5000)]
        public void FindManyCoincidentObjects()
        {
            var loader = new LoadModelFromCDBTextFile();
            var model = loader.Load(@"../../../../TestModelCore/Examples/CDB/10e+5Nodes.cdb");
            var finder = new FindCoincidentObjects();
            var coincidentNodes = finder.Find(model.ObjectData.GetObjects(ObjType.Узел).ToList(), 1.0f);
        }

        [TestMethod]
        [DataRow(@"../../../../TestModelCore/Examples/VIsual-Mesh/rhombDATA1.ASC",0)]
        [DataRow(@"../../../../TestModelCore/Examples/BPF2TXT/surfNodesSample.bpf2", 2173)]
        public void TestCalcFindOpenSurfaceNodes(string path,int excNumb)
        {
            //assing
            var model = new ModelData();

            var ext = Path.GetExtension(path);

            if(ext == "*.ASC")
                model.Loader = new LoadModelFromASCIITextFile();
            else
                model.Loader = new LoadModelFromBPF2TextFile();

            model.Load(path);

            //act
            var elements = model.ObjectData.E3DCollection;

            //assert
            var openSurfNodes = new List<int>();

            var finder = new FindOpenSurfaceNodes();
            var surfaceNodes = finder.Find(elements.GetObjects());
            var distinctNumbers = surfaceNodes.Distinct();
            if (distinctNumbers.Contains(excNumb))
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
