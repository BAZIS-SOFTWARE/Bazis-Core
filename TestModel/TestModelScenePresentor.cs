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
using Model.GeometryObjects;
using ModelControllerInterfaces;
using ModelInterfaces.GeometryObjects;
using System.IO;

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
            ObjType.Элемент2D, new int[] { 3, 9, 12, 9,3 })]
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
        public void TestMeshObjectsCreationManual(float[]coords, ObjType objType, int [] indsR)
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


            var model = new ModelData();
            var creator = new PresentersCreator();
            IObjsPresenter presenter;
            //act
            model.ObjectData.NodeCollection.AddRange(nodes);
            switch (objType)
            {
                case ObjType.Элемент3D:
                    model.ObjectData.E3DCollection.Add(new Tetra(0, nodes.ToArray()));
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.E3DCollection, true);
                    break;
                case ObjType.Элемент2D:
                    model.ObjectData.E2DCollection.Add(new Triangle(0, nodes.ToArray()));
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.E2DCollection, false);
                    break;
                default:
                    model.ObjectData.E1DCollection.Add(new Beam(0, nodes.ToArray()));
                    presenter = creator.CreateLineObjectsPresenter(model.ObjectData.E1DCollection);
                    break;
            }

            //act
            
            if (presenter.IsVolumeObjs)
            {
                var volPresenter = (IVolumeObjsPresenter)presenter;
                volPresenter.HideInsideSurfaces();
            }

            var inds = presenter.CreateIndexes();
            var ptrs = presenter.CreatePointers(inds.Item1);
            var crds = presenter.CreateVertexes(inds.Item2, "координаты");
            var colors = presenter.CreateVertexes(inds.Item3, "цвет");
            var normals = presenter.CreateVertexes(inds.Item2, "нормаль");
            var edges = presenter.CreateEdgeFlags(inds.Item4);
            //assert

            Assert.AreEqual(ptrs.Length, indsR[0]);
            Assert.AreEqual(crds.Length, indsR[1]);
            Assert.AreEqual(colors.Length, indsR[2]);
            Assert.AreEqual(normals.Length, indsR[3]);
            Assert.AreEqual(edges.Length, indsR[4]);
        }

        [DataTestMethod]
        [DataRow(new float[]
        {
            0, 0, 0,
            10,10,0,
        },
            ObjType.Линия, new int[] { 2, 6, 8, 6, 1 })]
        public void TestGeomObjectsCreationManual(float[] coords, ObjType objType, int[] indsR)
        {

            //assign
            var points = new List<GeometryPoint>();
            var len = coords.Length / 3;
            for (int i = 0; i < len; i++)
            {
                var x = coords[3 * i + 0];
                var y = coords[3 * i + 1];
                var z = coords[3 * i + 2];
                points.Add(new GeometryPoint(i, new Geometry.Point3D(x, y, z)));
            }

            var model = new ModelData();
            var creator = new PresentersCreator();
            IObjsPresenter presenter;

            switch (objType)
            {
                case ObjType.Линия:
                    model.ObjectData.LineCollection.Add(new Line(0, points.ToArray()));
                    presenter = creator.CreateLineObjectsPresenter(model.ObjectData.LineCollection);
                    break;
                default:
                    var surface = new GeometrySurface(0, points.ToArray());
                    model.ObjectData.Fig2DCollection.Add(new Figure2D(0,new GeometrySurface[] { surface } ));
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.Fig2DCollection, false);
                    break;
            }

            //act

            if (presenter.IsVolumeObjs)
            {
                var volPresenter = (IVolumeObjsPresenter)presenter;
                volPresenter.HideInsideSurfaces();
            }

            var inds = presenter.CreateIndexes();
            var ptrs = presenter.CreatePointers(inds.Item1);
            var crds = presenter.CreateVertexes(inds.Item2, "координаты");
            var colors = presenter.CreateVertexes(inds.Item3, "цвет");
            var normals = presenter.CreateVertexes(inds.Item2, "нормаль");
            var edges = presenter.CreateEdgeFlags(inds.Item4);
            //assert

            Assert.AreEqual(ptrs.Length, indsR[0]);
            Assert.AreEqual(crds.Length, indsR[1]);
            Assert.AreEqual(colors.Length, indsR[2]);
            Assert.AreEqual(normals.Length, indsR[3]);
            Assert.AreEqual(edges.Length, indsR[4]);
        }

        [DataTestMethod]
        [DataRow(ObjType.Фигура2D, @"..\..\..\Examples\STL\ACII.stl",new string[]
{
    "6", "18", "24", "18"
})]
        [DataRow(ObjType.Элемент3D, @"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC",new string[]
{
            "96", "288", "384", "288"
})]
        [DataRow(ObjType.Узел, @"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC",new string[]
{
            "7", "21", "28", "21"
})]
        [DataRow(ObjType.Элемент3D, @"..\..\..\Examples\CDB\longModel.cdb", new string[]
{
            "109620", "328860", "438480", "328860"
})]
        [DataRow(ObjType.Узел, @"..\..\..\Examples\CDB\longModel.cdb", new string[]
{
            "3321", "9963", "13284", "9963"
})]
        public void TestObjectsPresentorsCreation(ObjType objType,string path,string[] array)
        {
            //assing
            var model = new ModelData();

            var ext = Path.GetExtension(path);

            if(ext == ".stl")
                model.Loader = new LoadModelFromSTLFile();
            else if(ext == ".ASC")
                model.Loader = new LoadModelFromASCIITextFile();
            else if (ext == ".cdb")
                model.Loader = new LoadModelFromCDBTextFile();

            model.Loader.LoadEvent += (ar1, ar2) => { };
            model.Load(path);

            //act

            var creator = new PresentersCreator();
            IObjsPresenter presenter;

            switch (objType)
            {
                case ObjType.Узел:
                    presenter = creator.CreatePointObjectsPresenter(model.ObjectData.NodeCollection);
                    break;
                case ObjType.Линия:
                    presenter = creator.CreateLineObjectsPresenter(model.ObjectData.LineCollection);
                    break;
                case ObjType.Фигура2D:
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.Fig2DCollection, false);
                    break;
                case ObjType.Фигура3D:
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.Fig3DCollection, false);
                    break;
                case ObjType.Элемент1D:
                    presenter = creator.CreateLineObjectsPresenter(model.ObjectData.E1DCollection);
                    break;
                case ObjType.Элемент2D:
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.E2DCollection, false);
                    break;
                case ObjType.Элемент3D:
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.E3DCollection, true);
                    break;
                default:
                    presenter = creator.CreatePointObjectsPresenter(model.ObjectData.PointCollection);
                    break;
            }

            var inds = presenter.CreateIndexes();
            var ptrs = presenter.CreatePointers(inds.Item1);
            var coords = presenter.CreateVertexes(inds.Item2, "координаты");
            var colors = presenter.CreateVertexes(inds.Item3, "цвет");
            var normals = presenter.CreateVertexes(inds.Item2, "нормаль");
            //assert
            Assert.AreEqual(ptrs.Length, int.Parse(array[0]));
            Assert.AreEqual(coords.Length, int.Parse(array[1]));
            Assert.AreEqual(colors.Length, int.Parse(array[2]));
            Assert.AreEqual(normals.Length, int.Parse(array[3]));

        }            
    }
}
