using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.IO;
using Model.Interfaces;
using System.Data;
using Model.IO.STL;
using static System.Net.WebRequestMethods;

namespace TestModeCore_test.Model
{
    [TestClass]
    public class TestIO
    {


        [TestMethod]
        public void SaveModelToBpfTextFormat()
        {
            //assing
            var model = new ModelData();

            model.Saver.SaveEvent += (ar1, ar2) => { };

            //act
            model.Load(@"..\..\..\Examples\3D\model_with_groups.bpf");
            model.Save(@"..\..\..\Examples\3D\model_with_groups_save.bpf");

            //assert
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
