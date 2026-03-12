using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalController;
using Project.Interfaces.Tasks;
using Project.IO;

namespace TestProject
{
    [TestClass]
    public class Basic
    {
        string testDir = @"..\..\..\Examples";
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [DataTestMethod]
        [DataRow(@"c:\projs\AdditiveTest\TestNewMeshGen\out_m1.bpf2", TaskType.Volume, TaskKind.термическая | TaskKind.механическая)]
        [DataRow(@"..\..\..\Examples\proj_traj.bpf", TaskType.Volume, TaskKind.термическая | TaskKind.механическая)]
        [DataRow(@"c:\projs\testProj\HeatingByWelding_Tjoint\proj_SRF.bpf2", TaskType.Volume, TaskKind.термическая | TaskKind.механическая)]
        [DataRow(@"c:\projs\mashining\proj.bpf2", TaskType.Volume, TaskKind.термическая)]
        public void LoadProjectFiletextFormat(string name,TaskType taskType,TaskKind taskKind)
        {
            //assing
            //c:\projs\testProj\Tempering\
            var controller = new OperationalController.ProjectController();
            controller.Load(name);

            //project.Loader.LoadEvent += (ar1, ar2) => { };
      

            //assert
            Assert.AreEqual(controller.ProjectKind, taskKind);
            Assert.AreEqual(controller.ProjectType, taskType);
            Assert.AreNotEqual(controller.GetAllCondData().First(), null);
        }

        [DataTestMethod]
        [DataRow("newProj.bpf")]
        [DataRow("newProj.bpf2")]
        public void CreateProjectFiletextFormat(string name)
        {
            //assing
            //c:\projs\testProj\Tempering\
            var project = new ProjectController();
            project.CreateProject($"{testDir}\\{name}"); 
            //assert
            Assert.AreEqual(Path.GetFileName(name), project.Name);
        }

        [TestMethod]
        public void SaveProjectFiletextFormat()
        {
            //assing
            var loader = new LoadProjectFromTextFormat();
            var saver = new SaveProjectTextFormat();
            loader.LoadEvent += (ar1, ar2) => { };
            saver.SaveEvent += (ar1, ar2) => { };
            //act
            var proj = loader.Load($@"{testDir}\proj_traj.bpf");
            saver.Save(proj, $@"{testDir}\proj_traj_save.bpf");

            //Assert

            var proj_1 = loader.Load($@"{testDir}\proj_traj_save.bpf");

            Assert.AreEqual(proj.GeneralData.Name, proj_1.GeneralData.Name);
        }
    }
}
