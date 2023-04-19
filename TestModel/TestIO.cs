
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Events;
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
            var loader = new LoadModelFromGMSHTextFile();
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



        private void NewMesh_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new System.NotImplementedException();
        }
    }
}
