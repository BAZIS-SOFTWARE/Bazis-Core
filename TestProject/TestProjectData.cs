using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using Model.IO;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project;
using ProjectController;

namespace TestProject
{
    [TestClass]
    public class TestProjectData
    {
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 1 }, ObjType.Узел, "clamp true", DisplayName = "clamp true")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 0 }, ObjType.Узел, "clamp true", DisplayName = "clamp true")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 2 }, ObjType.Узел, "clamp true", DisplayName = "clamp true")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 0, 1 }, ObjType.Узел, "clamp false", DisplayName = "clamp false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 0, 1, 2, 3 }, ObjType.Узел, "load false", DisplayName = "load false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 0, 1 }, ObjType.Узел, "load true", DisplayName = "load true")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 5 }, ObjType.Элемент2D, "newMesh false", DisplayName = "newMesh false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", new int[] { 1 }, ObjType.Узел, "newMesh false", DisplayName = "newMesh false")]
        public void RemoveObjsByNumber(string path, int[] objNumbers, ObjType type, string resStr)
        {
            //assing
            var control = new ProjectController.ProjectController();
            control.ModelData.Loader = new LoadModelFromBPF2TextFile();

            control.Load(path);
            var grName = resStr.Split(' ')[0];
            var res = bool.Parse(resStr.Split(' ')[1]);
            //act
            control.DeleteModelObjects(type, objNumbers);

            //assert
            Assert.AreEqual(res, control.ModelData.GroupData.Find(grName) != null);
            Assert.AreEqual(res, control.TaskData.Contains(grName));
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", "A17", ObjType.Элемент2D, "newMesh false", DisplayName = "newMesh false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", "A17aa", ObjType.Элемент2D, "newMesh true", DisplayName = "newMesh true")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", "t4", ObjType.Элемент3D, "newMesh true", DisplayName = "newMesh true")]
        public void RemoveSetByNameAndType(string path, string setName, ObjType type, string resStr)
        {
            //assing
            var control = new ProjectController.ProjectController();
            control.ModelData.Loader = new LoadModelFromBPF2TextFile();

            control.Load(path);
            var grName = resStr.Split(' ')[0];
            var res = bool.Parse(resStr.Split(' ')[1]);
            //act
            control.DeleteModelSet(type, setName);

            //assert
            Assert.AreEqual(res, control.ModelData.GroupData.Find(grName) != null);
            Assert.AreEqual(res, control.TaskData.Contains(grName));
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", ObjType.Узел, "clamp false", DisplayName = "clamp false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", ObjType.Элемент2D, "newMesh false", DisplayName = "newMesh false")]
        [DataRow(@"..\..\..\Examples\TestCRUDFunction\one_quad_term_cycle.bpf2", ObjType.Элемент3D, "newMesh true", DisplayName = "newMesh true")]
        public void ClearMeshCollection(string path, ObjType type, string resStr)
        {
            //assing
            var control = new ProjectController.ProjectController();
            control.ModelData.Loader = new LoadModelFromBPF2TextFile();

            control.Load(path);
            var grName = resStr.Split(' ')[0];
            var res = bool.Parse(resStr.Split(' ')[1]);
            //act
            control.ClearModelCollection(type);

            //assert
            Assert.AreEqual(res, control.ModelData.GroupData.Find(grName) != null);
            Assert.AreEqual(res, control.TaskData.Contains(grName));
        }

        [TestMethod]
        [DataRow(@"..\..\..\Examples\proj_traj.bpf", DisplayName = "proj_traj")]
        //[DataRow(@"..\..\..\Examples\proj_traj_baseLine_not_sorted.bpf", DisplayName = "proj_traj_baseLine_not_sorted")]

        //[DataRow(@"c:\projs\testProj\TubeToTubeTJoint\proj_v1.bpf")]
        //[DataRow(@"c:\projs\kamaz\4.bpf")]
        public void CreateTaskData(string relativeProjPath)
        {
            //assign
            var proj = new ProjectData();

            //act
            proj.Load(relativeProjPath);
        }
    }
}
