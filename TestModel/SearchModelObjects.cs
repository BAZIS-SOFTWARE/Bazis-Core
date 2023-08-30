using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Events;
using Model.Interfaces;
using Model.IO;
using Model.Utilities;

namespace TestModel
{
    [TestClass]
    public class SearchModelObjects
    {
        [TestMethod]
        public void ByNumber()
        {
            //assing
            var nodes = new Node[4]
            {
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
            };
            var objs = new ModelObject[]
            {
                new Element3D( 4,nodes, ObjKind.Tetra),
                new Element3D(5,nodes,  ObjKind.Tetra),
                new Element3D( 6,nodes, ObjKind.Tetra),
                new Element3D(7,nodes,  ObjKind.Tetra),
                new Element3D(8,nodes,  ObjKind.Tetra)
            };

            var model = new ModelData();
            model.ObjectData.AddRange(objs);
            //act
            var element = ObjectsFinder.Find(objs, 8);
            //assert
            Assert.AreEqual(8, element.Number);
        }

        [TestMethod]
        public void ByNumberFromBPFFile()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromProjectTextFile();
            loader.LoadEvent += Model_LoadEvent;
            //act
            var modelLoaded = loader.Load(@"C:\Projs\TestProjTerm\tjoint\proj_med.bpf");
            model.Load(modelLoaded);
            var obj = model.ObjectData.Find(36292);
            //assert
            Assert.AreEqual(obj.Number, 36292);
        }

        private void Model_LoadEvent(object arg1, LoaderEventArgs arg2)
        {
            //throw new System.NotImplementedException();
        }
    }
}
