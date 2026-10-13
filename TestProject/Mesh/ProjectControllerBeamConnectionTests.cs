using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.MeshObjects;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Model.Interfaces;
using Model.IO;
using System.IO;
using OperationalController.MeshObjsUtility;
using OperationalController.MeshObjsCreator;
using Model.IO.STL;
using OperationalController;

namespace TestProject_test.Mesh
{
    [TestClass]
    public class ProjectControllerBeamConnectionTests
    {
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void BoundaryConnection(int maxBeams)
        {
            var controller = new ProjectController();
            controller.CreateNode(0, 0, 0, 0);
            controller.CreateNode(1, 0, 0, 1);
            controller.CreateNode(2, 1, 0, 1);
            var master = controller.GetModelObject(ObjType.Узел, 0);
            var firstSlave = controller.GetModelObject(ObjType.Узел, 1);
            var secondSlave = controller.GetModelObject(ObjType.Узел, 2);
            controller.CreateGroup("master", [master]);
            controller.CreateGroup("slave", [firstSlave, secondSlave]);

            controller.ConnectByBeams("master", "slave", 2, maxBeams);

            Assert.AreEqual(maxBeams, controller.GetModelObjects(ObjType.Элемент1D).Count());
        }
    }
}
