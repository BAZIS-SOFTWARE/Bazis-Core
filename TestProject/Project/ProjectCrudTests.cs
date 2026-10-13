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

namespace TestProject_test.Project
{
    [TestClass]
    public class ProjectCrudTests
    {
        string testDir = @"../../../SharedData/ProjectFiles";
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
        [TestMethod]
        [DataRow(@"../../../SharedData/ProjectFiles/proj_traj.bpf", TaskType.Volume, TaskKind.термическая | TaskKind.механическая)]
        [DataRow(@"c:\projs\testProj\HeatingByWelding_Tjoint\proj_SRF.bpf2", TaskType.Volume, TaskKind.термическая | TaskKind.механическая)]
        [DataRow(@"c:\projs\mashining\proj.bpf2", TaskType.Volume, TaskKind.термическая)]
        public void LoadProjectFiletextFormat(string name, TaskType taskType, TaskKind taskKind)
        {
            //assing
            //c:\projs\testProj\Tempering\
            var controller = new global::OperationalController.ProjectController();
            controller.Load(name);



            //assert
            Assert.AreEqual(controller.TaskKind, taskKind);
            Assert.AreEqual(controller.TaskType, taskType);
            Assert.AreNotEqual(controller.GetAllCondData().First(), null);
        }

        [TestMethod]
        [DataRow("newProj.bpf")]
        [DataRow("newProj.bpf2")]
        public void CreateProjectFiletextFormat(string name)
        {
            //assing
            //c:\projs\testProj\Tempering\
            var project = new ProjectController();
            project.CreateProject(Path.Combine(testDir, name));
            //assert
            Assert.AreEqual(Path.GetFileName(name), project.Name);
        }


    }
}
