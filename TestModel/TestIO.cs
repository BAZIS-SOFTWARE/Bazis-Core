using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.IO;
using ModelParcer;

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
            var modelINP = loader.Load(@"C:\Users\kvm\source\repos\ModelSolution\TestModel\Examples\3D\box2.inp");
            model.Load(modelINP);
            saver.Save(model, @"C:\Users\kvm\source\repos\ModelSolution\TestModel\Examples\3D\task1.bpf");

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
            loader.Load(@"C:\Users\kvm\source\repos\ModelSolution\TestModel\Examples\STL\ACII.stl");
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
            var modelINP = loader.Load(@"C:\Users\kvm\source\repos\ModelSolution\TestModel\Examples\3D\Korpus.inp");
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
            //var reader = new StreamReader(@"C:\Bazis_newGUI\MESH\big_cubeDATA1.ASC");

            //act
            var modelASCII = loader.Load(@"C:\Users\kvm\source\repos\ModelSolution\TestModel\Examples\VIsual-Mesh\rhombDATA1.ASC");
            //loader.Load(@"C:\Bazis_newGUI\MESH\RadicMesh\Plate2DATA.ASC");
            model.Load(modelASCII);
            //assert
            var elems = model.ObjectData.FindMany<Element3D>().ToList();
        }

        [TestMethod]
        public void ImportHexFromCDBTextFormat()
        {
            string path = @"C:\Users\kvm\source\repos\ModelSolution\TestModel\Examples\CDB\hex.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            var element = model.ObjectData.FindMany<Element3D>().First();
            Assert.AreEqual(8, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(1, model.ObjectData.NumberOfElements);
        }

        [TestMethod, Timeout(2000)]
        public void ImportLongModelFromCDBTextFormat()
        {
            string path = @"C:\Users\kvm\source\repos\ModelSolution\TestModel\Examples\CDB\cube.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            Assert.AreEqual(3321, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(9559, model.ObjectData.NumberOfElements);
        }

        [TestMethod]
        public void ImportPentaFromCDBTextFormat()
        {
            string path = @"C:\Users\kvm\source\repos\ModelSolution\TestModel\Examples\CDB\penta.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            var element = model.ObjectData.FindMany<Element3D>().First();
            Assert.AreEqual(6, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(1, model.ObjectData.NumberOfElements);
        }
    }
}