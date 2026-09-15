using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalController;
using System.Globalization;
using System.IO;
using System.Threading;

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
        [DataRow(@"..\..\..\Examples\CAD\Tavr.step", @"..\..\..\Examples\Scripts\write.gscript")]
        public void CompareSaveAndLoad(string cadFile, string scriptFile)
        {
            var project = new ProjectController();
            var outputFile = Path.GetTempFileName();

            try
            {
                project.InitializeGeometry(@"..\..\..\..\GmshApi\gmsh.dll");
                project.ImportCAD(cadFile);
                project.LoadSMF(scriptFile);
                project.SaveSMF(outputFile);

                var script = File.ReadAllText(outputFile);

                StringAssert.Contains(script, "Кривая 1 5 Progression 1");
                StringAssert.Contains(script, "Поверхность 3 6,9,7,10 Left quad");
                StringAssert.Contains(script, "Поверхность 17 17,18,21 Left tria");
            }
            finally
            {
                File.Delete(outputFile);
            }
        }
    }
}
