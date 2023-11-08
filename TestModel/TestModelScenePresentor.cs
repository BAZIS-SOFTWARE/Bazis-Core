using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelInterfaces;
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

            Model.ModelObject modelObject;
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

            var presenter = new ModelScenePresentator(model.ObjectData);

            //act
            if(objType == ObjType.Элементы3D)
                presenter.HideInsideSurfaces(model.ObjectData.FindMany<Element3D>());

            var inds = presenter[objType.ToString()].CreateIndexes();
            var ptrs = presenter[objType.ToString()].CreatePointers(inds.Item1);
            var crds = presenter[objType.ToString()].CreateVertexes(inds.Item2, "координаты");
            var colors = presenter[objType.ToString()].CreateVertexes(inds.Item3, "цвет");
            var normals = presenter[objType.ToString()].CreateVertexes(inds.Item2, "нормаль");
            var edges = presenter[objType.ToString()].CreateEdgeFlags(inds.Item4);
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
            model.Loader = new LoadModelFromASCIITextFile();

            //act

            model.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");
            var presenter = new ModelScenePresentator(model.ObjectData);

            if(array[0] == "Элементы3D")
                presenter.HideInsideSurfaces(model.ObjectData.FindMany<Element3D>());

            var inds = presenter[array[0]].CreateIndexes();
            var ptrs = presenter[array[0]].CreatePointers(inds.Item1);
            var coords = presenter[array[0]].CreateVertexes(inds.Item2, "координаты");
            var colors = presenter[array[0]].CreateVertexes(inds.Item3, "цвет");
            var normals = presenter[array[0]].CreateVertexes(inds.Item2, "нормаль");
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
            model.Loader = new LoadModelFromGMSHTextFile();

            model.Load($@"..\..\..\Examples\{array[0]}");

            var modelPresenter = new ModelScenePresentator(model.ObjectData);

            foreach (string objsType in modelPresenter.Keys)
            {
                if(objsType == "Элементы3D")
                    modelPresenter.HideInsideSurfaces(modelPresenter.Values.Select(x => (ISurfaceObject)x));

                var tuple = modelPresenter[objsType].CreateIndexes();
                int[] ptrs = modelPresenter[objsType].CreatePointers(tuple.Item1);
                float[] glCoords = modelPresenter[objsType].CreateVertexes(tuple.Item2, "координаты");
                float[] glColors = modelPresenter[objsType].CreateVertexes(tuple.Item3, "цвет");
                float[] glNormals = modelPresenter[objsType].CreateVertexes(tuple.Item2, "нормаль");
                bool[] edges = modelPresenter[objsType].CreateEdgeFlags(tuple.Item4);

                Assert.AreEqual(tuple.Item4, edges.Length);
                Assert.AreEqual(tuple.Item1, ptrs.Length);
                Assert.AreEqual(tuple.Item2, glCoords.Length);
                Assert.AreEqual(tuple.Item3, glColors.Length);
                Assert.AreEqual(tuple.Item2, glNormals.Length);
            }

        }

        [DataTestMethod]
        [DataRow("Элементы3D")]
        [DataRow("Узлы")]
        public void TestObjectsCreationFromCDB(string objsType)
        {
            //assing
            var model = new ModelData();
            model.Loader = new LoadModelFromCDBTextFile();

            //act
            model.Load(@"..\..\..\Examples\CDB\longModel.cdb");
            var presentator = new ModelScenePresentator(model.ObjectData);

            var tuple = presentator[objsType].CreateIndexes();
            int[] ptrs = presentator[objsType].CreatePointers(tuple.Item1);
            float[] glCoords = presentator[objsType].CreateVertexes(tuple.Item2, "координаты");
            float[] glColors = presentator[objsType].CreateVertexes(tuple.Item3, "цвет");
            float[] glNormals = presentator[objsType].CreateVertexes(tuple.Item2, "нормаль");
            bool[] edges = presentator[objsType].CreateEdgeFlags(tuple.Item4);

            //assert
            Assert.AreEqual(tuple.Item4, edges.Length);
            Assert.AreEqual(tuple.Item1, ptrs.Length);
            Assert.AreEqual(tuple.Item2, glCoords.Length);
            Assert.AreEqual(tuple.Item3, glColors.Length);
            Assert.AreEqual(tuple.Item2, glNormals.Length);
        }
    }
}
