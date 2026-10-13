using GmshApi;
using IronPython.Runtime;
using MathNet.Spatial.Euclidean;
using Model;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsFinders;
using Model.IO;
using Model.MeshObjects;
using OperationalController;
using OperationalController.GmshController;
using OperationalController.MeshObjsCreator.Extrude;
using System.Numerics;
using Point3D = global::Geometry.Point3D;

namespace TestProject_test.Mesh
{
    [TestClass]
    public class ProjectControllerMeshGroupTests
    {
        private string modelFolder = @"../../../../TestModelCore/Examples";

        [DataTestMethod]
        [DataRow(@"/CAD/cilindr.step", 3, 3, 1)]
        [DataRow(@"/CAD/cilindr.step", 2, 2, 1)]
        [DataRow(@"/CAD/cilindr.step", 0, 2, 1)]
        [DataRow(@"/CAD/cilindr.step", 0, 3, 1)]
        public void CreateGroupByGeoId(string path,int meshDim,int geoDim,int tag)
        {
            var proj = new ProjectController();
            proj.InitializeGeometry(GmshLibrary.FilePath);
            proj.ImportCAD(modelFolder + @"/" + path);

            proj.GenerateMesh(3);

            var gr = proj.CreateGroupByGeoObjs(meshDim, geoDim, tag);

            //Скрытие\показ 3д элементов сопряженных с объемом и проверка элементов
            Assert.AreNotEqual(gr.Count(), 0);
        }

        [DataTestMethod]
        [DataRow(@"/CAD/PART_3.stp")]
        public void CreateOpenSurfaceNodesGroup(string path)
        {
            var proj = new ProjectController();
            proj.InitializeGeometry(GmshLibrary.FilePath);
            proj.ImportCAD(modelFolder + @"/" + path);
            proj.GenerateMesh(3);
            //e3D_1
            var grSurfNodes = proj.CreateOpenSurfaceNodesGroup("e3D_2");

            //Скрытие\показ 3д элементов сопряженных с объемом и проверка элементов
            Assert.AreNotEqual(proj.GetModelGroup(grSurfNodes).Count(), 0);
        }
    }
}
