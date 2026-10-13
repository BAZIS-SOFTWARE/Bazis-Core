using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.IO;
using Model.Interfaces;
using System.Data;
using Model.IO.STL;

namespace TestProject_test.Mesh.IO
{
    [TestClass]
    public class ProjectControllerMeshIOTests
    {
        [DataTestMethod]
        [DataRow(@"../../../../TestModelCore/Examples/3D/cub_unsorted.bpf",6)]
        [DataRow(@"../../../../TestModelCore/Examples/3D/hexa.bpf", 3)]
        public void LoadModelFromBpfTextFormat(string path, int elCount)
        {
            //assing
            var controller = new global::OperationalController.ProjectController();

            //act
            controller.Load(path);

            //assert
            Assert.AreEqual(controller.GetAllModelElements().Count(), elCount);
        }

        [DataTestMethod]
        [DataRow(@"../../../../TestModelCore/Examples/3D/box2.inp")]
        public void SaveModelToBpf2FromInp(string loadPath)
        {
            var savePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.asc");
            //assing
            var controller = new global::OperationalController.ProjectController();

            //act
            controller.ImportMesh(loadPath);

            var node = controller.GetModelObjects(ObjType.Узел).Count();
            try
            {
                controller.ExportMesh(savePath);
                controller.ImportMesh(savePath);
                var loadedNodeCount = controller.GetModelObjects(ObjType.Узел).Count();
                Assert.AreEqual(node, loadedNodeCount);
            }
            finally
            {
                File.Delete(savePath);
            }
        }

        [DataTestMethod]
        [DataRow(@"../../../../TestModelCore/Examples/BPF2TXT/cub_unsorted.bpf2", 2, ObjType.Элемент3D, DisplayName = "Sets Элемент3D = 2")]
        [DataRow(@"../../../../TestModelCore/Examples/BPF2TXT/cub_unsorted.bpf2", 8, ObjType.Узел, DisplayName = "Objects Узел = 1")]
        [DataRow(@"../../../../TestModelCore/Examples/BPF2TXT/cub_unsorted_oneSet.bpf2", 1, ObjType.Элемент3D, DisplayName = "Sets Элемент3D = 1")]
        [DataRow(@"../../../../TestModelCore/Examples/BPF2TXT/cub_mix_numbers.bpf2", 2, ObjType.Элемент3D, DisplayName = "Sets mix numbers Элемент3D = 2")]
        
        public void LoadModelFromBpf2TextFormat(string path, int sets, ObjType objType)
        {
            //assing
            var controller = new global::OperationalController.ProjectController();

            //act
            controller.Load(path);

            //assert
            switch (objType)
            {
                case ObjType.Точка:
                    break;
                case ObjType.Кривая:
                    break;
                case ObjType.Поверхность:
                    break;
                case ObjType.Узел:
                    Assert.AreEqual(controller.GetModelObjects(ObjType.Узел).Count(), sets);
                    break;
                case ObjType.Элемент1D:
                    Assert.AreEqual(controller.GetModelSetsInfo(ObjType.Элемент1D).Count(), sets);
                    break;
                case ObjType.Элемент2D:
                    Assert.AreEqual(controller.GetModelSetsInfo(ObjType.Элемент2D).Count(), sets);
                    break;
                case ObjType.Элемент3D:
                    Assert.AreEqual(controller.GetModelSetsInfo(ObjType.Элемент3D).Count(), sets);
                    break;
                default:
                    break;
            }
        }

        [TestMethod]
        [DataRow(new string[] { "Box_1.stl", "Box_2.stl", @"../../../../TestModelCore/Examples/STL/Append" },DisplayName = "Box_1_Box_2")]
        [DataRow(new string[] { "Box.stl", "Large_Box3.stl", "Sticks.stl", @"../../../../TestModelCore/Examples/STL/Append" }, DisplayName = "Box_Large_Box3_Sticks")]
        public void AppendAndSaveToSTLTextFormat(string [] files)
        {
            string dir = files.Last();

            //assing
            var controller = new global::OperationalController.ProjectController();

            var ext = Path.GetExtension(files[0]);
            //model.Loader = new LoadFromSTLFile();

            //act

            if(ext == ".bpf2")
                controller.Load(Path.Combine(dir, files[0]));
            else
                controller.ImportMesh(Path.Combine(dir, files[0]));
            
            foreach (var item in files.Skip(1).SkipLast(1))
                controller.Append(Path.Combine(dir, item));


            //assert
            Assert.AreNotEqual(controller.GetModelObjects(ObjType.Элемент2D).Count(), 0);

            var outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.stl");
            try
            {
                controller.ExportMesh(outputPath);
                controller.ImportMesh(outputPath);
                Assert.AreNotEqual(controller.GetModelObjects(ObjType.Элемент2D).Count(), 0);
            }
            finally
            {
                File.Delete(outputPath);
            }

        }

        [TestMethod]
        [DataRow(@"block_allElements3D_25538.stl", DisplayName = "block_allElements3D_25538.stl")]
        [DataRow(@"block.stl", DisplayName = "block.stl")]
        [DataRow(@"Деталь_пример.stl", DisplayName = "Деталь_пример.stl")]
        public void ImportAndSaveToSTLBinFormat(string file)
        {
            string dir = @"../../../../TestModelCore/Examples/STL";
            //string dir = @"../../../../Тестовые заготовки деталей/";
            var controller = new global::OperationalController.ProjectController();

            var path = Path.Combine(dir, file);
            //assing
            //var model = new ModelData();

            //model.Loader = new LoadFromSTLFile();

            //act
            controller.ImportMesh(path);
            var e2D_Count = controller.GetModelObjects(ObjType.Элемент2D).Count();
            //assert
            Assert.AreNotEqual(e2D_Count, 0);

            var outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.bin.stl");
            try
            {
                controller.ExportMesh(outputPath);
                controller.ImportMesh(outputPath);
                Assert.AreNotEqual(e2D_Count, 0);
            }
            finally
            {
                File.Delete(outputPath);
            }
            //var model3 = new ModelData();
            //model3.Loader = new LoadModelFromSTLFile();
            //model3.Load(dir + $"\\{file}.txt.stl");

        }

        [DataTestMethod]
        [DataRow(8448, 17964,0, @"../../../../TestModelCore/Examples/3D/Korpus.inp", DisplayName = "Korpus.inp")]
        [DataRow(4, 1,0, @"../../../../TestModelCore/Examples/3D/abacus.inp", DisplayName = "abacus.inp")]
        [DataRow(1000, 5454, 0, @"../../../../TestModelCore/Examples/3D/box2.inp", DisplayName = "box2.inp")]
        public void ImportModelFromINPTextFormat(int nodeCount, int elemCount, int grCount,string path)
        {
            var controller = new global::OperationalController.ProjectController();

            controller.ImportMesh(path);

            Assert.AreEqual(nodeCount, controller.GetModelObjects(ObjType.Узел).Count());
            Assert.AreEqual(elemCount, controller.GetAllModelElements().Count());
        }
    }
}
