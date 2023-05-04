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
            var model = loader.Load(@"C:\Users\kns\source\repos\ModelSolution\Н2.inp");
            var presentator = new ModelScenePresentator(model);
            //presentator.GetSortedObjects();

            //var data = presentator.GetSortedObjects(model.ObjectData.FindMany(ObjKind.ElementSurface).ToArray());
            //var surf = model.ObjectData.FindMany<Element3D>();
            /*foreach(var s in surf.GetSurface())
            {

            }*/
            //var test = new SurfaceObjsPresenter()
            //var List<Surface>
  
        }
    }
}
