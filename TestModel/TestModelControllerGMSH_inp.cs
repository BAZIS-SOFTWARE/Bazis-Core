using System.Linq;
using GMSH_Importer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController;
using ModelController.IO;
using ModelInterface;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerGMSH_inp
    {
        [TestMethod]
        public void GlVertexesCreation2D3DObjs_Elem2D3D()
        {
            //assing
            var model = new ModelData();
            var loader = new Import_Mesh();

            loader.LoadEvent += Model_LoadEvent;
            var modelINP = loader.Load(@"C:\BazisXN\GMSH_Importer(inp)\mesh\test3.inp");
            model.Load(modelINP);

            //act
            var elems3D = model.ObjectData.FindMany<Element3D>().ToArray();
            var presentator = new ModelScenePresentator(model.ObjectData.ToArray());

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
    }
}
