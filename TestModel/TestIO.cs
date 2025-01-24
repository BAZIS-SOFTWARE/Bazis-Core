using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.IO;
using Model.Interfaces;

namespace TestModel
{
    [TestClass]
    public class TestIO
    {
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\3D\cub_unsorted.bpf",6)]
        [DataRow(@"c:\projs\AdditiveTest\nozzle.stlout_penta.bpf", 65606)]
        [DataRow(@"c:\projs\AdditiveTest\nozzle.stlout_penta_beam.bpf", 67245)]
        [DataRow(@"c:\projs\AdditiveTest\nozzle.stlout_penta_beam_v.bpf", 67245)]
        [DataRow(@"c:\projs\AdditiveTest\cube_rotated.bpf", 11)]
        public void LoadModelFromBpfTextFormat(string path, int elCount)
        {
            //assing
            var model = new ModelData();

            model.Loader = new LoadModelFromBPFTextFile();

            model.Loader.LoadEvent += (ar1, ar2) => { };

            //act
            model.Load(path);

            var pentas = model.ObjectData.E3DCollection.Where(x => x.ElementKind == Model.Interfaces.MeshObjects.ElementKind.Пента);

            //assert
            Assert.AreEqual(model.ObjectData.Count(ObjType.Элемент), elCount);
        }

        [TestMethod]
        public void OpenModelToBpf2TextFormat()
        {
            //assing
            var model = new ModelData();

            model.Loader = new LoadModelFromBPF2TextFile();

            model.Loader.LoadEvent += (ar1, ar2) => { };

            //act
            model.Load(@"..\..\..\Examples\3D\task1.bpf2");

            //assert
            Assert.AreNotEqual(model.ObjectData.Count(ObjType.Объект), 0);
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

        [TestMethod]
        public void SaveModelToBpf2TextFormat()
        {
            //assing
            var model = new ModelData();

            model.Loader = new LoadModelFromINPTextFile();
            model.Saver = new SaveModelToBPF2TextFile();

            model.Loader.LoadEvent += (ar1, ar2) => { };
            model.Saver.SaveEvent += (ar1, ar2) => { };

            //act
            model.Load(@"..\..\..\Examples\3D\box2.inp");
            model.Save(@"..\..\..\Examples\3D\task1.bpf2");

            //assert
        }


        [TestMethod]
        [DataRow(@"block_allElements3D_25538.stl", DisplayName = "block_allElements3D_25538.stl")]
        [DataRow(@"block.stl", DisplayName = "block.stl")]
        //[DataRow(@"Форсунка\1n.stl")]
        //[DataRow(@"Форсунка\nozzle.stl")]
        //[DataRow(@"Форсунка\nozzle_sup.stl")]
        //[DataRow(@"Образец\specimen.stl")]
        //[DataRow(@"Образец\specimen_solid_sup.stl")]
        //[DataRow(@"Образец\specimen_wall_sup.stl")]
        //[DataRow(@"Кронштейн\bracket.stl")]
        //[DataRow(@"Кронштейн\bracket_solid_sup.stl")]
        //[DataRow(@"Кронштейн\bracket_wall_sup.stl")]
        public void ImportModelFromSTLtextFormat(string file)
        {
            string dir = @"..\..\..\Examples\STL";
            //string dir = @"..\..\..\..\Тестовые заготовки деталей\";

            var path = Path.Combine(dir, file);
            //assing
            var model = new ModelData();

            model.Loader = new LoadModelFromSTLFile();

            //act
            model.Load(path);

            //assert
            Assert.AreNotEqual(model.ObjectData.E2DCollection.Count, 0);
            //Assert.AreNotEqual(model.GroupData.Count, 0);
        }

        [TestMethod]
        [DataRow(@"Блок каналов\block.stl")]
        //[DataRow(@"Блок каналов\block_solid_sup.stl")]
        //[DataRow(@"Форсунка\1n.stl")]
        //[DataRow(@"Форсунка\nozzle.stl")]
        //[DataRow(@"Форсунка\nozzle_sup.stl")]
        //[DataRow(@"Образец\specimen.stl")]
        //[DataRow(@"Образец\specimen_solid_sup.stl")]
        //[DataRow(@"Образец\specimen_wall_sup.stl")]
        //[DataRow(@"Кронштейн\bracket.stl")]
        //[DataRow(@"Кронштейн\bracket_solid_sup.stl")]
        //[DataRow(@"Кронштейн\bracket_wall_sup.stl")]
        public void ImportExportModelFromSTLtextFormat(string file)
        {
            string inputDir = @"\\fs\Plm-Ural\OrgUnits\CAE\+ESI_QForm\+SIMMAX\Тестовые заготовки деталей\";
            string outputDir = @"..\..\..\..\Тестовые заготовки деталей (test)\";

            var filePath = Path.Combine(inputDir, file);
            var outpath = Path.Combine(outputDir, file);

            //assing
            var model = new ModelData();

            model.Loader = new LoadModelFromSTLFile();
            model.Load(filePath);

            Directory.CreateDirectory(Path.GetDirectoryName(outpath));

            model.Saver = new SaveModelToBinarySTLFile();
            model.Save(outpath + ".bin.stl");

            model.Saver = new SaveModelToTxtSTLFile();
            model.Save(outpath + ".txt.stl");

            var model2 = new ModelData();
            model2.Loader = new LoadModelFromSTLFile();
            model2.Load(outpath + ".bin.stl");

            var model3 = new ModelData();
            model3.Loader = new LoadModelFromSTLFile();
            model3.Load(outpath + ".txt.stl");
            //assert
        }

        [DataTestMethod]
        [DataRow(8448, 17964, @"..\..\..\Examples\3D\Korpus.inp", DisplayName = "Korpus.inp")]
        [DataRow(4, 1, @"..\..\..\Examples\3D\abacus.inp", DisplayName = "abacus.inp")]
        public void ImportModelFromINPTextFormat(int nodeCount, int elemCount, string path)

        {
            var model = new ModelData();
            model.Loader = new LoadModelFromINPTextFile();

            model.Load(path);

            Assert.AreEqual(nodeCount, model.ObjectData.NodeCollection.Count);
            Assert.AreEqual(elemCount, model.ObjectData.GetAllElements().Count());
        }

        [DataTestMethod]
        [DataRow(7,8,0, @"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC", DisplayName = "rhombDATA1.ASC")]
        [DataRow(44, 113, 3, @"..\..\..\Examples\VIsual-Mesh\DATA3.ASC", DisplayName = "DATA3.ASC")]
        
        public void ImportModelFromVisualMeshASCIITextFormat(int nodeCount, int elemCount, int elem1D, string path)
        {
            //assing
            var loader = new LoadModelFromASCIITextFile();
            loader.LoadEvent += (ar1, ar2) => { };

            //act
            var model = loader.Load(path);
            //loader.Load(@"C:\Bazis_newGUI\MESH\RadicMesh\Plate2DATA.ASC");
            //assert
            Assert.AreEqual(nodeCount, model.ObjectData.Count(ObjType.Узел));
            Assert.AreEqual(elemCount, model.ObjectData.Count(ObjType.Элемент));
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
            Assert.IsTrue(modelBeforeSaving.ObjectData.Count(ObjType.Объект) == 
                modelAfterSaving.ObjectData.Count(ObjType.Объект));
        }

        [TestMethod]
        public void ImportHexFromCDBTextFormat()
        {
            string path = @"..\..\..\Examples\CDB\hex.cdb";
            var import = new LoadModelFromCDBTextFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(path);
            var element = model.ObjectData.E3DCollection.First();
            Assert.AreEqual(8, model.ObjectData.Count(ObjType.Узел));
            Assert.AreEqual(1, model.ObjectData.Count(ObjType.Элемент));
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
            var elements = model.ObjectData.E3DCollection;
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
            var elements = model.ObjectData.E3DCollection;
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
            Assert.AreEqual(nbrEl, model.ObjectData.Count(ObjType.Элемент));
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\Salome\Gear_V1.dat",3000,976, 4832,9600)]
        [DataRow(@"..\..\..\Examples\Salome\Gear_V2.dat",3161, 992, 3641, 5255)]
        public void LoadMeshFromSalomeFileCorrectly(string filePath, int nodes, int lineElms, int surfElms, int volElems)
        {
            var import = new LoadModelFromSalomeFile();
            import.LoadEvent += (ar1, ar2) => { };
            var model = import.Load(filePath);
            Assert.AreEqual(nodes, model.ObjectData.NodeCollection.Distinct().Count());
            Assert.AreEqual(lineElms, model.ObjectData.E1DCollection.Distinct().Count());
            Assert.AreEqual(surfElms, model.ObjectData.E2DCollection.Distinct().Count());
            Assert.AreEqual(volElems, model.ObjectData.E3DCollection.Distinct().Count());
        }
    }
}