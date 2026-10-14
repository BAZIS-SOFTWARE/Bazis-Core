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
    public class ProjectControllerMeshServicesTests
    {
        [DataTestMethod]
        [DataRow(5, @"../../../../TestModelCore/Examples/3D/cub_unsorted.bpf")]
        public void SelectionHelper_GenerateOnOutSideModel(float angle, string path)
        {
            var controller = new global::OperationalController.ProjectController();
            controller.Load(path);
            controller.Create2DForm3D();
            var first = controller.GetModelObjects(ObjType.Элемент2D).First().Number;
            var objs = controller.SelectE2DInPlane(angle, first, Color.Green);
            Assert.IsTrue(objs.Count > 0);
        }

        [TestMethod]
        public void FindBoundaryEdges_ShouldWork_OnModelWithDifferentElements()
        {
            var controller = new global::OperationalController.ProjectController();
            //controller.ModelData.Loader = new LoadModelFromBPFTextFile();
            //controller.CreateProject(@"../../../../TestModelCore/Examples/3D/cub_unsorted.bpf");
            controller.Load(@"../../../../TestModelCore/Examples/3D/cub_unsorted.bpf");
            var linesNodes = controller.FindBoundaryEdges();
            var bounds = controller.CreateBoundaryEdges(linesNodes);
            
            Assert.AreEqual(19, bounds.Count());

            var presentersCreator  = new PresentersCreator();
            var presentor = presentersCreator.CreateLineObjectsPresenter(bounds.ToList(), Color.Blue);
            Assert.AreEqual(19, presentor.Count());
            
        }

        [TestMethod]
        public void GetElementsNormals()
        {
            var controller = new global::OperationalController.ProjectController();
            //controller.ModelData.Loader = new LoadModelFromBPFTextFile();
            
            //controller.CreateProject(@"../../../../TestModelCore/Examples/3D/cub_unsorted.bpf");
            controller.Load(@"../../../../TestModelCore/Examples/3D/cub_unsorted.bpf");
            var normals = controller.CalcElemsNormals(3); 

            Assert.AreNotEqual(0, normals.Count());

            var presentersCreator = new PresentersCreator();
            var presentor = presentersCreator.CreateLineObjectsPresenter(normals.ToList(), Color.Blue);
            Assert.AreNotEqual(0, presentor.Count());

        }

        [TestMethod]
        public void FindElementNodes()
        {
            //assing
            var controller = new global::OperationalController.ProjectController();

            //act
            controller.Load(@"../../../../TestModelCore/Examples/3D/task_1.bpf");

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
    }
}
