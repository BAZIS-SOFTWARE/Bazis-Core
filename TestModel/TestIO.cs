using System.IO;
using System.Linq;
using GMSH_Importer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.IO;
using ModelInterface;
using STL_imp_exp;

namespace TestModel
{
    [TestClass]
    public class TestIO
    {
        [TestMethod]
        public void ImportModelFromVisualMeshASCIITextFormat()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += NewMesh_LoadEvent;
            //var reader = new StreamReader(@"C:\Bazis_newGUI\MESH\big_cubeDATA1.ASC");

            //act
            var modelASCII = loader.Load(@"C:\Projs\ExampleMesh\asc\DATA3.ASC");
            //loader.Load(@"C:\Bazis_newGUI\MESH\RadicMesh\Plate2DATA.ASC");
            model.Load(modelASCII);
            //assert
            var elems = model.ObjectData.FindMany<Element3D>().ToList();
        }

        [TestMethod]
        public void LoadModelFromBpfTextFormat()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromProjectTextFile();

            loader.LoadEvent += NewMesh_LoadEvent;
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
            var loader = new Import_Mesh();
            loader.LoadEvent += NewMesh_LoadEvent;
            var saver = new SaveModelToProjectTextFile();
            saver.SaveEvent += Model_SaveEvent;

            //act
            var modelINP = loader.Load(@"C:\Projs\TestProjTerm\box\box1.inp");
            model.Load(modelINP);
            saver.Save(model, @"C:\Projs\TestProjTerm\box\task_1.bpf");

            //assert
        }

        private void Model_SaveEvent(object arg1, SaverEventArgs arg2)
        {
            //throw new System.NotImplementedException();
        }

        [TestMethod]
        public void ImportModelFromSTLtextFormat()
        {
            //assing
            var model = new ModelData();

            var loader = new ImportSTL();
            //model.SetLoader(loader);

            //model.LoadEvent += NewMesh_LoadEvent;

            //act
            loader.Load(@"C:\BazisXN\Model\STLLoader\ACII.stl");
            //assert
        }

        [TestMethod]
        public void ImportModelFromGMSHtextFormat()
        {
            //assing
            var model = new ModelData();

            var loader = new Import_Mesh();       
            loader.LoadEvent += NewMesh_LoadEvent;

            //act
            //var sr = new StreamReader(@"C:\BazisXN\Model\GMSHLoader\test3");
            var modelINP = loader.Load(@"C:\BazisXN\GMSH_Importer(inp)\mesh\test3.inp");
            model.Load(modelINP);

            //assert
            var elems = model.ObjectData.FindMany<Element3D>().ToList();
        }

        private void NewMesh_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new System.NotImplementedException();
        }
    }
}
