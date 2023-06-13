using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
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
            loader.Load(@"..\..\Examples\STL\ACII.stl");
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
            var modelINP = loader.Load(@"..\..\Examples\3D\Korpus.inp");
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
            var modelASCII = loader.Load(@"..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");
            //loader.Load(@"C:\Bazis_newGUI\MESH\RadicMesh\Plate2DATA.ASC");
            model.Load(modelASCII);
            //assert
            Assert.AreEqual(7, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(8, model.ObjectData.NumberOfElements);
        }

        [TestMethod]
        public void ImportHexFromCDBTextFormat()
        {
            string path = @"..\..\Examples\CDB\hex.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            var element = model.ObjectData.FindMany<Element3D>().First();
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
            Assert.IsTrue(elements.All(element => element.GetSurface().
            All(surface => surface.GetPoints().Count() > 2)));
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