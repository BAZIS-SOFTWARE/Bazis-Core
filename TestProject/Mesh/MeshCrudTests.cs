using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using Model.IO;
using OperationalController;
using OperationalController.GmshController;
using Project;
using Project.Interfaces.Tasks;
using Project.IO;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using static IronPython.Modules.PythonIterTools;

namespace TestProject_test.Mesh
{
    [TestClass]
    public class MeshCrudTests
    {
        string testDir = @"../../../SharedData/ProjectFiles";
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
        [TestMethod]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", new int[] { 1 }, ObjType.Узел, "clamp true", DisplayName = "clamp true")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", new int[] { 0 }, ObjType.Узел, "clamp true", DisplayName = "clamp true")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", new int[] { 2 }, ObjType.Узел, "clamp true", DisplayName = "clamp true")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", new int[] { 0, 1 }, ObjType.Узел, "clamp false", DisplayName = "clamp false")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", new int[] { 0, 1, 2, 3 }, ObjType.Узел, "load false", DisplayName = "load false")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", new int[] { 0, 1 }, ObjType.Узел, "load true", DisplayName = "load true")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", new int[] { 5 }, ObjType.Элемент2D, "newMesh false", DisplayName = "newMesh false")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", new int[] { 1 }, ObjType.Узел, "newMesh false", DisplayName = "newMesh false")]
        public void RemoveObjsByNumber(string path, int[] objNumbers, ObjType type, string resStr)
        {
            //assing
            var control = new ProjectController();

            //control.CreateProject(path);
            //control.ModelData.Loader = new LoadModelFromBPF2TextFile();
            control.Load(path);
            var grName = resStr.Split(' ')[0];
            var res = bool.Parse(resStr.Split(' ')[1]);
            //act
            control.DeleteModelObjects(type, objNumbers);

            //assert

            Assert.AreEqual(res, control.GetModelGroup(grName) != null);
            Assert.AreEqual(res, control.ContainsConditions(grName));
        }

        [TestMethod]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", "A17", ObjType.Элемент2D, "newMesh false", DisplayName = "newMesh false")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", "A17aa", ObjType.Элемент2D, "newMesh true", DisplayName = "newMesh true")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", "t4", ObjType.Элемент3D, "newMesh true", DisplayName = "newMesh true")]
        public void RemoveSetByNameAndType(string path, string setName, ObjType type, string resStr)
        {
            //assing
            var control = new ProjectController();
            //control.CreateProject(path);
            //control.ModelData.Loader = new LoadModelFromBPF2TextFile();
            control.Load(path);
            var grName = resStr.Split(' ')[0];
            var res = bool.Parse(resStr.Split(' ')[1]);
            //act
            control.DeleteModelSet(type, setName);

            //assert
            Assert.AreEqual(res, control.GetModelGroup(grName) != null);
            Assert.AreEqual(res, control.ContainsConditions(grName));
        }

        [DataTestMethod]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", ObjType.Узел, "clamp false", DisplayName = "clamp false")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", ObjType.Элемент2D, "newMesh false", DisplayName = "newMesh false")]
        [DataRow(@"../../../OperationalController/Data/TestCRUDFunction/one_quad_term_cycle.bpf2", ObjType.Элемент3D, "newMesh true", DisplayName = "newMesh true")]
        public void ClearMeshCollection(string path, ObjType type, string resStr)
        {
            //assing
            var control = new ProjectController();

            //control.CreateProject(path);
            //control.ModelData.Loader = new LoadModelFromBPF2TextFile();
            control.Load(path);

            var grName = resStr.Split(' ')[0];
            var res = bool.Parse(resStr.Split(' ')[1]);
            //act
            control.ClearModelCollection(type);

            //assert
            Assert.AreEqual(res, control.GetModelGroup(grName) != null);
            Assert.AreEqual(res, control.ContainsConditions(grName));
        }

    }
}
