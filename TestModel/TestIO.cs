using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.IO;
using Model.ModelParcer;

namespace TestModel
{
    [TestClass]
    public class TestIO
    {
        [TestMethod]
        public void SaveModelToBpfTextFormat()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromGMSHTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            var saver = new SaveModelToProjectTextFile();
            saver.SaveEvent += (ar1, ar2) => { };

            //act
            var modelINP = loader.Load(@"..\..\Examples\3D\box2.inp");
            model.Load(modelINP);
            saver.Save(model, @"..\..\Examples\3D\task1.bpf");

            //assert
        }

        [TestMethod]
        public void ImportModelFromSTLtextFormat()
        {
            //assing
            var model = new ModelData();

            var loader = new LoadModelFromSTLFile();
            //model.SetLoader(loader);

            loader.LoadEvent += (ar1, ar2) => { };

            //act
            loader.Load(@"C:\BazisComponents\ModelSolution\TestModel\Examples\STL\ACII.stl");
            //assert
        }

        [TestMethod]
        public void ImportModelFromGMSHtextFormat()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromGMSHTextFile();
            loader.LoadEvent += (ar1, ar2) => { };

            //act
            var path = Path.GetFullPath(".\\").Replace("bin\\Debug\\", "");
            var files = Directory.GetFiles(path, "Korpus.inp", SearchOption.AllDirectories);
            //var sr = new StreamReader(@"C:\BazisXN\Model\GMSHLoader\test3");
            var modelINP = loader.Load(files[0]);
            model.Load(modelINP);

            //assert
            Assert.AreEqual(8448, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(17964, model.ObjectData.NumberOfElements);
        }

        [TestMethod]
        public void ImportModelFromVisualMeshASCIITextFormat()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += (ar1, ar2) => { };

            //act
            var path = Path.GetFullPath(".\\").Replace("bin\\Debug\\","");
            var files = Directory.GetFiles(path, "rhombDATA1.ASC", SearchOption.AllDirectories);
            var modelASCII = loader.Load(files[0]);

            //loader.Load(@"C:\Bazis_newGUI\MESH\RadicMesh\Plate2DATA.ASC");
            model.Load(modelASCII);
            //assert
            Assert.AreEqual(7, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(8, model.ObjectData.NumberOfElements);
        }

        [TestMethod]
        public void ImportHexFromCDBTextFormat()
        {
            var path = Path.GetFullPath(".\\").Replace("bin\\Debug\\", "");
            var files = Directory.GetFiles(path, "hex.cdb", SearchOption.AllDirectories);

            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(files[0]);
            Assert.AreEqual(8, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(1, model.ObjectData.NumberOfElements);
        }

        [TestMethod]
        public void ImportLongModelFromCDBTextFormat()
        {
            string path = @"..\..\Examples\CDB\longModel.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            Assert.AreEqual(3321, model.ObjectData.NumberOfNodes);

            var elements = model.ObjectData.FindMany<Element3D>();

            var countTetra = elements.Where(element => element.GetSurface().
            All(surface => surface.NumberOfPoints == 3)).Count();

            var countHexa = elements.Where(element => element.GetSurface().
All(surface => surface.NumberOfPoints == 4)).Count();

        }

        [TestMethod]
        public void FindCorrectPlaneNumberOfElements()
        {
            string path = @"..\..\Examples\CDB\longModel.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            var elements = model.ObjectData.FindMany<Element3D>();
            Assert.IsTrue(elements.All(element => element.GetSurface().Count() >= 4));
        }

        [TestMethod]
        public void ImportPentaFromCDBTextFormat()
        {
            string path = @"..\..\Examples\CDB\penta.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            var element = model.ObjectData.FindMany<Element3D>().First();
            Assert.AreEqual(6, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(1, model.ObjectData.NumberOfElements);
        }

        [TestMethod]
        public void ImportTetraFromCDBTextFormat()
        {
            string path = @"..\..\Examples\CDB\tetra.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            var element = model.ObjectData.FindMany<Element3D>().First();
            Assert.AreEqual(4, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(1, model.ObjectData.NumberOfElements);
        }
    }
}