
using System.IO;
using System.Linq;
using System.Reflection;
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
        public void LoadModelFromBpfTextFormat()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromProjectTextFile();

            loader.LoadEvent += (ar1, ar2) => { };
            //act
            //model.Load(@"C:\Bazis_newGUI\PROJ\Test_3D\cub_1.bpf");
            //C:\Projs\TestProjTerm\Lazer\test2.bpf
            //var modelBPF = loader.Load(@"C:\Projs\TestProjTernMech\twoElements\two_tetra_termoMech_calc1.bpf");
            //var modelBPF = loader.Load(@"C:\Projs\TestProjTernMech\twoElements\two_tetra_termoMech_calc1.bpf");
            var modelBPF = loader.Load(@"C:\BazisSolver3.0\TestTasksSolution\ExampleProj\Term\2D_axi\two_tetra_term_cycle.bpf");
            model.Load(modelBPF);
            //assert
        }

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
            var modelINP = loader.Load(@"C:\Projs\TestProjTerm\box\box1.inp");
            model.Load(modelINP);
            saver.Save(model, @"C:\Projs\TestProjTerm\box\task_1.bpf");

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
            var elems = model.ObjectData.FindMany<Element3D>().ToList();
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
        public void ImportModelFromCDBTextFormat()
        {
            var path = Path.GetFullPath(".\\").Replace("bin\\Debug\\", "");
            var files = Directory.GetFiles(path, "file1.cdb", SearchOption.AllDirectories);

            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(files[0]);
            Assert.AreEqual(8, model.ObjectData.NumberOfNodes);
            Assert.AreEqual(1, model.ObjectData.NumberOfElements);
        }

    }
}
