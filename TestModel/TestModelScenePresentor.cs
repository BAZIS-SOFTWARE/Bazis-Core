using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelInterfaces;
using Model.IO;
using ModelController.ModelScenePresentator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Model.MeshObjects;
using ModelController.ModelScenePresentator.GlObjsPresenters;

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
            ObjType.Фигура2D, new int[] { 3, 9, 12, 9,3 })]
        [DataRow(new float[]
        {
            0, 0, 0,
            10,10,0,
        },
            ObjType.Элемент1D, new int[] { 2, 6, 8, 6,1 })]
        [DataRow(new float[]
        {
            0, 0, 0,
            10, 0, 0,
            0, 10, 0,
            0, 0, 10
        },
            ObjType.Элемент3D, new int[] { 12, 36, 48, 36,12 })]
        public void TestObjectsCreationManual(float[]coords, ObjType objType, int [] indsR)
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
                case ObjType.Элемент3D:
                    modelObject = new Tetra(0, nodes.ToArray());
                    break;
                case ObjType.Фигура2D:
                    modelObject = new ElementSurface(0, nodes.ToArray());
                    break;
                default:
                    modelObject = new Beam(0, nodes.ToArray());
                    break;
            }


            var model = new ModelData();
            model.ObjectData.AddRange(nodes);
            model.ObjectData.Add(modelObject);

            var presenter = new ModelScenePresentator(model.ObjectData);

            //act
            if(objType == ObjType.Элемент3D)
                presenter.HideInsideSurfaces(model.ObjectData.FindMany<Element3D>());

            var inds = presenter[objType].CreateIndexes();
            var ptrs = presenter[objType].CreatePointers(inds.Item1);
            var crds = presenter[objType].CreateVertexes(inds.Item2, "координаты");
            var colors = presenter[objType].CreateVertexes(inds.Item3, "цвет");
            var normals = presenter[objType].CreateVertexes(inds.Item2, "нормаль");
            var edges = presenter[objType].CreateEdgeFlags(inds.Item4);
            //assert

            Assert.AreEqual(ptrs.Length, indsR[0]);
            Assert.AreEqual(crds.Length, indsR[1]);
            Assert.AreEqual(colors.Length, indsR[2]);
            Assert.AreEqual(normals.Length, indsR[3]);
            Assert.AreEqual(edges.Length, indsR[4]);
        }

        [DataTestMethod]
        [DataRow(ObjType.Фигура2D, new string[]
{
    "6", "18", "24", "18"
})]
        public void TestObjectsCreationfromSTLText(ObjType objType,string[] array)
        {
            //assing
            var model = new ModelData();
            model.Loader = new LoadModelFromSTLFile();

            model.Loader.LoadEvent += (ar1, ar2) => { };
            model.Load(@"C:\BazisComponents\ModelSolution\TestModel\Examples\STL\ACII.stl");

            //act
            var surfs = model.ObjectData.FindMany<ElementSurface>().ToArray();

            var presenter = new ModelScenePresentator(model.ObjectData);

            var inds = presenter[objType].CreateIndexes();
            var ptrs = presenter[objType].CreatePointers(inds.Item1);
            var coords = presenter[objType].CreateVertexes(inds.Item2, "координаты");
            var colors = presenter[objType].CreateVertexes(inds.Item3, "цвет");
            var normals = presenter[objType].CreateVertexes(inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, int.Parse(array[0]));
            Assert.AreEqual(coords.Length, int.Parse(array[1]));
            Assert.AreEqual(colors.Length, int.Parse(array[2]));
            Assert.AreEqual(normals.Length, int.Parse(array[3]));

        }


        [DataTestMethod]
        [DataRow(ObjType.Элемент3D, new string[]
{
            "24", "72", "96", "72"
})]
        [DataRow(ObjType.Узел, new string[]
{
            "7", "21", "28", "21"
})]
        public void TestObjectsCreationFromASC(ObjType objType, string[] array)
        {
            //assing
            var model = new ModelData();
            model.Loader = new LoadModelFromASCIITextFile();

            //act

            model.Load(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC");
            var presenter = new ModelScenePresentator(model.ObjectData);

            if(objType == ObjType.Элемент3D)
                presenter.HideInsideSurfaces(model.ObjectData.FindMany<Element3D>());

            var inds = presenter[objType].CreateIndexes();
            var ptrs = presenter[objType].CreatePointers(inds.Item1);
            var coords = presenter[objType].CreateVertexes(inds.Item2, "координаты");
            var colors = presenter[objType].CreateVertexes(inds.Item3, "цвет");
            var normals = presenter[objType].CreateVertexes(inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, int.Parse(array[0]));
            Assert.AreEqual(coords.Length, int.Parse(array[1]));
            Assert.AreEqual(colors.Length, int.Parse(array[2]));
            Assert.AreEqual(normals.Length, int.Parse(array[3]));
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

            foreach (var objsType in modelPresenter.Keys)
            {
                if(objsType == ObjType.Элемент3D)
                    modelPresenter.HideInsideSurfaces((SurfaceObjsPresenter<IElementSurface,INode>)modelPresenter[objsType]);

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
        [DataRow(ObjType.Элемент3D)]
        [DataRow(ObjType.Узел)]
        public void TestObjectsCreationFromCDB(ObjType objsType)
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
