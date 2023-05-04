using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.ModelScenePresentator;
using ModelParcer;
using System;
using System.IO;

namespace TestModel
{
    [TestClass]
    public class TestModelControllerCDB
    {
        [TestMethod]
        public void GlVertexesCreation_2D3D()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromCDBTextFile();

            loader.LoadEvent += (ar1, ar2) => { };

            var path = Path.GetFullPath(".\\").Replace("bin\\Debug\\", "");
            var files = Directory.GetFiles(path, "longModel.cdb", SearchOption.AllDirectories);
            var modelCDB = loader.Load(files[0]);

            model.Load(modelCDB);

            //act
            var presentator = new ModelScenePresentator(model);

            var indexes_nodes = presentator.CreateVBOIndexes("Узлы");
            var ptrs_nodes = presentator.CreateVBOPointers("Узлы", indexes_nodes.Item1);
            var coords_nodes = presentator.CreateVBOVertexes("Узлы", indexes_nodes.Item2, "координаты");
            var colors_nodes = presentator.CreateVBOVertexes("Узлы", indexes_nodes.Item2, "цвет");
            var normals_nodes = presentator.CreateVBOVertexes("Узлы", indexes_nodes.Item2, "нормаль");
            
            var indexes = presentator.CreateVBOIndexes("Элементы3D");
            var ptrs = presentator.CreateVBOPointers("Элементы3D", indexes.Item1);
            var coords = presentator.CreateVBOVertexes("Элементы3D", indexes.Item2, "координаты");
            var colors = presentator.CreateVBOVertexes("Элементы3D", indexes.Item2, "цвет");
            var normals = presentator.CreateVBOVertexes("Элементы3D", indexes.Item2, "нормаль");

            //assert

        }
    }
}
