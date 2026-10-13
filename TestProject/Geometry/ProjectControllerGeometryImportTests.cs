using Geometry;
using Model.Interfaces;
using Model;
using Model.MeshObjects;
using OperationalController.GmshController;
using OperationalController;
using MathNet.Spatial.Euclidean;
using Point3D = global::Geometry.Point3D;
using OperationalController.GeomObjsCreator;

namespace TestProject_test.Geometry
{
    [TestClass]
    public class ProjectControllerGeometryImportTests
    {
        private string modelFolder = @"../../../../TestModelCore/Examples";

        [DataTestMethod]
        [DataRow(@"../../../../TestModelCore/Examples/CAD/screw.step")]
        [DataRow(@"../../../../TestModelCore/Examples/CAD/block.igs")]

        //[DataRow(@"/GMSHAPI/cubes.geo")]
        //Для geo ядра презентация STL не работает (как и в gmsh)
        public void ImportCADoutside(string fileName)
        {
            var project = new ProjectController();
            project.InitializeGeometry(GmshLibrary.FilePath);
            project.ImportCAD(fileName);

            //assert
            Assert.AreNotEqual(project.GetAllModelObjects().Count(),0);
        }

        [DataTestMethod]
        [DataRow(@"invalid/вбс/part 1.step")]
        [DataRow(@"invalid/путь/part1.step")]
        [DataRow(@"invalid/part 1.step")]
        public void ImportCAD_ShouldFaill_WhenPathContainsForbiddenSymbols(string path)
        {
            var project = new ProjectController();
            Assert.ThrowsException<ArgumentException>(() =>
            project.ImportCAD(path), 
            "В пути файла присутствуют символы кириллицы или пробелы. Выберите другой файл или переименуйте этот!");
        }

        [DataTestMethod]
        [DataRow(@"/CAD/screw.step")]
        [DataRow(@"/CAD/assembly_1.step")]
        [DataRow(@"/CAD/assembly_2.stp")]
        [DataRow(@"/CAD/nasos.step")]
        [DataRow(@"/CAD/model_with_free_surfaces.stp")]
        [DataRow(@"/CAD/block.igs")]

        //[DataRow(@"/GMSHAPI/cubes.geo")]
        //Для geo ядра презентация STL не работает (как и в gmsh)
        public void ImportCAD(string fileName)
        {
            var project = new ProjectController();
            project.InitializeGeometry(GmshLibrary.FilePath);
            project.ImportCAD(modelFolder + fileName);

            //assert
            Assert.AreNotEqual(project.GetAllModelObjects().Count(), 0);
        }
    }
}
