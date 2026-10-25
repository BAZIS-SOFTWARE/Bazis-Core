using Geometry;
using Model.IO.STL;
using Model.MeshObjects;
using OperationalController.ModelScenePresentator.GlObjsVertexCreators;
using System.Drawing;
using System.IO;

namespace TestProject_test.Geometry
{
    [TestClass]
    public class Point3DPrecisionTests
    {
        /// <summary>Проверяет сохранение координат и точность операций с точкой.</summary>
        [DataTestMethod]
        [DataRow(1000000000.125, -1000000000.25, 0.000000125)]
        [DataRow(-0.000000125, 123456789.125, -987654321.375)]
        public void Point3DPreservesDoubleCoordinates(double x, double y, double z)
        {
            var point = new Point3D(x, y, z);

            Assert.AreEqual(x, point._x);
            Assert.AreEqual(y, point._y);
            Assert.AreEqual(z, point._z);

            var restored = point.Sub(new Point3D(0.125, 0.25, 0.375))
                .Sum(new Point3D(0.125, 0.25, 0.375));

            Assert.AreEqual(x, restored._x, 1e-9);
            Assert.AreEqual(y, restored._y, 1e-9);
            Assert.AreEqual(z, restored._z, 1e-9);
        }

        /// <summary>Проверяет преобразование мировых координат в локальные без потери малых смещений.</summary>
        [TestMethod]
        public void FramePreservesSubFloatOffsets()
        {
            var frame = new Frame();
            frame.Shift(new Point3D(1000000000, -1000000000, 1000000000));
            var worldPoint = new Point3D(1000000000.125, -999999999.75, 1000000000.375);

            var localPoint = frame.GetCoordsInFrame(worldPoint);

            Assert.AreEqual(0.125, localPoint._x, 1e-12);
            Assert.AreEqual(0.25, localPoint._y, 1e-12);
            Assert.AreEqual(0.375, localPoint._z, 1e-12);
        }

        /// <summary>Проверяет границу преобразования координат в графический буфер.</summary>
        [DataTestMethod]
        [DataRow(1000000000.125, -1000000000.25, 0.000000125)]
        [DataRow(-0.000000125, 123456789.125, -987654321.375)]
        public void VertexBufferConvertsCoordinatesOnlyAtPresentation(double x, double y, double z)
        {
            var position = new Point3D(x, y, z);
            var node = new Node(1, position);
            var vertices = new float[3];
            var stride = 0;
            var creator = new CoordsPointObjsCreator();

            creator.Create(ref stride, vertices, node, Color.White);

            Assert.AreEqual(3, stride);
            Assert.AreEqual((float)x, vertices[0]);
            Assert.AreEqual((float)y, vertices[1]);
            Assert.AreEqual((float)z, vertices[2]);
            Assert.AreEqual(x, position._x);
            Assert.AreEqual(y, position._y);
            Assert.AreEqual(z, position._z);
        }

        /// <summary>Проверяет сохранение точности при чтении текстового STL.</summary>
        [TestMethod]
        public void TextStlLoaderPreservesDoubleCoordinates()
        {
            var path = Path.GetTempFileName();
            try
            {
                var content = "solid precise\nfacet normal 0 0 1\nouter loop\n" +
                    "vertex 1000000000.125 0 0\n" +
                    "vertex 1000000001.125 0 0\n" +
                    "vertex 1000000000.125 1 0\n" +
                    "endloop\nendfacet\nendsolid precise\n";
                File.WriteAllText(path, content);

                var model = new LoadFromSTLFile().Load(path);
                var coordinates = model.ObjectData.NodesSet.Values.Select(node => node.Position._x).ToArray();

                CollectionAssert.Contains(coordinates, 1000000000.125);
                CollectionAssert.Contains(coordinates, 1000000001.125);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
