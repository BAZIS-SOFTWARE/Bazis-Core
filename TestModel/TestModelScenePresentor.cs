using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.IO;
using ModelController.ModelScenePresentator;
using System.Collections.Generic;
using Model.MeshObjects;
using ModelController.ModelScenePresentator.GlObjsPresenters;
using Model.GeometryObjects;
using ModelControllerInterfaces;
using System.IO;
using Geometry;
using System.Linq;
using GmshApi;
using Model.Interfaces.MeshObjects;
using ModelController.GmshController;
using System.Xml.Linq;
using System;
using ModelController.MeshObjsUtility;

namespace TestModel
{
    [TestClass]
    public class TestModelScenePresentor
    {
        [TestMethod]
        public void TestPointObjsPresenter()
        {
            var nodes = new Node[] { new Node(0, new Point3D(0.5f, 1.0f, -1.0f)) };
            var presenter = new PointObjsPresenter() { nodes };
            var indices = presenter.CreateIndexes();
            var pointers = presenter.CreatePointers(indices.Item1);
            var colors = presenter.CreateVertexes(indices.Item3, "цвет");
            //var expectedIndices = new Tuple<int,int,int,int>(1,3,4,0);
            var expectedPointers = new int[] { 0 };
            var expectedColors = new float[] { 0.0f, 0.0f, 1.0f, 1.0f };
            //Assert.AreEqual(expectedIndices, indices);
            CollectionAssert.AreEqual(expectedPointers, pointers);
            CollectionAssert.AreEqual(expectedColors, colors);
        }
        [TestMethod]
        public void TestLineObjsPresenter()
        {
            var nodes = new GeometryPoint[] { new GeometryPoint(0, new Point3D(0.5f, 1.0f, 1.0f)),
                                     new GeometryPoint(1, new Point3D(1.0f, 1.0f, -1.0f))
                                   };
            var lines = new Curve[] { new Curve(0, nodes) };
            var presenter = new LineObjsPresenter<GeometryPoint>();
            presenter.Add(lines);
            var indices = presenter.CreateIndexes();
            var pointers = presenter.CreatePointers(indices.Item1);
            var colors = presenter.CreateVertexes(indices.Item3, "цвет");
            //var expectedIndices = new Tuple<int, int, int,int>(2, 6, 8,1);
            var expectedPointers = new int[] { 0, 1 };
            var expectedColors = new float[] { 0.0f, 0.0f, 0.0f, 1.0f,
                                               0.0f, 0.0f, 0.0f, 1.0f };
            //Assert.AreEqual(expectedIndices, indices);
            CollectionAssert.AreEqual(expectedPointers, pointers);
            CollectionAssert.AreEqual(expectedColors, colors);
        }

        [TestMethod]
        public void TestSurfaceObjsPresenter()
        {
            var nodes = new Node[] { new Node(0, new Point3D(-0.5f, 0.0f, 0.0f)),
                                     new Node(1, new Point3D(0.5f, 0.0f, 0.0f)),
                                     new Node(2, new Point3D(0.0f, 0.5f, 0.0f))
                                   };
            var triangle = new Element2D[] { new Triangle(0, nodes) };
            var presenter = new SurfaceObjsPresenter<ElementSurface, Node>(false) { triangle };
            var indices = presenter.CreateIndexes();
            var pointers = presenter.CreatePointers(indices.Item1);
            var colors = presenter.CreateVertexes(indices.Item3, "цвет");
            //var expectedIndices = new Tuple<int, int, int,int>(3, 9, 12,3);
            var expectedPointers = new int[] { 0, 1, 2 };
            var expectedColors = new float[] { 0.0f, 0.0f, 1.0f, 1.0f,
                                               0.0f, 0.0f, 1.0f, 1.0f,
                                               0.0f, 0.0f, 1.0f, 1.0f};
            //Assert.AreEqual(expectedIndices, indices);
            CollectionAssert.AreEqual(expectedPointers, pointers);
            CollectionAssert.AreEqual(expectedColors, colors);
        }
        [TestMethod]
        public void TestEdgesOnSimpleTriangle()
        {
            var model = new ModelData();
            var nodes = new Node[]
            {
                new Node(0, new Point3D(0.0f,0.0f,0.0f)),
                new Node(1, new Point3D(1.0f,0.0f,0.0f)),
                new Node(2, new Point3D(0.5f,1.0f,0.0f)),
            };
            var el2D = new Triangle(0, nodes);
            model.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(), el2D);

            var creator = new PresentersCreator();

            var presentator = creator.CreateSurfaceObjectsPresenter(model.ObjectData.E2DCollection.GetObjects(), false);

            var values = presentator.CreateIndexes();
            var edges = presentator.CreateEdgeFlags(values.Item4);
            Assert.AreEqual(3, edges.Length);
            for (var i = 0; i < edges.Length; ++i)
                Assert.AreEqual(true, edges[i]);
        }
        [TestMethod]
        public void TestEdgesOnSimpleRectangle()
        {
            var model = new ModelData();
            var nodes = new Node[]
            {
                new Node(0, new Point3D(0.0f,0.0f,0.0f)),
                new Node(1, new Point3D(1.0f,0.0f,0.0f)),
                new Node(2, new Point3D(1.0f,1.0f,0.0f)),
                new Node(3, new Point3D(0.0f,1.0f,0.0f))
            };
            var el2D = new Quad(0, nodes);

            model.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(), el2D);

            var creator = new PresentersCreator();

            var presentator = creator.CreateSurfaceObjectsPresenter(model.ObjectData.E2DCollection.GetObjects(), false);
            var values = presentator.CreateIndexes();
            var edges = presentator.CreateEdgeFlags(values.Item4);
            Assert.AreEqual(6, edges.Length);
            var expectedFlags = new bool[] { true, true, false, false, true, true };
            for (var i = 0; i < expectedFlags.Length; ++i)
                Assert.AreEqual(expectedFlags[i], edges[i]);
        }

        [TestMethod]
        public void TestEdgesOnPolygon()
        {
            var model = new ModelData();
            var points = new GeometryPoint[]
            {
                new GeometryPoint(0, new Point3D(0.0f,0.0f,0.0f)),
                new GeometryPoint(1, new Point3D(1.0f,0.0f,0.0f)),
                new GeometryPoint(2, new Point3D(1.0f,1.0f,0.0f)),
                new GeometryPoint(3, new Point3D(0.0f,1.0f,0.0f)),
                new GeometryPoint(4, new Point3D(-1.0f,1.0f,0.0f)),
                new GeometryPoint(5, new Point3D(-1.0f,0.0f,0.0f))
            };
            var surface = new GeometrySurface(0, points);
            var poligon = new SurfaceFigure(0, new GeometrySurface[] { surface });

            model.ObjectData.SurfaceCollection.Add(ObjType.Поверхность.ToString(), poligon);
            var creator = new PresentersCreator();

            var presentator = creator.CreateSurfaceObjectsPresenter(model.ObjectData.SurfaceCollection.GetObjects(), false);

            var values = presentator.CreateIndexes();
            var edges = presentator.CreateEdgeFlags(values.Item4);

            Assert.AreEqual(12, edges.Length);
            var expectedFlags = new bool[] { true, true, false, false, true, false,
                                             false, true, false, false, true, true};
            for (var i = 0; i < expectedFlags.Length; ++i)
                Assert.AreEqual(expectedFlags[i], edges[i]);
        }

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
        public void TestObjectsPresentorsManual(float[]coords, ObjType objType, int [] indsR)
        {

            //assign
            var model = new ModelData();
            //var nodes = new List<Node>();
            var len = coords.Length / 3;
            for (int i = 0; i < len; i++)
            {
                var x = coords[3 * i + 0];
                var y = coords[3 * i + 1];
                var z = coords[3 * i + 2];
                var node = new Node(i, new Geometry.Point3D(x, y, z));
                model.ObjectData.NodesSet.Add(node.Number, node);
            }



            var creator = new PresentersCreator();
            IObjsPresenter presenter;
            //act

            switch (objType)
            {
                case ObjType.Элемент3D:
                    model.ObjectData.E3DCollection.Add(ObjType.Элемент3D.ToString(), new Tetra(0, model.ObjectData.NodesSet.Values.ToArray()));
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.E3DCollection.GetObjects(), true);
                    break;
                case ObjType.Элемент2D:
                    model.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(), new Triangle(0, model.ObjectData.NodesSet.Values.ToArray()));
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.E2DCollection.GetObjects(), false);
                    break;
                default:
                    model.ObjectData.E1DCollection.Add(ObjType.Элемент1D.ToString(), new Beam(0, model.ObjectData.NodesSet.Values.ToArray()));
                    presenter = creator.CreateLineObjectsPresenter(model.ObjectData.E1DCollection.GetObjects());
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
            ObjType.Кривая, new int[] { 2, 6, 8, 6, 1 })]
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
                case ObjType.Кривая:
                    model.ObjectData.CurveCollection.Add(ObjType.Кривая.ToString(), new Curve(0, points.ToArray()));
                    presenter = creator.CreateLineObjectsPresenter(model.ObjectData.CurveCollection.GetObjects());
                    break;
                default:
                    var surface = new GeometrySurface(0, points.ToArray());
                    model.ObjectData.SurfaceCollection.Add(ObjType.Поверхность.ToString(), new SurfaceFigure(0,new GeometrySurface[] { surface } ));
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.SurfaceCollection.GetObjects(), false);
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
        [DataRow(ObjType.Элемент2D, @"..\..\..\Examples\STL\ACII.stl",new string[]
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
        public void TestObjectsPresentorsFromFile(ObjType objType,string path,string[] array)
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
                    presenter = creator.CreatePointObjectsPresenter(model.ObjectData.NodesSet.Values);
                    break;
                case ObjType.Кривая:
                    presenter = creator.CreateLineObjectsPresenter(model.ObjectData.CurveCollection.GetObjects());
                    break;
                case ObjType.Поверхность:
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.SurfaceCollection.GetObjects(), false);
                    break;
                case ObjType.Объем:
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.VolumeCollection.GetObjects(), false);
                    break;
                case ObjType.Элемент1D:
                    presenter = creator.CreateLineObjectsPresenter(model.ObjectData.E1DCollection.GetObjects());
                    break;
                case ObjType.Элемент2D:
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.E2DCollection.GetObjects(), false);
                    break;
                case ObjType.Элемент3D:
                    presenter = creator.CreateSurfaceObjectsPresenter(model.ObjectData.E3DCollection.GetObjects(), true);
                    break;
                default:
                    presenter = creator.CreatePointObjectsPresenter(model.ObjectData.PointsSet.Values);
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

        //Тест на нормали, презентатор отправляет некоррректные нормали 3д элементов на сцену :(
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\CDB\longModel.cdb")]
        [DataRow(@"..\..\..\Examples\VIsual-Mesh\rhombDATA1.ASC")]
        [DataRow(@"..\..\..\Examples\CAD\cilindr.step")]
        public void TestNormals(string path)
        {
            var model = new ModelData();

            var ext = Path.GetExtension(path);
            if(ext == ".step")
            {
                var controller = new GmshController();
                controller.Load(@"..\..\..\..\GmshApi\gmsh.dll");
                controller.Gmsh.Open(path);
                controller.Gmsh.Model.Mesh.Generate(3);
                var elements = controller.GetMeshObjects().Item4;
                model.ObjectData.E3DCollection.AddRange("Элементы3D", elements);
                controller.Unload();
            }
            else
            {
                if (ext == ".stl")
                    model.Loader = new LoadModelFromSTLFile();
                else if (ext == ".ASC")
                    model.Loader = new LoadModelFromASCIITextFile();
                else if (ext == ".cdb")
                    model.Loader = new LoadModelFromCDBTextFile();
                model.Loader.LoadEvent += (ar1, ar2) => { };
                model.Load(path);
            }

            var presenter = new SurfaceObjsPresenter<ElementSurface, Node>(true);
            var elems = model.ObjectData.E3DCollection.GetObjects();
            presenter.Add(elems);

            var inds = presenter.CreateIndexes();
            var coords = presenter.CreateVertexes(inds.Item2, "координаты");
            var normals = presenter.CreateVertexes(inds.Item2, "нормаль");
            var separators = presenter.CreateSeparators();

            var checker = new NormalsChecker(presenter);
            checker.Log = $@".\{Path.GetFileNameWithoutExtension(path)}_log.log";
            checker.Check();

            Assert.IsTrue(checker.NotOthogonals == 0);
            Assert.IsTrue(checker.ZeroLengths == 0);
            Assert.IsTrue(checker.Inverted == 0);
        }

        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\3D\mixElements.bpf", false)]
        [DataRow(@"..\..\..\Examples\3D\mixElements.bpf", true)]
        public void TestSeparators(string fileName, bool isVolume)
        {
            var model = new ModelData();

            model.Loader = new LoadModelFromBPFTextFile();
            model.Loader.LoadEvent += (ar1, ar2) => { };
            model.Load(fileName);

            var surfPresenter = new SurfaceObjsPresenter<ElementSurface, Node>(isVolume);

            Element[] elements;
            if (isVolume)
            {
                var elems = model.ObjectData.E3DCollection.GetObjects().ToArray();
                surfPresenter.Add(elems);
                elements = elems;
            }
            else
            {
                var elems = model.ObjectData.E2DCollection.GetObjects().ToArray();
                surfPresenter.Add(elems);
                elements = elems;
            }

            var inds = surfPresenter.CreateIndexes();
            var coords = surfPresenter.CreateVertexes(inds.Item2, "координаты");
            var separators = surfPresenter.CreateSeparators();

            Assert.IsTrue(elements.Length == separators.Length - 1);

            for(var i = 1; i < separators.Length; ++i)
            {
                var trCount = separators[i] - separators[i - 1];
                Assert.IsTrue(NormalsChecker.IdentifyByTriangles(trCount) == elements[i - 1].ElementKind);
            }
        }
    }
}
