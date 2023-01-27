using System.IO;
using System.Linq;
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

            model.SetLoader(loader);
            model.LoadEvent += NewMesh_LoadEvent;
            //var reader = new StreamReader(@"C:\Bazis_newGUI\MESH\big_cubeDATA1.ASC");

            var reader = new StreamReader(@"C:\Bazis_newGUI\MESH\RadicMesh\Plate2DATA.ASC");


            //act
            model.Load(@"C:\Bazis_newGUI\MESH\RadicMesh\Plate2DATA.ASC");

            //assert
            var elems = model.ObjectData.FindMany<Element3D>().ToList();
        }

        [TestMethod]
        public void LoadModelFromBpfTextFormat()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromProjectTextFile();
            model.SetLoader(loader);

            model.LoadEvent += NewMesh_LoadEvent;
            //act
            //model.Load(@"C:\Bazis_newGUI\PROJ\Test_3D\cub_1.bpf");
            model.Load(@"C:\Projs\TestProjTernMech\twoElements\two_tetra_termoMech_calc1.bpf");
            //assert
        }

        [TestMethod]
        public void SaveModelToBpfTextFormat()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromGMSH_InpFile();
            model.SetLoader(loader);

            model.LoadEvent += NewMesh_LoadEvent;
            var saver = new SaveModelToProjectTextFile();
            model.SetSaver(saver);

            model.SaveEvent += Model_SaveEvent;
            //act
            model.Load(@"C:\Projs\TestProjTerm\box\box1.inp");

            model.Save(@"C:\Projs\TestProjTerm\box\task_1.bpf");

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
            model.Load(@"C:\BazisXN\Model\STLLoader\ACII.stl");
            //assert
        }

        [TestMethod]
        public void ImportModelFromGMSHtextFormat()
        {
            //assing
            var model = new ModelData();

            var loader = new LoadModelFromGMSH_InpFile();
            model.SetLoader(loader);

            model.LoadEvent += NewMesh_LoadEvent;

            //act
            //var sr = new StreamReader(@"C:\BazisXN\Model\GMSHLoader\test3");
            model.Load(@"C:\BazisXN\GMSH_Importer(inp)\mesh\test3.inp");
            

            //assert
            var elems = model.ObjectData.FindMany<Element3D>().ToList();
        }

        private void NewMesh_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new System.NotImplementedException();
        }
    }
}
