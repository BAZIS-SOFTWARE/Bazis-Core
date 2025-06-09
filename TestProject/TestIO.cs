using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using PostProc.IO;
using PostProc.ScenePresenter;
using Project;
using Project.Tasks;

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
        [DataRow("proj_traj.bpf")]
        //[DataRow("c:\\BazisSolver\\TestTaskSolutionMSTest\\ExampleProj\\Mech\\3D\\ConstLoadAndTemp500\\", "proj.bpf")]
        //[DataRow("c:\\projs\\ifm", "proj_v1.bpf")]
        public void LoadProjectFiletextFormat(string name)
        {
            //assing
            //c:\projs\testProj\Tempering\
            var project = new ProjectData();

            project.Loader.LoadEvent += (ar1, ar2) => { };
            project.Load($@"{testDir}\{name}");

            //assert
            Assert.AreNotEqual(project.TaskData.Find<PhysicalData>().First(),null);
        }

        [TestMethod]
        public void SaveProjectFiletextFormat()
        {
            //assing
            var project = new ProjectData("someName", testDir);
 
            project.ModelData.Loader.LoadEvent += (ar1, ar2) => { };
            project.ModelData.Saver.SaveEvent += (ar1, ar2) => { };

            project.Loader.LoadEvent += (ar1, ar2) => { };

            //act
            project.Load($@"{testDir}\proj_traj.bpf");

            project.Saver.SaveEvent += (ar1,ar2) => { };
            project.GeneralData.Name = "proj_traj_save.bpf";

            project.Save();

            //Assert

            project.Load($@"{testDir}\proj_traj_save.bpf");
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
