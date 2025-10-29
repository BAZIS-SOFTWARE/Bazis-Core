using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.GeometryObjects;
using Model.Interfaces;
using OperationalController;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        [DataRow(@"..\..\..\Examples\CAD\Tavr.step", @"..\..\..\Examples\Scripts\write.sfm")]

        public void CompareSaveAndLoad(string cadFile,string scriptFile)
        {
            var project = new ProjectController();
            project.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            project.ImportCAD(cadFile);
            project.GmshController.SetSize(1, 0.5);
            project.GmshController.SetSize(2, 0.5);

            project.GmshController.SetTransfiniteCurve(1,5);
            project.GmshController.SetTransfiniteCurve(2, 11);
            project.GmshController.SetTransfiniteSurface(2, GmshApi.Arrangement.Right, new[] {5,6,3,1} );
            project.GmshController.SetTransfiniteSurface(3);
            project.GmshController.SetTransfiniteSurface(17, GmshApi.Arrangement.Left, new[] { 17,18,21 });

            project.GmshController.SetTransfiniteVolume(1);
            project.GmshController.SetGradientVolume(2,1,1,1,5);

            project.SaveSMF(scriptFile);
            project.LoadSMF(scriptFile);


            //assert
            Assert.AreEqual(project.GmshController.GetSize(1), 0.5);
            Assert.AreEqual(project.GmshController.GetSize(2), 0.5);
            Assert.AreEqual(project.GmshController.GetTransfiniteCurve(1)[0], "5");
            Assert.AreEqual(project.GmshController.GetTransfiniteSurface(2)[0], "5,6,3,1");
            Assert.AreEqual(project.GmshController.GetTransfiniteSurface(3)[1], "Left");
            Assert.AreEqual(project.GmshController.GetTransfiniteVolume(1)[0], "регулярная");
            Assert.AreEqual(project.GmshController.GetTransfiniteVolume(2)[1], "1");
        }

    }
}
