using GmshApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace GmshApiTests_test.GmshApi
{
    [TestClass]
    public class GmshMeshApiTests
    {
        private Gmsh gmsh = null!;
        private string modelFolder = GetExamplesPath();

        [TestInitialize]
        public void Init()
        {
            GmshKernel.DLL = GmshKernel.Load(GetGmshLibraryPath());
            gmsh = new Gmsh();
            gmsh.Initialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            gmsh.Clear();
            gmsh.finalize();
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow(@"\CAD\nasos.step", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow(@"\CAD\block.igs", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        [DataRow(@"\GMSHAPI\sphere.geo", new int[] { -1, 1, 2 }, new int[] { 0, 1, 2 })]
        public void GenerateMeshGetAllNodesTest(string fileName, int[] nodesDim, int[] paramsCount)
        {
            gmsh.Open(modelFolder + fileName);
            gmsh.Model.Mesh.Generate(2);
            for (var i = 0; i < nodesDim.Length; ++i)
            {
                var data = gmsh.Model.Mesh.GetNodes(nodesDim[i], -1, true, true);
                if (nodesDim[i] == -1)
                    Assert.AreEqual(0, data.Item3.Length);
                else
                {
                    Assert.AreEqual(0, data.Item2.Length % 3);
                    var totalPoints = data.Item2.Length / 3;
                    Assert.AreNotEqual(0, data.Item3.Length);
                    Assert.AreEqual(data.Item3.Length, totalPoints * paramsCount[i]);
                }
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", new int[] { 1, 2, 3 }, new int[] { 6, 10, 6 })]
        [DataRow(@"\CAD\nasos.step", new int[] { 1, 2, 3 }, new int[] { 3, 3, 3 })]
        [DataRow(@"\CAD\block.igs", new int[] { 1, 2, 3 }, new int[] { 8, 2, 8 })]
        [DataRow(@"\GMSHAPI\sphere.geo", new int[] { 1, 2, 3 }, new int[] { 4, 4, 4 })]
        public void GenerateMeshGetLineNodesByTag(string fileName, int[] tags, int[] expectedLength)
        {
            gmsh.Open(modelFolder + fileName);
            gmsh.Model.Mesh.Generate(1);
            for (var i = 0; i < tags.Length; ++i)
            {
                var data = gmsh.Model.Mesh.GetNodes(1, tags[i], true, true);
                Assert.AreEqual(expectedLength[i], data.Item1.Length);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\nasos.step", new int[] { 170, 168, 166 }, new int[] { 4, 8, 8 })]
        [DataRow(@"\CAD\block.igs", new int[] { 5, 3, 12 }, new int[] { 4, 4, 9 })]
        public void GenerateMeshGetSurfacesNodesByTag(string fileName, int[] tags, int[] expectedLength)
        {
            gmsh.Open(modelFolder + fileName);
            gmsh.Model.Mesh.Generate(2);
            for (var i = 0; i < tags.Length; ++i)
            {
                var data = gmsh.Model.Mesh.GetNodes(2, tags[i], true, true);
                Assert.AreEqual(expectedLength[i], data.Item1.Length);
            }
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step", 77)]
        public void GenerateMeshGetElementsByCoordinates(string fileName, int expected)
        {
            gmsh.Open(modelFolder + fileName);
            gmsh.Model.Mesh.Generate(2);
            var point = new double[] { -8.74248599355, 1.39424515076, 6.51327932777 };
            var elements = gmsh.Model.Mesh.GetElementsByCoordinates(point[0], point[1], point[2], 2, false);
            Assert.AreEqual(expected, (int)elements[0]);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Cilindr.step")]
        public void AddRemoveFieldTest(string fileName)
        {
            gmsh.Open(modelFolder + fileName);
            var view = gmsh.Model.Mesh.Field.Add(FieldType.BoundaryLayer);
            var tags = gmsh.Model.Mesh.Field.List();
            CollectionAssert.AreEqual(tags, new int[] { view });
            gmsh.Model.Mesh.Field.Remove(view);
            tags = gmsh.Model.Mesh.Field.List();
            CollectionAssert.AreEqual(tags, new int[] { });
        }

        [DataTestMethod]
        [DataRow(@"\CAD\cilindr.step", 1)]
        [DataRow(@"\CAD\cilindr.step", 2)]
        [DataRow(@"\CAD\cilindr.step", 3)]
        [DataRow(@"\CAD\cilindr.step", 4)]
        public void CenterOfSurfaceTest(string fileName, int surfTag)
        {
            gmsh.Open(modelFolder + fileName);
            var data = gmsh.Model.Occ.GetCenterOfMass(2, surfTag);
            var coords = new double[] { data.Item1, data.Item2, data.Item3 };
            var status = gmsh.Model.IsInside(2, surfTag, coords);
            Assert.AreEqual(1, status);
            coords[1] -= 100;
            status = gmsh.Model.IsInside(2, surfTag, coords);
            Assert.AreEqual(0, status);
        }
        //Тест гарантирующий сброс трансфиниции кривой в состояние по умолчанию :)

        [DataTestMethod]
        [DataRow(@"\CAD\weld_seam_1.step", 38, new int[] { 18, 19, 23, 24 }, new int[] { 2 }, new double[] { 0.25, 0.05, 0.5 })]
        [DataRow(@"\CAD\weld_seam_2.step", 38, new int[] { 18, 19, 23, 24 }, new int[] { 2 }, new double[] { 0.25, 0.05, 0.5 })]
        [DataRow(@"\CAD\Tavr.step", 6, new int[] { 25, 27, 29, 31, 32, 33, 34, 36, 38, 40, 41, 42 }, new int[] { 3, 4 }, new double[] { 20, 10, 3.5, 3.5 })]
        public void VolumeSizesPointSizesCase(string fileName, int seamPointCurves, int[] seamCurves, int[] seamVolumes, double[] volumeSizes)
        {
            gmsh.Open(modelFolder + fileName);
            var volumes = gmsh.Model.GetEntities(3);
            //Вариант 1 - использование плотностей в точках, сначало генерируем плотности в точках, затем проводим трансфиницию (порядок важен!)
            for (int i = 0, j = 0; i < volumes.Length; i += 2, ++j)
            {
                var dimTags = new int[] { volumes[i], volumes[i + 1] };
                var points = gmsh.Model.GetBoundary(dimTags, true, true, true);
                gmsh.Model.Mesh.SetSize(points, volumeSizes[j]);
            }
            MakeTranfinite(seamPointCurves, seamCurves, seamVolumes);
            gmsh.Model.Mesh.Generate(3);
            var error = gmsh.Logger.GetLastError();
            Assert.IsTrue(string.IsNullOrEmpty(error));
            //var file = Path.GetFileNameWithoutExtension(fileName);
            //gmsh.Write($@"D:\{file}.inp");//Для визуального теста
        }

        [DataTestMethod]
        [DataRow(@"\CAD\weld_seam_1.step", 38, new int[] { 18, 19, 23, 24 }, new int[] { 2 }, new double[] { 0.25, 0.05, 0.5 })]
        [DataRow(@"\CAD\weld_seam_2.step", 38, new int[] { 18, 19, 23, 24 }, new int[] { 2 }, new double[] { 0.25, 0.05, 0.5 })]
        [DataRow(@"\CAD\Tavr.step", 6, new int[] { 25, 27, 29, 31, 32, 33, 34, 36, 38, 40, 41, 42 }, new int[] { 3, 4 }, new double[] { 20, 10, 3.5, 3.5 })]
        public void VolumeSizesMeshFieldsCase(string fileName, int seamPointCurves, int[] seamCurves, int[] seamVolumes, double[] volumeSizes)
        {
            gmsh.Open(modelFolder + fileName);
            var volumes = gmsh.Model.GetEntities(3);
            //Вариант 2 - использование Mesh Fields, сначало генерируем Mesh Fields, затем проводим трансфиницию (порядок важен!)
            for (int i = 0, j = 0; i < volumes.Length; i += 2, ++j)
            {
                var dimTags = new int[] { volumes[i], volumes[i + 1] };
                var field = gmsh.Model.Mesh.Field.Add(FieldType.Constant);
                gmsh.Model.Mesh.Field.SetNumbers(field, "VolumesList", new double[] { field });
                gmsh.Model.Mesh.Field.SetNumber(field, "VIn", volumeSizes[j]);
            }
            var list = gmsh.Model.Mesh.Field.List().Select(v => (double)v).ToArray();
            var baseField = gmsh.Model.Mesh.Field.Add(FieldType.Min);
            gmsh.Model.Mesh.Field.SetNumbers(baseField, "FieldsList", list);
            gmsh.Model.Mesh.Field.SetAsBackgroundMesh(baseField);

            MakeTranfinite(seamPointCurves, seamCurves, seamVolumes);

            gmsh.Model.Mesh.Generate(3);
            var error = gmsh.Logger.GetLastError();
            Assert.IsTrue(string.IsNullOrEmpty(error));
            //file = Path.GetFileNameWithoutExtension(fileName);
            //gmsh.Write($@"D:\{file}_field.inp");//Для визуального теста
        }

        [DataTestMethod]
        [DataRow("geo")]
        [DataRow("occ")]
        public void GetStringSettingsTest(string expected)
        {
            gmsh.Option.SetString("General.ScriptingLanguages", expected);

            var actual = gmsh.Option.GetString("General.ScriptingLanguages");
            Assert.IsTrue(expected == actual);
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Tavr.step", 0.58, 3.5, MeshAlgorithm3D.Delaunay, 1)]
        [DataRow(@"\CAD\cilindr.step", 0.33, 1.8, MeshAlgorithm3D.Frontal, 1.2)]
        [DataRow(@"\CAD\screw.step", 0.7, 1.7, MeshAlgorithm3D.HXT, 0.85)]
        [DataRow(@"\CAD\nasos.step", 10, 16, MeshAlgorithm3D.RTree, 1.8)]
        public void ChechMesh3DParameters(string fileName, double expMinSize, double expMaxSize, MeshAlgorithm3D expAlgo3d, double sizeFactor)
        {
            gmsh.Open(modelFolder + fileName);
            gmsh.Option.SetNumber("Mesh.MeshSizeMin", expMinSize);
            gmsh.Option.SetNumber("Mesh.MeshSizeMax", expMaxSize);
            gmsh.Option.SetNumber("Mesh.Algorithm3D", (int)expAlgo3d);
            gmsh.Option.SetNumber("Mesh.MeshSizeFactor", sizeFactor);

            gmsh.Model.Mesh.Generate(3);

            var error = gmsh.Logger.GetLastError();
            var actMinSize = gmsh.Option.GetNumber("Mesh.MeshSizeMin");
            var actMaxSize = gmsh.Option.GetNumber("Mesh.MeshSizeMax");
            var actAlgo3d = gmsh.Option.GetNumber("Mesh.Algorithm3D");
            var actSizeFactor = gmsh.Option.GetNumber("Mesh.MeshSizeFactor");

            Assert.IsTrue(string.IsNullOrEmpty(error));
            Assert.IsTrue(expMinSize == actMinSize);
            Assert.IsTrue(expMaxSize == actMaxSize);
            Assert.IsTrue((int)expAlgo3d == actAlgo3d);
            Assert.IsTrue(sizeFactor == actSizeFactor);
            //var file = Path.GetFileNameWithoutExtension(modelFolder + fileName);
            //gmsh.Write($@"C:\Mesh\INP\{file + "MinSize"}.inp");//Для визуального теста
        }

        [DataTestMethod]
        [DataRow(@"\CAD\Tavr.step", MeshAlgorithm2D.FrontalDelaunay)]
        [DataRow(@"\CAD\cilindr.step", MeshAlgorithm2D.Delaunay)]
        [DataRow(@"\CAD\screw.step", MeshAlgorithm2D.BAMG)]
        [DataRow(@"\CAD\nasos.step", MeshAlgorithm2D.Delaunay)]
        public void MeshAlgorithm2DTest(string fileName, MeshAlgorithm2D expAlgo2d)
        {
            gmsh.Open(modelFolder + fileName);

            gmsh.Option.SetNumber("Mesh.Algorithm", (int)expAlgo2d);

            gmsh.Model.Mesh.Generate(2);

            var error = gmsh.Logger.GetLastError();
            var actAlgo2d = gmsh.Option.GetNumber("Mesh.Algorithm");

            Assert.IsTrue(string.IsNullOrEmpty(error));
            Assert.IsTrue((int)expAlgo2d == actAlgo2d);

            //var file = Path.GetFileNameWithoutExtension(modelFolder + fileName);
            //gmsh.Write($@"C:\Mesh\INP\{file + "MeshAlgorithm" + expAlgo2d.ToString()}.inp");//Для визуального теста
        }

        [DataTestMethod]
        public void EmbedTest()
        {
            gmsh.Model.Occ.AddRectangle(0, 0, 0, 2, 2, -1, 0);
            gmsh.Model.Occ.AddRectangle(0.5, 0.5, 0, 1, 1, -1, 0);
            gmsh.Model.Occ.Synchronize();

            var expected = new int[] { 5, 6, 7, 8 };
            gmsh.Model.Mesh.Embed(1, expected, 2, 1);
            var actual = gmsh.Model.Mesh.GetEmbedded(2, 1).Where((v, i) => (i & 1) == 1).ToArray();
            CollectionAssert.AreEquivalent(expected, actual);

            //Если удалили, то должен показать пустой массив
            gmsh.Model.Mesh.RemoveEmbedded([2, 1]);
            var arr = gmsh.Model.Mesh.GetEmbedded(2, 1);
            Assert.AreEqual(0, arr.Length);

            gmsh.Model.Mesh.Embed(1, expected, 2, 1);
            gmsh.Model.Mesh.Generate(2);
            //gmsh.Write(@"D:\mixed.msh");//Для визуализации
        }

        /// <summary>
        /// Настраивает трансфинитную сетку для заданных кривых и объёмов.
        /// </summary>
        private void MakeTranfinite(int seamPointCurves, int[] seamCurves, int[] seamVolumes)
        {
            //Трансфиниция кривых
            for (var i = 0; i < seamCurves.Length; ++i)
                gmsh.Model.Mesh.SetTransfiniteCurve(seamCurves[i], seamPointCurves);
            //Трансфиниция поверхностей
            for (var i = 0; i < seamVolumes.Length; ++i)
            {
                var seamData = gmsh.Model.GetAdjacencies(3, seamVolumes[i]);
                var seamSurfs = seamData.Item2;
                for (var j = 0; j < seamSurfs.Length; ++j)
                    gmsh.Model.Mesh.SetTransfiniteAutomatic(new int[] { 2, seamSurfs[j] }, 2.35, false);
            }
        }

        /// <summary>
        /// Возвращает путь к каталогу с примерами тестов Gmsh.
        /// </summary>
        private static string GetExamplesPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Examples");
        }

        /// <summary>
        /// Возвращает путь к нативной библиотеке Gmsh для текущей ОС.
        /// </summary>
        private static string GetGmshLibraryPath()
        {
            return Path.Combine(AppContext.BaseDirectory, GmshKernel.LibraryFileName);
        }
    }
}

