using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalController;
using Project.Interfaces.Tasks;
using Project.IO;

namespace TestProject
{
    [TestClass]
    public class TestProjectData
    {
        string testDir = @"..\..\..\Examples";
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
