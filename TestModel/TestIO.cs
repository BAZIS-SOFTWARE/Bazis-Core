using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.MeshObjects;
using Model.IO;
using ModelInterfaces;

namespace TestModel
{
    [TestClass]
    public class TestIO
    {
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\3D\cub_unsorted.bpf",6)]
        [DataRow(@"c:\projs\6\6.bpf", 65502)]
        public void OpenModelToBpfTextFormat(string path, int elCount)
        {
            //assing
            var model = new ModelData();

            model.Loader = new LoadModelFromProjectTextFile();

            model.Loader.LoadEvent += (ar1, ar2) => { };

            //act
            model.Load(path);

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

            model.Loader = new LoadModelFromGMSHTextFile();
            model.Saver = new SaveModelToBPF2TextFile();

            model.Loader.LoadEvent += (ar1, ar2) => { };
            model.Saver.SaveEvent += (ar1, ar2) => { };

            //act
            model.Load(@"..\..\..\Examples\3D\box2.inp");
            model.Save(@"..\..\..\Examples\3D\task1.bpf2");

            //assert
        }


        [TestMethod]
        [DataRow(@"Блок каналов\block.stl")]
        [DataRow(@"Блок каналов\block_solid_sup.stl")]
        [DataRow(@"Форсунка\1n.stl")]
        [DataRow(@"Форсунка\nozzle.stl")]
        [DataRow(@"Форсунка\nozzle_sup.stl")]
        [DataRow(@"Образец\specimen.stl")]
        [DataRow(@"Образец\specimen_solid_sup.stl")]
        [DataRow(@"Образец\specimen_wall_sup.stl")]
        [DataRow(@"Кронштейн\bracket.stl")]
        [DataRow(@"Кронштейн\bracket_solid_sup.stl")]
        [DataRow(@"Кронштейн\bracket_wall_sup.stl")]
        public void ImportModelFromSTLtextFormat(string file)
        {
            string dir = @"\\fs\Plm-Ural\OrgUnits\CAE\+ESI_QForm\+SIMMAX\Тестовые заготовки деталей\";
            //string dir = @"..\..\..\..\Тестовые заготовки деталей\";

            var path = Path.Combine(dir, file);
            //assing
            var model = new ModelData();

            model.Loader = new LoadModelFromSTLFile();

            //act
            model.Load(path);

            //assert
        }

        [TestMethod]
        [DataRow(@"Блок каналов\block.stl")]
        [DataRow(@"Блок каналов\block_solid_sup.stl")]
        [DataRow(@"Форсунка\1n.stl")]
        [DataRow(@"Форсунка\nozzle.stl")]
        [DataRow(@"Форсунка\nozzle_sup.stl")]
        [DataRow(@"Образец\specimen.stl")]
        [DataRow(@"Образец\specimen_solid_sup.stl")]
        [DataRow(@"Образец\specimen_wall_sup.stl")]
        [DataRow(@"Кронштейн\bracket.stl")]
        [DataRow(@"Кронштейн\bracket_solid_sup.stl")]
        [DataRow(@"Кронштейн\bracket_wall_sup.stl")]
        public void ImportExportModelFromSTLtextFormat(string file)
        {
            string inputDir = @"\\fs\Plm-Ural\OrgUnits\CAE\+ESI_QForm\+SIMMAX\Тестовые заготовки деталей\";
            //string inputDir = @"..\..\..\..\Тестовые заготовки деталей\";
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

        [TestMethod]
        public void ImportModelFromGMSHtextFormat()

        {
            var model = new ModelData();
            model.Loader = new LoadModelFromGMSHTextFile();

            model.Load(@"..\..\..\Examples\3D\Korpus.inp");

            Assert.AreEqual(8448, model.ObjectData.NodeCollection.Count);
        }

        [TestMethod]
        public void LoadModelFromGMSHTextFile_ShouldLoadCorrectNumberOfElements()
        {
            var model = new ModelData();
            model.Loader = new LoadModelFromGMSHTextFile();

            model.Load(@"..\..\..\Examples\3D\Korpus.inp");

            Assert.AreEqual(17964, model.ObjectData.GetAllElements().Count());
        }

        [TestMethod]
        public void ImportModelFromVisualMeshASCIITextFormat()
        {
            //assing
            var model = new ModelData();
            model.Loader = new LoadModelFromASCIITextFile();

            model.Loader.LoadEvent += (ar1, ar2) => { };

            //act
            model.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");
            //loader.Load(@"C:\Bazis_newGUI\MESH\RadicMesh\Plate2DATA.ASC");
            //assert
            Assert.AreEqual(7, model.ObjectData.Count(ObjType.Узел));
            Assert.AreEqual(8, model.ObjectData.Count(ObjType.Элемент));
        }

        [TestMethod]
        public void LoadASCII_ShouldImport1DElements()
        {
            var loader = new LoadModelFromASCIITextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            var model = loader.Load(@"..\..\..\Examples\VIsual-Mesh\DATA3.ASC");
            Assert.AreEqual(3, model.ObjectData.Count(ObjType.Элемент1D));
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