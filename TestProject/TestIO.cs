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
using Project.IO;
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
        [DataRow(@"..\..\..\Examples\proj_traj.bpf", TaskType.Volume, TaskKind.термическая_механическая)]
        //[DataRow(@"c:\projs\AdditiveTest\MeshWithBounds\новый_проект.bpf2", TaskType.Volume, TaskKind.термическая_механическая)]
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
            saver.Save(proj, $@"{testDir}\proj_traj.bpf");

            //Assert

            var proj_1 = loader.Load($@"{testDir}\proj_traj.bpf");

            Assert.AreEqual(proj.GeneralData.Name, proj_1.GeneralData.Name);
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
