using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalController;
using Project.Interfaces.Tasks;
using Project.IO;

namespace TestProject_test.Project
{
    [TestClass]
    public class TestProjectData
    {
        string testDir = @"../../../SharedData/ProjectFiles";
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        } 

        [TestMethod]
        public void SaveProjectFiletextFormat()
        {
            //assing
            var loader = new LoadProjectFromTextFormat();
            var saver = new SaveProjectTextFormat();
            saver.SaveEvent += (ar1, ar2) => { };
            //act
            var proj = loader.Load(Path.Combine(testDir, "proj_traj.bpf"));
            saver.Save(proj, Path.Combine(testDir, "proj_traj_save.bpf"));

            //Assert

            var proj_1 = loader.Load(Path.Combine(testDir, "proj_traj_save.bpf"));

            Assert.AreEqual(proj.GeneralData.Name, proj_1.GeneralData.Name);
        }
    }
}
