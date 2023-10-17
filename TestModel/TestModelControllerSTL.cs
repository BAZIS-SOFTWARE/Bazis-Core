using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.ModelScenePresentator;
using Model.IO;
using System.Linq;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerSTL
    {
        [TestMethod]
        public void GlVertexesCreationSurf_fromSTLText()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromSTLFile();
            //model.SetLoader(loader);
            loader.LoadEvent += (ar1, ar2) => { };
            loader.Load(@"C:\BazisComponents\ModelSolution\TestModel\Examples\STL\ACII.stl");

            //act
            var surfs = model.ObjectData.FindMany<Surface>().ToArray();

            var presentator = new ModelScenePresentator(model.ObjectData);

            //var surfObjsCreator = presentator.CreateSurfaceObjsCreator("",true);

            //var indexes = surfObjsCreator.CreateIndexes();

            //var ptrs = surfObjsCreator.CreatePointers(indexes.Item1);
            //var coords = surfObjsCreator.CreateVertexes(indexes.Item2, new CoordsSurfaceObjsCreator());
            //var colors = surfObjsCreator.CreateVertexes(indexes.Item2, new ColorSurfaceObjsCreator());
            //var normals = surfObjsCreator.CreateVertexes(indexes.Item2, new NormalsSurfaceObjsCreator());

            //assert

        }
    }
}
