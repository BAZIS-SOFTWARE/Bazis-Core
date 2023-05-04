using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.ModelScenePresentator;
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
            var model = loader.Load(@"C:\Users\kns\source\repos\ModelSolution\Н2.inp");
            var presentator = new ModelScenePresentator(model);
            var surf = model.ObjectData.FindMany("Элементы3D");
            foreach (var s in surf)
            { 
                //s

            }
        }
    }
}
