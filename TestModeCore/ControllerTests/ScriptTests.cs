using OperationalController;
using OperationalController.GmshController;
using System.Globalization;

namespace TestModel.ControllerTests
{
    [TestClass]
    public class ScriptTests
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\CAD\Tavr.step", @"c:\BazisComponents\BazisCore\TestModel\Examples\Scripts\write.gscript")]

        public void CompareSaveAndLoad(string cadFile,string scriptFile)
        {
            var project = new ProjectController();
            project.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            project.ImportCAD(cadFile);
            project.GmshController.SetSize(1, 0.5);
            project.GmshController.SetSize(2, 0.5);

            project.GmshController.SetTransfiniteCurve(1,5);
            project.GmshController.SetTransfiniteCurve(2, 11);

            //var surf2Points = project.GmshController.GetSurfaceNodes(2);
            //project.GmshController.SetTransfiniteSurface(2, GmshApi.Arrangement.Left, surf2Points);

            //var surf0Points = project.GmshController.GetSurfaceNodes(1);
            //project.GmshController.SetTransfiniteSurface(1, GmshApi.Arrangement.Left);
            //project.GmshController.SetRecombineSurface(1);

            var surf3Points = project.GmshController.GetSurfaceNodes(3);
            project.GmshController.SetTransfiniteSurface(3, GmshApi.Arrangement.Left,surf3Points);
            project.GmshController.SetRecombineSurface(3);

            var surf17Points = project.GmshController.GetSurfaceNodes(17);
            project.GmshController.SetTransfiniteSurface(17, GmshApi.Arrangement.Left, new[] { 17,18,21 });

            //project.GmshController.SetTransfiniteVolume(1);
            //project.GmshController.SetGradientVolume(2,1,1,1,5);

            project.GmshController.Gmsh.Model.Mesh.Generate(2);

            var beforeLoad = project.GmshController.Gmsh.Logger.GetLastError();

            Assert.AreEqual(beforeLoad, "");

            project.SaveSMF(scriptFile);
            project.LoadSMF(scriptFile);

            project.GmshController.Gmsh.Model.Mesh.Generate(2);

            var afterLoad = project.GmshController.Gmsh.Logger.GetLastError();
            
            // TOOD доработать. Почему то возникает ошибка при генерации сетки
            //Assert.AreEqual(afterLoad, "");

            //assert
            Assert.AreEqual(project.GmshController.GetSize(1), 0.5);
            Assert.AreEqual(project.GmshController.GetSize(2), 0.5);
            Assert.AreEqual(project.GmshController.GetTransfiniteCurve(1)[0], "5");
            //Assert.AreEqual(project.GmshController.GetTransfiniteSurface(2)[0], "5,6,3,1");
            Assert.AreEqual(project.GmshController.GetTransfiniteSurface(3)[1], "Left");
            Assert.AreEqual(project.GmshController.GetTransfiniteSurface(17)[1], "Left");
            Assert.AreEqual(project.GmshController.GetRecombineSurface(0), false);
            Assert.AreEqual(project.GmshController.GetRecombineSurface(3), true);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\CAD\modelEmbedded.stp", @"..\..\..\Examples\Scripts\embedded.gscript")]
        public void EmbedToSurface(string cadFile, string scriptFile)
        {
            var project = new ProjectController();
            project.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            project.ImportCAD(cadFile);

            var curvesNumber = new int[] { 21, 22 };
            var numberSurface = 1;
            project.GmshController.Gmsh.Model.Mesh.Embed(1, curvesNumber, 2, numberSurface);

            var beforeLoad = project.GmshController.Gmsh.Logger.GetLastError();

            Assert.AreEqual(beforeLoad, "");

            project.SaveSMF(scriptFile);
            project.LoadSMF(scriptFile);

            var embedded = project.GmshController.Gmsh.Model.Mesh.GetEmbedded(2, numberSurface).Where((v, i) => (i & 1) == 1).ToArray();
            var joinStr = string.Join(",", embedded);
            //assert
            Assert.AreEqual(joinStr, "21,22");
        }
    }
}
