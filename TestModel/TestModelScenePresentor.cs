using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.IO;
using ModelController.ModelScenePresentator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TestModel
{
    [TestClass]
    public class TestModelScenePresentor
    {
        [DataTestMethod]
        [DataRow(new float[]
        { 
            0, 0, 0,
            10,10,0,
            20,10,0
        }, 
            ObjType.Поверхности, ObjKind.ElementSurface, new int[] { 3, 9, 12, 9,3 })]
        [DataRow(new float[]
        {
            0, 0, 0,
            10,10,0,
        },
            ObjType.Элементы1D, ObjKind.Beam, new int[] { 2, 6, 8, 6,1 })]
        [DataRow(new float[]
        {
            0, 0, 0,
            10, 0, 0,
            0, 10, 0,
            0, 0, 10
        },
            ObjType.Элементы3D, ObjKind.Tetra, new int[] { 12, 36, 48, 36,12 })]
        public void TestObjectsCreationManual(float[]coords, ObjType objType, ObjKind objKind, int [] indsR)
        {

            //assign
            var nodes = new List<Node>();
            var len = coords.Length / 3;
            for (int i = 0; i < len; i++)
            {
                var x = coords[3 * i + 0];
                var y = coords[3 * i + 1];
                var z = coords[3 * i + 2];
                nodes.Add(new Node(i, new Geometry.Point3D(x, y, z)));
            }

            ModelObject modelObject;
            switch (objType)
            {
                case ObjType.Поверхности:
                    modelObject = new Surface(0, nodes.ToArray(), objKind);
                    break;
                case ObjType.Элементы2D:
                    modelObject = new Element2D(0, nodes.ToArray(), objKind);
                    break;
                case ObjType.Элементы1D:
                    modelObject = new Element1D(0, nodes.ToArray(), objKind);
                    break;
                default:
                    modelObject = new Element3D(0, nodes.ToArray(), objKind);
                    break;
            }


            var model = new ModelData();
            model.ObjectData.AddRange(nodes);
            model.ObjectData.Add(modelObject);

            var presenter = new ModelScenePresentator(model);

            //act

            presenter.HideInsideObjects(objType.ToString());
            var inds = presenter.CreateVBOIndexes(objType.ToString());
            var ptrs = presenter.CreateVBOPointers(objType.ToString(), inds.Item1);
            var crds = presenter.CreateVBOVertexes(objType.ToString(), inds.Item2, "координаты");
            var colors = presenter.CreateVBOVertexes(objType.ToString(), inds.Item3, "цвет");
            var normals = presenter.CreateVBOVertexes(objType.ToString(), inds.Item2, "нормаль");
            var edges = presenter.CreateVBOEdges(objType.ToString(), inds.Item4);
            //assert

            Assert.AreEqual(ptrs.Length, indsR[0]);
            Assert.AreEqual(crds.Length, indsR[1]);
            Assert.AreEqual(colors.Length, indsR[2]);
            Assert.AreEqual(normals.Length, indsR[3]);
            Assert.AreEqual(edges.Length, indsR[4]);
        }


        [DataTestMethod]
        [DataRow(new string[]
{
            "Элементы3D", "24", "72", "96", "72"
})]
        [DataRow(new string[]
{
            "Узлы", "7", "21", "28", "21"
})]
        public void TestObjectsCreationFromASC(string[] array)
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += (ar1, ar2) => { };
            var modelASC = loader.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");
            model.Load(modelASC);
            //act

            //var nodes = model.ObjectData.FindMany(ObjKind.Node).ToArray();
            //nodes[0].ViewState = false;

            var presenter = new ModelScenePresentator(model);

            presenter.HideInsideObjects(array[0]);

            var inds = presenter.CreateVBOIndexes(array[0]);
            var ptrs = presenter.CreateVBOPointers(array[0], inds.Item1);
            var coords = presenter.CreateVBOVertexes(array[0], inds.Item2, "координаты");
            var colors = presenter.CreateVBOVertexes(array[0], inds.Item3, "цвет");
            var normals = presenter.CreateVBOVertexes(array[0], inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, int.Parse(array[1]));
            Assert.AreEqual(coords.Length, int.Parse(array[2]));
            Assert.AreEqual(colors.Length, int.Parse(array[3]));
            Assert.AreEqual(normals.Length, int.Parse(array[4]));
        }


        [DataTestMethod]
        [DataRow(new string[]
{
            "2D_axi\\2Dval.inp"
})]
        [DataRow(new string[]
{
            "3D\\Korpus.inp"
})]
        public void TestObjectsCreationFromINP(string[] array)
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromGMSHTextFile();

            loader.LoadEvent += (ar1, ar2) => { };
            var modelINP = loader.Load($@"..\..\..\Examples\{array[0]}");
            model.Load(modelINP);

            var presenter = new ModelScenePresentator(model);

            foreach (string item in presenter?.GetObjectsTypes())
            {
                presenter.HideInsideObjects(item);

                var tuple = presenter.CreateVBOIndexes(item);
                int[] ptrs = presenter.CreateVBOPointers(item, tuple.Item1);
                float[] glCoords = presenter.CreateVBOVertexes(item, tuple.Item2, "координаты");
                float[] glColors = presenter.CreateVBOVertexes(item, tuple.Item3, "цвет");
                float[] glNormals = presenter.CreateVBOVertexes(item, tuple.Item2, "нормаль");
                bool[] edges = presenter.CreateVBOEdges(item, tuple.Item4);

                Assert.AreEqual(tuple.Item4, edges.Length);
                Assert.AreEqual(tuple.Item1, ptrs.Length);
                Assert.AreEqual(tuple.Item2, glCoords.Length);
                Assert.AreEqual(tuple.Item3, glColors.Length);
                Assert.AreEqual(tuple.Item2, glNormals.Length);
            }

        }

        [DataTestMethod]
        [DataRow(new string[]
{
            "Элементы3D"
})]
        [DataRow(new string[]
{
            "Узлы"
})]
        public void TestObjectsCreationFromCDB(string[] array)
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromCDBTextFile();

            loader.LoadEvent += (ar1, ar2) => { };
            var modelCDB = loader.Load(@"..\..\..\Examples\CDB\longModel.cdb");

            model.Load(modelCDB);

            //act
            var presentator = new ModelScenePresentator(model);

            var indexes_nodes = presentator.CreateVBOIndexes(array[0]);
            var ptrs_nodes = presentator.CreateVBOPointers(array[0], indexes_nodes.Item1);
            var coords_nodes = presentator.CreateVBOVertexes(array[0], indexes_nodes.Item2, "координаты");
            var colors_nodes = presentator.CreateVBOVertexes(array[0], indexes_nodes.Item3, "цвет");
            var normals_nodes = presentator.CreateVBOVertexes(array[0], indexes_nodes.Item2, "нормаль");
            bool[] edges = presentator.CreateVBOEdges(array[0], indexes_nodes.Item4);

            //assert

        }
    }
}
