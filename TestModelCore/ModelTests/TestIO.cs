using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.IO;
using Model.Interfaces;
using System.Data;
using Model.IO.STL;
using static System.Net.WebRequestMethods;

namespace ModelTests
{
    [TestClass]
    public class TestIO
    {
        [TestMethod]
        public void AppendReturnsRenamedModelSets()
        {
            var path = @"..\..\..\Examples\BPF2TXT\cub_unsorted_oneSet.bpf2";
            var controller = (OperationalController.IProjectController)new OperationalController.ProjectController();
            controller.Load(path);
            var originalName = controller.GetModelSetsInfo(ObjType.Элемент3D).Single().Name;

            var appendedNames = controller.Append(path);

            CollectionAssert.AreEqual(
                new[] { "Узел", originalName + "_1", "Точка", "Кривая", "Поверхность" },
                appendedNames.ToArray());
            CollectionAssert.AreEquivalent(
                new[] { originalName, originalName + "_1" },
                controller.GetModelSetsInfo(ObjType.Элемент3D).Select(set => set.Name).ToArray());
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\3D\cub_unsorted.bpf",6)]
        [DataRow(@"..\..\..\Examples\3D\hexa.bpf", 3)]
        //[DataRow(@"c:\projs\AdditiveTest\nozzle.stlout_penta.bpf", 65606)]
        //[DataRow(@"c:\projs\AdditiveTest\nozzle.stlout_penta_beam.bpf", 67245)]
        //[DataRow(@"c:\projs\AdditiveTest\nozzle.stlout_penta_beam_v.bpf", 67245)]
        //[DataRow(@"c:\projs\AdditiveTest\cube_rotated.bpf", 11)]
        public void LoadModelFromBpfTextFormat(string path, int elCount)
        {
            //assing
            var controller = new OperationalController.ProjectController();

            //act
            controller.Load(path);

            //assert
            Assert.AreEqual(controller.GetAllModelElements().Count(), elCount);
        }

        [TestMethod]
        public void SaveModelToBpfTextFormat()
        {
            //assing
            var model = new ModelData();

            model.Loader.LoadEvent += (ar1, ar2) => { };
            model.Saver.SaveEvent += (ar1, ar2) => { };

            //act
            model.Load(@"..\..\..\Examples\3D\model_with_groups.bpf");
            model.Save(@"..\..\..\Examples\3D\model_with_groups_save.bpf");

            //assert
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\3D\box2.inp", @"..\..\..\Examples\BPF2TXT\box2_test.asc")]
        public void SaveModelToBpf2FromInp(string loadPath, string savePath)
        {
            //assing
            var controller = new OperationalController.ProjectController();

            //act
            controller.ImportMesh(loadPath);

            var node = controller.GetModelObjects(ObjType.Узел).Count();
            var elem1D = controller.GetModelSetsInfo(ObjType.Элемент1D).Count();
            var elem2D = controller.GetModelSetsInfo(ObjType.Элемент2D).Count();
            var elem3D = controller.GetModelSetsInfo(ObjType.Элемент3D).Count();

            controller.ExportMesh(savePath);
            controller.ImportMesh(savePath);

            var node_l = controller.GetModelObjects(ObjType.Узел).Count();
            var elem1D_l = controller.GetModelSetsInfo(ObjType.Элемент1D).Count();
            var elem2D_l = controller.GetModelSetsInfo(ObjType.Элемент2D).Count();
            var elem3D_l = controller.GetModelSetsInfo(ObjType.Элемент3D).Count();

            //assert

            Assert.AreEqual(node, node_l);
            //Assert.AreEqual(elem1D, elem1D_l);
            //Assert.AreEqual(elem2D, elem2D_l);
            //Assert.AreEqual(elem3D, elem3D_l);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\BPF2TXT\cub_unsorted.bpf2", 2, ObjType.Элемент3D, DisplayName = "Sets Элемент3D = 2")]
        [DataRow(@"..\..\..\Examples\BPF2TXT\cub_unsorted.bpf2", 8, ObjType.Узел, DisplayName = "Objects Узел = 1")]
        [DataRow(@"..\..\..\Examples\BPF2TXT\cub_unsorted_oneSet.bpf2", 1, ObjType.Элемент3D, DisplayName = "Sets Элемент3D = 1")]
        [DataRow(@"..\..\..\Examples\BPF2TXT\cub_mix_numbers.bpf2", 2, ObjType.Элемент3D, DisplayName = "Sets mix numbers Элемент3D = 2")]
        //[DataRow(@"c:\projs\AdditiveTest\sliceMeshes\лопатка-2D-3D.bpf2", 385, ObjType.Элемент3D, DisplayName = "Sets Элемент3D > 100")]
        //[DataRow(@"c:\projs\AdditiveTest\sliceMeshes\rotatedBlock-1.bpf2", 171, ObjType.Элемент3D, DisplayName = "Sets Элемент3D > 1000")]
        
        public void LoadModelFromBpf2TextFormat(string path, int sets, ObjType objType)
        {
            //assing
            var controller = new OperationalController.ProjectController();

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
        //[DataRow(new string[] { "c:\\projs\\AdditiveTest\\MeshWithBounds\\новый_проект.bpf2", "Box_2.stl" }, DisplayName = "Mix")]
        [DataRow(new string[] { "Box_1.stl", "Box_2.stl", @"..\..\..\Examples\STL\Append" },DisplayName = "Box_1_Box_2")]
        [DataRow(new string[] { "Box.stl", "Large_Box3.stl", "Sticks.stl", @"..\..\..\Examples\STL\Append" }, DisplayName = "Box_Large_Box3_Sticks")]
        [DataRow(new string[] { "out_m3.bpf2", "cube_2d_and_3d_support.bpf2", @"c:\projs\AdditiveTest\TestNewMeshGen" }, DisplayName = "Merge_two_bpf2")]
        public void AppendAndSaveToSTLTextFormat(string [] files)
        {
            string dir = files.Last();

            //assing
            var controller = new OperationalController.ProjectController();

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

            controller.ExportMesh(Path.Combine(dir, "append.stl"));
            controller.ImportMesh(Path.Combine(dir, "append.stl"));

            Assert.AreNotEqual(controller.GetModelObjects(ObjType.Элемент2D).Count(), 0);

        }   


        [TestMethod]
        [DataRow(@"block_allElements3D_25538.stl", DisplayName = "block_allElements3D_25538.stl")]
        [DataRow(@"block.stl", DisplayName = "block.stl")]
        [DataRow(@"Деталь_пример.stl", DisplayName = "Деталь_пример.stl")]
        //[DataRow(@"Форсунка\1n.stl")]
        //[DataRow(@"Форсунка\nozzle.stl")]
        //[DataRow(@"Форсунка\nozzle_sup.stl")]
        //[DataRow(@"Образец\specimen.stl")]
        //[DataRow(@"Образец\specimen_solid_sup.stl")]
        //[DataRow(@"Образец\specimen_wall_sup.stl")]
        //[DataRow(@"Кронштейн\bracket.stl")]
        //[DataRow(@"Кронштейн\bracket_solid_sup.stl")]
        //[DataRow(@"Кронштейн\bracket_wall_sup.stl")]
        public void ImportAndSaveToSTLBinFormat(string file)
        {
            string dir = @"..\..\..\Examples\STL";
            //string dir = @"..\..\..\..\Тестовые заготовки деталей\";
            var controller = new OperationalController.ProjectController();

            var path = Path.Combine(dir, file);
            //assing
            //var model = new ModelData();

            //model.Loader = new LoadFromSTLFile();

            //act
            controller.ImportMesh(path);
            var e2D_Count = controller.GetModelObjects(ObjType.Элемент2D).Count();
            //assert
            Assert.AreNotEqual(e2D_Count, 0);

            //model.Saver = new SaveModelToBinarySTLFile();
            controller.ExportMesh(dir + $"\\{file}.bin.stl");

            //model.Saver = new SaveModelToTxtSTLFile();
            //model.Save(dir + $"\\{file}.txt.stl");

            //var model2 = new ModelData();
            //model.Loader = new LoadModelFromSTLFile();
            controller.ImportMesh(dir + $"\\{file}.bin.stl");

            Assert.AreNotEqual(e2D_Count, 0);
            //var model3 = new ModelData();
            //model3.Loader = new LoadModelFromSTLFile();
            //model3.Load(dir + $"\\{file}.txt.stl");

        } 

        [DataTestMethod]
        [DataRow(8448, 17964,0, @"..\..\..\Examples\3D\Korpus.inp", DisplayName = "Korpus.inp")]
        [DataRow(4, 1,0, @"..\..\..\Examples\3D\abacus.inp", DisplayName = "abacus.inp")]
        [DataRow(1000, 5454, 0, @"..\..\..\Examples\3D\box2.inp", DisplayName = "box2.inp")]
        public void ImportModelFromINPTextFormat(int nodeCount, int elemCount, int grCount,string path)
        {
            var controller = new OperationalController.ProjectController();

            controller.ImportMesh(path);

            Assert.AreEqual(nodeCount, controller.GetModelObjects(ObjType.Узел).Count());
            Assert.AreEqual(elemCount, controller.GetAllModelElements().Count());
        }

        [DataTestMethod]
        [DataRow(7,8,0, @"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC", DisplayName = "rhombDATA1.ASC")]
        [DataRow(44, 113, 3, @"..\..\..\Examples\VIsual-Mesh\DATA3.ASC", DisplayName = "DATA3.ASC")]
        [DataRow(104, 337, 0, @"..\..\..\Examples\VIsual-Mesh\cilinder3DATA1.ASC", DisplayName = "cilinder3DATA1.ASC")]
        [DataRow(104, 337, 0, @"..\..\..\Examples\VIsual-Mesh\cilinder3DATA1.ASC", DisplayName = "cilinder3DATA1.ASC")]
        [DataRow(145286, 193532, 2380, @"c:\projs\Kamaz_ini\iniMesh_DATA3.ASC", DisplayName = "stress test")]
        public void ImportModelFromVisualMeshASCIITextFormat(int nodeCount, int elemCount, int elem1D, string path)
        {
            //assing
            var loader = new LoadModelFromASCIITextFile_v2();
            loader.LoadEvent += (ar1, ar2) => { };

            //act
            var model = loader.Load(path);
            //loader.Load(@"C:\Bazis_newGUI\MESH\RadicMesh\Plate2DATA.ASC");
            //assert
            Assert.AreEqual(nodeCount, model.ObjectData.Count(ObjType.Узел));
            Assert.AreEqual(elemCount, model.ObjectData.GetAllElements().Count());
            Assert.AreEqual(elem1D, model.ObjectData.Count(ObjType.Элемент1D));
        }

        [TestMethod]
        public void SaveToASCII_ShouldBeEquivalentSourceFile()
        {
            var loader = new LoadModelFromASCIITextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            var modelBeforeSaving = loader.Load(@"..\..\..\Examples\VIsual-Mesh\DATA3.ASC");
            var saver = new SaveModelToASCIITextFile();
            saver.Save(modelBeforeSaving, @"..\..\..\Examples\VIsual-Mesh\result.ASC");
            var modelAfterSaving = loader.Load(@"..\..\..\Examples\VIsual-Mesh\result.ASC");
            Assert.IsTrue(modelBeforeSaving.ObjectData.GetAllObjects().Count() == 
                modelAfterSaving.ObjectData.GetAllObjects().Count());
        }

        [TestMethod]
        public void ImportHexFromCDBTextFormat()
        {
            string path = @"..\..\..\Examples\CDB\hex.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            var element = model.ObjectData.E3DCollection.GetObjects().First();
            Assert.AreEqual(8, model.ObjectData.Count(ObjType.Узел));
            Assert.AreEqual(1, model.ObjectData.GetAllElements().Count());
            var surfaceNumber = element.NumberOfSides;
            Assert.AreEqual(6, surfaceNumber);
        }

        [TestMethod]
        public void ImportLongModelFromCDBTextFormat()
        {
            string path = @"..\..\..\Examples\CDB\longModel.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            Assert.AreEqual(3321, model.ObjectData.Count(ObjType.Узел));
            var elements = model.ObjectData.E3DCollection.GetObjects();
            Assert.IsTrue(elements.All(element => element.GetSurfaces().
            All(surface => surface.GetCoordinates().Count() > 2)));
        }

        [TestMethod]
        public void FindCorrectPlaneNumberOfElements()
        {
            string path = @"..\..\..\Examples\CDB\longModel.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            var elements = model.ObjectData.E3DCollection.GetObjects();
            Assert.IsTrue(elements.All(element => element.GetSurfaces().Count() >= 4));
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\CDB\penta.cdb", 6, 1)]
        [DataRow(@"..\..\..\Examples\CDB\tetra.cdb", 4, 1)]
        public void ImportPentaFromCDBTextFormat(string path, int nbrNodes, int nbrEl)
        {
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            var element = model.ObjectData.E3DCollection.First();
            Assert.AreEqual(nbrNodes, model.ObjectData.Count(ObjType.Узел));
            Assert.AreEqual(nbrEl, model.ObjectData.GetAllElements().Count());
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\Salome\Gear_V1.dat",3000,976, 4832,9600)]
        [DataRow(@"..\..\..\Examples\Salome\Gear_V2.dat",3161, 992, 3641, 5255)]
        public void LoadMeshFromSalomeFileCorrectly(string filePath, int nodes, int lineElms, int surfElms, int volElems)
        {
            var import = new LoadModelFromSalomeFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(filePath);
            Assert.AreEqual(nodes, model.ObjectData.NodesSet.Count);
            Assert.AreEqual(lineElms, model.ObjectData.E1DCollection.Count);
            Assert.AreEqual(surfElms, model.ObjectData.E2DCollection.Count);
            Assert.AreEqual(volElems, model.ObjectData.E3DCollection.Count);
        }

        [DataTestMethod]
        [DataRow(1000, 12, 6, 1, 0, @"..\..\..\Examples\3D\box2.inp")]
        //[DataRow(8, 12, 6, 1, 7, @"..\..\..\Examples\3D\Physical_groups_mesh.inp", DisplayName = "Physical_groups_mesh.inp")]
        public void ImportModelFromINPFormat_v2(int nodeCount, int elem1D, int elem2D, int elem3D, int countGroup, string path)
        {
            //assing
            var loader = new LoadModelFromINPTextFile_v2();
            loader.LoadEvent += (ar1, ar2) => { };

            //act
            var model = loader.Load(path);            

            //assert
            Assert.AreEqual(nodeCount, model.ObjectData.Count(ObjType.Узел));
            Assert.AreEqual(elem1D, model.ObjectData.E1DCollection.Count());
            Assert.AreEqual(elem2D, model.ObjectData.E2DCollection.Count());
            Assert.AreEqual(elem3D, model.ObjectData.E3DCollection.Count());
            Assert.AreEqual(countGroup, model.GroupData.Count());
        }
    }
}
