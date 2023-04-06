using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController;
using ModelController.GlObjsVertexCreators;
using ModelController.IO;
using ModelInterface;
using STL_imp_exp;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerASCII
    {
        [TestMethod]
        public void GlVertexesCreationSurf_fromSTLText()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromSTLFile();
            //model.SetLoader(loader);
            //loader.LoadEvent += Model_LoadEvent;
            loader.Load(@"C:\BazisXN\Model\STLLoader\ACII.stl");

            //act
            var surfs = model.ObjectData.FindMany<Surface>().ToArray();

            var presentator = new ModelScenePresentator(model);

            //var surfObjsCreator = presentator.CreateSurfaceObjsCreator("",true);

            //var indexes = surfObjsCreator.CreateIndexes();

            //var ptrs = surfObjsCreator.CreatePointers(indexes.Item1);
            //var coords = surfObjsCreator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = surfObjsCreator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = surfObjsCreator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());

            //assert

        }

        [TestMethod]
        public void GlVertexesCreation3DObjs_Elem3D()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += Model_LoadEvent;
            var modelASC = loader.Load(@"C:\Bazis_newGUI\MESH\rhombDATA1.ASC");
            model.Load(modelASC);
            //act
            var presentator = new ModelScenePresentator(model);

            //var surfObjsCreator = presentator.CreateSurfaceObjsCreator("Элементы3D", true);

            //var indexes = surfObjsCreator.CreateIndexes();

            //var ptrs = surfObjsCreator.CreatePointers(indexes.Item1);
            //var coords = surfObjsCreator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = surfObjsCreator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = surfObjsCreator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());

            //assert

        }

        [TestMethod]
        public void GlVertexesCreation3DObjs_Nodes()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += Model_LoadEvent;
            var modelASC = loader.Load(@"C:\Bazis_newGUI\MESH\rhombDATA1.ASC");
            model.Load(modelASC);

            //act
            var nodes = model.ObjectData.FindMany<Node>().ToArray();
            var presentator = new ModelScenePresentator(model);

            presentator.HideInsideObjects("Узлы");

            //var pointObjsCreator = presentator.CreatePointObjsCreator();

            //var indexes = pointObjsCreator.CreateIndexes();

            //var ptrs = pointObjsCreator.CreatePointers(indexes.Item1);
            //var coords = pointObjsCreator.CreateVertexes(indexes.Item2, new CoordsPointObjsCreator());
            //var colors = pointObjsCreator.CreateVertexes(indexes.Item2, new ColorPointObjsCreator());
            //var normals = pointObjsCreator.CreateVertexes(indexes.Item2, new NormalsPointObjsCreator());

            //assert

        }

        [TestMethod]
        public void GlVertexesCreation2D3DObjs_Elem2D3D_GMSH()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromGMSHTextFile();

            loader.LoadEvent += Model_LoadEvent;
            var modelINP = loader.Load(@"C:\BazisXN\GMSH_Importer(inp)\mesh\test3.inp");
            model.Load(modelINP);

            //act
            var presentator = new ModelScenePresentator(model);

            //var surfObjsCreator = presentator.CreateSurfaceObjsCreator("Элементы3D", true);

            //var indexes = surfObjsCreator.CreateIndexes();

            //var ptrs = surfObjsCreator.CreatePointers(indexes.Item1);
            //var coords = surfObjsCreator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = surfObjsCreator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = surfObjsCreator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());

            //assert

        }

        [TestMethod]
        public void GlVertexesCreation2D3DObjs_Elem2D3D()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += Model_LoadEvent;
            var modelASC = loader.Load(@"C:\Projs\ENERGOMASH\MESH\SECTOR_DATA1.ASC");
            model.Load(modelASC);

            //act
            var elems3D = model.ObjectData.FindMany<Element3D>().ToArray();
            var presentator = new ModelScenePresentator(model);

            //var surfObjsCreator = presentator.CreateSurfaceObjsCreator("Элементы3D", true);

            //var indexes = surfObjsCreator.CreateIndexes();

            //var ptrs = surfObjsCreator.CreatePointers(indexes.Item1);
            //var coords = surfObjsCreator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = surfObjsCreator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = surfObjsCreator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());

            //assert

        }

        private void Model_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new NotImplementedException();
        }

        [TestMethod]
        public void GlVertexesCreation2DObjs_Elem2D()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromASCIITextFile();

            loader.LoadEvent += Model_LoadEvent;
            var modelASC = loader.Load(@"C:\Bazis_newGUI\MESH\Test_DATA1.ASC");
            model.Load(modelASC);

            //act
            var elems2D = model.ObjectData.FindMany<Element2D>().ToArray();
            var presentator = new ModelScenePresentator(model);

            //var surfObjsCreator = presentator.CreateSurfaceObjsCreator("Элементы2D", true);

            //var indexes = surfObjsCreator.CreateIndexes();

            //var ptrs = surfObjsCreator.CreatePointers(indexes.Item1);
            //var coords = surfObjsCreator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = surfObjsCreator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = surfObjsCreator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());

            //assert

        }
    }
}
