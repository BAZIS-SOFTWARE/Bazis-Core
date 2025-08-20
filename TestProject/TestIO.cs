using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using OperationalController;
using PostProc.IO;
using PostProc.IO.GridExport;
using PostProc.ScenePresenter;
using Project;
using Project.Interfaces.Tasks;
using Project.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace TestProject
{
    [TestClass]
    public class TestIO
    {
        string testDir = @"..\..\..\Examples";
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [DataTestMethod]
        //[DataRow("c:\\projs\\testProj\\Tempering", "proj.bpf")]
        [DataRow("proj_traj.bpf",TaskType.Volume,TaskKind.термическая_механическая)]
        //[DataRow("c:\\BazisSolver\\TestTaskSolutionMSTest\\ExampleProj\\Mech\\3D\\ConstLoadAndTemp500\\", "proj.bpf")]
        //[DataRow("c:\\projs\\ifm", "proj_v1.bpf")]
        public void LoadProjectFiletextFormat(string name,TaskType taskType,TaskKind taskKind)
        {
            //assing
            //c:\projs\testProj\Tempering\
            var project = new ProjectData($@"{testDir}\{name}");

            project.Loader.LoadEvent += (ar1, ar2) => { };
            project.Load();

            //assert
            Assert.AreEqual(project.TaskData.TaskKind, taskKind);
            Assert.AreEqual(project.TaskData.TaskType, taskType);
            Assert.AreNotEqual(project.TaskData.Find<CondData>().First(),null);
        }

        [DataTestMethod]
        [DataRow("newProj.bpf")]
        [DataRow("newProj.bpf2")]
        public void CreateProjectFiletextFormat(string name)
        {
            //assing
            //c:\projs\testProj\Tempering\
            var project = new Controller();
            project.CreateProject($"{testDir}\\{name}");

            //assert
            Assert.AreEqual(Path.GetFileName(name), project.Name);
        }

        [TestMethod]
        public void SaveProjectFiletextFormat()
        {
            //assing
            var project = new ProjectData($@"{testDir}\proj_traj.bpf");
 
            project.ModelData.Loader.LoadEvent += (ar1, ar2) => { };
            project.ModelData.Saver.SaveEvent += (ar1, ar2) => { };

            project.Loader.LoadEvent += (ar1, ar2) => { };

            //act
            project.Load();

            project.Saver.SaveEvent += (ar1,ar2) => { };
            project.GeneralData.Name = "proj_traj_save.bpf";

            project.Save();

            //Assert

            project.Load();
        }


        [TestMethod]
        [DataRow(".bpf")]
        [DataRow(".STL-BIN")]
        [DataRow(".stl-bin")]
        [DataRow(".stl-text")]
        public void IResultExporter_ShouldPass_WhenExportGridFileCreated(string format)
        {
            var points = new List<Point3D>() { new Point3D(-1f, -1f, 0f), new Point3D(1f, 1f, 0f), new Point3D(-1f, 1f, 0f) };
            var figures = new List<ResultFigure> 
            { 
                new ResultFigure(points, Color.Black)
            };

            var exporter = new GridExporter();
            exporter.ExportGridSurfaces(figures, $@"{testDir}\export" + format.ToLower().Split('-')[0], format);
        }
    }
}
