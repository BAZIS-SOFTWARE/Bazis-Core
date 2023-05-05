using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using ModelController.ModelScenePresentator;
using ModelController.ModelScenePresentator.GlObjsArraysCreators;
using ModelParcer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestModel
{
    [TestClass]
    public class TestEdgeFlags
    {
        [TestMethod]
        public void TestEdgesGMSHData()
        {;
            var loader = new LoadModelFromGMSHTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            var model = loader.Load(@"C:\Users\kns\source\repos\ModelSolution\box1.inp");
            var presentator = new ModelScenePresentator(model);
        }
        [TestMethod]
        public void TestSurfaces()
        {
            var loader = new LoadModelFromGMSHTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            var model = loader.Load(@"C:\Users\kns\source\repos\ModelSolution\box1.inp");
            var presentator = new ModelScenePresentator(model);
            var values = presentator.CreateVBOIndexes("Элементы3D");
            var edges = presentator.CreateVBOEdges("Элементы3D",values.Item3);
        }

        [TestMethod]
        public void TestOnSortedObjects() 
        {
            var loader = new LoadModelFromGMSHTextFile();
            loader.LoadEvent += (ar1, ar2) => { };
            var model = loader.Load(@"C:\Users\kns\source\repos\ModelSolution\box1.inp");
            var presentator = new ModelScenePresentator(model);
            //var dict = presentator.GetSortedObjects(array);
        }
    }
}
